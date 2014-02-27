<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<div id="fne-header-view">
    <!-- Home button -->
    <a href="#/state" id="fne-action-home" class="show-faceplate-home-icon faceplate-fne-home-icon">
        <span class="home-btn-icon"></span>Home</a>
    <!-- Title-->
    <div id="fne-title">
        <%= Model != null && Model.Content != null ? Model.Content.Title : string.Empty %>
    </div>
    <!-- Contains the higlight options -->
    <% if (Model != null && !Model.IsRelatedContent)
       { %>
        <!-- Contains FNE actions -->
        <div id="fne-actions">
        <%  if (Model.Content.AllowComments)
            { %>
            <div id="highlight-menu-container">
                <span id="content_widget_highlight_menu" class="action_menu"></span><span class="highlight-count">
                </span>
            </div>
        <%  } %>
       <div class="fne-edit-tabs">
            <%  
            Html.RenderAction("XbookContentOptions", "ContentAreaWidget",
                new
                {
                    id = Model.Content.Id,
                    mode = Model.ActiveMode,
                    renderInFne = true
                }
            );
            if (Model.IncludeNavigation.HasValue && Model.IncludeNavigation.Value)
            {
                Model.IncludeContentModes = false;
                Model.IncludeBreadcrumb = false;
                Html.RenderPartial("DisplayItemContentNav", Model);
            } %>
        </div></div>
    <% } %>
</div>