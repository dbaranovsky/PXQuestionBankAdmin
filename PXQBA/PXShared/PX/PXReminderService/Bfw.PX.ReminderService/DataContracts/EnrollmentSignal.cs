using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;

namespace Bfw.PX.ReminderService.DataContracts
{
    public class EnrollmentSignal
    {
        public Signal Signal { get; set; }
        public Enrollment Enrollment { get; set; }
        public AgilixUser Instructor { get; set; }
    }
}
