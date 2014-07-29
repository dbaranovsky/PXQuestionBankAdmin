using System;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Common.Helpers
{
    /// <summary>
    /// Helper that handles convertion of question types to string display values
    /// </summary>
    public static class QuestionTypeHelper
    {
        private static readonly IEnumerable<QuestionType> AvailableTypes = ConfigurationHelper.GetQuestionTypes().Select(CreateQuestionType);

        /// <summary>
        /// HTS question type name
        /// </summary>
        public const string HTSType = "HTS";

        /// <summary>
        /// Graph question type name
        /// </summary>
        public const string GraphType = "FMA_GRAPH";

        /// <summary>
        /// Gets display name for question type
        /// </summary>
        /// <param name="key">Type name</param>
        /// <param name="customUrl">Custom type name is passed here</param>
        /// <returns>Display name</returns>
        public static string GetDisplayName(string key, string customUrl = "")
        {
            if (key == "custom")
            {
                var questionType = AvailableTypes.FirstOrDefault(t => t.Key.ToUpper() == customUrl.ToUpper());
                if (questionType != null)
                {
                    return questionType.DisplayValue;
                }
                return string.Empty;
            }
            return AvailableTypes.First(t => t.Key == key).DisplayValue;
        }

        private static QuestionType CreateQuestionType(List<string> info)
        {
            return new QuestionType
                   {
                       Key = info[0],
                       DisplayValue = info[1],
                       Custom = info.Count > 2 && info[2] == "custom" ? info[2] : null
                   };
        }

        /// <summary>
        /// Get question type by name
        /// </summary>
        /// <param name="key">Type name</param>
        /// <returns>Question type</returns>
        public static QuestionType GetQuestionType(string key)
        {
            return AvailableTypes.First(t => t.Key == key);
        }

        /// <summary>
        /// Get all question types
        /// </summary>
        /// <returns>Question types list</returns>
        public static IEnumerable<QuestionType> GetTypes()
        {
            return AvailableTypes;
        }
    }

    /// <summary>
    /// Question type
    /// </summary>
    public class QuestionType
    {
        /// <summary>
        /// Question type name
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Custom name if question is custom
        /// </summary>
        public string Custom { get; set; }
    }
}
