namespace Bfw.PXWebAPI.Mappers
{

	/// <summary>
	/// SearchResultDocMapper
	/// </summary>
	public static class SearchResultDocMapper
	{
		/// <summary>
		/// Converts to a Result Doc from a Biz Result Doc.
		/// </summary>
		/// <param name="biz">The biz.</param>
		/// <returns></returns>
		public static PX.PXPub.Models.SearchResultDoc ToSearchResultDoc(this Bfw.PX.Biz.DataContracts.SearchResultDoc biz)
		{
            var model = new PX.PXPub.Models.SearchResultDoc
							{
								itemid = biz.itemid.Replace("Shortcut__1__", ""),
								dlap_class = biz.dlap_class,
								dlap_hiddenfromstudent = biz.dlap_hiddenfromstudent,
								dlap_id = biz.dlap_id,
								dlap_itemtype = biz.dlap_itemtype,
								dlap_contenttype = biz.dlap_contenttype,
								dlap_subtitle = biz.dlap_subtitle,
								dlap_title = biz.dlap_title,
								doc_class = biz.doc_class,
								entityid = biz.entityid,
								score = biz.score,
								Url = biz.url,
								dlap_html_text = biz.dlap_html_text,
								dlap_text = biz.dlap_text,
								Metadata = biz.Metadata
							};


			return model;
		}

	}


}
