using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    /// <summary>
    /// Descriptor of metadata field
    /// </summary>
    public class ProductCourseSpecificMetadataFieldViewModel
    {
        /// <summary>
        /// Field display name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Field internal name
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// Field type
        /// </summary>
        public MetadataFieldType FieldType { get; set; }

        private IEnumerable<AvailableChoiceItem> valueOptions;

        /// <summary>
        /// Available values for field
        /// </summary>
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

        /// <summary>
        /// Display options of the field
        /// </summary>
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
