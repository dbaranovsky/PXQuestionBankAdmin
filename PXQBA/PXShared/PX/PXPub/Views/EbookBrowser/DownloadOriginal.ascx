<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentCollection>" %>

<% 
    var doc = Model.Documents[0];
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
%>

<br />
<div>
    The file you selected was originally uploaded on: <span style="font-size:12px;font-weight:bold;"><%= Model.AvailableDate %></span> <br /><br /> with the file name: <span style="font-size:12px;font-weight:bold;"><%= doc.FileName %></span>
</div>
</br>
</br>
</br>

<%= Html.RouteLink("DownLoad", "DownloadDocument", new { id = doc.Id, name = doc.FileName, docId = Model.Id,   }, new { target = "_blank" })%>
<a href="" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>