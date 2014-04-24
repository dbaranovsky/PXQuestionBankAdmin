using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class CourseHelper
    {
        private const string CourseParamName = "current_course";

        public static Course CurrentCourse
        {
            get { return HttpContext.Current.Session[CourseParamName] as Course; }
            set { HttpContext.Current.Session[CourseParamName] = value; }
        }
    }
}