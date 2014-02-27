<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.StudentGradebook>" %>

<% var student = Model.Student;
   var studentLastLogin = student.LastLogin.HasValue ? student.LastLogin.Value.ToString("MM/dd/yyyy - hh:mm tt") : string.Empty;
   var displayAverage = ViewData["DisplayAverage"] != null ? (bool)ViewData["DisplayAverage"] : false;
   var isStudent = ViewData["IsStudent"] != null ? (bool)ViewData["IsStudent"] : false;
   var displaySubmittedDate = ViewData["DisplaySubmittedData"] != null ? (bool)ViewData["DisplaySubmittedData"] : false;
   var lastUpdatedDate = ViewData["LastUpdate"];

   if (lastUpdatedDate.Equals(string.Empty))
   {
       lastUpdatedDate = "NA"; 
   }
   
   %>

<div id="studentgradebook">
<%  if (!isStudent)
    { %>
    <div class="assignmentSummaryHeader">
        <form id="assignedGrouping">
            <input type="radio" name="isassigned" id="assignedtab" checked="checked"/><label for="assignedtab">Assigned Scores</label>
            <input type="radio" name="isassigned" id="othertab"/><label for="othertab">Other Scores</label>
        </form>

        <% if (displayAverage)
           { %>
        <div class="divStudentAvg">
            Your average score for all assignment: <span class="studentAvgVal"></span>
        </div>
        <% } %>
            <div class="assignedExport">
                <a class="assingedExportButton" href="<%= Url.Action("ExportAssignedReport", "GradebookWidget", new { studentUserId = student.UserId, studentEnrollmentId = student.EnrollmentId}) %>">
                    <span class="spnAssignedExport">Export</span>
                </a>
            </div>
            <div class="unassignedExport" style="display:none">
                <a class="unassingedExportButton" href="<%= Url.Action("ExportAdditionalItemReport", "GradebookWidget", new { studentUserId = student.UserId, studentEnrollmentId = student.EnrollmentId}) %>">
                    <span class="spnUnassignedExport">Export</span>
                </a>
            </div>
    </div>
<%  } %>
    <div id="divassigneditems">
        <div id="filter">
            <label>Assignment Folder </label>
            <select id="assignmentFilter">
                <option id="all">View All</option>
            <% 
                foreach (var assignmentFolder in Model.Assignments)
                {
                    var assignmentFolderItem = assignmentFolder.Key;
                    %>
                    <option id="<%= assignmentFolderItem.Id %>"><%= assignmentFolderItem.Title %></option> 
              <%}
            %>
            </select>
        </div>

        <table class="studenttable modal" id="assignments">
            <thead>
                <tr>
                    <th class="hdassignmentTitle">
                        <div class="headerBox">  
                            Assignment
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="hdassignmentScore">
                        <div class="headerBox">  
                            Score
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <% if (displaySubmittedDate)
                       { %>
                    <th class="hdassignmentSubmitted">
                        <div class="headerBox">  
                            Submitted
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <% } %>
                    <th class="hdassignmentAttempts">
                        <div class="headerBox">  
                            Attempts
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
            <% 
                foreach (var assignment in Model.Grades) 
                {
                    var submittedDate = assignment.IsSubmitted ? assignment.SubmittedDate.Value.ToString("MM/dd/yyyy hh:mm tt") : string.Empty;
                     %>
                    <tr id="<%= assignment.ItemId %>" folder="<%= assignment.ParentFolderId %>">
                        <td class="assignmentTitle">
                            <span class="assignmentToggle toggleclose"></span>
                            <span><%= assignment.ItemTitle %></span>
                        </td>
                        <td class="assignmentScore"><%
                            if (assignment.IsSubmitted && !assignment.IsGraded)
                            {
                                if (isStudent)
                                {%>
                                <span class="notgradedstudent">not graded</span>
                            <%  }
                                else
                                {%>
                                <div class="notgraded">
                                    <%= Html.ActionLink("Grade", "DisplayItem", "ContentWidget", new { id= assignment.ItemId, enrollmentid = assignment.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                </div>
                            <%  }
                            }
                            else if (assignment.IsSubmitted && assignment.IsGraded)
                            {%>
                                <%=assignment.GradeDisplay%>
                        <%  } %>
                        </td>
                        <% if (displaySubmittedDate)
                           {%>
                        <td class="assignmentSubmitted"> <%= submittedDate %> </td>
                        <% } %>
                        <td>
                        <%  if (assignment.AttemptList.Count > 0)
                            {%>
                            <div class="submission"><%= assignment.AttemptDisplay%></div>
                        <%  }
                            else
                            {%>
                            <div class="no-attempt-table"><%= assignment.AttemptDisplay%></div>
                        <%  }%>
                        </td>
                    </tr>
            <%  }   
            %>
            </tbody>
        </table>
    </div>

    <div id="divunassigneditems" style="display:none">

        <table class="studenttable" id="unassigneditems">
            <thead>
                <tr>
                    <th class="hdassignmentTitle">
                        <div class="headerBox">  
                            Item Name
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="hdassignmentScore">
                        <div class="headerBox">  
                            Score
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <% if (displaySubmittedDate)
                       { %>
                    <th class="hdassignmentSubmitted">
                        <div class="headerBox">  
                            Submitted
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <% } %>
                    <th class="hdassignmentAttempts">
                        <div class="headerBox">  
                            Attempts
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
            <% foreach (var assignment in Model.UnAssingedGrades)
               {  
                   var submittedDate = assignment.IsSubmitted ?  assignment.SubmittedDate.Value.ToString("MM/dd/yyyy hh:mm tt") : string.Empty;
                   %>
                    <tr id="<%= assignment.ItemId %>" >
                        <td class="assignmentTitle">
                            <span class="assignmentToggle toggleclose"></span>
                            <span><%= assignment.ItemTitle %></span>
                        </td>
                        <td class="assignmentScore"><%
                            if (assignment.IsSubmitted && !assignment.IsGraded)
                            {
                                if (isStudent)
                                {%>
                                    <span class="notgradedstudent">not graded</span>
                            <%  }
                                else
                                {%>
                                 <div class="notgraded dialogcloser">
                                    <%= Html.ActionLink("Grade", "DisplayItem", "ContentWidget", new { id= assignment.ItemId, enrollmentid = assignment.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                </div>
                            <%  } 
                            }
                            else if (assignment.IsSubmitted && assignment.IsGraded)
                            {%>
                                <%=assignment.GradeDisplay%>
                        <%  } %>
                        </td>
                        <% if (displaySubmittedDate)
                           { %>
                        <td class="assignmentSubmitted"> <%= submittedDate %> </td>
                        <% } %>
                        <td>
                        <%  if (assignment.AttemptList.Count > 0)
                            {%>
                            <div class="submission"><%= assignment.AttemptDisplay%></div>
                        <%  }
                            else
                            {%>
                            <div class="no-attempt-table"><%= assignment.AttemptDisplay%></div>
                        <%  }%>
                        </td>
                    </tr>
            <% } %>
            </tbody>
        </table>
    
    </div>

    <div id="rowdetails">
        <%
            foreach (var grade in Model.Grades)
            {
                if (grade.AttemptList.Count > 0)
                { %>
                    <div id="<%= grade.ItemId + "rowdetails"%>" style="display:none">
                    <%  foreach (var result in grade.AttemptList)
                        {
                            if (grade.IsGraded)
                            {%>
                                <div class="details">
                                    <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                    submitted on  <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span> for a score of <span class="score"><%=result.Score + " (" + result.RawAchieved + "/" + result.RawPossible + ")"%></span>
                                </div>
                        <%  }
                            else
                            {%>
                                <div class="details">
                                    <!-- Add version once that becomes a parameter -->
                                    <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                    submitted on <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span>
                                <%  if (isStudent)
                                    {%>
                                        and has not been scored by the instructor.
                                <%  }
                                    else
                                    {%>
                                        and has not been scored.
                                        <span class="notgraded">
                                            <%= Html.ActionLink("Grade Submission", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                        </span>
                                <%  }%>
                                </div>
                        <%  }%>
                    <%  }%>
                        <div class="graderule">
                            <span class="gradeRuleNote">Note on Scoring:</span>
                            <span class="gradeRuleText">The <%= grade.GetGradeRule()%> of the attempts will be used as the students score.</span>
                        </div>
                    </div>
            <%  }
            }
            foreach (var grade in Model.UnAssingedGrades)
            {
                if (grade.AttemptList.Count > 0)
                { %>
                <div id="<%= grade.ItemId + "rowdetails"%>" style="display:none;">
                    <%  foreach (var result in grade.AttemptList)
                        {
                            if (grade.IsGraded)
                            {%>
                                <div class="details">
                                    <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                                    submitted on  <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span> for a score of <span class="score"><%=result.Score + " (" + result.RawAchieved + "/" + result.RawPossible + ")"%></span>
                                </div>
                        <%  }
                            else
                            {%>
                                <div class="details">
                                    <!-- Add version once that becomes a parameter -->
                                    <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                                    submitted on <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span>
                                <%  if (isStudent)
                                    {%>
                                        and has not been scored by the instructor.
                                <%  }
                                    else
                                    {%>
                                        and has not been scored.
                                        <span class="notgraded">
                                            <%= Html.ActionLink("Grade Submission", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                                        </span>
                                <%  }%>
                                </div>
                        <%  }%>
                    <%  }%>
                    <div class="graderule">
                        <span class="gradeRuleNote">Note on Scoring:</span>
                        <span class="gradeRuleText">The <%= grade.GetGradeRule()%> of the attempts will be used as the students score.</span>
                    </div>
                </div>
            <% }
            } %>
    </div>

    <% if (isStudent)
       { %>
    <div class="studentSummary">
    <div class="summary">SUMMARY</div>
    <%--<div><span class="summaryText">Pages Viewed:</span> <span class="summaryVal"></span></div>--%>
    <div class="login"><span class="summaryText login">Last Login:</span> <span class="summaryVal"><%= studentLastLogin%></span></div>
    <%--<div><span class="summaryText">Minutes Logged in:</span> <span class="summaryVal"></span></div>--%>
    <div class="updated"><span class="summaryText updated">Last Updated:</span> <span class="summaryVal"><%= lastUpdatedDate %></span></div>    
    </div>
<% } %>
<%= Html.Hidden("hdnAssignedAvg", string.Format("{0:0.0%}", Model.AssignedAverage), new { @class = "hdnAssignedAvg", id = "hdnAssignedAvg" })%>
<%= Html.Hidden("hdnUnAssignedAvg", string.Format("{0:0.0%}", Model.UnAssignedAverage), new { @class = "hdnUnAssignedAvg", id = "hdnUnAssignedAvg" })%>

</div>

<%--<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.dataTables.min.js") %>',
                        '<%= Url.ContentCache("~/Scripts/DataTablesExtras/FixedColumns.min.js") %>',
                        '<%= Url.ContentCache("~/Scripts/jquery/jquery.qtip.min.js") %>',
                        '<%= Url.ContentCache("~/Scripts/GradebookWidget/Gradebook.js") %>'], function () {
                            PxGradebook.InitStudentDetail();
                        });
        });
    } (jQuery))
</script>--%>

