using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using QuestionChoice = Bfw.Agilix.DataContracts.QuestionChoice;

namespace Bfw.PX.Biz.Direct.Services.Tests
{

    [TestClass]
    public class QuestionActionsTest
    {
        private IBusinessContext _context;
        private ISessionManager _sessionManager;
        private ISession _session;
        private QuestionActions _questionActions;
        private IContentActions _contentActions;
        private ICacheProvider _cacheProvider;
        private IItemQueryActions _itemQueryActions;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _sessionManager = Substitute.For<ISessionManager>();
            _session = Substitute.For<ISession>();
            _sessionManager.CurrentSession.Returns(_session);
            _contentActions = Substitute.For<IContentActions>();
            _cacheProvider = Substitute.For<ICacheProvider>();
            _itemQueryActions = Substitute.For<IItemQueryActions>();
            _context.CacheProvider.Returns(_cacheProvider);

            var serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            _questionActions = new QuestionActions(_context, _sessionManager, _contentActions, _cacheProvider, _itemQueryActions);
        }

        [TestCategory("QuestionActions"), TestMethod]
        public void Verify_Preview_Of_Blank_Advance_Question_Should_Show_Correct_Message()
        {
            const string response = @"<custom>
                              <version>1</version>
                              <score>http://dev.px.bfwpub.com/PxHTS/PxScore.ashx</score>
                              <display>Active,Review,Print,PrintKey</display>
                              <privatedata>26a01f1d-6ac5-4aa3-848e-e524556d86b9</privatedata>
                              <body><![CDATA[<div>The question has no content.</div>]]></body>
                            </custom>";
            WebRequest.RegisterPrefix("test", new TestWebRequestCreate());
            TestWebRequest request = TestWebRequestCreate.CreateTestRequest(response);

            const string questionId = "dummy_question_id";
            const string sUrl = @"test://DummyUrl";
            var xMl = string.Empty;

            var questionresponse = _questionActions.GetHTSQuestionPreview(sUrl,xMl,questionId);

            Assert.AreEqual(true, questionresponse.Contains("The question does not contain any data."), "Showing wrong content of blank questions.");
        }



        /// <summary>
        ///EditQuestionList can set question pool points
        ///</summary>
        [TestCategory("QuestionActions"), TestMethod]
        public void EditQuestionListTest_SetPoolPoints()
        {
            string entityId = "entityId"; 
            string parentId = "parentId"; 
            string questionId = "question2"; 
            string poolCount = "20"; 
            Nullable<int> points = 5; 
            var question2Elem = EditQuestionListHelper_GetQuestionsElem(parentId, entityId, questionId, poolCount, points);
            var question2Points = int.Parse(question2Elem.Attribute("score").Value);
            Assert.AreEqual(question2Points, points.Value);
        }


