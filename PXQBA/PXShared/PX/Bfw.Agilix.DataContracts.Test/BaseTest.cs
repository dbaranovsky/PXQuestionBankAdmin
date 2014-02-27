using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap;
using Bfw.Common.Test;
using Bfw.Agilix.Dlap.Session;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Test
{
    public abstract class BaseTest
    {
        #region Properties

        private MockDlapConnection dlapConnection = null;
        /// <summary>
        /// Returns the MockDlapConnection object
        /// </summary>
        public MockDlapConnection Connection
        {
            get
            {
                dlapConnection = dlapConnection ?? new MockDlapConnection();
                return dlapConnection;
            }
            set { dlapConnection = value; }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a DlapResponseFileLocator which contains three variables.
        /// 1. Test Case that is running (Loaded using reflection)
        /// 2. Class that contains the Test Case (Loaded using reflection)
        /// 3. Key is the uniq file name id
        /// 
        /// expected file TestClassName/TestMethod/Key.xml
        /// </summary>
        /// <param name="key">Uniq file name id</param>
        /// <returns></returns>
        public DlapResponseFileLocator getFileLocator(String key)
        {
            StackTrace trace = new StackTrace();
            foreach (var frame in trace.GetFrames())
            {
                var attributes = System.Attribute.GetCustomAttributes(frame.GetMethod()).ToList() ?? new List<Attribute>();
                if (attributes.Any(x => 
                    // x is TestCaseAttribute || x is TestAttribute || // nunit
                    x is TestMethodAttribute))                      // mstest
                {
                    return new DlapResponseFileLocator()
                    {
                        TestClass = frame.GetMethod().ReflectedType.Name,
                        TestMethod = frame.GetMethod().Name,
                        Key = key
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method that combines 3 common methods when executing a command into a single method call
        /// 1. Converts command to a request
        /// 2. Executes the request (response is loaded from file with help from the responseLocator)
        /// 3. Parses Response
        /// </summary>
        /// <param name="command">Dlap Command Object</param>
        /// <param name="responseLocater"></param>
        public void ProcessCommand(DlapCommand command, DlapResponseFileLocator responseLocator, String zipFileName = null)
        {
            var request = command.ToRequest();
            var response = Connection.Send(request, responseLocator, zipFileName);
            command.ParseResponse(response);
        }

        #endregion
    }
}
