<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TocWidget>" %>

<div class="toc">
<% if (!Model.Children.IsNullOrEmpty())
   { %>
    <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/ExpandSection.ascx", Model.Children); %>
<% } 
   else
   { %>
        <span class="noItemsMessage">No Table of Contents</span>
<% } %>
</div>