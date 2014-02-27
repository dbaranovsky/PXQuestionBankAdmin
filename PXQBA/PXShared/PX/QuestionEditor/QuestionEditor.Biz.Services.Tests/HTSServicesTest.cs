using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.QuestionEditor.Biz.DataContracts;
using Bfw.PX.QuestionEditor.Biz.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Routing.Handlers;
using Question = Bfw.Agilix.DataContracts.Question;

namespace QuestionEditor.Biz.Services.Tests
{
    [TestClass]
    public class HTSServicesTest
    {
        private ISessionManager _sessionManager;
        private HTSServices _services;
        private ISession _session;

        [TestInitialize]
        public void TestInitialize()
        {
            _sessionManager = Substitute.For<ISessionManager>();
            _session = Substitute.For<ISession>();
            _sessionManager.CurrentSession.Returns(_session);
            _services = new HTSServices(_sessionManager);
        }


        [TestMethod]
        public void Verify_ScoreValues_Are_Added_To_The_QuestionElement()
        {
            var dummyQuestion = new Question{Id="dummy_questionId"};
            var htsData = GetDummyHTSData();

            var item = GetDummyItem();

            _sessionManager.CurrentSession.WhenForAnyArgs(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                Type type = s.Arg<Batch>().GetType();
                var commands = from k in (Dictionary<string, DlapCommand>)(type.GetProperty("CommandSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(s.Arg<Batch>(), null))
                               select k.Value;
                var getItems = commands.ToArray()[0] as GetItems;
                if (getItems != null)
                    getItems.Items = new List<Item> { item };
            });
            

            var quiz = _services.CreateMockQuiz(dummyQuestion, htsData);
            var questionsEl = quiz.Data.Elements("questions");
            var questionEls1 = questionsEl.Elements("question").First();
            var score = questionEls1.Attribute("score").Value;

            Assert.AreEqual("5",score);
        }

        private Item GetDummyItem()
        {
            return new Item()
            {
                Id = "1",
                Data = GetXmlData()
            };
        }

        private XElement GetXmlData()
        {
            var dataElement = new XElement("data");
            var questionsElement = new XElement("questions");
            var questionElement = new XElement("question");
            questionElement.Add(new XAttribute("id", "dummy_questionId"));
            questionsElement.Add(questionElement);
            questionElement.Add(new XAttribute("score", "5"));
            dataElement.Add((questionsElement));

            return dataElement;
        }
       

        private HTSData GetDummyHTSData()
        {
            return new HTSData
            {
                EntityId = "dummy_entityId",
                QuestionId = "dummy_questionId",
                QuizId = "dummy_quizId",
                EnrollmentId = "dummy_enrollmentId"
            };
        }
    }
}
