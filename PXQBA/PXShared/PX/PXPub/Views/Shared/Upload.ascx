<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Upload>" %>
<div id="doc-upload-window" class="divPopupWin upload-window">
    <h2 id="doc-title" class="divPopupTitle">
        <span>Upload a Document </span><a href="#" id="doc-unblock-action" class="divPopupClose"></a>
    </h2>

    <div id="doc-content" class="divPopupContent px-default-text">

<script type="text/javascript" language=javascript>
    (function ($) {

        PxPage.OnReady(function () {
            if (typeof (UploadRPC) == 'undefined') {
                UploadRPC = new easyXDM.Rpc({
                    remote: '<%= Url.ActionCache("Provider", "EasyXDM") %>'
                }, {
                    local: {
                        ResponseCallBack: function (id, domain, location, response) {
                            if (location == 'about:blank') {
                                return;
                            }
                            PxUpload.OnUploadComplete(response, <%= Model.OnCompleteScript %>);
                        },
                        FinishedLoading: function () {
                            UploadRPC.getElementById('UploadDocument', 'EasyXDM_response');
                        }
                    },
                    remote: {
                        getDocument: {},
                        getElementById: {},
                        getElementByName: {},
                        getElementByTagName: {}
                    }
                });
            }
        });
    } (jQuery));    

</script>

        <% using (
               Html.BeginForm("Upload", "Upload", FormMethod.Post,
                               new { enctype = "multipart/form-data", target = "BFW_EasyXDM_Container" }))
           {%>
        <%=Html.HiddenFor(m=>m.ParentId)%>
        <%=Html.HiddenFor(m=>m.UploadType) %>
        <%=Html.HiddenFor(m=>m.UploadFileType) %>
        <%=Html.HiddenFor(m => m.OnSuccessActionUrl)%>
        <%=Html.HiddenFor(m => m.UploadFilePath)%>
        <%=Html.HiddenFor(m => m.AddToResourceMap)%>
        <%=Html.HiddenFor(m => m.UploadTitle)%>
        <input type="hidden" id="tempOnSuccessActionUrl" value="<%=Model.OnSuccessActionUrl%>" />
        <input type="hidden" id="domain" name="Domain" value="" />
        <input type="hidden" id="onCompleteScript" name="onCompleteScript" value="<%=Model.OnCompleteScript %>" />

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

        <iframe id="postFrame" name="postFrame" style="display:none"></iframe>

        <span class="help">Click the 'Choose File' button to select a document to upload from your computer. Supported file types <%= supportedFiles%>.</span>
        <p></p>
        <div id="uploadDocMessage" class="field-validation-error"></div>
        <span class="field buttons">
            <table width="100%">
                <tr>
                    <td style="text-align:right"><label for="uploadTitle">Document Title</label></td>
                    <td style="text-align:left">
                        <input id="uploadTitle" name="UploadTitle" type="text" onblur="PxUpload.AddWaterMark();" onfocus="PxUpload.ClearWaterMark();"/>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right">File</td>
                    <td style="text-align:left"><input id="uploadFile" name="UploadFile" type="file" /></td>
                </tr>
                <tr>
                    <td style="text-align:right">&nbsp;</td>
                    <td style="text-align:left"><input type="submit" name="behavior" value="Upload" id="" class="confirm-buttons btnUploadDoc" onclick="return PxUpload.OnUploadBegin(event,'<%=Model.UploadFileType %>','<%=Model.DownloadOnlyDocumentTypes %>');<%=Model.OnBeginScript %>" />
                        <input type="button" name="cancel" value="Cancel" class="confirm-buttons" onclick= "PxPage.CloseNonModal();PxUpload.CloseDialog(event);$(PxPage.switchboard).trigger('removenewnode');" /></td>
                </tr>
            </table>
            
            
            
        </span>
        <span class="clear"></span>
         <% } %>
    </div>
</div>
