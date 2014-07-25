using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.JqGridHelper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class ProductCourseOperation : IProductCourseOperation
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IContext businessContext;

        private const string DbColumnProductCourseId = "CourseId";
        private const string DraftCourseDomain = "6650";

        public ProductCourseOperation(IDatabaseManager databaseManager, IContext businessContext)
        {
#if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
#endif
            this.databaseManager = databaseManager;
            this.businessContext = businessContext;
        }

        public IEnumerable<Course> GetUserAvailableCourses(bool requiredQuestionBankRepository=false)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBAUserCourses";
            var userIdParam = new SqlParameter("@userId", businessContext.CurrentUser.Username);
            command.Parameters.Add(userIdParam);
            var dbRecords = databaseManager.Query(command);

            var courseIds = dbRecords.Select(record => record.String(DbColumnProductCourseId)).ToList();
            return GetCoursesByCourseIds(courseIds, requiredQuestionBankRepository);
        }

        public IEnumerable<Course> GetAllCourses()
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBACourses";
            var dbRecords = databaseManager.Query(command);

            var courseIds = dbRecords.Select(record => record.String(DbColumnProductCourseId)).ToList();
            return GetCoursesByCourseIds(courseIds);
        }

        public Course CreateDraftCourse(string title)
        {
            var cmd = new CreateCourses();
            cmd.Courses.Add(new Bfw.Agilix.DataContracts.Course { Title = title, Domain =  new Domain(){Id = DraftCourseDomain}});
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            var createdCourse = GetAgilixCourse(cmd.Entity.FirstOrDefault().Id);
            createdCourse.QuestionBankRepositoryCourse = createdCourse.Id;
            createdCourse.IsDraft = true;
          

            ExecuteUpdateCourse(createdCourse);
            AddToAvailableCourses(createdCourse.Id);

            return Mapper.Map<Course>(createdCourse);
        }

        public string AddSiteBuilderCourseToQBA(string url)
        {
            var siteInfo = businessContext.GetSiteInfo(url);
            if (siteInfo != null && !string.IsNullOrEmpty(siteInfo.AgilixSiteID))
            {
                AddToAvailableCourses(siteInfo.AgilixSiteID);
                return siteInfo.AgilixSiteID;
            }
            return null;
        }


        private void AddToAvailableCourses(string id)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.AddQBACourse";
            var courseIdParam = new SqlParameter("@courseId", id);
            command.Parameters.Add(courseIdParam);
            databaseManager.ExecuteNonQuery(command);

        }

        public IEnumerable<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds, bool requiredQuestionBankRepository = false)
        {
            IList<Course> courses = new List<Course>();
            var batch = new Batch { RunAsync = true };

            foreach (var courseId in courseIds)
            {
                batch.Add(new GetCourse()
                {
                    SearchParameters = new CourseSearch()
                    {
                        CourseId = courseId
                    }
                });
            }

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            for (int index = 0; index < batch.Commands.Count(); index++)
            {
                if (batch.CommandAs<GetCourse>(index).Courses.IsNullOrEmpty())
                {
                    continue;
                }
                var re = batch.CommandAs<GetCourse>(index).Courses.First();
                courses.Add(Mapper.Map<Course>(batch.CommandAs<GetCourse>(index).Courses.First()));
            }

            if (requiredQuestionBankRepository)
            {
                courses = GetQuestionBankRepository(courses);
            }

            return courses;
        }

        public Course UpdateCourse(Course course)
        {
            var agilixCourse = GetAgilixCourse(course.ProductCourseId);
            Mapper.Map(course, agilixCourse);
            ExecuteUpdateCourse(agilixCourse);
            return course;
        }

        private void ExecuteUpdateCourse(Bfw.Agilix.DataContracts.Course agilixCourse)
        {
            var cmd = new UpdateCourses();
            cmd.Add(agilixCourse);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

        private IList<Course> GetQuestionBankRepository(IList<Course> courses)
        {
            var results = GetQuestionBankRepositoryCourseFromItems(
                courses.Where(c => String.IsNullOrEmpty(c.QuestionRepositoryCourseId)).Select(c => c.ProductCourseId));

            foreach (var result in results)
            {
                var course = courses.SingleOrDefault(c => c.ProductCourseId == result.Key);
                course.QuestionRepositoryCourseId = result.Value;
            }

            return courses;
        }

        /// <summary>
        /// Get QuestionBankRepositoryCourse from items (property HrefDisciplineCourseId)
        /// </summary>
        /// <param name="courseIds"></param>
        /// <returns>Dictionary contains key=courseId value=questionRepositoryCourseId </returns>
        private Dictionary<string, string> GetQuestionBankRepositoryCourseFromItems(IEnumerable<string> courseIds)
        {
            var resultDictionary = new Dictionary<string, string>();

            var batch = new Batch { RunAsync = true };

            foreach (var courseId in courseIds)
            {
                batch.Add(new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = courseId,
                        Query = @"/bfw_meta/bfw_metadata@name=""AgilixDisciplineId"""
                    }
                });
            }

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(batch);

            for (int index = 0; index < batch.Commands.Count(); index++)
            {
                var command = batch.CommandAs<GetItems>(index);

                string courseId = command.SearchParameters.EntityId;
                string questionRepositoryCourseId = String.Empty;

                if (!command.Items.IsNullOrEmpty())
                {
                    var firstItem = command.Items.FirstOrDefault();
                    if (firstItem != null)
                    {
                        questionRepositoryCourseId = firstItem.QuestionRepositoryCourseId;
                    }
                }

                resultDictionary.Add(courseId, questionRepositoryCourseId);
            }

            return resultDictionary;
        }

        public Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository=false)
        {
            var agilixCourse = GetAgilixCourse(productCourseId);
           
            var course = Mapper.Map<Course>(agilixCourse);

            if ((String.IsNullOrEmpty(course.QuestionRepositoryCourseId))&&
                (requiredQuestionBankRepository))
            {
                var result = GetQuestionBankRepositoryCourseFromItems(new[] { course.ProductCourseId });
                course.QuestionRepositoryCourseId = result.GetValue(course.ProductCourseId, String.Empty);
            }

            return course;
        }

        private Bfw.Agilix.DataContracts.Course GetAgilixCourse(string productCourseId)
        {
            var cmd = new GetCourse()
            {
                SearchParameters = new CourseSearch()
                {
                    CourseId = productCourseId
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Courses.FirstOrDefault();
        }

        public void RemoveResources(string itemId, List<string> questionRelatedResources)
        {
            if (!questionRelatedResources.Any())
            {
                return;
            }

            var cmd = new DeleteResources
            {
                ResourcesToDelete =
                    questionRelatedResources.Select(relatedResource => new Resource
                    {
                        EntityId = itemId,
                        Url = ConfigurationHelper.GetBrainhoneyCourseImageFolder() +
                               relatedResource
                    }).ToList()
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

        public void PutResources(List<Resource> resources)
        {
            if (!resources.Any())
            {
                return;
            }

   
            var batch = new Batch { RunAsync = true };

            foreach (var resource in resources)
            {
                batch.Add(new PutResource()
                {
                   Resource = resource
                });
            }

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(batch);
        }
    }
}
