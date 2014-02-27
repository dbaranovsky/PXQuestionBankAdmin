<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentCollection>" %>
<%
    
    var parentLessonId = ExtensionMethods.GetMultipartPartLessonId(Request, ViewData);
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
   
    
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";
   
    string action = "SaveDocumentCollection";
    string controller = "DocumentCollection";
    
%>
<% var behaviorType = "";
    
    //Just want to point out, this is likely a hack to fix an issue with the iframe workflow of the OnFormSubmit method in PxCommon.
    //If it ever gets fixed we don't need this.
    if (Model.IsBeingEdited)
    {
        behaviorType = "save";
    }
    var formAction = Url.Action(action, controller, new { behavior = behaviorType }); %>
<form method="post" action="<%= formAction %>" enctype="multipart/form-data" id="saveItem">
<% var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
<div id="form" class="doc-collection">
    <%= Html.Hidden("Id", Model.Id) %>
    <%= Html.Hidden("ParentId", Model.ParentId) %>
    <%= Html.Hidden("Type", Model.Type) %>
    <%= Html.Hidden("Url", Model.Url) %>
    <%= Html.Hidden("IsAssignable", Model.IsAssignable) %>
    <%= Html.HiddenFor(m => m.Sequence) %>
    <% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
    <%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>
    <%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
    <input type="hidden" id="targetMode" value="normal" />
    <%=Html.HiddenFor(m => m.SourceTemplateId) %>
    <%=Html.HiddenFor(m => m.IsBeingEdited) %>
    <%=Html.HiddenFor(m => m.DocumentCollectionSubType) %>
    <input type="hidden" id="hasParentLesson" name="hasParentLesson" value="<%= parentLessonId %>" />
    <div class="create-new-wrapper">
    <ol>

        <% Html.RenderPartial("ItemEditorTemplate", Model); %>
        <li>
            <%= Html.Label("Description (optional)")%>
            <%= Html.TextArea("Description", Model.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>
        </li>
    </ol>

    <% 
           var showSave = true;
           var saveAndOpenText = "Save & Open";
           if (Model.IsBeingEdited)
           {
               showSave = false;
               saveAndOpenText = "Save";
           } %>
            
            <ol>
             <li>
                <br />
                <label class="sub-title">Documents</label>
             </li>
            
                <div id="documentList">
                    <% Html.RenderAction("DocumentList", "DocumentCollection", new { collection = Model }); %>
                </div>   

                <li>

                    <div id="editForm" style="padding-right:20px;">
                        <%if (string.IsNullOrEmpty(Model.ApplicableEnrollmentId))
                        {%>
                            <div class="doc-collection-content-view"></div>
                            <form action="" method="post" id="formUpload"></form>
                            <a href="#" class="px-default-text upload-link" rel="docUploadWindow">Attach a document</a>
                      <%} %>

                        <%
                        var uploadCustomParam = new Dictionary<string, string>
                                            {
                                            };
                        var onSuccessUrl = Url.Action("DocumentList", "DocumentCollection", new
                                                                                  {
                                                                                      collectionId = Model.Id
                                                                                  });
                        %>
            
                            <% Html.RenderAction("UploadForm", "Upload", new
                                                                 {
                                                                     parentId = Model.Id,
                                                                     onCompleteScript = "ContentWidget.OnUploadComplete",
                                                                     uploadType = UploadType.Default,
                                                                     uploadFileType = UploadFileType.Any,
                                                                     onSuccessActionUrl = onSuccessUrl,
                                                                     uploadCustomParam = uploadCustomParam
                                                                 }); %>
                    </div>

                </li>
            </ol>      
     </div>
     <div class="create-new-btn-wrapper">
        <input type="button" name="behavior" class="saveandopen submit-action" value="<%= saveAndOpenText %>" onclick="if(PxPage.ValidateTitle()){PxPage.PrepareDocForm();PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }, rules: { docTitle: { required: '#docFile:filled' }, docFile: { required: '#docTitle:filled' } } ,updateSelector: '#content-item', iframe: { frameContainer: '#content-item' }, success: <%= OnSuccessSaveAndOpen %> })} else { return false; }" />                
        <% if (showSave)
            { %>
        <input type="button" name="sub" value="Save" class="savebtn submit-action" onclick="if(PxPage.ValidateTitle()){ PxContentTemplates.SetTemplateReloadMode('modal');PxPage.PrepareDocForm();PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }, rules: { docTitle: { required: '#docFile:filled'}, docFile: { required: '#docTitle:filled' }}, iframe: { frameContainer: '#content-item' }, success: <%= OnSuccess %>}, ContentWidget.NavigateAway)}" />
        <% } %> 

        <%--<%if (Model.Status != ContentStatus.New)
            {%>
        <%= Html.ActionLink("Edit Questions", "EditQuiz", "Quiz", new { id = Model.Id }, new { @class = "fne-link linkButton px-default-text" }) %>
        <%} %>--%>
     </div>
</div>
</form>

<%
    var isScriptLoaded = ViewData["isScriptLoaded"] == null ? false : Convert.ToBoolean(ViewData["isScriptLoaded"]);
    if (!isScriptLoaded)
    {
%>        
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {           
            
            if (tinyMCE.activeEditor) {
                try {
                    tinyMCE.activeEditor.remove();
                }
                catch (ex) {
                }
            } 

        });
    } (jQuery));
</script>
<%
    }
%>