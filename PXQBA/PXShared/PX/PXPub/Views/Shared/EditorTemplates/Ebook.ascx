<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>
<script type="text/javascript" language="javascript">
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/Module/PxUnit.js") %>']);
</script>
<%
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

    var title = (Model.Status == ContentStatus.New) ? "Create New Course Unit" : Model.Title;
    using (Html.BeginForm("SaveLesson", "PxUnit", FormMethod.Post, new { id = "saveItem" }))
    { %>  
        <h2 class="content-title titleunderline"><%= title %></h2>
        <div id="form" class="doc-collection">
            <%= Html.Hidden("Id", Model.Id) %>
            <%= Html.Hidden("ParentId", Model.ParentId) %>
            <%= Html.Hidden("Type", Model.Type) %>
            <%= Html.Hidden("Url", Model.Url) %>
            <%= Html.Hidden("IsAssignable", Model.IsAssignable) %>
            <% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
            <%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>
            
            <%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
            <input type="hidden" id="assignToCourse" />
            <input type="hidden" id="targetMode" value="normal" />
            <%=Html.HiddenFor(m => m.SourceTemplateId) %>
            <%=Html.HiddenFor(m => m.IsBeingEdited) %>
            <div class="create-new-wrapper">
            <ol>

                <% Html.RenderPartial("ItemEditorTemplate", Model); %>
                <li>
                   <%= Html.Label("Description") %>
                    <%= Html.TextArea("Description", Model.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>                    
                </li>                       
            </ol>
            </div>
            <div class="create-new-btn-wrapper">
                    <input type="hidden" id="DueDate" name="DueDate" value="<%=Model.DueDate %>"/>    
                    <label>&nbsp;</label>
                    <%if (CourseType.FACEPLATE.ToString().ToLowerInvariant() != Model.CourseInfo.CourseType.ToString().ToLowerInvariant())
                      { %>
                    <input type="button" name="behavior" value="Save & Open" class="savebtn submit-action" onclick="PXUnit.OnClickSaveandOpen(<%= OnSuccessSaveAndOpen %>)" />

                    <%} %>
                    <input type="button" name="behavior" value="Save" onclick="PXUnit.OnClickSave(<%= OnSuccess %>)" />
                   | <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>
            </div>
        </div>
<% } %>

