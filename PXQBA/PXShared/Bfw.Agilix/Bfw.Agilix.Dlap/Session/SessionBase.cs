using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Bfw.Common.Collections;
using Bfw.Common.Logging;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Base implementation of the ISession interface to streamline development
    /// </summary>
    public abstract class SessionBase : ISession
    {
        #region Properties

        /// <summary>
        /// Connection to the DLAP server
        /// </summary>
        protected DlapConnection Connection { get; set; }

        /// <summary>
        /// Cached Admin Connection to the DLAP server
        /// </summary>
        protected DlapConnection CachedAdminConnection { get; set; }

        #endregion

        #region ISession Members

        /// <summary>
        /// Indicates if the session is anonymous
        /// </summary>
        /// <value>True if session is anonymous, false otherwise</value>
        public virtual bool IsAnnonymous { get; set; }

        /// <summary>
        /// Stores a reference to the <see cref="ILogger"/>, if any
        /// </summary>
        /// <value>Null unless set to an instance of <see cref="ILogger"/></value>
        public virtual Bfw.Common.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Stores a reference to the <see cref="ITraceManager"/>, if any
        /// </summary>
        /// <value>Null unless et to an instance of <see cref="ITraceManager"/></value>
        public virtual Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        /// <summary>
        /// Unique identifier of the user whose session this is
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// If true, then asynchronous commands will be run asynchronously.
        /// </summary>
        public virtual bool AllowAsync { get; set; }

        /// <summary>
        /// Establishes a new DlapConnection using the elevated privileges specified in the configuration file
        /// </summary>
        /// <returns>A connection to the DLAP server with elevated privileges</returns>
        protected DlapConnection AdminConnection()
        {
            DlapConnection connection = CachedAdminConnection;

            if (connection == null)
            {
                var useradminId = string.Empty;
                var useradminPwd = string.Empty;
                var defaultuseradminPrefix = string.Empty;

                var config = ConfigurationManager.GetSection("agilixSessionManager") as Bfw.Agilix.Dlap.Configuration.SessionManagerSection;
                useradminId = useradminId = config.AdminUser.Id;

                connection = ConnectionFactory.GetDlapConnection(config.Connection.Url);
                connection.Logger = Logger;
                connection.Tracer = Tracer;
                connection.TrustHeaderUsername = useradminId;
                connection.TrustHeaderKey = config.Connection.SecretKey;
                

                CachedAdminConnection = connection;
                AllowAsync = config.Connection.AllowAsync;
            }

            return connection;
        }

        /// <summary>
        /// Establishes a new <see cref="DlapConnection"/> using the elevated privileges specified in the configuration file
        /// </summary>
        /// <returns>A connection to the DLAP server with elevated privileges</returns>
        protected DlapConnection AdminConnection(String environment)
        {
            var useradminId = string.Empty;
            var useradminPwd = string.Empty;
            var defaultuseradminPrefix = string.Empty;
            DlapConnection connection = null;

            var config = ConfigurationManager.GetSection("agilixSessionManager" + environment.ToLower()) as Bfw.Agilix.Dlap.Configuration.SessionManagerSection;
            useradminId = config.AdminUser.Id;

            connection = ConnectionFactory.GetDlapConnection(config.Connection.Url);
            connection.Logger = Logger;
            connection.Tracer = Tracer;
            connection.TrustHeaderUsername = useradminId;
            connection.TrustHeaderKey = config.Connection.SecretKey;
            
            AllowAsync = config.Connection.AllowAsync;
            
            return connection;
        }

        /// <summary>
        /// Executes the command against DLAP by sending the request and parsing the response
        /// </summary>
        /// <param name="command">Command to execute against DLAP</param>
        public virtual void Execute(DlapCommand command)
        {
            using (Tracer.DoTrace("ISession.Execute"))
            {
                ExecuteCmd(Connection, command);
            }
        }

        /// <summary>
        /// Execute the command against DLAP using elevated privileges
        /// </summary>
        /// <param name="command">Command to execute against DLAP</param>
        public virtual void ExecuteAsAdmin(DlapCommand command)
        {
            using (Tracer.DoTrace("ISession.ExecuteAsAdmin"))
            {
                ExecuteCmd(AdminConnection(), command);
            }
        }

        /// <summary>
        /// Executes the command against DLAP using elevated privilegs for the specified environment
        /// </summary>
        /// <param name="environment">Environment to access</param>
        /// <param name="command">Command to execute against DLAP</param>
        public virtual void ExecuteAsAdmin(String environment, DlapCommand command)
        {
            using (Tracer.DoTrace("ISession.ExecuteAsAdmin(environment={0})", environment))
            {
                ExecuteCmd(AdminConnection(environment), command);
            }
        }

        /// <summary>
        /// Executes a DLAP command against a specific DLAP connection. This connection can be either the
        /// user's session connection, or a connection with elevated privileges
        /// </summary>
        /// <param name="connection">Connection to a DLAP server</param>
        /// <param name="command">Command to execute against DLAP</param>
        protected virtual void ExecuteCmd(DlapConnection connection, DlapCommand command)
        {
            DlapRequest request = null;
            DlapResponse response = null;

            if (command.RunAsync && AllowAsync)
            {
                ExecuteCmdAsync(connection, command);
            }
            else
            {
                using (Tracer.DoTrace("ExecuteCmd"))
                {
                    using (Tracer.DoTrace("Build Request"))
                    {
                        request = command.ToRequest();
                    }

                    using (Tracer.DoTrace("Send Request"))
                    {
                        response = Send(connection, request);
                    }

                    using (Tracer.DoTrace("Parse Response"))
                    {
                        command.ParseResponse(response);
#if DEBUG
                        if (response.Code != DlapResponseCode.OK && response.Code != DlapResponseCode.None)
                        {
                            throw new BadDlapResponseException(response.Message);
                        }
#endif
                    }
                }
            }
        }

        protected virtual void ExecuteCmdAsync(DlapConnection connection, DlapCommand command)
        {
            var requests = command.ToRequestAsync();
            var tasks = new List<Task<DlapResponse>>();

            foreach (var request in requests)
            {
                tasks.Add(SendAsync(connection, request));
            }

            Task.WaitAll(tasks.ToArray());

            command.ParseResponseAsync(tasks.Map(task => task.Result));
        }

        protected virtual Task<DlapResponse> SendAsync(DlapConnection connection, DlapRequest request)
        {
            var conn = connection.Clone() as DlapConnection;
            var task = Task.Factory.StartNew<DlapResponse>(() =>
            {
                DlapResponse response = null;
                conn.Tracer = null;
                conn.Logger = null;
                response = conn.Send(request);

                return response;
            });

            return task;
        }

        /// <summary>
        /// Sends a request to the currently configured DLAP server (<see cref="Connection" />)
        /// </summary>
        /// <typeparam name="TRequest">Type that will generate the DLAP request</typeparam>
        /// <typeparam name="TResponse">Type that will parse the DLAP response</typeparam>
        /// <param name="tRequest">Request to send to DLAP</param>
        /// <returns>Response returned by DLAP, and parsed by the TResponse type</returns>
        public virtual TResponse Send<TRequest, TResponse>(TRequest tRequest)
            where TRequest : IDlapRequestTransformer, new()
            where TResponse : IDlapResponseParser, new()
        {
            var tResponse = new TResponse();

            using (Tracer.DoTrace("ISession.Send(generic)"))
            {
                var request = tRequest.ToRequest();
                var response = Send(request);

                tResponse.ParseResponse(response);
            }

            return tResponse;
        }

        /// <summary>
        /// Sends a request to DLAP and returns the response
        /// </summary>
        /// <param name="request">Request to send to DLAP</param>
        /// <returns>Response returned by DLAP after executing the request</returns>
        public virtual DlapResponse Send(DlapRequest request, bool asAdmin = false, string environment = "")
        {
            DlapResponse response = null;

            using (Tracer.DoTrace("ISession.Send"))
            {
                if (!asAdmin)
                {
                    response = Send(Connection, request);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(environment))
                    {
                        response = Send(AdminConnection(environment), request);
                    }
                    else
                    {
                        response = Send(AdminConnection(), request);
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Sends a request to the specified DLAP server
        /// </summary>
        /// <param name="connection">Connection to a DLAP server</param>
        /// <param name="request">Request being sent to the connection</param>
        /// <returns>Response returned by DLAP after executing the request</returns>
        protected virtual DlapResponse Send(DlapConnection connection, DlapRequest request)
        {
            DlapResponse response = null;
            Connection.Tracer = Tracer;
            Connection.Logger = Logger;

            using (Tracer.DoTrace("ISession.Send(connection)"))
            {
                response = connection.Send(request);
            }

            return response;
        }

        #endregion
    }
}
