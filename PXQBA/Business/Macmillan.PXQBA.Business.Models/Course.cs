using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    public class Course
    {
        public string Title { get; set; }
        public string ProductCourseId { get; set; }

        public IEnumerable<LearningObjective> LearningObjectives;

        public string QuestionCardLayout { get; set; }

        public IEnumerable<Chapter> Chapters { get; set; }

        public IEnumerable<string> Banks { get; set; }

        public int QuestionsCount
        {
            get
            {
                if (Chapters != null && Chapters.Any())
                {
                    return Chapters.Sum(ch => ch.QuestionsCount);
                }
                return 0;
            }
        }
    }
}
