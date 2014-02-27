using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class PxUnit : LessonBase
    {
        /// <summary>
        /// The default value for a PxUnit's meta-subcontainer property.
        /// </summary>
        public const string DEFAULT_SUBCONTAINER = "PX_MULTIPART_LESSONS";

        /// <summary>
        /// Not sure what this means
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PxUnit"/> class.
        /// </summary>
        public PxUnit()
        {
            Type = "PxUnit";
            TrackMinutesSpent = false;
        }
    }
}
