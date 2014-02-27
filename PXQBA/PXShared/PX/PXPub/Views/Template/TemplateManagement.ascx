<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.Template>>" %>
<div id="fne-window-container" class="template-management">
    <div class="template-gearbox">
        <b>Templates</b><div class="gear-box">
        </div>
        <input type="hidden" id="templateParentId" />
    </div>
    <div class="template-management-header"><span><b><label class="TemplateTitle"></label>&nbsp; Settings</b></span>&nbsp;&nbsp;&nbsp;<a href="#" style="visibility:hidden">View Description</a>
</div>
    <div class="template-management-list">
        <ul class="template-list">
            <% 
                foreach (var item in Model)
               { %>
            <li id="<%= item.Id %>" itemid="<%= item.Id %>" title="<%= item.Title %>" templateparentid="<%=item.TemplateParentId%>">
                <%= item.Title %>
            </li>
            <% } %>
        </ul>
    </div>
    <div class="template-management-details">
    </div>
</div>
<div id="divEditTemplate" class="divPopupWin">
</div>
<div id="divSaveTemplateAs" class="divPopupWin">
    <h2 class="divPopupTitle">
        SAVE AS
    </h2>
    <input type="hidden" id="newItemId" />
    <div id="divSaveTemplateAsContent" class="divPopupContent">
        <label>
            Template Name :</label><br />
        <input id="txtTemplateName" class="txtTemplateName" name="Name" type="text"/>
        <span id="spnNameError" class="field-validation-error px-default-text" >Please enter a template name.</span>
        <br />
        <label>
            Description :</label><br />
        <div class="description-text-area">
        </div>
        <br />
        <span id="template-save-btns">
            <button id="btnSaveAsTemplate" onclick="PxSettingsTab.SaveAsTemplate();">
                Save</button>
            <button id="btnSaveEdit" onclick="PxSettingsTab.SaveClick();">Save Edit</button>
            <button id="btnCancelSaveAsTemplate" onclick="PxSettingsTab.SaveAsTemplateCancel();">
                Cancel</button>
        </span>
    </div>
</div>
<div id="delete-confirm" style="display: none;">
    <div class="dialog-title" id="confirmText">
        Are you sure you wish to delete this template?</div>
</div>
<div id="divSaveTemplate" class="divPopupWin">
    <h2 class="divPopupTitle">
        SAVE
    </h2>
    <input type="hidden" id="Hidden1" />
    <div id="div2" class="divPopupContent">
        <span id="Span1">Saving this template will replace the saved version of the template.
        This will affect the settings of each activity using this template that has not yet been completed by a student.</span>
        <br />
        <br />
        <div class="description-text-area"><textarea id="ta1" rows="15" cols="50">Loading...</textarea>
        </div>
        <br />
        Are you sure you want to save?
        <span>
            <button id="Button1" onclick="PxSettingsTab.SaveTemplateCommit();">
                Save</button>            
            <button id="Button3" onclick="PxSettingsTab.SaveTemplateCommitUpdate();">
                Save and Update Content</button>
            <button id="Button2" onclick="PxSettingsTab.SaveAsTemplateCancel();">
                Cancel</button>
        </span>
    </div>
</div>