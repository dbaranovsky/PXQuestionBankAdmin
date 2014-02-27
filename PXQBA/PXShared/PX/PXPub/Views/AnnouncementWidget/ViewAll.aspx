<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.AnnouncementWidget>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
	ViewAll
</asp:Content>

<asp:Content ContentPlaceHolderID="CenterContent" runat="server">
    <%  Html.RenderPartial("BhIFrameComponent", new BhComponent() { ComponentName = "AnnouncementList" }); %>
</asp:Content>