namespace Bfw.Common.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    public  class PaginationOptions
    {
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowNext { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowLast { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowPrevious { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowFirst { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static string NextText { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static string LastText { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static string PreviousText { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static string FirstText { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? DisablePreviousIfNoMorePages { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? DisableNextIfNoMorePages { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? DisableFirstIfNoMorePages { get; set; }


        /// <summary>
        /// 
        /// </summary>
		public static bool ? DisableLastIfNoMorePages { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowDisplayingItemsAlways { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static bool ? ShowDisplayingItemsLegend { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static string DisplayingItemsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static int ? PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public static int ? PagesInPagination { get; set; }

 
    }
}
