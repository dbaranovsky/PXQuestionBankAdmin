<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>
<%
    bool isCourseSandbox = (ViewData["IsSandboxCourse"] != null) && Convert.ToBoolean(ViewData["IsSandboxCourse"]);
    var parentId = ViewData["ParentId"] == null ? "" : ViewData["ParentId"].ToString();
    var isCourseSandboxClass = "";
    if (isCourseSandbox)
    {
        isCourseSandboxClass = "sandbox";
    }
   
    var updateTargetId = "content";
    var isAssigned = (Model.DueDate.Year != DateTime.MinValue.Year);
    var assignmentSettingClass = "assigntab";
    if (Model.IsContentCreateAssign)
    {
        assignmentSettingClass = "contentcreate";
    }

    Model.IsContentCreateAssign = false;
    var buttonText = "Assign";
    if (isAssigned)
    {
        buttonText = Model.SourceType != "PxUnit" ? "Change due date/points possible" : "Edit due date / points possible";
    }

    var editUrl = Url.GetComponentHash("item", Model.Id,
                                            new
                                            {
                                                mode = ContentViewMode.Edit,
                                                hasParentLesson = parentId,
                                                includeNavigation = true,
                                                isBeingEdited = true,
                                                renderFne = true
                                            });
%>
<%             
    var checkedDateTime = isAssigned ? "checked" : "";
    var showDateTime = isAssigned ? "display: inline" : "display: none";
    var ddlabel = (ViewData["isRange"] != null && Boolean.Parse(ViewData["isRange"].ToString())) ? "—" : Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy");
    var ddlTime = (ViewData["isRange"] != null && Boolean.Parse(ViewData["isRange"].ToString())) ? "—" : (ddlabel == "") ? "" : Model.DueDate.ToShortTimeString();
    
