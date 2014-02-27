<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>

<% string loadOnReady = (ViewData["loadonReady"] == null) ? "" : ViewData["loadonReady"].ToString();
   string id = (ViewData["Id"] == null) ? "" : ViewData["Id"].ToString();
   string retrievalLimit = (ViewData["retrievalLimit"] == null) ? "" : ViewData["retrievalLimit"].ToString();
   string scrollingRestricted = (ViewData["scrollingRestricted"] == null) ? "false" : ViewData["scrollingRestricted"].ToString();
   if (loadOnReady.ToLower().Equals("true"))
   {
       
       %>
            <div class="rss-data"></div>
            <script type="text/javascript">
            (function ($) {

                PxPage.OnReady(function () {
                    PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
                        PxRssArticle.LoadFaceplateRssWidget("<%=id %>", "<%=retrievalLimit %>", "<%=scrollingRestricted %>");
                    });
                });

            } (jQuery));    
            </script>
  <% }
   else
   { 
       %>

<% if (!Model.RSSFeeds.IsNullOrEmpty())
   { 
       %>
<div class = "customRSSWidgetMaindiv customRSSWidgetOuterDiv" PageIndex = "1">
     <%if (ViewData["feedDescription"] != "")
     {%>
        <div class = "customRSSfeedDescriptionCompact">
            <%--<%= ViewData["feedDescription"]%>--%>
        </div>
    <%}%>
        <%--<div class="rssTitle">Latest from <i><%=Model.Title %></i></div>--%>
        <%--<a value="More" class="more-rss-feed" href="#">more</a>--%>
        <% foreach (var rss in Model.RSSFeeds)
           { %>

              <div IsArchived = "<%= rss.IsArchived %>" class="rssFeedParent" id="rssFeedParent_<%= rss.FeedCounter %>" RssUrl = "<%= rss.RssUrl %>" LinkDescription = "<%= rss.LinkDescription %>" LinkTitle = "<%= rss.LinkTitle %>" LinkUrl = "<%= rss.Link %>" PubDate = "<%= rss.PubDate %>" PubDateCalculated = "<%= rss.PubDateCalculated %>">
                <div class="compactFeedContainer">
                    <div class="compactTitlePubDateContainer" >
                        <%
               var fnelink = Url.GetComponentLink(rss.LinkTitle.Truncate("...", 0, 100), "item", id, new
               {
                   mode = ContentViewMode.ExternalUrl,
                   includeDiscussion = false,
                   renderFNE = true,
                   externalUrl = rss.Link
               }, new { @class = "customRssLink compactLinkTitle" });
                %>
                        <div><%=fnelink %>
                            <%--<a class="customRssLink compactLinkTitle"  href="#" onclick="PxPage.LargeFNE.OpenFNELink('<%= rss.Link %>', '<%= rss.LinkTitle %>',true, null);" ><%= rss.LinkTitle%></a>--%>
                            </div>
                        <div class="customRssPubDate compactPubDate"><%= rss.PubDateCalculated.ToString() %></div>
                    </div>
                    
                    <input type="hidden" id="RSSArticleID" name="RSSArticleID" value="<%= rss.ArchivedItemId %>" />
                    <input type="hidden" id="RSSArticleDueDate" name="RSSArticleDueDate" value="<%= rss.AssignedDate %>" />
                    <input type="hidden" class="customValues" value="<%= Model.Title %>" />
                    <input type="hidden" class="imageUrl" value="<%= ViewData["imageURL"] %>" />
                    <input type="hidden" class="totalArchivedArticles" value="<%= Model.TotalArchivedArticles.ToString()%>" />
                    <input type="hidden" class="retrieval-cap" value="<%= ViewData["retrievalCap"] %>" />
                </div>
              </div>
        <% } %>
            </div>
<% } 
   else
   { %>
        <span class="noArticlesMessage">No articles available.....</span>
<% } %>

<span><a class="rss-more-resources-link" href="#" >More articles</a></span>

    <div class="customRSSModal">
        <div class="placeHolderCustomRSS">
        </div>
  </div>
<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
                PxRssArticle.BindControls();
            });
        });

    } (jQuery));    
</script>

<%} %>
