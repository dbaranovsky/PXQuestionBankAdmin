using System;

namespace Bfw.PXWebAPI.Models
{
    public class Item
    {
        /// <summary>
        /// Title of the item
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Title of the item
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Description of the item
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Category the item belongs to.
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// Date the item is due.
        /// </summary>
        public DateTime dueDate { get; set; }
        /// <summary>
        /// Number of points the item is worth.
        /// </summary>
        public double pointPossible { get; set; }

        /// <summary>
        /// Visibility.
        /// </summary>
        public bool visibility { get; set; }

        /// <summary>
        /// iconUri. 
        /// </summary>
        public string iconUri { get; set; }

        /// <summary>
        /// targetUri. 
        /// </summary>
        public string targetUri { get; set; }
    }
}
