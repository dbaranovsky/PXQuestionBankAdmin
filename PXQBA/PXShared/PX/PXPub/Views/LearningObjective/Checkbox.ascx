<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LearningObjective>" %>
<div>
    <input name="[<%= ViewData["index"] %>].Guid" type="hidden" value="<%= Model.Guid %>" />
    <input name="[<%= ViewData["index"] %>].Checked" type="checkbox" value="true" />
    <input name="[<%= ViewData["index"] %>].Checked" type="hidden" value="false" />
    <%= Model.Title %>
</div>