using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    [Serializable]
    public class GradebookPreferencesChangeState
    {
        public string EntityId { get; set; }
        public string AboveId { get; set; }
        public string AboveSequence { get; set; }
        public string BelowId { get; set; }
        public string BelowSequence { get; set; }
        public string[] ItemIdList { get; set; }
        public string NewValue { get; set; }
        public bool UseWeighted { get; set; }
    }
}
