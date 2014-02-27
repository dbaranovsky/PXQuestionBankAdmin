<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="banner">    
    <div class="courseinfo">
        <% Html.RenderAction("CourseHeader", "Header", new { menuName = "" }); %>
    </div>
    <div id="banner-image">
    
    </div>
    
</div>