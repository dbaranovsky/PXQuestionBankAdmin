<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Upload>" %>
<%
    var uploadFileName = ViewData["uploadFileName"].ToString();
%>

<div id="doc-upload-and-submit">
        <% using (
               Html.BeginForm("UploadAndSubmit", "Upload", FormMethod.Post,
                               new {
                                   enctype = "multipart/form-data",
                                   onsubmit =
                                   "PxPage.Loading('ui-dialog', true);return PxPage.AjaxForm.submit(this, { 'onComplete':function(response){ PxUpload.OnUploadComplete(response," + Model.OnCompleteScript + "); } })"
                               })) {%>
        <%=Html.HiddenFor(m=>m.ParentId)%>
        <%=Html.HiddenFor(m=>m.UploadType) %>
        <%=Html.HiddenFor(m=>m.UploadFileType) %>
        <%=Html.HiddenFor(m => m.OnSuccessActionUrl)%>
        <%=Html.HiddenFor(m => m.UploadFilePath)%>
        <%=Html.HiddenFor(m => m.AddToResourceMap)%>
        <%=Html.HiddenFor(m => m.UploadTitle)%>
        <%=Html.HiddenFor(m => m.RetainOriginalFile, new { @class = "RetainOriginalSubmittedFile" })%>
        <input type="hidden" id="tempOnSuccessActionUrl" value="<%=Model.OnSuccessActionUrl%>" />
        <%
            if (Model.CustomParams != null && Model.CustomParams.Count > 0)
            {
                var count = 0;
                foreach (var param in Model.CustomParams)
                {%>
            <input type="hidden" name="CustomParams[<%=count%>].Key" value="<%=param.Key%>"/>
            <input type="hidden" name="CustomParams[<%=count%>].Value" value="<%=param.Value%>"/>
        <%
                    count++;
                }
            }%>
            <%
            var supportedFiles = "(" + ".doc, .docx, .rtf";
            if (!Model.DownloadOnlyDocumentTypes.IsNullOrEmpty())
            {
                var downloanOnlytypes = Model.DownloadOnlyDocumentTypes.Split('|');
                foreach(string downloadtype in downloanOnlytypes)
                {
                    supportedFiles += ", ." + downloadtype;
                }
            }
            supportedFiles += ")";
            %>
        <p></p>
        <div id="uploadDocMessage" class="field-validation-error"></div>
            <%if (!uploadFileName.IsNullOrEmpty())
                { %>
                    <div class="currentSubmittedFile">
                        <div style="float:left;padding-right:10px;padding-top:8px;padding-left:4px;">
                            <div class="removeStudentSubmittedFile"></div>
                        </div>
                        <div style="float:left;padding-top:14px;"><%= uploadFileName%></div>
                    </div>
            <%} %>
            <div class="editcontainer">
                <div style="float:left;padding-right:10px;padding-top:7px;">File</div>
                <div style="float:left"><input id="uploadFile" name="UploadFile" type="file" /></div>
            </div>

            <div class="editcontainer">
                <div class="label">Comment</div>
                <div><%= Html.TextAreaFor(m => m.UploadComment, new { @class = "html-editor", @id = String.Format("Body_{0}", new Random().Next()), style = "visibility:hidden;width:auto;" })%></div>     
            </div>

            <div class="editcontainer">
                <input type="submit" name="behavior" value="Submit" id="" class="confirm-buttons"/>
                <input type="button" name="cancel" value="Cancel" class="confirm-buttons" onclick= "PxPage.CloseNonModal();PxUpload.CloseDialog(event);" />
            </div>
         <% } %>
</div>
