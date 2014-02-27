<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProductMinimal.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.ECommerceInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

<% Html.RenderPartial("ECommerceMarketing", Model); %>

<% Html.RenderPartial("ECommerceActionBar", Model); %>
<% Html.RenderAction("RenderFne","Course"); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
</asp:Content>
