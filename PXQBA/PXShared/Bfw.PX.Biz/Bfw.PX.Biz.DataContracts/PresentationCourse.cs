using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a presentation course. 
    /// </summary>
    public class PresentationCourse
    {
        public Course Course { get; set; }
        public string Shared { get; set; }
        public int DocumentsCount { get; set; }
    }
}
