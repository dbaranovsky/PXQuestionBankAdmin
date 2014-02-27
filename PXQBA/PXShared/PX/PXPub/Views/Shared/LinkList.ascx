<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LinkCollection>" %>

<%                
    if (Model != null && !Model.Links.IsNullOrEmpty())
    {
            foreach (var link in Model.Links)
            { 
%>  
        <li>            
                <div class="docTitle">
                <a href="<%= link.Url %>"><%= link.Title.IsNullOrEmpty() ? link.Url : link.Title %></a>
            </div>
            <div class="docFilename">
                <a href="<%= link.Url %>"><%= link.Url %></a>
            </div>
            <div id="docControls">
                <input type="hidden" class="docId" value="<%= link.Id %>" />
                <a href="#" onclick="return PxPage.ValidateDeleteLinkResource(event);">Remove</a>
                <%= Ajax.ActionLink("Remove", "RemoveLinksFromCollection", "LinkCollection", new { collectionId = Model.Id, linkIds = link.Id, isBeingEdited = Model.IsBeingEdited }, new AjaxOptions { OnBegin = "PxLinkCollection.OperationLinkBegin", OnComplete = "PxLinkCollection.OperationLinkComplete", OnSuccess = "PxUpload.DocumentLoaded", UpdateTargetId = "linkList" }, new { @class = "ajaxRemove", style = "display:none" })%>
            </div>
            <br />
            <br />
        </li>
<%                                       
            }                    
    }
%>