<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.EcomerceJoinCourse>" %>
<%
    var courseType = "";

    switch (Model.course.CourseSectionType.ToUpper())
    {
        case "XBOOK":
            courseType = "X-book";
            break;
        case "LAUNCHPAD":
            courseType = "LaunchPad";
            break;
        case "LEARNINGCURVE":
            courseType = "LearningCurve";
            break;
    }
%>

<div class="course-item">
<%
    var enrollmentId = Model.EnrollmentID;
    var lmsStudentId = ViewData["lmsStudentId"] == null ? string.Empty : ViewData["lmsStudentId"].ToString();
     if (string.IsNullOrEmpty(enrollmentId) || string.IsNullOrEmpty(lmsStudentId))
    {
%>
    <div id="greeting">
        OK <%= Model.CurrentUser %>
    </div>
    <% //temporary code for launchpad
        if (Model.University == "PxGeneric" && courseType == "LaunchPad")
       { %>
        <h2 class="course-greeting-header">
            Are you sure you want to access <%= courseType %> resources without joining a course? 
        </h2>
    <% }
       else
       {
           if (Model.EnrollmentStatus == Bfw.Agilix.DataContracts.EnrollmentStatus.Withdrawn)
           {
    %>
                <div class="unenrolledText">
                    <img src="<%= Url.Action("Index", "Style", new { path="images/info_icon.png" }) %>" />
                    You were unenrolled from the course <%= String.Format("{0} ({1}) at {2}.", Model.course.Title, Model.Instructor, Model.University) %>
                    Click <b>Join Course</b> below to re-enroll or select a different course.
                </div>            
    <%
           }
    %>
        <h2 class="course-greeting-header">
            Please confirm this is the course you wish to join:
        </h2>
    <% } %>
    <div class="description">
    </div>
    <% using (Html.BeginForm("EnrollmentConfirmation", "Ecommerce"))
    {
    %>
    <%= Html.Hidden("courses", Model.course.Id)%>
    <%= Html.Hidden("domain", Model.Domain )%>
    <%= Html.Hidden("instructors", Model.Instructor)%>
    <%= Html.Hidden("switchEnrollFromCourse", Model.SwitchEnrollFromCourse) %>
    <% Html.RenderPartial("GenericCourse", Model);%>
    <% //temporary code for LaunchPad
       var btnText = "Join Course";
       var linkText = " Join a different course.";
       var linkPreText = "If this is not the course you wish,";

       if (Model.University == "PxGeneric" && courseType == "LaunchPad")
       { 
            btnText = "Access LaunchPad";
            linkText = " CLICK HERE";
            linkPreText= "To find a course to join,";
       } 
    %>
    <input id="join-course-confirmation" type="submit" value="<%= btnText %>" class="course-continue" />
    <span class="wrong-link"> <%= linkPreText %>
        <%= Html.ActionLink(linkText, "Enroll", new {courseid = Model.course.Id}, new {id = "wrong-course-link"}) %>
    </span>
    <% }
    }
    else //already enrolled in this class
       { %>
    <div>
        You are already enrolled in this course.
    </div>
    <div class="enroll-continue" style="padding-top: 20px;">
        <%= Html.ActionLink("Take me to my course", actionName: "", controllerName: "", routeValues: new { courseid = Model.course.Id }, htmlAttributes: new { Class = "course-continue linkButton" })%>
    </div>
    <% } %>
</div>
