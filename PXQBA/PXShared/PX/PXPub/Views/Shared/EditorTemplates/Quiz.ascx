<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
<%
	var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";    
	var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

%>


<% using (Html.BeginForm("SaveQuiz", "Quiz", FormMethod.Post, new { id = "saveItem" }))
   {        
	   %>
		<% var title = (Model.Status == ContentStatus.New) ? "Create New Quiz" : Model.Title; %>
		<div id="form" class="quiz">            
			<%= Html.HiddenFor(m => m.Id) %>
            <%= Html.HiddenFor(m => m.HostMode) %>
			<%= Html.HiddenFor(m => m.ParentId) %>
			<%= Html.HiddenFor(m => m.Type) %>
			<%= Html.HiddenFor(m => m.Url) %>
			<%= Html.HiddenFor(m => m.IsAssignable) %>
			<%= Html.HiddenFor(m => m.Sequence)%>
			<%= Html.HiddenFor(m => m.QuizType) %>
			<% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
			<%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>
			<%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
			<input type="hidden" id="targetMode" value="normal" />
			<%=Html.HiddenFor(m => m.SourceTemplateId) %>
			<%=Html.HiddenFor(m => m.IsBeingEdited) %>
			<div class="create-new-wrapper">
			    <ol class="formlist">
				    <% Html.RenderPartial("ItemEditorTemplate", Model); %>
				    <li>
				        <%= Html.Label("Directions") %>
					    <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style="visibility:hidden;width:auto;" })%>
				    </li>
                </ol>
            </div>

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

                                
                                <input type="button" name="behavior" class="savebtn saveandopen" value="<%= saveAndOpenText %>" onclick="if(PxPage.ValidateTitle()){ PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccessSaveAndOpen %>});} else {return false;}" />					
                                
					            <% if(showSave) 
					               { %>
					            <input class="savebtn submit-action" type="button" name="behavior" value="Save" onclick="if(PxPage.ValidateTitle()){ PxContentTemplates.SetTemplateReloadMode('modal');PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccess %>}, ContentWidget.NavigateAway)}" />
					            <% } %>
				             <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>
					            <%if (Model.Status != ContentStatus.New){%>                       
						            <%= Html.ActionLink("Edit Questions", "EditQuiz", "Quiz", new { id = Model.Id }, new { @class = "fne-link linkButton px-default-text edit-questions" }) %>
					            <%} %>
                        </div>
		            </li>
                </ol>
            </div>
                
		</div>
<% } %>
