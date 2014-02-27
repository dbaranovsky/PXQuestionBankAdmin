using System;
using Bsc = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents an activity that the user can view and interact with
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Id of the content item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Title of the content item
        /// </summary>        
        public string Title { get; set; }
        /// <summary>
        /// Title of the content item
        /// </summary>        
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>        
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been submmited by user.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has been submitted; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserSubmitted { get; set; }

        /// <summary>
        /// True if item is assigned [even with no due date].
        /// </summary>
        public bool isAssigned { get; set; }

        /// <summary>
        /// MetaTopic which we group Activities by
        /// </summary>
        public string MetaTopic { get; set; }

        /// <summary>
        /// Gets or sets the user access.
        /// </summary>
        /// <value>
        /// The user access.
        /// </value>
        public Bsc.AccessLevel UserAccess { get; set; }

        /// <summary>
        /// Whether this is an arga item with Url transformed
        /// </summary>
        public bool IsTransformedArgaItem { get; set; }

        /// <summary>
        /// Common property to store the url the content item points to or represents.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// DateText being displayed depending on if your a Instructor, Student, and if the item is complete/Not Due/Assigned
        /// </summary>
        public string DateText
        {
            get
            {
                var dateText = "";

                var dueDate = this.DueDate;
                var defaultDate = dueDate.Year.ToString();
                string dueDateString = this.DueDate.ToString();
                switch (this.UserAccess)
                {
                    case Bfw.PX.Biz.ServiceContracts.AccessLevel.Student:

                        if (IsUserSubmitted)
                        {
                            dateText = "Completed";
                        }
                        else if (defaultDate == "1" && !isAssigned)
                        {
                            dateText = "No Due Date";
                        }
                        else if ((defaultDate != "1" && defaultDate != "9999") && isAssigned)
                        {
                            dateText = string.Format("Due {0} ", dueDateString.Substring(0, dueDateString.IndexOf(' ')));
                        }
                        else
                        {
                            dateText = "Assigned";
                        }
                        break;
                    
                    case Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor:
                       
                        if (defaultDate == "1" && this.isAssigned == false)
                        {
                            dateText = "Assign";
                        }
                        else if ((defaultDate == "9999" || defaultDate == "1") && this.isAssigned == true)
                        {
                            dateText = "Assigned";
                        }
                        else
                        {
                            dateText = string.Format("Due {0}", dueDateString.Substring(0, dueDateString.IndexOf(' ')));
                        }
                        break;
                }


                return dateText;
            }
        }
    }
}
