using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mhe.GrantAccess.DAL.Models
{
    public class PxWebUserRights
    {
        public long? Id { get; set; }

        public string CourseId { get; set; }

        public string UserId { get; set; }

        public PxWebRights Component { get; set; }

        public long Rights { get; set; }
    }
}
