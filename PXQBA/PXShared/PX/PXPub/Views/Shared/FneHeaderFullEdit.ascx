<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<%
    Model.IncludeContentModes = false;
    //remove Results view mode (Full Screen FNE has a seperate Results view)
    Model.AllowedModes &= ~(ContentViewMode.Results);
    //remove 'View' tab - view is already handled by the FNE editor
    Model.AllowedModes &= ~(ContentViewMode.Preview);
    //add 'Edit' tab
    Model.AllowedModes = Model.AllowedModes | ContentViewMode.Edit;
    Model.IncludeNavigation = false; //don't include navigation in ContentModes links
    
    var doneUrl = Url.GetComponentHash("item", Model.Path,
                                new
                                    {
                                        mode = ContentViewMode.Preview,
                                        includeNavigation = true,
                                        isBeingEdited = false,
                                        renderFne = true,
                                        toc = Model.Toc
                                    }); %>
<div id="fne-header-edit">
    <!-- Done button -->

    <a itemurl="<%=doneUrl %>" href="javascript:" id="fne-done" class="show-faceplate-home-icon faceplate-fne-home-icon fne-done-link">
        <span class="doneEditing-btn-icon"></span>Done
        Editing
    </a>
    <div id="fne-title">
        <%= Model != null && Model.Content != null ? Model.Content.Title : string.Empty %>
    </div>
    <div class="fne-edit-tabs">
        <% Model.RenderContentModesInFne = true; %>
        <% Html.RenderPartial("ContentModes", Model, ViewData); %>
    </div>
</div>
