<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<VerticalMenu>" %>

<div class="vertical-menu">
    <ul>    
        <li class="highlighted"><%= Html.RouteLink("Home >", "CourseSectionHome")%></li>    
    <% if (!Model.NavItems.IsNullOrEmpty())
       { %>
            
            <% foreach (var navItem in Model.NavItems)
               {
                   var courseId = ViewContext.RouteData.Values["courseid"].ToString() == "0" ? ViewData["EntityId"] : ViewContext.RouteData.Values["courseid"];
                   var linkUrl = Url.RouteUrl("FeaturedContentItem", new { id = navItem.Id, courseid = courseId });
                   %>
                    <li <%= navItem.Highlighted ? "class='highlighted'" : "" %>>
                        <a href="<%= linkUrl %>"><%= navItem.Title%></a>
                        <% if (!navItem.Categories.IsNullOrEmpty())
                           { %>
                            <div class="category-nav">
                            <%
                               foreach (var cat in navItem.Categories)
                               { %>
                                <%= Html.RouteLink(cat.Text, "FeaturedContentItem", new { id = navItem.Id, category = cat.Id }) %>
                        <%     } %>
                            </div>
                        <% } %>
                    </li>
            <% } %>            
     <% } %>  
     <% if (Model.ShowAssignmentLink)
       { %>
            <li><%= Html.ActionLink("Assignments", "Index", "AssignmentCenter", null, new { color = "#660000" })%></li>
    <% } %>
    <% if (!Model.IsProductCourse && !Model.IsAnonymous)
       {
           if (Model.IsInstructor)
           { %>
                <li><%= Html.ActionLink("Gradebook", "ManageGroups", "Groups")%></li>
        <% }
           else
           { %>
                <li><%= Html.RouteLink("Gradebook", "Scorecard", null, new { @class = "fne-link" })%></li>
        <% }
       }

       if (!Model.IsAnonymous && Model.ShowAssignmentLink)        
       { %>            
            <li><%= Html.ActionLink("My Writing", "Index", "WritingTab")%></li>
    <% }                     
       Html.RenderAction("Index", "SyllabusTool"); %>   
    </ul>
</div>
    

