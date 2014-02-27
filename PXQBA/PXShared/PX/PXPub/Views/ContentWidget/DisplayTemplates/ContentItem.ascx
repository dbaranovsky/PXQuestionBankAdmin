<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<%--<h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %></h2>--%>

<div id="document-viewer-wrapper" class="arga-container">
<% 
    var hlDescription = Model.Title;
    var doc = new DocumentToView()
    {
        ItemId = Model.Id,
        Url = Model.Url,
        HighlightType = 1,
        HighlightDescription = hlDescription,
        AllowComments = Model.AllowComments,
        DisciplineId = Model.DisciplineId,
        NoteId = Model.NoteId
    };
    Html.RenderPartial("DocumentViewer", doc);
%>
</div>