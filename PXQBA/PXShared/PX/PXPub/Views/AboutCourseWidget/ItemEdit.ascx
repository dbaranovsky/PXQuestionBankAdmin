<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AboutCourse>" %>

<script src="<%: Url.Content("~/Scripts/jquery-1.5.1.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>AboutCourse</legend>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.InstructorName) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.InstructorName) %>
            <%: Html.ValidationMessageFor(model => model.InstructorName) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.OfficeHours) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.OfficeHours) %>
            <%: Html.ValidationMessageFor(model => model.OfficeHours) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.SyllabusURL) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.SyllabusURL) %>
            <%: Html.ValidationMessageFor(model => model.SyllabusURL) %>
        </div>

        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>
