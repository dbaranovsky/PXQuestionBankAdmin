<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProductMinimal.Master"
    Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Confirmation
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
    <div id="PX_HOME_ZONE_2" class="zoneParent">
        <% Html.RenderPartial("EnrollmentConfirmationForm"); %>
    </div>
    <div id="PX_HOME_ZONE_3" class="zoneParent">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
</asp:Content>