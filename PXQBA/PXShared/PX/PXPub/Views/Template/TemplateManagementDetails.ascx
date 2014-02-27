<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<% if (Model.Type != "Quiz") { %>
    <div id ="divBhIFrameComponent">
        <input id = "txtEnroll" type="hidden" value="" />
        <%
       Html.RenderPartial("BhIFrameComponent", new BhComponent() {
           ComponentName          = "ItemEditor",
           Parameters             = new {
               Id                 = "itemsettings",
               EnrollmentId       = ViewData["EnrollmentId"],
               ItemId             = Model.Id,
               GroupId            = "",
               ShowOnlyProperties = true,
               ShowSave           = false
           }
       });
        %>
    </div>

<% } else { %>
    <div id="assessment-template-form">
        <% Html.RenderAction("Settings", "ContentWidget", new { id = Model.Id, isTemplateEditor = true }); %>
    </div>
<% } %>

<span class="tplmanagment-btns"><br />
<button class="save-template" onclick="PxSettingsTab.SaveTemplate('<%= Model.Id %>');">Save Template</button>
<button class="save-template-as" onclick="PxSettingsTab.OpenSaveAs();">Save Template As ...</button>
<button class="btnTemplateCancel" id="btnTemplateCancel" onclick ="PxSettingsTab.CloseDialog();">Cancel</button>
</span>