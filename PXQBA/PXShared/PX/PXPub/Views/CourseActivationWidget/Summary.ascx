<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="display-container">
    <div id="activate-dashboard-course">
        <%  Html.RenderPartial("~/Views/DashboardCoursesWidget/ActivateDashboardCourse.ascx", Model);%>
    </div>
</div>
<div class="homepageheader hideHeader">
    <table>
        <%
            var activatedDate = (Convert.ToDateTime(Model.ActivatedDate) == DateTime.MinValue)? "" : Model.ActivatedDate;   
        %>
        <tr data-dw-id="<%= Model.Id %>" 
            data-dw-course-section="<%= Model.SectionNumber %>"
            data-dw-course-number="<%= Model.CourseNumber %>" 
            data-dw-start-date="<%= activatedDate%>"
            data-dw-school-name="<%= Model.SchoolName %>" 
            data-dw-academic-term="<%= Model.AcademicTerm %>"
            data-dw-timezone="<%= Model.CourseTimeZone %>"
            data-dw-lms-id-required="<%= Model.LmsIdRequired %>">
            <td>
                <p class="course-title disable">
                    <%= Model.Title %></p>
                <div class="course-actions">
                    <a id="lnkActivateCourse" name="lnkActivateCourse" data-dw-id="<%= Model.Id %>" class="creation-button activate-button activation-button activate-from-course-page">
                        <div class="create-instructions">
                            <span class="acticon pxicon pxicon-warningsign"></span>
                            <p>
                                Click here to <b>activate your course</b> when you're ready to distribute the course
                                URL to your students.</p>
                        </div>
                    </a>
                </div>
                <div style="display:none">
                    <%= Html.DropDownList("academicTerm", new SelectList((IEnumerable<CourseAcademicTerm>)ViewData["PossibleAcademicTerms"], "Id", "Name"), new { @class = "course-dropdown comboStyle" })%>
                </div>  
                <div style="display:none">
                    <%= Html.DropDownList("courseTimeZone", new SelectList((IEnumerable<TimeZoneInfo>)ViewData["TimeZones"], "Id", "DisplayName", TimeZoneInfo.Local.Id), new { @class = "course-dropdown comboStyle" })%>
                </div>                
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/DashboardWidget/PxDashboard.js")%>'], function () {

                $(".homepageheader").PxDashboardWidget();

            });
        });
    } (jQuery));
</script>
