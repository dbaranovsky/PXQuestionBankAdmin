using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Linq;
using Bfw.Common.Caching;
using Bfw.Common.Logging;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class ReportActionsTest
    {
        private IBusinessContext _context;
        private ISessionManager _sessionManager;        
        private ICourseActions _courseActions;        
        private IContentActions _contentActions;
        private ITraceManager _traceManager;
        private ICacheProvider _cacheProvider;
        private IEnrollmentActions _enrollmentActions;
        private IRubricActions _rubricActions;
        private IDashboardActions _dashboardActions;
        private IGradeActions _gradeActions;        
        private IUserActions _userActions;
        private IServiceLocator _serviceLocator;

        private ReportActions _reportActions;


        [TestInitialize]
        public void TestInitialize()
        {
            ConfigurationManager.AppSettings["InstructorPermissionFlags"] = "552155348992";
            ConfigurationManager.AppSettings["StudentPermissionFlags"] = "131073";

            
            Substitute.For<IDomainActions>();
            _contentActions = Substitute.For<IContentActions>();
            _traceManager = Substitute.For<ITraceManager>();
            _context = Substitute.For<IBusinessContext>();
            _context.Tracer.Returns(_traceManager);
            _sessionManager = Substitute.For<ISessionManager>();
            _cacheProvider = Substitute.For<ICacheProvider>();
            _courseActions = Substitute.For<ICourseActions>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _rubricActions = Substitute.For<IRubricActions>();
            _dashboardActions = Substitute.For<IDashboardActions>();
            _gradeActions = Substitute.For<IGradeActions>();            
            _userActions = Substitute.For<IUserActions>();
            _context.CacheProvider.Returns(_cacheProvider);
            _serviceLocator = Substitute.For<IServiceLocator>();
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);

            _reportActions = new ReportActions(_context, _enrollmentActions, _rubricActions, _dashboardActions, _sessionManager, _contentActions, _gradeActions, _courseActions, _userActions);

        }

        private BizDC.RubricsReportContainer GetRubricsReportData()
        {
            var rubricResults = new Dictionary<string, BizDC.RubricGrade>
            {
                {
                    "tempKey", new BizDC.RubricGrade
                    {
                        Achieved = 1.0,
                        EnrollmentId = "45456",
                        Possible = 5.0,
                        RubricRuleId = "12345"
                    }
                }
            };
            var studentReports = new List<BizDC.StudentRubricReport>
            {
                new BizDC.StudentRubricReport
                {
                    AssignmentsCompleted = 1,
                    AverageAllCriteria = 100,
                    EnrollmentId = "45456",
                    IsStudentDeleted = false,
                    ReviewedSubmissions = 2,
                    RubricResults = rubricResults
                }
            };

            var reportData = new BizDC.RubricsReportContainer { RubricsReport = new List<BizDC.RubricsReport>() };
            var rubricReportItem = new BizDC.RubricsReport()
            {
                CourseName = "Temp Course",
                EntityId = "99999",
                InstructorId = "12345",
                StudentReports = studentReports
            };
            reportData.RubricsReport.Add(rubricReportItem);

            var rubricReportItem2 = new BizDC.RubricsReport()
            {
                CourseName = "Temp Course 2",
                EntityId = "88888",
                InstructorId = "11111",
                StudentReports = studentReports
            };

            reportData.RubricsReport.Add(rubricReportItem2);

            reportData.RubricContentItem = null;

            return reportData;
        }
    }
}
