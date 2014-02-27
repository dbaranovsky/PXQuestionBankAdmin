<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AboutCourse>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<div class="course-info-wrapper">

    <% if (Model.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor &&  Model.ShowEditLink) {%>
    <div class="editlinks">        
        <a href="javascript:" class="widgetEditLink">
            Edit
        </a>
    </div>
    <% } %>

<div class="datafields">
    <span class="display-field">
    <% 
        if (Model != null && Model.ShowCourseTitle) {
            var courseNumberSection = string.Empty;

            courseNumberSection += Model.CourseNumber;

            if (courseNumberSection.Length > 0 && Model.SectionNumber.Length > 0)
            {
                courseNumberSection += String.Format(", {0}", Model.SectionNumber);
            }
            else 
            {
                courseNumberSection += Model.SectionNumber;
            }
    %>

    <div class="coursetitle"><%= Html.DisplayFor(model => model.CourseTitle) %></div>
    <span class="display-label"><%= courseNumberSection %></span><br />

    <% } %>

    </span>

    <span class="display-field name">
        <% if ( Model != null )
           { %>
           <%                              
            if (Model.ShowInstructorName) {%> 
               <span>
                    <%:Html.DisplayFor(model => Model.InstructorName)%>
                    <br />
                </span>
            <% } %>
        <% } %>
    </span>
    <% if (Model.LmsIdEnabled && Model.AccessLevel == AccessLevel.Student)
       { %>
    <span class="display-field lms-id">
        <span class="lms-id-hide">
            <a href="javascript:" class="lms-action-show">Show Campus LMS ID</a>
        </span>
        <span class="lms-id-show">
            <label>Campus LMS ID:</label>
            <span class="lms-id-label"><%= Model.CampusLmsId %></span>
            <a href="javascript:" class="lms-action-hide">hide</a> | 
            <a href="javascript:" class="lms-action-edit">edit</a>
        </span>
        <span class="lms-id-edit">
            <label>Campus LMS ID:</label>
            <input type="text" id="txtLmsId" />
            <a href="javascript:" class="lms-action-save">save</a> |
            <a href="javascript:" class="lms-action-cancel">cancel</a>
        </span>
    </span>
    <% } %>
    <span class="display-field hours">
        <% 
            if ( Model != null )
           { %>
           <% 
            if (Model.ShowOfficeHours && !Model.OfficeHours.IsNullOrEmpty()) {%>
            <span class="display-label"><em>Office Hours: </em></span>
        <%:
        Html.DisplayFor(model => Model.OfficeHours, new {@class = "InputForControllerAction"}) %><br />
        <% } %>
        <% } %>
    </span>

    <% if ( Model != null) { %>	 	
        <% 
           foreach ( AboutCourseContactInfo c in Model.ContactInfo) {
               if (c.Info.IsNullOrEmpty()) continue;               
               %>
            <em><%= c.Type%>: </em>&nbsp;<span><%= c.Info %></span><br />
        <% }
    }%>

    <span class="display-field syllabus">
    <% if ( Model != null ) { %>
        <% 
            if (Model.ShowSyllabusUrl && !string.IsNullOrEmpty(Model.SyllabusUrl)) {%> 
        
    <span>
         <a href="<%= Model.SyllabusUrl %>" target="_blank"><%= Model.SyllabusDisplayText %></a>
   </span>
        <% }
     }%>
    </span>
</div>

</div>