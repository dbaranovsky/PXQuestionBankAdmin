using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PxWebUser;

namespace Bfw.PXAP.Models
{
	public class PxWebUserModel
	{
		public List<ExternalMenuModel> ExternalMenuModel { get; set; }
		public List<MainMenuModel> MainMenuModel { get; set; }
		public List<MainMenuModel> MenuModel { get; set; }

		public string UserId { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Valid Email is required.")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Valid CourseId is required.")]
		[DataType(DataType.Text)]
		[Display(Name = "CourseId")]
		public string CourseId { get; set; }

		public PxWebUserRights WebUserRights { get; set; }

		public string Actions { get; set; }
	}
}
