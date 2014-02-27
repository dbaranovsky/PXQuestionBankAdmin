<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RssFeed>" %>
<% if (Request.Params["mode"] == "4" || Request.Params["mode"] == null || Request.Params["mode"] == "Preview")
   { // THIS IS THE NORMAL PAGE VIEW MODE:
	   var hasParentLesson = ViewData["hasParentLesson"];
	   var rssPopupUrl = Url.Action("ShowRssPopup", "RssFeed", new { id = Model.Id, eId = Model.EnrollmentId });
%>
<div class="rss-content-view">
	<input type="hidden" id="rss-popup-url" value="<%=rssPopupUrl %>" />
	<h2 class="content-title">
		<%= HttpUtility.HtmlDecode(Model.Title) %>
		<% if (!Model.ReadOnly)
	 { %>
		<div class="menu edit-link">
			<% var editUrl = Url.Action("DisplayItem", "ContentWidget",
						new
						{
							id = Model.Id,
							mode = ContentViewMode.Edit,
							hasParentLesson = hasParentLesson,
							includeNavigation = false,
							isBeingEdited = true
						});
			%>
			<a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
		</div>
		<% } %>
	</h2>
	<p>
		<%= HttpUtility.HtmlDecode(Model.Description) %></p>
	<%= Html.HiddenFor(m => m.Id)%>
	<% if (!Model.RssFeeds.IsNullOrEmpty())
	{ %>
	<h3 class="sub-title">
		Articles</h3>
	<div>
		<div class="rss-summary">
			You have
			<%=Model.RssFeeds.Count()%>
			articles available from this RSS Feed.
			<%= Ajax.ActionLink("REFRESH", "ViewRssFeed", "RssFeed", new { contentId = Model.Id }, new AjaxOptions() { UpdateTargetId = "content-item", OnSuccess = "ContentWidget.ContentCreated(null); PxRssFeed.Init();" }) %>
		</div>
	</div>
	<div class="feed">
		<% var i = 0;
	 foreach (var item in Model.RssFeeds)
	 { %>
		<div class="rss-item">
			<table>
				<tr>
					<td class="rss-item-title">
						<%=Html.ActionLink(item.LinkTitle, "ShowRssPopup", "RSSFeed", new
							{
								id = Model.Id,
								eId = Model.EnrollmentId,
								initialindex = i,
							}, new
							{
								@class = "fne-link other"
							})%>
					</td>
					<td class="rss-item-desc">
						<%= item.LinkDescription%>
					</td>
					<td class="rss-item-date"><%=item.PubDate%></td>
				</tr>
			</table>
		</div>
		<%
		 i++;
	 } %>
	</div>
	<% } %>
	<%else
	{%>
	<div>
		<span>There are no articles available from this RSS Feed.</span>
	</div>
	<%}%>
</div>
<% }
   else
   { // THIS IS THE FNE VIEW MODE: %>
<div class="rss-content-view" style="width: 100%; height: 100%">
	<h2 id="divRssPopupTitle" class="content-title">
		Rss Feed Articles
	</h2>
	<%

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

           url = String.Format("{0}/brainhoney/Component/ActivityPlayer?id=frameviewitem&showheader=false&EnrollmentId={1}&itemid={2}", urlBase, Model.EnrollmentId, Model.Id);
       }
       else
       {
           url = String.Format("http://{0}/brainhoney/Component/ActivityPlayer?id=frameviewitem&showheader=false&EnrollmentId={1}&itemid={2}", host, Model.EnrollmentId, Model.Id);
       }
	%>
	<iframe id="rssframe" style="width: 100%; height: 100%" src="<%= url %>"></iframe>
	<script type="text/javascript">
	    (function ($) {
	        PxPage.Toasts.Error('<%= url %>');
		    PxPage.OnReady(function () {
		        PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
		            
		            PxRssFeed.ComponentUrl = '<%= url %>';
		            var iframeHeight = $("#fne-content").height();
		            $("#rssframe").height(iframeHeight - 150);
		        });
		    });
		} (jQuery))
	</script>
</div>
<%}%>