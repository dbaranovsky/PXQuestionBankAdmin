using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    public class ProductCourseViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public IEnumerable<ChapterViewModel> Chapters { get; set; }

        public int QuestionsCount
        {
            get
            {
                if (Chapters == null)
                {
                    return 0;
                }
                return Chapters.Sum(c => c.QuestionsCount);
            }
            
        }

        public bool CanViewQuestionList { get; set; }
    }
}
