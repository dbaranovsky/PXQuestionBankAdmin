using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class QuestionHelper
    {
        public static string GetQuestionHtmlPreview(string interactionData)
        {
            if (String.IsNullOrEmpty(interactionData))
            {
                return "failed convert";
            }

            return "Test";
        }

   }
}
