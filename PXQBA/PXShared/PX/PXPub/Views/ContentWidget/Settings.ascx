<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<% 
    if (Model.IsTemplateEditor == false)
   { %>
    <div class="settings-container">
        <form id="Form1" runat="server">
            <div id="settingswrapper">
                <span style="float:right">
                    <label>Template:</label>
                    <select id="ddlTemplateList" onchange ="PxSettingsTab.OnChangeTemplateList(this);"></select>    
                </span>
            </div>
        </form>
        <input id = "txtEnroll" type="hidden" value="" />
    </div>
<% } %>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            if (PxPage.FrameAPI) {
                PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
            }
        });
    } (jQuery));   
</script>

<% 
    if (Model.Content.SubType == "quiz" || Model.Content.SubType == "homework" || Model.Content.SubType == "htmlquiz" || Model.Content.SubType == "epage")
    { %>
    <div id="assessment-settings-container">
        <% 
            Html.RenderAction("Index", "AssessmentSettings", new { itemId = Model.Content.Id }); %>
    </div>
<% } else
   {
       if (Model.Content.Type.ToLower() == "linkcollection" ||
           Model.Content.Type.ToLower() == "htmldocument" ||
           Model.Content.Type.ToLower() == "eBook" ||
           Model.Content.Type.ToLower() == "documentcollection" ||
           Model.Content.Type.ToLower() == "link" ||
           Model.Content.Type.ToLower() == "externalcontent"
           ) {
           Html.RenderAction("Index", "SettingsView", Model );
       } else
       {
           Html.RenderPartial("BhIFrameComponent", new BhComponent()
                                                       {
                                                           ComponentName = "ItemEditor",
                                                           DomainUserSpace = Model.DomainUserSpace,
                                                           Parameters = new
                                                                            {
                                                                                EnrollmentId =
                                                               Model.Content.EnrollmentId,
                                                                                Id = "itemeditor",
                                                                                ItemId = Model.Content.Id,
                                                                                GroupId = "",
                                                                                ShowBeforeUnloadPrompts = false,
                                                                                ShowOnlyProperties = true
                                                                            }
                                                       });
       }
   }
%>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            if (PxPage.FrameAPI) {
                PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
            }
        });
    } (jQuery));   
</script>

<% if (Model.IsTemplateEditor == false && Model.Content.Type == "quiz" || Model.Content.Type == "homework")
   {
    Html.RenderAction("Index", "LearningObjective", new { itemId = Model.Content.Id }); %>

    <div id="divAddIndividual" class="divPopupWin">   
        <h2 id="divAddWinTitle" class="divPopupTitle">
           SEARCH CLASS ROSTER
        </h2>
        <div id="divAddContent" class="divPopupContent">       
            <span class="field buttons">
                <input id="studentName" name="Name" type="text" onclick="$('#spnNameError').hide('slow');" />
                <span id="spnNameError" class="field-validation-error px-default-text" >Please enter a name</span>
                <input type="hidden" id="studentId" />         
                <input type="button" value="Cancel" id="btnAddCancel" />
                <input type="button" value="Add" id="btnAdd" onclick="PxSettingsTab.OnAdd()"/>
            </span>       
        </div>
    </div>
<% } %>