using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class QuestionMetadataSection
    {

       
        public string ProductCourseId { get; set; }

        public string Title { get; set; }

        public string Bank { get; set; }

        public string Chapter { get; set; }
        public string Sequence { get; set; }

        private Dictionary<string, List<string>> dynamicValues;

        public Dictionary<string, List<string>> DynamicValues
        {
            get
            {
                if (dynamicValues == null)
                {
                    dynamicValues = new Dictionary<string, List<string>>();
                }
                return dynamicValues;
            }
            set
            {
                dynamicValues = value;
            }
        }

        public string QuestionIdDuplicateFromShared { get; set; }

        public string ParentProductCourseId { get; set; }

        public string DraftFromQuestionId { get; set; }
    }
}