using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
	public interface IAdminMetaDataActions
	{
		void UpdateResourceSubType(string newResourceSubType, List<ContentItem> items);

		void DeleteResourceSubType(List<ContentItem> items);

	}


}
