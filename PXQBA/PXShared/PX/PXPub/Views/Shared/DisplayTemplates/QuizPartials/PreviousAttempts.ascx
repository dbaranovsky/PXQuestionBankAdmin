<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>


<h2>Previous Attempts</h2>
<table class="previousattempts">
    <tr class="attemptsheader">
        <% if (Model.ShowGrade)
           {
        %>
        <th>Date</th><th>Grade</th><th>Attempts</th>
        <%
           }
           else
           {%>
        <th>Date</th><th>Attempts</th>
           <%}%>
        
    </tr>
     <% var fneClasses = "fne-link submission-link";
        if (Model.CourseInfo.CourseType == CourseType.FACEPLATE)
        {
            fneClasses += " faceplatefne ";
        }
       foreach (var submission in Model.Submissions)
       { %>
            <tr>
                <td class="attemptdate"><%= String.Format("{0} {1}", submission.DateSubmitted, ViewData["timeZone"].ToString()) %></td>
                <% if (Model.ShowGrade)
                   { %>
                <td class="attemptscore">
                    <% if (submission.Score != null)
                       {
                           // Truncate the submission score's decimal value to 2 places 
                           string percentageStr = "";
                           try
                           {
                               var percentage = submission.Grade.Possible > 0 ?
                                   (submission.Score.Value/submission.Grade.Possible)*100 :
                                   (submission.Grade.RawAchieved/submission.Grade.RawPossible)*100;

                               percentage = Math.Floor(percentage*100)/100;
                               percentageStr = String.Format("{0}%", percentage);
                           }
                           catch
                           {
                           }
                    %>
                        <%= percentageStr %>
                    <% }
                       else
                       {
                    %>Not graded<%
                       } %>
                </td>
                <% } %>
                <td class="attemptslink">
                <%= Html.ActionLink("View Submission", "SubmissionHistory", "Quiz", new { quizId = Model.Id, version = submission.Version }, new { @class = fneClasses })%>
            </tr>
    <% } %>
</table>