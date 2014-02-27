<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Upload>" %>
<div id="doc-upload-window" >
<script type="text/javascript" language=javascript>
    (function ($) {
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
                    getDocument: { },
                    getElementById: { },
                    getElementByName: { },
                    getElementByTagName: { }
                }
            });
        }
    } (jQuery)); 

</script>

        <% using (
               Html.BeginForm("ImageUpload", "Upload", FormMethod.Post,
                               new {
                                   enctype = "multipart/form-data",
                                   target = "BFW_EasyXDM_Container"
                               })) {%>
        <%=Html.HiddenFor(m=>m.ParentId)%>
        <%=Html.HiddenFor(m=>m.UploadType) %>
        <%=Html.HiddenFor(m=>m.UploadFileType) %>
        <%=Html.HiddenFor(m => m.OnSuccessActionUrl)%>
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

        <iframe id="postFrame" name="postFrame" style="display:none"></iframe>
        <span class="help">Click 'Choose File' to select an image to upload from your computer.</span>
        <div id="uploadDocMessage" class="field-validation-error"></div>
        <span class="field buttons">
            <table width="100%">
                <tr>
                    <td style="text-align:right">File</td>
                    <td style="text-align:left"><input id="uploadFile" name="UploadFile" type="file" /></td>
                </tr>
                <tr>
                    <td style="text-align:right">&nbsp;</td>
                    <td style="text-align:left"><input type="submit" name="behavior" value="Save" id="" class="confirm-buttons" onclick="return PxUpload.OnImageUploadBegin(event,'<%=Model.UploadFileType %>');<%=Model.OnBeginScript %>" />
                    </td>
                </tr>
            </table>
        </span>        
        <span class="clear"></span>
         <% } %>
</div>