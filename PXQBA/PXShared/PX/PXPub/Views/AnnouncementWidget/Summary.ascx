<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AnnouncementWidget>" %>

<div id="announcementWidget" class="announcementWidget" location="summary">

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
    <input type="button" class="btnPostAnnouncement" value="Post"/>
<% } %>       

<% if (!Model.Announcements.IsNullOrEmpty())
   { %>
        <ul class="<%= clsAnnoucementList %>">  
        <%  
            foreach (var ann in Model.Announcements)
           { %>
            <li id="sort" class="announcementItem" sequence="<%= ann.PrimarySortOrder %>" pinSortOrder="<%= ann.PinSortOrder %>" isArchived="<%= ann.IsArchived.ToString().ToLower() %>" >
                <% if (Model.IsInstructor)
                   { %>                
                <div style="cursor: pointer" class="announcementPin UnPin"></div>
                <% } %>

                <% if (Model.IsInstructor)
                    { %>
                    <div class ="announcement-menu">
                        <span class="announcementEdit"></span>
                        <input type="button" value="Archive" name="Archive" class="announcementArchiveButton" />
                    </div>
                    <%  } %>

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
                <div class="editWrapper" style="display:none;">
                    <div class="containerEditAnnouncement">
                        <textarea class="txtEditAnnouncement" announcementID="<%= ann.Path %>" creationDate="<%= ann.DisplayDate%>" ><%= ann.Body %></textarea>
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
        <div class="noItemsMessage" style="padding-top:10px;padding-bottom:10px;">There are currently no announcements for this course.</div>
<% } %>

    <% if (Model.ArchivedCount > 0)
    { %>
        <div class="announcementViewAllBar">
            <span><%= Model.ArchivedCount.ToString()%> archived announcements &nbsp;&nbsp;</span>           
            <span class="announcementViewAllLink">View all&gt;</span>
        </div>
    <% } %>

    <div class="announcementModal">
        <div class="placeHolderAnnouncement">
        </div>
    </div>
    <hr/>
    <input type="button" value="Done" name="Done" class="closeEditAnnouncement" />
</div>

<script type="text/javascript">
    (function ($) {        

        PxPage.OnReady(function() {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/AnnouncementWidget/AnnouncementWidget.js") %>'], function () {
                PxAnnouncementWidget.BindControls();
            });
        });

    } (jQuery));    
</script>
