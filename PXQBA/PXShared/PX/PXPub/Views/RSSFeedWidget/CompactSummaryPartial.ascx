
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>
<% if (!Model.RSSFeeds.IsNullOrEmpty())
   { 
       %>
        <% foreach (var rss in Model.RSSFeeds)
           { %>

              <div IsArchived = "<%= rss.IsArchived %>" class="rssFeedParent" id="rssFeedParent_<%= rss.FeedCounter %>" RssUrl = "<%= rss.RssUrl %>" LinkDescription = "<%= rss.LinkDescription %>" LinkTitle = "<%= rss.LinkTitle %>" LinkUrl = "<%= rss.Link %>" PubDate = "<%= rss.PubDate %>" PubDateCalculated = "<%= rss.PubDateCalculated %>">
                <div class="compactFeedContainer">
                    <div class="compactTitlePubDateContainer" >
                        <div><a class="customRssLink compactLinkTitle"  href="<%= rss.Link %>" target="new"><%= rss.LinkTitle%></a></div>
                        <div class="customRssPubDate compactPubDate"><%= rss.PubDateCalculated.ToString()%></div>
                    </div>
                     <div class="compactButtonContainer" >
                    <%if (true.Equals(ViewData["InstructorAccessLevel"]))
                      {%> 
                            <a class="CompactButtonSetContainer compactButtonSetController"  href="#"></a>
                            <div class="CompactButtonSetMenu compactButtonSetMenuContainer">
                                
                                <% if (rss.IsArchived == false)
                                   {%>
                                    <div class= "lnkArchiveRSSFeed"">
                                        <div class="buttonCheckOff" style="float:left;"></div>
                                        <div class="lnkButtonSet" style="float:left;">Archive</div>
                                    </div>
                                    <div class= "lnkUnarchiveRSSFeed" style="display:none;">
                                        <div class="buttonCheckOn" style="float:left;"></div>
                                        <div class="lnkButtonSet" style="float:left;">Archived</div>
                                    </div>
                                 <%}%>

                                <% else
                                   {%>
                                    <div class= "lnkArchiveRSSFeed" style="display:none;">
                                        <div class="buttonCheckOff" style="float:left;"></div>
                                        <div class="lnkButtonSet" style="float:left;">Archive</div>
                                    </div>
                                    <div class= "lnkUnarchiveRSSFeed" >
                                        <div class="buttonCheckOn" style="float:left;"></div>
                                        <div class="lnkButtonSet" style="float:left;">Archived</div>
                                    </div>
                                 <%}%>

                                
                                <% if (rss.IsAssigned == false)
                                   {%>
                                        <div class= "lnkAssignRSSFeed">
                                            <div class="buttonCheckOff" style="float:left; "></div>
                                            <div class="lnkButtonSet" style="float:left;">Assign</div>
                                        </div>
                                        <div class= "lnkUnassignRSSFeed" style="display:none;">
                                            <div class="buttonCheckOn" style="float:left;"></div>
                                            <div class="lnkButtonSet" style="float:left;">Assigned</div>
                                        </div>
                                 <%}%>

                                <% else
                                   {%>
                                        <div class= "lnkAssignRSSFeed" style="display:none;">
                                            <div class="buttonCheckOff" style="float:left;"></div>
                                            <div class="lnkButtonSet" style="float:left;">Assign</div>
                                        </div>
                                        <div class= "lnkUnassignRSSFeed">
                                            <div class="buttonCheckOn" style="float:left;"></div>
                                            <div class="lnkButtonSet" style="float:left;">Assigned</div>
                                        </div>
                                 <%}%>
                            </div>
                     <%}%>
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
