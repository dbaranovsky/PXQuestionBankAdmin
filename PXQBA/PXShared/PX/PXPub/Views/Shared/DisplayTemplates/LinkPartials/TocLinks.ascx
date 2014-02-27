<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QuickLink>" %>


<% if (Model.TocItem.IsNullOrEmpty()) { return; }
    
   foreach (var section in Model.TocItem) {
       var title = string.IsNullOrEmpty(HttpUtility.HtmlDecode(section.Title)) ? "No Title" : HttpUtility.HtmlDecode(section.Title);
       var childContainerId = string.Format("assign_children_of_{0}", section.Id);
       var linkText = "<ins class='" + section.ItemType + "'></ins>" + HttpUtility.HtmlDecode(title);
%>

<div class="level">
    <span class="expandContainer <%=section.ItemType %>" id="<%=section.Id %>">
    
        <%= Ajax.ActionLink("[replaceme]", "ExpandSection", "LinkLibrary",
                                                         new {  id = section.Id,
                                                                linkId = Model.Id
                                                         },
                                             new AjaxOptions {
                                                 HttpMethod = "POST",
                                                 OnSuccess = "PxLinkLibrary.ToggleSection",
                                                 InsertionMode = InsertionMode.Replace,
                                                 UpdateTargetId = childContainerId
                                             },
                                             new { @class = "expand addLinkItemClass" }).ToHtmlString().Replace("[replaceme]",linkText) %>
        <%
       var contentUrl = Url.Action("Index", "Content",
                                   new {
                                           id = section.Id,
                                           mode = ContentViewMode.ReadOnly,
                                           includeToc = false,
                                           includeDiscussion = false
                                       }); %>    
      
    </span>
    <a href="#" class="addLinkToNote"></a>   
    <input type="hidden" class="linkUrl" value="<%= contentUrl %>" />
    <input type="hidden" class="linkSearchedName" value="<%= HttpUtility.HtmlDecode(title) %>">
    <span id="<%= childContainerId %>" class="children" style="display: none;"></span>
    
</div>
<% } %>