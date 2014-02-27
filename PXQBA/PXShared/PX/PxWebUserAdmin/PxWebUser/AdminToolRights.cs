namespace PxWebUser
{
	public class AdminToolAccess
	{

		public bool None
		{
			get { return adminRights.Is(enumAdminToolRights.None); }
			internal set
			{
				if (value) adminRights = adminRights.Remove(adminRights);
			}
		}

		public bool AllowEditSandboxCourse
		{
			get { return adminRights.Has(enumAdminToolRights.AllowEditSandboxCourse); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumAdminToolRights.AllowEditSandboxCourse) : adminRights.Remove(enumAdminToolRights.AllowEditSandboxCourse);
			}
		}

		public bool AllowPublishCourse
		{
			get { return adminRights.Has(enumAdminToolRights.AllowPublishCourse); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumAdminToolRights.AllowPublishCourse) : adminRights.Remove(enumAdminToolRights.AllowPublishCourse);
			}
		}

		public bool AllowRevertCourse
		{
			get { return adminRights.Has(enumAdminToolRights.AllowRevertCourse); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumAdminToolRights.AllowRevertCourse) : adminRights.Remove(enumAdminToolRights.AllowRevertCourse);
			}
		}

		internal enumAdminToolRights adminRights;

        internal AdminToolAccess(enumAdminToolRights rights)
		{
			adminRights = rights;
		}

	}
}
