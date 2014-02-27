<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>
<style>
    ul.outline li
    {
        padding: 0px;
    }
</style>
<%
    var updateTargetId = "content";

    var isAssigned = (Model.DueDate.Year != DateTime.MinValue.Year);

    var disabledMode = Model.IsReadOnly ? "disabled" : string.Empty;
%>

<% 
    var selectedFilter = Model.Syllabus.ChildrenFilterSections.Find(i => i.Id == Model.SyllabusFilter);
    var selectedFilterId = (selectedFilter == null) ? Model.SyllabusFilter : selectedFilter.Id.ToString();              
%>
<div id="cal-box">
   
    <div id="assignment-calendar" class="range px-calendar">
    </div>
</div>

<%var showAssignedSameDay = (Model.AssignTabSettings.ShowAssignedSameDay) ? "" : "ShowAssignedSameDay";  %>
<% if (Model.AssignTabSettings.ShowAssignedSameDay)
   { %>
<div id="other-assignments">
</div>
<div style="clear: both">
</div>
<% } %>

<div id="form" class="<%=showAssignedSameDay%>">
    <%             
        var checkedDateTime = isAssigned ? "checked" : "";
        var showDateTime = isAssigned ? "display: inline" : "display: none";
        var ddlabel = Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy");
        var ddlTime = Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("hh:mm tt");
    %>
    <ol style="padding: 0;">
        <li id="liDueDate">
            <input type="hidden" class="isCalendarInitialized" />
            <input type="text" id="settingsAssignDueDate" value="<%= (ddlabel == "") ? "" : ddlabel %>" placeholder='<%= "e.g. " + DateTime.Now.ToShortDateString() %>'  <%= disabledMode %> />
            <input type="text" id="settingsAssignTime" value="<%=ddlTime%>" style="width: 75px;" <%= disabledMode %>/>
            <% if (!Model.IsReadOnly) { %>
                <a href="#" id="clearDateField">clear</a>
            <% } %>
            <%
                var hours = (Model.DueDate.Year < 1900) ? "0" : (Model.DueDate.Hour > 12) ? (Model.DueDate.Hour - 12).ToString() : Model.DueDate.Hour.ToString();
                var minutes = (Model.DueDate.Year < 1900) ? "0" : Model.DueDate.Minute.ToString();
                var year = (Model.DueDate.Year < 1900) ? "" : Model.DueDate.Year.ToString().ToLowerInvariant();
                var amPM = (Model.DueDate.Year < 1900) ? "am" : (Model.DueDate.ToShortTimeString().ToLowerInvariant().Contains("am")) ? "am" : "pm";
                    
            %>
            <input type="hidden" name="dueHour" id="dueHour" value="<%=hours%>" />
            <input type="hidden" name="dueMinute" id="dueMinute" value="<%=minutes%>" />
            <input type="hidden" name="dueAmpm" id="dueAmpm" value="<%=amPM%>" />
            <input type="hidden" name="dueYear" id="dueYear" value="<%=year%>" />
            <input type="hidden" id="DueDate" name="DueDate" class="DueDate" value="<%=ddlabel %>" style="width: 140px; display: none;" />
        </li>
        <li>
        <span class="invaliddate">Please enter due date in an acceptable format such as '<%=DateTime.Now.ToString("MM/dd/yyyy")%>.'<br /></span>
        <span class="invalidtime">Please enter time in an acceptable format such as '<%=DateTime.Now.ToString("hh:mm")%>.'<br /></span>
        </li>
    </ol>
    <fieldset id="fsDateTime" style="<%= showDateTime %>">
        <ol>
            <%--------------------- Region Allow Late Submission ------------------------%>
            <% if (Model.AssignTabSettings.ShowAllowLateSubmissions)
               {
                   var lateSubmissionChecked = Model.IsAllowLateSubmission ? "checked" : "";
                   var lateSubmissionDisplay = Model.IsAllowLateSubmission ? "" : "disabled = 'disabled'";
                   var lateSubmissionDisplayStyle = Model.IsAllowLateSubmission ? "" : "display:none;";
                   var highlightLateChecked = "checked";
                   var gracePeriodChecked = Model.IsAllowLateGracePeriod ? "checked" : "";
                   var gracePeriodDisplay = Model.IsAllowLateGracePeriod ? "" : "disabled = 'disabled'";
                   var gracePeriodDisplayStyle = Model.IsAllowLateGracePeriod ? "" : "display:none;";
                   LateGraceDurationType lateGraceDurationType;
                   if(!Enum.TryParse(Model.LateGraceDurationType, true, out lateGraceDurationType))
                       lateGraceDurationType = LateGraceDurationType.Minute;
                   
                   if ((Model.LateGraceDurationType == "minute" || string.IsNullOrEmpty(Model.LateGraceDurationType)) && Model.LateGraceDuration == 0)
                   {
                       Model.LateGraceDuration = 15;
                   }
                   var disableGracePeriodInput = Model.IsReadOnly || lateGraceDurationType == LateGraceDurationType.Infinite? "disabled" : string.Empty;
                   
            %>
            <li id="liAllowLaterSubmissions">
                <input type="hidden" id="hdnHighlightLateSubmission" value="<%= Model.IsHighlightLateSubmission %>" />
                <input type="hidden" id="hdnAllowLateUntilGracePeriod" value="<%= Model.IsAllowLateGracePeriod %>" />
                <input type="hidden" id="hdnLateGracePeriodDuration" value="<%= Model.LateGraceDuration %>" />
                <input type="hidden" id="hdnLateGracePeriodDurationType" value="<%= Model.LateGraceDurationType %>" />
                
                <input type="checkbox" id="chkAllowLateSubmissions" <%= lateSubmissionChecked %>
                    onclick="ContentWidget.IsFormChanged()" <%= disabledMode %> />
                <div id="allowLateSubmissionsLabel">
                    Allow late submissions until grace period expires
                </div>
                <div id="gracePeriodDetails" style="<%= gracePeriodDisplayStyle%>">
                    <label for="LateGracePeriodDuration"> Grace Period: </label>
                    <input type="text" <%= gracePeriodDisplay %> id="LateGracePeriodDuration" class="reminderDayCount"
                        onkeypress="return ContentWidget.AllowNumbersOnly(event);" onblur="ContentWidget.IsFormChanged()" <%= disableGracePeriodInput %>
                        value="<%= Model.LateGraceDuration %>" />
                    <select <%= gracePeriodDisplay %> id="LateGracePeriodDurationType" onchange="ContentWidget.IsFormChanged()" <%= disabledMode %>>
                        <option <%= (lateGraceDurationType == LateGraceDurationType.Minute) ? "selected='selected'" : "" %>
                            value="<%= LateGraceDurationType.Minute.ToString() %>">
                            minutes
                        </option>
                        <option <%= (lateGraceDurationType == LateGraceDurationType.Hour) ? "selected='selected'" : "" %>
                            value="<%= LateGraceDurationType.Hour.ToString() %>">
                            hours
                        </option>
                        <option <%= (lateGraceDurationType == LateGraceDurationType.Day) ? "selected='selected'" : "" %>
                            value="<%= LateGraceDurationType.Day.ToString() %>">
                            days
                        </option>
                        <option <%= (lateGraceDurationType == LateGraceDurationType.Week) ? "selected='selected'" : "" %>
                            value="<%= LateGraceDurationType.Week.ToString() %>">
                            weeks
                        </option>
                        <option <%= (lateGraceDurationType == LateGraceDurationType.Infinite) ? "selected='selected'" : "" %>
                            value="<%= LateGraceDurationType.Infinite.ToString() %>">
                            infinite
                        </option>
                    </select>
                </div>
            </li>
            
            <% } %>
            <%--------------------- Region Send Reminder ------------------------%>
            <% if (Model.AssignTabSettings.ShowSendReminder)
               {
                   var sendReminderChecked = Model.IsSendReminder ? "checked" : "";
                   var sendReminderDisplay = Model.IsSendReminder ? "" : "disabled = 'disabled'";
                   var sendReminderDisplayStyle = Model.IsSendReminder ? "" : "display:none;";

                   if (Model.ReminderEmail.DaysBefore == 0)
                   {
                       Model.ReminderEmail.DaysBefore = 1;
                   }
            %>
            <li id="liScheduleEmailReminder">
                <label for="chkScheduleEmailReminder" style="width: 300px">
                    <input type="checkbox" class="fsEnabler" id="chkScheduleEmailReminder" <%= sendReminderChecked %> <%= disabledMode %>
                        onclick="ContentWidget.IsFormChanged()" />
                    Schedule an email reminder for students</label>
            </li>
            <li>
                <input type="hidden" id="hdnReminderBeforeCount" value="<%= Model.ReminderEmail.DaysBefore %>" />
                <input type="hidden" id="hdnReminderBeforeType" value="<%= Model.ReminderEmail.DurationType %>" />
                <input type="hidden" id="hdnReminderSubject" value="<%= Model.ReminderEmail.Subject %>" />
                <input type="hidden" id="hdnReminderBody" value="true" />
                <ol style="margin-left: 10px; <%= sendReminderDisplayStyle%>" id="reminderEmailDeatils">
                    <li>
                        <%--<label style="width:auto;" for="ReminderBeforeCount">Send advance warning: </label> --%>
                        <input type="text" <%= sendReminderDisplay %> id="ReminderBeforeCount" class="reminderDayCount"
                            onkeypress="return ContentWidget.AllowNumbersOnly(event);" onblur="ContentWidget.IsFormChanged()"
                            value="<%= Model.ReminderEmail.DaysBefore %>" <%= disabledMode %> />
                        <select <%= sendReminderDisplay %> id="ReminderBeforeType" onchange="ContentWidget.IsFormChanged()" <%= disabledMode %>>
                            <option <%= (Model.ReminderEmail.DurationType=="minute") ? "selected='selected'" : "" %>
                            value="minute">minutes</option>
                            <option <%= (Model.ReminderEmail.DurationType=="hour") ? "selected='selected'" : "" %>
                            value="hour">hours</option>
                            <option <%= (Model.ReminderEmail.DurationType=="day") || (Model.ReminderEmail.DurationType== null) ? "selected='selected'" : "" %>
                            value="day">days</option>
                            <option <%= (Model.ReminderEmail.DurationType=="week") ? "selected='selected'" : "" %>
                            value="week">weeks</option>
                        </select>
                        <span id="emailReminderText">before the due date</span> </li>
                    <li style="float: left;">
                        <label for="ReminderSubject" style="width: auto;">
                            Subject</label>
                        <br />
                        <input <%= sendReminderDisplay %> type="text" id="ReminderSubject" class="reminderTitle"
                            value="<%= Model.ReminderEmail.Subject %>" onblur="ContentWidget.IsFormChanged()" <%= disabledMode %> />
                    </li>
                    <li>
                        <label>
                            Message</label></li>
                    <li>
                        <!-- Changed from tinymce textarea to normal text area by design -->

                        <% var texareaAttributes =
                               Model.IsReadOnly ? new { disabled = disabledMode } : new object { }; %>
                        <%= Html.TextAreaFor(m => m.ReminderEmail.Body, texareaAttributes)%>
                    </li>
                </ol>
            </li>
            <% } %>
        </ol>
    </fieldset>
