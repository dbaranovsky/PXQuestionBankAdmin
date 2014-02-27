<%@ Page Title="Image Browser" Language="C#" MasterPageFile="~/Views/Shared/Minimal.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Image Browser</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ImageUpload/ImageUpload.js") %>"></script>
    <link type="text/css" href="<%= Url.Content("~/Scripts/tiny_mce/plugins/netImageBrowser/css/browser.css") %>" rel="stylesheet"/>
    <link type="text/css" href="<%= Url.Content("~/Scripts/tiny_mce/plugins/netImageBrowser/css/jquery.contextMenu.css") %>" rel="stylesheet"/>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/tiny_mce/tiny_mce_popup.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/tiny_mce/plugins/netImageBrowser/js/netbrowser.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/tiny_mce/plugins/netImageBrowser/js/jquery.contextMenu.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/tiny_mce/plugins/netImageBrowser/js/jquery.jqURL.js") %>"></script>
    <%= ResourceEngine.IncludesFor("~/Scripts/grid.js") %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
<table>
    <tr>
        <td style="height: 300px; width: 160px; padding-left: 5px;" valign="top">            
            <div class="Menu">UPLOAD NEW IMAGE</div>
            <% Html.RenderAction("UploadImageForm", "ImageUpload", 
            new {
                parentId = "",
                onCompleteScript = "PxImageUpload.ReloadImageGrid",
                uploadType = UploadType.Default,
                uploadFileType = UploadFileType.Restricted
            }); %>
            <div class="Menu">SELECT IMAGE</div>
            <span class="help">Click on an image to the right to insert into text.</span>
        </td>
        <td style="height: 300px; width: 450px; padding-left: 10px;" valign="top"><br />
            <% Html.RenderPartial("ImageGrid"); %> 
        </td>
    </tr>
</table>
    <ul id="myMenu" class="contextMenu">
        <li class="paste"><a href="#paste">Select</a></li>
    </ul>
</asp:Content>