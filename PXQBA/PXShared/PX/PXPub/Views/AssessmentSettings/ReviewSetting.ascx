<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ReviewOption>" %>
<div class="review-setting" id="review-setting-<%= Model.Key %>">
    <div class="setting-large-wrapper"><%: Html.LabelFor(Model.Option) %></div>
    <div style="height: 40px">
        <%: Html.RadioButtonFor(Model.Option, "Each", new { @class = "setting-header setting-header-each" })%>
        <% if (Model.AssessmentSettings.AssessmentType == AssessmentType.Homework)
            { %>
            <%: Html.RadioButtonFor(Model.Option, "Second", new { @class = "setting-header" })%>
            <%: Html.RadioButtonFor(Model.Option, "Final", new { @class = "setting-header" })%>
        <% } %>
        <%: Html.RadioButtonFor(Model.Option, "DueDate", new { @class = "setting-header setting-header-due-date" })%>
        <%: Html.RadioButtonFor(Model.Option, "Never", new { @class = "setting-header setting-header-never" }) %>
    </div>
</div>