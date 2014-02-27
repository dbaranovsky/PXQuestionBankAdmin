using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap.Session;
using Pxdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PXWebAPI.Helpers
{
    public class Helper
    {
        public static Dictionary<string, string> GetPostParameters(HttpRequest httpRequest)
        {
            var retParams = new Dictionary<string, string>();
            foreach (string name in httpRequest.Form)
            {
                if (retParams.ContainsKey(name))
                    retParams[name] = httpRequest.Form[name];
                else
                    retParams.Add(name, httpRequest.Form[name]);
            }
            return retParams;
        }

        public static string GetCategoryName(string courseId, string categoryId, ISessionManager sessionManager)
        {
            var gbAction = new GradeBookActions(sessionManager);
            var gbWeights = gbAction.GetGradeBookWeights(courseId);
            if (gbWeights != null)
            {
                var gwCategories = gbWeights.GradeWeightCategories;
                if (!gwCategories.IsNullOrEmpty())
                {
                    return (from swCategory in gwCategories where swCategory.Id.Equals(categoryId) select swCategory.Text).FirstOrDefault();
                }
                
            }
            return null;
        }
    }
}
