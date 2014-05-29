using System;

namespace Macmillan.PXQBA.Business.Models
{
    
  public class QuestionVersion
    {

        public string Id { get; set; }

        public string VersionNumber { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsInitial { get; set; }
    }
}
