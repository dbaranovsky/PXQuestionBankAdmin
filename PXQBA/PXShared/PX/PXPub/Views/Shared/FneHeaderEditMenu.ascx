<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%
    Model.IncludeContentModes = false;
    //save original allowed modes - ensure we dont change it
    var allowedModesOriginal = Model.AllowedModes;
    //remove Results view mode (Full Screen FNE has a seperate Results view)
    Model.AllowedModes &= ~(ContentViewMode.Results);
    //remove 'View' tab - view is already handled by the FNE editor
    Model.AllowedModes &= ~(ContentViewMode.Preview);
    //add 'Edit' tab
    Model.AllowedModes = Model.AllowedModes | ContentViewMode.Edit;
    var includeNavigation = Model.IncludeNavigation;
    Model.IncludeNavigation = false; //don't include navigation in ContentModes links
    Model.RenderContentModesInFne = true;
    %>
    
<div id="fne-edit-menu">
        <% Html.RenderPartial("ContentModes", Model, ViewData); %>
</div>

<%
    //preserve old include navigation value
    Model.IncludeNavigation = includeNavigation;
    Model.AllowedModes = allowedModesOriginal;
%>