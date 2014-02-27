<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>

<div id="fne-header-view" class="not-in-course">
        <!-- Home button -->
    <a href="javascript:" id="fne-unblock-action-home" class="show-faceplate-home-icon faceplate-fne-home-icon">
        <span class="home-btn-icon"></span>Home</a>
    <div id="fne-title">
        <%= Model != null && Model.Content != null ? Model.Content.Title : string.Empty%>
    </div>
    <div id="fne-actions">
        <% 
            var accessLevel = Model.Content != null ? Model.Content.UserAccess : (AccessLevel)ViewData["accessLevel"];
                
            if (Model != null && (Model.Content != null || Model.Url.Length > 0) && accessLevel != AccessLevel.Student)
           { %>
        <div id="fne-subtitle">This item is not in your course. Add to <span id="chapter-name"></span>?</div>
            <span  class="fne-action-btn">
                <a href="#" id="fne-add" class="fneadd primary">
                    <span>Add to course</span>
                </a>
            </span>
            <% } %>
    </div>
     
</div>
