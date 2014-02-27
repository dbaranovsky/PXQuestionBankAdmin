using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Notes
{
    /// <summary>
    /// NSett
    /// </summary>
    public class NoteSettingEntity
    {
        public string StudentId{get;set;} 
        public string SharedStudentId {get;set;} 
        public string CourseId {get;set;}
        public string FirstNameSharer { get; set; }
        public string LastNameSharer { get; set; }
        public string FirstNameSharee { get; set; }
        public string LastNameSharee { get; set; } 
    }
}
