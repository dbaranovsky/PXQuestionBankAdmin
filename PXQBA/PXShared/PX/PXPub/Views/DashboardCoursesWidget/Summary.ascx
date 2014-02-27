<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardData>" %>
<%
    string viewQuestionBankUrl = "";


    string sandboxCourseUrl = Url.Action("EditSandboxCourse", "Sandbox", null);

    string launchpadClass = "";
    string caseSwitch = "";
    string lastLevel = "0";
    bool lastChild = false;
    int index = 0;

    string fneClass = "fne-link";
    if (!Model.isMultipleDomains)
    {
        fneClass = "";
    }
    string visibility = "delete-button-invisible";

    var createCourseLink = Html.ActionLink("Create a New Course", "ShowCreateCourse", "Course", null, new { @class = string.Format("show-creation-button fixed {0}", fneClass), title = "" });

    DashboardItem nextDashboardItem = new DashboardItem();
    
    if (Model.LaunchPadMode == true)
    {
        launchpadClass = "launchpad";
        caseSwitch = "launchpad";
    }
%>

<%
    if (Model.LaunchPadMode == false)
    { 
%>

<div id="dashboard-header">
    <div id="widget-title">
        <h2>My Dashboard</h2>

    </div>
    <div id="create-course-regular">
        <%=createCourseLink%>
    </div>
</div>
<%} %>
<div class="<%=launchpadClass %>">
    <%
        if (Model.LaunchPadMode == true)
        { 
    %>
    <h2 class="my-courses">My Courses <span id="dashboard-logout">(<a id="logout-button" href="<%= Url.Action("Logout", "Account") %>"> Log Out </a>)
    </span></h2>
    <div id="create-course">
        <p>
            <a href="#" class="createcourseoption" id="createcourseoption">Create Course</a>
        </p>
    </div>
    <%
        }
        else
        {
    %>
    <h2 class="my-courses">My Courses</h2>
    <%
        } 
    %>
    <!-- Table goes in the document BODY -->
    <table class="dashboardgrid">
        <tbody>
            <%if (Model.LaunchPadMode == false)
              { %><tr>
                  <th>Course Name
                  </th>
                  <th class="header-titles">School
                  </th>
                  <th class="header-titles">Status
                  </th>
                  <th class="header-titles">Students Enrolled
                  </th>
              </tr>
            <%} %>
            <%
                var courses = Model.InstructorCourses;
                var baseId = "";

                if (!courses.IsNullOrEmpty())
                {
                    for (int i = 0; i < courses.Count; i++)
                    {
                        string rowClass = courses[i].RowClass;
                        string classStatus = string.Empty;
                        string classStatusStyle = string.Empty;
                        visibility = "delete-button-invisible";
                        if (!string.IsNullOrEmpty(courses[i].Status) && courses[i].Status.ToLowerInvariant() == "open")
                        {
                            courses[i].Status = "Open";
                            classStatus = "Deactivate";
                            classStatusStyle = "deactivate-dashboard-course";
                        }
                        else
                        {
                            courses[i].Status = "Closed";
                            classStatus = "Activate";
                            classStatusStyle = "activate-dashboard-course";
                        }
                        
                        var nextIndex = i + 1;
                        
                        if (courses[i].Level == "1")
                        {
                            if (nextIndex < courses.Count && courses[nextIndex].Level == "0")
                            {
                                rowClass = "child-course";
                            }
                            else if (nextIndex <= courses.Count)
                            {
                                rowClass = "child-course";
                            }
                        }
                        else if ((courses[i].Level == "0"))
                        {
                            baseId = courses[i].CourseId;                            
                        }
                        
                        if (Model.LaunchPadMode == false)
                        {
                            classStatus = string.Empty;
                        }
            %>

            <%
                var activatedDate = (Convert.ToDateTime(courses[i].Course.ActivatedDate) == DateTime.MinValue)? "" : courses[i].Course.ActivatedDate;   
            %>

            <tr class="entityidofcourse <%=rowClass %> <%= classStatus %>" data-dw-id="<%= courses[i].CourseId %>" 
                data-dw-course-section="<%=HttpUtility.HtmlEncode(courses[i].Course.SectionNumber) %>" 
                data-dw-course-number="<%=  HttpUtility.HtmlEncode(courses[i].Course.CourseNumber)%>"
                data-dw-start-date="<%= activatedDate %>" 
                data-dw-school-name="<%=courses[i].Course.SchoolName %>" 
                data-dw-academic-term="<%=courses[i].Course.AcademicTerm %>"
                data-dw-timezone="<%=courses[i].Course.CourseTimeZone %>"
                data-current-level="<%=courses[i].Level %>" 
                data-derived-id="<%= courses[i].Course.DerivedCourseId %>"
                data-base-id="<%= courses[i].Level == "0" ? string.Empty : baseId %>"
                data-dw-lms-id-required="<%=courses[i].Course.LmsIdRequired %>">
                <%
                if (Model.AllowCreateAnotherBranchColumn == true)
                { 
                %>
                <td class="link-icon"></td>
                <%} %>
                <td class="title-cell">
                    <div class="left">
                        <% if (Model.AllowCourseTitleColumn)
                           { %>
                                <% Html.RenderPartial("CourseTitleLink", courses[i], new ViewDataDictionary { { "index", Model.AllowCourseOpenInNewWindow } }); %>
                        <%} %>
                        <% if (Model.LaunchPadMode){ %>
                            <span href="<%= courses[i].CourseId %>" class="course-url">
                                <%: Url.RouteUrl("CourseSectionHome", new { courseId = courses[i].CourseId }, Request.Url.Scheme) %>
                            </span>
                        <%} %>
                        <div class="course-info">
                            <%if (Model.AllowInstructorNameColumn)
                              { %>
                                <p class="professor-name">
                                    <%= courses[i].Course.InstructorName%>
                                </p>
                            <%} %>
                            <%if (Model.AllowDomainNameColumn)
                              { %>
                                <p class="domain-name" data-dw-domain-id="<%= courses[i].DomainId %>">
                                    <%= courses[i].DomainName%>
                                </p>
                            <%} %>
                            <%if (Model.AllowAcademicTermColumn)
                              { %>
                                <p class="semester">
                                    <%
                                        string academicTermName = (Model.PossibleAcademicTerms.Exists(a => a.Id == courses[i].Course.AcademicTerm))
                                                                ? Model.PossibleAcademicTerms.Find(a => a.Id == courses[i].Course.AcademicTerm).Name
                                                                : courses[i].Course.AcademicTerm;
                                    %>
                                    <%= academicTermName %>
                                </p>
                            <%} %>
                            <%if (Model.AllowCourseIdColumn)
                              { %>
                                <p class="class-id">
                                    ID:
                                    <%= courses[i].CourseId%>
                                </p>
                            <%} %>
                        </div>
                        <div class="course-actions">
                            <%if (Model.AllowCreateAnotherBranchColumn && courses[i].Level != "1")
                              { %>
                                <p class="create-another-branch">
                                    <%if (Model.IsBranchCreated == true)
                                      { %>
                                        <a class="create-another-branch-link" href="#"><i class="create-branch-this-icon">&nbsp;
                                        </i>Create another branch</a>
                                    <%}
                                      else
                                      { %>
                                        <a class="create-another-branch-link" href="#"><i class="create-branch-this-icon">&nbsp;
                                        </i>Branch this course</a>
                                    <%} %>
                                </p>
                            <%} %>
                            <%if (Model.AllowActivateButtonColumn)
                              { %>
                                <p class="activate-button">
                                    <a class="<%=classStatusStyle %>" href="#" data-dw-id="<%= courses[i].CourseId %>">
                                        <%=classStatus%></a>
                                </p>
                            <%} %>
                            <%if (Model.LaunchPadMode && Model.AllowEditingCourseInformation)
                              { %>
                                <p class="edit-button">
                                    <a class="edit-dashboard-course" href="#" data-dw-id="<%= courses[i].CourseId %>">Edit</a>
                                </p>
                            <%} %>
                            <%if (Model.AllowDeleteButtonColumn)
                              {
                                  visibility = "delete-button-visible";
                              }                      
                            %>
                            <p class="delete-button <%=visibility %>">
                                <a class="delete-dashboard-course" href="#" data-dw-id="<%= courses[i].CourseId %>">Delete</a>
                            </p>
                        </div>
                    </div>
                    <a href="JavaScript:void(0);" class="show-url-hover">Show URL</a><br />
                    <span href="<%= courses[i].CourseId %>" class="show-url course-url">
                        <%: Url.RouteUrl("CourseSectionHome", new { courseId = courses[i].CourseId }, Request.Url.Scheme) %>
                    </span>
                </td>
                <td class="domain-cell">
                    <%if (Model.AllowDomainNameColumn)
                      { %>
                        <p class="domain-name" data-dw-domain-id="<%= courses[i].DomainId %>">
                            <%= courses[i].DomainName%>
                        </p>
                    <%} %>
                </td>
                <%if (Model.AllowStatusColumn)
                  { %>
                    <td class="status-cell">
                        <div class="right">
                            <%=courses[i].Status%>
                        </div>
                    </td>
                <%} %>
                <%if (Model.AllowEnrollmentCountColumn)
                  { %>
                    <td class="enrollment-count-cell">
                        <div class="right">
                            <%= courses[i].Count%>
                            students
                            <%if (Model.AllowEditingCourseInformation)
                              {
                                  string editClassName = courses[i].Count <= 0 ? "show-url-view-edit edit-course-link" : "show-url-view-edit edit-course-link s-en";
                            %>
                                <a href="JavaScript:void(0);" class="<%=editClassName %>">&nbsp;| Edit</a>
                            <%
                              }%>
                            <%if (Model.AllowDeleteButtonColumn && courses[i].Count <= 0)
                              { %>
                                <a href="JavaScript:void(0);" class="show-url-delete">Delete Course </a>
                                <br />
                            <%} %>
                            <%if (courses[i].Count > 0 && Model.AllowViewingRoster)
                              { %>
                                <a href="JavaScript:void(0);" class="show-url-view-roster">View Roster </a>
                                <br />
                            <%} %>
                        </div>
                    </td>
                <%} %>
            </tr>
            <%              
                    }
                }                
            %>
        </tbody>
    </table>
    <br />
    <%
    if (Model.QuestionAdminLink)
    {
      viewQuestionBankUrl = Url.Action("Index", "QuestionAdmin", null);
    %>
    <div id="question-admin-container">
        <a href="<%=viewQuestionBankUrl %>" id="question-admin-link">Question Bank Admin</a>
    </div>

    <%
    }

    %>
    <%
    if (Model.SandBoxLink)
    {
    %>

    <div id="question-admin-container">
        <a href="<%=sandboxCourseUrl %>" id="A1">Sandbox</a>
    </div>

    <%
    }
    %>