</div>

<div style="clear: both;" />
<%if (ViewData["isError"] != null)
  {%>
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
<input type="hidden" value="<%= Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy") %>"
    id="hdnDueDate" />
<input type="hidden" value="<%= Model.DueDate.Hour%12 %>" id="hdnDueHour" />
<input type="hidden" value="<%= Model.DueDate.Minute %>" id="hdnDueMinute" />
<input type="hidden" value="<%= (Model.DueDate.Hour>=12) ? "pm" : "am" %>" id="hdnDueAmpm" />
<input type="hidden" value="<%= (int)Model.CompletionTrigger %>" id="hdnCompletionTrigger" />
<input type="hidden" value="<%= Model.Category %>" id="hdnGradebookCategory" />
<input type="hidden" value="<%= selectedFilterId %>" id="hdnSyllabusCategory" name="hdnSyllabusCategory" />
<input type="hidden" value="<%= Model.IncludeGbbScoreTrigger %>" id="hdnIncludeGbbScoreTrigger" />
<input type="hidden" value="<%= Model.GradeRule%>" id="hdnCalculationTypeTrigger" />
<input type="hidden" value="<%= Model.IsGradeable %>" id="hdnGradable" />
<input type="hidden" value="<%= Model.IsMarkAsCompleteChecked %>" id="hdnIsMarkAsComplete" />
<input type="hidden" value="<%= Model.Score.Possible %>" id="hdnGradePoints" />
<input type="hidden" value="<%= Model.IsAllowLateSubmission %>" id="hdnLateSubmission" />
<input type="hidden" value="<%= Model.IsSendReminder %>" id="hdnSendReminder" />
<input type="hidden" value="false" id="hdnAssignTabContentCreate" />
<input type="hidden" value="<%= Model.CourseType %>" id="courseType" />
<%--</div>--%>
<div class="">
</div>
