using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    public class MetadataConfigViewModel
    {

        public MetadataConfigViewModel()
        {
            AvailableFieldTypes = new List<AvailableChoiceItem>()
                                  {
                 new AvailableChoiceItem(((int)MetadataFieldType.Text).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.Text)),
                 new AvailableChoiceItem(((int)MetadataFieldType.SingleSelect).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.SingleSelect)),
                 new AvailableChoiceItem(((int)MetadataFieldType.MultilineText).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.MultilineText)),
                 new AvailableChoiceItem(((int)MetadataFieldType.MultiSelect).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.MultiSelect)),
                 new AvailableChoiceItem(((int)MetadataFieldType.Keywords).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.Keywords)),
                 new AvailableChoiceItem(((int)MetadataFieldType.ItemLink).ToString(), EnumHelper.GetEnumDescription(MetadataFieldType.ItemLink)),
                                  };
        }

        public string CourseId { get; set; }

        public string Chapters { get; set; }

        public string Banks { get; set; }

        public IList<TitleSpecificMetadataField> Fields { get; set; }

        public IList<AvailableChoiceItem> AvailableFieldTypes { get; set; }
    }
}
