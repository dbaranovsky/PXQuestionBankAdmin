using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;
using Bfw.Common.Collections;

using BizDC = Bfw.PX.Biz.DataContracts;


namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all the data required to display a Domain Selection Dialog
    /// </summary>
    public class DomainSelection
    {
        /// <summary>
        /// Gets or sets the domains the user is a member of
        /// </summary>
        /// <value>
        /// The domains
        /// </value>
        public IEnumerable<BizDC.Domain> Domains
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain the user selected
        /// </summary>
        public Domain SelectedDomain { get; set; }

        public Int32 SelectedDomainId { get; set; }
        public String CallbackController { get; set; }

        public String CallbackAction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DomainSelection()
        {
            Domains = new List<BizDC.Domain>();
        }
    }
}
