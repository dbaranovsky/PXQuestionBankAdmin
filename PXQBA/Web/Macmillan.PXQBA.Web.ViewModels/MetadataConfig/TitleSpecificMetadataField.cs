using System;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    public class TitleSpecificMetadataField
    {
        public string FieldName { get; set; }

        public string InternalName { get; set; }

        public MetadataFieldType FieldType { get; set; }


        //Stubs for (QBA-73, BA-28, QBA-68, QBA-219, QBA-61, QBA-50, QBA-40)
        public Object ValuesOptions { get; set; }

        public Object DisplayOptions { get; set; }
    }
}
