<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<script type="text/css">
.student-started-dashboard { color:Red; padding-bottom:10px; }
.studentemail-dashboard { font-family: Calibri, Sans-Serif, Arial;padding-top:15px; }
ol.studentemail-instruction-dashboard { list-style: decimal inside !important; }
ol.studentemail-instruction-dashboard li { overflow: "" !important }
ul.contact-details { margin-left:100px; padding:10px; list-style: circle inside !important; }
.contact-details-left { text-align:right; padding-right:10px; }
.contact-details-right { padding-left:10px; margin-left:30px; }
.contact-time { font-size: 11.0pt; font-family: Calibri, sans-serif; width: 350px; }
.block-space {margin: 0 0 0 0; border: none; padding: 0;}
table { padding: 0; border-spacing: 0; border-collapse: collapse; }
tr td, table tr th { border: 0; }
#createcourse .course-item li {overflow: visible !important;}
</script>
<% 
    var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, "http" );
    link = (!string.IsNullOrEmpty(link)) ? link.Replace ( ":" + Request.Url.Port, "" ): string.Empty;
        
    var school = (ViewData["School"] == null || string.IsNullOrEmpty(ViewData["School"].ToString())) ? string.Empty : ViewData["School"].ToString();
%>

<div class="creation-info-text active">
   Your course for <%=Model.CourseProductName %> has been activated and is ready for you and your students. Your unique course URL is below, along with language you can use to invite students to join (and cut down on the chances of students using you for technical support).  
</div>

<div class="course-item">

    <table class="course-info">
        <tr>
             <td class="first-col">Your course URL:</td>
             <td class="second-col"><%=link %></td>
        </tr>
        <tr>
             <td class="first-col">Instructor:</td>
             <td class="second-col"><%=Model.InstructorName %></td>
        </tr>
        <%--<tr>
             <td class="first-col">State:</td>
             <td class="second-col">&nbsp;</td>
        </tr>--%>
        <tr>
             <td class="first-col">School:</td>
             <td class="second-col"><%= school %></td>
        </tr>
        <tr>
             <td class="first-col">Course Title:</td>
             <td class="second-col"><%=Model.CourseProductName %></td>
        </tr>
        <tr>
            <td class="first-col">Course Number:</td>
            <td class="second-col"><%=Model.CourseNumber %></td>
        </tr>
        <tr>
            <td class="first-col">Course Section:</td>
            <td class="second-col"><%=Model.SectionNumber %></td>
        </tr>
        <tr> 
            <td class="first-col">Course Start Date:</td>
            <td class="second-col"><%=Model.ActivatedDate %></td>
        </tr>
    </table>
    <% Html.RenderPartial("StudentInstructions", Model, ViewData); %>
</div>
