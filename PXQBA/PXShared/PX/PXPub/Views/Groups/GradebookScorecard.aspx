<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<% if (((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
   { %>
       Gradebook
    <%} else { %>
        Scorecard
<%} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

<% if (((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
   { %>
        <span id="manage-groups">
        <%=Html.ActionLink("Manage Groups", "ManageGroupsFne", "Groups", new { @class = "fne-link" })%>
        </span>

        <%Html.RenderAction("ViewAll", "ProgressWidget"); %>
    <%} else { %>
        <%Html.RenderAction("Summary", "ProgressWidget"); %>
<%} %>

</asp:Content>