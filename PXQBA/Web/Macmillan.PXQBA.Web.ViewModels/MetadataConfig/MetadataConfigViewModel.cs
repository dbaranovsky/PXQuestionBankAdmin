using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    /// <summary>
    /// View model for metadata config page
    /// </summary>
    public class MetadataConfigViewModel
    {

        public MetadataConfigViewModel()
        {
            AvailableFieldTypes =
                EnumHelper.GetEnumValues(typeof (MetadataFieldType))
                    .Select(p => new AvailableChoiceItem(p.Key, p.Value))
                    .ToList();
        }

        /// <summary>
        /// Course id metadata config is modified for
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Chapters entered for current course
        /// </summary>
        public string Chapters { get; set; }

        /// <summary>
        /// Banks configured for current course
        /// </summary>
        public string Banks { get; set; }

        /// <summary>
        /// Question card layout configured for current course
        /// </summary>
        public string QuestionCardLayout { get; set; }

        /// <summary>
        /// List of metadata fields configured for current course
        /// </summary>
        public IList<ProductCourseSpecificMetadataFieldViewModel> Fields { get; set; }

        /// <summary>
        /// List of available field types to configure fields
        /// </summary>
        public IList<AvailableChoiceItem> AvailableFieldTypes { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit values
        /// </summary>
        public bool CanEditMetadataValues { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit question card layout
        /// </summary>
        public bool CanEditQuestionCardTemplate { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit title metadata in reduced way
        /// </summary>
        public bool CanEditTitleMetadataReduced { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit title metadata in full way
        /// </summary>
        public bool CanEditTitleMetadataFull { get; set; }
    }
}
