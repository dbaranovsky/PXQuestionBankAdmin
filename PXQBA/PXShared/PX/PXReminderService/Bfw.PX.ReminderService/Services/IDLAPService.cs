using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.PX.ReminderService.DataContracts;

namespace Bfw.PX.ReminderService
{
    public interface IDLAPService
    {
        List<Signal> GetSignalList(string lastSignalId, string signalType);

        List<EnrollmentSignal> GetDroppedEnrollmentList(List<Signal> signals);

        Course GetCourse(string courseId);

        Item GetContent(string entityId, string itemId);

        List<Sender> GetStudentList(string entityId);

        List<Item> GetPastDueItems(List<ReminderEmail> reminders);
    }
}
