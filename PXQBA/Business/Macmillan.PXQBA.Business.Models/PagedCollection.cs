using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class PagedCollection<T>
    {
        public IEnumerable<T> CollectionPage { get; set; }
        public int TotalItems { get; set; }
    }
}