</div>

<div id="roster-information-dialog" title="Course Roster"></div>

<input type="hidden" value="<%=Model.IsBranchCreated %>" id="isBranchCreated" />

<% if (Model.LaunchPadMode == true)
{
    if (Model.InstructorCourses.Count > 0)
    {
        Model.InstructorCourses[0].SchoolList = Model.SchoolList;
        Model.InstructorCourses[0].Course.PossibleAcademicTerms = Model.PossibleAcademicTerms;
        Html.RenderPartial("DashboardDialogs", Model.InstructorCourses[0]);
    }
    else
    {
        Bfw.PX.PXPub.Models.DashboardItem fakeItem = new Bfw.PX.PXPub.Models.DashboardItem()
        {
            Course = new Bfw.PX.PXPub.Models.Course(),
            Type = new Bfw.PX.PXPub.Models.Course()
        };
        fakeItem.SchoolList = Model.SchoolList;
        fakeItem.Course.PossibleAcademicTerms = Model.PossibleAcademicTerms;
        Html.RenderPartial("DashboardDialogs", fakeItem);
    }
} %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/DashboardWidget/PxDashboard.js")%>'], function () {

                $(".dashboardgrid").PxDashboardWidget();
            });
        });

    }(jQuery));
</script>

<table>
    <tr class="entityidofcourse parent-course Activate template" data-dw-id=""
        data-dw-course-section="" data-dw-course-number="" data-dw-start-date=""
        data-dw-school-name="" data-dw-academic-term="" data-dw-timezone=""
        data-current-level="" data-derived-id="" data-dw-lms-id-required="" >
        <%
            if (Model.AllowCreateAnotherBranchColumn == true)
            { 
        %>
        <td class="link-icon"></td>
        <%} %>
        <td class="title-cell">
            <div class="left">
                <% if (Model.AllowCourseTitleColumn)
                           { %>
                <% Html.RenderPartial("CourseTitleLink", null, new ViewDataDictionary { {"index", Model.AllowCourseOpenInNewWindow}}); %>
                <%} %>
                <% if (Model.LaunchPadMode){ %>
                    <span class="course-url"></span>
                <%} %>
                <div class="course-info">
                    <%if (Model.AllowInstructorNameColumn)
                    { %>
                        <p class="professor-name" />
                    <%} %>
                    <%if (Model.AllowDomainNameColumn)
                    { %>
                        <p class="domain-name" data-dw-domain-id=""/>
                    <%} %>
                    <%if (Model.AllowAcademicTermColumn)
                    { %>
                        <p class="semester"/>
                    <%} %>
                    <%if (Model.AllowCourseIdColumn)
                    { %>
                        <p class="class-id"/>
                        ID:                               
                    <%} %>
                </div>
                <div class="course-actions">
                    <%if (Model.AllowCreateAnotherBranchColumn)
                    { %>
                        <p class="create-another-branch">
                            <%if (Model.IsBranchCreated == true)
                            { %>
                                <a class="create-another-branch-link" href="#"><i class="create-branch-this-icon">&nbsp;
                                </i>Create another branch</a>
                            <% }
                            else
                            { %>
                                <a class="create-another-branch-link" href="#"><i class="create-branch-this-icon">&nbsp;
                                </i>Branch this course</a>
                            <%} %>
                        </p>
                    <%} %>
                    <%if (Model.AllowActivateButtonColumn)
                    { %>
                        <p class="activate-button">
                            <a class="activate-dashboard-course" href="#" data-dw-id="">Activate</a>
                        </p>
                    <%} %>
                    <%if (Model.LaunchPadMode && Model.AllowEditingCourseInformation)
                     { %>
                        <p class="edit-button">
                            <a class="edit-dashboard-course" href="#" data-dw-id="">Edit</a>
                        </p>
                    <%} %>
                    <% if (Model.AllowDeleteButtonColumn){ %>
                    <p class="delete-button">
                        <a class="delete-dashboard-course" href="#" data-dw-id="">Delete</a>
                    </p>
                    <%} %>
                </div>
            </div>
            <a href="JavaScript:void(0);" class="show-url-hover">Show URL</a><br />
            <span href="" class="show-url course-url"><%= ViewData["CourseDomainUrl"] %></span>
        </td>
        <td class="domain-cell">
            <%if (Model.AllowDomainNameColumn)
             { %>
                <p class="domain-name" data-dw-domain-id="">
                </p>
            <%} %>
        </td>
        <%if (Model.AllowStatusColumn)
         { %>
        <td class="status-cell">
            <div class="right">
            </div>
        </td>
        <%} %>
        <%if (Model.AllowEnrollmentCountColumn)
        { %>
            <td class="enrollment-count-cell">
                <div class="right">
                    students
                    <%if (Model.AllowEditingCourseInformation)
                    {
                        string editClassName = courses.Count <= 0 ? "show-url-view-edit edit-course-link" : "show-url-view-edit edit-course-link s-en";
                    %>
                        <a href="JavaScript:void(0);" class="<%=editClassName %>">&nbsp;| Edit</a>
                    <%
                      }%>
                    <%if (Model.InstructorCourses.Count > 0 && Model.AllowDeleteButtonColumn)
                    { %>
                        <a href="JavaScript:void(0);" class="show-url-delete">Delete Course </a>
                        <br />
                    <%} %>
                    <%if (Model.InstructorCourses.Count > 0 && Model.AllowViewingRoster)
                     { %>
                        <a href="JavaScript:void(0);" class="show-url-view-roster">View Roster </a>
                        <br />
                    <%} %>
                </div>
            </td>
        <%} %>
    </tr>
</table>
