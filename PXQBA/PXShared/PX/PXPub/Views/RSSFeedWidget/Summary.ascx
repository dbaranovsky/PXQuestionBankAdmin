
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>
<% if (!Model.RSSFeeds.IsNullOrEmpty())
   { 
       %>
    <div class = "customRSSWidgetMaindiv customRSSWidgetOuterDiv" PageIndex = "1">

     <%if (ViewData["feedDescription"] != "")
     {%>
        <div class = "customRSSfeedDescription">
            <%--<%= ViewData["feedDescription"]%>--%>
        </div>
    <%}%>

        <% foreach (var rss in Model.RSSFeeds)
           { %>

              <div IsArchived = "<%= rss.IsArchived %>" class="rssFeedParent" id="rssFeedParent_<%= rss.FeedCounter %>" RssUrl = "<%= rss.RssUrl %>" LinkDescription = "<%= rss.LinkDescription %>" LinkTitle = "<%= rss.LinkTitle %>" LinkUrl = "<%= rss.Link %>" PubDate = "<%= rss.PubDate %>" PubDateCalculated = "<%= rss.PubDateCalculated %>">
                <div class="feedContainer">
                    <div><a class="customRssLink linkTitle"  href="<%= rss.Link %>" target="new"><%= rss.LinkTitle%></a></div>
                    <%if (true.Equals(ViewData["InstructorAccessLevel"]))
                      {%> 
                            <div  class = "RSSButtons buttonContainer">
                        <% if (rss.IsAssigned == false)
                           {%>
                           <div class="buttonRSS" >
                               <div class="btnAssignRSSFeed btnAssignRSSFeedVisible">
                                    <input type="button" value ="Assign" class="RSSFeedButton" />
                               </div>
                               <div  class="btnUnassignRSSFeed btnAssignRSSFeedNonVisible">
                                   <input type="button" value ="Assigned" class="RSSFeedButton" />
                               </div>
                           </div>
                         <%}%>
                        <% else
                           {%>
                           <div class="buttonRSS">
                               <div class="btnAssignRSSFeed btnAssignRSSFeedNonVisible">
                                   <input type="button" value ="Assign" class="RSSFeedButton" />
                               </div>
                               <div class="btnUnassignRSSFeed btnAssignRSSFeedVisible">
                                    <input type="button" value ="Assigned" class="RSSFeedButton" />
                               </div>    
                           </div>
                         <%}%>

                        <% if (rss.IsArchived == false)
                           {%>
                           <div class="buttonRSS">
                               <div class="btnArchiveRSSFeed btnArchiveRSSFeedVisible">
                                    <input type="button" value ="Archive"  class="RSSFeedButton"  />
                               </div>
                                <div class="btnUnarchiveRSSFeed btnArchiveRSSFeedNonVisible">
                                    <input type="button" value ="Archived"  class="RSSFeedButton"  />
                                </div>
                           </div>
                               
                         <%}%>

                        <% else
                           {%>
                           <div class="buttonRSS" >
                               <div class="btnArchiveRSSFeed btnArchiveRSSFeedNonVisible">
                                   <input type="button" value ="Archive" class="RSSFeedButton" />
                               </div>
                               <div  class="btnUnarchiveRSSFeed btnArchiveRSSFeedVisible">
                                    <input type="button" value ="Archived"  class="RSSFeedButton" />
                               </div>
                               </div>
                         <%}%>


                    </div>
                    <%}%>
                    <div class="customRssPubDate pubDate"><%= rss.PubDateCalculated.ToString()%></div>
                    <input type="hidden" id="RSSArticleID" name="RSSArticleID" value="<%= rss.ArchivedItemId %>" />
                    <input type="hidden" id="RSSArticleDueDate" name="RSSArticleDueDate" value="<%= rss.AssignedDate %>" />
                    <input type="hidden" class="customValues" value="<%= Model.Title %>" />
                    <input type="hidden" class="imageUrl" value="<%= ViewData["imageURL"] %>" />

                    <input type="hidden" class="totalArchivedArticles" value="<%= Model.TotalArchivedArticles.ToString()%>" />
                </div>
              </div>
        <% } %>
   </div>
<% } 
   else
   { %>
        <span class="noArticlesMessage">No articles available.....</span>
<% } %>

    <div class="customRSSModal">
        <div class="placeHolderCustomRSS">
        </div>
  </div>



<div class="customRSSViewAllBar">
    <div class="customRSSViewAllBarArticleCount"><%= Model.TotalArchivedArticles.ToString()%></div>
    <div class="customRSSViewAllBarText">archived articles</div>
    <div class="customRssViewAllLink customRSSViewAllBarLink">
        <% if (Model.TotalArchivedArticles > 0) 
            {               
        %>
            View all&gt;
        <% } %>
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
