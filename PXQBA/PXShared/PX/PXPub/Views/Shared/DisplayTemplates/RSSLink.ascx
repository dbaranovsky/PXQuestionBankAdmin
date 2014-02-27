<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RssLink>" %>
<% if (Request.Params["mode"] == "4" || Request.Params["mode"] == null || Request.Params["mode"] == "Preview")
{ // THIS IS THE NORMAL PAGE VIEW MODE:
    var hasParentLesson = ViewData["hasParentLesson"];
    %>
       <%-- <h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
        </h2>
        <p><%= HttpUtility.HtmlDecode(Model.Description) %></p>
        
        <div style= "padding-top: 25px; font-size: smaller;" > Read the article at:</div>
        <div><a class="customRssLink"  href="<%= Model.Link %>" target="new"><%= Model.Link%></a></div>--%>
        <iframe  id="rssframe"  style="width: 100%;height:100%" src="<%=Model.Link %>"></iframe>
        <script type="text/javascript">
            (function ($) {
                PxPage.OnReady(function () {
                        $(".contentwrapper").replaceWith($("#rssframe"));
                });
            } (jQuery))            
        </script>
<% } 
else 
{ // THIS IS THE FNE VIEW MODE: %>
    <div class="rss-content-view" style="width: 100%;height:100%">
        <h2 id="divRssPopupTitle" class="content-title">
            Rss Feed Articles
        </h2>
        <%
            var urlBase = ConfigurationManager.AppSettings["EnvironmentUrl"];
            var url = String.Format("{0}/Component/ActivityPlayer?id=frameviewitem&showheader=false&EnrollmentId={1}&itemid={2}", urlBase, Model.EnrollmentId, Model.Id); 
        %>                    
        <iframe id="rssframe"  style="width: 100%;height:100%" src="<%= url %>"></iframe>
        <script type="text/javascript">
            (function ($) {
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


