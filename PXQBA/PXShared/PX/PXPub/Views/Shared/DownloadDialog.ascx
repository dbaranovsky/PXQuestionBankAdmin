<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<% string docId = ViewData["DocId"] == null ? string.Empty : ViewData["DocId"].ToString(); %>
<div style="display:none" class="downloadBox px-default-text" data-doc-id="<%=docId %>">

<%
   using (Html.BeginForm("ResourceDocuments", "Download", new { id = Model.Id }, FormMethod.Post, new { id = "frmDownload" }))
        { %>
            Select Format
            <select id="downloadFormat">
                <option value="doc" selected="selected">Microsoft Word 97-2003(.doc)</option>
                <option value="docx">Microsoft Word 2007(.docx)</option>
                <option value="pdf">Pdf</option>
            </select>
            <div class="dialog-actions">
                <input type="submit" name="behavior" value="Download" id="btnDownloadConfirm" class="btnDownloadConfirm" />
                <input type="button" value="Cancel" id="btnCancelDownload" class="btnCancelDownload" />
            </div>
        <input type="hidden" id="zipFileName" value="<%=HttpUtility.HtmlDecode(Model.Title) %>" name="zipFileName" />
        <input type="hidden" id="format" name="format" />
        <input type="hidden" id="documentIds" name="documentIds" value="<%=docId %>" />
        <input type="hidden" id="downloadtype" name="downloadtype" />
    <% } %>

</div>
