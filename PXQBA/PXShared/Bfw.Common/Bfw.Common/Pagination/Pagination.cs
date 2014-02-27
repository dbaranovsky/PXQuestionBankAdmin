using System;

namespace Bfw.Common.Pagination
{
    /// <summary>
    /// 
    /// </summary>
	[Serializable]
	public class Pagination
    {
        /// <summary>
        /// 
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPageByButton
        {
            set
            {
                CurrentPage = value;
            }
            get { return CurrentPage; }
        }

    	/// <summary>
    	/// 
    	/// </summary>
		public string PageSubmitFunction { get; set; }

		/// <summary>
		/// For Each Pagination always Use Unique Pagination Prefix 
		/// </summary>
		public string PaginationUniquePrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// 
        /// </summary>
		
		public PaginationOptions Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public Pagination()
        {
			SetDefaultPaginationOptions();
            CurrentPage = 1;
        }


		/// <summary>
		/// 
		/// </summary>
		private static void SetDefaultPaginationOptions()
		{
			if (!PaginationOptions.ShowNext.HasValue) PaginationOptions.ShowNext = true;
			if (!PaginationOptions.ShowLast.HasValue) PaginationOptions.ShowLast = true;
			if (!PaginationOptions.ShowPrevious.HasValue) PaginationOptions.ShowPrevious = true;
			if (!PaginationOptions.ShowFirst.HasValue) PaginationOptions.ShowFirst = true;

			if (!PaginationOptions.DisableFirstIfNoMorePages.HasValue) PaginationOptions.DisableFirstIfNoMorePages = true;
			if (!PaginationOptions.DisableLastIfNoMorePages.HasValue) PaginationOptions.DisableLastIfNoMorePages = true;
			if (!PaginationOptions.DisableNextIfNoMorePages.HasValue) PaginationOptions.DisableNextIfNoMorePages = true;
			if (!PaginationOptions.DisablePreviousIfNoMorePages.HasValue) PaginationOptions.DisablePreviousIfNoMorePages = true;
			if (!PaginationOptions.ShowDisplayingItemsLegend.HasValue) PaginationOptions.ShowDisplayingItemsLegend = true;
			if (!PaginationOptions.ShowDisplayingItemsAlways.HasValue) PaginationOptions.ShowDisplayingItemsAlways = true;

			if (!PaginationOptions.PageSize.HasValue) PaginationOptions.PageSize = 25;
			if (!PaginationOptions.PagesInPagination.HasValue) PaginationOptions.PagesInPagination = 25;

			if (string.IsNullOrEmpty(PaginationOptions.DisplayingItemsText)) PaginationOptions.DisplayingItemsText = "Displaying items {0} - {1} of {2}";
			if (string.IsNullOrEmpty(PaginationOptions.NextText)) PaginationOptions.NextText = ">";
			if (string.IsNullOrEmpty(PaginationOptions.LastText)) PaginationOptions.LastText = ">>";
			if (string.IsNullOrEmpty(PaginationOptions.PreviousText)) PaginationOptions.PreviousText = "<";
			if (string.IsNullOrEmpty(PaginationOptions.FirstText)) PaginationOptions.FirstText = "<<";

		}




        /// <summary>
        /// 
        /// </summary>
        public int Take
        {
            get { return PaginationOptions.PageSize.GetValueOrDefault(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Skip
        {
            get
            {
                int skip = (CurrentPage - 1) * PaginationOptions.PageSize.GetValueOrDefault();

                if (TotalItems == skip)
                {
                    skip = (CurrentPage - 2) * PaginationOptions.PageSize.GetValueOrDefault();

                    if (skip < 0)
                    {
                        skip = 0;
                    }
                }
                return skip;
            }
        }
    }
}
