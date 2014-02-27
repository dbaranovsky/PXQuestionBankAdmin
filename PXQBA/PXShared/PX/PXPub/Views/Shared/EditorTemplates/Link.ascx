<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Link>" %>

<%
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";
%>

<% using (Html.BeginForm("SaveLink", "Link", FormMethod.Post, new { id = "saveItem" }))
   { 
       var title = Model.Title;
       
       if (Model.Status == ContentStatus.New)
       {
           title = "Create New Activity";
           Model.Url = string.Empty;
       }       
       
%>
        <div id="form" class="link-collection">
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
                <!--li>
                  <%= Html.Label("Directions")%>
                    <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>
                </li-->        
            </ol>
            
            <ol>
                <li>
                    <label for="linkUrl">URL</label><em><label>required</label></em>
                    <input id="linkUrl" name="linkUrl" type="text" class="title" value="<%= Model.Url %>" />  
                    <span id="spnLinkError"></span>
                    <%= Html.ValidationMessage("linkUrl")%>
                    <%= Html.ValidationMessage("content.linkUrl")%>
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
                    
                   
                    <input type="button" name="behavior" class="saveandopen submit-action" value="<%= saveAndOpenText %>" onclick="PxLinkCollection.OnClickSave(<%= OnSuccessSaveAndOpen %>, true);" />                    
                 
                    <% if (showSave)
                       { %>
                  
                    <input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="PxLinkCollection.OnClickSave(<%= OnSuccess %>, false)" />                   
                    <% } %> 
                  <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>
                </div>
		</li>
    </ol>
</div>
                    </div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            var deps = ['<%= Url.ContentCache("~/Scripts/LinkCollection/LinkCollection.js") %>'];
            PxPage.Require(deps, function () {
                PxLinkCollection.BindControls();
            });
        });
    } (jQuery));    
</script>