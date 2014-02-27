
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>

<% if (!Model.RSSFeeds.IsNullOrEmpty())
   { 
              %>
<div class="maindiv" id= maindiv_<%= ViewData["WidgetId"] %>>
        <% foreach (var rss in Model.RSSFeeds)
           { %>

              <div IsArchived = "<%= rss.IsArchived %>" class="rssFeedParent" id="rssArticleParent_<%= rss.FeedCounter %>" RssUrl = "<%= rss.RssUrl %>" LinkDescription = "<%= rss.LinkDescription %>" LinkTitle = "<%= rss.LinkTitle %>" LinkUrl = "<%= rss.Link %>" PubDate = "<%= rss.PubDate %>" PubDateCalculated = "<%= rss.PubDateCalculated %>">
                <div class="feedContainer">
                    <div><a class="customRssLink linkTitle"  href="<%= rss.Link %>" target="new"><%= rss.LinkTitle%></a></div>
                    <%if (true.Equals(ViewData["InstructorAccessLevel"]))
                      {%> 
                            <div  class = "RSSButtons buttonContainer">
                        <% if (rss.IsAssigned == false) 
                           {%>
                           <div class="buttonRSS" >
                               <div class="btnAssignRSSArticle btnAssignRSSFeedVisible">
                                    <input type="button" value ="Assign" class="RSSFeedButton" />
                               </div>
                               <div  class="btnUnassignRSSArticle btnAssignRSSFeedNonVisible">
                                   <input type="button" value ="Assigned" class="RSSFeedButton" />
                               </div>
                           </div>
                         <%}%>
                        <% else 
                           {%>
                           <div class="buttonRSS">
                               <div class="btnAssignRSSArticle btnAssignRSSFeedNonVisible">
                                   <input type="button" value ="Assign" class="RSSFeedButton" />
                               </div>
                               <div class="btnUnassignRSSArticle btnAssignRSSFeedVisible">
                                    <input type="button" value ="Assigned" class="RSSFeedButton"  />
                               </div>    
                           </div>
                         <%}%>

                        <% if(rss.IsArchived == false) 
                           {%>
                           <div class="buttonRSS" >
                               <div class="btnArchiveRSSArticle btnArchiveRSSFeedVisible">
                                    <input type="button" value ="Archive"  class="RSSFeedButton"   />
                               </div>
                                <div class="btnUnarchiveRSSArticle btnArchiveRSSFeedNonVisible">
                                    <input type="button" value ="Archived" class="RSSFeedButton"    />
                                </div>
                           </div>
                               
                         <%}%>

                        <% else 
                           {%>
                           <div class="buttonRSS" >
                               <div class="btnArchiveRSSArticle btnArchiveRSSFeedNonVisible">
                                   <input type="button" value ="Archive" class="RSSFeedButton"  />
                               </div>
                               <div  class="btnUnarchiveRSSArticle btnArchiveRSSFeedVisible">
                                    <input type="button" value ="Archived" class="RSSFeedButton"  />
                               </div>
                               </div>
                         <%}%>


                    </div>
                    <%}%>
                    <div  class="customRssPubDate pubDate"><%= rss.PubDate.ToString()%></div>
                    <input type="hidden" id="RSSArticleID" name="RSSArticleID" value="<%= rss.ArchivedItemId %>" />
                    <input type="hidden" id="RSSArticleDueDate" name="RSSArticleDueDate" value="<%= rss.AssignedDate %>" />
                </div>
              </div>
        <% } %>
        
           <input type="hidden" class="totalArchivedArticlesViewAll" value="<%= Model.TotalArchivedArticles.ToString()%>" />
<% } 
   else
   { %>
                <span class="noArticlesMessage">No archived articles available.....</span>
<% } %>
     <input type="hidden" id="unArchivedArticlId" name="unArchivedArticlId"/>
     <input type="hidden" id="unAssignedArticlId" name="unAssidnedArticlId"/>
     <input type="hidden" id="assignedArticlId" name="assidnedArticlId"/>
     
     <div id = "NoArchivedArticleSpan" class = "NoArticleMessageNotVisible">
                <span class="noArticlesMessage">No archived articles available.....</span>
     </div>

    <div class="customRSSModal" id="customRSSModal">
        <div class="placeHolderCustomRSS">
        </div>
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
