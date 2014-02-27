<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LinkCollection>" %>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
                var deps = ['<%= Url.ContentCache("~/Scripts/LinkCollection/LinkCollection.js") %>', '<%= Url.ContentCache("~/Scripts/Assignment/Assignment.js") %>'];
                PxPage.Require(deps, function () {
                    PxAssignment.BindControls();
                    PxLinkCollection.BindControls();
                });

            });
    } (jQuery));    
</script>

<%
    var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

%>

<% using (Html.BeginForm("SaveLinkCollection", "LinkCollection", FormMethod.Post, new { id = "saveItem" }))
   { 
       var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
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
                <li>
                  <%= Html.Label("Directions")%>
                    <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>
                </li>        

                <li>
                    <br />
                    <label class="sub-title">Links</label>
                </li>
            
                <div id="linkList">
                    <% Html.RenderAction("LinkList", "LinkCollection", new { collection = Model }); %>
                </div>

                    <%if (Model.IsContentCreateAssign){  %>
                    <li>
                        <div id="assignContainer" class="contentcreate contentwrapper"><%Html.RenderAction("AssignTab", "ContentWidget", new { item = new ContentView { Content = Model, GroupId = Model.GroupId }, IsContentCreateAssign = true}); %></div>
                    </li>
                    <% } %>


                    <li id="editForm">
                        <a href="#" id="lnkAddLink" class="px-default-text linkOpenPopup link" rel="divLinkWin">Attach a link</a>
                    </li>

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
                              <a href="#" class="create-closecancel" onclick="<%= cancel %>" style="display:none">Cancel</a>
                        </div>
		            </li>
                </ol>

            </div>
               
        </div>
<% } %>

<div id="divLinkWin" class="divPopupWin">
    <div id="divLinkContent" class="divPopupContent">
                    
        <%  var onSuccessAddLink = "";
            if (Model.CourseInfo.CourseType.ToString().ToLower() == "faceplate") {
                onSuccessAddLink = "PxLinkCollection.CloseAddResourse();";
        } else { 
                    onSuccessAddLink = "ContentWidget.ContentCreated(null);PxLinkCollection.CloseAddResourse();";
            } %>

        <% using (Ajax.BeginForm("AddLinkToCollection", "LinkCollection", new { isBeingEdited = Model.IsBeingEdited }, new AjaxOptions() { OnBegin = "PxLinkCollection.OperationLinkBegin", OnComplete = "PxLinkCollection.OperationLinkComplete", OnSuccess = onSuccessAddLink, UpdateTargetId = "linkList" }, new { id = "addLinkForm" })) { %>  
            <input type="hidden" id="id" name="id" value="<%= Model.Id %>" />
            <%= Html.HiddenFor(m => m.Title) %>
            
            <span class="field buttons">
                <ol>
                    <li><span id="spnLinkError" class="px-default-text" style="display: none;">Please enter a valid link</span> 
                    </li>
                    <li>
                        <label for="linkTitle">Link title</label><br />
                        <input id="linkTitle" name="linkTitle" type="text" onclick="$('#spnLinkError').hide('slow');" />
                               
                    </li>
                    <li>
                        <label for="lblUrl">URL</label><br />
                        <input id="linkUrl" name="linkUrl" type="text" onclick="$('#spnLinkError').hide('slow');" />
                    </li>
                            
                    <li class="savecancelbtns">
                        <label>&nbsp;</label>
                        <input type="button" name="behavior" value="Save" onclick="return PxLinkCollection.AddLink()" />
                        <input type="button" name="cancel" value="Cancel" onclick="return PxLinkCollection.CloseAddResourse()" />
                        <input type="submit" id="formsubmit" name="behavior" value="Save" style="visibility:hidden;"/>
                    </li>
                </ol>
            </span><span class="clear"></span>
        <% } %>
    </div>
</div>


