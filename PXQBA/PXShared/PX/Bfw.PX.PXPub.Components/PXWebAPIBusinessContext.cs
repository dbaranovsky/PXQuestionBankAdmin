using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.Common.SSO;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.Direct.Services;

namespace Bfw.PX.PXPub.Components
{
	public class PXWebApiBusinessContext : BusinessContextBase
	{
		/// <summary>
		/// bfw-user-id header key.
		/// </summary>
		private const string BfwAuthSession = "bfw-auth-session";

		/// <summary>
		/// bfw-user-id header key.
		/// </summary>
		private const string BfwUserId = "bfw-uid";

		/// <summary>
		/// bfw-dlap-header header key.
		/// </summary>
		private const string BfwDlapHeader = "bfw-dlap-header";

		/// <summary>
		/// bfw-bh-header header key.
		/// </summary>
		private const string BfwBhHeader = "bfw-bh-header";

		/// <summary>
		/// bfw-user-data header key.
		/// </summary>
		private const string BfwUserData = "bfw-user-data";

		/// <summary>
		/// bfw-user-data header key.
		/// </summary>
		private const string BfwSwitchToProtected = "bfw-projected-switch";

		/// <summary>
		/// Value is 1 if URL being accessed is a protected resource. Header is either missing
		/// or value is != 1 if resource being accessed is unprotected.
		/// </summary>
		private const string BfwProtected = "bfw-protected";


		/// <summary>
		/// Stores the raw level of access value as-is from RA
		/// </summary>
		private int RaLevelOfAccess { get; set; }

		public PXWebApiBusinessContext(ISessionManager sm, ILogger logger, ITraceManager tracer, ICacheProvider cacheProvider)
		{
			SessionManager = sm;
			Logger = logger;
			Tracer = tracer;
			CacheProvider = cacheProvider;
		}


		/// <summary>
		/// returns the Product Course ID of the URL passed in
		/// </summary>
		/// <param name="course"> </param>
		/// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
		/// <returns>Product Master Course Id for URL</returns>
        public override string GetProductCourseId(String course, string url)
        {
            using (Tracer.DoTrace("GetAgilixCourseID from RAWS"))
            {
                var siteInfo = GetSiteInfo(url);

                return siteInfo.AgilixSiteID;
            }
        }

		protected override void InitializeFromRequest()
		{
			using (Tracer.DoTrace("FormsAuthBusinessContext.InitializeFromRequest"))
			{
				var principal = System.Web.HttpContext.Current.User;

				if (principal != null && principal.Identity.IsAuthenticated)
				{
					var identity = principal.Identity;

					SSOData = new SSOData()
					{
						User = new SSOUser()
						{
							Email = string.Empty
						},
						UserId = identity.Name
					};
				}
				else
				{
					var message = "user is unauthenticated because: ";
					if (principal == null)
					{
						message += "principal is null";
					}
					else if (principal.Identity.IsAuthenticated == false)
					{
						message += "principal.Identity.IsAuthenticated is false";
					}
					else
					{
						message += "unknown reason";
					}

					Logger.Log(message, LogSeverity.Debug);
				}

				var url = GetCurrentURL();
				PopulateSiteInfo(url);
			}
		}

