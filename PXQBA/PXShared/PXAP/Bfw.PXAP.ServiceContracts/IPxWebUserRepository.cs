
using System.Linq;
using Bfw.PXAP.Models;


namespace Bfw.PXAP.ServiceContracts
{
	public interface IPxWebUserRepository
	{
		IQueryable<PxWebUserModel> SearchPxWebUsers(PxWebUserModel user, string actions = null);
		PxWebUserModel GetPxWebUser(string username);
		void SavePxWebUserRights(PxWebUserModel user);

	}
}

