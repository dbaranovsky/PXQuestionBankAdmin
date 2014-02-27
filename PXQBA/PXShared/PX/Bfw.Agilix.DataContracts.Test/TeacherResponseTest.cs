using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Bfw.Common.Test;

namespace Bfw.Agilix.DataContracts.Test
{
    [TestClass]
    public class TeacherResponseTest : BaseTest
    {
        #region Before and After each Test methods

        private const String
            path = @"TeacherResponseTest\",
            outputDir = "TeacherResponseTest";

        private MockDlapConnection connection;

        [TestInitialize]
        public void BeforeEachTest()
        {
        }

        [TestCleanup]
        public void AfterEachTest()
        {
        }

        #endregion

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void ParseEntity()
        {
            XElement testFile = new MockDlapConnection().GetTestXmlResponse( getFileLocator("FullXML"));

            var actual = new TeacherResponse();
            actual.ParseEntity(testFile);
            var expected = new TeacherResponse()
            {
                ForeignId = "7",
                Mask = GradeStatus.None,
                PointsAssigned = 1.1d,
                PointsComputed = 1.3d,
                PointsPossible = 1.5d,
                ScoredVersion = 3,
                Status = GradeStatus.None,
                SubmittedDate = DateTime.Parse("9999-12-31T23:59:59.9999998Z"),
                TeacherResponseType = TeacherResponseType.None,
                Responses = new List<TeacherResponse>()
                {
                    new TeacherResponse()
                    {
                        ForeignId = "8",
                Mask = GradeStatus.ShowScore,
                PointsAssigned = 1.2d,
                PointsComputed = 1.4d,
                PointsPossible = 1.6d,
                ScoredVersion = 10,
                Status = GradeStatus.ShowScore,
                SubmittedDate = DateTime.Parse("9999-12-31T23:59:59.9999999Z"),
                TeacherResponseType = TeacherResponseType.Submission,
                Responses = new List<TeacherResponse>()
                    }
                }
            };

            expected.Assert_AreEqual(actual, "");
        }
    }
}
