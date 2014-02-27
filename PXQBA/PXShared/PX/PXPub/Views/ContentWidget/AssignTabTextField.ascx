<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>

<%
    var isMultipartLessons = ExtensionMethods.IsMultipartLessons(Request, ViewData);
    Model.lessonId = ExtensionMethods.GetMultipartPartLessonId(Request, ViewData);
    Model.isMultipart = isMultipartLessons.ToString();

    var updateTargetId = "content";
    var eportfolio = "Eportfolio";

    var callback = "null";
    if (isMultipartLessons)
        callback = "null";

    var isAssigned = (Model.DueDate.Year != DateTime.MinValue.Year);

    var assignmentSettingClass = "assigntab";
    if (Model.IsContentCreateAssign)
    {
        assignmentSettingClass = "contentcreate";
    }
			
%>

<div id="form">
    <%             
        var checkedDateTime = isAssigned ? "checked" : "";
        var showDateTime = isAssigned ? "display: inline" : "display: none";
        var ddlabel = Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToString("MM/dd/yyyy");
        var ddlTime = Model.DueDate.Year < 1900 ? "" : Model.DueDate.ToShortTimeString();          
    %>
    <ol style="padding: 0;">
        <li id="liDueDate">
            <input type="hidden" class="isCalendarInitialized" />
            <span class="datelabel">Date:</span>
            <input type="text" id="settingsAssignDueDate" value="<%= (ddlabel == "") ? "e.g. " + DateTime.Now.ToShortDateString() : ddlabel %>" style="width: 140px;" />
            <span class="timelabel">Time:</span>
            <input type="text" id="settingsAssignTime" value="<%=ddlTime%>" style="width: 75px;" />
            <a href="#" id="clearDateField">clear</a>
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
            <input type="hidden" id="DueDate" name="DueDate" class="DueDate" value="<%=ddlabel %> <%=ddlTime%>" style="width: 140px; display: none;" />
        </li>
        <li><span class="subtext">
            <%=Html.ToScreenStepLink("PX_UNIVERSE_WHATWILLTHISDO", "what will this do?")%></span></li>
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
                   var gracePeriodChecked = "checked";
                   var gracePeriodDisplay = Model.IsAllowLateGracePeriod ? "" : "disabled = 'disabled'";
                   var gracePeriodDisplayStyle = Model.IsAllowLateGracePeriod ? "" : "display:none;";
                   LateGraceDurationType lateGraceDurationType;
                   if (!Enum.TryParse(Model.LateGraceDurationType, true, out lateGraceDurationType))
                       lateGraceDurationType = LateGraceDurationType.Minute;
                   
            %>
            <li id="liAllowLaterSubmissions">
                <input type="checkbox" id="chkAllowLateSubmissions" <%= lateSubmissionChecked %>
                    onclick="ContentWidget.IsFormChanged()" />
                <label for="chkAllowLateSubmissions">
                    Allow late submissions until grace period expires</label>
            </li>
            <li>
                <input type="hidden" id="hdnHighlightLateSubmission" value="<%= Model.IsHighlightLateSubmission %>" />
                <input type="hidden" id="hdnAllowLateUntilGracePeriod" value="<%= Model.IsAllowLateGracePeriod %>" />
                <input type="hidden" id="hdnLateGracePeriodDuration" value="<%= Model.LateGraceDuration %>" />
                <input type="hidden" id="hdnLateGracePeriodDurationType" value="<%= Model.LateGraceDurationType %>" />
                <ol style="margin-left: 50px; <%= lateSubmissionDisplayStyle%>" id="lateSubmissionDeatils">
                    <li style="display: none;">
                        <input type="checkbox" id="chkHighlightLateSubmission" <%= highlightLateChecked %>
                            onclick="ContentWidget.IsFormChanged()" />
                        <label for="chkHighlightLateSubmission">
                            Highlight late submission in results</label>
                    </li>
                    <li style="display: none;">
                        <input type="checkbox" id="ckhAllowLateUntilGracePeriod" <%= gracePeriodChecked %>
                            onclick="ContentWidget.IsFormChanged()" />
                        <label for="ckhAllowLateUntilGracePeriod">
                            Allow late until graceperiod</label>
                    </li>
                    <li>
                        <div id="gracePeriodDetails" style="<%= gracePeriodDisplayStyle%>">
                            <label style="width: auto;" for="LateGracePeriodDuration">
                                Grace Period:</label>
                            <input type="text" <%= gracePeriodDisplay %> id="LateGracePeriodDuration" class="reminderDayCount"
                                onkeypress="return ContentWidget.AllowNumbersOnly(event);" onblur="ContentWidget.IsFormChanged()"
                                value="<%= Model.LateGraceDuration %>" />
                            <select <%= gracePeriodDisplay %> id="LateGracePeriodDurationType" onchange="ContentWidget.IsFormChanged()">
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
                            </select>
                        </div>
                    </li>
                </ol>
            </li>
            <% } %>
            <%--------------------- Region Send Reminder ------------------------%>
            <% if (Model.AssignTabSettings.ShowSendReminder)
               {
                   var sendReminderChecked = Model.IsSendReminder ? "checked" : "";
                   var sendReminderDisplay = Model.IsSendReminder ? "" : "disabled = 'disabled'";
                   var sendReminderDisplayStyle = Model.IsSendReminder ? "" : "display:none;";
            %>
            <li id="liScheduleEmailReminder">
                <label for="chkScheduleEmailReminder" style="width: 300px">
                    <input type="checkbox" class="fsEnabler" id="chkScheduleEmailReminder" <%= sendReminderChecked %>
                        onclick="ContentWidget.IsFormChanged()" />
                    Schedule an email reminder for students</label>
            </li>
            <li>
                <input type="hidden" id="hdnReminderBeforeCount" value="<%= Model.ReminderEmail.DaysBefore %>" />
                <input type="hidden" id="hdnReminderBeforeType" value="<%= Model.ReminderEmail.DurationType %>" />
                <input type="hidden" id="hdnReminderSubject" value="<%= Model.ReminderEmail.Subject %>" />
                <input type="hidden" id="hdnReminderBody" value="true" />
                <ol style="margin-left: 50px; <%= sendReminderDisplayStyle%>" id="reminderEmailDeatils">
                    <li>
                        <%--<label style="width:auto;" for="ReminderBeforeCount">Send advance warning: </label> --%>
                        <input type="text" <%= sendReminderDisplay %> id="ReminderBeforeCount" class="reminderDayCount"
                            onkeypress="return ContentWidget.AllowNumbersOnly(event);" onblur="ContentWidget.IsFormChanged()"
                            value="<%= Model.ReminderEmail.DaysBefore %>" />
                        <select <%= sendReminderDisplay %> id="ReminderBeforeType" onchange="ContentWidget.IsFormChanged()">
                            <option <%= (Model.ReminderEmail.DurationType=="day") ? "selected='selected'" : "" %>
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
                            value="<%= Model.ReminderEmail.Subject %>" onblur="ContentWidget.IsFormChanged()" />
                    </li>
                    <li>
                        <label>
                            Message</label></li>
                    <li>
                         <!-- Changed from tinymce textarea to normal text area by design -->
                        <%= Html.TextAreaFor(m => m.ReminderEmail.Body)%>
                    </li>
                </ol>
            </li>
            <% } %>
        </ol>
    </fieldset>
</div>