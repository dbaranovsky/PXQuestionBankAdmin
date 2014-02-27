using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class AssignmentUnit
    {
        public string Title { get; set; }

        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string ParentId { get; set; }

        public bool Selected { get; set; }
    }
}