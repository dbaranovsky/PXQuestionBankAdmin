using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Text;
using System.Text.RegularExpressions;

namespace Bfw.PX.PXPub.Controllers
{
    public class ViewProvider : VirtualPathProvider
    {
        public ViewProvider()
            : base()
        {

        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return new ViewFile(virtualPath);

        }

        public class ViewFile : System.Web.Hosting.VirtualFile
        {

            public ViewFile(string path)
                : base(path)
            {
            }

            /// <summary>
            /// Removes calls to RenderAction from the views to avoid runtime errors while testing
            /// </summary>
            /// <returns></returns>
            public override System.IO.Stream Open()
            {
                string contents = System.IO.File.ReadAllText(HostingEnvironment.ApplicationPhysicalPath + this.VirtualPath);
                var app = this.VirtualPath.Split(new char[] {'/'})[2];
                var renderActionRegEx = new Regex(@"Html.RenderAction.*?;", RegexOptions.Singleline);
                contents = renderActionRegEx.Replace(contents, "");
                renderActionRegEx = new Regex(@"<%=Html.ToScreenStepLink.*?%>", RegexOptions.Singleline);
                contents = renderActionRegEx.Replace(contents, "");
                var resourceEngineRegEx = new Regex(@"<%=?\s*ResourceEngine.*?%>", RegexOptions.Singleline);
                contents = resourceEngineRegEx.Replace(contents, "[]");
                contents = contents.Replace("Html.RenderPartial(\"~/Views/", "Html.RenderPartial(\"~/Views/" + app + "/");
                return new System.IO.MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(contents));
            }
        }
    }
}