%>
<div class="FacePlateAsssign contentwrapper">
    <input class="management-card-tracker" type="hidden" value="" />
    <%
        var vwShowAssignmentUnitFlow = ViewData["ShowAssignmentUnitWorkflow"];
        var showAssignmentUnitFlow = false;
        if (vwShowAssignmentUnitFlow != null)
        {
            bool.TryParse(vwShowAssignmentUnitFlow.ToString(), out showAssignmentUnitFlow);
        }

        if (showAssignmentUnitFlow)
        { 
    %>

    <div class="faceplate-assign-dropdown-container" style="display:none">
        <label for="addassignmentsselection">Add this to a new or existing assignment:</label>
        <% Html.RenderPartial("AssignmentUnitSelector", Model); %>
        
        <%
           var assignmentUnitToc = ViewData["AssignmentUnitToc"];
           var assignmentUnitTemplateId = ViewData["AssignmentUnitTemplateId"];
           if (assignmentUnitTemplateId != null)
           {
        %>
                <input type="hidden" class="assignmentUnitTemplateId" value="<%= assignmentUnitTemplateId %>" />
                <input type="hidden" class="assignmentUnitToc" value="<%= assignmentUnitToc %>" />
        <%
           }
        %>
        <input type="button" class="assignment-selection-btn button primary large" value="Add New Assignment" />
    </div>
    
    <%  } %>
    <div class="faceplate-manage-header">
        <h2 class="assignsettingstitle">
            <%= HttpUtility.HtmlDecode(Model.Title) %>
            <span id="managementcard-close" class="managementcard-close"></span>
        </h2>
        <% var assignButtonText = "Assign"; %>
    </div>
    <input type="button" class="collapse-menu assign-showCalendar-open <%= isCourseSandboxClass%>" value="<%=buttonText%>"
        onclick="$.fn.ContentTreeWidget('showAssignCalendar', 'open', '<%= Model.Id %>')" />
    <div class="faceplate-manage-body <%= isCourseSandboxClass%>">
        <div class="assign-arrow">
        </div>
        <div id="assignment-settings" class="faceplate-unit-calendar <%= assignmentSettingClass %>">
            <% 
                var selectedFilter = Model.Syllabus.ChildrenFilterSections.Find(i => i.Id == Model.SyllabusFilter);
                var selectedFilterId = (selectedFilter == null) ? Model.SyllabusFilter : selectedFilter.Id.ToString();
                var selectedRubricId = Model.rubricId;
                var rubricDisabledText = (String.IsNullOrEmpty(Model.RubricPath)) ? "disabled" : "";
                var rubricCheckedText = (String.IsNullOrEmpty(Model.RubricPath)) ? "" : "checked";
                var rubricViewStyle = (String.IsNullOrEmpty(Model.RubricPath)) ? "display:none;" : "";                 
            %>
            <div id="cal-box">
                <span class="instructions">Due:
                    <% 
                        if (Model.SourceType == "PxUnit")
                        {
                    %>
                    <a href="<%= ConfigurationManager.AppSettings.Get("assignUnitHelpUrl") %>" class="assignUnitHelp pxicon pxicon-questionmark" target="_blank"></a>
                    <%
                        }
                    %> 
                </span>
                <div id="assignment-calendar" class="range px-calendar">
                </div>
            </div>
            <div id="form">
                <%if (ViewData["isError"] != null)
                  {%>
                <input type="hidden" id="isError" value="true" />
                <%} %>
                <%= Html.HiddenFor(m => m.Id) %>
                <%= Html.HiddenFor(m => m.Title) %>
                <input type="hidden" id="multipartlesson" name="multipartlesson" value="<%=(updateTargetId == "content-item") %>" />
                <input type="hidden" id="lessonid" name="lessonid" value="<%= Model.lessonId %>" />
                <input type="hidden" id="SyllabusFilter" name="SyllabusFilter" value="<%= selectedFilterId %>" />
                <input type="hidden" id="rubricId" name="rubricId" value="<%= selectedRubricId %>" />
                <%= Html.HiddenFor(m => m.Category)%>
                <%= Html.HiddenFor(m => m.CompletionTrigger)%>
                <%= Html.HiddenFor(m => m.SourceType)%>
                <input type="hidden" id="requestType" name="requestType" value="<%=Model.RequestType%>" />
                <ol>
                    <li>
                        <input type="text" id="facePlateAssignDueDate" class="facePlateAssignDueDate" name="DueDate"
                            value="<%= (ddlabel == "") ? "" : ddlabel %>" placeholder='<%= "e.g. " + DateTime.Now.ToShortDateString() %>' />
                        <input type="hidden" id="facePlateAssignDueDateHidden" class="facePlateAssignDueDateHidden"
                            value="<%= (ddlabel == "") ? "" : ddlabel %>" />
                        <input type="text" id="facePlateAssignTime" class="facePlateAssignTime" name="DueDate"
                            value="<%=ddlTime%>" style="width: 75px;" />
                        <input type="hidden" id="facePlateAssignTimeHidden" class="facePlateAssignTimeHidden"
                            value="<%=ddlTime%>" />
                        <input type="hidden" id="OriginalDueDate" value="<%=ddlabel%> <%=ddlTime%>" />
                        <span class="clearDateField"><a href="#">Clear</a></span>
                    </li>
                    <li class="invaliddate"></li>
                    <li>
                        <div style="display: none; font-size: small;" class="whatwillthisdo">
                            <br />
                            If you set the due date of this unit, all auto-gradeable items within the unit will
                            inherit this due date and be placed into the gradebook with default point settings.
                            You can adjust due dates and gradebook points on an item-by-item basis if necessary.
                            <br />
                            <br />
                        </div></li>
                    <li id="liDueDate" style="display: none;">
                        <input type="checkbox" class="chkDueDate" <%= checkedDateTime %> style="display: none;" />
                        <label for="Assignment_DueDate">
                            Date Due:</label>
                        <input type="text" id="DueDate" name="DueDate" class="DueDate" value="<%= (ddlabel == "" ) ?"1/1/0001" : ddlabel %>" />
                    </li>
                </ol>

                <a href="#" id="help-link"></a>

                <fieldset id="fsDateTime" style="<%= showDateTime %>">
                    <ol>
                        <li id="liDateTime" style="display: none;">
                            <select name="dueHour" id="dueHour" onchange="ContentWidget.IsFormChanged()">
                                <%if (Model.DueDate.Year < 1900)
                                  {
                                      for (var i = 1; i <= 12; i++)
                                      {
                                          var v = i;
                                          if (i == 11)
                                          {%>
                                <option selected='selected' value="<%= v %>">
                                    <%= v%></option>
                                <%
                                          }
                                          else
                                          { %>
                                <option value="<%= v %>">
                                    <%= v%></option>
                                <%}
                                      }
                                  }
                                  else
                                  {
                                      for (var i = 0; i <= 12; i++)
                                      {
                                          var v = i == 0 ? 12 : i; %>
                                <option <%=(i == (Model.DueDate.Hour%12)) ? "selected='selected'" : ""%> value="<%=v%>">
                                    <%=v%></option>
                                <% }
                                  } %>
                            </select>
                            <span class="input-label">:</span>
                            <select name="dueMinute" id="dueMinute" onchange="ContentWidget.IsFormChanged()">
                                <% for (int i = 0; i <= 59; i++)
                                   {
                                       var v = String.Format("{0:00}", i);
                                       if (Model.DueDate.Year < 1900 && i == 59)
                                       {%>
                                <option selected='selected' value="<%= v %>">
                                    <%= v%></option>
                                <%}
                                       else
                                       {%>
                                <option <%= (i == (Model.DueDate.Minute)) ? "selected='selected'" : "" %> value="<%= v %>">
                                    <%= v%></option>
                                <%}
                                   }
                                %>
                            </select>
                            <select name="dueAmpm" id="dueAmpm" onchange="ContentWidget.IsFormChanged()">
                                <%if (Model.DueDate.Year < 1900)
                                  {%>
                                <option value="am">am</option>
                                <option selected='selected' value="pm">pm</option>
                                <%}
                                  else
                                  { %>
                                <option <%= (Model.DueDate.Hour>=12) ? "selected='selected'" : "" %> value="pm">pm</option>
                                <option <%= (Model.DueDate.Hour<12) ? "selected='selected'" : "" %> value="am">am</option>
                                <%} %>
                            </select>
                        </li>
                        <li><%= Html.ValidationMessage("DueDate")%></li>
                    </ol>
                </fieldset>
                <fieldset id="fsGradebook">
                    <ol>
                        <% 
                            var gradableChecked = "";
                            var gradableVisible = "";
                            if (Model.SourceType != "PxUnit")
                            {
                                gradableChecked = "checked";
                                gradableVisible = "display:block";
                            }
                            else
                            {
                                gradableVisible = "display:none";
                            } %>
                        <% if (Model.AssignTabSettings.ShowMakeGradeable && Model.SourceType.ToLowerInvariant() != "pxunit")
                           { %>
                        <li id="liAssignmentGradable" style="display: none;">
                            <input type="checkbox" id="chkAssignmentGradeable" class="chkAssignmentGradeable"
                                <%= gradableChecked %> />
                            <label for="Assignment_Gradeable">
                                Make assignment gradeable</label>
                        </li>
                        <% } %>
                        <% if (Model.SourceType.ToLowerInvariant() != "not")
                           {
                               if (Model.AssignTabSettings.ShowGradebookCategory)
                               {  %>
                        <li id="divGradebookCategory1" style="<%= gradableVisible %>">
                            <% if (Model.SourceType != "PxUnit")
                               {%>
                            <div class="points">
                                <label for="txtGradePoints">Points:</label>

                                <input type="text" id="txtGradePoints" class="txtGradePoints txtGradePointsMGC"
                                    value="<%= Model.Score.Possible %>" onkeypress="return ContentWidget.AllowNumbersOnly(event);"
                                    onkeyup="return PxManagementCard.ShowValidationMessage(event);" onblur="ContentWidget.IsFormChanged()" />
                                <input type="hidden" id="txtGradePointsHidden" class="txtGradePointsHidden" value="<%=Model.Score.Possible%>" />
                                <div style="font-size: 0.7em; display: none; padding-top: 7px; color: Red;" class="points-error-message">Invalid characters</div>
                            </div>
                            <%} %>
                            <div class="category">
                                <label for="selgradebookweights">
                                    Gradebook Category:</label>

                                <%Html.RenderPartial("GradeBookWeightsOptions", Model); %>
                            </div>
                        </li>
                        <%
                               }
                           } %>
                        <% 
                            if (Model.AssignTabSettings.ShowIncludeScore && 1 == 2)
                            {%>
                        <li id="divIncludeGbbScore" style="<%= gradableVisible %>">
                            <label id="Assignment_IncludeGbbScore">
                                Include score in gradebook:
                            </label>
                            <select id="selIncludeGbbScoreTrigger" name="selIncludeGbbScoreTrigger">
                                <option value="1">After item is complete or due date has passed</option>
                                <option value="2">Only after due date has passed</option>
                                <option value="0">Always</option>
                            </select>
                        </li>
                        <% } %>
                        <%
                            if (Model.AssignTabSettings.ShowCalculationType)
                            {%>
                        <li id="divCalculationType" style="display: none;">
                            <label for="Assignment_CalculationType">
                                Calculation Type:</label>
                            <select id="selCalculationTypeTrigger" name="selCalculationTypeTrigger" onchange="ContentWidget.IsFormChanged()"
                                style="max-width: 135px;">
                                <%
                                foreach (var option in Model.AvailableSubmissionGradeAction)
                                {
                                %><option value="<%=option.Key %>" <%= (option.Key == Model.SubmissionGradeAction.ToString())? "selected" : "" %>>
                                    <%=option.Value.ToString().Replace ( "_", " ") %></option>
                                <%}
                                %>
                            </select>
                        </li>
                        <%}%>
                        <%= Html.HiddenFor(m=>m.SourceType)%>
                        <% if ((Model.SourceType != "PxUnit") && !Model.AssignTabSettings.ShowMakeGradeable)
                           { %>
                        <li id="assignmentpoints">
                            <label for="Assignment_Score_Possible">
                                Points possible:</label>
                            <%= Html.TextBoxFor(m => m.Score.Possible, new { onkeypress = "return ContentWidget.AllowNumbersOnly(event);", onblur = "ContentWidget.IsFormChanged()" })%>
                            <%= Html.ValidationMessage("Points")%>
                        </li>
                        <%} %>
                        <li id="startDateField" style="display: none;">
                            <label for="Assignment_StartDate">
                                Start Date:</label>
                            <% var ddlabelStart = Model.StartDate.Year == DateTime.MinValue.Year ? "N/A" : Model.StartDate.ToString("dddd MMM d, yyyy"); %>
                            <input type="text" class="readonly" readonly="readonly" id="StartDate" name="StartDate"
                                value="<%= ddlabelStart %>" />
                        </li>
                    </ol>
                </fieldset>
                <%--xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx--%>
                <div class="managagementcard-section">
                    <fieldset id="ePortfolioFields">
                        <ol>
                            <%                                           
                                var btnAssignDisplay = "none";
                                var btnSaveChangesDisplay = "none";
                                var btnUnassignDisplay = "none";
                                if (isAssigned)
                                {
                                    btnSaveChangesDisplay = "inline";
                                    btnUnassignDisplay = "inline";
                                }
                                else
                                {
                                    btnAssignDisplay = "inline";
                                } 
                            %>
                            <%                      
                                if (!Model.IsContentCreateAssign)
                                {%>
                            <li style="display: none;">
                                <br />
                                <input type="button" id="btnAssign" value="Assign" class="assign btnAssign" style="display: <%= btnAssignDisplay %>" />
                                <input type="button" id="btnSaveChanges" class="btnSaveChanges" value="Save Changes"
                                    style="background-color: #717D7D; display: <%= btnSaveChangesDisplay %>" class="assign"
                                    disabled="disabled" />
                                &nbsp;
                                <input type="button" id="btnUnassign" class="btnUnassign assign" value="Unassign"
                                    style="display: <%= btnUnassignDisplay %>" />
                            </li>
                            <li>
                                <input type="button" class="collapse-menu assign-showCalendar-close button primary large" value="Done" />
                            </li>
                            <li>
                                <%if (isAssigned)
                                  { %>
                                <a class="management-card-unassign" href="#" onclick="return PxManagementCard.unAssign('<%= Model.Id %>');">Unassign</a>
                                <%} %>
                            </li>
                            <% }
                                else
                                { %>
                            <input type="hidden" value="true" id="hdnIsContentCreateAssign" />
                            <% } %>
                        </ol>
                        <input type="hidden" value="<%= isAssigned %>" id="hdnIsAssigned" />
                        <input type="hidden" value="<%= Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy") %>"
                            id="hdnDueDate" />
                        <input type="hidden" value="<%= Model.DueDate.Hour%12 %>" id="hdnDueHour" />
                        <input type="hidden" value="<%= Model.DueDate.Minute %>" id="hdnDueMinute" />
                        <input type="hidden" value="<%= (Model.DueDate.Hour>=12) ? "pm" : "am" %>" id="hdnDueAmpm" />
                        <input type="hidden" value="<%= Model.CompletionTrigger %>" id="hdnCompletionTrigger" />
                        <input type="hidden" value="<%= Model.Category %>" id="hdnGradebookCategory" />
                        <input type="hidden" value="<%= selectedFilterId %>" id="hdnSyllabusCategory" />
                        <input type="hidden" value="<%= Model.IncludeGbbScoreTrigger %>" id="hdnIncludeGbbScoreTrigger" />
                        <input type="hidden" value="<%= Model.GradeRule%>" id="hdnCalculationTypeTrigger" />
                        <input type="hidden" value="<%= Model.IsGradeable %>" id="hdnGradable" />
                        <input type="hidden" value="<%= Model.IsMarkAsCompleteChecked %>" id="hdnIsMarkAsComplete" />
                        <input type="hidden" value="<%= Model.Score.Possible %>" id="hdnGradePoints" />
                        <input type="hidden" value="<%= Model.IsAllowLateSubmission %>" id="hdnLateSubmission" />
                        <input type="hidden" value="<%= Model.IsSendReminder %>" id="hdnSendReminder" />
                        <input type="hidden" value="false" id="hdnAssignTabContentCreate" />
                    </fieldset>
                </div>
            </div>
            <div style="clear: both;">
            </div>
            <input type="hidden" value="<%= Model.CourseType %>" id="courseType" />
        </div>
        <div class="faceplate-student-completion">
            <div class="faceplate-student-completion-stats"></div>
            <%
                if (Model.SourceType != "PxUnit")
                {
            %>

            <%= Url.GetComponentLink("View results", "item", Model.Id, new
                            {
                                mode = ContentViewMode.Results,
                                hasParentLesson = parentId,
                                includeNavigation = true,
                                isBeingEdited = true,
                                renderFne = true
                            },
                            new
                            {
                                @onclick = "$.fn.ContentTreeWidget('hideManagementCard')"
                            })%>

            <%
                }
            %>
        </div>
        <div class="faceplate-student-hide">
            <% bool hiddenFromStudents = (ViewData["HiddenFromStudents"] != null) && (bool)ViewData["HiddenFromStudents"];%>
            <a href="#" class="managementcard_students_show"></a>
            <a href="#" class="managementcard_students_hide"></a>
            <span id="card_item_visible-label" class="ui-slider" for="card_item_visible">Visible to students</span>
        </div>
        <div class="faceplate-sub-menu">
            <% if (Model.SourceType == "PxUnit")
               { %>
            <a href="#" class="edit link button" onclick="PxManagementCard.showEditContentTitleDialog('<%= Model.Id %>', '', '', '', 'rename'); return false;">Edit</a> <%}
               else
               { %>
            <a href="<%= editUrl %>" id="fne-edit" class="edit link button" onclick="$.fn.ContentTreeWidget('hideManagementCard')">
                <span>Edit</span></a>

            <% } %>
            <a href="#" class="moveorcopy link button" onclick="PxManagementCard.showMoveCopyDialog('<%= Model.Id %>'); return false;">Move <span>or Copy</span></a>
            <span class="moveorcopy"></span>
            <%
               var clsRemovable = Model.IsRemovable ? "" : "hide";
               var tocs = (ViewData["Removable_Tocs"] == null) ? "" : ViewData["Removable_Tocs"].ToString();
            %>
            <a href="#" class="remove link button <%= clsRemovable %>" onclick="PxManagementCard.checkForSubmissionAndRemoveItem('<%= Model.Id %>', '<%= tocs %>'); return false;">Remove</a>
        </div>
    </div>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.ptTimeSelect.js") %>'], function () {
                PxManagementCard.Init('<%= Model.Id %>', '<%= assignmentSettingClass %>', '<%=hiddenFromStudents%>');
            });
        });

    } (jQuery));
</script>