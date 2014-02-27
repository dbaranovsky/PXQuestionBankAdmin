<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="System.Collections.ObjectModel" %>
<% var EnrollmentId = ViewData["EnrollmentId"] != null ? ViewData["EnrollmentId"] : -1; %>
<div id="content-item" class="fne-local content <%= Model.Content.Type.ToLower() %>" bfwtype=<%= Model.Content.GetType().Name %>>
    <input type="hidden" class="item-id" value="<%= Model.Content.Id %>" />
    <input type="hidden" class="track-minutes-spent" value="<%= Model.Content.TrackMinutesSpent.ToString().ToLower() %>" />
    <input type="hidden" class="enrollment-id" value="<%= EnrollmentId %>" />
    <input type="hidden" class="content-item-readonly" value="<%= Model.Content.ReadOnly %>" />
    <input type="hidden" class="content-item-title" value="<%= Model.Content.Title %>" />
    <!-- The TOC the item was opened/created/edited from -->
    <input type="hidden" class="toc" value="<%= Model.Toc %>" />
    <% 
        Model.IncludeNavigation = !Model.IncludeNavigation.HasValue || Model.IncludeNavigation.Value;
        
        if (Model.IncludeNavigation.Value)
        {
            Html.RenderPartial("DisplayItemContentNav", Model);
        }       

        var resultsClass = string.Empty;

        if (ContentViewMode.Results == Model.ActiveMode)
        {
            resultsClass = "content-results";
        }
    %>
    <div id="contentwrapper" class="contentwrapper <%= resultsClass %>">
        <% for (int i = 0; i < Model.ContentItems.Count; i++)
           {
               Model.Content = Model.ContentItems[i];
               if (ViewData["accessLevel"] != null && !string.IsNullOrEmpty(ViewData["accessLevel"].ToString()) && Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.None)
               {
                   Model.Content.UserAccess = (Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["accessLevel"];
               }
        %>
        <div id="content" class="content">
            <span id="content-item-id" class="content-item-id" style="display: none;"><%= Model.Content.Id%></span><%--This line has to stay on one line. Do not move. --%>
            <%= Html.Hidden("content-item-parentid",Model.Content.ParentId) %>
            <%
               if (ContentViewMode.Create == Model.ActiveMode)
               {
                   Html.RenderAction("CreateAndAssign", "ContentWidget", new { item = Model });
               }
               else if (ContentViewMode.Preview == Model.ActiveMode || ContentViewMode.ReadOnly == Model.ActiveMode)
               {
                   if (Model.Content.ApplyRestrictedAccess())
                   {
                       Html.RenderAction("RestrictedContent", "ContentWidget", new { ContentItem = Model.Content });
                   }
                   else
                   {
                       %><%= Html.DisplayFor(m => m.Content) %><%
                       if (Model.Content.CourseInfo.SocialCommentingAllowedTypes.Contains(Model.Content.GetType().Name.ToLowerInvariant()))
                       {
                           Html.RenderAction("DisplayCommenting", "SocialCommentingWidget", new { item = Model.Content });
                       }
                   }
               }
               else if (ContentViewMode.Assign == Model.ActiveMode)
               {
                   Html.RenderAction("AssignTab", "ContentWidget", new { item = Model, IsContentCreateAssign = false });
               }
               else if (ContentViewMode.Edit == Model.ActiveMode)
               {
                   if (Model.Content.Type.ToLower().Equals("externalcontent"))
                   {
                       ViewData["mode"] = "rename";
                       ViewData["sourceWindow"] = "fne";
                       Html.RenderPartial("~/Views/LaunchPadTreeWidget/EditContentView.ascx", Model.Content);
                   }
                   else
                   { 
                   %><%= Html.EditorFor(m => m.Content)%><%
                   }
               }
               else if (ContentViewMode.Settings == Model.ActiveMode && Model.Content != null)
               {
                   Html.RenderAction("ContentSettings", "ContentWidget", new { item = Model.Content });
               }
               else if (ContentViewMode.Questions == Model.ActiveMode && Model.Content != null)
               {
                   Html.RenderAction("EditQuiz", "Quiz", new {id = Model.Content.Id});
               }
               else if (ContentViewMode.Metadata == Model.ActiveMode)
               {
                   Html.RenderAction("MetaDataIndex", "AdminMetaData", new { id = Model.Content.Id });
               }
               else if (Model.Content is LearningCurveActivity && ContentViewMode.Results == Model.ActiveMode)
               {
                   ViewBag.Results = true;
                   %><%= Html.DisplayFor(m => m.Content) %><%
               }
               else if (ContentViewMode.Results == Model.ActiveMode)
               {
                   Html.RenderPartial("DisplayItemResults", Model);
               }
               else if (ContentViewMode.Grading == Model.ActiveMode)
               {
                   Html.TemplateFor(Model.Content, "Grading");
               }
               else if (ContentViewMode.Rubrics == Model.ActiveMode)
               {
                   var selectedCategory = String.Empty;
                   var viewMode = String.Empty;
                  if (Model.Content is ReflectionAssignment)
                   {
                       selectedCategory = ((ReflectionAssignment)Model.Content).SelectedCategory;

                   }
                   if (!selectedCategory.IsNullOrEmpty())
                   {
                       viewMode = "readonly";
                   }
                   Html.RenderAction("Rubrics", "ContentWidget", new { item = Model, viewMode = viewMode });

               }
               else if (Model.ActiveMode == ContentViewMode.MoreResources)
               {
                   Html.RenderAction("MoreResources", "ContentWidget", new { id = Model.Content.Id });
               }
               else if (Model.ActiveMode == ContentViewMode.MoreResourcesStatic)
               { %>
                <div id="moreresourcesstatic">
                </div>
            <%}
               else
               { %>
                <span>This type of view is not supported for the content you are viewing.</span>
            <% } %>
                <div id="content-loading" class="content-loading" style="display: none; margin-top: 10px;
                    height: 20px;">
                    <div id="loadingBlock"></div>
                </div>
        </div>
        <% } %>
    </div>
</div>
<%
    Model.Content.Id = Model.Content.Id ?? "";
    Model.Content.ParentId = Model.Content.ParentId ?? "";
    if (Model.Content.ParentId != "PX_TEMP" && Model.Content.Id.Length > 0 && Model.Content.Type != "Content404")
    {
        var category = ViewData["Category"] != null ? ViewData["Category"].ToString() : string.Empty;

        var categorytext = string.Empty;
        if (!Model.Categories.IsNullOrEmpty() && !category.IsNullOrEmpty())
        {
            var toccategory = Model.Categories.Where(c => c.Id == category).FirstOrDefault();
            categorytext = toccategory != null ? toccategory.Text : string.Empty;
        }              
%>
<script type="text/javascript">
    if (typeof jQuery != "undefined") {
        $(document).ready(function () {
            // sets cookie for last item accessed
            PxPage.OnReady(function () {
                PxPage.Require(['<%= Url.ContentCache("~/Scripts/Other/login_redir.js") %>'], function () {
                    var studentCookie = get_cookie('StudentView');
                    var viewMode = '1';

                    if (studentCookie == 'true') {
                        set_cookie('viewMode', '0', 100, '/');
                        viewMode = '0';
                    } else {
                        set_cookie('viewMode', '1', 100, '/');
                        viewMode = '1';
                    }

                    if (window.top.location.href.toLowerCase().indexOf("/item/") > 0 && viewMode == '1') {
                        set_cookieLastItem('LastItem', '/<%= ViewContext.RouteData.Values["courseid"]%>/item/', 100, '/');
                    }
                });
            });
           
            $("#AssignTabLinkId").click(function () {
                $(".assign-tab a").click();
            });

            ContentWidget.BindRequireConfirmControls();
        });
    }

    function removeResize() {
        $('#left').removeClass('ui-resizable');
        $('.ui-resizable-handle.ui-resizable-e').remove();
        $('.clearfix').remove();
        $('#content-nav').remove();
    }

</script>
<%}%>
