<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<div id="content-item">
    <% if (Model != null && !String.IsNullOrWhiteSpace(Model.Url))
       { %>
        <iframe src='<%= Model.Url %>'></iframe>
    <% } %>
</div>