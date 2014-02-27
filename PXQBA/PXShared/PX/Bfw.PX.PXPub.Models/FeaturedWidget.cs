using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class FeaturedWidget : Widget
    {

        /// <summary>
        /// ContentType of the Feature content widget. Ex: LearningCurve, VideoTools etc.
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// Description of the feature content widget
        /// </summary>
        public String Description { get; set; }
    }
}
