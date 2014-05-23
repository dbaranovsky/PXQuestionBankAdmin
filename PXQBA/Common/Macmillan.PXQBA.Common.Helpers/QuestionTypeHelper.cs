using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class QuestionTypeHelper
    {
        private static readonly IEnumerable<QuestionType> AvailableTypes = ConfigurationHelper.GetQuestionTypes().Select(CreateQuestionType);
        public const string HTSType = "HTS";
        public const string GraphType = "FMA_GRAPH";


        public static string GetDisplayName(string key, string customUrl = null)
        {
            if (key == "custom")
            {
                return AvailableTypes.First(t => t.Key.ToUpper() == customUrl.ToUpper()).DisplayValue;
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

        public static QuestionType GetQuestionType(string key)
        {
            return AvailableTypes.First(t => t.Key == key);
        }

        public static IEnumerable<QuestionType> GetTypes()
        {
            return AvailableTypes;
        }
    }

    public class QuestionType
    {
        public string Key { get; set; }
        public string DisplayValue { get; set; }

        public string Custom { get; set; }
    }
}
