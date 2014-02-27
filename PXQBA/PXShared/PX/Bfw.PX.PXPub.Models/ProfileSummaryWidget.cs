using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
	public class ProfileSummaryWidget
	{
		/// <summary>
		/// Top level error code
		/// </summary>
		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public string ProfileToEdit { get; set; }

		public UserInfo UserProfile { get; set; }

		public List<UserInfo> UsersSummaryList { get; set; }

		public Biz.ServiceContracts.AccessLevel AccessLevel { get; set; }

        public string CourseType { get; set; }
    }
}