        ///<summary>
        ///GetQuestions Should Not Load QuestionPool By Default
        ///</summary>
        [TestCategory("QuestionActions"), TestMethod]
        public void GetQuestions_Should_Not_Load_QuestionPool_By_Default()
        {

            const string entityId = "11111";
            const bool ignoreBanks = true;
            var quizItem = new ContentItem
            {
                Id = "Quiz1",
                ActualEntityid = entityId,
                QuizQuestions = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        EntityId = "22222",
                        Count = "2",
                        QuestionId = "Pool1",
                        Type = "2"
                    },
                    new QuizQuestion
                    {
                        EntityId = "33333",
                        Count = "1",
                        QuestionId = "Pool2",
                        Type = "2"
                    }
                }
            };

            GetQuestions_Substitutions(entityId);

            var questionResultSet = _questionActions.GetQuestions(entityId, quizItem, ignoreBanks, entityId, null, null);
            
            Assert.AreEqual(2, questionResultSet.Questions.Count);
            Assert.IsNull(questionResultSet.Questions[0].Questions);
            Assert.IsNull(questionResultSet.Questions[1].Questions);

        }

        private void GetQuestions_Substitutions(string entityId)
        {
            var quizPool1Item = new ContentItem
            {
                Id = "Pool1",
                ActualEntityid = "22222",
                QuizQuestions = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        QuestionId = "Pool1Question1",
                        Type = "1",
                        EntityId = "22222"
                    },
                    new QuizQuestion
                    {
                        QuestionId = "Pool1Question2",
                        Type = "1",
                        EntityId = "22222"
                    }
                }
            };

            var quizPool2Item = new ContentItem
            {
                Id = "Pool2",
                ActualEntityid = "33333",
                QuizQuestions = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        QuestionId = "Pool2Question1",
                        Type = "1",
                        EntityId = "33333"
                    }
                }
            };

            _context.EntityId.Returns(entityId);
            _context.CurrentUser = new UserInfo { Id = "99999" };

            var newSession = Substitute.For<ISession>();
            newSession.When(s => s.ExecuteAsAdmin(Arg.Any<GetQuestions>()))
                .Do(s =>
                {
                    var questionCmd = s.Arg<GetQuestions>();
                    if (questionCmd.SearchParameters.EntityId == "22222")
                    {
                        questionCmd.Questions = new List<Agilix.DataContracts.Question>()
                        {
                            new Agilix.DataContracts.Question()
                            {
                                Id = "Pool1Question1",
                                Title = "Pool1Question1 Title",
                                InteractionType = "choice",
                                Choices = new List<QuestionChoice>()
                            },
                            new Agilix.DataContracts.Question()
                            {
                                Id = "Pool1Question2",
                                Title = "Pool1Question2 Title",
                                InteractionType = "choice",
                                Choices = new List<QuestionChoice>()
                            }
                        };
                    }
                    else if (questionCmd.SearchParameters.EntityId == "33333")
                    {
                        questionCmd.Questions = new List<Agilix.DataContracts.Question>()
                        {
                            new Agilix.DataContracts.Question()
                            {
                                Id = "Pool2Question1",
                                Title = "Pool2Question1 Title",
                                InteractionType = "choice",
                                Choices = new List<QuestionChoice>()
                            }
                        };
                    }
                    else
                    {
                        questionCmd.Questions = new List<Agilix.DataContracts.Question>();
                    }
                });

            _sessionManager.StartNewSession(null, null, false, _context.CurrentUser.Id).ReturnsForAnyArgs(newSession);

            _contentActions.GetContent(Arg.Is<string>(entityId), Arg.Is("Pool1"), Arg.Any<bool>())
                .Returns(quizPool1Item);
            _contentActions.GetContent(Arg.Is<string>(entityId), Arg.Is("Pool2"), Arg.Any<bool>())
                .Returns(quizPool2Item);
        }

        ///<summary>
        ///GetQuestions Should Load QuestionPool If IgnoreBanks Is False
        ///</summary>
        [TestCategory("QuestionActions"), TestMethod]
        public void GetQuestions_Should_Load_QuestionPool_If_IgnoreBanks_Is_False()
        {

            const string entityId = "11111";
            const bool ignoreBanks = false;
            var quizItem = new ContentItem
            {
                Id = "Quiz1",
                ActualEntityid = entityId,
                QuizQuestions = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        EntityId = "22222",
                        Count = "2",
                        QuestionId = "Pool1",
                        Type = "2"
                    },
                    new QuizQuestion
                    {
                        EntityId = "33333",
                        Count = "1",
                        QuestionId = "Pool2",
                        Type = "2"
                    }
                }
            };

            GetQuestions_Substitutions(entityId);

            var questionResultSet = _questionActions.GetQuestions(entityId, quizItem, ignoreBanks, entityId, null, null);

            Assert.AreEqual(2, questionResultSet.Questions.Count);
            Assert.AreEqual(2, questionResultSet.Questions[0].Questions.Count);
            Assert.AreEqual(1, questionResultSet.Questions[1].Questions.Count);

        }

        /// <summary>
        ///EditQuestionList can set question pool count
        ///</summary>
        [TestCategory("QuestionActions"), TestMethod]
        public void EditQuestionListTest_SetPoolCount()
        {
            string entityId = "entityId"; 
            string parentId = "parentId"; 
            string questionId = "question2"; 
            string poolCount = "20"; 
            Nullable<int> points = 5; 
            var question2Elem = EditQuestionListHelper_GetQuestionsElem(parentId, entityId, questionId, poolCount, points);
            var question2Count = question2Elem.Attribute("count").Value;
            Assert.AreEqual(question2Count, poolCount);
        }

        [TestMethod]
        public void GetCourseChapters_Should_Return_List_Of_Chapters()
        {
            var item = new DataContracts.ContentItem()
            {
                Id = "12345",
                Type = "PxUnit"
            };
            _context.CurrentUser = new UserInfo() { Id = "userId" };
            _itemQueryActions.BuildListChildrenQuery("entityId", "", 1, "", _context.CurrentUser.Id).ReturnsForAnyArgs(new Agilix.DataContracts.ItemSearch());
            _contentActions.FindContentItems(new Agilix.DataContracts.ItemSearch()).ReturnsForAnyArgs(new List<DataContracts.ContentItem>() 
            { 
                item
            });

            var result = _questionActions.GetCourseChapters("entityId");

            Assert.AreEqual(item, result.First());
        }

        [TestMethod]
        public void GetQuestionRepositoryCourse_Should_Return_Value_From_Cache()
        {
            _cacheProvider.FetchCourseItem("entityId", "QUESTION_COURSE").Returns("questionCourseId");

            var result = _questionActions.GetQuestionRepositoryCourse("entityId");

            Assert.AreEqual("questionCourseId", result);
        }

        [TestMethod]
        public void GetQuestionRepositoryCourse_Should_Return_Value_From_Course()
        {
            _cacheProvider.FetchCourseItem("entityId", "QUESTION_COURSE").Returns(null);
            _context.Course = new Course() { QuestionBankRepositoryCourse = "questionCourseId" };

            var result = _questionActions.GetQuestionRepositoryCourse("entityId");

            Assert.AreEqual("questionCourseId", result);
        }

        [TestMethod]
        public void GetQuestionRepositoryCourse_Should_Return_Value_From_Quizzes()
        {
            _cacheProvider.FetchCourseItem("entityId", "QUESTION_COURSE").Returns(null);
            _context.Course = new Course();
            _context.CurrentUser = new UserInfo();
            _contentActions.FindContentItems(null).ReturnsForAnyArgs(new List<ContentItem>() 
            {
                new ContentItem()
                {
                    Id = "itemId",
                    QuizQuestions = new List<QuizQuestion>()
                    {
                        new QuizQuestion()
                        {                            
                            EntityId ="questionCourseId"
                        }
                    }
                }
            });

            var result = _questionActions.GetQuestionRepositoryCourse("entityId");

            Assert.AreEqual("questionCourseId", result);
        }

        private XElement EditQuestionListHelper_GetQuestionsElem(string parentId, string entityId, string questionId,
            string poolCount, int? points)
        {
            //The mock returns the following question data:
            //<questions>
            //  <question id="question1" score="3" type="2" count="4" />
            //  <question id="question2" score="1" type="2" count="3" />
            //  <question id="question3" score="1" type="1" />
            //  <question id="question4" score="1" type="2" count="2" />
            //</questions>
            _contentActions.GetRawItem(entityId, parentId).Returns(TestHelper.Helper.GetXDocument("Quiz"));

            //_session.When(s => s.ExecuteAsAdmin(Arg.Is<GetRawItem>(x => x.ItemId == parentId && x.EntityId == entityId)))
            //    .Do(s => { s.Arg<GetRawItem>().ItemDocument = TestHelper.Helper.GetXDocument("Quiz"); });


            XDocument itemStored = null; //itemStored will contain the data stored to DLAP
            _session.When(s => s.ExecuteAsAdmin(Arg.Any<PutRawItem>())).Do(ci => itemStored = ci.Arg<PutRawItem>().ItemDoc);

            //ACT
            _questionActions.EditQuestionList(entityId, parentId, questionId, poolCount, points);


            Assert.IsNotNull(itemStored); //data has been saved
            var dataElem = itemStored.Element("item").Element("data");
            var questionsElem = dataElem.Element("questions");
            return questionsElem.Elements("question").FirstOrDefault(x => x.Attribute("id").Value == questionId);
        }
    }

    class TestWebRequestCreate : IWebRequestCreate
    {
        static WebRequest _nextRequest;
        static readonly object LockObject = new object();

        static public WebRequest NextRequest
        {
            get { return _nextRequest; }
            set
            {
                lock (LockObject)
                {
                    _nextRequest = value;
                }
            }
        }

        public WebRequest Create(Uri uri)
        {
            return _nextRequest;
        }

        /// <summary>Utility method for creating a TestWebRequest and setting
        /// it to be the next WebRequest to use.</summary>
        /// <param name="response">The response the TestWebRequest will return.</param>
        public static TestWebRequest CreateTestRequest(string response)
        {
            var request = new TestWebRequest(response);
            NextRequest = request;
            return request;
        }
    }

    class TestWebRequest : WebRequest
    {
        readonly MemoryStream _requestStream = new MemoryStream();
        readonly MemoryStream _responseStream;

        public override string Method { get; set; }
        public override string ContentType { get; set; }
        public override long ContentLength { get; set; }

        /// <summary>Initializes a new instance of <see cref="TestWebRequest"/>
        /// with the response to return.</summary>
        public TestWebRequest(string response)
        {
            _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response));
        }

        /// <summary>Returns the request contents as a string.</summary>
        public string ContentAsString()
        {
            return System.Text.Encoding.UTF8.GetString(_requestStream.ToArray());
        }

        /// <summary>See <see cref="WebRequest.GetRequestStream"/>.</summary>
        public override Stream GetRequestStream()
        {
            return _requestStream;
        }

        /// <summary>See <see cref="WebRequest.GetResponse"/>.</summary>
        public override WebResponse GetResponse()
        {
            return new TestWebReponse(_responseStream);
        }
    }

    class TestWebReponse : WebResponse
    {
        readonly Stream _responseStream;

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse(Stream responseStream)
        {
            this._responseStream = responseStream;
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream()
        {
            return _responseStream;
        }
    }
}
