<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>"  %>
<% 
        var activateCourse = ViewData["activateCourse"] == null || (bool)ViewData["activateCourse"];
       var header = !activateCourse ? "Edit Your Course Information" : "Activate your course.";
       var selectedDomain = !activateCourse && !Model.DerivativeDomainId.IsNullOrEmpty() ? Model.DerivativeDomainId : null;
       string behavior = !activateCourse ? "edit" : null;
    %>
    <div id="createcourse">
        <%= Html.Hidden("courseFneTitle", "Create your course for " + Model.ProductName)%>
       <script type="text/javascript">
           (function($) {
               PxPage.OnReady(function () { PxPage.Require(['<%= Url.Content("~/Scripts/CreateCourse/CreateCourse.js") %>'], function() {
                   CreateCourse.Init('<%=header%>', '<%=selectedDomain%>', '<%= Url.ContentCache("~/Scripts/Common/") %>');
               }); });
           }(jQuery));

       </script>

   <% using (Ajax.BeginForm("UpdateCourse", "Course", new { behavoir = behavior }, new AjaxOptions() { UpdateTargetId = "createcourse", OnSuccess = "" }))
       {%>
        <%= Html.ValidationSummary(true) %>
        <%if (activateCourse)
          { %>
            <div class="creation-info-text">
                Please enter the information below to activate your course.
            </div>
        <%} %>
        <%= Html.HiddenFor(model => model.Id) %>
        <%= Html.HiddenFor(model => model.ProductName) %>        
        <%= Html.HiddenFor(model => model.CourseType) %>
        <%= Html.HiddenFor(model => model.SelectedDerivativeDomain) %>
             
        <ol class="course-item">
            <li>
                <div class="left-col"><%= Html.LabelFor(model => Model.AcademicTerm, "SCHOOL:") %></div>
                <div id="SelectedSchool" class=right-col><%= Html.DropDownListFor ( model => model.PossibleDomains, new SelectList( Model.PossibleDomains, "Id", "Name"))  %><a id="FindSchool" href="#" class="coursefindschool">Find School</a></div>
                <div id="FindSchoolPopup" class="right-col"><% Html.RenderPartial("FindSchool"); %></div>
            </li>

            <li>
                <div class=left-col><%= Html.LabelFor(model => Model.AcademicTerm, "ACADEMIC TERM:") %></div>
                <div class=right-col><%= Html.DropDownListFor ( model => model.AcademicTerm, new SelectList( Model.PossibleAcademicTerms, "Id", "Name", Model.AcademicTerm))  %></div>
            </li>
            
            <li>
                <div class=left-col><%= Html.LabelFor(m => m.CourseUserName)%></div>
                <div class=right-col><%= Html.TextBoxFor(model => model.CourseUserName, new { size = 50 })%>
                <p>Enter the instructor name or names, as it/they appear to students when they register for your course.  For example, if the course is being taught by 
                Professors William and Smith, you may enter "Williams/Smith".</p>
                <%= Html.ValidationMessageFor(model => model.CourseUserName)%></div>
            </li>
            <li>
                <div class=left-col><%= Html.LabelFor(m=>m.Title) %></div>
                <div class=right-col><%= Html.TextBoxFor(model => model.Title, new { @class="create-course-title", size = 50 })%>
                <p>We recommend that you enter the course name as it appears in your school's course catalog, e.g. "Introduction to Psychology".</p>
                <%= Html.ValidationMessageFor(model => model.Title)%></div>
            </li>
            <li>
               <div class=left-col> <%= Html.LabelFor(m => m.CourseNumber) %></div>
                <div class=right-col><%= Html.TextBoxFor(model => model.CourseNumber, new { @class="create-course-number", size = 50 }) %>
                <p>We recommend that you enter the course number as it appears in your school's course catalog, e.g. "PSY101". If your school does not
                use course numbers, leave the field blank.</p></div>
            </li>
            <li>
                <div class=left-col><%= Html.LabelFor(m => m.SectionNumber) %></div>
                <div class=right-col><%= Html.TextBoxFor(model => model.SectionNumber, new {@class="create-section-number", size=50}) %>
                 <p>We recommend that you enter the section number as it appears in your school's course catalog, e.g. "001". If your school does not
                use section numbers, leave the field blank.</p></div>
            </li>

            <li>
                <div class=left-col><%= Html.LabelFor(m => m.CourseTimeZone)%></div>
                <div class=right-col><%= 
                Html.DropDownListFor( model => model.CourseTimeZone, new SelectList( TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName", TimeZoneInfo.Local.StandardName ) )%>
                <p>The time zone used when displaying time information in your course.</p>
                <%= Html.ValidationMessageFor(model => model.CourseTimeZone)%></div>
            </li>
            <li>
                <div class=left-col><%= Html.LabelFor(m => m.CourseProductName)%></div>
                <div class=right-col><%= Html.TextBoxFor(model => model.CourseProductName, new { size = 50, disabled = "disabled", @class="course_product-name" })%>
                <p>This is the course name that will be displayed to your student</p>
                <%= Html.ValidationMessageFor(model => model.CourseProductName)%></div>
            </li>
        </ol>
        <div class="activate-btns">
                <input class="activate-submit" type="submit" value="Submit" onclick="PxPage.OnFormSubmit('Creating Course')" />
                <input class="activate-cancel" type="submit" value="Cancel" onclick="window.location ='';return false;" />
                <span>Clicking Submit will initiate the process of creating your course.</span>
        </div>
    <% } %>    
  </div>