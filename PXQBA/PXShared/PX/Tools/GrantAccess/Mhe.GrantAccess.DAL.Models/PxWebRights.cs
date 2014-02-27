using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mhe.GrantAccess.DAL.Models
{
    public class PxWebRights
    {
        public static readonly PxWebRights AdminTool = new PxWebRights { RightType = "AdminTool" };
        public static readonly PxWebRights QuestionBankAdmin = new PxWebRights { RightType = "QuestionBank" };

        public int Id { get; set; }

        public string RightType { get; set; }
    }
}
