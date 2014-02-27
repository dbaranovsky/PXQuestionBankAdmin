using System.Text;
using System.Web.Mvc;


namespace Bfw.Common.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    public static class PaginationCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="paginationViewModel"></param>
        /// <returns></returns>
        public static MvcHtmlString Paging(this HtmlHelper html, Pagination paginationViewModel)
        {
            int totalPages = MathHelper.CalcTotalPages(paginationViewModel.TotalItems, PaginationOptions.PageSize.GetValueOrDefault());
            int previousPage = MathHelper.CalcPreviousPage(paginationViewModel.CurrentPage);
            int nextPage = MathHelper.CalcNextPage(paginationViewModel.CurrentPage, totalPages);
            int lowerBound = MathHelper.CalcLowerBound(paginationViewModel.CurrentPage, PaginationOptions.PagesInPagination.GetValueOrDefault(), totalPages);
			int upperBound = MathHelper.CalcUpperBound(paginationViewModel.CurrentPage, PaginationOptions.PagesInPagination.GetValueOrDefault(), totalPages);

            StringBuilder result = new StringBuilder();

            if (paginationViewModel.TotalItems != 0)
            {
            	var currentPage = paginationViewModel.CurrentPage;
				var paginationPrefix = paginationViewModel.PaginationUniquePrefix ?? "Pagination";
				
				CreateCurrentPageHidden(result, currentPage, paginationPrefix);

                if (PaginationOptions.ShowFirst.GetValueOrDefault())
                {
					CreateButton(result, PaginationOptions.FirstText, 1, paginationPrefix,
                        (PaginationOptions.DisableFirstIfNoMorePages.GetValueOrDefault() &&
						currentPage== 1 ), submitFunction: paginationViewModel.PageSubmitFunction);
                }

                if (PaginationOptions.ShowPrevious.GetValueOrDefault())
                {
					CreateButton(result, PaginationOptions.PreviousText, previousPage, paginationPrefix,
                        (PaginationOptions.DisablePreviousIfNoMorePages.GetValueOrDefault() &&
						currentPage == 1 ), submitFunction: paginationViewModel.PageSubmitFunction);
                }

                for (int i = lowerBound; i <= upperBound; i++)
                {
					if (i == paginationViewModel.CurrentPage)
						CreateButton(result, i.ToString(), i, paginationPrefix, true, true);
					else
						CreateButton(result, i.ToString(), i, paginationPrefix, submitFunction: paginationViewModel.PageSubmitFunction);
                }

                if (PaginationOptions.ShowNext.GetValueOrDefault())
                {
					CreateButton(result, PaginationOptions.NextText, nextPage, paginationPrefix,
                        (PaginationOptions.DisableNextIfNoMorePages.GetValueOrDefault() &&
						currentPage == totalPages ), submitFunction: paginationViewModel.PageSubmitFunction);
                }

                if (PaginationOptions.ShowLast.GetValueOrDefault())
                {
					CreateButton(result, PaginationOptions.LastText, totalPages, paginationPrefix,
                        (PaginationOptions.DisableLastIfNoMorePages.GetValueOrDefault() &&
						currentPage == totalPages ), submitFunction: paginationViewModel.PageSubmitFunction);
                }                
            }

            if (PaginationOptions.ShowDisplayingItemsLegend.GetValueOrDefault() &&
                (PaginationOptions.ShowDisplayingItemsAlways.GetValueOrDefault() || paginationViewModel.TotalItems != 0))
            {
                int firstItem = MathHelper.CalcFirstItem(paginationViewModel.CurrentPage, 
                    PaginationOptions.PageSize.GetValueOrDefault(),
                    paginationViewModel.TotalItems);
                int lastItem = MathHelper.CalcLastItem(paginationViewModel.CurrentPage, 
                    PaginationOptions.PageSize.GetValueOrDefault(),
                    paginationViewModel.TotalItems);
                var newSpan = "<span>" + PaginationOptions.DisplayingItemsText + "</span>";
                StringBuilder itemsText = new StringBuilder(string.Format(newSpan, firstItem,
            	              lastItem, paginationViewModel.TotalItems));

                itemsText.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
				result = itemsText.Append(result.ToString());
                
            }

            return MvcHtmlString.Create(result.ToString());
        }

        private static void CreateCurrentPageHidden(StringBuilder result, int currentPage, string paginationUniquePrefix)
        {
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttribute("type", "hidden");
            tagBuilder.MergeAttribute("name", string.Format("{0}.CurrentPage", paginationUniquePrefix));
			tagBuilder.MergeAttribute("id",   string.Format("{0}.CurrentPage", paginationUniquePrefix));
            tagBuilder.MergeAttribute("value", currentPage.ToString());
            result.Append(tagBuilder.ToString());
        }

		private static void CreateButton(StringBuilder result, string text, int gotoPage, string paginationUnuquePrefix, bool disabled = false, bool isCurrentPage = false, string submitFunction = null)
        {
        	TagBuilder tagBuilder = new TagBuilder("input");// {InnerHtml = text};
        	tagBuilder.MergeAttribute("type", "image");
			tagBuilder.MergeAttribute("id", string.Format("{0}.PageButton{1}", paginationUnuquePrefix,gotoPage));
			tagBuilder.MergeAttribute("alt", text);
			
            tagBuilder.MergeAttribute("value", gotoPage.ToString());

		    tagBuilder.AddCssClass("paginationLink");

			if (disabled)
            {
				tagBuilder.MergeAttribute("disabled", "disabled");
				if (!isCurrentPage) tagBuilder.AddCssClass("paginationLinkHidden");
            }

			if (!disabled & !string.IsNullOrEmpty(submitFunction)) tagBuilder.MergeAttribute("onClick", string.Format("{0}({1})",submitFunction, gotoPage));

            result.Append(tagBuilder.ToString());
        }
    }
}
