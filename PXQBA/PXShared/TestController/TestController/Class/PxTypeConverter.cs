using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.Expressions;

namespace Bfw.PX.PXPub.Controllers
{
    public class PxTypeConverter : JavaScriptTypeResolver
    {
        public override Type ResolveType(string id)
        {
            return Type.GetType(id);
        }

        public override string ResolveTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.Name;
        }
    }
}