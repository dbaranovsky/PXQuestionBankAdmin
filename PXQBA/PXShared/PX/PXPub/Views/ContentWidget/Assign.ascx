<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>
<%
	var updateTargetId = "content";

	var callback = "null";

	var isAssigned = (Model.DueDate.Year != DateTime.MinValue.Year);

	var assignmentSettingClass = "assigntab";
	if (Model.IsContentCreateAssign)
	{
		assignmentSettingClass = "contentcreate";
	}
			
%>

<script type="text/javascript">
	(function ($) {
		PxPage.OnReady(function () {
			PxPage.Require([], function() {
				var assignmentSettingsClass = '<%= assignmentSettingClass %>'
				PxPage.log("assignmentSettingClass = <%= assignmentSettingClass %>");
				ContentWidget.InitAssign(assignmentSettingsClass, false);
                var itemId = '<%= Model.Id %>',
					callback = <%= callback %>;

				//assign button
				$('.btnAssign').click(function () {
					var contentWrapper = $(this).closest('.contentwrapper');
					ContentWidget.ContentAssigned('assign', itemId, callback, contentWrapper);
				});

				 //unassign button
				$('.btnUnassign').click(function () {
					var contentWrapper = $(this).closest('.contentwrapper');
					ContentWidget.ContentAssigned('unassign', null, null, contentWrapper);
				});

				//save changes button
				$('.btnSaveChanges').click(function () {
					var contentWrapper = $(this).closest('.contentwrapper');
					ContentWidget.ContentAssigned('assign', itemId, callback, contentWrapper);
				});

			});
		});
	} (jQuery));
</script>

<h2 class="assignsettingstitle">
	
