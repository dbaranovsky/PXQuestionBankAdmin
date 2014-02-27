<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Assignment>" %>

<% if (Model.TocItem.IsNullOrEmpty()) { return; }
    
   foreach (var section in Model.TocItem) {
       var title = string.IsNullOrEmpty(HttpUtility.HtmlDecode(section.Title)) ? "No Title" : HttpUtility.HtmlDecode(section.Title);
       var childContainerId = string.Format("assign_children_of_{0}", section.Id);
%>
<div class="level">
    <span class="expandContainer" id="<%=section.Id %>">
        <%= Ajax.ActionLink(HttpUtility.HtmlDecode(title), "ExpandSection", "Assignment",
                                                         new {  id = section.Id, 
                                                                assignmentId = Model.Id, 
                                                                hasLinkColl = (Model.LinkCollection.Id != null)
                                                         },
                                             new AjaxOptions {
                                                 HttpMethod = "POST",
                                                 OnSuccess = "PxAssignment.ToggleSection",
                                                 InsertionMode = InsertionMode.Replace,
                                                 UpdateTargetId = childContainerId
                                             },
                                             new { @class = "expand" }) %>
        <% var contentUrl = Url.RouteUrl("FeaturedContentItem", new { id = section.Id, courseid = ViewContext.RouteData.Values["courseid"] }); %>
        <%= Ajax.ActionLink("Add Link", "AddLinkToCollection", "Assignment",
                        new { id = Model.Id, linkCollid = Model.LinkCollection.Id, linkTitle = HttpUtility.HtmlDecode(title), linkUrl = contentUrl },
                                    new AjaxOptions {
                                        OnBegin = "function(){ PxAssignment.ShowWaitOverlay('divSelectIntLink'); }",
                                            OnSuccess = "PxAssignment.OnIntLinkAdded", 
                                            UpdateTargetId = "content-item" },
                        new { Id = "lnk_" + section.Id, @class = "addIntLink" })%>
    </span>
    <span id="<%= childContainerId %>" class="children" style="display: none;"></span>
</div>
<% } %>
<input type="hidden" id="IntLinkPopupLoaded" value="true" />