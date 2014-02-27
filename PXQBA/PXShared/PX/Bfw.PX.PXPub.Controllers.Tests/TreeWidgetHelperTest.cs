using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using PxDC = Bfw.PX.Biz.DataContracts;
using PxM = Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class TreeWidgetHelperTest
    {
        private TreeWidgetHelper _helper;
        private IBusinessContext _context;
        private IContentActions _contentActions;
        private IGradeActions _gradeActions;

        private const string EntityId = "11234";

        private readonly List<PxDC.ContentItem> ContentListA = new List<PxDC.ContentItem>()
        {
            new PxDC.ContentItem()
            {
                Type = "ExternalContent",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcde")
                },
                MaxPoints = 2
            },
            new PxDC.ContentItem()
            {
                Type = "HtmlDocument",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                },
                MaxPoints = 0
            },
            new PxDC.ContentItem()
            {
                Type = "HtmlDocument",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcde")
                },
                MaxPoints = 5
            },
            new PxDC.ContentItem()
            {
                Type = "Quiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abc")
                },
                MaxPoints = 10
            },
            new PxDC.ContentItem()
            {
                Type = "HtmlQuiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcde")
                },
                MaxPoints = 10
            },
            new PxDC.ContentItem()
            {
                Type = "Quiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcd")
                },
                MaxPoints = 0
            },
            new PxDC.ContentItem()
            {
                Type = "PxUnit",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcd")
                },
                MaxPoints = 2
            },
            new PxDC.ContentItem()
            {
                Type = "PxUnit",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("xbookfilter", "getsome")
                },
                MaxPoints = 2
            }
        };

        private readonly List<PxDC.ContentItem> ContentListB = new List<PxDC.ContentItem>()
        {
            new PxDC.ContentItem()
            {
                Id = "a",
                Type = "Quiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcde")
                },
                MaxPoints = 10
            },
            new PxDC.ContentItem()
            {
                Id = "b",
                Type = "WritingAssignment",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                },
                MaxPoints = 15
            },
            new PxDC.ContentItem()
            {
                Id = "c",
                Type = "HtmlDocument",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcde")
                },
                MaxPoints = 20
            },
            new PxDC.ContentItem()
            {
                Id = "d",
                Type = "Quiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abc")
                },
                MaxPoints = 10
            },
            new PxDC.ContentItem()
            {
                Id = "e",
                Type = "Quiz",
                SubContainerIds = new List<PxDC.Container>() {
                    new PxDC.Container("syllabusfilter", "abcef")
                },
                MaxPoints = 1
            }
        };

        private readonly List<PxDC.Grade> GradeListB = new List<PxDC.Grade>()
        {
            new PxDC.Grade
            {
                GradedItem = null,
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Achieved = 1,
                Possible = 10
            },
            new PxDC.Grade
            {
                GradedItem = null,
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Achieved = 10,
                Possible = 10
            },
            new PxDC.Grade
            {
                GradedItem = null,
                Status = GradeStatus.NeedsGrading,
                Achieved = 14,
                Possible = 20
            },
            new PxDC.Grade
            {
                GradedItem = null,
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Achieved = 0,
                Possible = 5
            },
            new PxDC.Grade
            {
                GradedItem = null,
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Achieved = 0,
                Possible = 0
            }
        };

        [TestInitialize]
        public void Setup()
        {
            _helper = new TreeWidgetHelper();
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();
            _gradeActions = Substitute.For<IGradeActions>();

            _context.EntityId.Returns(EntityId);
            _context.ProductCourseId.Returns("0");

            GradeListB[0].GradedItem = ContentListB[0];
            GradeListB[1].GradedItem = ContentListB[1];
            GradeListB[2].GradedItem = ContentListB[2];
            GradeListB[3].GradedItem = ContentListB[3];
            GradeListB[4].GradedItem = ContentListB[4];
        }
        
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainerItems_WithEmptyList_ReturnsEmptyList()
        {
            List<PxDC.ContentItem> listContainers = null;
            _contentActions.GetContainerItems(String.Empty, String.Empty, String.Empty).Returns(listContainers);

            var retval = _helper.GetContainersItems(_contentActions, _context, new TreeWidgetSettings(), String.Empty, String.Empty);
            
            Assert.IsInstanceOfType(retval, typeof(List<TreeWidgetViewItem>));
            Assert.AreEqual(0, retval.Count);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainerItems_WithTOCInSettings_ReturnsExpectedValues()
        {
            const string testId = "122222";
            const string toc = "tttfilter";
            const string parentId = "parentpppp";
            const string sequence = "zzz";

            var listContainers = new List<PxDC.ContentItem>()
            {
                new PxDC.ContentItem()
                {
                    Id = testId,
                    Type = "",
                    Categories = new List<PxDC.TocCategory>()
                    {
                        new PxDC.TocCategory()
                        {
                            Id = toc,
                            ItemParentId = parentId,
                            Sequence = sequence,
                            Text = "nothing"
                        }
                    }
                }
            };
            _contentActions.GetContainerItems(EntityId, String.Empty, String.Empty, toc).Returns(listContainers);
            
            var settings = new TreeWidgetSettings {TOC = toc};
            var retval = _helper.GetContainersItems(_contentActions, _context, settings, String.Empty, String.Empty);
            
            Assert.IsInstanceOfType(retval, typeof(List<TreeWidgetViewItem>));
            Assert.IsTrue(retval.Count > 0);
            Assert.AreEqual(retval[0].ParentId, parentId);
            Assert.AreEqual(retval[0].Item.ParentId, parentId);
            Assert.AreEqual(retval[0].Item.Sequence, sequence);
            Assert.AreEqual(retval[0].Settings.TOC, toc);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetSubcontainerItemIds_WithEmptyList_ReturnsEmptyList()
        {
            var retval = _helper.GetSubcontainerItemIds(null, "syllabusfilter");
            Assert.IsInstanceOfType(retval, typeof (List<string>));
            Assert.AreEqual(0, retval.Count);
        }

        /// <summary>
        /// Testing that SubContainerId is properly grouped together when calling GetTopLevelItemIds
        /// </summary>
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetSubcontainerItemIds_WithListA_Returns3Strings()
        {
            var retval = _helper.GetSubcontainerItemIds(ContentListA, "syllabusfilter");
            Assert.AreEqual(3, retval.Count);
            Assert.IsTrue(retval.Contains("abc") && retval.Contains("abcd") && retval.Contains("abcde"));
        }

        /// <summary>
        /// Testing that and emptry SubContainerId is filtered out of a GetTopLevelItemsIds call
        /// </summary>
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetSubcontainerItemIds_WithEmptyStringSubContainerId_ReturnsEmptyList()
        {
            var retval = _helper.GetSubcontainerItemIds(
                new List<PxDC.ContentItem>()
                {
                    new PxDC.ContentItem()
                    {
                        SubContainerIds = new List<PxDC.Container>()
                    }
                }, "syllabusfilter");
            Assert.AreEqual(0, retval.Count);
        }

        /// <summary>
        /// Testing that passing in a different toc will return the proper subcontainer items
        /// </summary>
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetSubcontainerItemIds_WithListA_AndTocSet_Returns1String()
        {
            var retval = _helper.GetSubcontainerItemIds(ContentListA, "xbookfilter");
            Assert.AreEqual(1, retval.Count);
            Assert.IsTrue(retval.Contains("getsome"));
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_SetGrade_WithNullItemOrGrade_ThrowsNoException()
        {
            _helper.SetGrade(null, null);
            Assert.IsTrue(true);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_SetGrade_WithCompletedItem_SubmittedIsSet()
        {
            var contentItem = new PxM.ContentItem();
            var grade = new PxDC.Grade()
            {
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Possible = 10,
                Achieved = 8
            };
            _helper.SetGrade(contentItem, grade);
            Assert.IsTrue(contentItem.IsUserSubmitted);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_SetGrade_WithCompletedItem_ScoreIsSet()
        {
            var contentItem = new PxM.ContentItem();
            var grade = new PxDC.Grade()
            {
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Possible = 10,
                Achieved = 8
            };
            _helper.SetGrade(contentItem, grade);
            Assert.AreEqual<double>(8d, contentItem.Score);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_SetGrade_WithCompletedLearningCurveItem_WithZeroAchievedGrade_UserSubmittedFalsed()
        {
            var contentItem = new PxM.ContentItem();
            contentItem.FacetMetadata["meta-content-type"] = "LearningCurve";
           
            var grade = new PxDC.Grade()
            {
                Status = GradeStatus.Completed | GradeStatus.ShowScore,
                Possible = 10,
                Achieved = 0
            };
            _helper.SetGrade(contentItem, grade);
            Assert.IsFalse(contentItem.IsUserSubmitted);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetTotalAndMaxPossibleScore_WithInvalidArgs_ReturnsNull()
        {
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            var result = helper.InvokeStatic("GetTotalAndMaxPossibleScore", new object[] { null, null });
            Assert.IsNull(result);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetTotalAndMaxPossibleScore_WithListB_MatchExpectedScores()
        {
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            var result = (Bfw.PX.PXPub.Controllers.Helpers.TreeWidgetHelper.UnitCompletionStats)
                helper.InvokeStatic("GetTotalAndMaxPossibleScore", new object[] { GradeListB, ContentListB });
            Assert.AreEqual(4, result.CompletedItems);
            Assert.AreEqual(35, result.MaxScore);
            Assert.AreEqual(16, result.TotalScore);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_CalculateCompletedPercentage_WithNullSubcontainer_NoExceptionsThrown()
        {
            //Make sure an exception isn't thrown
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            helper.InvokeStatic("CalculateCompletedPercentage", new object[] { null, 8, 4, 50, 100 });
            Assert.IsTrue(true);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_CalculateCompletedPercentage_WithZeroDenominators_NoDivideByZeroError()
        {
            var subcontainer = new PxM.ContentItem();
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            helper.InvokeStatic("CalculateCompletedPercentage", new object[] { subcontainer, 0, 4, 50, 0 });
            Assert.AreEqual(0, subcontainer.StudentCompletedPercentage);
            Assert.AreEqual(0, subcontainer.StudentScorePercentage);
            Assert.AreEqual(4, subcontainer.StudentCompletedItems);
            Assert.AreEqual(0, subcontainer.StudentItemsAssigned);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_CalculateCompletedPercentage_WithNotEvenDivisibleNumbers_NumbersAreProperlyRounded()
        {
            var subcontainer = new PxM.ContentItem();
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            helper.InvokeStatic("CalculateCompletedPercentage", new object[] { subcontainer, 11, 4, 50, 227 });
            Assert.AreEqual(36, subcontainer.StudentCompletedPercentage);
            Assert.AreEqual(22, subcontainer.StudentScorePercentage);
            Assert.AreEqual(4, subcontainer.StudentCompletedItems);
            Assert.AreEqual(11, subcontainer.StudentItemsAssigned);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_CalculateCompletedPercentage_FiveInTenthsPlaceOfCompletedPercentage_PercentageRoundsUp()
        {
            //Whats the actual desired rounding functionality here?
            var subcontainer = new PxM.ContentItem();
            PrivateType helper = new PrivateType(typeof(TreeWidgetHelper));
            helper.InvokeStatic("CalculateCompletedPercentage", new object[] { subcontainer, 11, 6, 50, 227 });
            Assert.AreEqual(55, subcontainer.StudentCompletedPercentage);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TreeWidgetHelperAction_LoadItemCalledWithoutImplementation_ThrowsException()
        {
            _helper.LoadItem(_contentActions, _context, _gradeActions, string.Empty, string.Empty);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TreeWidgetHelperAction_ProcessStudentGradesCalledWithoutImplementation_ThrowsException()
        {
            PxM.ContentItem item = new PxM.ContentItem();
            _helper.ProcessStudentGrades(_contentActions, _context, _gradeActions, null, ref item);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainersItems_Returns_Items_From_Derived_Course_LegacyXML()
        {
            var settings = new TreeWidgetSettings()
            {
                UseProductCourse = false
            };

            _helper.GetContainersItems(_contentActions, _context, settings, "1", "2");
            var calls = _contentActions.ReceivedCalls();

            calls.First().GetArguments()[0].Equals(_context.EntityId);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainersItems_Returns_Items_From_Derived_Course()
        {
            var settings = new TreeWidgetSettings()
            {
                 UseProductCourse = false
            };

            _helper.GetContainersItems(_contentActions, _context, settings, "1", "2");
            var calls = _contentActions.ReceivedCalls();

            calls.First().GetArguments()[0].Equals(_context.EntityId);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainersItems_Returns_Items_From_Product_Course_LegacyXML()
        {
            var settings = new TreeWidgetSettings()
            {
                UseProductCourse = true
            };

            _helper.GetContainersItems(_contentActions, _context, settings, "1", "2");
            var calls = _contentActions.ReceivedCalls();

            calls.First().GetArguments()[0].Equals(_context.ProductCourseId);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetContainersItems_Returns_Items_From_Product_Course()
        {
            var settings = new TreeWidgetSettings()
            {
                UseProductCourse = true
            };

            _helper.GetContainersItems(_contentActions, _context, settings, "1", "2");
            var calls = _contentActions.ReceivedCalls();

            calls.First().GetArguments()[0].Equals(_context.ProductCourseId);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetUnitTemplateId_IfNoTemplateExists_ReturnsEmptyString()
        {
            _contentActions.GetAllTemplates().Returns(new List<PxDC.ContentItem>());
            var result = _helper.GetUnitTemplateId(_contentActions);

            Assert.AreEqual(string.Empty, result);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetUnitTemplateId_IfTemplateExists_ReturnsTemplateId()
        {
            var testid = "testid";
            _contentActions.GetAllTemplates().Returns(new List<PxDC.ContentItem>()
            {
                new PxDC.ContentItem()
                {
                    Id = testid,
                    Subtype = "PxUnit"
                }
            });
            var result = _helper.GetUnitTemplateId(_contentActions);

            Assert.AreEqual(testid, result);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void TreeWidgetHelperAction_GetUnitTemplateId_IfTemplateExists_WithUnmatchCase_ReturnsTemplateId()
        {
            var testid = "testid";
            _contentActions.GetAllTemplates().Returns(new List<PxDC.ContentItem>()
            {
                new PxDC.ContentItem()
                {
                    Id = testid,
                    Subtype = "PXUNIT"
                }
            });
            var result = _helper.GetUnitTemplateId(_contentActions);

            Assert.AreEqual(testid, result);
        }
    }
}
