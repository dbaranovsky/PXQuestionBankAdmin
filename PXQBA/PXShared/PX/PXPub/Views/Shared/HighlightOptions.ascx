<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentToView>" %>
<div id="highlight-links">
    <ul class="action-links">
        <% if (!Model.IsReadOnly) { %>
            <li>
                <span id="content_widget_highlight_menu" class="action_menu"></span>
                <span class="highlight-count"></span>
            </li>
        <% } %>    
    </ul>
</div>