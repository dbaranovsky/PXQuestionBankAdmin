using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    public class Course
    {
        public string Title { get; set; }
        public string ProductCourseId { get; set; }

        public string QuestionRepositoryCourseId { get; set; }

        public string QuestionCardLayout { get; set; }

        public bool IsDraft { get; set; }

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

        public static bool IsFieldExist(this Course course, string fieldName)
        {
            if (course.FieldDescriptors == null)
            {
                return false;
            }

            return course.FieldDescriptors.Any(f => f.Name.ToUpper() == fieldName.ToUpper());
        }

        public static bool IsValueExist(this Course course, string fieldName, string value, string itemLinkPatterm="{0}")
        {
            if (course.FieldDescriptors == null)
            {
                return false;
            }

            var field = course.FieldDescriptors.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
            {
                return false;
            }

            if((field.Type== MetadataFieldType.Text)||(field.Type== MetadataFieldType.MultilineText))
            {
                return true;
            }

            if (field.Type == MetadataFieldType.ItemLink)
            {
                return field.CourseMetadataFieldValues.Select(v => String.Format(itemLinkPatterm, v.Id)).Any(l => l.ToUpper() == value.ToUpper());
            }

            return field.CourseMetadataFieldValues.Any(v => v.Text.ToUpper() == value.ToUpper());
        }
    }
}
