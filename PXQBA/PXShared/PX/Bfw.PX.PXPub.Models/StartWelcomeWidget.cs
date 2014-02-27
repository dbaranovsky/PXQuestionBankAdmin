using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class StartWelcomeWidget
    {
        [Required(ErrorMessage = "Title is required!")]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Course number is required!")]
        [Display(Name = "Course number")]
        public string CourseNumber { get; set; }

        [Required(ErrorMessage = "Section number is required!")]
        [Display(Name = "Section number")]
        public string SectionNumber { get; set; }

        [Required(ErrorMessage = "Instructor name(s) is required!")]
        [Display(Name = "Instructor name(s)")]
        public string Instructor { get; set; }

        [Required(ErrorMessage = "Contents is required!")]
        [Display(Name = "Contents")]
        public string Contents { get; set; }
    }
}
