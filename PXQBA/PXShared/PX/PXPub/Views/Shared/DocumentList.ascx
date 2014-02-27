<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentCollection>" %>

<%                
    if (Model != null && !Model.Documents.IsNullOrEmpty())
    {
        foreach (var doc in Model.Documents)
        { 
%>  
    <li>
        <div class="docTitle"><%= Html.RouteLink(doc.Title.IsNullOrEmpty() ? doc.FileName : doc.Title, "DownloadDocument", new { id = doc.Id, name = doc.FileName, docId = Model.Id }, new { target = "_blank" }) %></div>
        <div class="docFilename"><%= doc.FileName %></div>
        <div class="docFilesize"><%= doc.Size / 1024 %>Kb</div>
        <div id="docControls">
            <input type="hidden" class="docId" value="<%= doc.Id %>" />
            <a href="#" onclick="return PxPage.ValidateDeleteDocResource(event);">Remove</a>
            <%= Ajax.ActionLink("Remove", "RemoveDocumentsFromCollection", "DocumentCollection", new { collectionId = Model.Id, docIds = doc.Id }, new AjaxOptions { OnBegin = "PxPage.Loading('fne-content')", OnComplete = "PxPage.Loaded('fne-content')", UpdateTargetId = "documentList" }, new { @class="ajaxRemove", style="display:none" }) %>
        </div>
    </li>
<%                                       
        }                    
    }
%>
