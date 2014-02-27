<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<%	var assignmentSettingClass = "contentsettingstab";
 %>
 
<input type="hidden" id="isShowOnSuccessMessage"  value="true" />
<% using (Ajax.BeginForm("SaveContentSettings", "SettingsView", new AjaxOptions() { OnSuccess = "ContentWidget.ShowSaveMessage();" }))
   {
       var vd = new ViewDataDictionary();
       var config = new ToggleSectionConfiguration() { };       
       var hasCustomSettings = (bool)ViewData["hasCustomSettings"];
    %>


 <h1 class="settingsheader"><%=Model.Content.Title%></h1>
<div id="assignment-settings" class="<%= assignmentSettingClass %>">

<ul class="outline contentsetttingsUL">

    <% if (hasCustomSettings)
       { %>
    <li class="sectionparent" default-state="open">
        <div class="sectiontitle"><%=Model.Content.Type.ToString() + " Settings" %></div>
        <div class="sectionbody">
        <% 
           if (Model.IsTemplateEditor == false)
           {
               if (Model.GroupId == null)
               {
                   Model.GroupId = "";
               } 
               %>
            <div class="settings-container">
                <form id="Form1" runat="server">
                    <%= Html.Hidden("settingsList", Url.Action("GetSettingsTabList", "Groups"))%>
                    <% Html.RenderPartial("~/Views/Shared/GroupSelector.ascx", new ViewDataDictionary() { { "GroupSelectorLabel", "Settings for" }, { "OnChangeEvent", "PxPage.LargeFNE.OnChangeSettingsList(this)" } }); %>
                    <div id="settingswrapper">
                        <% if ((bool)ViewData["hasCustomSettings"])
                           { %>
                        <span id="assignment-settings-templates" style="float:right">
                            <label>Template:</label>
                            <select id="ddlTemplateList" onchange ="PxSettingsTab.OnChangeTemplateList(this);"></select>    
                        </span>
                        <%} %>
                    </div>
                </form>
                <input id = "txtEnroll" type="hidden" value="" />
                <input type="hidden" id="SettingsEntityId" value="<%=Model.GroupId.ToLowerInvariant().Replace ( "groupidvalue", "")  %>" />  
            </div>
        <% } %>        
      </div>
    </li>


    
    <li class="sectionparent" default-state="open">
        <div class="sectiontitle">Settings</div>
        <div class="sectionbody" style="display:none;">
            <%var customSettings = ViewData["CustomSettings"].ToString().Split('|');%>
                    
            <%Html.RenderAction(customSettings[1], customSettings[0], new { contentItem = Model.Content }); %>
        </div>

    </li>

    <%} %>
    <li class="sectionparent" default-state="hide">
        <div class="sectionbody" style="display:none;">
    <% if (Model.IsTemplateEditor == false )
       {
    %>

    <div id="divAddIndividual" class="divPopupWin">   
        <h2 id="divAddWinTitle" class="divPopupTitle">
           SEARCH CLASS ROSTER
        </h2>
        <div id="divAddContent" class="divPopupContent">      
        <label id="errorMessage" style="color:Red">&nbsp;</label><br /> 
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
</div>

</li>

<%var state = hasCustomSettings ? "open" : "open";   %>
<li class="sectionparent visibility" default-state="<%=state%>">
       <div class="sectiontitle">Visibility</div>
        <div class="sectionbody" style="display:none;">
        <% Html.RenderPartial("Visibility", Model); %>
        </div>
</li>


<li>
    <input type="button" value="Save" id="savecontentsettings" class="savebtn submit-action"  />
</li>

</ul>

</div>
<%} %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.ptTimeSelect.js") %>',
                            '<%= Url.ContentCache("~/Scripts/SettingsTab/ContentSettings.js") %>'], function () {

                                var assignmentSettingsClass = '<%= assignmentSettingClass %>';
                                PxPage.log("assignmentSettingClass = '<%= assignmentSettingClass %>'");
                                ContentWidget.InitAssign(assignmentSettingsClass);

                                $('.outline').ContentSettings({ readOnly: true });
                                if (PxPage.FrameAPI) {
                                    PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
                                }
                            });

        });
        
    } (jQuery));    
</script>
