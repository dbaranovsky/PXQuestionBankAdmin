using System;
using System.Text;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    public static class GridHelper
    {
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
    }
}