using System.Collections;
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

        public IEnumerable<CourseMetadataFieldDescriptor> FieldDescriptors { get; set; } 

    }

    public static class CourseExtensions
    {
        public static IEnumerable<CourseMetadataFieldValue> GetChaptersList(this Course course)
        {
            var chapter = course.FieldDescriptors.SingleOrDefault(v => v.Name == MetadataFieldNames.Chapter);
            if (chapter == null)
            {
                return new List<CourseMetadataFieldValue>();
            }

            return chapter.CourseMetadataFieldValues;
        }
    }
}
