<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%
    var isSharedCourse = ViewData["IsSharedCourse"] != null && Convert.ToBoolean(ViewData["IsSharedCourse"].ToString());
    var courseType = (ViewData["courseType"] == null) ? "" : ViewData["courseType"].ToString();
    var accessLevel = (ViewData["accessLevel"] == null) ? "" : ViewData["accessLevel"].ToString();
    bool isSandboxCourse = (ViewData["IsSandboxCourse"] == null) ? false : Convert.ToBoolean(ViewData["IsSandboxCourse"]);
    bool includeNavigation = true;
    if (Model != null && Model.IncludeNavigation.HasValue)
    {
        includeNavigation = Model.IncludeNavigation.Value;
    }

    if (Model != null)
    {
        var selectedCategory = string.Empty;       

        if (Model.Content.GetType() == typeof (ReflectionAssignment))
        {
            selectedCategory = ((ReflectionAssignment) Model.Content).SelectedCategory;
        }


        var routeValues = new RouteValueDictionary(new
            {
                id = Model.Content.Id,
                mode = ContentViewMode.Preview,
                getChildrenGrades = false,
                groupId = "groupIdValue",
                category = selectedCategory,
                includeNavigation = includeNavigation,
                renderFne = Model.RenderContentModesInFne == true,                
                isBeingEdited = true
            });
        if (!String.IsNullOrWhiteSpace(Model.Toc))
        {
            routeValues["toc"] = Model.Toc;
        }
        var ajaxOptions = new AjaxOptions()
            {
                UpdateTargetId = "content-item",
                OnSuccess = "",
                InsertionMode = InsertionMode.Replace,
                //OnBegin = "ContentWidget.OnBeginLoad",
                OnComplete = String.Format("ContentWidget.LoadContentModes({0});", Model.RenderContentModesInFne)
            };
        Func
            <string, string, string, ContentViewMode, RouteValueDictionary, AjaxOptions, Dictionary<string, object>,
                MvcHtmlString> createContetModeLink =
                    (string title, string actionName, string controllerName, ContentViewMode mode,
                     RouteValueDictionary r, AjaxOptions o, Dictionary<string, object> htmlAttr) =>
                        {
                            if (r == null)
                            {
                                r = new RouteValueDictionary();
                            }
                            if (o == null)
                            {
                                o = new AjaxOptions();
                            }
                            if (htmlAttr == null)
                            {
                                htmlAttr = new Dictionary<string, object>();
                            }
                            //combine route value dictionaries
                            var rv =
                                new RouteValueDictionary(
                                    r.Concat(routeValues.Where(x => !r.Keys.Contains(x.Key))).ToDictionary(e => e.Key,
                                                                                                           e => e.Value));
                            rv["mode"] = mode;
                            //combine ajax options and html attribtues into one attributes ductionary
                            var newHtmlAttr = o.ToUnobtrusiveHtmlAttributes();
                            var attributes = newHtmlAttr
                                .Concat(ajaxOptions.ToUnobtrusiveHtmlAttributes()
                                            .Where(x => !newHtmlAttr.Keys.Contains(x.Key)))
                                .Concat(htmlAttr)
                                .ToDictionary(e => e.Key, e => e.Value);

                            if (Model.RenderContentModesInFne == true)
                            {
                                var id = rv["id"].ToString();
                                rv.Remove("id");
                               
                                return new MvcHtmlString(Url.GetComponentLink(title, "item", Model.Path, rv, htmlAttr));
                            }
                            else
                            {
                                //create ajax link
                                //attributes["data-ajax-complete"] = "";
                                return Ajax.ActionLink(title, actionName, controllerName, rv, null, attributes);
                                //return new MvcHtmlString(Url.GetComponentLink(title, "item", rv["id"].ToString(), rv, htmlAttr));
                            }
                        };
%>
    <div class="menu content-menu-tabs">
        <% if (!isSharedCourse)
           { %>
            <ul id="content-modes" class="link-list">
                <%
                    if (((Model.AllowedModes & ContentViewMode.Preview) == ContentViewMode.Preview) ||
                        ((Model.AllowedModes & ContentViewMode.ReadOnly) == ContentViewMode.ReadOnly))
                    {
                %>
                    <li class=" view-tab <%= (ContentViewMode.Preview == Model.ActiveMode) ? "active" : "" %> ">
                        <%= createContetModeLink("View", "DisplayItem", "ContentWidget", ContentViewMode.Preview,
                                              new RouteValueDictionary(new {getChildrenGrades = true}),
                                              new AjaxOptions() {OnSuccess = "PxPage.ViewLoadComplete"},
                                              new Dictionary<string, object> {{ "class", "requireConfirmation" }, {"id", "view"}}) %>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <!-- Edit -->
                <% if ((Model.AllowedModes & ContentViewMode.Edit) == ContentViewMode.Edit)
                   {
                      %>
                    <li class="questions-tab <%= (ContentViewMode.Edit == Model.ActiveMode) ? "active" : "" %>">
                        <%= createContetModeLink("Basic Info", "DisplayItem", "ContentWidget",
                                                ContentViewMode.Edit, null, null,
                                                new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationBasicInfo" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                    
                <%      
                   } %>
                <!-- Assign -->
                <% if (((Model.AllowedModes & ContentViewMode.Assign) == ContentViewMode.Assign) && !isSandboxCourse)
                   { %>
                    <li <%= (ContentViewMode.Assign == Model.ActiveMode)
                             ? "class=\"assign-tab active\""
                             : "class=\"assign-tab\"" %>>
                        <%= createContetModeLink("Assignment", "DisplayItem", "ContentWidget",
                                              ContentViewMode.Assign, null, null,
                                              new Dictionary<string, object> { { "class", "assignTabGroupLink requireConfirmation" }, { "id", "requireConfirmationAssign" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <!-- settings -->
                <% if ((Model.AllowedModes & ContentViewMode.Settings) == ContentViewMode.Settings)
                   { %>
                    <li class="settings-tab <%= (ContentViewMode.Settings == Model.ActiveMode) ? "active" : "" %>">
                        <%= createContetModeLink("Settings", "DisplayItem", "ContentWidget", ContentViewMode.Settings,
                                              null,
                                              new AjaxOptions() {OnSuccess = "ContentWidget.InitSettingsTab"},
                                              new Dictionary<string, object> { { "class", "settingsTabGroupLink requireConfirmation" }, { "id", "requireConfirmationSettings" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <!-- questions -->
                <% if ((Model.AllowedModes & ContentViewMode.Questions) == ContentViewMode.Questions)
                   { %>
                    <li class="questions-tab <%= (ContentViewMode.Questions == Model.ActiveMode) ? "active" : "" %>">
                        <%= createContetModeLink("Questions", "DisplayItem", "ContentWidget",
                                                ContentViewMode.Questions, null, null, 
                                                new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationQuestions" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <!-- rubrics -->
                <% if ((Model.AllowedModes & ContentViewMode.Rubrics) == ContentViewMode.Rubrics)
                   {
                       string resultsUrl = Url.Action("DisplayItem", "ContentWidget",
                                                      new
                                                          {
                                                              id = Model.Content.Id,
                                                              mode = ContentViewMode.Rubrics,
                                                          }); %>
                    <li <%= (ContentViewMode.Rubrics == Model.ActiveMode)
                             ? "class=\"rubric-tab active\""
                             : "class=\"rubric-tab\"" %>>
                        <% 
                       
                       { %>
                        <%= createContetModeLink("Rubrics & Learning Objectives", "DisplayItem",
                                              "ContentWidget",
                                              ContentViewMode.Rubrics,
                                              null,
                                              new AjaxOptions() { OnSuccess = "ContentWidget.InitResultsTab" },
                                              new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "view" } })%>
                        <%
                       }
                        %>
                        <div class="assign-arrow">
                        </div>
                    </li>
                    <% if ((Model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results)
                       { %>
                        <li <%= (ContentViewMode.Results == Model.ActiveMode)
                             ? "class=\"results-tab active\""
                             : "class=\"results-tab\"" %>>
                            <%= createContetModeLink("Results", "DisplayItem", "ContentWidget", ContentViewMode.Results, null,
                                                    new AjaxOptions() { OnSuccess = "ContentWidget.ResultLoadComplete" },
                                                    new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationResults" } })%>

                            <div class="assign-arrow">
                            </div>
                        </li>
                    <% } %>
                <% }
                   else
                   {
                       var resultsTabAllowed = !(Model.Content is ExternalContent) || (Model.Content is Folder);

                       if (((Model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results ||
                            (Model.AllowedModes & ContentViewMode.ReadOnly) == ContentViewMode.ReadOnly)
                           && Model.Content.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()
                           && resultsTabAllowed == true
                           && Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
                       {
                %>
                        <li <%= (ContentViewMode.Results == Model.ActiveMode)
                             ? "class=\"results-tab active\""
                             : "class=\"results-tab\"" %>>
                            <%= createContetModeLink("Results", "DisplayItem", "ContentWidget", ContentViewMode.Results, null,
                                              new AjaxOptions() {OnSuccess = "ContentWidget.ResultLoadComplete"},
                                              new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationQuestionsResults" } })%>
                            <div class="assign-arrow">
                            </div>
                        </li>
                <% }
                   }%>
                <%
                   if (((Model.AllowedModes & ContentViewMode.MoreResources) == ContentViewMode.MoreResources) &&
                       Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
                   {
                %>
                    <li <%= (ContentViewMode.MoreResources == Model.ActiveMode) ? "class=\"active\"" : "" %>>
                        <%= createContetModeLink("More Resources", "DisplayItem", "ContentWidget",
                                              ContentViewMode.MoreResources, null,
                                              new AjaxOptions()
                                                  {OnSuccess = "ContentWidget.InitMoreResourcesTab"},
                                              new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationMore" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <%
                   if (((Model.AllowedModes & ContentViewMode.MoreResourcesStatic) ==
                        ContentViewMode.MoreResourcesStatic) &&
                       Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
                   {
                %>
                    <li <%= (ContentViewMode.MoreResourcesStatic == Model.ActiveMode) ? "class=\"active\"" : "" %>>
                        <%= createContetModeLink("More Resources", "DisplayItem", "ContentWidget",
                                              ContentViewMode.MoreResources, null,
                                              new AjaxOptions()
                                                  {
                                                      OnSuccess =
                                                          "ContentWidget.InitMoreResourcesStaticTab"
                                                  },
                                             new Dictionary<string, object> { { "class", "requireConfirmation" }, { "id", "requireConfirmationBasicInfo" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
                <% if (Model.Content.CourseInfo.IsSandboxCourse)
                   {
                %>
                    <li <%= (ContentViewMode.Metadata == Model.ActiveMode) ? "class=\"active\"" : "" %>>
                        <%= createContetModeLink("Metadata", "DisplayItem", "ContentWidget",
                                              ContentViewMode.Metadata, null,null,
                                              new Dictionary<string, object> { { "class", "addActive" }, { "id", "requireConfirmationMetadata" } })%>
                        <div class="assign-arrow">
                        </div>
                    </li>
                <% } %>
            </ul>
        <% } %>
    </div>
<%
    }
%>