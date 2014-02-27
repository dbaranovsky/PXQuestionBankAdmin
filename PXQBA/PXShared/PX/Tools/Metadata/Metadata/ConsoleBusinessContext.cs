using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.Common.SSO;

namespace Metadata
{
    public class ConsoleBusinessContext : IBusinessContext
    {
        #region Properties
        public Bfw.Common.Logging.ILogger Logger
        {
            get;
            set;
        }

        public Bfw.Common.Logging.ITraceManager Tracer
        {
            get;
            set;
        }

        public Bfw.Common.Caching.ICacheProvider CacheProvider
        {
            get;
            set;
        }

        public Bfw.PX.Biz.DataContracts.UserInfo CurrentUser
        {
            get;
            set;
        }

        public bool IsAnonymous
        {
            get;
            set;
        }

        public bool IsPublicView
        {
            get;
            set;
        }

        public Bfw.PX.Biz.DataContracts.Domain Domain
        {
            get;
            set;
        }

        public string DashboardCourseId
        {
            get;
            set;
        }

        public string EntityId
        {
            get;
            set;
        }

        public string EnrollmentId
        {
            get;
            set;
        }

        public string EnrollmentStatus
        {
            get;
            set;
        }

        public string EnvironmentUrl
        {
            get;
            set;
        }

        public string AppDomainUrl
        {
            get;
            set;
        }

        public string ExternalResourceBaseUrl
        {
            get;
            set;
        }

        public string ProxyUrl
        {
            get;
            set;
        }

        public string DiscussionPrefix
        {
            get;
            set;
        }

        public Bfw.PX.Biz.DataContracts.Course Product
        {
            get;
            set;
        }

        public Bfw.PX.Biz.DataContracts.Course Course
        {
            get;
            set;
        }

        public bool CourseIsProductCourse
        {
            get;
            set;
        }

        public AccessLevel AccessLevel
        {
            get;
            set;
        }

        public bool IsCourseReadOnly
        {
            get;
            set;
        }

        public bool IsSharedCourse
        {
            get;
            set;
        }

        public bool ImpersonateStudent
        {
            get;
            set;
        }

        public string StudentViewCookieKey
        {
            get;
            set;
        }

        public string PreviewAsVisitorCookieKey
        {
            get;
            set;
        }

        public AccessType AccessType
        {
            get;
            set;
        }

        public bool CanCreateCourse
        {
            get;
            set;
        }

        public string CourseId
        {
            get;
            set;
        }

        public string ProductCourseId
        {
            get;
            set;
        }

        public string StudentDashboardId
        {
            get;
            set;
        }

        public string SiteID
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public string RABaseUrl
        {
            get;
            set;
        }

        public string ProductType
        {
            get;
            set;
        }

        public string BhAuthCookieValue
        {
            get;
            set;
        }

        public string BrainHoneyUrl
        {
            get;
            set;
        }

        #endregion

        public ConsoleBusinessContext()
        {
            Logger = new Bfw.Common.Logging.NullLogger();
            Tracer = new Bfw.Common.Logging.NullTraceManager();
            CacheProvider = new Bfw.Common.Caching.NullCacheProvider();
        }

        #region Methods
        public string GetProductCourseId(string course, string url)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {            
        }

        public string NewItemId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string Sequence(string min, string max)
        {
            return Bfw.Common.Tumbler.GetTumbler(min, max);
        }

        public Dictionary<string, string> GetQuestionTypes()
        {
            throw new NotImplementedException();
        }

        public List<string> GetDownloadOnlyDocuments()
        {
            throw new NotImplementedException();
        }       

        public IEnumerable<Bfw.PX.Biz.DataContracts.Course> FindAllEnrolledCoursesForRAUser(bool isSingleDomain)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Bfw.PX.Biz.DataContracts.Course> FindCoursesByUserEnrollment(string userId, string domainId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Bfw.PX.Biz.DataContracts.Course> FindCoursesByUserEnrollment(string userId, string domainId, string parentId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentUser(string domainId)
        {            
        }

        public IEnumerable<Bfw.PX.Biz.DataContracts.Domain> GetRaUserDomains()
        {
            throw new NotImplementedException();
        }

        public void RefreshCourse()
        {            
        }

        public string GetSamlAuthenticationBHComponentUrl()
        {
            throw new NotImplementedException();
        }

        public Bfw.PX.Biz.DataContracts.UserInfo GetNewUserData()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Bfw.PX.Biz.DataContracts.Course> FindCoursesByUserEnrollmentBatch(List<Bfw.PX.Biz.DataContracts.UserInfo> userInfo, string productCourseId, bool titleAndIdOnly = false)
        {
            throw new NotImplementedException();
        }

        public void InitializeDomains()
        {
        }
        #endregion
    }
}
