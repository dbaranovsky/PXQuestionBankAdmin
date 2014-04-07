using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Macmillan.PXQBA.Web.App_Start.BootstrapBundleConfig), "RegisterBundles")]

namespace Macmillan.PXQBA.Web.App_Start
{
	public class BootstrapBundleConfig
	{
		public static void RegisterBundles()
		{
			// Add @Styles.Render("~/Content/bootstrap") in the <head/> of your _Layout.cshtml view
			// For Bootstrap theme add @Styles.Render("~/Content/bootstrap-theme") in the <head/> of your _Layout.cshtml view
			// Add @Scripts.Render("~/bundles/bootstrap") after jQuery in your _Layout.cshtml view
			// When <compilation debug="true" />, MVC4 will render the full readable version. When set to <compilation debug="false" />, the minified version will be rendered automatically
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap/bootstrap.js").Include("~/Scripts/bootstrap/bootstrap-notify.js"));
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/bootstrap-components").Include("~/Scripts/bootstrap/bootstrap-paginator.min.js"));
            BundleTable.Bundles.Add(new StyleBundle("~/bundles/bootstrapcss").Include("~/Content/bootstrap/bootstrap.css").Include("~/Content/bootstrap/bootstrap-notify.css"));
            BundleTable.Bundles.Add(new StyleBundle("~/bundles/bootstrap-theme").Include("~/Content/bootstrap/bootstrap-theme.css"));
		}
	}
}
