<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Bfw.PX.PXPub.Models.ContentItem>>"  MasterPageFile="~/Views/Shared/Home.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderAdditions" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
   
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CenterContent" runat="server" >
<% Html.RenderAction("LoadPage", "PageAction", new { pageId = "PX_DASHBOARD_1COLUMN" }); %>
    
</asp:Content>