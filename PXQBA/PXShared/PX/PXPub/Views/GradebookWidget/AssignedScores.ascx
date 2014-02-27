<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GradeBook>" %>

<div id="headerToolTips">
<% 
    foreach (var folder in Model.Assignments)
    {
        foreach (var assignment in folder.Value)
        {
            var rule = Model.GetGradeRule((GradeRule)assignment.AssignmentSettings.GradeRule, true);
            var settingsUrl = Url.GetComponentHash("item", assignment.Id, new
            {
                mode = ContentViewMode.Settings,
                includeNavigation = true,
                isBeingEdited = false,
                renderFNE = true
            });
            var summaryUrl = Url.GetComponentHash("gradebook", "assignmentSummary", new Dictionary<string, object>() { { "assignmentid", assignment.Id } });
            var assignmentUrl = Url.GetComponentHash("gradebook", "viewAssignment", new Dictionary<string, object>() { { "itemid", assignment.Id } });
    
%>
    <!-- View code for the "Assigned Scores" section of the gradebook -->
    <div id="<%=assignment.Id + "headerToolTips"%>" class="menutip" style="display:none">
        <span class="ttgradebook headerSort">Sort by this column</span>
        <span class="ttgradebook headerAssignment" ref="<%=assignmentUrl %>">View Assignment</span>
        <span class="ttgradebook headerSummary" ref="<%=summaryUrl %>">Assignment Summary</span>
        <span class="displaying">Displaying: <span id="gradeRule"><%=rule%></span> Score </span><a href="<%=settingsUrl %>" class="headerChange">Change</a>
    </div>
<%      }
    } %>
</div>
<div id="gridkey" style="display:none">
    <table id="keytable">
        <thead>
            <tr class="header">
                <th id="keyCol">Gradebook Key</th>
                <th id="descCol"></th>
                <th id="closeCol"><span class="closeicon"></span> Close</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td class="noattempt">no attempt</td>
                <td class="keydesc">The student has not attempted the assignment yet.</td>
            </tr>
            <tr>
                <td class="needsgrading">grade</td>
                <td class="keydesc">The submission needs grading.</td>
            </tr>
            <tr>
                <td class="graded">100% (10/10)</td>
                <td class="keydesc">100% (10/10) - Indicates the student's score (with points earned/points possible).</td>
            </tr>
            <tr>
                <td class="assignmentheader">assignment title <span class="gradingcount">(2)</span></td>
                <td class="keydesc">The number in brackets indicates the number of submissions that need grading.</td>
            </tr>
        </tbody>
    </table>
