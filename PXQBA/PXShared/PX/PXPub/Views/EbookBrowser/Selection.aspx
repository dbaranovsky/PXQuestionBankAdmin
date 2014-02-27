<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.EbookBrowser>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderAdditions" runat="server">         

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
        <div id="right" style="padding-left: 0px">
         <div id="content-item">
            <% 
                Html.RenderAction("EbookSelection", "EbookBrowser"); 
                %></div>
    </div>
</asp:Content>


