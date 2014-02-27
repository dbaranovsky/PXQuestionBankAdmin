<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.EcomerceJoinCourse>" %>

<div id="GeneralCourseDetails">

   <table class="course-info">
                <tr>
                    <td class="first-col">
                        <%= Html.Label("SCHOOL:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.TextBox("University",Model.University, new { size = 50, disabled = "disabled", @class = "course_product-name" })%>
                    </td>
                </tr>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("ACADEMIC TERM:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.AcademicTerm)%>
                    </td>
                </tr>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("INSTRUCTOR:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.Instructor)%>
                    </td>
                </tr>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE TITLE:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.course.CourseProductName)%>
                    </td>
                </tr>
                <% if (Model.ShowCourseNumber)
                   { %>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE NUMBER:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.course.CourseNumber)%>
                    </td>
                </tr>
                <% } %>
                <% if (Model.ShowCourseSection)
                   {  %>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE SECTION:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.course.SectionNumber)%>
                    </td>
                </tr>
                <%} %>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE START DATE:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.course.ActivatedDate)%>
                    </td>
                </tr>
            </table>
</div>
