<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.LayoutConfiguration>" %>

<asp:Content ContentPlaceHolderID="HeaderAdditions" runat="server">    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.Title %>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

 <%Html.RenderPartial("PageContainer", Model.PageDefinitions);%>
</asp:Content>