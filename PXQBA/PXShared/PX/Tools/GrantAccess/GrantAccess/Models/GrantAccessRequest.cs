using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrantAccess.Models
{
    public class GrantAccessRequest
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public string UserEmail { get; set; }
        public string CourseId { get; set; }
    }
}