using System;
using System.Text;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    /// <summary>
    /// Question list helper
    /// </summary>
    public static class GridHelper
    {
        /// <summary>
        /// Gets predefined list of metadata fields that should be shown in question list when first loaded
        /// </summary>
        /// <returns></returns>
        public static string GetInitialFieldSet()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(MetadataFieldNames.Bank).Append("+")
                         .Append(MetadataFieldNames.Sequence).Append("+")
                         .Append(MetadataFieldNames.DlapTitle).Append("+")
                         .Append(MetadataFieldNames.DlapType).Append("+")
                         .Append(MetadataFieldNames.QuestionStatus);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Calculates total number of pages in the list
        /// </summary>
        /// <param name="totalItems">Total number of items</param>
        /// <param name="itemPerPage">Number of items per page</param>
        /// <returns></returns>
        public static int GetTotalPages(int totalItems, int itemPerPage)
        {
            return (totalItems + itemPerPage - 1) / itemPerPage;
        }
    }
}