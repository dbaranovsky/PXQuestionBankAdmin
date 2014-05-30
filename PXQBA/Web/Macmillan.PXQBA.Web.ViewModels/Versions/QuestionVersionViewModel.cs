using System;

namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    public class QuestionVersionViewModel
    {
        public string Id { get; set; }

        public string VersionNumber { get; set; }

        public string  ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsInitial { get; set;  }
    }

}
