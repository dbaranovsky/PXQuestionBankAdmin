<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderAdditions" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
    Upgrade
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CenterContent" runat="server">
    <script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/jquery/jquery-1.4.1.min.js") %>"></script>

    <% Html.RenderAction("AuthenticationScripts", "Account"); %>
    <div id="MySite_page_grayed" style="display:none;"></div>  
 <div id="RAif_div" style="margin-top:10px;"></div>
 <div id="MySite_UserInfo_LoggedIn" class="MySite_module">
            <div><p>You are logged in as : <div id="MySite_RA_UserName"></div></p></div>
            <br />
            <div><a href="JavaScript:MySite_RAif_init('dologout')">log out</a></div>
            <br />         
             <p>
             <p>To View, please purchase the product</p>
             <p><input id="btnPurchase" type="button" value="Click here to Purchase" onclick="JavaScript:MySite_RAif_init('codeorcart');" /></p>
            </p>     
  </div>  
  <div id="login-status"></div>
  <div ID="RAServerOutput" runat="Server" style="display:none;"></div>
   
</asp:Content>
