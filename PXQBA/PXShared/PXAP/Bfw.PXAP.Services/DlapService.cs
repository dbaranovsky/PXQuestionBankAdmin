using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.PXAP.Components;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.Common.Collections;
using System.Configuration;


namespace Bfw.PXAP.Services
{
    public class DlapService : IDlapService
    {

        private ISession Session { get; set; }
        private IApplicationContext Context { get; set; }

        public DlapService(IApplicationContext context)
        {
            Context = context;
            ConfigureServiceLocator();
            var sm = EstablishConnection();
            Session = sm.CurrentSession;
            
        }
        /// <summary>
        /// Runs a DLAP command and returns raw output
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string RunCommand(string command, ref string entityid, DlapCommandModel.HttpMethod httpMethod, string postdata = "")
        {
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            if (entityid.IsNullOrEmpty())
            {
                entityid = string.Empty;
            }
            
            if (!command.Trim().StartsWith("cmd="))
            {
                command = command.Trim().Insert(0, "cmd=");
            }
            var parameterList = command.Split('&');
            foreach (var parameter in parameterList)
            {
                var p = parameter.Split('=');
                var key = p.First();
                var val = p.Length > 1 ? p[1]:string.Empty;
                if (key == "query")
                {
                    val = parameter.Substring(parameter.IndexOf("query=") + 6);
                }
                else if (key == "entityid" || key == "courseid")
                {
                    entityid = val;
                }

                
                paramaters.Add(key,val);     
            }
            if (!paramaters.ContainsKey("entityid") && !paramaters.ContainsKey("courseid") && !entityid.IsNullOrEmpty())
            {
                paramaters.Add("entityid", entityid);
            }
            DlapRequestType type = httpMethod == DlapCommandModel.HttpMethod.POST
                                       ? DlapRequestType.Post
                                       : DlapRequestType.Get;
         
                DlapRequest request = new DlapRequest()
                    {
                        Type = type,
                        Parameters = paramaters,
                        Mode = DlapRequestMode.Single,
                        UseCompression = false
                    };
            if (!postdata.IsNullOrEmpty())
            {
                postdata = postdata.Replace("-entityid-", entityid);
                request.AppendData(XElement.Parse(postdata));
            }
            
            XElement result;
            var response = Session.Send(request, true, Context.Environment);
            var responses = response.ResponseXml.Element("responses");
            if (responses != null)
            {
                response.ResponseXml = new XDocument(responses.Elements().First());
            }
            return response.ResponseXml.ToString();
        }

        /// <summary>
        /// Converts a GET command from DLAP to equivalent post command
        /// Currently only supports "getitemslist" => "putitems"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="entityid"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public DlapCommandModel ConvertGetToPost(DlapCommandModel command, string commandName = "putitems")
        {
            command.method = DlapCommandModel.HttpMethod.POST;

            if (command.postdata.IsNullOrEmpty())
            {
                command.postdata = command.result;
            }
            XDocument doc = XDocument.Parse(command.postdata);
            if (doc == null || !doc.Elements().Any())
            {
                command.result = "Invalid Post Data";
                return command;
            }
            var requests = doc.Element("requests");
            if (requests == null)
            {//add <requests> tag
                requests = new XElement("requests");
                requests.Add(doc.Root);
                doc = new XDocument(requests);
            }
            //PUTITEMS specific XML transformations
            if (commandName.Contains("getitem"))
            {
                commandName = "putitems";
            }
            if (commandName.Contains("getcourse"))
            {
                commandName = "updatecourses";
            }
            if (commandName.Contains("getquestion"))
            {
                commandName = "putquestions";
            }
            if (commandName == "putitems")
            {
                command.command = "cmd=putitems";
                if (requests.Element("items") != null)
                {
                    //remove <items> tag
                    requests.Add(requests.Element("items").Elements());
                    requests.Element("items").Remove();
                }
                var items = requests.Elements("item");
                foreach (var item in items)
                {
                    if (item.Attributes("id").Any())
                    {
                        var id = item.Attribute("id").Value;

                        item.RemoveAttributes();
                        item.SetAttributeValue("itemid", id);
                        item.SetAttributeValue("entityid", command.entityid);
                    }
                }
            }
            else if (commandName == "updatecourses")
            {
                command.command = "cmd=updatecourses";
            }
            else if (commandName == "putquestions")
            {
                command.command = "cmd=putquestions";
                if (requests.Element("response") != null)
                {
                    //remove <items> tag
                    requests.Add(requests.Element("question").Elements());
                    requests.Element("response").Remove();
                }
                var questions = requests.Elements("question");
                foreach (var question in questions)
                {
                    if (question.Attributes("actualentityid").Any())
                    {
                        question.Attribute("actualentityid").Remove();
                        question.SetAttributeValue("entityid", command.entityid);
                    }
                }
            }
            

            //END PUTITEMS 

            command.postdata = doc.ToString();

            
            return command;


        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static ISessionManager EstablishConnection()
        {            
            var username = ConfigurationManager.AppSettings["user"];
            var userid = ConfigurationManager.AppSettings["userid"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, false, string.Empty);
            return sm;
        }
    }
}
