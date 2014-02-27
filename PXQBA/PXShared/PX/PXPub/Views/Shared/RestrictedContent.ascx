<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<div id="restrictedcontent" style="width:100%">
<div style="width:100%;background-color:#D0D3CC;margin-bottom:35px;font-size:large;">This content will be available after <%= DateTime.Parse(Model.RestrictedDate()).GetCourseDateTime().ToShortDateString() + " " + DateTime.Parse(Model.RestrictedDate()).GetCourseDateTime().ToShortTimeString() + " " + ViewData["TimeZone"].ToString()  %></div>
<b>Title:</b> <%=Model.Title %>
</div>

