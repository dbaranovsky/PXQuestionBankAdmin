using System.Web;
using System.Web.Optimization;

namespace Macmillan.PXQBA.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                  "~/Scripts/chosen/chosen.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-{version}.js",
                        "~/Scripts/jquery/jquery-ui-1.10.4.min.js",
                        "~/Scripts/jquery/jquery.switchButton.js",
                        "~/Scripts/jquery/jquery.highlight-4.js",
                        "~/Scripts/jquery/jquery.fileupload.js",
                        "~/Scripts/jquery/jquery.iframe-transport.js"));

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
                "~/Scripts/crossroads/hasher.js",
                "~/Scripts/crossroads/crossroads.js"));

            bundles.Add(new ScriptBundle("~/bundles/handlebars").Include(
                      "~/Scripts/handlebars/handlebars-v{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/commonjs").Include(
                      "~/Scripts/session/sessionKeeper.js",
                      "~/Scripts/changesMonitor.js"));

            bundles.Add(new ScriptBundle("~/bundles/easyXDM").Include(
                   "~/Scripts/easyXDM/easyXDM.debug.js"));


            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/site.css",
                      "~/Content/paging.css",
                      "~/Content/chosen.css",
                      "~/Content/jquery.switchButton.css",
                      "~/Content/jquery.fileupload-ui.css"));

            bundles.Add(new ScriptBundle("~/bundles/customQuestions").Include(
                "~/Scripts/customQuestions/CQ.js",
                "~/Scripts/customQuestions/tiny_mce.js",
                  "~/Scripts/customQuestions/tinymce.js",
                "~/Scripts/customQuestions/XMLWriter.js"));
        }
    }
}