</h2>
<div id="assignment-settings" class="<%= assignmentSettingClass %>">
	<% 
	   var selectedFilter = Model.Syllabus.ChildrenFilterSections.Find(i => i.Id == Model.SyllabusFilter);
	   var selectedFilterId = (selectedFilter == null) ? Model.SyllabusFilter : selectedFilter.Id.ToString();             
	%>

	<div id="cal-box">
		
		<div id="assignment-calendar" class="range px-calendar">
		</div>
	</div>

	<% if (Model.AssignTabSettings.ShowAssignedSameDay){ %>
    <div id="other-assignments"></div>
    <div style="clear:both" ></div>
    <% } %>    
    <div id="form">
		<%             
		var checkedDateTime = isAssigned ? "checked" : "";
		var showDateTime = isAssigned ? "display: inline" : "display: none";
		var ddlabel = Model.DueDate.Year < 1900 ? "Not Specified" : Model.DueDate.ToString("MM/dd/yyyy");         
		%>
		
		<ol>
			<li id="liDueDate">
				<input type="checkbox" class="chkDueDate" <%= checkedDateTime %> />
				<label for="Assignment_DueDate">
					Date Due:</label>              
				<input type="text" class="readonly DueDate" readonly="readonly" id="DueDate" name="DueDate" 	value="<%= ddlabel %>"/>
			</li>
		</ol>

		<fieldset id="fsDateTime" style="<%= showDateTime %>" >
			<ol>
                <%--------------------- Region Due Date ------------------------%>
                <li id="liDateTime">
			        <select name="dueHour" id="dueHour" onchange="ContentWidget.IsFormChanged()">
			        <%for (var i = 1; i <= 12; i++) {
				        var v = i;
                        if (Model.DueDate.Year < 1900) {%>
					        <option <% = (i == 11) ? "selected='selected'" : "" %> value="<%= v %>"><%= v %></option>
				        <%}
                        else {%>
					        <option <%=(i == (Model.DueDate.Hour%12)) ? "selected='selected'" : ""%> value="<%= v %>"><%= v %></option>
				        <%}
			        }%>
			        </select>
			        <span class="input-label">:</span>
			        <select name="dueMinute" id="dueMinute" onchange="ContentWidget.IsFormChanged()">
			        <% for (int i = 0; i <= 59; i++) {
				        var v = String.Format("{0:00}", i);
				        if (Model.DueDate.Year < 1900 && i == 59) {%>
					        <option selected='selected' value="<%= v %>"><%= v%></option>
				        <%}
				        else {%>
					        <option <%= (i == (Model.DueDate.Minute)) ? "selected='selected'" : "" %> value="<%= v %>"><%= v %></option>
				        <%}
			        }%>
			        </select>
			        <select name="dueAmpm" id="dueAmpm" onchange="ContentWidget.IsFormChanged()">
				    <%if (Model.DueDate.Year < 1900) {%>
					    <option value="am">am</option>
					    <option selected='selected' value="pm">pm</option>
				    <%}
				    else { %>
					    <option <%= (Model.DueDate.Hour>=12) ? "selected='selected'" : "" %> value="pm">pm</option>
					    <option <%= (Model.DueDate.Hour<12) ? "selected='selected'" : "" %> value="am">am</option>
				    <%} %>
			        </select>
                </li>
		        <li><%= Html.ValidationMessage("DueDate")%></li>

                <%--------------------- Region Allow Late Submission ------------------------%>
				<% if (Model.AssignTabSettings.ShowAllowLateSubmissions) {
				var lateSubmissionChecked = Model.IsAllowLateSubmission ? "checked" : "";
				var lateSubmissionDisplay = Model.IsAllowLateSubmission ? "" : "disabled = 'disabled'";
				var lateSubmissionDisplayStyle = Model.IsAllowLateSubmission ? "" : "display:none;";
				var highlightLateChecked = Model.IsHighlightLateSubmission ? "checked" : "";
				var gracePeriodChecked = Model.IsAllowLateGracePeriod ? "checked" : "";
				var gracePeriodDisplay = Model.IsAllowLateGracePeriod ? "" : "disabled = 'disabled'";
				var gracePeriodDisplayStyle = Model.IsAllowLateGracePeriod ? "" : "display:none;";
				%>
				<li id="liAllowLaterSubmissions">
					<input type="checkbox" id="chkAllowLateSubmissions" <%= lateSubmissionChecked %> onclick="ContentWidget.IsFormChanged()"/>
					<label for="chkAllowLateSubmissions">Allow late submissions until grace period expires</label>
				</li>
				<li>
					<input type="hidden" id="hdnHighlightLateSubmission" value="<%= Model.IsHighlightLateSubmission %>" />
					<input type="hidden" id="hdnAllowLateUntilGracePeriod" value="<%= Model.IsAllowLateGracePeriod %>" />
					<input type="hidden" id="hdnLateGracePeriodDuration" value="<%= Model.LateGraceDuration %>" />
					<input type="hidden" id="hdnLateGracePeriodDurationType" value="<%= Model.LateGraceDurationType %>" />
					<ol style="margin-left:50px;<%= lateSubmissionDisplayStyle%>" id="lateSubmissionDeatils" >
					<li>
						<input type="checkbox" id="chkHighlightLateSubmission" <%= highlightLateChecked %> <%= lateSubmissionDisplay %> onclick="ContentWidget.IsFormChanged()"/>
						<label for="chkHighlightLateSubmission">Highlight late submission in results</label>
					</li>
					<li>
						<input type="checkbox" id="ckhAllowLateUntilGracePeriod" <%= gracePeriodChecked %> <%= lateSubmissionDisplay %> onclick="ContentWidget.IsFormChanged()"/>
						<label for="ckhAllowLateUntilGracePeriod">Allow late until graceperiod</label>
					</li>
					<li style="padding-left:25px;">
						<div id="gracePeriodDetails" style="<%= gracePeriodDisplayStyle%>">
							<label style="width:auto;" for="LateGracePeriodDuration">Grace Period:</label>
							<input type="text"  <%= gracePeriodDisplay %> id="LateGracePeriodDuration" class="reminderDayCount" onkeypress = "return ContentWidget.AllowNumbersOnly(event);" onblur = "ContentWidget.IsFormChanged()"  value="<%= Model.LateGraceDuration %>"/> 
							<select <%= gracePeriodDisplay %>  id="LateGracePeriodDurationType" onchange="ContentWidget.IsFormChanged()">
								<option <%= (Model.LateGraceDurationType=="minute") ? "selected='selected'" : "" %> value="<%=LateGraceDurationType.Minute.ToString() %>">minutes</option>
								<option <%= (Model.LateGraceDurationType=="hour") ? "selected='selected'" : "" %> value="<%=LateGraceDurationType.Hour.ToString() %>">hours</option>
							</select>
						</div>
					</li>
					</ol>
				</li>
				<% } %>
                
                <%--------------------- Region Send Reminder ------------------------%>
                <% if (Model.AssignTabSettings.ShowSendReminder) {
				var sendReminderChecked = Model.IsSendReminder ? "checked" : "";
				var sendReminderDisplay = Model.IsSendReminder ? "" : "disabled = 'disabled'";
				var sendReminderDisplayStyle = Model.IsSendReminder ? "" : "display:none;";
				%>
				<li id="liScheduleEmailReminder">
					<label for="chkScheduleEmailReminder" style="width:300px">
                    <input type="checkbox" class="fsEnabler" id="chkScheduleEmailReminder" <%= sendReminderChecked %> />
					Schedule an email reminder for students</label>
				</li>
				<li>
					<input type="hidden" id="hdnReminderBeforeCount" value="<%= Model.ReminderEmail.DaysBefore %>" />
					<input type="hidden" id="hdnReminderBeforeType" value="<%= Model.ReminderEmail.DurationType %>" />
					<input type="hidden" id="hdnReminderSubject" value="<%= Model.ReminderEmail.Subject %>" />
					<input type="hidden" id="hdnReminderBody" value="true" />
					<ol style="margin-left:50px;<%= sendReminderDisplayStyle%>" id="reminderEmailDeatils">
					<li>
						<label style="width:auto;" for="ReminderBeforeCount">Send advance warning: </label> 
						<input type="text"  <%= sendReminderDisplay %>  id="ReminderBeforeCount" class="reminderDayCount" onkeypress = "return ContentWidget.AllowNumbersOnly(event);" onblur = "ContentWidget.IsFormChanged()"  value="<%= Model.ReminderEmail.DaysBefore %>"/> 
						<select <%= sendReminderDisplay %>  id="ReminderBeforeType" onchange="ContentWidget.IsFormChanged()">
							<option <%= (Model.ReminderEmail.DurationType=="day") ? "selected='selected'" : "" %> value="day">days</option>
							<option <%= (Model.ReminderEmail.DurationType=="week") ? "selected='selected'" : "" %> value="week">weeks</option>
						</select>
						<span id="emailReminderText"> before the due date</span> 
					</li>
					<li>
						<label for="ReminderSubject" style="width:auto;">Subject</label>
						<input <%= sendReminderDisplay %>  type="text" id="ReminderSubject" class="reminderTitle" value="<%= Model.ReminderEmail.Subject %>"   onblur = "ContentWidget.IsFormChanged()"/>
					</li>
					<li><label>Message</label></li>
					<li>
                        <!-- Changed from tinymce textarea to normal text area by design -->
						<%= Html.TextAreaFor(m => m.ReminderEmail.Body)%>  
					</li></ol>
				</li>
				<% } %>
			</ol>
		</fieldset>
        
		<fieldset id="fsGradebook">
			<ol>
			<% 
                Model.IsGradeable = true;
            var gradableChecked = (Model.IsGradeable || Model.Score.Possible > 0) ? "checked" : "";
            var gradableVisible = (Model.IsGradeable || Model.Score.Possible > 0) ? "display:block" : "display:none";
            var disaleGradable = (Model.IsItemLocked) ? "disabled" : "";
            %>   

			<% if (Model.AssignTabSettings.ShowMakeGradeable) { %>
				<li style="display:none;" id="liAssignmentGradable">                                                
					<input type="checkbox" id="chkAssignmentGradeable" class="chkAssignmentGradeable" <%= gradableChecked %>  />
					<label for="Assignment_Gradeable">
						Make assignment gradeable</label>
				</li>

				<li  id="divGradePoints" style="<%= gradableVisible %>">
					<label id="lblValue"> Value: </label>
					<input type="text" id="txtGradePoints" value="<%= Model.Score.Possible %>" onkeypress="return ContentWidget.AllowNumbersOnly(event);" 
						onblur="ContentWidget.IsFormChanged()" <%= disaleGradable %>/>  
					<label id="lblPoints"> points </label>                           
				</li>
			<% } %>

			<% if (Model.SourceType.ToLowerInvariant() != "pxunit" && Model.AssignTabSettings.ShowGradebookCategory) { %>
				<li id="divGradebookCategory" style="<%= gradableVisible %>">
					<label for="Assignment_Syllabus">Gradebook Category:</label>
					<select id="selgradebookweights" name="selgradebookweights" class="selgradebookweights" onchange="ContentWidget.IsFormChanged()">
						<%Model.Category = string.IsNullOrEmpty(Model.Category) ? "1" : Model.Category;%> 
						<%foreach (var filter in Model.GradeBookWeights.GradeWeightCategories.OrderBy(i => int.Parse(i.Id))) {%>
							<option value="<%=filter.Id%>" <% = (filter.Id == Model.Category) ? "selected='selected'" : "" %>><%=filter.Text%></option>
						<% } %>
					</select>
					<a href="#" id="ggbcategorydialog_link" class="link-list ggbcategorydialog_link">Add New</a>
				</li>
			<% } %>

			<% if (Model.AssignTabSettings.ShowIncludeScore) { %>
				<%-- ShowIncludeScore , gradereleasedate--%>
				<li id="divIncludeGbbScore" style="<%= gradableVisible %>">
					<label id="Assignment_IncludeGbbScore">Include score in gradebook: </label>
					<select id="selIncludeGbbScoreTrigger" name="selIncludeGbbScoreTrigger">
						<option value="1">After item is complete or due date has passed</option>
						<option value="2">Only after due date has passed</option>
						<option value="0">Always</option>
					</select>
				</li>
			<% } %>
				
			<% if (Model.AssignTabSettings.ShowCalculationType ) {%>
				<li id="divCalculationType" style="<%=gradableVisible%>">
					<label for="Assignment_CalculationType">Calculation Type:</label>
					<select id="selCalculationTypeTrigger" name="selCalculationTypeTrigger" onchange="ContentWidget.IsFormChanged()">
						<% foreach (var option in Model.AvailableSubmissionGradeAction)
         {%>
                            <option value="<%=option.Key %>" <%= (option.Key == Model.SubmissionGradeAction.ToString())? "selected" : "" %>>
                                <%=option.Value.ToString().Replace("_", " ") %></option>
						<% }%>
					</select>
				</li>
			<%}%>
				
                <li id="startDateField" style="display: none;">
				    <label for="Assignment_StartDate">Start Date:</label>
				    <% var ddlabelStart = Model.StartDate.Year == DateTime.MinValue.Year ? "N/A" : Model.StartDate.ToString("dddd MMM d, yyyy"); %>
				    <input type="text" class="readonly" readonly="readonly" id="StartDate" name="StartDate" value="<%= ddlabelStart %>" />
			    </li>
			</ol>
		</fieldset>
	    
        <%--Region Mark as Complete--%>        
        <fieldset id="fsMarkAsComplete">
            <% 
            var markAsCompleteChecked = (Model.IsMarkAsCompleteChecked) ? "checked" : "";
            var markAsCompletedVisible = (Model.IsMarkAsCompleteChecked) ? "display:block" : "display:none";
            %>
		   
            <ol>
                <%if (Model.AssignTabSettings.ShowMarkAsComplete) {%>
                <li id="liMarkAsComplete">
	                <input type="checkbox" id="chkMarkAsComplete" class="chkMarkAsComplete" <%= markAsCompleteChecked %> />
	                <label for="Assignment_ShowMarkAsComplete">Mark as completed when the student:</label>
                </li>
		   
                <li id="divShowMarkAsComplete" style=" <%= markAsCompletedVisible %>" >
                    <label for="ShowMarkAsComplete">Completion Category:</label>
                    <select id="selCompletionTrigger" name="selCompletionTrigger" onchange="ContentWidget.IsFormChanged()">
                        <option value="1" <%=Model.CompletionTrigger == CompletionTrigger.Submission ? "selected" : "" %>>Receives a score</option>
                        <option value="0" <%=Model.CompletionTrigger == CompletionTrigger.Minutes ? "selected" : "" %>>Minutes</option>
                        <option value="2" <%=Model.CompletionTrigger == CompletionTrigger.PassingScore ? "selected" : "" %>>Achieves a passing score on this activity</option>
                        <option value="3" <%=Model.CompletionTrigger == CompletionTrigger.View ? "selected" : "" %>>Views the activity</option>
                        <option value="4" <%=Model.CompletionTrigger == CompletionTrigger.View ? "selected" : "" %>>Number of sumbissions</option>
                    </select>
                </li>
		        <% } %>
            </ol>
        </fieldset>
	
	    <fieldset id="ePortfolioFields">
			<ol>
				<%                                           
                var btnAssignDisplay = (isAssigned) ? "none" : "inline";
				var btnSaveChangesDisplay = (isAssigned) ? "inline" : "none";
                var btnUnassignDisplay = (isAssigned) ? "inline" : "none";
				%>

				<% if (!Model.IsContentCreateAssign) {%>
				<li>
					<br/>                               
					<input type="button" id="btnAssign" value="Assign" class="assign btnAssign submit-action" style="display:<%= btnAssignDisplay %>" />
					<input type="button" id="btnSaveChanges" class="assign btnSaveChanges submit-action" value="Save Changes" style="background-color:#717D7D; display:<%= btnSaveChangesDisplay %>" 
					disabled="disabled" />
					&nbsp;
					<input type="button" id="btnUnassign" class="assign btnUnassign" value="Unassign" style="display:<%= btnUnassignDisplay %>" />                 
				</li>                
				<% }
                else { %>
                <li>
					<input type="hidden" value="true" id="hdnIsContentCreateAssign" />
                </li>
				<% } %>
			</ol>
		</fieldset>

        <div class="divAllowedTemplates">
	        <% foreach (var relatedTemplate in Model.RelatedTemplates) {
                Html.RenderPartial("AllowedContentTemplate", relatedTemplate);
	        } %>
        </div>

	</div>
	<div style="clear: both;"/>

    <%if (ViewData["isError"] != null) {%>
	<input type="hidden" id="isError" value="true" />
	<%} %>
	<%= Html.HiddenFor(m => m.Id) %>
	<%= Html.HiddenFor(m => m.Title) %>
	<input type="hidden" id="lessonid" name="lessonid" value="<%= Model.lessonId %>" />
	<input type="hidden" id="SyllabusFilter" name="SyllabusFilter" value="<%= selectedFilterId %>" />
	<%= Html.HiddenFor(m => m.Category)%>
	<%= Html.HiddenFor(m => m.CompletionTrigger)%>
	<%= Html.HiddenFor(m => m.SourceType)%>
	<input type="hidden" id="requestType" name="requestType" value="<%=Model.RequestType%>" />
    <input type="hidden" value="<%= isAssigned %>" id="hdnIsAssigned" />
	<input type="hidden" value="<%= Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy") %>" id="hdnDueDate" />            
	<input type="hidden" value="<%= Model.DueDate.Hour%12 %>" id="hdnDueHour" />
	<input type="hidden" value="<%= Model.DueDate.Minute %>" id="hdnDueMinute" />
	<input type="hidden" value="<%= (Model.DueDate.Hour>=12) ? "pm" : "am" %>" id="hdnDueAmpm" />
	<input type="hidden" value="<%= Model.CompletionTrigger %>" id="hdnCompletionTrigger" />
	<input type="hidden" value="<%= Model.Category %>" id="hdnGradebookCategory" />
	<input type="hidden" value="<%= selectedFilterId %>" id="hdnSyllabusCategory" />
	<input type="hidden" value="<%= Model.IncludeGbbScoreTrigger %>" id="hdnIncludeGbbScoreTrigger"/>
	<input type="hidden" value="<%= Model.GradeRule%>" id="hdnCalculationTypeTrigger" />
	<input type="hidden" value="<%= Model.IsGradeable %>" id="hdnGradable" />
	<input type="hidden" value="<%= Model.IsMarkAsCompleteChecked %>" id="hdnIsMarkAsComplete" />
	<input type="hidden" value="<%= Model.Score.Possible %>" id="hdnGradePoints" />
	<input type="hidden" value="<%= Model.IsAllowLateSubmission %>" id="hdnLateSubmission" />
	<input type="hidden" value="<%= Model.IsSendReminder %>" id="hdnSendReminder" />
	<input type="hidden" value="false" id="hdnAssignTabContentCreate" />
    <input type="hidden" value="<%= Model.CourseType %>" id="courseType" />
</div>