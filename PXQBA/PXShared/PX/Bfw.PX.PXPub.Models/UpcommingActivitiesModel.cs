using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Xml.Linq;
using Bsc = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents an activity that the user can view and interact with
    /// </summary>
    public class UpcommingActivitiesModel
    {
        /// <summary>
        /// The assignment that is due next
        /// </summary>
        public AssignedItem NextAssignment { get; set; }
        

        /// <summary>
        /// The count of assignments due two weeks from today
        /// </summary>
       public int CountOfAssignments {get; set;}


       public string DueInDays { get; set; }
    }
}
