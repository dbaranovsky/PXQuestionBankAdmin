<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% 
    System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            
    var title = ViewData["Title"].ToString();
    var enrollments = (IEnumerable<Bfw.PX.Biz.DataContracts.UserInfo>) ViewData["Enrollments"];
    var studentList = new SelectList(enrollments, "EnrollmentIdForCurrentCourse", "FormattedName");
    var dueDate = ViewData["DueDate"].ToString();
    var currentGrade = ((List<Bfw.PX.Biz.DataContracts.Grade>)ViewData["Grade"]).FirstOrDefault();
    var gradeStatus = ViewData["GradeStatus"].ToString();
    var isInstructor = (bool) ViewData["IsInstructor"];
    var lookup = jss.Serialize(ViewData["Lookup"]);

    var imgSaved = Url.Content("~/Content/images/assignment_not_submitted.png");
    var imgSubmitted = Url.Content("~/Content/images/assignment_submitted.png");
    var imgGraded = Url.Content("~/Content/images/assignment_graded.png");

    
%>

<div class="gradingHeader">
    <div id="studentList">
        <span class="studentLabel">Student:</span>
        <% if (isInstructor)
           { %>
              <select id="studentGradingList" class="studentGradingList">
              <% var statusImage = string.Empty;
                  foreach (var enrollment in enrollments)
                 {
                     switch (enrollment.SubmissionStatus)
                     {
                         case Bfw.PX.Biz.DataContracts.SubmissionStatus.Saved:
                             statusImage = imgSaved;
                             break;
                         case Bfw.PX.Biz.DataContracts.SubmissionStatus.NotGraded: //this is the status when the student has submitted but the instructor has not graded it yet.
                             statusImage = imgSubmitted;
                             break;
                         case Bfw.PX.Biz.DataContracts.SubmissionStatus.Graded:
                             statusImage = imgGraded;
                             break;
                         default:
                             statusImage = string.Empty;
                             break;
                     }
                      %>
                <option class="<%= enrollment.SubmissionStatus %>" data-image="<%= statusImage %>" value="<%= enrollment.Id %>"> <%= enrollment.FormattedName %> </option>
                <% } %>
              </select>
         <% }
           else
           {
               if (gradeStatus == "NeedsGrading")
               {  %>
                   Not Graded Yet 
               <%}
               else if(gradeStatus=="Graded")
               { %>
                   Graded <%= currentGrade.SubmittedDate.Value.ToShortDateString()%> 
               <%} %>
        <% } %>
    </div>
    <div class="studentGrading">
        <div class="gradeWrapper" >
            <% if (isInstructor)
               {%>
                <% if (gradeStatus == "None")
                   { %>
                   Not Yet Submitted
                <% } %>
                  <% else
                   { %>
                        <span class="gradeLabel">Grade</span>

                            <%= Html.TextBox("grade", currentGrade.RawAchieved, new { @class = "gradeTxt", style = "width:35px" })%>
                        <span class="possiblescoreLabel">of <%= currentGrade.RawPossible%></span>
                        <% if (gradeStatus == "Graded")
                           { %>
                                <input class="unsubmitButton" type="button" value="Unsubmit" />
                        <% }
                           else
                           { %>
                                <input class="savebutton" type="button" value="Save" />
                                <input class="submitbutton" type="button" value="Submit" />
                        <% } %>
                <% } %>
           <% } %>
        </div>
        <div class="duedateWrapper">
            <span class="duedateLabel">Due Date:</span>
            <span class="duedateValue"><%= dueDate %></span>
        </div>
    </div>
</div>

<link rel="stylesheet" type="text/css" href="<%= Url.Content("~/Content/jquery.ui/jquery-dd.css") %>" />
<script type="text/javascript">
    (function ($) {
        PxPage.Require(['<%= Url.ContentCache("~/Scripts/GradingScreen/GradingScreen.js") %>', '<%= Url.ContentCache("~/Scripts/jquery/jquery.autocomplete-combobox.js") %>', '<%= Url.Content("~/Scripts/jquery/jquery.dd.min.js") %>'], function () {
            jQuery(document).ready(function () {
                $("#fne-title").text("<%= title %>");
                PxGradingScreen.setLookup(<%= lookup %>);
                PxGradingScreen.initdropdown();
            });
        });
    } (jQuery));  
</script>