using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    public class UpdatePassword : DlapCommand
    {
        public string UserId { get; set; }

        public string Password { get; set; }

        public bool Success { get; set; }

        public override Dlap.DlapRequest ToRequest()
        {
            var request = new DlapRequest
            {
                Type = DlapRequestType.Post,
                Attributes = new Dictionary<string, object> {
                    { "cmd", "updatepassword" },
                    { "userid", UserId },
                    { "password", Password }
                }
            };

            return request;
        }

        public override void ParseResponse(Dlap.DlapResponse response)
        {
            Success = response.Code == DlapResponseCode.OK;
        }
    }
}
