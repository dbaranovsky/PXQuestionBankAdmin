<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ProfileSummaryWidget>" %>
<%@ Import Namespace="System.Globalization" %>
<% Random rand = new Random(); %>

<% 
   var linkDash = Url.RouteUrl("CourseSectionHome", new { courseid = ViewData["DashboardID"] });
   var isReadOnly = ViewData["IsReadOnly"] != null ? Convert.ToBoolean(ViewData["IsReadOnly"]) : false;
   %>

<div class="widgetBody">
    <% if (Model.UserProfile != null)
       { %>
    <%--instructor--%>
    <div class="instructor-wrapper">
    <ul class="register-instructor-list">
        <li class="instructor-profile">
            <% if (!Model.UserProfile.AvatarUrl.IsNullOrEmpty())
               {%>
            <div class="instructor-image">
                    <img src="<%= Model.UserProfile.AvatarUrl %>?<%= rand.Next() %>" alt="Image" class="avatar-image" />
            </div>
            <% }
               else
               { %>
            <div class="instructor-image">
                <img src="<%= Url.Action("Index", "Style", new {path = "images/unknown_user.jpg"}) %>"
                        alt="Image" class="avatar-image" />
                </div>
            <% } %>
            <div class="info-wrapper">
                <ul class="instructor-info">
                    <li class="instructorname">
                        <%= Model.UserProfile.FirstName.Replace("^$#!", "'").Replace("^$#~", @"""")%>&nbsp;<%= Model.UserProfile.LastName.Replace("^$#!", "'").Replace("^$#~", @"""")%>
                    </li>
                   <% if (Model.CourseType != null && Model.CourseType.ToLowerInvariant() == CourseType.XBOOK.ToString().ToLowerInvariant())
                      { %>
                        <li class="status">
                        <%= Model.AccessLevel%>
                    </li>
                    <% } %>
                    <%else
                      { %>
                    <li class="status">Status:
                        <%= Model.AccessLevel%>
                    </li>
                    <% } %>
                </ul>
                <span class="editProfile-icon"></span>
            </div>
         </div>
         

         <div class="instructor-profile-links">
                <% if (!isReadOnly)
                  { %>    
                    <div class="edit-profile-btn">
                        <a href="#" onclick="return PXProfileSummaryWidget.ShowProfileSummaryEditorWidgetModal({
					    userId: '<%= Model.UserProfile.Id %>',
					    userRefId: '<%= Model.UserProfile.ReferenceId %>',
					    firstName : '<%= Model.UserProfile.FirstName %>', 
					    lastName : '<%= Model.UserProfile.LastName %>', 
					    email : '<%= Model.UserProfile.Email %>', 
					    imageUrl : '<%= Model.UserProfile.AvatarUrl %>',
                        isStudentView: <%= ViewData["IsStudentView"]==null?"false":ViewData["IsStudentView"] %>
					    });">
                        <span class="editprofile-icon"></span>
                        Edit your profile
                        <span class="next-btn-arrow"></span>
                        </a></div>
                    <% } %>

                  <div class="goto-eport-btn">
                     <a href="#" onclick="return PXProfileSummaryWidget.RedirectToEportfolioDashboard({ dashboardLnk : '<%= linkDash %>', isStudentView: <%= ViewData["IsStudentView"] %> });">
                            <span class="got-eport-icon"></span> 
                             Go to your dashboard <span class="next-btn-arrow"></span></a>
                    </div>

         </div>

         </li>
    </ul>
 
    <% } %>
</div>
<div class="showProfileSummaryEditorWidgetModal">
    <div class="placeHoldershowProfileSummaryEditorWidgetModal">
    </div>
</div>
