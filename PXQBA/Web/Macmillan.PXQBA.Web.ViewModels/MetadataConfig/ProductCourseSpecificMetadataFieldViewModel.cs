using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    public class ProductCourseSpecificMetadataFieldViewModel
    {
        public string FieldName { get; set; }

        public string InternalName { get; set; }

        public MetadataFieldType FieldType { get; set; }

        private IEnumerable<AvailableChoiceItem> valueOptions;

        public IEnumerable<AvailableChoiceItem> ValuesOptions
        {
            get
            {
                if (valueOptions == null)
                {
                    valueOptions = new List<AvailableChoiceItem>();
                }
                return valueOptions;
            }
            set
            {
                valueOptions = value;
            }
        }

        private MetadataFieldDisplayOptionsViewModel displayOptions;

        public MetadataFieldDisplayOptionsViewModel DisplayOptions
        {
            get
            {
                if (displayOptions == null)
                {
                    displayOptions = new MetadataFieldDisplayOptionsViewModel();
                }
                return displayOptions;
            }
            set
            {
                displayOptions = value;
            }
        }
    }
}
