<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RssFeed>" %>
<div id="divRssPopup" style="height: 100%">
	<h2 id="divRssPopupTitle" class="content-title">
		Rss Feed Articles
	</h2>
	<%
		var id = ViewData["id"];
		var enrollmentId = ViewData["eId"];
		var initialindex = ViewData["initialindex"];

		 // this is a hack, this shouldn't be done anywhere else. we are doing this because it
	   // is expedient. it would be better to use an action filter to populate the viewdata with the
	   // domain name.
	   var locator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
	   var ctx = locator.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>();
	   string DomainUserSpace = ctx.Domain.Name;
       var samlSubDomain = ConfigurationManager.AppSettings["SAMLsubDomain"];
	   // parse domain name from url
	   string host = Request.Url.Host.ToString();
	   string LocalDomainName = "";
       string url = string.Empty;

       if (!string.IsNullOrEmpty(samlSubDomain))
       {
           if (host.Split('.').Length > 1)
           {
               int last = host.LastIndexOf(".");
               string concatURL = host.Substring(0, last);
               LocalDomainName = host.Substring(concatURL.LastIndexOf(".") + 1, host.Length - (concatURL.LastIndexOf(".") + 1));
           }
           else
           {
               LocalDomainName = host;
           }

           var urlBase = String.Format("http://{0}.{1}.{2}", DomainUserSpace, ConfigurationManager.AppSettings["SAMLsubDomain"], (host == "localhost") ? "bfwpub.com" : LocalDomainName);	 //ConfigurationManager.AppSettings["EnvironmentUrl"];                     
           url = String.Format("{0}/brainhoney/Component/ActivityPlayer?id=frameviewitem&showheader=false&EnrollmentId={1}&itemid={2}&Extra=initialindex^{3}", urlBase, enrollmentId, id, initialindex);
       }
       else
       {
           url = String.Format("http://{0}/brainhoney/Component/ActivityPlayer?id=frameviewitem&showheader=false&EnrollmentId={1}&itemid={2}&Extra=initialindex^{3}", host, enrollmentId, id, initialindex);
       }
	%>
	<iframe style="width: 100%; height: 100%" src="<%= url %>"></iframe>
	<script type="text/javascript">
		PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
			PxRssFeed.ComponentUrl = '<%= url %>';
		});
	</script>
</div>