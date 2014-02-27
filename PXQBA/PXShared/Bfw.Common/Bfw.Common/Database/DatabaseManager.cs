using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.Unity;

namespace Bfw.Common.Database
{
    /// <summary>
    /// Manages the connection to a database and allows queries to be executed against it.
    /// </summary>
    public class DatabaseManager : IDisposable, IDatabaseManager
    {
        #region Constants

        /// <summary>
        /// Default provider name for when a provider isn't specified in the connection string.
        /// </summary>
        protected const string DEFAULT_PROVIDER = "System.Data.SqlClient";

        #endregion

        #region Data Members

        /// <summary>
        /// Name of the connection string in the config file to use.
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// Connection string to use when opening a new connection to the database.
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Provider name to use when opening a new connection to the database.
        /// </summary>
        protected string ProviderName { get; set; }

        /// <summary>
        /// Handle to the provider factory that can create connections to the database.
        /// </summary>
        private DbProviderFactory ProviderFactory { get; set; }

        /// <summary>
        /// <c>null</c> if no session has been started, otherwise the open connection used for all commands in this session.
        /// </summary>
        protected DbConnection SessionConnection { get; set; }

        /// <summary>
        /// Small cache of information about query results we've seen so far.
        /// </summary>
        private IDictionary<string, ReaderInfo> OrdinalCache { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// To allow instattiation by ServiceLocator
        /// </summary>
        [InjectionConstructor]
        public DatabaseManager()
        {
            OrdinalCache = new Dictionary<string, ReaderInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DatabaseManager(string connectionStringName)
        {
            ConfigureConnection(connectionStringName);

            OrdinalCache = new Dictionary<string, ReaderInfo>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseManager"/> class with the whole connection string path.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="providerName">provider name</param>
        public DatabaseManager(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
            ProviderFactory = DbProviderFactories.GetFactory(ProviderName);

            OrdinalCache = new Dictionary<string, ReaderInfo>();
        }

        #endregion

        #region Members

        /// <summary>
        /// Opens a new connection to the database.
        /// </summary>
        /// <returns>Newly opened connection to the database.</returns>
        public DbConnection OpenConnection()
        {
            var connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

            return connection;
        }

        /// <summary>
        /// Starts a session in which all command will run against the same connection.
        /// </summary>
        public void StartSession()
        {
            if (SessionConnection != null)
            {
                throw new InvalidOperationException("Can not start a session while a session is already running");
            }

            SessionConnection = OpenConnection();
        }

        /// <summary>
        /// Ends the current session, if any.
        /// </summary>
        public void EndSession()
        {
            if (SessionConnection != null && SessionConnection.IsOpen())
            {
                SessionConnection.Close();
                SessionConnection = null;
            }
        }

        /// <summary>
        /// Executes a query against the connection.
        /// </summary>
        /// <param name="connection">Connection to query against.</param>
        /// <param name="sql">SQL that forms the query.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Yield return of all records in result set as ExpandoObject instances.</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace.</exception>
        public IEnumerable<DatabaseRecord> Query(DbConnection connection, string sql, params object[] args)
        {
            var cmd = BuildCommand(sql, args);
            cmd.Connection = connection;

            foreach (var record in Query(cmd))
            {
                yield return record;
            }
        }

        /// <summary>
        /// Executes a query against a new connection.
        /// </summary>
        /// <param name="sql">SQL that forms the query.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Yield return of all records in result set as ExpandoObject instances.</returns>
        public IEnumerable<DatabaseRecord> Query(string sql, params object[] args)
        {
            foreach (var record in Query(null, sql, args))
            {
                yield return record;
            }
        }

        /// <summary>
        /// Queries for data are assumed to be read only and therefore no transaction will be started or managaged. If the
        /// command is not attached to a connection then a new one will be opened and closed by this method, otherwise the
        /// caller must manage it itself. If the method is called while a session is active, the session connection will be used
        /// if there is no other connection attached ot the command.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>An enumerable of database records matching the query.</returns>
        public IEnumerable<DatabaseRecord> Query(DbCommand cmd)
        {
            DbConnection ourConnection = null;
            DbDataReader reader = null;

            if (cmd.Connection == null && SessionConnection != null)
            {
                cmd.Connection = SessionConnection;
            }
            else if (cmd.Connection == null)
            {
                cmd.Connection = ourConnection = OpenConnection();
            }

            try
            {
                CheckCommand(cmd);

                reader = cmd.ExecuteReader();
                ReaderInfo info = null;
                if (!OrdinalCache.TryGetValue(cmd.CommandText, out info))
                {
                    var fieldCount = reader.FieldCount;
                    info = new ReaderInfo() { FieldCount = fieldCount, FieldNames = new string[fieldCount] };

                    OrdinalCache[cmd.CommandText] = info;

                    for (var field = 0; field < fieldCount; ++field)
                    {
                        info.FieldNames[field] = reader.GetName(field);
                    }
                }

                while (reader.Read())
                {
                    var record = new DatabaseRecord();
                    var values = new object[info.FieldCount];
                    reader.GetValues(values);

                    for (var field = 0; field < info.FieldCount; ++field)
                    {
                        record[info.FieldNames[field]] = values[field];
                    }

                    yield return record;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                if (ourConnection != null)
                {
                    ourConnection.Close();
                }
            }
        }

        /// <summary>
        /// Executes SQL against the connection without returning a record set.
        /// </summary>
        /// <param name="connection">Connection to execute SQL against.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Number of records modified by the query.</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace.</exception>
        public int ExecuteNonQuery(DbConnection connection, string sql, params object[] args)
        {
            var cmd = BuildCommand(sql, args);
            cmd.Connection = connection;

            return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes the command. If cmd's connection is null then a new connection is opened and closed by the method.
        /// If the command doesn't have a transaction then a new one will be started and committed/rolled back. If the 
        /// command has its own connection/transaction it must be managed by the caller. If the method is called while a session is active, 
        /// the session connection will be used if there is no other connection attached ot the command.
        /// </summary>
        /// <param name="cmd">Command to execute.</param>
        /// <returns>The result of executing the command.</returns>
        public int ExecuteNonQuery(DbCommand cmd)
        {
            int result = 0;
            DbConnection ourConnection = null;

            if (cmd.Connection == null && SessionConnection != null)
            {
                cmd.Connection = SessionConnection;
            }
            else if (cmd.Connection == null)
            {
                cmd.Connection = ourConnection = OpenConnection();
            }

            if (cmd.Transaction == null)
            {
                try
                {
                    CheckCommand(cmd);

                    cmd.Transaction = cmd.Connection.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();
                    }

                    throw ex;
                }
                finally
                {
                    if (ourConnection != null)
                    {
                        ourConnection.Close();
                    }
                }
            }
            else
            {
                CheckCommand(cmd);
                result = cmd.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Executes SQL against the connection inside of a transaction.
        /// </summary>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Number of records modified by the query.</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace.</exception>
        public int ExecuteNonQuery(string sql, params object[] args)
        {
            return ExecuteNonQuery(null, sql, args);
        }

        /// <summary>
        /// Executes a query against the connection, assuming a scalar result.
        /// </summary>
        /// <param name="connection">Connection to query against.</param>
        /// <param name="sql">SQL that forms the query.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Result of scalar query.</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace.</exception>
        public object ExecuteScalar(DbConnection connection, string sql, params object[] args)
        {
            var cmd = BuildCommand(sql, args);
            cmd.Connection = connection;

            return ExecuteScalar(cmd);
        }

        /// <summary>
        /// Executes a query against the connection in a transaction, assuming a scalar result.
        /// </summary>
        /// <param name="sql">SQL that forms the query.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>Result of scalar query.</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace</exception>
        public object ExecuteScalar(string sql, params object[] args)
        {
            return ExecuteScalar(null, sql, args);
        }

        /// <summary>
        /// Executes a scalar command against the database. If no connection is provided then a new one is opened and closed by this method.
        /// If the command is not in a transaction then a new one is started and committed/rolledback by this method.
        /// Otherwise, it is expected that the caller is managing the connection/transaction. If the method is called while a session is active,
        /// the session connection will be used if there is no other connection attached ot the command.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <returns>Scalar result of the command.</returns>
        public object ExecuteScalar(DbCommand cmd)
        {
            object result = null;
            DbConnection ourConnection = null;

            if (cmd.Connection == null && SessionConnection != null)
            {
                cmd.Connection = SessionConnection;
            }
            else if (cmd.Connection == null)
            {
                cmd.Connection = ourConnection = OpenConnection();
            }

            if (cmd.Transaction == null)
            {
                try
                {
                    CheckCommand(cmd);

                    cmd.Transaction = cmd.Connection.BeginTransaction();
                    result = cmd.ExecuteScalar();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();
                    }

                    throw ex;
                }
                finally
                {
                    if (ourConnection != null)
                    {
                        ourConnection.Close();
                    }
                }
            }
            else
            {
                CheckCommand(cmd);
                result = cmd.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// Configures all members necessary to open a new connection to the database using
        /// the given connection string name.
        /// </summary>
        /// <param name="connectionStringName">name of the connection string to use from the config file</param>
        /// <exception cref="System.ArgumentException">if connectionStringName is given and doesn't exist in the config file</exception>
        public void ConfigureConnection(string connectionStringName)
        {
            ConnectionStringSettings connectionSettings = null;

            if (string.IsNullOrEmpty(connectionStringName))
            {
                connectionSettings = ConfigurationManager.ConnectionStrings[0];
            }
            else if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
            {
                throw new ArgumentException(string.Format("ConnectionString {0} does not exist in the config file", connectionStringName), "connectionStringName");
            }
            else
            {
                connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            }

            ConnectionStringName = connectionSettings.Name;
            ConnectionString = connectionSettings.ConnectionString;

            if (string.IsNullOrEmpty(connectionSettings.ProviderName))
            {
                ProviderName = DEFAULT_PROVIDER;
            }
            else
            {
                ProviderName = connectionSettings.ProviderName;
            }

            ProviderFactory = DbProviderFactories.GetFactory(ProviderName);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks the arguments, and if they are valid, returns a new DbCommand.
        /// </summary>
        /// <param name="sql">SQL that forms the query.</param>
        /// <param name="args">Parameter values, if any.</param>
        /// <returns>DbCommand created</returns>
        /// <exception cref="System.ArgumentException">If connection is null or closed, or if SQL is null, empty, or whitespace.</exception>
        protected DbCommand BuildCommand(string sql, params object[] args)
        {
            return ProviderFactory.CreateCommand(sql, args);
        }

        /// <summary>
        /// Checks a <see cref="DbCommand"/> command for a valid connection and command text.
        /// </summary>
        /// <param name="cmd">The <see cref="DbCommand"/> to check.</param>
        protected void CheckCommand(DbCommand cmd)
        {
            if (cmd.Connection.IsNullOrClosed())
            {
                throw new ArgumentException("Connection can not be null and must be open in order to query against it", "connection");
            }

            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                throw new ArgumentException("Can not query using an empty SQL statement", "sql");
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Cleans up any open sessions.
        /// </summary>
        public void Dispose()
        {
            EndSession();
        }

        #endregion
    }

    /// <summary>
    /// Used to store information about a DbDataReader.
    /// </summary>
    internal class ReaderInfo
    {
        /// <summary>
        /// Name of every field in the reader.
        /// </summary>
        public string[] FieldNames;

        /// <summary>
        /// Number of fields in the reader.
        /// </summary>
        public int FieldCount;
    }
}