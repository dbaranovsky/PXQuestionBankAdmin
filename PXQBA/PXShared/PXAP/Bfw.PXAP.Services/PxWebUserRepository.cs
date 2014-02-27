using System.Configuration;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PXAP.Services
{
	public class PxWebUserRepository : IPxWebUserRepository
	{
		private ISession Session { get; set; }
		private IApplicationContext Context { get; set; }

		public PxWebUserRepository(IApplicationContext context)
		{
			Context = context;

			// Configure the Service
			ConfigureServiceLocator();
			var sm = EstablishConnection();
			Session = sm.CurrentSession;
		}

		#region Implementation of IPXWebUserRightsRepository

		public IQueryable<PxWebUserModel> SearchPxWebUsers(PxWebUserModel pxWebUser, string actions)
		{
			string courseId = pxWebUser.CourseId;
			var domainId = GetDomainIdByCourseId(courseId);

			//GetUserList
			var cmdGetUsers = new GetUsers
						{
							SearchParameters = new UserSearch
							{
								Username = string.Format(pxWebUser.Email, "*"),
								DomainId = domainId
							}
						};

			Session.ExecuteAsAdmin(Context.Environment, cmdGetUsers);

			var pxWebUsers = cmdGetUsers.Users.Select(dlapUser => new PxWebUserModel
															{
																UserId = dlapUser.Reference,
																Email = dlapUser.Email,
																CourseId = courseId,
																Actions = actions
																//,WebUserRights = (PxWebUserRightsModel)new PxWebUserRights(dlapUser.Reference, courseId)
															}).AsQueryable();

			return pxWebUsers;

		}

		private string GetDomainIdByCourseId(string courseId)
		{
			//GetDomainIdByCourseId
			var cmdGetCourse = new GetCourse
								{
									SearchParameters = new CourseSearch { CourseId = courseId }
								};

			Session.ExecuteAsAdmin(Context.Environment, cmdGetCourse);
			string domainId = cmdGetCourse.Courses.Select(course => course.Domain.Id).First();
			return domainId;
		}

		public PxWebUserModel GetPxWebUser(string username)
		{
			return null;
		}

		public void SavePxWebUserRights(PxWebUserModel user)
		{

		}

		#endregion



		private static void ConfigureServiceLocator()
		{
			var locator = new UnityServiceLocator();
			locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
			locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
			ServiceLocator.SetLocatorProvider(() => locator);
		}

		private static ISessionManager EstablishConnection()
		{
			var username = ConfigurationManager.AppSettings["user"];
			var userid = ConfigurationManager.AppSettings["userid"];
			var password = ConfigurationManager.AppSettings["password"];
			var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
			sm.CurrentSession = sm.StartNewSession(username, password, true, string.Empty);
			return sm;
		}




	}

}
