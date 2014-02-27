using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.DataContracts;

//This is a simple console application that refers to a static copy of the Bfw.Agilix libraries.
//The only purpose of this application is to show how simple commands are submitted to DLAP for processing.
namespace SimpleConsoleApp
{
    class Program
    {
        static ISessionManager SessionManager { get; set; }

        static void Main(string[] args)
        {
            //sets up the SessionManager property
            Configure();

            //technically the Configure method should have already logged us in, but this demonstrates how to make
            //a login call using strongly typed objects
            var user = LoginUser();
            Console.WriteLine("logged in as \"{0} {1}\" from userspace {2} with id {3} and e-mail {4}", user.FirstName, user.LastName, user.Credentials.UserSpace, user.Id, user.Email);

            var status = GetStatus();
            Console.WriteLine("\n\nServer Status:");
            PrintResponse(status);

            var domains = GetDomainList();
            Console.WriteLine("\n\nDomains:");
            PrintResponse(domains);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Gets the status of the DLAP server
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        static DlapResponse GetStatus()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getstatus" } }
            };

            return SessionManager.CurrentSession.Send(request);
        }

        /// <summary>
        /// Gets the list of all domains
        /// </summary>
        /// <returns></returns>
        static DlapResponse GetDomainList()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getdomainlist" } }
            };

            return SessionManager.CurrentSession.Send(request);
        }

        /// <summary>
        /// Demonstrates using the strongly typed DataContracts instead of the raw DlapRequest/DlapResponse classes
        /// </summary>
        /// <returns></returns>
        static AgilixUser LoginUser()
        {
            var user = ConfigurationManager.AppSettings["agilixUser"];
            var pass = ConfigurationManager.AppSettings["agilixPassword"];
            var creds = new LoginCredentials() { Username = user, Password = pass };

            return SessionManager.CurrentSession.Send<LoginCredentials, AgilixUser>(creds);
        }

        /// <summary>
        /// Sets up the session manager. This is something that should normally be done by Dependency Injection
        /// in a real application.
        /// </summary>
        static void Configure()
        {
            var user = ConfigurationManager.AppSettings["agilixUser"];
            var pass = ConfigurationManager.AppSettings["agilixPassword"];

            //session manager uses external configuration information (take a look at the App.config)
            SessionManager = new ThreadSessionManager();
            SessionManager.CurrentSession = SessionManager.StartNewSession(user, pass, string.Empty);
        }

        static void PrintResponse(DlapResponse response)
        {
            Console.WriteLine(response.ResponseXml.ToString());
        }
    }
}
