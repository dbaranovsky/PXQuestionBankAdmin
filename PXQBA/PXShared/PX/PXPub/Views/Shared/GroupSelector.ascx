<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div class="assignment-group-selector">
    <label><%= ViewData["GroupSelectorLabel"] %>:</label>
    <select id="ddlSettingsList" onchange="<%= ViewData["OnChangeEvent"] %>" <%= (bool)ViewData.GetValue("disabled", false) ? "disabled='disabled'": ""%>"></select>
</div>