<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AnnouncementWidget>" %>
<div class="announcementViewAll">

<% 
    var clsAnnoucementList = "";
    if (Model.IsInstructor)
    {
        clsAnnoucementList = "announcementList";
%>
    <div class="containerPostAnnouncement">
        <textarea class="txtPostAnnouncement"></textarea>
        <div class="textCopy">
        </div>
    </div>
    <input type="button" class="btnPostAnnouncement" value="Post" />
<%  } %>

    <% if (!Model.Announcements.IsNullOrEmpty())
       { %>
    <ul style="clear:both;" class="<%= clsAnnoucementList %>">
        <%  
           foreach (var ann in Model.Announcements)
           {
               var clsArchive = "";
               if (ann.IsArchived)
               {
                   clsArchive = "announcementInActiveModal";
               }               
        %>
        <li class="announcementItem <%= clsArchive %> announcementItemModal" sequence="<%= ann.PrimarySortOrder %>" pinSortOrder="<%= ann.PinSortOrder %>" isArchived="<%= ann.IsArchived.ToString().ToLower() %>" >        
            <% if (Model.IsInstructor)
            { %>            
              <div style="cursor: pointer" class="announcementPin UnPin"></div>
            <% } %>
            <span class="announcementDate">
                <%= ann.DisplayDate.ToString("MMM dd") %>,&nbsp;<%= ann.DisplayDate.ToString("h:mm tt") %>
            </span>
          
            <div class="displayWrapper">
                <div class="announcementBody">
                    <%= ann.Body.Replace(System.Environment.NewLine, "<br/>") %>
                </div>
                
            </div>

            <% if (Model.IsInstructor)
                { %>
                <div class="announcmentMenu">
                    <span class="announcementEdit"></span><span class="announcementDelete" announcementid="<%= ann.Path %>">
                    </span>
                    <% if (!ann.IsArchived)
                       {%>
                    <input type="button" value="Archive" name="Archive" class="announcementArchiveButton" />
                    <% }
                       else
                       {%>
                    <input type="button" value="Repost" name="Repost" class="announcementRepostButton" />
                    <% } %>
                </div>
                <%  } %>

            <% if (Model.IsInstructor)
                { %>
            <div class="editWrapper" style="display: none;">
                <div class="containerEditAnnouncement">
                    <textarea class="txtEditAnnouncement" announcementid="<%= ann.Path %>" creationdate="<%= ann.DisplayDate%>"><%= ann.Body %></textarea>
                    <div class="textCopy">
                        <%= ann.Body.Replace(System.Environment.NewLine, "<br/>") %> 
                    </div>
                </div>
                <div class="txtEditAnnoucementButtons">
                    <div>
                        <input type="button" value="Save" name="Save" class="saveEditAnnouncement" />
                    </div>
                    <div class="cancelEditAnnouncement">
                        Cancel
                    </div>
                </div>
            </div>
            <%  } %>
        </li>
        <% } %>
    </ul>

    <% }
       else
       { %>
    <div class="noItemsMessage" style="padding-top: 10px; padding-bottom: 10px;">
        There are currently no announcements for this course.</div>
    <% } %>

    <div class="announcementModalChange">
        <div style="float:left;">
            <input type="button" value="Done" class="announcementModalDone"/>
        </div>            
    </div>

</div>

<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/AnnouncementWidget/AnnouncementWidget.js") %>'], function () {
                PxAnnouncementWidget.BindControls();
            });
        });

    } (jQuery));    
</script>
