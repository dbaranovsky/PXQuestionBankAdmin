using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
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


        public IEnumerable<Course> GetAvailableCourses()
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
            return GetCoursesByCourseIds(courseIds);
        }

        private IEnumerable<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds)
        {
            var courses = new List<Course>();
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
            return courses;
        }

        public Course GetProductCourse(string productCourseId)
        {
            var cmd = new GetCourse()
            {
                SearchParameters = new CourseSearch()
                {
                    CourseId = productCourseId
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return Mapper.Map<Course>(cmd.Courses.FirstOrDefault());
        }
    }
}
