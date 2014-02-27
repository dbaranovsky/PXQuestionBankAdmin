<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ItemGradeData>" %>
<!-- View for detailed scoring information for either a specific quiz or a specific student in the gradebook control -->
<% 
    var itemName = Model.ItemName;
    var lastSubmission = Model.Grades.Max(g => g.SubmittedDate);
    var average = Model.Average;
    var itemType = Model.ItemType;
    var gradeRule = Model.GradeRule;

    IEnumerable<Grade> itemColl = Model.Grades;

%>
<span id="backToScores" ref="<%= Url.GetComponentHash("gradebook", "otherScores") %>">Back to Scores</span>
<div id="itemInfo">
    <div class="name">
        <%=itemName%>
    </div>
    <div class="lastsub">
        Last Submission:<div class="icon"></div> <%=lastSubmission.HasValue ? lastSubmission.Value.ToString("MM/dd/yyyy ddd") : string.Empty%>
    </div>
    <div class="average">
    <%  if (itemType == "Item")
        {   %>
            Average: <%=average%> <span class="gradeRule">Average based on the <%= gradeRule %> score of each students attempt. <a id="gradeRuleChange">Change</a></span>
    <% }
        else
        {  %>
            Average: <%=average%>
        <%} %>
    </div>
</div>
<table id="itemScoreContent" class="hide">
    <thead>
        <tr>
            <th class="nameCol">
                <div class="headerBox">  
                    <%= Model.ItemType == "Item" ? "Student" : "Item"%>
                    <span class="sortIcon"></span>
                </div>
            </th>
            <th class="avgCol">
                <div class="headerBox">  
                    Score
                    <span class="sortIcon"></span>
                </div>
            </th>
            <th class="attemptsCol">
                <div class="headerBox">  
                    Attempts
                    <span class="sortIcon"></span>
                </div>
            </th>
        </tr>
    </thead>
    <tbody>
        <% 
        foreach (var item in itemColl)  
        { %>
        <tr id="<%=item.ItemId %>">
            <td class="name">
                <span class="assignmentToggle toggleclose"></span>
                <%=itemType == "Item" ? item.EnrollmentName : item.ItemTitle %>
            </td>
            <td class="average"><%
                if (item.IsSubmitted && !item.IsGraded)
                {%>
                    <div class="notgraded">
                    <%= Html.ActionLink("Grade", "DisplayItem", "ContentWidget", new { id= item.ItemId, enrollmentid = item.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
            <%  }
                else if (item.IsSubmitted && item.IsGraded)
                {%>
                <div class="graded">
                    <%= Html.ActionLink(item.GradeDisplay, "DisplayItem", "ContentWidget", new { id= item.ItemId, enrollmentid = item.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
            <%  } %>
            </td>
            <td>
            <%  if (item.AttemptList.Count > 0)
                {%>
                <div class="submission"><%= item.AttemptDisplay%></div>
            <%  }
                else
                {%>
                <div class="no-attempt-table"><%= item.AttemptDisplay%></div>
            <%  }%>
            </td>
        </tr>
    <%  }   
        %>
    </tbody>
</table>
<div id="rowdetails">
    <%
    foreach (var item in itemColl)
    {
        if (item.AttemptList.Count > 0)
        {%>
            <div id="<%=item.ItemId + "rowdetails"%>" style="display: none">
        
            <%  foreach (var result in item.AttemptList)
                {
                    if (item.IsGraded)
                    {%>
                        <div class="details">
                            <%= Html.ActionLink("Attempt" + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= item.ItemId, enrollmentid = item.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                            submitted on <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span> for a score of <span class="score"><%=result.Score + " (" + result.RawAchieved + "/" + result.RawPossible + ")"%></span>
                        </div>
                <%  }
                    else
                    {%>
                        <div class="details">
                            <!-- Add version once that becomes a parameter -->
                            <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= item.ItemId, enrollmentid = item.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                            submitted on <span class="date"><%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%></span> and has not been scored.
                            <span class="notgraded">
                                <%= Html.ActionLink("Grade Submission", "DisplayItem", "ContentWidget", new { id= item.ItemId, enrollmentid = item.EnrollmentId, assigned=false, mode = ContentViewMode.Grading }, new { @class = "fne-link" }) %>
                            </span>
                        </div>
                <%  }%>
            <%  }%>
            </div>
    <%  }
    } 
 %>
</div>
