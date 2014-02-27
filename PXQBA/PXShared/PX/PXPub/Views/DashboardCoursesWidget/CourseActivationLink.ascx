<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>
 <% 
                  string classStatus = string.Empty;
                  string classStatusStyle = string.Empty;
                  
                  if (!string.IsNullOrEmpty(Model.Status) && Model.Status.ToLowerInvariant() == "open")
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
<p class="activate-button">
    <a class="<%=classStatusStyle %>" href="#" data-dw-id="<%= Model.CourseId %>">
        <%=classStatus %></a></p>
