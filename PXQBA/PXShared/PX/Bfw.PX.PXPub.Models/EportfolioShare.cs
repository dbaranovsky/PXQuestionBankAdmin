using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class EportfolioShare
    {
        /// <summary>
        /// List of students enrolled for the current course
        /// </summary>
        public IList<TocCategory> StudentList { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public EportfolioShare()
        {

        }
    }
}
