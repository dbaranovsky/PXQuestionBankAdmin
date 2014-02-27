<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="CreateCourseOption" class="create-course-option">
    <div id="CreateDashBoardCourseOptionHeader" class="courseoptionheader">
        <h1>
            Create Course</h1>
    </div>
    <p>
        Do you wish to base the course on an existing course?</p>
    <ul class="courseoption-collection">
        <li class="courseoption-list">
            <input type="radio" id="courseoptionyes" name="courseoption" value="yes" />Yes</li>
        <li class="courseoption-list">
            <input type="radio" id="courseoptionno" name="courseoption" value="no" checked="checked" />No</li>
    </ul>
</div>
