<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>  
    <%
        if (Model.IncludeBreadcrumb != false)
        {%>
            <div class="breadcrumb">
            </div>
        <%
        }
        if (Model.IncludeContentModes != false)
        {
            Html.RenderPartial("ContentModes", Model, ViewData);
        }
        //}%>
    <%--<div id="assignmentcenternav">
    </div>--%>
    <% if (ContentViewMode.Preview == Model.ActiveMode)
       {
           var fneLinkcontentViewMode = ContentViewMode.Preview;
           var fneLinkReadonly = false;
           var includeDisc = true;
           var fneLocal = "fne-local";
           if (Model.Content.Type.ToLowerInvariant() == "assignment" ||
           Model.Content.Type.ToLowerInvariant() == "linkcollection" ||
           Model.Content.Type.ToLowerInvariant() == "rsslink" ||
           Model.Content.Type.ToLowerInvariant() == "rssfeed")
           {
               fneLinkcontentViewMode = ContentViewMode.ReadOnly;
               fneLinkReadonly = true;
               includeDisc = false;
               fneLocal = "";
           }
    %>
    <div id="content-nav">
        <%  var container = Model.Content.GetContainer(Model.Toc).IsNullOrEmpty() ? "" : Model.Content.GetContainer(Model.Toc).ToLower();
            if (Model.Content.AllowComments && !container.Equals("launchpad") && !container.Equals("ebook"))//TODO: this is not a good way to test if item is in course
           { %>
        <div id="highlight-menu-container">
            <span id="content_widget_highlight_menu" class="action_menu"></span><span class="highlight-count">
            </span>
        </div>
        <% } %>
        <span id="nav-container">
            <span id="back" class="navigate-back">
                <span class="navbtn-back-icon"></span>
                <% if (Model.Content.CourseInfo.CourseType.ToString().ToLower().Equals("faceplate"))
                   { %>
                        Previous
                   <%} %>
                   <% else if (Model.Content.CourseInfo.CourseType.ToString().ToLower().Equals("xbook"))
                   { %> 
                   Previous
                   <% } %>
            </span> 
            <span id="next" class="navigate-next">
            <% if (Model.Content.CourseInfo.CourseType.ToString().ToLower().Equals("faceplate"))
                   { %>
                        Next
                   <%} %>  
            <% else if (Model.Content.CourseInfo.CourseType.ToString().ToLower().Equals("xbook"))
                   { %> 
                   Next
                   <% } %>
                <span class="navbtn-next-icon"></span>
            </span> 
        </span>
    </div>
    <% } %>