</div>
<table id="assignedScoresContent">
    <thead>
        <tr>
            <th id="nameCol" class="nameCol fixedCol">
                <div class="headerBox">
                    Name
                    <span class="sortIcon"></span>
                </div>
            </th>
            <th id="totalAvgCol" class="avgCol fixedCol">
                <div class="headerBox">
                    Average for all assignments
                    <span class="sortIcon"></span>
                </div>
            </th>
            <%  foreach (var assignmentFolder in Model.Assignments)
                {
                    var assignmentFolderItem = assignmentFolder.Key;
                    var assignmentItems = assignmentFolder.Value;
                   %>
                    <th id="<%= assignmentFolderItem.Id %>" class="sortableCol folder">
                        <div class="headerBox">        
                            <%= assignmentFolderItem.Title%>
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                <%  foreach (var assignmentItem in assignmentItems)
                    {
                        string gradeRule = Model.GetGradeRule((GradeRule)assignmentItem.AssignmentSettings.GradeRule, true);
                        var upforgrading = string.Format("({0})", Model.UpForGrading(assignmentItem.Id));
                     %>
                        <th id="<%= assignmentItem.Id %>" class="menuHeaderCol assignmentitems" gradeRule="<%= gradeRule %>">
                            <div class="headerBox"> 
                                 <span> <%= assignmentItem.Title %> </span>
                                 <span class="upforgrading"> <%= Model.UpForGrading(assignmentItem.Id) != 0 ? upforgrading : string.Empty%></span>
                                 <span class="sortIcon"></span>
                            </div>
                        </th>
                        <!-- This is just for sorting when we have a column that needs to sort on non-visible data... like a sequence-->
                        <th id="<%=assignmentItem.Id %>-sort" class="headersort" />
                <%  } %>
            <%  } %>
        </tr>
    </thead>
    <tbody>
    <% foreach (var student in Model.Students)
       {
           var studentEnrollmentId = student.EnrollmentId;
           var studentGrade = Model.GetAssignment(studentEnrollmentId, null);
           var studentAggGradeDisplay = studentGrade != null ? studentGrade.GradeDisplay : string.Empty;
           %>
        <tr>
            <td userid="<%= student.UserId %>" enrollmentid="<%= studentEnrollmentId %>" class="colName" ref="<%= Url.GetComponentHash("gradebook", "studentDetail", new Dictionary<string, object>() {
                                                                                                                        {"studentUserId", student.UserId },
                                                                                                                        {"studentEnrollmentId", studentEnrollmentId }
                                                                                                                    }) %>">
                <div class="studentName"><%= student.FormattedName%></div>
                <div class="studentEmail"><%= student.Email%></div>
            </td>
            <td class="studentGrade"> <%= studentAggGradeDisplay %></td>
            <% 
                var folderClassIncrement = 0;
                foreach (var assignmentFolder in Model.Assignments)
                {
                    var assignmentFolderItem = assignmentFolder.Key;
                    var assignmentItems = assignmentFolder.Value;
                    var folderGrade = Model.GetAssignment(studentEnrollmentId, assignmentFolderItem.Id);
                    folderClassIncrement++;
                    var folderClass = string.Format("{0}{1}", "assignmentfolder", folderClassIncrement);
                    var folderLabel = string.Format("{0}", "assignmentfolder");
                    %>
                    <td enrollmentid="<%= studentEnrollmentId %>" itemid="<%= folderGrade.ItemId %>" class="<%= folderClass %> <%= folderLabel %>"><%= folderGrade.GradeDisplay %></td>
                <%  foreach (var assignmentItem in assignmentItems) 
                    {
                        var assignmentGrade = Model.GetAssignment(studentEnrollmentId, assignmentItem.Id);
                        var assignmentClass = string.Format("{0}{1}", "assignmentitem", folderClassIncrement);
                        var assignmentLabel = string.Format("{0}", "assignmentitem");
                    %>
                        <td enrollmentid="<%= studentEnrollmentId %>" itemid="<%= assignmentItem.Id %>" class="<%= assignmentClass %> <%= assignmentLabel %>"> 
                        <%  if (assignmentGrade!=null && assignmentGrade.IsSubmitted && assignmentGrade.IsGraded)
                            { %>
                                <div class="graded"> 
                                <%= Html.ActionLink(assignmentGrade.GradeDisplay, "DisplayItem", "ContentWidget", new { id = assignmentGrade.ItemId, enrollmentid = studentEnrollmentId, assigned = true, mode = ContentViewMode.Grading }, new { @class = "fne-link" })%>
                                </div>
                        <%  }
                            else if (assignmentGrade!=null && assignmentGrade.IsSubmitted && !assignmentGrade.IsGraded)
                            {%>
                                <div class="notgraded">
                                <%= Html.ActionLink("Grade", "DisplayItem", "ContentWidget", new { id = assignmentGrade.ItemId, enrollmentid = studentEnrollmentId, assigned = true, mode = ContentViewMode.Grading }, new { @class = "fne-link" })%>
                                </div>
                        <%  }
                            else
                            {%>
                                <div class="no-attempt-table">no attempt</div>
                        <%  } %>
                        </td>
                        <td> 
                        <%  if (assignmentGrade!=null && assignmentGrade.IsSubmitted && assignmentGrade.IsGraded)
                            { %>
                                <%= assignmentGrade.GradeScoreNumeric%>
                        <%  }
                            else if (assignmentGrade!=null && assignmentGrade.IsSubmitted && !assignmentGrade.IsGraded)
                            {%>
                                -2
                        <%  }
                            else
                            {%>
                                -1
                        <%  } %>
                        </td>
                <%  }
                %>
            <% } %>
        </tr>
    <% } %>
    </tbody>
</table>
<%= Html.Hidden("hdnClassAvg", string.Format("{0:0.0%}", Model.ClassAverage), new { @class = "hdnClassAvg", id = "hdnClassAvg" })%>