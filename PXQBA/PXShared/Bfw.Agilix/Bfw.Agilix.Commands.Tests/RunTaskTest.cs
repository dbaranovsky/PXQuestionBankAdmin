using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class RunTaskTest
    {
        private RunTask RunTask;

        [TestInitialize]
        public void TestInitialize()
        {
            this.RunTask = new RunTask();
        }
        [TestMethod]
        public void RunTask_Request_Should_Be_Initialized()
        {
            this.RunTask.SearchParameters = new TaskSearch() { 
                Command = "Test_RunTasks",
                TaskId = "long_Running_Task"
            };
            var request = this.RunTask.ToRequest();
            Assert.AreEqual("RunTask", request.Parameters["cmd"]);
            Assert.AreEqual("long_Running_Task", request.Parameters["taskid"]);
        }

        [TestMethod]
        public void RunTask_Should_Parse_Returned_Response()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.RunTask));
            this.RunTask.ParseResponse(new DlapResponse() { ResponseXml = messages });
        }
    }
}
