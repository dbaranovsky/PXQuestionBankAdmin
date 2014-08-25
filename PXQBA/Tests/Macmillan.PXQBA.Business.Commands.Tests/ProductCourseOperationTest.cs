using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RAg.Net.RAWS.GetCourseSiteID;
using Course = Bfw.Agilix.DataContracts.Course;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class ProductCourseOperationTest
    {
        private IContext context;
        private IDatabaseManager databaseManager;
        private IProductCourseOperation productCourseOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            context.CurrentUser = new UserInfo {Username = "3454"};

            databaseManager = Substitute.For<IDatabaseManager>();
            modelProfileService = Substitute.For<IModelProfileService>();

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();


            productCourseOperation = new ProductCourseOperation(databaseManager, context);
        }

        static void ExecuteAsAdminFillOneCourse(GetCourse getCourseCommand)
        {
            getCourseCommand.Courses = new List<Course>()
                          {
                              new Course()
                              {
                                  Id = getCourseCommand.SearchParameters.CourseId
                              }
                          };
        }

        static void ExecuteAsAdminFillGetItems(GetItems getItemsCommand)
        {
            getItemsCommand.Items = new List<Item>()
                                    {
                                        new Item()
                                        {
                                            Id = "1122",
                                        }
                                    };
        }

        [TestMethod]
        public void GetProductCourse_AnyCourseId_ReturnCourseIsNotNull()
        {
            const string courseId = "123";
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));

            var course = productCourseOperation.GetProductCourse(courseId, true);

            Assert.IsNotNull(course);
        }

        [TestMethod]
        public void GetProductCourse_SpecificCourseId_CorrenctInvokeGetCourseCommand()
        {
            const string courseId = "123";
            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(Arg.Is<GetCourse>(a => a.SearchParameters.CourseId == courseId)))
                                                  .Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId);

            Assert.IsTrue(invokeCount == 1);
        }

        [TestMethod]
        public void GetProductCourse_SpecificCourseIdAndRequiredQuestionBankRepositoryFlagOn_CorrenctInvokeGetItems()
        {
            const string courseId = "123";
            const string queryForGetItems = @"/bfw_meta/bfw_metadata@name=""AgilixDisciplineId""";

            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(x => ExecuteAsAdminFillGetItems((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                        Arg.Is<Batch>(a => (
                                                             ((GetItems)a.Commands.ToList()[0]).SearchParameters.EntityId == courseId
                                                           )
                                                           &&
                                                           (
                                                            ((GetItems)a.Commands.ToList()[0]).SearchParameters.Query == queryForGetItems
                                                           )
                                                           ))).Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId, true);

            Assert.IsTrue(invokeCount == 1);
        }

        [TestMethod]
        public void GetProductCourse_AnyCourseIdAndRequiredQuestionBankRepositoryFlagOff_GetItemsNotInvoked()
        {
            const string courseId = "123";
 
            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(x => ExecuteAsAdminFillGetItems((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                        Arg.Any<Batch>())).Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId, false);

            Assert.IsTrue(invokeCount == 0);
        }

        [TestMethod]
        public void GetUserAvailableCourses_WithoutRequiredBankRepo_2CoursesWithoutBanks()
        {
            
            
            var databaseRecord1 = new DatabaseRecord();
            databaseRecord1["CourseId"] = "72378";

            var databaseRecord2 = new DatabaseRecord();
            databaseRecord2["CourseId"] = "216781";

            databaseManager.Query(Arg.Do<DbCommand>(x =>
                                                    {
                                                        Assert.IsTrue(x.CommandText == "dbo.GetQBAUserCourses");
                                                        Assert.IsTrue(x.Parameters["@userId"].Value == context.CurrentUser.Username);
                                                    }))
                                .Returns(new List<DatabaseRecord>() { databaseRecord1, databaseRecord2 });

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Batch>(ExecuteAsAdminBatch));

            var courses = productCourseOperation.GetUserAvailableCourses(false);
            Assert.IsTrue(courses.Count() == 2);
            Assert.IsTrue(courses.Any(x=> x.ProductCourseId == "72378"));
            Assert.IsTrue(courses.Any(x => x.ProductCourseId == "216781"));

        }


        [TestMethod]
        public void GetUserAvailableCourses_WithRequiredBankRepo_2CoursesWithBanks()
        {
            
            
            var databaseRecord1 = new DatabaseRecord();
            databaseRecord1["CourseId"] = "72378";

            var databaseRecord2 = new DatabaseRecord();
            databaseRecord2["CourseId"] = "216781";

            databaseManager.Query(Arg.Do<DbCommand>(x =>
                                                    {
                                                        Assert.IsTrue(x.CommandText == "dbo.GetQBAUserCourses");
                                                        Assert.IsTrue(x.Parameters["@userId"].Value == context.CurrentUser.Username);
                                                    }))
                                .Returns(new List<DatabaseRecord>() { databaseRecord1, databaseRecord2 });

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Batch>(ExecuteAsAdminBatch));

      
            
            var courses = productCourseOperation.GetUserAvailableCourses(true);
            Assert.IsTrue(courses.Count() == 2);
            Assert.IsTrue(courses.Any(x => x.ProductCourseId == "72378"));
            Assert.IsTrue(courses.Any(x => x.ProductCourseId == "216781"));
            Assert.IsTrue(courses.Count(x=> x.QuestionRepositoryCourseId == "12345") == 2);

        }

        [TestMethod]
        public void GetAllCourses_NoParams_2AvailibleCourses()
        {
            var databaseRecord1 = new DatabaseRecord();
            databaseRecord1["CourseId"] = "72378";

            var databaseRecord2 = new DatabaseRecord();
            databaseRecord2["CourseId"] = "216781";

            databaseManager.Query(Arg.Do<DbCommand>(x => Assert.IsTrue(x.CommandText == "dbo.GetQBACourses")))
                                .Returns(new List<DatabaseRecord>() { databaseRecord1, databaseRecord2 });

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Batch>(ExecuteAsAdminBatch));



            var courses = productCourseOperation.GetAllCourses();
            Assert.IsTrue(courses.Count() == 2);
            Assert.IsTrue(courses.Any(x => x.ProductCourseId == "72378"));
            Assert.IsTrue(courses.Any(x => x.ProductCourseId == "216781"));
        }

         [TestMethod]
        public void CreateDraftCourse_NewCourseTitle_DraftCourse()
        {
            var title = "title";
            var agilixCourse = new Course()
                               {
                                   Id = "3241",
                                   Title = title
                               };
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(cmd =>
                                                                                   {
                                                                                       cmd.Courses = new List<Course>()
                                                                                                     {
                                                                                                       agilixCourse
                                                                                                     };
                                                                                   }));

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<CreateCourses>(cmd =>
                                                                                       {
                                                                                           Assert.IsTrue(cmd.Courses.First().Domain.Id == "6650");
                                                                                           cmd.ParseResponse(new DlapResponse(XDocument.Parse(createCourseResponse)));
                                                                                       }));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Batch>(ExecuteAsAdminBatch));

            var createdCourse = productCourseOperation.CreateDraftCourse(title);
            Assert.IsTrue(createdCourse.IsDraft);
            Assert.IsTrue(createdCourse.Title == title);
            Assert.IsTrue(createdCourse.QuestionRepositoryCourseId == createdCourse.ProductCourseId);
        }


        [TestMethod]
        public void AddSiteBuilderCourseToQBA_UrlWithNotExistingSiteInfo_Null()
        {        
            Assert.IsTrue(productCourseOperation.AddSiteBuilderCourseToQBA("url") == null); 
        }

        [TestMethod]
        public void AddSiteBuilderCourseToQBA_UrlWithExistingSiteInfo_AgilixSiteID()
        {
            context.GetSiteInfo(Arg.Any<string>()).Returns(new SiteInfo()
            {
                AgilixSiteID = "21"
            });
            Assert.IsTrue(productCourseOperation.AddSiteBuilderCourseToQBA("url") == "21"); 
        }

        [TestMethod]
        public void UpdateCourse_CourseToUpdate_SuccessUpdate()
        {
            var course = new Models.Course()
                         {
                             ProductCourseId = "134",
                             Title = "erip"
                         };

            var agilixCourse = new Course()
            {
                Id = "3241",
                Title = "hobbit"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(cmd =>
                                                                                   {
                                                                                       cmd.Courses = new List<Course>
                                                                                                     {
                                                                                                         agilixCourse
                                                                                                     };
                                                                                   }));
            var result = productCourseOperation.UpdateCourse(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<UpdateCourses>(cmd=> Assert.IsTrue(cmd.Courses.First().Title == "erip")));
            Assert.IsTrue(result.Title == "erip");

        }

        [TestMethod]
        public void UpdateCourse_NotExistingCourseToUpdate_SuccessRun()
        {
            var course = new Models.Course()
            {
                ProductCourseId = "134",
                Title = "erip"
            };


            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(cmd =>
                                                                                   {
                                                                                       cmd.Courses = new List<Course>
                                                                                                     {
                                                                                                         null
                                                                                                     };
                                                                                   }));
            var result = productCourseOperation.UpdateCourse(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<UpdateCourses>(cmd => Assert.IsTrue(cmd.Courses.First().Title == "erip")));
            Assert.IsTrue(result.Title == "erip");

        }

        [TestMethod]
        public void RemoveResources_ResourcesToRemove_CorrectAgilixCall()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<DeleteResources>(cmd =>
                                                                                         {
                                                                                             foreach (var resource in cmd.ResourcesToDelete)
                                                                                             {
                                                                                                 Assert.IsTrue(resource.EntityId == "123");
                                                                                                 Assert.IsTrue(resource.Url == @"Assets\1.jpg" || resource.Url == @"Assets\2.jpg");
                                                                                             }
                                                                                         }));
            productCourseOperation.RemoveResources("123", new List<string>(){@"\1.jpg", @"\2.jpg"});
        }


        [TestMethod]
        public void RemoveResources_NoResourcesToRemove_SuccessRun()
        {
            productCourseOperation.RemoveResources(string.Empty, new List<string>());
        }


        [TestMethod]
        public void PutResources_ResourcesToRemove_CorrectAgilixCall()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Batch>(cmd =>
                                                                               {
                                                                                   Assert.IsTrue(cmd.RunAsync);


                                                                                   for (int index = 0;
                                                                                       index < cmd.Commands.Count();
                                                                                       index++)
                                                                                   {
                                                                                       Assert.IsTrue(cmd.CommandAs<PutResource>(index).Resource.Url == "url1" ||
                                                                                                     cmd.CommandAs<PutResource>(index).Resource.Url == "url2");
                                                                                       Assert.IsTrue(cmd.CommandAs<PutResource>(index).Resource.EntityId == "123");
                                                                                    
                                                                                   }
                                                                               }));
            productCourseOperation.PutResources(new List<Resource>()
                                                {
                                                    new Resource()
                                                    {
                                                        Url = "url1",
                                                        EntityId = "123"
                                                    },
                                                      new Resource()
                                                    {
                                                        Url = "url2",
                                                        EntityId = "123"
                                                    }
                                                });

        }

        [TestMethod]
        public void PutResources_NoResourcesToRemove_SuccessRun()
        {
            productCourseOperation.PutResources(new List<Resource>());
        }

        private void ExecuteAsAdminBatch(Batch cmd)
        {
            Assert.IsTrue(cmd.RunAsync);

            if (cmd.CommandAs<GetCourse>(0) != null)
            {
                for (var index = 0; index < cmd.Commands.Count(); index++)
                {
                    cmd.CommandAs<GetCourse>(index).Courses = new List<Course>
                                                              {
                                                                  new Course
                                                                  {
                                                                      Id = cmd.CommandAs<GetCourse>(index).SearchParameters.CourseId
                                                                  },
                                                              
                                                              };
                }
            }
            else
            {
                for (var index = 0; index < cmd.Commands.Count(); index++)
                {
                    Assert.IsTrue(cmd.CommandAs<GetItems>(index).SearchParameters.Query ==
                                  @"/bfw_meta/bfw_metadata@name=""AgilixDisciplineId""");
                    cmd.CommandAs<GetItems>(index).Items = new List<Item>
                                                           {
                                                               new Item
                                                               {
                                                                   QuestionRepositoryCourseId = "12345"
                                                               }
                                                           };
                }
            }
        }

        private const string createCourseResponse = @"<responses code=""OK""><responses>
                                                  <response code=""OK"">
                                                 
                                                      <course courseid=""4564""  />
                                                 
                                                  </response>
                                             
                                               </responses></responses>";
   }
}
