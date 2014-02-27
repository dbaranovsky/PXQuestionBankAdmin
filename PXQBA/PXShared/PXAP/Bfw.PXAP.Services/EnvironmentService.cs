using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml.Linq;

using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Dapper;
using Microsoft.ApplicationServer.Caching;
using System.Web;
using Bfw.PXAP.Components;

namespace Bfw.PXAP.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private static Dictionary<EnvironmentType, List<string>> environmentVariables = new Dictionary<EnvironmentType, List<string>>
        {
            { EnvironmentType.DEV, new List<string> { "vspxafabdev01.web.hbpub.net" } },
            { EnvironmentType.QA, new List<string> { "hbpxafabqa01.hbpna.com", "hbpxafabqa02.hbpna.com" } },
            { EnvironmentType.PR, new List<string> { "vspxafabpr01.web.hbpub.net", "vspxafabpr02.web.hbpub.net" } },
            { EnvironmentType.PROD, new List<string> { "hbpxafab01.hbpna.com", "hbpxafab02.hbpna.com", "hbpxafab03.hbpna.com" } },
            { EnvironmentType.LOADTESTING, new List<string> { "NY1MHEAFABLT01.tier3.macmillanusa.com" } }
        };

        private static string StoredDataCacheFactoryEnvironment;

        /// <summary>
        /// Add / Update environment in the datbase
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public int AddUpdateEnvironment(PXEnvironment env, out string message)
        {
            
            XmlSerializer serializer = new XmlSerializer(typeof(PXEnvironment));
            XDocument xDoc = new XDocument();
            var writer = xDoc.CreateWriter();
            var xmlNamespace = new XmlSerializerNamespaces();
            xmlNamespace.Add(string.Empty, string.Empty);
            serializer.Serialize(writer, env, xmlNamespace);
            writer.Close();

            string xmlEnv = xDoc.ToString();    

            int newEnvId = env.EnvironmentId;
            IDbConnection conn = this.GetDbConnection();

            try
            {               
                var res = conn.Query("sp_AddUpdateEnvironment @envXml", new { @envXml = xmlEnv }).FirstOrDefault();
                newEnvId = res.EnvironmentId;
                message = res.Message;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                newEnvId =  - 1;              
            }
            finally
            {
                conn.Close();
            }

            return newEnvId;
        }

        public bool DeleteEnvironment(int envId)
        {
            if (envId < 0)
            {
                return true;
            }

            IDbConnection conn = this.GetDbConnection();
            string sql = @" sp_DeleteEnvironment @EnvironmentId";
            try
            {
                int deleteCount = conn.Execute(sql, new { EnvironmentId = envId });
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

       /// <summary>
       /// Queries database to get list of all environments
       /// </summary>
       /// <returns></returns>
        public List<PXEnvironment> GetEnvironments()          
        {
            IDbConnection conn = this.GetDbConnection();
            List<PXEnvironment> pxEnvironments = null;

            try
            {
                string sql = "select * from Environment select EnvironmentId, Source from EnvironmentSources";

                var reader = conn.QueryMultiple(sql);
                pxEnvironments = reader.Read<PXEnvironment>().ToList();

                var pxEnvSources = reader.Read<EnvSources>().ToList();

                foreach (var env in pxEnvironments)
                {
                    var sources = (from envSource in pxEnvSources
                                   where envSource.EnvironmentId == env.EnvironmentId
                                   select envSource.Source).ToList();

                    env.Sources = sources;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return pxEnvironments;
        }

        public void ChangeCachingConfiguration(string environment)
        {
            EnvironmentType type;
            if (Enum.TryParse(environment, out type))
            {
                if (!String.IsNullOrEmpty(environment) && StoredDataCacheFactoryEnvironment != environment)
                {
                    var configuration = new DataCacheFactoryConfiguration();
                    configuration.ChannelOpenTimeout = new TimeSpan(0);
                    configuration.MaxConnectionsToServer = 10;
                    var endpoints = environmentVariables[type];
                    var dataCacheEndpoints = endpoints.Select(e => new DataCacheServerEndpoint(e, 22233)).ToList();
                    configuration.Servers = dataCacheEndpoints;
                    var factory = new DataCacheFactory(configuration);
                    var factoryName = ConfigurationManager.AppSettings["DataCacheFactoryName"];
                    HttpContext.Current.Application[factoryName] = factory;
                    StoredDataCacheFactoryEnvironment = environment;
                }
            }
        }

        /// <summary>
        /// Creates a db connection, opens it and returns it. 
        /// </summary>
        /// <returns></returns>
        private IDbConnection GetDbConnection()
        {
            string connString = ConfigurationManager.ConnectionStrings["PXAP"].ConnectionString;
            IDbConnection conn = new SqlConnection(connString);
            conn.Open();

            return conn;
        }
    }

    


}
