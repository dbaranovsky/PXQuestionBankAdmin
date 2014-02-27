<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.StudentGradebook>" %>
<!-- View you get when clicking on an assignment from the gradebook widget's Assigned Scores view -->

<% 
    var name = ViewData["ItemName"];
    var average = ViewData["Average"];
%>
<div id="assignmentsummary">
    <h1 class="title">Assignment Summary</h1><br />
    <div class="content">
        <h2 class="assignmenttitle"><%=name %></h2><br />
        <div class="average">Average: <span><%=average.ToString() %>%</span></div>
        <table id="summaryContent">
            <thead>
                <tr>
                    <th>
                        <div class="headerBox">  
                            Student
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th>
                        <div class="headerBox">  
                            Score
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th>
                        <div class="headerBox">  
                            Attempts
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
            <% 
                foreach (var grade in Model.Grades) 
                { %>
                    <tr id="<%=grade.EnrollmentId%>">
                        <td>
                            <span class="assignmentToggle toggleclose"></span>
                            <span><%=grade.EnrollmentName%></span>
                        </td>
                        <td><%
                            if (grade.IsSubmitted && !grade.IsGraded)
                            {%>
                               <div class="notgraded">
                                <%= Html.ActionLink("Grade", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                </div>
                        <%  }
                            else if (grade.IsSubmitted && grade.IsGraded)
                            {%>
                                <%=grade.GradeScore%>
                        <%  } %>
                        </td>
                        <td>
                        <%  if (grade.AttemptList.Count > 0)
                            {%>
                            <div class="submission"><%= grade.AttemptDisplay%></div>
                        <%  }
                            else
                            {%>
                            <div class="no-attempt-table"><%= grade.AttemptDisplay%></div>
                        <%  }%>
                        </td>
                    </tr>
            <%  }   
            %> 
            </tbody>
        </table>
        <div id="rowdetails">
        <%
            foreach (var grade in Model.Grades)
            {
                if(grade.AttemptList.Count > 0) 
                {%>
                <div id="<%=grade.EnrollmentId + "rowdetails"%>" style="display:none">
                <%  foreach (var result in grade.AttemptList)
                    {
                        if (grade.IsGraded)
                        {%>
         
                            <label>
                                <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                submitted on <%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%> for a score of <%=result.Score + " (" + result.RawAchieved + "/" + result.RawPossible + ")"%>
                            </label>
                    <%  }
                        else
                        {%>
                            <br />
                            <label>
                                <!-- Add version once that becomes a parameter -->
                                <%= Html.ActionLink("Attempt " + result.Count + " Results", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                submitted on <%=result.Submitted.ToString("MM/dd/yy hh:mm tt")%> and has not been scored.
                                <span class="notgraded">
                                    <%= Html.ActionLink("Grade Submission", "DisplayItem", "ContentWidget", new { id= grade.ItemId, enrollmentid = grade.EnrollmentId, assigned=true, mode = ContentViewMode.Grading }, new { @class = "fne-link dialogcloser" }) %>
                                </span>
                            </label>
                    <%  }%>
                <%  } %>
                </div>
            <%  }      
            }%>
        </div>
    </div>
</div>