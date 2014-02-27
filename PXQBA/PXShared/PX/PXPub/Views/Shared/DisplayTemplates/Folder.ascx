<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Folder>" %>

<%var hasParentLesson = ViewData["hasParentLesson"];%>


<h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
    <% if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor && !Model.ReadOnly && Model.DefaultCategoryParentId.ToLowerInvariant() != "px_toc")
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
<div style="clear:both;"></div>

<div class="coverfpo">CoverFPO</div>

<% if (!string.IsNullOrEmpty(Model.Description))
   { %>
        <div class="html-container folder-title description-content"><%= System.Web.HttpUtility.HtmlDecode(Model.Description)%></div>
<% }
   else
   {
       
       %>
        <% if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
           {
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
            <div class="html-container folder-title description-content"><i>There is no description for this folder. <a href="<%=editUrl %>" class="nonmodal-link">Add some?</a></i></div>
            <%}
           else {%>
            <div class="html-container folder-title description-content">There is no description for this folder.</div>
            <% } %>
<% 
   } %>

<% if ((!Model.Items.IsNullOrEmpty() ||!Model.Folders.IsNullOrEmpty() ))
   { %>
<div class="folder-container">
    <div class="html-container folder-title description-content">Chapter Outline</div>
    <ul class="folderul">    
    <% if (Model.Folders.Count > 0)
       {
           foreach (var child in Model.Folders)
           { %>
            <li><a class="folder-child-link" href="#" rel="<%= child.Id %>"><%= child.Title%></a></li>
    <%      }
       } %>
    </ul>

<% if (!Model.Items.IsNullOrEmpty())
   { %>
    <span class="folder-items-title">Items</span>
    <ul class="folderul">
        <% foreach (var child in Model.Items)
           { %>
        <li><a class="folder-child-link" href="#" rel="<%= child.Id %>">
            <%= child.Title %></a></li>
        <% } %>
    </ul>
    <% } %>
</div>
<% } %>
<div style="clear:both;"></div>