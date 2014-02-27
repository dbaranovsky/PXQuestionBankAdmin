using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.ReminderService
{
    public interface IRAService
    {
        IEnumerable<KeyValuePair<string, string>> GetCourseList(IEnumerable<string> courses);
    }
}
