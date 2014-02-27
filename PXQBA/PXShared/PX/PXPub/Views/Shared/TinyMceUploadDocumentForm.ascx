<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Upload>" %>
<div id="doc-upload-window" >
        <% using (
               Html.BeginForm("ImageUpload", "Upload", FormMethod.Post,
                               new {
                                   enctype = "multipart/form-data",
                                   target = "upload-document-target"
                               })) {%>
        <iframe id="upload-document-target" name="upload-document-target" style="display:none"></iframe>
        <span class="help">Click 'Choose File' to select a document to upload from your computer.</span>
        <div id="uploadDocMessage" class="field-validation-error" style="color:red;display:none;">You have to select a file first</div>
        <table>
            <tr>
                <td style="text-align:right">File</td>
                <td style="text-align:left"><input id="uploadFile" name="UploadFile" type="file" /></td>
            </tr>
            <tr>
                <td style="text-align:right">&nbsp;</td>
                <td style="text-align:left">
                    <input type="submit" name="behavior" value="Save" id="submit-upload-file" class="confirm-buttons"/>
                </td>
            </tr>
        </table>      
        <span class="clear"></span>
         <% } %>
</div>