<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentToView>" %>
<%
var uri = new Uri(Model.Url, UriKind.RelativeOrAbsolute);

string contentType = string.Empty;
try
{
    contentType = VirtualPathUtility.GetExtension(Model.Url).TrimStart('.');
}
catch (Exception){}

string fileDescription = Model.HighlightDescription;

string targetUri = Model.Url;
string pdfImgPath = Url.Content("~/Content/images/pdf_view.jpg");
string pptImgPath = Url.Content("~/Content/images/ppt_view.jpg");

switch (contentType)
{
    case "zip":%>
        <hr /><br />
        <a href="<%= targetUri %>" target="_blank"><%= Html.Encode(fileDescription)%></a><br />
        (ZIP archive file; opens in new window)<br /><br />
        To open this ZIP file, you may need to download a program such as WinZip, <br />
        <a href="http://www.winzip.com" target="_blank">available as a free download.</a>
    <% break;
    case "pdf":%>
        <hr /><br />
        <a href="<%= targetUri %>" target="_blank"><%= Html.Encode(fileDescription)%></a><br />
        (PDF document; opens in new window)<br /><br />
        <div style="float:left;width:100px; "><img src="<%= pdfImgPath%>" alt="pdf" /></div>
        <div style="float:left;">
        To view this document, you will need a PDF viewing application such as
        Adobe Reader, <br /><a href="http://www.adobe.com/products/reader.html" target="_blank">available as a free download.</a></div>
    <% break;
    case "ppt":
    case "pptx":
    case "pptm":%>
        <hr /><br />
        <a href="<%= targetUri %>" target="_blank"><%= Html.Encode(fileDescription)%></a><br />
        (PowerPoint document; opens in new window)<br /><br />
        <div style="width:600px;">
        <b>Windows Users:</b><br /><br />
        To save the document to your own computer, right-click the link above and select
        'Save Target As' or "Save Link As" from the pop-up menu. To open the file from its
        current location, just left-click the link.<br /><br />
        <img src="<%= pptImgPath%>" alt="ppt" /> To view this document, you will need a PPT viewing application. If you do not have
        Microsoft PowerPoint, the Microsoft PowerPoint Viewer is available as a free download
        for Windows.<br /><br />
        <b>Mac Users:</b><br /><br />
        To save the document to your own computer, control-click the mouse button on the
        link above, then select 'Copy Link to Disk' or 'Save Link As' from the pop-up menu. To
        open the file from its current location, simply click the link.<br /><br />
        <img src="<%= pptImgPath%>" alt="ppt" /> To view this document, you will need a PPT viewing application such as Microsoft
        PowerPoint.
        </div>
    <% break;
    default: %>
        <hr /><br />
        <a href="<%= targetUri %>" target="_blank"><%= Html.Encode(fileDescription)%></a><br />
        (Download file; opens in new window)<br /><br />
        To open this file, you may need to install the appropriate program to run it.<br />
    <% break;
}  
%>
