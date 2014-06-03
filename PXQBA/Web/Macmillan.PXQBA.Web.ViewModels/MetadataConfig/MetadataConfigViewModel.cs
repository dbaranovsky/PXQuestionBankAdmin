using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    public class MetadataConfigViewModel
    {
        public string CourseId { get; set; }

        public string Chapters { get; set; }

        public string Banks { get; set; }

        public IList<TitleSpecificMetadataField> Fields { get; set; }
    }
}
