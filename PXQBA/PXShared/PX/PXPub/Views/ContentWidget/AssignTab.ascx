<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<% 
    var assignmentSettingClass = Model.AssignedItem.IsContentCreateAssign ? "contentcreate" : "assigntab";
    string modelType = null;
    if (null != Model.AssignedItem)
    {
        modelType = Model.AssignedItem.SubType;
    }
    
    %>
<% using (Ajax.BeginForm("SaveContentSettings", "SettingsView", new AjaxOptions() { }))
   {%>

<%if (!Model.AssignedItem.IsContentCreateAssign) {%>
                        
 <h1 class="settingsheader"><%=Model.Content.Title%></h1>
 <%} %>
<div id="assignment-settings" class="<%= assignmentSettingClass %>">

 <input type="hidden" id="AssignmentTabItemId" value="<%=Model.Content.Id%>" />
 <div id="form">
<ul class="outline assignmentSettingsUL">

    <li class="sectionparent1" default-state="open">   
    <div class="sectionbody">
      <% 
           if (Model.IsTemplateEditor == false)
           {
               if (Model.GroupId == null) 
               { Model.GroupId = ""; 
               } %>
            <div class="settings-container sectiontitle">
                
                <%= Html.Hidden("settingsList", Url.Action("GetSettingsTabList", "Groups"))%> 

                <% Html.RenderPartial("~/Views/Shared/GroupSelector.ascx", new ViewDataDictionary()
                   {
                       { "GroupSelectorLabel", "Settings for" }, 
                       { "OnChangeEvent", "PxPage.LargeFNE.OnChangeSettingsList(this)" },
                       { "disabled", !Model.Content.IsAssigned}
                   }); %>

                <input id = "txtEnroll" type="hidden" value="" />
                <input type="hidden" id="SettingsEntityId" value="<%=Model.GroupId.ToLowerInvariant().Replace ( "groupidvalue", "")  %>" />                
            </div>
        <% } %>
       </div>  
    
    </li>
    <li class="sectionparent" default-state="open">
        <div class="sectiontitle">Due date and time</div>          
        <div class="sectionbody">            
            <% Html.RenderPartial("AssignmentCalendar", Model.AssignedItem); %>
      </div>
    </li>

    <li class="sectionparent" default-state="open">
        <div class="sectiontitle">Gradebook Settings</div>      
        <div class="sectionbody">
            <% Html.RenderPartial("Gradebook", Model.AssignedItem, ViewData); %>
        </div>    
    </li>

   <li class="sectiontitleBorderTop" >

                <fieldset id="ePortfolioFields">
                    <ol>
                       <% Html.RenderPartial("AssignTabButtons", Model, ViewData); %>

                        <% if (Model.RelatedTemplates.Count > 0)
                           { %>
                        <li class="sectionparent" default-state="open">
                                <div class="sectiontitle">Related Items</div>      
                                <div class="sectionbody">                                
                                <% Html.RenderPartial("DisplayRelatedTemplates", Model.RelatedTemplates); %>
                                </div>
                        </li>
                      <%} %>
                        <%if (Model.Content.IsContentCreateAssign)
                          { %>
                        <li>
                            <input type="hidden" value="true" id="hdnIsContentCreateAssign" />
                        </li>
                        <% } %>
                    </ol>
                </fieldset>             
    </li>

</ul>
    </div>
    <div style="clear:both"> </div>
    <div class="faceplate-student-completion-stats" style="display: none"></div>
 </div>
<%} %>


 <% if (Model.IsTemplateEditor == false)
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

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.ptTimeSelect.js") %>',
                            '<%= Url.ContentCache("~/Scripts/SettingsTab/AssignmentSettings.js") %>'], function () {

                var assignmentSettingsClass = '<%= assignmentSettingClass %>';
                PxPage.log("assignmentSettingClass = '<%= assignmentSettingClass %>'");

                //When control is being used in the assignment tab, initialize here. If called from reflection dialog, we initialize once
                //the dialog is open or else it won't work.
                if (assignmentSettingsClass === 'assigntab') {
                    ContentWidget.InitAssign(assignmentSettingsClass);

                    AssignmentSettings.init(assignmentSettingsClass);

                    if (PxPage.FrameAPI) {
                        PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
                    }
                }
            });
        });

    } (jQuery));    
</script>
