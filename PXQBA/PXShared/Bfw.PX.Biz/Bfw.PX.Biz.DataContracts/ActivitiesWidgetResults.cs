using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Model object representing an Activities Widget Results.
    /// </summary>
    public class ActivitiesWidgetResults
    {
        /// <summary>
        /// Dictionary of ContentItems that have the same meta-topic value
        /// </summary>
        public List<ContentItem> Activities;

        public ActivitiesWidgetResults()
        {
            Activities = new List<ContentItem>();
        }        
    }
}
