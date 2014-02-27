<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardData>" %>
<% 
    string classStatus = string.Empty;
    string classStatusStyle = string.Empty;

    if (Model.Status.ToLowerInvariant() == "open")
    {
        Model.Status = "Active";
        classStatus = "Deactivate";
        classStatusStyle = "deactivate-dashboard-course";
    }
    else
    {
        Model.Status = "Inactive";
        classStatus = "Activate";
        classStatusStyle = "activate-dashboard-course";
    }

    %>
   
    <td class="title-cell"> 
        <div class="left">
        <% if (Model.AllowCourseTitleColumn)
           { %>
            <a href="<%= i.CourseUrl %><%= i.CourseId %>" class="course-title">
                <%= i.CourseTitle%></a>

                <%} %>


            <div class="course-info">

            <%if(Model.AllowInstructorNameColumn){ %>
                <p class="professor-name">
                    <%= i.Course.InstructorName  %>
                    </p>

                    <%} %>

                    <%if(Model.AllowDomainNameColumn){ %>
                <p class="domain-name" data-dw-domain-id="<%= i.DomainId %>">
                    <%= i.DomainName%></p>

                    <%} %>

                    <%if(Model.AllowAcademicTermColumn){ %>
                <p class="semester">
                    <%= i.Course.AcademicTerm %></p>

                    <%} %>

                    <%if(Model.AllowCourseIdColumn){ %>
                <p class="class-id">
                    ID:
                    <%= i.CourseId %></p>

                    <%} %>
            </div>

            <div class="course-actions">
                   <%if (Model.AllowCreateAnotherBranchColumn && i.Level != "1")
                  { %>
                <p class="create-another-branch">

         <%if (Model.IsBranchCreated == true)
           { %>

         <a class="create-another-branch-link" href="#" >Create another branch</a>

         <%}
           else
           { %>

                   <a class="create-another-branch-link" href="#" >Branch this course</a>

                   
                   <%} %>

                   </p>
                   <%} %>


                   <%if(Model.AllowActivateButtonColumn){ %>
                <p class="activate-button">
                    <a class="<%=classStatusStyle %>" href="#" data-dw-id="<%= i.CourseId %>"><%=classStatus %></a>
                    <%--<%= activateDeactivateLink%>--%></p>

                    <%} %>

                    <%if(Model.AllowDeleteButtonColumn){ %>
                <p class="delete-button">
                    <a class="delete-dashboard-course" href="#" data-dw-id="<%= i.CourseId %>">Delete</a>
                    </p>

                    <%} %>
            </div>
        </div>
    </td>    
    <%if(Model.AllowStatusColumn){ %>
    <td class="status-cell">

        <div class="right">
            <%=i.Status %>
        </div>
       
    </td>
     <%} %>

        <%if(Model.AllowEnrollmentCountColumn){ %>
            <td class="enrollment-count-cell">
        <div class="right">
            <%= i.Count %>
            students
        </div>
    </td>
       <%} %>
