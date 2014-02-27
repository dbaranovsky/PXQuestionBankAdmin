<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.StudentGradebook>" %>
<!-- View used when an instructor clicks on a student in the new gradebook -->
<% 
    var student = Model.Student;
    var assignments = Model.Assignments;
    var studentLastLogin = student.LastLogin.HasValue ? student.LastLogin.Value.ToString("MM/dd/yyyy - hh:mm tt") : string.Empty;

%>
<div id="studentinfo">
<div class="leftcol">
    <h1 class="title">Student Detail</h1>
    <div class="content">
        <table class="studentdetails">
            <tbody>
                <tr>
                    <td class="name">Name</td>
                    <td class="nameval"><%= student.FormattedName %></td>
                </tr>
                <tr>
                    <td class="email">Email Address</td>
                    <td class="emailval"> <%= student.Email %> </td>
                </tr>
                <tr>
                    <td class="studentAvg">Average</td>
                    <td class="studentAvgVal"><%= (Math.Round((Model.AssignedAverage + Model.UnAssignedAverage) / 2, 2) * 100D) + "%" %></td>
                </tr>
                <tr>
                    <td class="studentLogin">Last Login</td>
                    <td class="studentLoginVal"><%= studentLastLogin %></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>

<div class="rightcol">
<div id="studentassignments">
    <h1 class="title">Assignment Summary</h1>
    <div class="content">
        <% Html.RenderPartial("StudentGradebook", Model); %>
    </div>
</div>

</div>
</div>