<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
string contentType = ViewData["contentType"].ToString().ToLower().Trim();
string fileDescription = ViewData["fileDescription"].ToString().Trim();

switch (contentType)
{
    case "application/zip":%>
        <hr /><br />
        <a href="<%= Html.Encode(ViewData["targetUri"]) %>"><%= Html.Encode(fileDescription)%></a><br />
        (ZIP archive file; opens in new window)<br /><br />
        To open this ZIP file, you may need to download a program such as WinZip, <br />
        <a href="http://www.winzip.com" target="_blank">available as a free download.</a>
    <% break;
    case "application/pdf":%>
        <hr /><br />
        <a href="<%= Html.Encode(ViewData["targetUri"]) %>"><%= Html.Encode(fileDescription)%></a><br />
        (PDF document; opens in new window)<br /><br />
        <div style="float:left;width:100px; "><img src="<%= Url.Content("~/Content/images/pdf_view.jpg")%>" alt="pdf" /></div>
        <div style="float:left;">
        To view this document, you will need a PDF viewing application such as
        Adobe Reader, <br /><a href="http://www.adobe.com/products/reader.html" target="_blank">available as a free download.</a></div>
    <% break;
    case "application/ms-powerpoint":
    case "application/mspowerpoint":%>
        <hr /><br />
        <a href="<%= Html.Encode(ViewData["targetUri"]) %>"><%= Html.Encode(fileDescription)%></a><br />
        (PowerPoint document; opens in new window)<br /><br />
        <div style="width:600px;">
        <b>Windows Users:</b><br /><br />
        To save the document to your own computer, right-click the link above and select
        'Save Target As' or "Save Link As" from the pop-up menu. To open the file from its
        current location, just left-click the link.<br /><br />
        <img src="/Content/images/ppt_view.jpg" alt="ppt" /> To view this document, you will need a PPT viewing application. If you do not have
        Microsoft PowerPoint, the Microsoft PowerPoint Viewer is available as a free download
        for Windows.<br /><br />
        <b>Mac Users:</b><br /><br />
        To save the document to your own computer, control-click the mouse button on the
        link above, then select 'Copy Link to Disk' or 'Save Link As' from the pop-up menu. To
        open the file from its current location, simply click the link.<br /><br />
        <img src="<%= Url.Content("~/Content/images/ppt_view.jpg")%>" alt="ppt" /> To view this document, you will need a PPT viewing application such as Microsoft
        PowerPoint.
        </div>
    <% break;
    default: %>
        <hr /><br />
        <a href="<%= Html.Encode(ViewData["targetUri"]) %>"><%= Html.Encode(fileDescription)%></a><br />
        (Download file; opens in new window)<br /><br />
        To open this file, you may need to install the appropriate program to run it.<br />
    <% break;
}  
%>
