using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.Agilix.Dlap;
using TestHelper;
using Grade = Bfw.Agilix.DataContracts.Grade;

namespace Bfw.Agilix.Commands.Tests
{
    
    
    /// <summary>
    ///This is a test class for GetGradebookSummaryTest and is intended
    ///to contain all GetGradebookSummaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GetGradebookSummaryTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private class TestGrade : Grade
        {
            public int PublicSubmittedVersion
            {
                set
                {
                    SubmittedVersion = value;
                }
            }
            public int PublicScoredVersion
            {
                set
                {
                    ScoredVersion = value;
                }
            }
            public double PublicAchieved
            {
               
                set
                {
                    Achieved = value;
                }
            }
            public double PublicPossible
            {
                
                set
                {
                    Possible = value;
                }
            }
            public int PublicAttempts
            {
                
                set
                {
                    Attempts = value;
                }
            }

            public Item PublicItem
            {
                
                set
                {
                    Item = value;
                }
            }
        }
        /// <summary>
        ///A test for ParseResponse
        ///</summary>
        [TestMethod()]
        public void ParseResponse_Should_Parse_XML_Format()
        {
            GetGradebookSummary target = new GetGradebookSummary()
            {
                EntityId = "entityId1",
                ItemIds = new List<string>() {"item1", "item2"}
            };

         var expectedEnrollmentGrades = new Dictionary<string, Grade>()
            {
                {"216455", new TestGrade() { 
                    Status = GradeStatus.Completed | GradeStatus.ShowScore | GradeStatus.Released, 
                    SubmittedDate = DateTime.Parse("2013-09-03T19:27:12.233Z"), 
                    ScoredDate = DateTime.Parse("2013-09-03T19:27:12.327Z"),
                    PublicSubmittedVersion = 1,
                    PublicScoredVersion = 1,
                    PublicAchieved = 5,
                    PublicPossible = 6,
                    PublicAttempts = 1,
                    PublicItem = new Item(){Id = "item1", IsGradable = true},
                    Seconds = 576
                    }},
                {"219849", new TestGrade() { 
                    Status = GradeStatus.Completed | GradeStatus.ShowScore| GradeStatus.Released, 
                    SubmittedDate = DateTime.Parse("2013-09-06T03:59:33.127Z"), 
                    ScoredDate = DateTime.Parse("2013-09-06T03:59:33.207Z"),
                    PublicSubmittedVersion = 1,
                    PublicScoredVersion = 1,
                    PublicAchieved = 4,
                    PublicPossible = 6,
                    PublicAttempts = 2,
                    PublicItem = new Item(){Id = "item1", IsGradable = true},
                    Seconds = 523
                    }},
                 {"214629", new TestGrade() { 
                    Status = GradeStatus.Completed | GradeStatus.ShowScore| GradeStatus.Released, 
                    SubmittedDate = DateTime.Parse("2013-09-03T18:55:26.497Z"), 
                    ScoredDate = DateTime.Parse("2013-09-03T18:55:26.667Z"),
                    PublicSubmittedVersion = 1,
                    PublicScoredVersion = 1,
                    PublicAchieved = 3,
                    PublicPossible = 6,
                    PublicAttempts = 1,
                    PublicItem = new Item(){Id = "item2", IsGradable = true},
                    Seconds = 129
                    }},
            };
            
                    

            DlapResponse response = new DlapResponse();
            response.ResponseXml = Helper.GetXDocument("GetGradebookSummary");
            
            target.ParseResponse(response);

            Assert.AreEqual(target.EnrollmentGrades.Count, 3);

            foreach (var actualGrade in target.EnrollmentGrades)
            {
                var expectedGrade = expectedEnrollmentGrades[actualGrade.Key] as Grade;
                Assert.IsNotNull(expectedGrade);
                Assert.IsTrue(ObjectComparer.AreObjectsEqual(actualGrade.Value, expectedGrade, new []{"Data"}));
            }
        }

        /// <summary>
        ///A test for ToRequest
        ///</summary>
        [TestMethod()]
        public void ToRequest_IncludesEntityId_ItemIds_verbose()
        {
            GetGradebookSummary target = new GetGradebookSummary()
            {
                EntityId = "entityId1",
                ItemIds = new List<string>() {"item1", "item2"}
            };
            DlapRequest expected = new DlapRequest()
            {
                Parameters = new Dictionary<string, object>()
                {
                    {"cmd", "GetGradebookSummary"},
                    {"entityid", "entityId1"},
                    {"itemid", "item1|item2"},
                    {"verbose", "true"}
                },
                Type = DlapRequestType.Get
            };
            DlapRequest actual;
            actual = target.ToRequest();
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected, actual));
            
        }
    }
}
