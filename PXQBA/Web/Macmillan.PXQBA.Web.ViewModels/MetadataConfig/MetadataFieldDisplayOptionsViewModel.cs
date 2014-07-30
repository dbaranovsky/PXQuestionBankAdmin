namespace Macmillan.PXQBA.Web.ViewModels.MetadataConfig
{
    /// <summary>
    /// Represents possible display options for metadata field
    /// </summary>
    public class MetadataFieldDisplayOptionsViewModel
    {
        /// <summary>
        /// Indicates if field can be used in filters
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// Indicates if field is displayed in banks
        /// </summary>
        public bool DisplayInBanks { get; set; }

        /// <summary>
        /// Indicates if field is shown in filter in banks
        /// </summary>
        public bool ShowFilterInBanks { get; set; }

        /// <summary>
        /// Indicates if search results match this field in question banks
        /// </summary>
        public bool MatchInBanks { get; set; }

        /// <summary>
        /// Indicates if field is displayed in current quiz
        /// </summary>
        public bool DisplayInCurrentQuiz { get; set; }

        /// <summary>
        /// Indicates if field is displayed in instructor quiz
        /// </summary>
        public bool DisplayInInstructorQuiz { get; set; }

        /// <summary>
        /// Indicates if field is displayed in resources
        /// </summary>
        public bool DisplayInResources { get; set; }

        /// <summary>
        /// Indicates if field should be shown in filter in resources
        /// </summary>
        public bool ShowFilterInResources { get; set; }

        /// <summary>
        /// Indicates if field should match search results in resources
        /// </summary>
        public bool MatchInResources { get; set; }
    }
}