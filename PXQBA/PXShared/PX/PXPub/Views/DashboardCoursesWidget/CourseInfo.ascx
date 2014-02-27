<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<ol class="course-info" dw-courseid="">
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.Title) %>
        </div>
        <div class="right-col">
            <label for="title" class="title">
                <%= Model.Title %></label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.CourseNumber) %>
        </div>
        <div class="right-col">
            <label for="coursenumber" class="coursenumber">
                <%= Model.CourseNumber %></label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.SectionNumber) %>
        </div>
        <div class="right-col">
            <label for="sectionnumber" class="sectionnumber">
                <%= Model.SectionNumber %></label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.SchoolName) %>
        </div>
        <div class="right-col">
            <label for="schoolname" class="schoolname">
                <%= Model.SchoolName %></label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.AcademicTerm) %>
        </div>
        <div class="right-col">
            <label for="academicterm" class="academicterm">
                <%= Model.AcademicTerm %></label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.ActivatedDate) %>
        </div>
        <div class="right-col">
            <%
                var activatedDate = (Convert.ToDateTime(Model.ActivatedDate) == DateTime.MinValue)? "" : Model.ActivatedDate;   
            %>
            <label for="activateddate" class="activateddate">
                <%= activatedDate %>
            </label>
        </div>
    </li>
    <li>
        <div class="left-col">
            <%: Html.LabelFor(m => m.CourseTimeZone) %>
        </div>
        <div class="right-col">
            <label for="coursetimezone" class="coursetimezone">
                <%= Model.CourseTimeZone %></label>
        </div>
    </li>
</ol>