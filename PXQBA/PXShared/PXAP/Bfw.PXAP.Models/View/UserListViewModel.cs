using System.Collections.Generic;
using Bfw.PXAP.Models.Domain;

namespace Bfw.PXAP.Models.View
{
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
