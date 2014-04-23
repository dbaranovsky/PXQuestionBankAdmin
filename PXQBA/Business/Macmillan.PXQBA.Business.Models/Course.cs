using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class Course
    {
        public string Title { get; set; }
        public string ProductCourseId { get; set; }

        public IEnumerable<LearningObjective> LearningObjectives;

        public string QuestionCardLayout { get; set; }
    }
}
