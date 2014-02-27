<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<% 
    bool isSandboxCourse = (ViewData["IsSandboxCourse"] == null) ? false : Convert.ToBoolean(ViewData["IsSandboxCourse"]);
    TreeWidgetSettings settings = new TreeWidgetSettings()
    {
        IsSandboxCourse = isSandboxCourse
    };
    //CHANGE!
    TreeWidgetViewItem viewitem = new TreeWidgetViewItem(Model.Content, settings);
    %>
<div id="fne-header-view">
    <!-- Home button -->
    <a href="javascript:" id="fne-unblock-action-home" class="show-faceplate-home-icon faceplate-fne-home-icon">
        <span class="home-btn-icon"></span>Home</a>
    <!-- Title/Breadcrumb-->
    <div id="fne-title">
        <%= Model != null && Model.Content != null ? Model.Content.Title : string.Empty %>
        <%--<div class="breadcrumb">
            </div>--%>
        <%--<span id="fne-title-breadcrumb"></span>--%>
    </div>
    <!-- Contains the higlight options -->
    <% if (Model != null)
       { %>
        <!-- Contains FNE actions -->
        <div id="fne-actions">
            <% if (Model.Content.AllowComments)
               { %>
                <div id="highlight-menu-container">
                    <span id="content_widget_highlight_menu" class="action_menu"></span><span class="highlight-count">
                                                                                        </span>
                </div>
            <% } %>
            <% if (Model.Content.GetContainer(Model.Toc).ToLower().Equals("launchpad"))
               {
                  
                   var assignUrl = Url.GetComponentHash("item", Model.Path,
                                                        new
                                                            {
                                                                mode = ContentViewMode.Assign,
                                                                isMultipartLessons = !string.IsNullOrWhiteSpace(Model.Content.ParentId),
                                                                hasParentLesson = Model.Content.ParentId,
                                                                includeNavigation = true,
                                                                isBeingEdited = true,
                                                                renderFne = true
                                                            });
                   var editUrl = Url.GetComponentHash("item", Model.Path,
                                                      new
                                                          {
                                                              mode = ContentViewMode.Edit,
                                                              hasParentLesson = Model.Content.ParentId,
                                                              includeNavigation = true,
                                                              isBeingEdited = true,
                                                              renderFne = true
                                                          });

                
                   if (!Model.Content.IsAssigned && !isSandboxCourse && Model.Content.UserAccess != AccessLevel.Student && 
                       (Model.AllowedModes & ContentViewMode.Assign) == ContentViewMode.Assign)
                   { %>
                    <span class="fne-action-btn">
                        <a href="<%= assignUrl %>" id="fne-item-assign">
                            <span>Assign</span></a>
                    </span>
                <% }
                   else if(Model.Content.UserAccess == AccessLevel.Instructor)
                   { %>
                    <span class="fne-action-status">
                        
                        <a href="<%= assignUrl %>" id="fne-item-status">
                            <%      Html.RenderPartial("LaunchPadItemStatus", viewitem); %>
                        </a>
                    </span>
                <% }
                   else if (Model.Content.UserAccess == AccessLevel.Student)
                   { %>
                    <span class="fne-action-status"><% Html.RenderPartial("LaunchPadItemStatus", viewitem); %></span>
                <%  }
                   if ((Model.AllowedModes & ContentViewMode.Edit) == ContentViewMode.Edit)
                   { %>
                    <span  class="fne-action-btn">
                        <a href="<%= editUrl %>" id="fne-edit">
                            <span>Edit</span>
                            <span class="dropdown-icon"></span></a>
                    </span>
                <% } %>
            
            
                <%--        
               Html.RenderPartial("~/Views/FacePlate/FacePlateStudentDetails.ascx", ViewData);
           %>--%>
           
                <% Html.RenderPartial("FneHeaderEditMenu"); %>
            <% } %>
            <% //if ((Model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results)
           if (Model.Content.UserAccess == AccessLevel.Instructor && (Model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results)
               {
                   var resultsUrl = Url.GetComponentHash("item", Model.Path,
                                                         new
                                                             {
                                                                 mode = ContentViewMode.Results,
                                                                 hasParentLesson = Model.Content.ParentId,
                                                                 includeNavigation = true,
                                                                 isBeingEdited = true,
                                                                 renderFne = true,
                                                                 toc = Model.Toc
                                                             }); %>
                <span  class="fne-action-btn">
                    <a href="<%= resultsUrl %>" id="fne-results">
                        <span>Results</span></a>
                </span>
            <% }
               if (Model.IncludeNavigation.HasValue && Model.IncludeNavigation.Value)
               {
                   Model.IncludeContentModes = false;
                   Model.IncludeBreadcrumb = false;
                   Html.RenderPartial("DisplayItemContentNav", Model);
               } %>
        </div>
    <% } %>
</div>

<%  if (Model != null && Model.Content != null)
    { %>
    <div class="fne-hidden">
        <% Html.RenderPartial("FneHeaderDoneButton", Model); %>
    </div>
<%  } %>