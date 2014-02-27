<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Folder>" %>

<%   
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";
    string randomId = Guid.NewGuid().ToString().Replace("-", string.Empty);
    
%>
    
<% using (Html.BeginForm("SaveFolder", "Folder", FormMethod.Post, new { id = "saveItem" }))
   { %>
        <% var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
        <div id="form" class="folder">            
            <%= Html.HiddenFor(m => m.Id)%>
            <%= Html.HiddenFor(m => m.ParentId)%>
            <%= Html.HiddenFor(m => m.Type)%>
            <%= Html.HiddenFor(m => m.Url)%>
            <%= Html.HiddenFor(m => m.IsAssignable)%>
            <%= Html.HiddenFor(m => m.Sequence)%>        
            <% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
            <%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>
            <%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
            <input type="hidden" id="targetMode" value="normal" />
            <%=Html.HiddenFor(m => m.SourceTemplateId) %>
            <%=Html.HiddenFor(m => m.IsBeingEdited) %>
            <div class="create-new-wrapper">
            <ol>

                <% Html.RenderPartial("ItemEditorTemplate", Model); %>
                <li>
                  <%= Html.Label("Description") %>
                    <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor", style = "visibility:hidden;width:auto;", id = "folder" + randomId })%>
                </li>
              </ol>
              </div>
              
                
        </div>
<% } %>
<div id="assign">
    <ol>
        <%if (Model.IsContentCreateAssign){  %>
        <li>
            <div id="assignContainer" class="contentcreate contentwrapper"><%Html.RenderAction("AssignTab", "ContentWidget", new { item = new ContentView { Content = Model, GroupId = Model.GroupId }, IsContentCreateAssign = true}); %></div>
        </li>
        <% } %>
        <li>
			<div class="create-new-btn-wrapper"> 
                    
                    <% 
                    var showSave = true;
                    var saveAndOpenText = "Save & Open";      
                    if (Model.IsBeingEdited)
                    {
                        showSave = false;
                        saveAndOpenText = "Save";
                        OnSuccessSaveAndOpen = OnSuccess;
                    } %>
                      <%if (CourseType.FACEPLATE.ToString().ToLowerInvariant() != Model.CourseInfo.CourseType.ToString().ToLowerInvariant())
                      { %>
                    <input type="button" name="behavior" value="<%= saveAndOpenText %>" onclick="if(PxPage.ValidateTitle(true)){ PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccessSaveAndOpen %>});} else {return false;}" />
                  <%} %>
                <% if(showSave) 
                    { %>
                    <input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="if(PxPage.ValidateTitle()){ PxContentTemplates.SetTemplateReloadMode('modal');PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }, updateSelector: '#content-item', success: <%= OnSuccess %> })}"/>                    
                <% } %>
                <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>
                </div>
		</li>
    </ol>
</div>
