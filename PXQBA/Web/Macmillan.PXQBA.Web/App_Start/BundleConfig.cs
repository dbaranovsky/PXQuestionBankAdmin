using System.Web;
using System.Web.Optimization;

namespace Macmillan.PXQBA.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                     "~/Scripts/respond/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/react").Include(
                    "~/Scripts/react/react-with-addons-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/crossroads").Include(
                "~/Scripts/crossroads/signals.js",
                "~/Scripts/crossroads/crossroads.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
        }
    }
}
