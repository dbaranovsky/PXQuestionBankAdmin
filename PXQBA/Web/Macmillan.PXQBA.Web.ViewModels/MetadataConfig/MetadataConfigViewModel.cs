using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    public class MetadataConfigViewModel
    {

        public MetadataConfigViewModel()
        {
            AvailableFieldTypes =
                EnumHelper.GetEnumValues(typeof (MetadataFieldType))
                    .Select(p => new AvailableChoiceItem(p.Key, p.Value))
                    .ToList();
        }

        public string CourseId { get; set; }

        public string Chapters { get; set; }

        public string Banks { get; set; }

        public string QuestionCardLayout { get; set; }

        public IList<ProductCourseSpecificMetadataFieldViewModel> Fields { get; set; }

        public IList<AvailableChoiceItem> AvailableFieldTypes { get; set; }
    }
}
