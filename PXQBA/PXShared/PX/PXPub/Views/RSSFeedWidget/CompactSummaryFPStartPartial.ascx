
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>
<% if (!Model.RSSFeeds.IsNullOrEmpty())
   { 
       %>
        <% foreach (var rss in Model.RSSFeeds)
           {
               var i = Guid.NewGuid();
               var rssLink = "<a title='" + rss.LinkTitle + "' id='rss_" + i + "' class='lnkMoreResourceItem customRssLink lnkMoreResourceItemRSS' style='display:none' href='" + Url.GetComponentHash("item", "externalUrl", new { mode = "ExternalUrl", renderFNE = "True", externalUrl = Url.Encode(Url.Encode(rss.Link)) }) + "'>" + rss.LinkTitle + "</a>";
               %>

              <div IsArchived = "<%= rss.IsArchived %>" class="rssFeedParent" id="rssFeedParent_<%= rss.FeedCounter %>" RssUrl = "<%= rss.RssUrl %>" LinkDescription = "<%= rss.LinkDescription %>" LinkTitle = "<%= rss.LinkTitle %>" LinkUrl = "<%= rss.Link %>" PubDate = "<%= rss.PubDate %>" PubDateCalculated = "<%= rss.PubDateCalculated %>">
                <div class="compactFeedContainer">
                    <div class="compactTitlePubDateContainer" >
                        <div><%= rssLink %><span class="customRssLink compactLinkTitle" style="cursor: pointer" onclick="PxRssArticle.OpenRSSLink('rss_<%= i %>')"><%= rss.LinkTitle %></span></div>
                        <div class="customRssPubDate compactPubDate"><%= rss.PubDateCalculated.ToString()%></div>
                    </div>
                    
                    <input type="hidden" id="RSSArticleID" name="RSSArticleID" value="<%= rss.ArchivedItemId %>" />
                    <input type="hidden" id="RSSArticleDueDate" name="RSSArticleDueDate" value="<%= rss.AssignedDate %>" />
                    <input type="hidden" class="customValues" value="<%= Model.Title %>" />
                    <input type="hidden" class="totalArchivedArticles" value="<%= Model.TotalArchivedArticles.ToString()%>" />
                </div>
              </div>
        <% }
   }%>

<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
                PxRssArticle.BindControls();
            });
        });

    } (jQuery));    
</script>
