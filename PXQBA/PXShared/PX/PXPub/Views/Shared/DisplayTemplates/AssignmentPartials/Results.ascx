<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Assignment>" %>

<h2 class="content-title">
    <%= HttpUtility.HtmlDecode(Model.Title) %>
    <% if (!Model.ReadOnly) { %>
    <input type="hidden" value="false" id="IsModelReadonly" />
    <% } %> 
</h2>

<%var hideCommentsCol = false; %>
<div id="results-page" type="<%=Model.SubType %>">   
    <input type="hidden" id="hideCommentsCol" value="<%=hideCommentsCol%>" />

    <div class="px-default-text">
        <%
        foreach (var policy in Model.Policies) { %>
            <div class="policy"><%= policy %></div>
        <% } %>
    </div>
    <% if (Model.DueDate.Year == DateTime.MinValue.Year) {%>
        <div class="result-message"><span class="px-default-text">This assignment has not yet been assigned.</span> </div>
    <% return; } %>
    <div class="result-message"><span id="resultsTableMessage" class="px-default-text">Please wait: Checking for student submission...</span> </div>
    <div id="divResults" class="results-table-container">
        <%=Html.Hidden("gridAjaxUrl", Url.Action("GetStudentsSubmissionInfo", "Assignment", new { id = Model.Id }))%>
        <%=Html.Hidden("assignment-title", Model.Title)%>
        
        <table id="gridResults" class="pxTable"></table>
        
        <% if (!Model.ReadOnly)
           { %>
            <div class="grid-actions" id="divgridActions">
                <a href="" id="lnkDownloadDocs" class="px-default-text linkButton">
                    Download
                </a>
            </div>
            <%
                using (
                    Html.BeginForm("Submissions", "Download",
                        new { id = Model.Id },
                        FormMethod.Post,
                        new
                        {
                            id = "frmDownload"
                        }))
                {%>
            <div style="display:none;" class="downloadBox px-default-text">
                Select Format
                <select id="downloadFormat">
                    <option value="doc" selected="selected">Microsoft Word 97-2003(.doc)</option>
                    <option value="docx">Microsoft Word 2007(.docx)</option>
                    <option value="pdf">Pdf</option>
                </select>
                <div class="dialog-actions">
                    <input type="submit" name="behavior" value="Download" id="btnDownloadConfirm"/>
                    <input type="button" value="Cancel" id="btnCancelDownload"/>
                </div>
            </div>
            <input type="hidden" id="zipFileName" value="<%=HttpUtility.HtmlDecode(Model.Title) %>" name="zipFileName" />
            <input type="hidden" id="format" name="format" />
            <input type="hidden" id="documentIds" name="documentIds"/>
            <% } %>
        <% } %>
    </div>
</div>
<script type="text/javascript">
    PxPage.Require(['<%=Url.ContentCache("~/Scripts/jquery/jquery.jqGrid.min.js") %>', '<%= Url.ContentCache("~/Scripts/Assignment/Assignment.js") %>'], function () {
        PxAssignment.Init();
    });
</script>
