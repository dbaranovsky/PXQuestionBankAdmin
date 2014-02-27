<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

    <input type="hidden" value="<%=Model.Content.Id%>" name = "Id" />

    <ul>
        <li>            
            <div style="float:left">
                <input type="radio" id="visibleforstudent" name="visibility" value="visibleforstudent" class="visibility" <%= !(Model.Content.HideFromStudents() || Model.Content.RestrictedByDate()) ? "checked" : "" %> /> 
                <span class="spanVisibility">Visible to students</span>
            </div>                
            <br />
            <div style="clear:both" />
            <div style="float:left">
                <input type="radio" id="hidefromstudent" name="visibility" value="hidefromstudent" class="visibility" <%= Model.Content.HideFromStudents() ? "checked" : "" %> /> 
                <span class="spanVisibility">Hide from students</span>
            </div>                
            <div class="lockIndividualStudent" style="float:left; display:none" title="This option cannot be changed for a group or an individual"></div>
            <br />
            <div style="clear:both" />
            <div style="float:left">
                <input type="radio" id="restrictedbydate" name="visibility" value="restrictedbydate" class="restricted" <%= Model.Content.RestrictedByDate() ? "checked" : "" %>/> 
                <span class="spanRestricted">Hide from students until date</span>
            </div>                
            <div class="lockIndividualStudent" style="float:left; display:none" title="This option cannot be changed for a group or an individual"></div>
            <br />

            <div class="restricted-calendar restricted" style="display:none;">
                <input type="hidden" class="isCalendarInitialized" />
                <%
                        string restrictedDate = Model.Content.RestrictedDate();
                        var ddlabel = string.Empty;
                        var ddlTime = string.Empty;

                        if (!restrictedDate.IsNullOrEmpty())
                        {
                            var date = DateTime.Parse(restrictedDate).GetCourseDateTime();                            
                            
                            if (date.Kind == DateTimeKind.Unspecified)
                            {
                                // for the date saved in "old" "MM/dd/yyyy hh:mm tt" format 
                                ddlabel = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local).ToShortDateString();
                                ddlTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local).ToShortTimeString();
                            }
                            else
                            {
                                // for the date saved in DLAP standard format with timezone 
                                ddlabel = date.ToShortDateString();
                                ddlTime = date.ToShortTimeString();
                            }
                        }
                 %>
                
                <ul>
                    <li class="li-cal-box">
                        <div id="cal-box" class="cal-box">
                            <span class="instructions">Click on a date to change the date.</span>                            
                            <div id="assignment-calendar" class="range px-calendar"></div>
                        </div>
                    </li>            
                    <li class="assign-picker-inputs">
                        <input type="text" id="settingsAssignDueDate"  value="<%= (ddlabel == "") ? "e.g. " + DateTime.Now.ToShortDateString() : ddlabel %>" />
                        <div class="calendar_toggle"></div>
                        <input type="text" id="settingsAssignTime"  value="<%=ddlTime%>" />
                        <input type="text" id="DueDate" name="DueDate" class="DueDate" value="<%= ddlabel %> <%=ddlTime%>" style="width:140px;display:none;" />
                        <a href="#" id="clearDateField">clear</a>
                    </li>
                </ul>
            </div>
        </li>  
    </ul>