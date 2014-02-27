<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.UserInfo>" %>

<% var showstudentprofile = ViewData["category"] != null;
   var StudentItemCount = ViewData["StudentItemCount"] != null ? ViewData["StudentItemCount"].ToString() : string.Empty;
   var anonymousName = ViewData["anonymousName"] != null ? ViewData["anonymousName"].ToString() : string.Empty;
   var studentName = anonymousName.IsNullOrEmpty() ? Model.FormattedName : anonymousName;
   
   if (showstudentprofile) { 
   %>
    <div class="student-profile-widget">

            <%  var avatarurl = string.IsNullOrEmpty(Model.AvatarUrl) ? Url.Action("Index", "Style", new { path = "images/unknown_user.jpg" }) : Model.AvatarUrl;
            %>
            
            <div class="student-image">
                <img src="<%= avatarurl %>" alt="Image" class="avatar-image" />
            </div>

            <div class="student-info">
                <div class="student-name">
                    <%= studentName %>
                </div>
                <div class="student-item-count">
                    <%= StudentItemCount%> items
                </div>
            </div>

    </div>

<% } %>


