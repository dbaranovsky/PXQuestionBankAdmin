using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class ReflectionAssignment : Assignment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectioAssignment"/> class.
        /// </summary>
        public ReflectionAssignment()
        {
            SubType = "ReflectionAssignment";
        }

        /// <summary>
        /// Gets or sets the student enrollment id.
        /// </summary>
        /// <value>
        /// The student enrollment id.
        /// </value>
        public string StudentEnrollmentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Category.
        /// </summary>
        /// <value>
        /// The applicable enrollment id.
        /// </value>
        public string SelectedCategory { get; set; }
    }
}
