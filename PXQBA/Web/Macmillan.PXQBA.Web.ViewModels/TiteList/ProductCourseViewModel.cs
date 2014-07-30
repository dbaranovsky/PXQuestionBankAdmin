using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    /// <summary>
    /// View model for product course 
    /// </summary>
    public class ProductCourseViewModel
    {
        /// <summary>
        /// Product course id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Product course title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Chapters list inside the course
        /// </summary>
        public IEnumerable<ChapterViewModel> Chapters { get; set; }

        /// <summary>
        /// Questions count per course
        /// </summary>
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

        /// <summary>
        /// Indicates if current user has capability to view question list for product course
        /// </summary>
        public bool CanViewQuestionList { get; set; }

        /// <summary>
        /// Indicates if product course is draft
        /// </summary>
        public bool IsDraft { get; set; }
    }
}
