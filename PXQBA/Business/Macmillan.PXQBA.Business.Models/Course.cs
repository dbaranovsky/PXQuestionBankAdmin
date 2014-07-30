using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Course model
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Course title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Course id
        /// </summary>
        public string ProductCourseId { get; set; }

        /// <summary>
        /// Question bank repository id
        /// </summary>
        public string QuestionRepositoryCourseId { get; set; }

        /// <summary>
        /// Question card layout
        /// </summary>
        public string QuestionCardLayout { get; set; }

        /// <summary>
        /// Indicates if course is draft
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// List of question field descriptors
        /// </summary>
        public IEnumerable<CourseMetadataFieldDescriptor> FieldDescriptors { get; set; } 

    }

    /// <summary>
    /// Course extension methods
    /// </summary>
    public static class CourseExtensions
    {
        /// <summary>
        /// Builds chapters list for course
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>Chapters list</returns>
        public static IEnumerable<CourseMetadataFieldValue> GetChaptersList(this Course course)
        {
            var chapter = course.FieldDescriptors.SingleOrDefault(v => v.Name == MetadataFieldNames.Chapter);
            if (chapter == null)
            {
                return new List<CourseMetadataFieldValue>();
            }

            return chapter.CourseMetadataFieldValues;
        }

        /// <summary>
        /// Checks if field exists in the course
        /// </summary>
        /// <param name="course">Course</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Check result</returns>
        public static bool IsFieldExist(this Course course, string fieldName)
        {
            if (String.IsNullOrEmpty(fieldName))
            {
                return false;
            }

            if (course.FieldDescriptors == null)
            {
                return false;
            }

            return course.FieldDescriptors.Any(f => f.Name.ToUpper() == fieldName.ToUpper());
        }

        /// <summary>
        /// Checks if value of the field exists among all the field values
        /// </summary>
        /// <param name="course">Course</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value to check</param>
        /// <param name="itemLinkPatterm">Item link pattern</param>
        /// <returns>Check result</returns>
        public static bool IsValueExist(this Course course, string fieldName, string value, string itemLinkPatterm="{0}")
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }

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
