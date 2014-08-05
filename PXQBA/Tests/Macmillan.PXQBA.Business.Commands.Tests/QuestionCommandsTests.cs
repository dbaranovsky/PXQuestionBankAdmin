using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Question = Bfw.Agilix.DataContracts.Question;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class QuestionCommandsTests
    {
        private IContext context;
        private IProductCourseOperation productCourseOperation;
        private IUserOperation userOperation;
        private INoteCommands noteCommands;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;
        private IQuestionCommands questionCommands;
        private string productCourseId = "12";
        private string twoQuestionsSOLRSearchResultResponse = @" <results>
                                                              <result name=""response"" numFound=""2"" start=""0"" maxScore=""16.067871"" time=""8"">
                                                                <doc entityid=""39768"" class=""question"" questionid=""f13f2cd1-2ddf-430c-85c9-2577a5f009f4"">
                                                                  <str name=""dlap_id"">39768|Q|f13f2cd1-2ddf-430c-85c9-2577a5f009f4</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                </doc>
                                                                <doc entityid=""39768"" class=""question"" questionid=""4684f693-7997-4dc2-8496-56eb645e47ac"">
                                                                  <str name=""dlap_id"">39768|Q|4684f693-7997-4dc2-8496-56eb645e47ac</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                </doc>
                                                              </result>
                                                            </results>";

        private const string productCourseSection = @"<product-course-id-12>
<bank>Test Bank</bank>
<productcourseid>12</productcourseid>
</product-course-id-12>";
        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            questionCommands = new QuestionCommands(context, productCourseOperation);

            userOperation = Substitute.For<IUserOperation>();
            noteCommands = Substitute.For<INoteCommands>();
            modelProfileService = new ModelProfileService(productCourseOperation, questionCommands, userOperation, noteCommands);

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
        }


        [TestMethod]
        public void GetQuestionList_TwoUserFilters_NoQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(x => ExecuteAsAdminFillNoQuestions(((Search)x))));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             },
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Chapter,
                                 Values = new List<string>(){"Chapter 1"}
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            


            var criteria = new SortCriterion();
            var  result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 0);

        }

        [TestMethod]
        public void GetQuestionList_OneUserFilter_ListOfTwoFilteredQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             }
                         };

           
            AddNessesaryProductCourseFilter(filter);


            var criteria = new SortCriterion();
            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.ToList().Select(x => x.Id).Contains("4684f693-7997-4dc2-8496-56eb645e47ac"));
        }


        [TestMethod]
        public void GetQuestionList_NoSortingParameters_SeqSortingByDefault()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/bank";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            
            context.SessionManager.CurrentSession.When(x=>x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a=>a.SearchParameters.Fields==expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria=true);

            var filter = new List<FilterFieldDescriptor>();
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion();

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }

        [TestMethod]
        public void GetQuestionList_SortingCustomFieldDesc_ApplySortingForSearchCommand()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/CustomField";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a => a.SearchParameters.Fields == expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria = true);

            var filter = new List<FilterFieldDescriptor>();
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion()
                           {
                               SortType = SortType.Desc,
                               ColumnName = "CustomField"
                           };

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }

        [TestMethod]
        public void GetQuestionList_OneQuestionPerPageAndPageNo2_ReturnSecondQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>();
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion();

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 1, 1);

            Assert.IsTrue(result.TotalItems==2);
            Assert.IsTrue(result.CollectionPage.Count() == 1);
            Assert.IsTrue(result.CollectionPage.FirstOrDefault().Id == "4684f693-7997-4dc2-8496-56eb645e47ac");
        }

        [TestMethod]
        public void GetQuestionList_OneQuestionPerPageAndPageNo1_ReturnFirstQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>();
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion();

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 1);

            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.Count() == 1);
            Assert.IsTrue(result.CollectionPage.FirstOrDefault().Id == "f13f2cd1-2ddf-430c-85c9-2577a5f009f4");
        }


        #region private methods

        private void ExecuteAsAdminFillTwoQuestions(Search search)
        {
            search.SearchResults = XDocument.Parse(twoQuestionsSOLRSearchResultResponse);
        }

        private void ExecuteAsAdminFillNoQuestions(Search search)
        {
            search.SearchResults = new XDocument();

        }

        private void ExecuteAsAdminGetAgilixQuestions(GetQuestions questionSearch)
        {
            var questions = new List<Question>();
            foreach (var quesionId in questionSearch.SearchParameters.QuestionIds)
            {
                questions.Add(new Question()
                              {
                                  Id = quesionId,
                                  MetadataElements = new Dictionary<string, XElement> { { "<product-course-id-12>", XElement.Parse(productCourseSection) } }, 
                              });
            }

            questionSearch.Questions = questions;
        }

        private void AddNessesaryProductCourseFilter(  List<FilterFieldDescriptor> filter)
        {
            filter.Add(new FilterFieldDescriptor()
            {
                Field = MetadataFieldNames.ProductCourse,
                Values = new List<string>() { productCourseId }
            });
            
        }

        private void ExecuteAsAdminGetTwoAgilixQuestions(GetQuestions questionSearch)
        {
            questionSearch.Questions = new List<Question>()
                                       {
                                           new Question()
                                           {
                                               Id = "f13f2cd1-2ddf-430c-85c9-2577a5f009f4",
                                               MetadataElements = new Dictionary<string, XElement>{{"<product-course-id-12>", XElement.Parse(productCourseSection)}},

                                           },
                                           new Question(){
                                               Id = "4684f693-7997-4dc2-8496-56eb645e47ac",
                                               MetadataElements = new Dictionary<string, XElement>{{"<product-course-id-12>", XElement.Parse(productCourseSection)}},
                                           }
                                       };
        }

        #endregion
    }
}
