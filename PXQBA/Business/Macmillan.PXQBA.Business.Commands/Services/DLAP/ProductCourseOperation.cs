using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class ProductCourseOperation : IProductCourseOperation
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IContext businessContext;

        private const string DbColumnProductCourseId = "CourseId";

        public ProductCourseOperation(IDatabaseManager databaseManager, IContext businessContext)
        {
            this.databaseManager = databaseManager;
            this.businessContext = businessContext;
        }

        public IEnumerable<Course> GetAvailableCourses(bool requiredQuestionBankRepository=false)
        {
            var query = String.Format(@"
                    DECLARE @webRight int
                    SET @webRight = (SELECT TOP 1 PxWebRightId FROM PxWebRights WHERE PxWebRightType = 'QuestionBank')
                    DECLARE @userId int
                    SELECT DISTINCT
                        CourseId
                    FROM
                        PxWebUserRights
                    WHERE
                        PxWebRightId = @webRight
                        AND UserId = '{0}'", businessContext.CurrentUser.Username);
            var dbRecords = databaseManager.Query(query);

            var courseIds = dbRecords.Select(record => record.String(DbColumnProductCourseId)).ToList();
            return GetCoursesByCourseIds(courseIds, requiredQuestionBankRepository);
        }

        private IEnumerable<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds, bool requiredQuestionBankRepository)
        {
            //GetQuestions();
            //GetQuestionList("71836", null, null, 1, 50);
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
                courses.Add(Mapper.Map<Course>(batch.CommandAs<GetCourse>(index).Courses.First()));
            }

            if (requiredQuestionBankRepository)
            {
                courses = GetQuestionBankRepository(courses);
            }

            return courses;
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
            var cmd = new GetCourse()
            {
                SearchParameters = new CourseSearch()
                {
                    CourseId = productCourseId
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            var course = Mapper.Map<Course>(cmd.Courses.FirstOrDefault());

            if ((String.IsNullOrEmpty(course.QuestionRepositoryCourseId))&&
                (requiredQuestionBankRepository))
            {
                var result = GetQuestionBankRepositoryCourseFromItems(new[] { course.ProductCourseId });
                course.QuestionRepositoryCourseId = result.GetValue(course.ProductCourseId, String.Empty);
            }

            return course;
        }

    }
}
