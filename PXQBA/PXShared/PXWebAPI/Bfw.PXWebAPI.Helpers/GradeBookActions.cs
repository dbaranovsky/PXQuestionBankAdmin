using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.PXWebAPI.Helpers
{
    public class GradeBookActions
    {
        protected ISessionManager SessionManager { get; set; }

        public GradeBookActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public Adc.GradeBookWeights GetGradeBookWeights(string courseid)
        {
            
                var cmd = new GetGradeBookWeights
                {
                    SearchParameters = new Adc.GradeBookWeightSearch
                    {
                        EntityId = courseid
                    }
                };

               SessionManager.CurrentSession.Execute(cmd);
               

                return cmd.GradeBookWeights;
            }
        
    }
}
