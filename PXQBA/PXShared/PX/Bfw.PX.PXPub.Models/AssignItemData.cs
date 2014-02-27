using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class AssignItemData
    {
        public string itemId;
        public int dueYear;
        public int dueMonth;
        public int dueDay;
        public int dueHour;
        public int dueMinute;
        public string dueAmpm;
        public string behavior;
        public string completionTrigger;
        public string gradebookCategory;
        public string syllabusFilter;
        public int points;
        public string rubricId;
        public bool isGradeable;
        public bool isAllowLateSubmission;
        public bool isSendReminder;
        public int reminderDurationCount;
        public string reminderDurationType;
        public string reminderSubject;
        public string reminderBody;
        public int IncludeGbbScoreTrigger;
        public bool isHighlightLateSubmission;
        public bool isAllowLateGracePeriod;
        public long lateGraceDuration;
        public string lateGraceDurationType;
        public string CalculationTypeTrigger;
    }
}
