using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Determines the user's level of access in the system for a specific course.
    /// </summary>
    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Instructor = 2,
        Student = 4
    }
    public enum AccessType
    {
        Anonymous,
        Adopter, // Adopted Instructor
        Demo,     // Basic - Sample Instructor
        Premium, // Premium Student 
        Basic // Basic Student 

    }

    public enum CourseAccessType
    {
        Owner = 0,
        ReadOnly = 1
    }
}
