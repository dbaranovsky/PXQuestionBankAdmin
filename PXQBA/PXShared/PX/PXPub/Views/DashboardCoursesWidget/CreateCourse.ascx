<%@ control language="C#" inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>

<div id="cover">
    <div id="title-header">
        <h1>Create Course</h1>
    </div>
    <div id="error-message" class="error-message">
        This is a required field.
    </div>
    <label class="dashboard-labels" for="course-title">
        Course Title</label>
    <input type="text" class="dashboard-input inputStyle" name="course-title" id="course-title" />
    <div id="smaller-dashboard-inputs">
        <div class="stack">
            <label class="dashboard-labels dashboard-course-number-label" for="course-number">
                Course number <i class="optional-text">optional</i></label><br>
            <input type="text" class="dashboard-course-number-input  short-run inputStyle" name="course-number" id="course-number" />
        </div>
        <div class="stack">
            <label class="dashboard-labels dashboard-section-number-label" for="section-number">
                Section Number <i class="optional-text">optional</i></label><br>
            <input type="text" class="dashboard-section-number-input inputStyle" name="section-number" id="section-number" />
        </div>
    </div>
    <label class="dashboard-labels" for="instructor-name">
        Instructor name(s) <i class="optional-text">optional</i></label>
    <input type="text" class="dashboard-input inputStyle" name="instructor-name" id="instructor-name" />
    <label class="dashboard-labels" for="school-name">
        School</label>
    <div id="SelectedSchool">
        <input type="text" class="dashboard-input inputStyle" name="school-name" id="school-name" />
        <select id="UserSelectedSchool" class="disable"></select>
        <p id="FindSchoolError" class="dashboard-section-number-label error-message disable">The school you entered is not in our system</p>
        <a id="FindSchool" href="#" class="coursefindschool disable">Search schools by city/state/country or zip code?</a>
    </div>
    <div id="FindSchoolPopup">
        <% Html.RenderPartial("FindSchool", Model.Course); %>
    </div>
    <label class="dashboard-labels" for="academic-term">
        Academic term</label><br />

    <%= Html.DropDownListFor(model => Model.Course.AcademicTerm, new SelectList(Model.Course.PossibleAcademicTerms, "Id", "Name", Model.Course.AcademicTerm), new { @class = "comboStyle", @id = "academicTerm", @name = "academicTerm" })%>
    <br />
    <label class="dashboard-labels" for="time-zone">
        Time Zone</label>

    <%= Html.DropDownList("courseTimeZone", new SelectList((IEnumerable<TimeZoneInfo>)ViewData["TimeZones"], "Id", "DisplayName", TimeZoneInfo.Local.Id), new { @class = "course-dropdown comboStyle" })%>
    
    <div id="lmsDiv" class="dashboard-input-radio">
        <label class="dashboard-labels" for="lms-id">Student LMS ID Sync:</label>
        <input type="radio" id="LmsIdRequiredFalse" name="LmsIdRequired" value="false" />
        <span class="dashboard-radio">Do NOT Prompt students to provide their campus LMS ID</span><br />
        <input type="radio" id="LmsIdRequiredTrue" name="LmsIdRequired" value="true" />
        <span class="dashboard-radio">Prompt students to provide their campus LMS ID</span><br />
    </div>

    <input type="hidden" id="parent-course-id" value="" />
    <input type="hidden" id="school-list" value='<%= Model.SchoolList %>' />
    
</div>
