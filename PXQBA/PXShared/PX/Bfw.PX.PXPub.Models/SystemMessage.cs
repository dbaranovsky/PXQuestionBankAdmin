
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class SystemMessage
    {
        #region Properties
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AvilableDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsValid
        {
            get
            {
                return DateTime.Today >= AvilableDate && DateTime.Today <= ExpirationDate;
            }
            set{ }
        }
        #endregion Properties



    }
}
