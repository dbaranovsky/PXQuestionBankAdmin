using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;

using Bfw.Common.Collections;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var sm = GetSessionManager();

            var cmd = new GetUsers() { SearchParameters = new UserSearch() { Id = "125" } };
            sm.CurrentSession.Execute(cmd);

            if (!cmd.Users.IsNullOrEmpty())
            {
                var sorted = cmd.Users.OrderBy(u => int.Parse(u.Id));

                foreach (var user in cmd.Users)
                {
                    Console.WriteLine("{0} {1} ({2})", user.FirstName, user.LastName, user.Id);
                }
            }

            Console.WriteLine("...");
            Console.ReadKey();
            
        }

        static ISessionManager GetSessionManager()
        {
            var sm = new Bfw.Agilix.Dlap.Components.Session.ThreadSessionManager();
            var session = sm.StartAnnonymousSession();

            if (null != session)
                sm.CurrentSession = session;

            return sm as ISessionManager;
        }

        static void PrintResponse(DlapResponse response)
        {
            Console.WriteLine(response.ResponseXml.ToString());
        }
    }
}
