using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class DashboardList
    {
        public IList<DashboardListItem> Items { get; set; }

        public DashboardList()
        {
            Items = new List<DashboardListItem>();
        }
    }

    public class DashboardListItem
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string Domain { get; set; }
    }
}
