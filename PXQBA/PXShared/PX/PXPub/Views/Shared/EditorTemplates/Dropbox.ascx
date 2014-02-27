
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Dropbox>" %>

<%      

    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";
%>

<% var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
<% using (Html.BeginForm("SaveDropbox", "Assignment", FormMethod.Post, new { id = "saveItem" }))
   { %>
    <div id="form" class="assignment">
        <%= Html.Hidden("Id", Model.Id)%>
        <%= Html.Hidden("ParentId", Model.ParentId)%>
        <%= Html.Hidden("Type", Model.Type)%>
        <%= Html.Hidden("Url", Model.Url)%>
        <%= Html.Hidden("IsAssignable", Model.IsAssignable)%>
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
              <%= Html.Label("Directions") %>
                <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>
            </li>
            </ol>
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
                       } %>
                
                    
                    <input type="button" name="behavior"  class="saveandopen submit-action" value="<%= saveAndOpenText %>" onclick="if(PxPage.ValidateTitle()){ PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccessSaveAndOpen %>});} else {return false;}" />    
                 
                    <% if (showSave)
                       { %>
                <input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="if(PxPage.ValidateTitle()){ PxContentTemplates.SetTemplateReloadMode('modal');PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccess %>}, ContentWidget.NavigateAway)}"/>
                    <% } %>                               
			   <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>               
        </div>
		</li>
    </ol>
</div>
</div>
