<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.HtmlDocument>" %>
<%   
    var cancel = "PxPage.TriggerHtmlSave(); return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    bool disableButtonOnSaved = ViewData["DisableButtonOnSaved"] == null ? false : (bool)ViewData["DisableButtonOnSaved"];

    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

    string action = "SaveHtmlDocument";
    string controller = "HtmlDocument";
    var updateSelector = "#content-item";

    using (Html.BeginForm(action, controller, new { name = (ViewData["itemParentId"] as string) }, FormMethod.Post, new { id = "saveItem" }))
    { %>
        <% var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
        <h2 class="content-title"><%= title%></h2>
        <div id="form" class="html-doc">            
            <%= Html.HiddenFor(m => m.Id)%>
            <%= Html.HiddenFor(m => m.ParentId)%>
            <%= Html.HiddenFor(m => m.Type)%>
            <%= Html.HiddenFor(m => m.Url)%>
            <%= Html.HiddenFor(m => m.IsAssignable)%>
            <%= Html.HiddenFor(m => m.Sequence)%>         
            <% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
            <%= Html.HiddenFor(m => m.DefaultCategoryParentId)%>
            <%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
            <input type="hidden" id="targetMode" value="normal" />
            <%=Html.HiddenFor(m => m.SourceTemplateId)%>
            

            <%=Html.HiddenFor(m => m.IsBeingEdited)%>
            <% if (!ViewData.ModelState.IsValid)
               { %>
                <input type="hidden" id="content_save_failed" name="content_save_failed" value="1" />
            <% } %>
            <div class="create-new-wrapper">
            <ol>
                
                <% Html.RenderPartial("ItemEditorTemplate", Model); %>
                
                <li>
                    <%= Html.LabelFor(m => m.Body)%>
                    <%= Html.TextAreaFor(m => m.Body, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>                    
                </li>
                </ol>
                </div>
                
            <div id="confirm" style="display: none">Are you sure you want to close without saving?</div>
        </div>
<% } %>
<div id="assign">
    <ol>
        <%if (Model.IsContentCreateAssign)
          {  %>
        <li>
            <div id="assignContainer" class="contentcreate contentwrapper">
                <%Html.RenderAction("AssignTab", "ContentWidget", new { item = new ContentView { Content = Model, GroupId = Model.GroupId }, IsContentCreateAssign = true }); %></div>
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
                    
                <% 
                    if (Model.ParentId != "PX_COURSE_MATERIALS" && (!Model.Categories.IsNullOrEmpty() && Model.Categories.ToList().Exists(i => i.Id.ToLowerInvariant() != "eportfolio_compose")))
                       {%>
                    <input type="button" name="behavior" class="saveandopen submit-action" value="<%= saveAndOpenText %>" onclick="if(PxPage.ValidateTitle()){PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '<%=updateSelector%>', success: <%= OnSuccessSaveAndOpen %> });} else {return false;}" />
                    <% } %>

                    <% if(showSave) 
                       { %>
                        <input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="if(PxPage.ValidateTitle()){ <%=disableButtonOnSaved? "$(this).prop('disabled', true);":"" %> PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '<%=updateSelector%>', success: <%= OnSuccess %> }, ContentWidget.NavigateAway)}"/>                    
                    <% } %>                  
                    
<a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>             </div>
        </li>
    </ol>
</div>
