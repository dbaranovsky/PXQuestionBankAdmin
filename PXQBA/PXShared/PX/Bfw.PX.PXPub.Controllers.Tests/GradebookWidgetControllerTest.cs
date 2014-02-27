using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Web.Mvc;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class GradebookWidgetControllerTest
    {
        /// <summary>
        /// Checks wether Assigned Scores action method throws up an error for a null / empty Grade List 
        /// </summary>
        [TestMethod]
        public void AssignedScores_Gradebook_DoesNotThrowError_WhenGradeListIsNull()
        {
            //Arrange
            var context = Substitute.For<IBusinessContext>();;
            var enrollmentActions = Substitute.For<IEnrollmentActions>();;
            var gradeBookActions = Substitute.For<IPxGradeBookActions>();
            var contentActions = Substitute.For<IContentActions>();
            var gradebookWidgetController = new GradebookWidgetController(context, enrollmentActions, gradeBookActions, contentActions);;

            var fakeStudentList = new List<BizDC.StudentProfile>
                              {
                                  new BizDC.StudentProfile {
                                      EnrollmentId = "1234", 
                                      UserId = "4321", 
                                      FirstName = "testFirstName",
                                      LastName = "testLastName"
                                  }
                              };
            //build a fake Assignment Folders dictionary consisting of Assignment Folders and Assignment Items
            var fakeAssignmentItems = new List<BizDC.ContentItem>
                                  {
                                      new BizDC.ContentItem { Id = "db262756e34149c9a5a026ba71c4a6ba" },
                                      new BizDC.ContentItem { Id = "eb262756e34149c9a5a026ba71c4a6ba" }
                                  };
            var fakeAssignmentFolders = new Dictionary<BizDC.ContentItem, IList<BizDC.ContentItem>>();
            fakeAssignmentFolders.Add(new BizDC.ContentItem() { Id = "ab262756e34149c9a5a026ba71c4a6ba" }, fakeAssignmentItems);
            
            //fake student enrollment consisting of a Grade List that is null
            var fakeEnrollments = new List<BizDC.Enrollment>
                                  {
                                      new BizDC.Enrollment() { Id = "testEnrollmentId", ItemGrades = null }
                                  };

            //Act
            gradeBookActions.GetStudentList().Returns(fakeStudentList);
            gradeBookActions.GetGradeBookAssignments().Returns(fakeAssignmentFolders);
            gradeBookActions.GetEnrollments().Returns(fakeEnrollments);
            
            var result = gradebookWidgetController.AssignedScores() as ViewResult;
            var model = result.Model as GradeBook;

            //Assert
            Assert.IsNotNull(model);

        }
    }
}
