<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Discussion>" %>

<%var hasParentLesson = ViewData["hasParentLesson"];%>
<div class="discussion-content">
<h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
   <% if (!Model.ReadOnly)
       { %>
        <div class="menu edit-link">
            <%
                var editUrl = Url.Action("DisplayItem", "ContentWidget",
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
<div class="html-container description-content"><%= Model.Description %></div>
</div>
<% 
   BhComponent component = new BhComponent()
   {
       ComponentName = "ActivityPlayer",
       Id = "frameviewitem",
       Parameters = new
       {
           EnrollmentId = Model.EnrollmentId,
           ItemId = Model.Id,
           ShowHeader = false,
           ShowBeforeUnloadPrompts = false
       }
   };

   Html.RenderPartial("BhIFrameComponent", component);
   
 %>
 <script type="text/javascript">
     (function ($) {
         PxPage.OnReady(function () {
             PxPage.Require(['<%= Url.ContentCache("~/Scripts/Discussion/Discussion.js") %>'], function () {                
                 PxDiscussion.Init();
             });
         });
    } (jQuery));  
</script>