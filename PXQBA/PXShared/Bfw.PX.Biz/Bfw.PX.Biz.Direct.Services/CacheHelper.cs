using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Models;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Helper class to build item queries.
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// Helper method to attempt to retrieve item of type T from cache with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the item to search for.</typeparam>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="cacheKey">Key for the requested cache item.</param>
        /// <param name="result">Output parameter returning cached item if found, null otherwise.</param>
        /// <returns>True if item found in cache, false otherwise.</returns>
        public static bool TryFetch<T>(this ICacheProvider cacheProvider, string cacheKey, out T result) where T : class
        {
            result = cacheProvider.Fetch(cacheKey) as T;
            if (result == null)
            {
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as T;
            }

            return result != null;
        }

        #region Cache Fetch methods

        public static Object FetchByProductCourseRegion(this ICacheProvider cacheProvider, string key)
        {
            Object obj = null;
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            if (context != null)
            {
                var courseId = !string.IsNullOrEmpty(context.CourseId) ? context.CourseId : null;
                var productCourseId = !string.IsNullOrEmpty(context.ProductCourseId) ? context.ProductCourseId : courseId;
                obj = cacheProvider.Fetch(key, "ProductCourse_" + productCourseId);
            }
            else
            {
                obj = cacheProvider.Fetch(key);
            }
            return obj;
        }

        public static IDictionary<string, object> FetchByProductCourseRegion(this ICacheProvider cacheProvider, List<string> keys)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            if (context != null)
            {
                var courseId = !string.IsNullOrEmpty(context.CourseId) ? context.CourseId : null;
                var productCourseId = !string.IsNullOrEmpty(context.ProductCourseId) ? context.ProductCourseId : courseId;
                result = cacheProvider.Fetch(keys, "ProductCourse_" + productCourseId);
            }
            else
            {
                result = cacheProvider.Fetch(keys, "");
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve course item from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Cached course item if found, null otherwise.</returns>
        public static object FetchCourseItem(this ICacheProvider cacheProvider, string courseId, string key)
        {
            object result = null;

            if (!courseId.IsNullOrEmpty() && !key.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_{1}", courseId, key);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey);
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve page definition object from cache.StoreCourseItem
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <param name="pageId">ID of the page definition to search for in cache.</param>
        /// <returns>Cached page definition if found, null otherwise.</returns>
        public static Bdc.Widget FetchWidget(this ICacheProvider cacheProvider, string courseId, string widgetId)
        {
            Bdc.Widget result = null;

            if (!courseId.IsNullOrEmpty() && !widgetId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_WIDGET_{1}", courseId, widgetId);

                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Widget;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve course item from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Fetch for QBA.</returns>
        public static object FetchQBAQuestions(this ICacheProvider cacheProvider, string courseId, string ListType)
        {
            object result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_QBA_Course_{0}_{1}", courseId, ListType);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Dictionary<string, string>;
            }
            return result;
        }


        /// <summary>
        /// Attempts to retrieve course item from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Cached course item if found, null otherwise.</returns>
        public static Bdc.Course FetchCourse(this ICacheProvider cacheProvider, string courseId)
        {
            Bdc.Course result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}", courseId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Course;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve course list from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="course">Course item to store.</param>
        public static List<Bdc.Course> FetchCourseList(this ICacheProvider cacheProvider, string ref_id)
        {
            List<Bdc.Course> result = null;

            if (!ref_id.IsNullOrEmpty())
            {
                string cacheKey = string.Format("COURSE_LIST_REF_{0}", ref_id);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as List<Bdc.Course>;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve user item from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="userId">ID of the user to search for in cache.</param>
        /// <returns>Cached user item if found, null otherwise.</returns>
        public static Bdc.UserInfo FetchUser(this ICacheProvider cacheProvider, string userId)
        {
            Bdc.UserInfo result = null;

            if (!userId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}", userId);
                result = cacheProvider.Fetch(cacheKey) as Bdc.UserInfo;
            }
            return result;
        }

        public static Bdc.UserInfo FetchUserByReference(this ICacheProvider cacheProvider, string domainId, string referenceId) //***LMS - this is no longer unique
        {
            Bdc.UserInfo result = null;

            if (!referenceId.IsNullOrEmpty() && !domainId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_DOMAIN_{0}_REF_{1}", domainId, referenceId);
                result = cacheProvider.Fetch(cacheKey) as Bdc.UserInfo;
            }
            return result;
        }

        public static IEnumerable<Adc.AgilixUser> FetchUsersByReference(this ICacheProvider cacheProvider, string referenceId) //***LMS - this is no longer unique
        {
            IEnumerable<Adc.AgilixUser> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USERS_REF_{0}", referenceId);
                result = cacheProvider.Fetch(cacheKey) as IEnumerable<Adc.AgilixUser>;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve enrollment object from cache by specified course and user ID.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="enrollmentId">ID of the user to search for in cache.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Cached user item if found, null otherwise.</returns>
        public static Bdc.Enrollment FetchEnrollmentByCourse(this ICacheProvider cacheProvider, string userId, string courseId)
        {
            Bdc.Enrollment result = null;

            if (!userId.IsNullOrEmpty() && !courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_ENTITY_{1}", userId, courseId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Enrollment;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve enrollment object from cache by specified user reference ID.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="referenceId">ID of the user to search for in cache.</param>
        /// <returns>Cached user item if found, null otherwise.</returns>
        public static List<Bdc.Enrollment> FetchUserEnrollmentList(this ICacheProvider cacheProvider, string referenceId) //***LMS - this is no longer unique
        {
            List<Bdc.Enrollment> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENTS", referenceId);
                result = cacheProvider.Fetch(cacheKey) as List<Bdc.Enrollment>;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve enrollment object from cache by specified course and user ID.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="referenceId">ID of the user to search for in cache.</param>
        /// <param name="productId">ID of the product course to search for in cache.</param>
        /// <returns>Cached user item if found, null otherwise.</returns>
        public static List<Bdc.Enrollment> FetchUserEnrollmentList(this ICacheProvider cacheProvider, string referenceId, string productId, bool loadCourses, bool getEnrollmentCount) //***LMS - this is no longer unique
        {
            List<Bdc.Enrollment> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", referenceId, productId,loadCourses.ToString(), getEnrollmentCount.ToString());
                result = cacheProvider.Fetch(cacheKey) as List<Bdc.Enrollment>;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve enrollment object from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="enrollmentId">ID of the user to search for in cache.</param>
        /// <param name="courseId">ID of the enrollment to search for in cache.</param>
        /// <returns>Cached user item if found, null otherwise.</returns>
        public static Bdc.Enrollment FetchEnrollment(this ICacheProvider cacheProvider, string userId, string enrollmentId)
        {
            Bdc.Enrollment result = null;

            if (!userId.IsNullOrEmpty() && !enrollmentId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_{1}", userId, enrollmentId);
                result = cacheProvider.Fetch(cacheKey) as Bdc.Enrollment;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve page definition object from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <param name="pageId">ID of the page definition to search for in cache.</param>
        /// <returns>Cached page definition if found, null otherwise.</returns>
        public static Bdc.PageDefinition FetchPageDefinition(this ICacheProvider cacheProvider, string courseId, string pageId)
        {
            Bdc.PageDefinition result = null;

            if (!courseId.IsNullOrEmpty() && !pageId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_PAGE_{1}", courseId, pageId);

                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.PageDefinition;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve navigration menu object from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <param name="menuId">ID of the menu to search for in cache.</param>
        /// <returns>Cached menu object if found, null otherwise.</returns>
        public static Bdc.Menu FetchMenu(this ICacheProvider cacheProvider, string courseId, string menuId, bool showHiddenMenu = false)
        {
            Bdc.Menu result = null;

            if (!courseId.IsNullOrEmpty() && !menuId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_NAV_{1}_MENU_{2}", courseId, menuId, showHiddenMenu.ToString().ToUpper());

                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Menu;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve question list from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="userId">ID of the question to search for in cache.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Cached questions list if found, null otherwise.</returns>
        public static Bdc.Question FetchQuestion(this ICacheProvider cacheProvider, string courseId, string questionId)
        {
            Bdc.Question result = null;

            if (!courseId.IsNullOrEmpty() && !questionId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_QUESTION_{1}", courseId, questionId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Question;
            }
            return result;
        }

        /// <summary>
        /// Attempts to retrieve question list from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="userId">ID of the question to search for in cache.</param>
        /// <param name="courseId">ID of the course to search for in cache.</param>
        /// <returns>Cached questions list if found, null otherwise.</returns>
        public static List<Bdc.Question> FetchQuestions(this ICacheProvider cacheProvider, string courseId, List<string> questionIds)
        {
            List<Bdc.Question> result = null;

            if (!courseId.IsNullOrEmpty() && questionIds != null && questionIds.Count() > 0)
            {
                var cacheKeys = questionIds.Map(q => string.Format("PX_COURSE_{0}_QUESTION_{1}", courseId, q)).ToList();
                var cachedQuestions = cacheProvider.FetchByProductCourseRegion(cacheKeys);
                if (!cachedQuestions.IsNullOrEmpty())
                {
                    result = cachedQuestions.Where(o => o.Value != null).Map(o => o.Value as Bdc.Question).ToList();
                }
            }
            return result;
        }
       
        public static Biz.DataContracts.DashboardData FetchDashboardData(this ICacheProvider cacheProvider, string courseId)
        {
            Bdc.DashboardData result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_DASHBOARDATA_{0}", courseId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.DashboardData;
            }
            return result;
        }

        public static object FetchLearningObjectivePerformanceReport(this ICacheProvider cacheProvider, string enrollmentId, string id)
        {
            object result = null;

            if (!enrollmentId.IsNullOrEmpty() && !id.IsNullOrEmpty())
            {
                string cacheKey = string.Format("perf_report_{0}_{1}", enrollmentId, id);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey);
            }
            return result;
        }

        public static Bdc.Resource FetchResource(this ICacheProvider cacheProvider, string entityId, string resourceUri)
        {
            Bdc.Resource result = null;
            if (!entityId.IsNullOrEmpty() && !resourceUri.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PxResource_{0}_{1}", entityId, resourceUri);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as Bdc.Resource;
            }
            return result;
        }

        public static object FetchRubricPerformanceReport(this ICacheProvider cacheProvider, string enrollmentId, string itemId, string termId)
        {
            object result = null;

            if (!enrollmentId.IsNullOrEmpty() && !itemId.IsNullOrEmpty() && !termId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("rubric_perf_report_{0}_{1}_{2}", enrollmentId, itemId, termId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey);
            }
            return result;
        }

        public static IEnumerable<Bdc.ContentItem> FetchLaunchPadData(this ICacheProvider cacheProvider, string courseId)
        {
            IEnumerable<Bdc.ContentItem> result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_LAUNCHPAD_DATA_{0}", courseId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as IEnumerable<Bdc.ContentItem>;
            }
            return result;
        }
        /// <summary>
        /// Fetches the item analysis report by course.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static IDictionary<string, Bdc.ItemReport> FetchItemAnalysisReportByCourse(this ICacheProvider cacheProvider, string courseId)
        {
            IDictionary<string, Bdc.ItemReport> result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_COURSE_{0}_ANALYSISREPORTS", courseId);
                result = cacheProvider.FetchByProductCourseRegion(cacheKey) as IDictionary<string, Bdc.ItemReport>;
            }
            return result;
        }

        /// <summary>
        /// Fetches the item analysis report.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="contentId">The content id.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static Bdc.ItemReport FetchItemAnalysisReport(this ICacheProvider cacheProvider, string contentId, string courseId)
        {
            Bdc.ItemReport result = null;

            if (!courseId.IsNullOrEmpty() && !contentId.IsNullOrEmpty())
            {
                // Attempt to load course questions collection from cache.
                var qResult = cacheProvider.FetchItemAnalysisReportByCourse(courseId);
                if (qResult != null)
                {
                    string cacheKey = string.Format("PX_COURSE_{0}_ITEMANALYSISREPORT_{1}", courseId, contentId);
                    if (qResult.ContainsKey(cacheKey))
                    {
                        result = qResult[cacheKey];
                    }
                }
            }
            return result;
        }

        public static object FetchRASiteUserData(this ICacheProvider cacheProvider, string userId, string foundCourseId)
        {
            object obj = null;

            var cacheKey = string.Format("{0}:{1}", userId, foundCourseId);

            cacheProvider.TryFetch(cacheKey, out obj);

            return obj;
        }

        public static object FetchRASiteInfo(this ICacheProvider cacheProvider, string url)
        {
            object obj = null;

            var key = string.Format("BCTXT_SITEINFO_{0}", url);

            cacheProvider.TryFetch(key, out obj);

            return obj;

        }

        public static Bdc.Domain FetchDomain(this ICacheProvider cacheProvider, string domainId)
        {
            Bdc.Domain domain = null;
            string cacheKey = string.Format("BIZCTX_{0}", domainId);
            object cachedObject = null;
            cacheProvider.TryFetch(cacheKey, out cachedObject);
            domain = cachedObject as Bdc.Domain;
            return domain;
        }
        public static Bdc.Domain FetchDomainByName(this ICacheProvider cacheProvider, string domainName)
        {
            Bdc.Domain domain = null;
            string cacheKey = string.Format("BIZCTX_NAME_{0}", domainName);
            object cachedObject = null;
            cacheProvider.TryFetch(cacheKey, out cachedObject);
            domain = cachedObject as Bdc.Domain;
            return domain;
        }

        public static List<Bdc.Domain> FetchDomainList(this ICacheProvider cacheProvider, string parentDomainId)
        {
            List<Bdc.Domain> result = null;
            string cacheKey = string.Format("BIZCTX_PARENT_DOMAIN_{0}", parentDomainId);
            object cachedObject = null;
            cacheProvider.TryFetch(cacheKey, out cachedObject);
            result = cachedObject as List<Bdc.Domain>;
            return result;
        }
        public static string FetchUserIdByReferenceId(this ICacheProvider cacheProvider, string domainUserReferenceId) //***LMS - this is no longer unique
        {
            string cacheKey = string.Format("BCUSERCACHE_{0}", domainUserReferenceId);
            object cachedObj;
            cacheProvider.TryFetch(cacheKey, out cachedObj);
            if (cachedObj != null)
            {
                return cachedObj.ToString();
            }
            else
            {
                return null;
            }
        }

        public static FacetedSearchResults FetchFacetedSearchResults(this ICacheProvider cacheProvider, Bdc.SearchQuery bizQuery)
        {
            var key = "PX_COURSE_" + bizQuery.EntityId + "_FACETED_SEARCH_"
                + bizQuery.FacetedQuery.Fields.Reduce((a, b) => a + b, "")
                + bizQuery.FacetedQuery.Limit.ToString() +
                      bizQuery.FacetedQuery.MinCount.ToString();

            FacetedSearchResults searchResults = null;
            searchResults = cacheProvider.FetchByProductCourseRegion(key) as FacetedSearchResults;
            return searchResults;

        }

        public static string FetchWidgetCss(this ICacheProvider cacheProvider, string type)
        {
            var key = "PX_WIDGET_CSS" + "_" + type;
            var result = cacheProvider.Fetch(key);
            if (result != null)
            {
                return result.ToString();
            }
            else
            {
                return null;
            }
        }

        public static string FetchCssFile(this ICacheProvider cacheProvider, string url, string courseId)
        {
            var key = string.Format("PX_CSS_{0}", url);
            if (!courseId.IsNullOrEmpty())
            {
                key = key.Replace(courseId, string.Empty);
            }

            var result = cacheProvider.Fetch(key);
            if (result != null)
            {
                return result.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Cache Store methods

        public static void StoreByProductCourseRegion(this ICacheProvider cacheProvider, string cacheKey, object item, CacheSettings settings)
        {
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            if (context != null)
            {
                var courseId = !string.IsNullOrEmpty(context.CourseId) ? context.CourseId : null;
                var productCourseId = !string.IsNullOrEmpty(context.ProductCourseId) ? context.ProductCourseId : courseId;
                cacheProvider.Store(cacheKey, item, settings, "ProductCourse_" + productCourseId, "Course_" + courseId);
            }
            else
            {
                cacheProvider.Store(cacheKey, item, settings);
            }
        }

        public static void StoreByProductCourseRegion(this ICacheProvider cacheProvider, Dictionary<string, object> items, CacheSettings settings)
        {
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            if (context != null)
            {
                var courseId = !string.IsNullOrEmpty(context.CourseId) ? context.CourseId : null;
                var productCourseId = !string.IsNullOrEmpty(context.ProductCourseId) ? context.ProductCourseId : courseId;
                
                cacheProvider.Store(items, settings, "ProductCourse_" + productCourseId, "Course_" + courseId);    
                
            }
            else
            {
                 cacheProvider.Store(items, settings, "", "");    
            }
        }

        /// <summary>
        /// Stores an item associated with a course
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="cacheKey"></param>
        /// <param name="courseId"></param>
        /// <param name="item"></param>
        public static void StoreCourseItem(this ICacheProvider cacheProvider, string cacheKey, string courseId, object item)
        {
            cacheKey = string.Format("PX_COURSE_{0}_{1}", courseId, cacheKey);

            CacheSettings settings = new CacheSettings()
            {
                Aging = AgingMechanism.Sliding,
                Duration = 18000,
                Priority = CachePriority.High
            };

            cacheProvider.StoreByProductCourseRegion(cacheKey, item, settings);
        }
        /// <summary>
        /// Stores course item in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="course">Course item to store.</param>
        public static void StoreCourse(this ICacheProvider cacheProvider, Bdc.Course course)
        {
            if (course != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}", course.Id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.High
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, course, settings);
            }
        }

        /// <summary>
        /// Stores a List of courses in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="course">Course item to store.</param>
        public static void StoreCourseList(this ICacheProvider cacheProvider, List<Bdc.Course> courses, string ref_id)
        {
            if (courses != null)
            {
                string cacheKey = string.Format("COURSE_LIST_REF_{0}", ref_id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.High
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, courses, settings);
            }
        }

        /// <summary>
        /// Stores user item in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="user">User item to store.</param>
        public static void StoreUser(this ICacheProvider cacheProvider, Bdc.UserInfo user)
        {
            if (user != null)
            {
                string cacheKey = string.Format("PX_USER_{0}", user.Id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.Normal
                };
                cacheProvider.Store(cacheKey, user, settings);

                if (!user.Username.IsNullOrEmpty() && !user.DomainId.IsNullOrEmpty())
                {
                    cacheKey = string.Format("PX_USER_DOMAIN_{0}_REF_{1}", user.DomainId, user.Username);
                    cacheProvider.Store(cacheKey, user, settings);
                }
            }
        }

        /// <summary>
        /// Stores user item in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="user">User item to store.</param>
        public static void StoreUsersByReference(this ICacheProvider cacheProvider, string referenceId, List<Adc.AgilixUser> users) //***LMS - this is no longer unique
        {
            if (!users.IsNullOrEmpty() && !referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USERS_REF_{0}", referenceId);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.Normal
                };
                cacheProvider.Store(cacheKey, users, settings);
            }
        }

        /// <summary>
        /// Stores enrollment item in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="user">Enrollment item to store.</param>
        public static void StoreEnrollment(this ICacheProvider cacheProvider, Bdc.Enrollment enrollment)
        {
            if (enrollment != null)
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_{1}", enrollment.User.Id, enrollment.Id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 180000,
                    Priority = CachePriority.Low
                };

                cacheProvider.Store(cacheKey, enrollment, settings);

                if (enrollment.Course != null || enrollment.CourseId != null)
                {
                    string courseid = enrollment.Course != null ? enrollment.Course.Id : enrollment.CourseId;
                    cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_ENTITY_{1}", enrollment.User.Id, courseid);
                    cacheProvider.StoreByProductCourseRegion(cacheKey, enrollment, settings);
                }
            }
        }

        /// <summary>
        /// Stores enrollment item in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="user">Enrollment item to store.</param>
        public static void StoreUserEnrollmentList(this ICacheProvider cacheProvider, List<Bdc.Enrollment> enrollment, string refId)
        {
            if (enrollment != null)
            {
                string key = string.Format("PX_USER_{0}_ENROLLMENTS", refId);
                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.Low
                };

                cacheProvider.Store(key, enrollment, settings);
            }
        }

        /// <summary>
        /// Stores enrollment item in cache by reference id and course id.
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="enrollment"></param>
        /// <param name="refId"></param>
        /// <param name="productCourseId"></param>
        public static void StoreUserEnrollmentList(this ICacheProvider cacheProvider, List<Bdc.Enrollment> enrollment, string refId, string productCourseId, bool loadCourses, bool getEnrollmentCount)
        {
            if (enrollment != null)
            {
                string key = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", refId, productCourseId, loadCourses.ToString(), getEnrollmentCount.ToString());
                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.Normal
                };

                cacheProvider.Store(key, enrollment, settings);
            }
        }
        /// <summary>
        /// Stores page definition.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the page definition.</param>
        /// <param name="pageDefinition">Page definition object to cache.</param>
        public static void StorePageDefinition(this ICacheProvider cacheProvider, string courseId, Bdc.PageDefinition pageDefinition)
        {
            if (pageDefinition != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_PAGE_{1}", courseId, pageDefinition.Name);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 18000,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, pageDefinition, settings);
            }
        }

        /// Stores widget
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the page definition.</param>
        /// <param name="pageDefinition">Page definition object to cache.</param>
        public static void StoreWidget(this ICacheProvider cacheProvider, string courseId, Bdc.Widget widget)
        {
            if (widget != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_WIDGET_{1}", courseId, widget.Id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 1800,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, widget, settings);
            }
        }

        /// <summary>
        /// Stores menu in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the menu.</param>
        /// <param name="menu">Menu object to cache.</param>
        public static void StoreMenu(this ICacheProvider cacheProvider, string courseId, Bdc.Menu menu, bool showHiddenMenu = false)
        {
            if (menu != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_NAV_{1}_MENU_{2}", courseId, menu.Id, showHiddenMenu.ToString().ToUpper());
                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 1800,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, menu, settings);
            }
        }

        /// <summary>
        /// Stores user bookmarks in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the bookmarks.</param>
        /// <param name="bookmarks">User bookmarks collection to store.</param>
        public static void StoreQBASelectionList(this ICacheProvider cacheProvider, string courseId, Dictionary<string, string> chapterList, string listType)
        {
            if (chapterList != null)
            {
                string cacheKey = string.Format("PX_USER_QBA_Course_{0}_{1}", courseId, listType);
                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 600,
                    Priority = CachePriority.Low
                };
                cacheProvider.StoreByProductCourseRegion(cacheKey, chapterList, settings);
            }
        }

        /// <summary>
        /// Stores questions in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the bookmarks.</param>
        /// <param name="bookmarks">Questions collection to store.</param>
        public static void StoreQuestionsByCourse(this ICacheProvider cacheProvider, IEnumerable<Bdc.Question> questions)
        {
            if (!questions.IsNullOrEmpty())
            {
                foreach (Bdc.Question q in questions)
                {
                    cacheProvider.StoreQuestion(q);
                }
            }
        }

        /// <summary>
        /// Stores questions in cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the bookmarks.</param>
        /// <param name="bookmarks">Questions collection to store.</param>
        public static void StoreQuestion(this ICacheProvider cacheProvider, Bdc.Question question)
        {
            if (question != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_QUESTION_{1}", question.EntityId, question.Id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 600,
                    Priority = CachePriority.Low
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, question, settings);

            }
        }

       public static void StoreDashboardData(this ICacheProvider cacheProvider, Biz.DataContracts.DashboardData dashboardData, string courseId)
        {
            Bdc.DashboardData d = new Bdc.DashboardData();
            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_DASHBOARDATA_{0}", courseId);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 600,
                    Priority = CachePriority.Low
                };
                cacheProvider.StoreByProductCourseRegion(cacheKey, dashboardData, settings);
            }
        }

        public static void StoreLaunchPadData(this ICacheProvider cacheProvider, List<Bdc.ContentItem> items, string courseId)
        {
            if (!items.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_LAUNCHPAD_DATA_{0}", courseId);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 5000,
                    Priority = CachePriority.High
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, items, settings);
            }
        }
        /// <summary>
        /// Stores the item analysis by course.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="reports">The reports.</param>
        public static void StoreItemAnalysisReportByCourse(this ICacheProvider cacheProvider, IEnumerable<Bdc.ItemReport> reports)
        {
            if (!reports.IsNullOrEmpty())
            {
                foreach (Bdc.ItemReport report in reports)
                {
                    cacheProvider.StoreItemAnalysisReport(report);
                }
            }
        }

        /// <summary>
        /// Stores the item analysis.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="report">The report.</param>
        public static void StoreItemAnalysisReport(this ICacheProvider cacheProvider, Bdc.ItemReport report)
        {
            if (report != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_ANALYSISREPORTS", report.EntityId);
                string reportKey = string.Format("PX_COURSE_{0}_ITEMANALYSISREPORT_{1}", report.EntityId, report.ItemId);

                var reports = cacheProvider.FetchItemAnalysisReportByCourse(report.EntityId);

                if (reports != null)
                {
                    reports[reportKey] = report;
                }
                else
                {
                    reports = new Dictionary<string, Bdc.ItemReport>();
                    reports.Add(reportKey, report);
                }

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Sliding,
                    Duration = 600,
                    Priority = CachePriority.Low
                };
                cacheProvider.Store(cacheKey, reports, settings);

            }
        }

       
        public static object StoreRASiteUserData(this ICacheProvider cacheProvider, string userId, string foundCourseId, object SiteUserData)
        {
            var cacheKey = string.Format("{0}:{1}", userId, foundCourseId);
            cacheProvider.Store(cacheKey, SiteUserData, new CacheSettings
            {
                Aging = AgingMechanism.Sliding,
                Duration = 300,
                Priority = CachePriority.Normal
            });
            return SiteUserData;
        }

        public static object StoreRASiteInfo(this ICacheProvider cacheProvider, string url, object siteInfo)
        {
            var key = string.Format("BCTXT_SITEINFO_{0}", url);

            cacheProvider.Store(key, siteInfo, new CacheSettings
            {
                Aging = AgingMechanism.Static,
                Duration = 18000,
                Priority = CachePriority.High
            });
            return siteInfo;
        }

        public static Bdc.Domain StoreDomain(this ICacheProvider cacheProvider, string domainId, Bdc.Domain domain)
        {
            string cacheKey = string.Format("BIZCTX_{0}", domainId);
            string cacheKey2 = string.Format("BIZCTX_NAME_{0}", domain.Name);
            cacheProvider.Store(cacheKey, domain, new CacheSettings
            {
                Aging = AgingMechanism.Sliding,
                Duration = 18000,
                Priority = CachePriority.High
            });
            cacheProvider.Store(cacheKey2, domain, new CacheSettings
            {
                Aging = AgingMechanism.Sliding,
                Duration = 18000,
                Priority = CachePriority.High
            });
            return domain;
        }

        public static void StoreDomainList(this ICacheProvider cacheProvider, string parentDomainId, List<Bdc.Domain> domains)
        {
            string cacheKey = string.Format("BIZCTX_PARENT_DOMAIN_{0}", parentDomainId);

            cacheProvider.Store(cacheKey, domains, new CacheSettings
            {
                Aging = AgingMechanism.Sliding,
                Duration = 18000,
                Priority = CachePriority.High
            });
        }

        public static void StoreUserIdByReferenceId(this ICacheProvider cacheProvider, string domainUserReferenceId, string userId) //***LMS - this is no longer unique
        {
            string cacheKey = string.Format("BCUSERCACHE_{0}", domainUserReferenceId);
            cacheProvider.Store(cacheKey, userId, new CacheSettings
            {
                Aging = AgingMechanism.Sliding,
                Duration = 18000,
                Priority = CachePriority.Normal
            });			
        }

        public static void StoreFacetedSearchResults(this ICacheProvider cacheProvider, Bdc.SearchQuery bizQuery, FacetedSearchResults searchResults)
        {
            var key = "PX_COURSE_" + bizQuery.EntityId + "_FACETED_SEARCH_"
              + bizQuery.FacetedQuery.Fields.Reduce((a, b) => a + b, "")
              + bizQuery.FacetedQuery.Limit.ToString() +
                    bizQuery.FacetedQuery.MinCount.ToString();

            cacheProvider.Store(key, searchResults, new CacheSettings { Aging = AgingMechanism.Static, Duration = 3600 });
        }

        public static void StoreCssFile(this ICacheProvider cacheProvider, string url, string courseIdToReplace, string css)
        {
            var key = string.Format("PX_CSS_{0}", url);
            if (!courseIdToReplace.IsNullOrEmpty())
            {
                key = key.Replace(courseIdToReplace, string.Empty);
            }

            cacheProvider.Store(key, css,
                new Common.Caching.CacheSettings()
                {
                    Aging = Common.Caching.AgingMechanism.Sliding,
                    Duration = 1200,
                    Priority = Common.Caching.CachePriority.Normal
                });
        }
       
        public static string StoreWidgetCss(this ICacheProvider cacheProvider, string type, string css)
        {
            var key = "PX_WIDGET_CSS" + "_" + type;
        
            cacheProvider.Store(key, css,
                new Common.Caching.CacheSettings()
                {
                    Aging = Common.Caching.AgingMechanism.Sliding,
                    Duration = 1200,
                    Priority = Common.Caching.CachePriority.Normal
                });
            return css;
        }
        public static void StoreLearningObjectivePerformanceReport(this ICacheProvider cacheProvider, object data, string enrollmentId, string id)
        {
            if (data != null)
            {
                string cacheKey = string.Format("perf_report_{0}_{1}", enrollmentId, id);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Static,
                    Duration = 600,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, data, settings);
            }
        }

        public static void StoreRubricPerformanceReport(this ICacheProvider cacheProvider, object data, string enrollmentId, string itemId, string teamId)
        {
            if (data != null)
            {
                string cacheKey = string.Format("rubric_perf_report_{0}_{1}_{2}", enrollmentId, itemId, teamId);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Static,
                    Duration = 600,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, data, settings);
            }
        }

        public static void StoreResource(this ICacheProvider cacheProvider, Bdc.Resource resource, string entityId, string resourceUri)
        {
            if (resource != null)
            {
                string cacheKey = string.Format("PxResource_{0}_{1}", entityId, resourceUri);

                CacheSettings settings = new CacheSettings()
                {
                    Aging = AgingMechanism.Static,
                    Duration = 5000,
                    Priority = CachePriority.Normal
                };

                cacheProvider.StoreByProductCourseRegion(cacheKey, resource, settings);
            }
        }


        #endregion

        

        #region Cache Invalidate methods

        public static Object RemoveByProductCourseRegion(this ICacheProvider cacheProvider, string key)
        {
            object obj = null;
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            if (context != null)
            {
                var courseId = !string.IsNullOrEmpty(context.CourseId) ? context.CourseId : null;
                var productCourseId = !string.IsNullOrEmpty(context.ProductCourseId) ? context.ProductCourseId : courseId;
                obj = cacheProvider.Remove(key, "ProductCourse_" + productCourseId);
            }
            else
            {
                obj = cacheProvider.Remove(key);
            }
            return obj;
        }

        /// <summary>
        /// Removes QBA questions
        /// </summary>
        /// 

        public static object InvalidateQBAQuestionData(this ICacheProvider cacheProvider, string courseId)
        {
            object result = null;
            string Cont_ChapterList = "chapterList";
            string Cont_QuizList = "quizList";

            string cacheKey = string.Format("PX_USER_QBA_Course_{0}_{1}", courseId, Cont_QuizList);
            result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Dictionary<string, string>;

            cacheKey = string.Format("PX_USER_QBA_Course_{0}_{1}", courseId, Cont_ChapterList);
            result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Dictionary<string, string>;

            return result;
        }

        /// <summary>
        /// Removes course item and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="course">Course item to remove from cache.</param>
        /// <returns>Course item removed from cache, null if item not found.</returns>
        public static Bdc.Course InvalidateCourseContent(this ICacheProvider cacheProvider, Bdc.Course course)
        {
            Bdc.Course result = null;

            if (course != null)
            {
                if (!cacheProvider.Disabled)
                {
                    string cacheKey = string.Format("PX_COURSE_{0}", course.Id);
                    result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Bdc.Course;

                    var productCourseId = course.ProductCourseId;
                    if (productCourseId.IsNullOrEmpty())
                    {
                        var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
                        productCourseId = context.ProductCourseId;
                    }
                    cacheProvider.RemoveByTag("Course_" + course.Id, "ProductCourse_" + productCourseId);

                    //also invalidate eportfolio dashboard cache;
                    
                }
            }

            return result;
        }

        public static Bdc.Course InvalidateProductCourse(this ICacheProvider cacheProvider, Bdc.Course course)
        {
            Bdc.Course result = null;

            if (course != null)
            {
                if (!cacheProvider.Disabled)
                {
                    string cacheKey = string.Format("ProductCourse_{0}", course.Id);
                    cacheProvider.ClearRegion(cacheKey);
                }
            }

            return result;
        }

        /// <summary>
        /// Removed just the course data from the cache
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="courseId"></param>
        public static void InvalidateCourse(this ICacheProvider cacheProvider, string courseId)
        {
            string cacheKey = string.Format("PX_COURSE_{0}", courseId);
            cacheProvider.RemoveByProductCourseRegion(cacheKey);
        }


        /// <summary>
        /// Removes course list from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="course">Course item to store.</param>
        public static List<Bdc.Course> InvalidateCourseList(this ICacheProvider cacheProvider, string ref_id)
        {
            List<Bdc.Course> result = null;

            if (ref_id != null)
            {
                string cacheKey = string.Format("COURSE_LIST_REF_{0}", ref_id);
                result = cacheProvider.Remove(cacheKey) as List<Bdc.Course>;
            }
            return result;
        }

        /// <summary>
        /// Removes user item and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="user">User item to remove from cache.</param>
        /// <returns>User item removed from cache, null if item not found.</returns>
        public static Bdc.UserInfo InvalidateUser(this ICacheProvider cacheProvider, Bdc.UserInfo user)
        {
            Bdc.UserInfo result = null;

            if (user != null)
            {
                string cacheKey = string.Format("PX_USER_{0}", user.Id);
                result = cacheProvider.Remove(cacheKey) as Bdc.UserInfo;
            }

            return result;
        }

        /// <summary>
        /// Removes enrollment item and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="enrollment">Enrollment item to remove from cache.</param>
        /// <returns>Enrollment item removed from cache, null if item not found.</returns>
        public static Bdc.Enrollment InvalidateEnrollment(this ICacheProvider cacheProvider, Bdc.Enrollment enrollment)
        {
            Bdc.Enrollment result = null;

            if (enrollment != null)
            {
                var userId = enrollment.User != null ? enrollment.User.Id : null;
                var enrollmentId = enrollment.Id;
                string courseid = enrollment.CourseId ?? enrollment.Course.Id;

                result = InvalidateEnrollment(cacheProvider, userId, enrollmentId, courseid);
            }

            return result;
        }

        public static Bdc.Enrollment InvalidateEnrollment(this ICacheProvider cacheProvider, string userId, string enrollmentId,
            string courseid)
        {
            Bdc.Enrollment result = null;
            if (!enrollmentId.IsNullOrEmpty() && !userId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_{1}", userId, enrollmentId);
                result = cacheProvider.Remove(cacheKey) as Bdc.Enrollment;
            }

            if (!courseid.IsNullOrEmpty() && !userId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENT_ENTITY_{1}", userId, courseid);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Bdc.Enrollment;
            }
            return result;
        }

       
        /// <summary>
        /// Removes enrollment item and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="enrollment">Enrollment item to remove from cache.</param>
        /// <returns>Enrollment item removed from cache, null if item not found.</returns>
        public static List<Bdc.Enrollment> InvalidateUserEnrollmentList(this ICacheProvider cacheProvider, string referenceId) //***LMS - this is no longer unique
        {
            List<Bdc.Enrollment> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_{0}_ENROLLMENTS", referenceId);
                result = cacheProvider.Remove(cacheKey) as List<Bdc.Enrollment>;
            }
            return result;
        }

        /// <summary>
        /// Removes enrollment item and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static List<Bdc.Enrollment> InvalidateUserEnrollmentList(this ICacheProvider cacheProvider, string referenceId, string courseId) //***LMS - this is no longer unique
        {
            List<Bdc.Enrollment> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey1 = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", referenceId, courseId, true, true);
                string cacheKey2 = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", referenceId, courseId, true, false);
                string cacheKey3 = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", referenceId, courseId, false, true);
                string cacheKey4 = string.Format("PX_USER_{0}_COURSE_{1}_ENROLLMENTS_lc_{2}_ec_{3}", referenceId, courseId, false, false);
              
                cacheProvider.Remove(cacheKey1);
                cacheProvider.Remove(cacheKey2);
                cacheProvider.Remove(cacheKey3);
                cacheProvider.Remove(cacheKey4);
            }
            return result;
        }

        /// <summary>
        /// Removes widget object and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the page definition.</param>
        /// <param name="pageDefinition">PageDefinition object to remove from cache.</param>
        /// <returns>PageDefinition object removed from cache, null if object not found.</returns>
        public static Bdc.Widget InvalidateWidget(this ICacheProvider cacheProvider, string courseId, Bdc.Widget widget)
        {
            Bdc.Widget result = null;

            if (widget != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_WIDGET_{1}", courseId, widget.Id);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Bdc.Widget;
            }

            return result;
        }
        /// <summary>
        /// Removes page definition object and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the page definition.</param>
        /// <param name="pageDefinition">PageDefinition object to remove from cache.</param>
        /// <returns>PageDefinition object removed from cache, null if object not found.</returns>
        public static Bdc.PageDefinition InvalidatePageDefinition(this ICacheProvider cacheProvider, string courseId, Bdc.PageDefinition pageDefinition)
        {
            Bdc.PageDefinition result = null;

            if (pageDefinition != null)
            {
                result = InvalidatePageDefinition(cacheProvider, courseId, pageDefinition.Name);
            }

            return result;
        }

        /// <summary>
        /// Removes page definition object and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the page definition.</param>
        /// <param name="pageName">Name of the page definition to look for in cache.</param>        
        /// <returns>PageDefinition object removed from cache, null if object not found.</returns>
        public static Bdc.PageDefinition InvalidatePageDefinition(this ICacheProvider cacheProvider, string courseId, string pageName)
        {
            Bdc.PageDefinition result = null;

            if (!string.IsNullOrEmpty(courseId) && !string.IsNullOrEmpty(pageName))
            {
                string cacheKey = string.Format("PX_COURSE_{0}_PAGE_{1}", courseId, pageName);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Bdc.PageDefinition;
            }

            return result;
        }

        /// <summary>
        /// Removes menu object and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the menu.</param>
        /// <param name="menu">Menu object to remove from cache.</param>
        /// <returns>Menu object removed from cache, null if object not found.</returns>
        public static Bdc.Menu InvalidateMenu(this ICacheProvider cacheProvider, string courseId, Bdc.Menu menu, bool showHiddenMenu = false)
        {
            Bdc.Menu result = null;

            if (!string.IsNullOrEmpty(courseId) && menu != null)
            {
                result = InvalidateMenu(cacheProvider, courseId, menu.Id, showHiddenMenu: showHiddenMenu);
            }

            return result;
        }

        /// <summary>
        /// Removes menu object and all dependent objects from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the menu.</param>
        /// <param name="menuId">ID of the menu to remove from cache.</param>
        /// <returns>Menu object removed from cache, null if object not found.</returns>
        public static Bdc.Menu InvalidateMenu(this ICacheProvider cacheProvider, string courseId, string menuId, bool showHiddenMenu = false)
        {
            Bdc.Menu result = null;

            if (!string.IsNullOrEmpty(courseId) && !string.IsNullOrEmpty(menuId))
            {
                string cacheKey = string.Format("PX_COURSE_{0}_NAV_{1}_MENU_{2}", courseId, menuId, showHiddenMenu.ToString().ToUpper());
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Bdc.Menu;
            }

            return result;
        }

        /// <summary>
        /// Remove questions from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>        
        /// <param name="questions">Questions collection to remove.</param>
        public static void InvalidateQuestions(this ICacheProvider cacheProvider, IEnumerable<Bdc.Question> questions)
        {
            if (!questions.IsNullOrEmpty())
            {
                foreach (Bdc.Question q in questions)
                {
                    cacheProvider.InvalidateQuestion(q);
                }
            }
        }

        /// <summary>
        /// Remove question from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseId">ID of the course for the bookmarks.</param>
        /// <param name="bookmarks">Questions collection to store.</param>
        public static void InvalidateQuestion(this ICacheProvider cacheProvider, Bdc.Question question)
        {
            if (question != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_QUESTION_{1}", question.EntityId, question.Id);
                cacheProvider.RemoveByProductCourseRegion(cacheKey);
            }
        }

        /// <summary>
        /// Remove question from cache.
        /// </summary>
        /// <param name="cacheProvider">The cache provider instance.</param>
        /// <param name="courseIds">ID of the course for the bookmarks.</param>
        /// <param name="bookmarks">Questions collection to store.</param>
        public static void InvalidateQuestionFromQBA(this ICacheProvider cacheProvider, Bdc.Question question, List<string> courseIds)
        {
            if (question != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_QUESTION_{1}", question.EntityId, question.Id);
                cacheProvider.RemoveByProductCourseRegion(cacheKey, courseIds);
            }
        }

        public static void RemoveByProductCourseRegion(this ICacheProvider cacheProvider, string key, List<string> courseIds)
        {

            if (courseIds != null && courseIds.Any())
            {
                foreach (var courseId in courseIds)
                {
                    cacheProvider.Remove(key, "ProductCourse_" + courseId);
                }
            }
            else
            {
                cacheProvider.Remove(key);
            }
        }

        /// <summary>
        /// Invalidate LaunchPadData
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public static IEnumerable<Bdc.ContentItem> InvalidateLaunchPadData(this ICacheProvider cacheProvider, string courseId)
        {
            IEnumerable<Bdc.ContentItem> result = null;

            if (!courseId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_LAUNCHPAD_DATA_{0}", courseId);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as IEnumerable<Bdc.ContentItem>;
            }
            return result;
        }

        public static object InvalidateLearningObjectivePerformanceReport(this ICacheProvider cacheProvider, string enrollmentId, string id)
        {
            object result = null;

            if (!enrollmentId.IsNullOrEmpty() && !id.IsNullOrEmpty())
            {
                string cacheKey = string.Format("perf_report_{0}_{1}", enrollmentId, id);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey);
            }
            return result;
        }

        public static object InvalidateRubricPerformanceReport(this ICacheProvider cacheProvider, string enrollmentId, string itemId, string termId)
        {
            object result = null;

            if (!enrollmentId.IsNullOrEmpty() && !itemId.IsNullOrEmpty() && !termId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("rubric_perf_report_{0}_{1}_{2}", enrollmentId, itemId, termId);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey);
            }
            return result;
        }

        public static Adc.Resource InvalidateResource(this ICacheProvider cacheProvider, string entityId, string resourceUri)
        {
            Adc.Resource result = null;
            if (!entityId.IsNullOrEmpty() && !resourceUri.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PxResource_{0}_{1}", entityId, resourceUri);
                result = cacheProvider.RemoveByProductCourseRegion(cacheKey) as Adc.Resource;
            }
            return result;
        }
  
        /// <summary>
        /// Invalidate user by reference.
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="domainId"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static Bdc.UserInfo InvalidateUserByReference(this ICacheProvider cacheProvider, string domainId, string referenceId) //***LMS - this is no longer unique
        {
            Bdc.UserInfo result = null;

            if (!referenceId.IsNullOrEmpty() && !domainId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USER_DOMAIN_{0}_REF_{1}", domainId, referenceId);
                result = cacheProvider.Remove(cacheKey) as Bdc.UserInfo;
            }
            return result;
        }

        /// <summary>
        /// Invalidate user by reference.
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="domainUserReferenceId"></param>
        /// <returns></returns>
        public static Bdc.UserInfo InvalidateUserIdByReferenceId(this ICacheProvider cacheProvider, string domainUserReferenceId) 
        {
            Bdc.UserInfo result = null;

            if (!domainUserReferenceId.IsNullOrEmpty() && !domainUserReferenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("BCUSERCACHE_{0}", domainUserReferenceId);
                result = cacheProvider.Remove(cacheKey) as Bdc.UserInfo;
            }
            return result;
        }

        /// <summary>
        /// Invalidate users by reference
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static IEnumerable<Adc.AgilixUser> InvalidateUsersByReference(this ICacheProvider cacheProvider, string referenceId) //***LMS - this is no longer unique
        {
            IEnumerable<Adc.AgilixUser> result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_USERS_REF_{0}", referenceId);
                result = cacheProvider.Remove(cacheKey) as IEnumerable<Adc.AgilixUser>;
            }
            return result;
        }

        /// <summary>
        /// Invalidates the item analysis reports.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="reports">The reports.</param>
        public static void InvalidateItemAnalysisReports(this ICacheProvider cacheProvider, IEnumerable<Bdc.ItemReport> reports)
        {
            if (!reports.IsNullOrEmpty())
            {
                foreach (Bdc.ItemReport report in reports)
                {
                    cacheProvider.InvalidateItemAnalysisReport(report);
                }
            }
        }

        /// <summary>
        /// Invalidates the item analysis report.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="report">The report.</param>
        public static void InvalidateItemAnalysisReport(this ICacheProvider cacheProvider, Bdc.ItemReport report)
        {
            if (report != null)
            {
                string cacheKey = string.Format("PX_COURSE_{0}_ANALYSISREPORTS", report.EntityId);
                string reportKey = string.Format("PX_COURSE_{0}_ITEMANALYSISREPORT_{1}", report.EntityId, report.ItemId);

                var reports = cacheProvider.FetchItemAnalysisReportByCourse(report.EntityId);

                if (reports != null)
                {
                    reports.Remove(reportKey);
                }
            }
        }

        /// <summary>
        /// Invalidate users by reference
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static Bdc.DashboardData InvalidateLearningCurveDashboard(this ICacheProvider cacheProvider, string referenceId) //***LMS - this is no longer unique - courseid is coming in as referenceid
        {
            Bdc.DashboardData result = null;

            if (!referenceId.IsNullOrEmpty())
            {
                string cacheKey = string.Format("PX_DASHBOARDATA_{0}", referenceId);
                result = cacheProvider.Remove(cacheKey) as Bdc.DashboardData;
            }
            return result;
        }


        #endregion

    }
}