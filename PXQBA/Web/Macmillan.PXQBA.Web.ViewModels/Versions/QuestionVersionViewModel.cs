using System;
using System.Security;

namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    public class QuestionVersionViewModel
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string  ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DuplicateFromViewModel DuplicateFrom { get; set; }

        public string RestoredFromVersion { get; set; }

        public bool IsPublishedFromDraft { get; set; }
    }
}
