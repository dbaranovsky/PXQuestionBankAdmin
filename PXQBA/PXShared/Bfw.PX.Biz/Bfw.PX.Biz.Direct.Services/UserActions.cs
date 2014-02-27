using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Configuration;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.Biz.Direct.Services
{
	/// <summary>
	/// Implements IUserActions using direct connection to DLAP.
	/// </summary>
	public class UserActions : IUserActions
	{
		#region Properties

		/// <summary>
		/// Gets or sets the context.
		/// </summary>
		/// <value>
		/// The context.
		/// </value>
		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// Gets or sets the content actions.
		/// </summary>
		/// <value>
		/// The content actions.
		/// </value>
		protected BizSC.IContentActions ContentActions { get; set; }

		/// <summary>
		/// Gets or sets the note actions.
		/// </summary>
		/// <value>
		/// The note actions.
		/// </value>
		protected BizSC.INoteActions NoteActions { get; set; }

		/// <summary>
		/// Gets or sets the enrollment actions.
		/// </summary>
		/// <value>
		/// The enrollment actions.
		/// </value>
		protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected ISessionManager SessionManager { get; set; }


		/// <summary>
		/// 
		/// </summary>
		protected IDomainActions DomainActions { get; set; }


		#endregion

        private const string INSTRUCTOR_FLAG = "262144";

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActions"/> class.
		/// </summary>
		/// <param name="ctx">The IBusinessContext implementation.</param>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="contentActions"> </param>
		/// <param name="domainActions"> </param>
		public UserActions(IBusinessContext ctx, ISessionManager sessionManager, IContentActions contentActions, IDomainActions domainActions)
		{
			Context = ctx;
			SessionManager = sessionManager;
			ContentActions = contentActions;
			DomainActions = domainActions;
		}

		#endregion

		#region IUserActions Members

		/// <summary>
		/// Logs the user in as the anonymous user.
		/// </summary>
		/// <returns>A UserInfo object.</returns>
		public Bdc.UserInfo Login()
		{
			Bdc.UserInfo user = null;

			using (Context.Tracer.StartTrace("UserActions.Login Anonymous"))
			{
				SessionManager.CurrentSession = SessionManager.StartAnnonymousSession();

				var cmd = new GetUsers()
				{
					SearchParameters = new Adc.UserSearch()
					{
						Id = SessionManager.CurrentSession.UserId
					}
				};
				SessionManager.CurrentSession.Execute(cmd);

				user = cmd.Users.FirstOrDefault().ToUserInfo();
				Context.CurrentUser = user;
			}

			return user;
		}

		/// <summary>
		/// Uses the credentials specified to login the user.
		/// </summary>
		/// <param name="username">Username to use for login.</param>
		/// <param name="password">Password to use for login.</param>
		/// <returns>Userinfo object if successfully logged in, null otherwise.</returns>
		public Bdc.UserInfo Login(string username, string password)
		{
			Bdc.UserInfo user = null;

			using (Context.Tracer.StartTrace(string.Format("UserActions.Login(username={0}, password={1})", username, password)))
			{
				var cmd = new Login()
				{
					Username = username,
					Password = password
				};

				SessionManager.CurrentSession.Execute(cmd);

				user = cmd.User.ToUserInfo();

				Context.CurrentUser = user;
			}

			return user;
		}

		/// <summary>
		/// Logs out the currently logged in user.
		/// </summary>
		public void Logout()
		{
			using (Context.Tracer.StartTrace("UserActions.Logout"))
			{
				var logout = new DlapRequest()
				{
					Type = DlapRequestType.Get,
					Parameters = new Dictionary<string, object>() { { "cmd", "logout" } }
				};

				SessionManager.CurrentSession.Send(logout);
				SessionManager.EndSession(SessionManager.CurrentSession);
			}
		}

		/// <summary>
		/// Gets the currently logged in user.
		/// </summary>
		/// <returns>A UserInfo object representing the currently logged in user.</returns>
		public Bdc.UserInfo CurrentUser()
		{
			Bdc.UserInfo user = null;

			using (Context.Tracer.StartTrace("UserActions.CurrentUser"))
			{
				user = Context.CurrentUser;

				if (null == user)
				{
					if (null != SessionManager.CurrentSession)
					{
						user = GetUser(SessionManager.CurrentSession.UserId.ToString(CultureInfo.InvariantCulture));
						Context.CurrentUser = user;
					}
				}

				if (Context.IsAnonymous)
					user = null;
			}

			return user;
		}

		/// <summary>
		/// Gets the user with the specified ID.
		/// </summary>
		/// <param name="id">ID of the user to search for.</param>
		/// <returns>A userinfo object if user found with specified ID.</returns>
		public Bdc.UserInfo GetUser(string id)
		{
			Bdc.UserInfo user = null;
			using (Context.Tracer.DoTrace("UserActions.GetUser(id={0})", id))
			{
				user = Context.CacheProvider.FetchUser(id);
				if (user != null)
				{
					Context.Logger.Log(string.Format("User {0} loaded from cache", id), LogSeverity.Debug);
				}
				else
				{
					var users = ListUsersLike(new Bdc.UserInfo() { Id = id });
					user = users.FirstOrDefault();
					Context.CacheProvider.StoreUser(user);
				}
			}

			return user;
		}

        /// <summary>
        /// Gets the user with the specified ID.
        /// </summary>
        /// <param name="id">ID of the user to search for.</param>
        /// <returns>A userinfo object if user found with specified ID.</returns>
        public Bdc.UserInfo GetUserByRAId(string RAId)
        {
            Bdc.UserInfo user = null;
            using (Context.Tracer.DoTrace("UserActions.GetUser(RA Id={0})", RAId))
            {
                user = Context.CacheProvider.FetchUser(RAId);
                if (user != null)
                {
                    Context.Logger.Log(string.Format("User {0} loaded from cache", RAId), LogSeverity.Debug);
                }
                else
                {
                    List<UserInfo> users = null;

                    users = ListUsersLike(new Bdc.UserInfo() { Username = RAId }); 

                    user = users.FirstOrDefault();
                    Context.CacheProvider.StoreUser(user);
                }
            }

            return user;
        }

		public UserInfo GetUserByReferenceAndDomainId(string referenceId, string domainId)
		{
			Bdc.UserInfo user = null;
			using (Context.Tracer.DoTrace("UserActions.GetUserByReferenceAndDomainId(referenceid={0}, domainid)", referenceId, domainId))
			{
			    if (string.IsNullOrEmpty(referenceId) || string.IsNullOrEmpty(domainId))
			        return user;

		        user = new UserInfo() {DomainId = domainId};
                user.Username = Context.CurrentUser.Username;

			    var temp = ListUsersLike(user);
				if (temp != null && temp.Count > 0)
				{
					user = temp.FirstOrDefault();
				}
				else
				{
					Domain domain = new Domain();
					domain = DomainActions.GetDomainById(domainId);
					//add the user to the domain
					user = Context.GetNewUserData();
                    user = CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer,
						user.FirstName, user.LastName, user.Email, domainId, domain.Name,  string.Empty); //set reference id as blank since new user have no LMS id
                    Context.CacheProvider.InvalidateUsersByReference(user.Username);
				}
			}

			return user;
		}

		/// <summary>
		/// Gets a list of specific user ids from dlap
		/// Initial usage of method is to get the formatted name of specific users
		/// </summary>
		/// <param name="userIds">A list of userids</param>
		/// <returns>A list of UserInfo objects for the list of user ids</returns>
		public IList<UserInfo> ListUsers(IEnumerable<String> userIds)
		{
			IList<UserInfo> users = new List<UserInfo>();

			using (Context.Tracer.DoTrace("UserActions.ListUsers(userIds={0})", userIds))
			{
				// constructing batch command to return the list of users
				var batch = new Batch();
				foreach (var userId in userIds)
				{
					var cmd = new GetUsers()
					{
						SearchParameters = new Adc.UserSearch()
						{
							Id = userId
						}
					};
					batch.Add(cmd);
				}

				SessionManager.CurrentSession.ExecuteAsAdmin(batch);

				for (int index = 0; batch.Commands != null && index < batch.Commands.Count(); index++)
				{
					var cmd = batch.CommandAs<GetUsers>(index);
					var user = cmd.Users.FirstOrDefault();
					if (user != null)
					{
						users.Add(user.ToUserInfo());
					}
					//users = cmd.Users.Map(au => au.ToUserInfo()).ToList();
				}
			}

			return users;
		}

		/// <summary>
		/// Lists all users in the system.
		/// </summary>
		/// <returns>A list collection of UserInfo objects.</returns>
		public List<Bdc.UserInfo> ListUsers()
		{
			List<Bdc.UserInfo> users = null;

			using (Context.Tracer.StartTrace("UserActions.ListUsers"))
			{
				var cmd = new GetUsers()
				{
					SearchParameters = new Adc.UserSearch() { }
				};

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

				if (!cmd.Users.IsNullOrEmpty())
				{
					users = cmd.Users.Map(au => au.ToUserInfo()).ToList();
				}
			}

			return users;
		}

		/// <summary>
		/// Invalidate user
		/// </summary>
		/// <param name="userRefId"></param>
		public void invalidateUser(string userRefId)
		{
			Context.CacheProvider.InvalidateUserByReference(Context.Domain.Id, userRefId);
			Context.CacheProvider.InvalidateUsersByReference(userRefId);
		}

		/// <summary>
		/// Returns combined message from RA services for uploading images and updating profiles
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="userRefId">RA Refernce ID</param>
		/// <param name="email">User E-Mail</param>
		/// <param name="firstName">First Name</param>
		/// <param name="lastName">Last Name</param>
		/// <returns>RAWebServiceResponse Object</returns>
		public UserProfileResponse SetUserProfileInfo(string userId, string userRefId, string email, string firstName, string lastName)
		{

			RAServices raService = new RAServices();
			UserProfileResponse response = new UserProfileResponse();

			using (Context.Tracer.DoTrace("UserActions.SetUserProfileInfo(userId={0})", userId))
			{

				response.Error = new Error();
				try
				{
					response = raService.SetUserProfileInfo(userId, userRefId, email, firstName, lastName);
				}
				catch (Exception ex)
				{
					Context.Logger.Log(ex);
				}

				int profileErrorCode;
				int.TryParse(response.Error.Code, out profileErrorCode);

				// Storing Resource and updating user info in DLAP too.
				bool updated = UpdateUser(userId, Context.Domain.Id, Context.Domain.Name, string.Empty, firstName, lastName, email, userRefId);

				if (profileErrorCode != 0 && profileErrorCode != 300)
				{
					Context.Logger.Log(" Error: " + response.Error.Code + " Message: " + response.Error.Message, LogSeverity.Error);
				}
			}
			return response;
		}

		/// <summary>
		/// Sets an image or avatar for the user in RA
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="userRefId"> </param>
		/// <param name="fileType">File extention</param>
		/// <param name="file">File Stream</param>
		/// <returns>RAWebServiceResponse Object</returns>
		public UserProfileResponse SetUserProfileImage(string userRefId, string fileType, Stream file)
		{
			RAServices raService = new RAServices();
			UserProfileResponse response = new UserProfileResponse();
			response.Error = new Error();

			using (Context.Tracer.DoTrace("UserActions.SetUserProfileImage(userRefId={0})", userRefId))
			{

				if (file.Length > 0)
				{
					try
					{
						response = raService.SetUserProfileImage(userRefId, fileType, file);

					}
					catch (Exception ex)
					{
						Context.Logger.Log(ex);

					}

					int imageErrorCode;
					int.TryParse(response.Error.Code, out imageErrorCode);
					if (imageErrorCode != 0)
					{
						Context.Logger.Log(" Error: " + response.Error.Code + " Message: " + response.Error.Message, LogSeverity.Error);
					}
				}
				else
				{
					response.Error.Code = "0";
					response.Error.Message = "";
				}
			}

			return response;
		}

		/// <summary>
		/// Gets a list of users either from RA or from DLAP if RA service is not available
		/// </summary>
		/// <returns>RAWebServiceResponse Object</returns>
		public UserProfileResponse LoadUserProfile()
		{
			UserProfileResponse userRASummaryList = null;
			RAServices raServ = new RAServices();

			using (Context.Tracer.DoTrace("UserActions.LoadUserProfile"))
			{

				EnrollmentActions ea = new EnrollmentActions(Context, SessionManager, NoteActions, this, ContentActions);

				List<UserInfo> userSummaryList = ea.GetEntityEnrollments(Context.EntityId).Select(a => a.User).ToList(); //.Where(a => a.ReferenceId != "")

				// if webservice is down we catch exception
				try
				{
                    userRASummaryList =
                        raServ.GetUserProfile(userSummaryList.Where(a => a.Username != "" && a != null).ToList());
				}
				catch (Exception ex)
				{
					Context.Logger.Log(ex);
				}

				//// replace error records with DLAP user info if user has an error in RA response
				if (userRASummaryList.UserProfile != null)
				{
					int ee = 0;
					foreach (var userInfo in userRASummaryList.UserProfile)
					{
						int.TryParse(userInfo.Error.Code, out ee);
						if (ee > 0)
						{
						    Bdc.UserInfo ui = null;
							// if RA return user with error on userinfo level
                            ui = userSummaryList.FirstOrDefault(x => x.Username == userInfo.ReferenceId);						        

						    if (ui != null)
							{
								userInfo.UserId = ui.Id;
								userInfo.ReferenceId = ui.ReferenceId;
								userInfo.FirstName = ui.FirstName;
								userInfo.LastName = ui.LastName;
								userInfo.Email = ui.Email;
								userInfo.AvatarUrl = ui.AvatarUrl;
							}
						}
						else
						{
							//no error on userinfo level
						    Bdc.UserInfo ui = null;
                            ui = userSummaryList.FirstOrDefault(x => x.Username == userInfo.ReferenceId);

						    userInfo.UserId = ui.Id;
							userInfo.LastLogin = ui.LastLogin;

						}
					}
				}
				else  //there was an error communicating with the web service, fall back and get everything from dlap
				{

					userRASummaryList.UserProfile = new List<UserProfile>();

					foreach (var userInfo in userSummaryList)
					{
						Bdc.UserProfile up = new UserProfile();
						up.Email = userInfo.Email;
						up.FirstName = userInfo.FirstName;
						up.LastName = userInfo.LastName;
						up.UserId = userInfo.Id;
					    up.Username = userInfo.Username;
						up.ReferenceId = userInfo.ReferenceId;
						up.AvatarUrl = userInfo.AvatarUrl;
						up.LastLogin = userInfo.LastLogin;

						userRASummaryList.UserProfile.Add(up);
					}
				}
			}
			return userRASummaryList;
		}

        
		/// <summary>
		/// Lists users that match the search criteria.
		/// </summary>
		/// <param name="example"></param>
		/// <returns>A collection of UserInfo objects matching specified criteria.</returns>
		public List<Bdc.UserInfo> ListUsersLike(Bdc.UserInfo example)
		{
			List<Bdc.UserInfo> users = null;

			using (Context.Tracer.StartTrace("UserActions.ListUsersLike"))
			{
				var cmd = new GetUsers()
				{
					SearchParameters = new Adc.UserSearch()
					{
						Username = string.IsNullOrEmpty(example.Username)?null:example.Username,
						DomainId = string.IsNullOrEmpty(example.DomainId)?null:example.DomainId,
						Name = ( string.IsNullOrEmpty(example.FirstName) ? example.LastName : example.FirstName ),
						Id = string.IsNullOrEmpty(example.Id)?null:example.Id
					}
				};

                //external id assignment
                cmd.SearchParameters.ExternalId = example.Username;

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

				if (!cmd.Users.IsNullOrEmpty())
				{
					users = cmd.Users.Map(au => au.ToUserInfo()).ToList();
				}
			}

			return users;
		}

		/// <summary>
		/// Accepts list of UserSearch as parameter and returns List of List of UserInfo in batch execution mode.
		/// </summary>
		/// <param name="userSearchs">List of search criterias.</param>
		/// <param name="failures">List containing failure collections.</param>
		/// <returns>List of List of UserInfo objects.</returns>
		public List<List<Bdc.UserInfo>> ListUsersLike(List<Adc.UserSearch> userSearchs, out List<Adc.ItemStorageFailure> failures)
		{
			List<List<Bdc.UserInfo>> users = new List<List<Bdc.UserInfo>>();

			using (Context.Tracer.StartTrace("UserActions.ListUsersLike Batch UserSearch"))
			{
				var cmd = new GetUsersBatch()
				{
					SearchParameters = userSearchs
				};

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
				failures = cmd.Failures;

				int index = 0;
				foreach (var agxUserList in cmd.Users)
				{
					List<Bdc.UserInfo> batchUser = new List<Bdc.UserInfo>();
					if (!agxUserList.IsNullOrEmpty())
					{
						batchUser = agxUserList.Distinct((a, b) => a.Id == b.Id).Map(au => au.ToUserInfo()).ToList();
					}
					users.Add(batchUser);
					index++;
				}
			}

			return users;
		}

		/// <summary>
		/// Creates the user with the given information.
		/// </summary>
		/// <param name="username">Username of the new user account.</param>
		/// <param name="password">Password for the new account. There is no maximum length for this field.</param>
		/// <param name="pwdQuestion">Security question that end-user can respond to to reset their password. The maximum length is 256 characters.</param>
		/// <param name="pwdAnswer">Answer to passwordquestion. The maximum length is 128 characters.</param>
		/// <param name="firstName">User’s first (given) name. The maximum length is 256 characters.</param>
		/// <param name="lastName">User’s last (surname) name. The maximum length is 256 characters.</param>
		/// <param name="email">User’s e-mail address. The maximum length is 256 characters.</param>
		/// <param name="domainId">User’s domain ID.</param>
		/// <param name="domainName">User's domain name.</param>
		/// <param name="referenceId">User's external ID.</param>        
		/// <returns>UserInfo object for user created.</returns>
		public Bdc.UserInfo CreateUser(string username, string password, string pwdQuestion, string pwdAnswer, string firstName, string lastName, string email, string domainId, string domainName, string referenceId)
		{
			Bdc.UserInfo user = null;

			using (Context.Tracer.DoTrace("UserActions.CreateUser(username={0})", username))
			{
				var credentials = new Adc.Credentials() { Username = username, Password = password, PasswordQuestion = pwdQuestion, PasswordAnswer = pwdAnswer };
				var domain = new Adc.Domain() { Id = domainId, Name = domainName == string.Empty ? "root" : domainName };

				var cmd = new CreateUsers();

				cmd.Add(new Adc.AgilixUser()
				{
					FirstName = firstName,
					LastName = lastName,
					Email = email,
					Credentials = credentials,
                    UserName = username,
					Domain = domain,
					Reference = referenceId
				});

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

				user = cmd.Users.FirstOrDefault().ToUserInfo();
			}

			return user;
		}

		/// <summary>
		/// Creates users with the given information.
		/// </summary>
		/// <param name="users">List of users to create.</param>
		/// <returns></returns>
		public List<Bdc.UserInfo> CreateUsers(IList<Adc.AgilixUser> users)
		{
			List<Bdc.UserInfo> userInfoList = null;

			using (Context.Tracer.StartTrace("UserActions.CreateUsers(users)"))
			{
				var cmd = new CreateUsers();
				foreach (var user in users)
					cmd.Add(user);

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

				userInfoList = cmd.Users.Map(u => u.ToUserInfo()).ToList();
			}

			return userInfoList;
		}


		/// <summary>
		/// Updates the user with passed information.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <param name="domainId">The domain ID.</param>
		/// <param name="domainName">Name of the domain.</param>
		/// <param name="username">The username.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="email">The email.</param>
		/// <param name="referenceId">The reference ID.</param>
		/// <returns>True if user updated successfully.</returns>
		public bool UpdateUser(string userId, string domainId, string domainName, string username, string firstName, string lastName, string email, string referenceId)
		{
			bool updateUserResult = false;

			using (Context.Tracer.DoTrace("UserActions.UpdateUser(userId={0})", userId))
			{
				Adc.Credentials credentials = null;
				var domain = new Adc.Domain() { Id = domainId, Name = domainName == string.Empty ? "root" : domainName };

				if (!string.IsNullOrEmpty(username))
				{
					credentials = new Adc.Credentials() { Username = username };
				}

				var cmd = new UpdateUsers();
				cmd.Add(new Adc.AgilixUser()
				{
					Id = userId,
					FirstName = firstName,
					LastName = lastName,
					Email = email,
                    UserName = username,
					Credentials = credentials,
					Domain = domain,
					Reference = referenceId
				});

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

				updateUserResult = cmd.UpdateResult;

			    Context.CacheProvider.InvalidateUser(Context.CurrentUser);
                Context.CacheProvider.InvalidateUserByReference(domainId, Context.CurrentUser.Username);
                Context.CacheProvider.InvalidateUsersByReference(Context.CurrentUser.Username);
			}

			return updateUserResult;
		}

		/// <summary>
		/// Update the user info
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool UpdateUser(Bdc.UserInfo user)
		{
            return UpdateUsers(new List<Bdc.UserInfo>() { user });
		}

		/// <summary>
		/// Update users with the given information.
		/// </summary>
		/// <param name="users">List of users to update.</param>
		/// <returns></returns>
		public bool UpdateUsers(IList<Bdc.UserInfo> users)
		{
			bool updateUserResult = false;

			using (Context.Tracer.StartTrace("UserActions.UpdateUsers(users)"))
			{
				var cmd = new UpdateUsers();
				foreach (Bdc.UserInfo user in users)
				{
					cmd.Add(user.ToUserInfo());
				}

				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
				updateUserResult = cmd.UpdateResult;
			    foreach (var userInfo in users)
			    {
                    var domainReferenceId = string.Format("{0}/{1}", userInfo.DomainId, userInfo.Username);

                    Context.CacheProvider.InvalidateUserEnrollmentList(userInfo.Username);
                    Context.CacheProvider.InvalidateUserByReference(userInfo.DomainId, userInfo.Username);
                    Context.CacheProvider.InvalidateUser(userInfo);
                    Context.CacheProvider.InvalidateUsersByReference(userInfo.Username);
                    Context.CacheProvider.InvalidateUserIdByReferenceId(domainReferenceId);
			    }
			}
			return updateUserResult;
		}

		/// <summary>
		/// Gets the List of Instructors associated with a program
		/// </summary>
		/// <param name="search">search</param>
		/// <returns></returns>
		public IEnumerable<UserInfo> ListProgramInstructors(UserProgramSerach search)
		{
			var db = new DatabaseManager("PXData");
            List<UserInfo> instructorsInfo = new List<UserInfo>();

			using (Context.Tracer.DoTrace("UserActions.ListProgramInstructors(search.ManagerId={0},search.UserDomainId={1})", search.ManagerId, search.UserDomainId))
			{
				try
				{
					db.StartSession();
					var records = db.Query("GetProgramInstructors @0, @1", search.ManagerId, search.UserDomainId);
					if (!records.IsNullOrEmpty())
					{
					    foreach (var record in records)
					    {
					        UserInfo userInfo;
					        try
					        {
					            userInfo = GetUser(record.String("User_id"));
					        }
					        catch (Exception e)
					        {
					            Context.Logger.Log(string.Format("UserActions.ListProgramInstructors: Error getting user info, UserId={0}", record.String("User_id")), Bfw.Common.Logging.LogSeverity.Error);
					            continue;
					        }
					        instructorsInfo.Add(userInfo);
					    }
					}
				}
				finally
				{
					db.EndSession();
				}
			    
			}
            return instructorsInfo;
		}

		public IEnumerable<Course> FindCoursesByInstructor(String application_type, string instructorId, string domainId, string academicTerm, string parentId)
		{
			using (Context.Tracer.StartTrace("UserActions.FindCourseByInstructor"))
			{
                string productCourseId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.ProductCourseId;
				
				//no get the enrollments
				var enrollmentsCmd = new GetUserEnrollmentList()
				{
					SearchParameters = new Adc.EntitySearch()
					{
						UserId = instructorId,
                        Query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type='{1}' AND /meta-product-course-id='{2}'", academicTerm, application_type, productCourseId)
					}
				};

                SessionManager.CurrentSession.ExecuteAsAdmin(enrollmentsCmd);

                var enrollments = enrollmentsCmd.Enrollments.Filter(c => c.Domain.Id == domainId && c.Flags.HasFlag(DlapRights.SubmitFinalGrade));
                
                List<Course> courseList = enrollments.Map(c => c.Course.ToCourse()).ToList();

				return courseList;
			}

		}

        /// <summary>
        /// Get all instructors in the domain in any academic Term
        /// </summary>
        /// <param name="application_type">course type</param>
        /// <param name="domainId">domain id</param>
        /// <returns></returns>
        public IEnumerable<UserInfo> ListInstructorsForDomain(string application_type, string domainId)
        {
            List<UserInfo> users = ListUsersLike(new UserInfo { DomainId = domainId});

            List<UserInfo> instructors = new List<UserInfo>();
            if(users.IsNullOrEmpty())
                return instructors;

            EnrollmentActions ea = new EnrollmentActions(Context, SessionManager, NoteActions, this, ContentActions);
            var usersEnrollments = ea.ListUsersEnrollments(users.Select(u => u.Id), INSTRUCTOR_FLAG, "/meta-bfw_course_type='" + CourseType.Eportfolio.ToString() + "'");

            foreach (UserInfo user in users)
            {
                bool hasEnrolled = usersEnrollments.Find(i => i.User.Id == user.Id) != null;
                if (hasEnrolled)
                    instructors.Add(user);
            }
            return instructors;
        }

        /// <summary>
        /// Get all instructors in the domain
        /// </summary>
        /// <param name="application_type">course type</param>
        /// <param name="domainId">domain id</param>
        /// <param name="academicTerm">academic term</param>
        /// <returns></returns>
		public IEnumerable<UserInfo> ListInstructorsForDomain(String application_type, string domainId, string academicTerm)
		{
			var batch = new Batch();

			var instructors = new List<UserInfo>();

			using (Context.Tracer.StartTrace("UserActions.ListInstructorsForDomain"))
			{
				//get all the courses first for the domain/term selected
				var cmdCourses = new GetCourse()
				{
					SearchParameters = new Adc.CourseSearch()
					{
						DomainId = domainId,
						Query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'", academicTerm, application_type)
					}
				};
				SessionManager.CurrentSession.ExecuteAsAdmin(cmdCourses);

				if (cmdCourses.Courses != null)
				{
					foreach (Adc.Course c in cmdCourses.Courses.Filter(c => c.Domain.Id == domainId))
					{
						batch.Add(new GetEntityEnrollmentList()
						{
							SearchParameters = new Adc.EntitySearch()
							{
								CourseId = c.Id
							}
						});
					}
					if (!batch.Commands.IsNullOrEmpty())
					{
						SessionManager.CurrentSession.ExecuteAsAdmin(batch);
					}
					var enrollmentUserId = new List<Adc.Enrollment>();
					for (int ord = 0; ord < batch.Commands.Count(); ord++)
					{
                        enrollmentUserId.AddRange(batch.CommandAs<GetEntityEnrollmentList>(ord).Enrollments.Where(u => (u.User != null && u.User.Id != PxMigrationUserId && u.Flags.HasFlag(DlapRights.SubmitFinalGrade))));
					}

					if (!enrollmentUserId.IsNullOrEmpty())
					{
						foreach (Adc.Enrollment e in enrollmentUserId)
						{
							if (!instructors.Exists(i => i.Id == e.User.Id))
							{
								instructors.Add(e.User.ToUserInfo());
							}
						}
					}
				}


			}

			return instructors;
		}

		/// <summary>
		/// Gets the User Profile Image URL. The profile image stored in BrainHoney can be of various types (jpeg, gif, png etc.)
		/// So, the image with the latest modified date is supposed to be the the latest profile image.
		/// </summary>
		/// <param name="UserProfile"></param>
		/// <returns></returns>
		public string GetUserAvatarUrl(UserInfo UserProfile)
		{
			var avatarUrl = string.Empty;
			var resourceUri = string.Format("{0}/{1}*", ConfigurationManager.AppSettings["ProfileImageFolder"], UserProfile.Id);
			var resourceList = new GetResourceList()
			{
				EntityId = ConfigurationManager.AppSettings["ProfileImageEntity"],
				ResourcePath = resourceUri
			};

			SessionManager.CurrentSession.ExecuteAsAdmin(resourceList);

			if (!resourceList.Resources.IsNullOrEmpty())
			{
				var resourceInfo = resourceList.Resources.OrderByDescending(r => r.ModifiedDate).First();
				avatarUrl = string.Format("{0}{1}", ConfigurationManager.AppSettings["ProfileImagePath"], resourceInfo.Url);
			}

			return avatarUrl;
		}

        /// <summary>
        /// This is used only in intergration tests.
        /// </summary>
        /// <param name="user"></param>
        void IUserActions.DeleteUsers(params Bdc.UserInfo[] users)
        {
            using (Context.Tracer.StartTrace("CourseActions.DeleteUser()"))
            {
                var cmd = new DeleteUsers();
                foreach (var user in users)
                {
                    cmd.Add(user.ToUserInfo());
                }
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        /// <summary>
        /// Returns the pxmigration/user user ID, which is "7" across DLAP
        /// </summary>
        public string PxMigrationUserId
        {
            get { return "7"; }
        }

		#endregion



    }
}
