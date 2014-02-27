<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

    <div id="assessment-settings-container">
        <% Html.RenderAction("Index", "AssessmentSettings", new { itemId = Model.Id }); %>
    </div>
