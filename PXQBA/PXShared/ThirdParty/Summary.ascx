<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardData>" %>
<div id="dashboard-header">
    <div id="widget-title">
        <h2>
            My Dashboard</h2>
    </div>
    <div id="create-course">
                <%
           
                    string fneClass = "fne-link";
                    if (!Model.isMultipleDomains)
                    {
                        fneClass = "";
                    }

                    var lnkHtml = Html.ActionLink("Create a New Course", "ShowCreateCourse", "Course", null, new { @class = string.Format("show-creation-button fixed {0}", fneClass), title = "" });
            %> 
            

        <%=lnkHtml %></div>
</div>
<h2 class="my-courses">
    My Courses</h2>
<!-- Table goes in the document BODY -->
<table class="dashboardgrid">
    <tbody>

        <tr>
            <th>
                Course Name
            </th>
             <th class="header-titles">
                School
            </th>
            <th class="header-titles">
                Status
            </th>
            <th class="header-titles">
                Students Enrolled
            </th>
        </tr>
        <%foreach (DashboardItem i in Model.InstructorCourses)
          { %>

         
        <tr class="entityidofcourse" data-dw-id="<%=i.CourseId %>" >
            <td class="title-cell">
               <a href="<%= Model.CourseUrl %><%=i.CourseId %>" target="_blank"><%= i.CourseTitle %></a>
               
                <a href="JavaScript:void(0);" class="show-url-hover" >Show URL</a><br />
                <span href="<%= Model.CourseUrl %><%=i.CourseId %>" class="show-url course-url" ><%= Model.CourseUrl %><%=i.CourseId %></span>
            </td>
              <td class="domain-cell">
              <%= i.DomainName %>
              
            </td>
            <td class="status-cell">
               <%= i.Status %>
            </td>
            
            <td class="enrollment-count-cell">
           
                <%=i.Count %>
                <%if (i.Count <= 0)
                  { %>
                <a href="JavaScript:void(0);" class="show-url-delete">Delete Course</a><br />
                <%} %>
            </td>
           
        </tr>
        <%} %>
             
    </tbody>
</table>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/DashboardWidget/PxDashboard.js")%>'], function () {

                $(".dashboardgrid").PxDashboardWidget();
            });
        });

    } (jQuery));    
                   
</script>
