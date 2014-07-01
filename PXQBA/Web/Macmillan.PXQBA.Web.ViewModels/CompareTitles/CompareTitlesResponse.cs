using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.CompareTitles
{
    public class CompareTitlesResponse
    {
        public IList<ComparedQuestionViewModel> Questions { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public bool OneQuestionRepositrory { get; set; }
    }
}
