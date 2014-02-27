using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Bfw.PX.PXPub.Models
{
    public class ReviewOption
    {
        public AssessmentSettings AssessmentSettings { get; set; }
        public Expression<Func<ReviewOption, ReviewSetting?>> Option { get; set; }
        public string Key { get; set; }
    }
}