		protected override void InitializeUser()
		{
			using (Tracer.StartTrace("FormsAuthBusinessContext InitializeUser"))
			{
				IsAnonymous = ( SSOData == null || String.IsNullOrEmpty(SSOData.UserId) );

				if (IsAnonymous || CourseIsProductCourse)
				{
					using (Tracer.StartTrace("FormsAuthBusinessContext InitializeUser Anonymous Session"))
					{
						if (Course.CourseOwner.IsNullOrEmpty())
						{
							SessionManager.CurrentSession = SessionManager.StartAnnonymousSession();
						}
						else
						{
							SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
						}

						//set the IsPublicView property for a Presentation Course
						var currentUserId = CurrentUser == null ? string.Empty : CurrentUser.Id;
						SetPublicView(currentUserId);
					}
				}
				else
				{
					Logger.Log("FormsAuthBusinessContext Have RA User", LogSeverity.Debug);
					var domainReferenceId = string.Format("{0}/{1}", Domain.Id, SSOData.UserId);
					CacheUserId(domainReferenceId);

					using (Tracer.StartTrace(String.Format("FormsAuthBusinessContext StartSession for logged in user '{0}'", domainReferenceId)))
					{

						CurrentUser = ExistingUser(AdminConnection(AdminAccessType.RootAdmin), Course.Domain.Id, SSOData.UserId);

						if (CurrentUser != null)
						{
							string userName = string.Format("{0}/{1}", Domain.Userspace, CurrentUser.Username);

							var session = SessionManager.ResumeSession(userName, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
							if (session == null)
							{
								Logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                                session = SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
							}

							SessionManager.CurrentSession = session;

							Logger.Log(String.Format("Course domain ID is {0}", Course.Domain.Id), LogSeverity.Debug);

							if (CurrentUser != null)
							{
								session.UserId = CurrentUser.Id;
								CacheUserId(domainReferenceId, session.UserId);
								SetPublicView(CurrentUser.Id);
								if (IsPublicView)
								{
									SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
								}
							}
						}
						else
						{
							SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
						}
					}
				}

				// If we've not found a user, create one from the RA info.        
				if (CurrentUser == null && SSOData != null && SSOData.User != null)
				{
					CurrentUser = GetNewUserData();
				}
			}
		}

		protected override void InitializePermissions()
		{
			using (Tracer.StartTrace("BusinessContext InitializePermissions"))
			{
				var url = GetCurrentURL();
				GetSiteUserData(url);

				Logger.Log(string.Format("Level of Access is {0}", RaLevelOfAccess), LogSeverity.Debug);

				if (RaLevelOfAccess >= 70)
				{
					AccessLevel = AccessLevel.Instructor;
					AccessType = AccessType.Adopter;
					CanCreateCourse = true && ( EntityId == ProductCourseId );
				}
				else if (RaLevelOfAccess >= 40)
				{
					AccessLevel = AccessLevel.Instructor;
					AccessType = AccessType.Demo;
				}
				else if (RaLevelOfAccess >= 30)
				{
					AccessLevel = AccessLevel.Student;
					AccessType = AccessType.Adopter;
				}
				else if (RaLevelOfAccess >= 20)
				{
					AccessLevel = AccessLevel.Student;
					AccessType = AccessType.Basic;
				}
				else
				{
					AccessLevel = AccessLevel.None;
				}

				if (ImpersonateStudent)
				{
					AccessLevel = AccessLevel.Student;
					AccessType = AccessType.Adopter;
				}
			}
		}


		/// <summary>
		/// Given a domain id, set the current user object to be the user with the current ref ID, but
		/// in the given domain.
		/// </summary>
		/// <param name="domainId">The id of the domain the user is in.</param>
		public override void UpdateCurrentUser(string domainId)
		{
			UserInfo user = null;
            Logger.Log(String.Format("Updating current user to user with reference {0} in domain {1}.", CurrentUser.Username, domainId), LogSeverity.Debug);

            user = CacheProvider.FetchUserByReference(domainId, CurrentUser.Username);
			if (user == null)
			{
                var userCmd = new GetUsers() { SearchParameters = new Bfw.Agilix.DataContracts.UserSearch() { DomainId = domainId, ExternalId = CurrentUser.Username } };

				SessionManager.CurrentSession.ExecuteAsAdmin(userCmd);
				CurrentUser = userCmd.Users.FirstOrDefault().ToUserInfo();
				CacheProvider.StoreUser(CurrentUser);
			}
			else
			{
				CurrentUser = user;
                Logger.Log(string.Format("User with reference id {0} and domain {1} loaded from cache", CurrentUser.Username, CurrentUser.DomainId), LogSeverity.Debug);
			}
		}



		/// <summary>
		/// In this context Get PX User Info.
		/// </summary>
		/// <returns>UserInfo based on Px UserId</returns>
		public override UserInfo GetNewUserData()
		{
			UserInfo user = null;

			using (Tracer.StartTrace(String.Format("BusinessContext ExistingUser, parameters {0}", CurrentUser.Id)))
			{
				user = CacheProvider.FetchUser(CurrentUser.Id);
				if (user != null)
				{
					Logger.Log(string.Format("User with id {0} loaded from cache", CurrentUser.Id), LogSeverity.Debug);
				}
				else
				{
					var search = new GetUsers { SearchParameters = new Bfw.Agilix.DataContracts.UserSearch { Id = CurrentUser.Id } };

					var request = search.ToRequest();

					var connection = AdminConnection(AdminAccessType.RootAdmin);
					var response = connection.Send(request);

					search.ParseResponse(response);

					if (!search.Users.IsNullOrEmpty())
					{
						var agxUser = search.Users.First();
						user = agxUser.ToUserInfo();
						CacheProvider.StoreUser(user);
					}
				}
			}

			return user;
		}

		#region Helper Methods



		/// <summary>
		/// Sets all values of SSOData object that can be read from HTTP headers.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="data">The data.</param>
		protected void ExtractHeaderData(HttpRequest request, SSOData data)
		{
			bool testing = false;

			using (Tracer.DoTrace("ExtractHeaderData"))
			{
				if (request.QueryString["testing"] != null)
				{
					testing = true;
				}
				var expectedHeaders = new string[] { BfwSwitchToProtected, BfwProtected, BfwAuthSession, BfwUserId, BfwDlapHeader, BfwBhHeader, BfwUserData };

				var headers = new System.Collections.Specialized.NameValueCollection();
				if (testing)
				{
					//headers.Add(BfwSwitchToProtected, "true");
					//headers.Add(BfwProtected, "1");
					//headers.Add(BfwAuthSession, "IPCZQX03a36c6c0a=000002000a01fe8864a6d40142c20fb507ff33d8");
					//headers.Add(BfwSwitchToProtected, "1");
					//headers.Add(BfwUserId, "156");
					//headers.Add(BfwDlapHeader, "");
					//headers.Add(BfwBhHeader, "");
					//headers.Add(BfwUserData, "{\"LastName\":\"Instructor\",\"Email\":\"instructor@school.edu\",\"AgilixUsers\":[{\"Enrollments\":[{\"ID\":\"377\",\"CourseDomainID\":\"1\",\"CourseID\":\"357\"}],\"ID\":\"372\",\"Userspace\":\"root\",\"DomainID\":\"1\"}],\"FirstName\":\"Joe\",\"CustomerID\":\"123456\"}");
				}
				else
				{
					using (Tracer.DoTrace("Iterate Headers"))
					{
						foreach (var expected in expectedHeaders)
						{
							if (request.Headers.AllKeys.Contains(expected))
							{
								headers.Add(expected, request.Headers[expected]);
							}
						}
					}
				}

				using (Tracer.DoTrace("Set Headers if they Exist"))
				{
					if (headers.AllKeys.Contains(BfwAuthSession))
					{
						data.AuthSession = headers[BfwAuthSession];
					}
					if (headers.AllKeys.Contains(BfwSwitchToProtected))
					{
						data.SwitchToProtected = ( headers[BfwSwitchToProtected] == "true" || headers[BfwSwitchToProtected] == "1" ) ? true : false;
					}
					if (headers.AllKeys.Contains(BfwProtected))
					{
						data.IsProtected = ( headers[BfwProtected] == "1" ) ? true : false;
					}
					if (headers.AllKeys.Contains(BfwSwitchToProtected))
					{
						data.SwitchToProtected = ( headers[BfwSwitchToProtected] == "true" || headers[BfwSwitchToProtected] == "1" ) ? true : false;
					}
					if (headers.AllKeys.Contains(BfwUserId))
					{
						data.UserId = headers[BfwUserId];
					}
				}

				using (Tracer.DoTrace("Set Auth Headers for userid"))
				{
					if (!string.IsNullOrEmpty(data.UserId))
					{
						if (headers.AllKeys.Contains(BfwDlapHeader))
						{
							data.DlapAuth = headers[BfwDlapHeader];
						}

						if (headers.AllKeys.Contains(BfwBhHeader))
						{
							data.BrainHoneyAuth = request.Headers[BfwBhHeader];
						}

						if (headers.AllKeys.Contains(BfwUserData))
						{
							try
							{
								var jser = new JavaScriptSerializer();
								data.User = jser.Deserialize<SSOUser>(headers[BfwUserData]);
							}
							catch (Exception ex)
							{
								throw new Exception("Could not deserialize bfw-user-data JSON object", ex);
							}
						}
					}
				}

				using (Tracer.DoTrace("Determine Protected Status"))
				{
					var protectedResource = headers.AllKeys.Contains(BfwProtected);
					if (protectedResource && string.IsNullOrEmpty(data.UserId))
					{
						// If user is not authenticated and trying to access a protected resource, indicate that they should be redirected.
						data.SwitchToProtected = false;
						data.SwitchToUnprotected = true;
					}
					else if (!protectedResource && !string.IsNullOrEmpty(data.UserId))
					{
						// If user is authenticated and is access an unprotected resource, indicate that they should be redirected.
						data.SwitchToUnprotected = false;
						data.SwitchToProtected = true;
					}
					else
					{
						// User is unauthenticated and accessing a non-protected resource (which is ok and should not result in redirect)
						// OR
						// User is authenticated and accessing a protected resource (which is ok and should not result in redirect)
						data.SwitchToProtected = false;
						data.SwitchToProtected = false;
					}
				}

				using (Tracer.DoTrace("Log Headers"))
				{
					//LogHeaders(headers);
				}
			}
		}




		/// <summary>
		/// Gets any site info necessary from the external provider, in this case RA.
		/// </summary>
		private void PopulateSiteInfo(string url)
		{
			using (Tracer.DoTrace("FormsAuthBusinessContext.PopulateSiteInfo"))
			{
				var raws = new RAg.Net.RAWS.GetCourseSiteID.RAGetAgilixCourseIDSoapClient();

				using (Tracer.DoTrace("GetAgilixCourseID from RAWS"))
				{
					try
					{
						url = BusinessContextBase.AdjustForSubdomain(url);
						RAg.Net.RAWS.GetCourseSiteID.SiteInfo RawsSiteInfo = raws.GetAgilixCourseID(url);
						if (RawsSiteInfo != null)
						{
							SiteID = RawsSiteInfo.SiteID;
							URL = RawsSiteInfo.BaseURL;
							ProductCourseId = RawsSiteInfo.AgilixSiteID;
						}
					}
					catch (System.Exception ex)
					{
						Logger.Log(string.Format("GetSiteData failed: {0}", ex.Message), LogSeverity.Error);
						throw new Exception("Can not initialize businesscontext because GetSiteData failed", ex);
					}
				}
			}
		}

		/// <summary>
		/// Gets the correct user's entitlement based on the URL they are accessing
		/// </summary>
		/// <param name="url">URL the user is accessing</param>
		private void GetSiteUserData(string url)
		{
			using (Tracer.DoTrace("GetSiteUserData"))
			{
				if (SSOData != null && !string.IsNullOrEmpty(SSOData.UserId))
				{
					RAg.Net.RAWS.GetSiteUserData.SiteUserData RawsSiteUserData = null;

					const string exp = "/(?<courseid>[0-9]+)/?";
					var match = System.Text.RegularExpressions.Regex.Match(url, exp);
					string foundCourseId;
					if (match.Success)
					{
						foundCourseId = match.Groups["courseid"].Value;
					}
					else
					{
						match = System.Text.RegularExpressions.Regex.Match(System.Web.HttpContext.Current.Request.Url.ToString(), exp);
						foundCourseId = string.Empty;

						if (match.Success)
						{
							foundCourseId = match.Groups["courseid"].Value;
						}
					}

					if (string.IsNullOrEmpty(foundCourseId) && !string.IsNullOrEmpty(ProductCourseId))
					{
						foundCourseId = ProductCourseId;
					}


					using (Tracer.DoTrace("GetSiteUserData from RAWS"))
					{
						object cachedObject = null;

						if (!string.IsNullOrEmpty(foundCourseId))
						{
                            cachedObject = CacheProvider.FetchRASiteUserData(SSOData.UserId, foundCourseId);
						}

						if (cachedObject == null)
						{
                            using (Tracer.DoTrace("From Service {0}:{1}", SSOData.UserId, foundCourseId))
							{
								try
								{
									int siteId = 0;
									int.TryParse(SiteID, out siteId);
									var rawsSiteUserDataService = new RAg.Net.RAWS.GetSiteUserData.RAGetSiteUserDataSoapClient();
									RawsSiteUserData = rawsSiteUserDataService.GetSiteUserData(siteId, Convert.ToInt32(SSOData.UserId), System.Web.HttpContext.Current.Request.UserHostAddress);
								    CacheProvider.StoreRASiteUserData(SSOData.UserId, foundCourseId, RawsSiteUserData);
								}
								catch (System.Exception ex)
								{
									Logger.Log(string.Format("GetSiteUserData failed: {0} with URL", ex.Message, url), LogSeverity.Error);
								}
							}
						}
						else
						{
                            using (Tracer.DoTrace("From Cache {0}:{1}", SSOData.UserId, foundCourseId))
							{
								RawsSiteUserData = cachedObject as RAg.Net.RAWS.GetSiteUserData.SiteUserData;
							}
						}
					}

					if (RawsSiteUserData != null)
					{
						int intVal = 0;
						RaLevelOfAccess = 10;
						if (Int32.TryParse(RawsSiteUserData.LevelOfAccess, out intVal))
						{
							RaLevelOfAccess = intVal;
						}
					}
					else
					{
						Logger.Log("RawsSiteUserData is null", LogSeverity.Debug);
					}
				}
				else
				{
					Logger.Log("SSOData.UserId is null", LogSeverity.Debug);
				}
			}
		}

		private string GetCurrentURL()
		{
			var currentHttpContext = System.Web.HttpContext.Current;

			var urlHelper = new System.Web.Mvc.UrlHelper(currentHttpContext.Request.RequestContext);
			var uri = currentHttpContext.Request.Url;
			var route = string.Empty;

			var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(currentHttpContext));
			if (routeData != null)
			{
				object course = routeData.Values["course"];
				object section = routeData.Values["section"];

				if (course != null && section != null)
				{
					route = urlHelper.RouteUrl("ProductHome", new { course = course.ToString(), section = section.ToString() });
				}
			}

			var url = string.Format("http://{0}{1}", uri.Host, route);

			return url;


		}

		#endregion
	}
}
