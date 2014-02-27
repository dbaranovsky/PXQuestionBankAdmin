<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<%
    var toggleSectionConfiguration = (ToggleSectionConfiguration)ViewData["ToggleSectionConfiguration"];

    var HeaderTitle = toggleSectionConfiguration.HeadTitle;
    var openedState = toggleSectionConfiguration.IsOpenByDefault ? "" : "display:none";
    var closedState = toggleSectionConfiguration.IsOpenByDefault ? "display:none" : "";
    var isItemLocked = toggleSectionConfiguration.IsItemLocked ? "disabled" : "";
%>

<%if ( toggleSectionConfiguration.IsHideTopArea == false ) { %>
<div class="sectiontitle" style="width:100%"><%=HeaderTitle %></div>
<%} %>
