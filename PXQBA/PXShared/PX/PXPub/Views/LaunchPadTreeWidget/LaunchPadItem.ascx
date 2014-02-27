<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.TreeWidgetViewItem>>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<%
    var firstitem = Model.FirstOrDefault();
    var settings = firstitem.Settings;

    var disableEditing = !settings.AllowEditing;
    var showOnly = settings.ShowOnlyFilter; 
    bool grayoutpastduelater = settings.GreyoutPastDue; 
    bool collapseUnassigned = settings.CollapseUnassigned; 
    bool collapseDuelater = settings.AllowDueLater; 
    bool collapsePastDue = settings.AllowPastDue; 
    bool hideDescription = !settings.ShowDescription; 
    bool hideDateStudentData = !settings.ShowStudentDateData;
    bool fneOnlyLearningCurve = settings.FneOnlyLearningCurve;
    var course = settings.CourseId;
    int toggleDueLaterDays = settings.DueLaterDays; 
    string pastDueVisibility = " ";
    string dueLaterVisibility = " ";
    string collapseUnassignedVisibility = " ";

    bool isAssigned = false;

    if (!showOnly.IsNullOrEmpty() && showOnly == "assigned")
    {
        collapseUnassignedVisibility = " hide ";
    }

    bool isCourseSandbox = settings.IsSandboxCourse; 
    var isCourseSandboxClass = "";
    var sbManagementCardClass = "";
    if (isCourseSandbox)
    {
        isCourseSandboxClass = "sandbox-inactive";
        sbManagementCardClass = "sandbox";
    }
    bool quizBrowser = settings.QuizBrowser; 
%>
<% foreach (var item in Model)
   {%>
    <%
        var level = item.Level;
        var hasParent = item.ParentId != null;
        bool hideDataVisibleAttr = false;
        var isContentFolder = item.Item is PxUnit && !string.IsNullOrWhiteSpace(item.Item.Url);
        var isLearningCurve = item.Item.FacetMetadata.ContainsKey("meta-content-type") && 
                                item.Item.FacetMetadata["meta-content-type"].Contains("LearningCurve");
        if (Request.Cookies.AllKeys.Contains(course + "collapse_all") &&
            Request.Cookies[course + "collapse_all"].Value == "1" && !item.Item.IsAssigned &&
            item.Item.DueDate.Year == DateTime.MinValue.Year)
        {
            collapseUnassignedVisibility = "hide";
            hideDataVisibleAttr = true;
        }
        else if (collapseUnassigned && item.Item.DueDate.Year == DateTime.MinValue.Year)
        {
            collapseUnassignedVisibility = "hide";
            hideDataVisibleAttr = true;
        }
        
        if (item.Item.level > 0)
        {
            level = item.Item.level;
        }

        var parentId = item.ParentId;
        item.Item.Id = item.Item.Id.Replace("Shortcut__1__", "");
        bool isPastDueChapter = false;
        bool isDueLater = false;
        
        var path = item.Path;
        var isTaggedForEbook = (item.Item is PxUnit) ||
                               (item.Item.Categories.FirstOrDefault(i => i.Id == "bfw_faceplate_filter" && i.Text == "ebook") !=
                                null);
        bool isBarren;
                                        
        //TODO: Get rid of the checks against syllabusfilter and pxunit.  Should be able to just use the view item prop.
        var hasChildren = (item.Item.GetSyllabusFilterFromCategory(item.Settings.TOC) == item.Item.Id) ||
            (item.Item is PxUnit && level == 1) || item.HasChildren;

        if (item.Item.Type.ToLowerInvariant() == "pxunit")
        {
            isBarren = false;
        }
        else
        {
            isBarren = true;
        }   
        
        if (item.Item.DueDate.Year != DateTime.MinValue.Year)
        {
            if (showOnly.IsNullOrEmpty() && showOnly == "assigned")
            {
                collapseUnassignedVisibility = "";
            }
            isAssigned = true;
            if (level == 1 && item.Item.DueDate <= DateTime.Now.GetCourseDateTime())
            {
                isPastDueChapter = true;

                if (Request.Cookies.AllKeys.Contains(course + "hide_past_due"))
                {
                    if (Request.Cookies[course + "hide_past_due"].Value == "1")
                    {
                        pastDueVisibility = " hide";
                        hideDataVisibleAttr = true;
                    }
                }
                else if (collapsePastDue)
                {
                    pastDueVisibility = " hide";
                    hideDataVisibleAttr = true;
                }
            }
            if (level == 1 && (item.Item.DueDate.Subtract(DateTime.Now.GetCourseDateTime()).Days > toggleDueLaterDays) &&
                (item.Item.StartDate == DateTime.MinValue || 
                item.Item.StartDate.Subtract(DateTime.Now.GetCourseDateTime()).Days > toggleDueLaterDays))
            {
                isDueLater = true;
                if (Request.Cookies.AllKeys.Contains(course + "hide_due_later"))
                {
                    if (Request.Cookies[course + "hide_due_later"].Value == "1")
                    {
                        dueLaterVisibility = " hide";
                        hideDataVisibleAttr = true;
                    }
                }
                else if (collapseDuelater)
                {
                    dueLaterVisibility = " hide";
                    hideDataVisibleAttr = true;
                }
            }
        }

        bool isVisibleToStudents = (item.Item.Visibility.Descendants("student").Any());
        if (isVisibleToStudents)
        {
            if (item.Item.ApplyRestrictedAccess(true))
            {
                isVisibleToStudents = false;
            }
        }
        
        
        bool isInstructor = (settings.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor);
        bool hideInFneNavigation = (item.Item.Type.ToLower().Equals("pxunit", StringComparison.InvariantCultureIgnoreCase) && !isContentFolder) ||
                                   item.Item.Type.ToLower().Equals("browseresourcesfixed",
                                                              StringComparison.InvariantCultureIgnoreCase) ||
                                   item.Item.Type.ToLower().Equals("chapterresourceslinksfixed",
                                                              StringComparison.InvariantCultureIgnoreCase);
        string Title = Server.HtmlDecode(item.Item.Title.IsNullOrEmpty() ? string.Empty : 
            item.Item.Title.Replace('\u0097', '\u2014')); //Decode title, fix &mdash

%>
    <%
        bool show = false;
        if (isVisibleToStudents && isInstructor == false)
        {
            show = true;
        }
        else if (isInstructor)
        {
            show = true;
        }

        if (show)
        {
            var itemClasses = " unitrowlevel" + level.ToString();
            itemClasses += (isTaggedForEbook) ? "" : " HideForEbook";
            itemClasses += " item-type-" + item.Item.Type.ToLower();
            itemClasses += (hideInFneNavigation) ? " hide-in-fne" : "";
            itemClasses += (hideInFneNavigation && isTaggedForEbook) ? " hide-in-fne" : " hide-in-fne-ebook";
            itemClasses += isVisibleToStudents ? "" : " hidden-from-students";
            itemClasses += " " + settings.UserAccess.ToString().ToLower();
            itemClasses += (isPastDueChapter) ? " past-due " + pastDueVisibility : "";
            itemClasses += (isDueLater) ? " due-later " + dueLaterVisibility : "";
            itemClasses += " " + collapseUnassignedVisibility;
            itemClasses += (!isAssigned) ? " is-not-assigned" : "";

%>
        <li class="faux-tree-node pxunit-item-row <%= itemClasses %>" 
            data-ud-itemtype="<%= item.Item.Type %>"
            data-ud-points="<%= item.Item.MaxPoints %>" 
            data-ud-isvisibletostudents="<%= isVisibleToStudents %>"
            data-ft-parent="<%= parentId %>" 
            data-ft-id="<%= item.Item.Id %>" 
            data-ud-id="<%= item.Item.Id %>"
			data-ud-islc="<%= isLearningCurve %>"
            data-ud-date-mode="<%= isBarren? "single" : "range" %>" <%= isBarren ? "data-ft-state=\"barren\"" : "" %>
            data-ft-sequence="<%= item.Item.GetSequenceFromCategory(item.Settings.TOC) %>" 
            data-ft-has-children="<%= hasChildren.ToString().ToLowerInvariant() %>" 
            data-ud-start-date="<%= item.Item.StartDate.ToString("MM/dd/yyyy") %>" 
            data-ud-start-time="<%= item.Item.StartDate.ToString("hh:mm:ss tt") %>"  
            data-ud-due-date="<%= item.Item.DueDate.ToString("MM/dd/yyyy") %>" 
            data-ud-due-time="<%= item.Item.DueDate.Year == 1 ? "12:00 AM" : item.Item.DueDate.ToString("hh:mm:ss tt") %>" 
            data-ud-wasduedatemanuallyset="<%= item.Item.WasDueDateManuallySet %>"
            data-ud-sortlevel="<%= itemClasses %>" 
            data-ud-read-only="<%= item.Item.ReadOnly.ToString().ToLower() %>"
            data-ud-is-assigned="<%= isAssigned.ToString().ToLower() %>"
            data-ft-level = "<%= level %>"
            data-ft-chapter = "<%= item.Item.GetSubContainer(item.Settings.TOC) %>" 
            hide-student-data = "<%= hideDateStudentData %>"
            <% if (item.Item.DefaultPoints > 0)
               { %> data-ud-default-points="<%= item.Item.DefaultPoints %>"<% } %>
            <% if (item.Item.MaxPoints > 0)
               { %> data-ud-max-points="<%= item.Item.MaxPoints %>"<% } %>
            <% if (hideDataVisibleAttr)
               {%> data-ft-visible="false"<% } %>
            >

            <div class="faux-tree-node-title col faceplate-hover-div <%= (isBarren) ? "showaslink" : "" %>">
                <% if (item.Item is PxUnit)
                   {
                    if (string.IsNullOrEmpty(((PxUnit)item.Item).Thumbnail))
                    {
                        ((PxUnit)item.Item).Thumbnail = Url.Action("Index", "Style", 
                            new { path = ConfigurationManager.AppSettings.Get("DefaultImage") });
                    }

                    if (HttpContext.Current.Request.Url.Host == "localhost" && 
                        ((PxUnit)item.Item).Thumbnail.Contains("brainhoney/resource") && 
                        !((PxUnit)item.Item).Thumbnail.Contains("http://dev.worthpublishers.com"))
                    {
                        ((PxUnit)item.Item).Thumbnail = String.Format("{0}{1}", "http://dev.worthpublishers.com", 
                            ((PxUnit)item.Item).Thumbnail);
                    }
                %>                       
                        <div class="fp-img-wrapper">
                            <img class="fpimage" alt="" src="<%= ((PxUnit)item.Item).Thumbnail %>"  width="30" height="45"  /></div>
               <% 
                   }
                   else
                   { %>
                    <div class="fp-img-wrapper">
                        <span class="fpimage"></span>
                    </div>
                <% } %>
                <% if (item.Item.Type == "ChapterResourcesLinksFixed" && level == 2)
                   { %>
                    <div class="more_resources_folder_title">
                        <% if (disableEditing)
                           { %>
                            <a href='#' onclick=" $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', 'chapter', '<%= item.Item.Title %>', 'disableediting'); return false; ">Browse more resources for this unit...</a>
                        <% }
                           else
                           { %>
                            <a href='#' onclick=" $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', 'chapter', '<%= item.Item.Title %>'); return false; ">Browse more resources for this unit...</a>
                        <% } %>
                
                    </div>
                <% }
                   else
                   {
                       //The icon 
                       if (item.Item is PxUnit && (settings.ShowExpandIconAtAllLevels || (!settings.ShowExpandIconAtAllLevels && level > 1)))
                           //if this is a collapsible node that is not a chapter (root) node
                       { %>
                        <span class="icon collapsed"></span>
                    <% }
                           else
                           { %>
                        <span class="icon-placeholder"></span>
                    <% } %>
                        <span class="<%= item.Item is PxUnit && level == 1 ? "unitfptitle" : "fptitle" %>">
                    <%  
                        var text = string.Empty;

                        //We don't link content when using the quiz browser.
                        if ((item.Item is PxUnit && string.IsNullOrWhiteSpace(item.Item.Url)) || quizBrowser)
                        {
                            text = Title.Truncate("...", 0, 75);
                        }
                        else
                        {
                             //For XB we only want learning curve items to render in FNE mode.
                            var inFne = (!fneOnlyLearningCurve || isLearningCurve);
         
                            text = Url.GetComponentLink(Title.Truncate("...", 0, 100), "item", path + item.Item.Id, new
                            {
                                mode = ContentViewMode.Preview,
                                getChildrenGrades = settings.UserAccess == AccessLevel.Student,
                                includeDiscussion = false,
                                readOnly = !item.Settings.AllowEditing,
                                renderFNE = inFne,
                                toc = item.Settings.TOC
                            }, new {@class = "faux-tree-link"});
                        } %>
                        <%= text %>
                       </span>
                    <% if (!(item.Item is PxUnit))
                       { %>
                        <span class="item_subtitle">
                            <%= item.Item.GetFriendlyItemContentType() %>
                        </span>
                    <% }
                   } %>
        
            </div>
            <% if (!hideDateStudentData)
               {%>
            <div class="faceplate-item-status">
                <div class="faceplate-row-on-hover">
                    <div class="pxunit-subitems-menu">
                        <a class="view-pxunit-menu" href="#">
                                  <%
                   if (!item.Item.ReadOnly &&
                       settings.UserAccess != Bfw.PX.Biz.ServiceContracts.AccessLevel.Student &&
                       !disableEditing)
                   {
                                  %>
                                      <div id="face-plate-unit-menu" class="face-plate-unit-menu">
                                          <% var assignStyle = (item.Item.IsAssigned) ? "display:none;" : string.Empty; %>
                                          <input type="button" class="faceplate-item-assign <%= isCourseSandboxClass%>" value="Assign" style="<%= assignStyle %>" />
                                          <div class="gearbox gradient" style="float: right">
                                              <span class="gearbox-icon pxicon pxicon-gear"></span>
                                          </div>
                                          <div style="clear: both;">
                                          </div>
                                      </div>
                                  <% } %>
                              </a>
                    </div>
                </div>
                <% Html.RenderPartial("LaunchPadItemStatus", item, ViewData); %>
            </div>
            <% } %>
            <%
                bool showDescription = (!string.IsNullOrWhiteSpace(item.Item.Description));
                if (!hideDescription && item.Item is PxUnit )
                {%>
                <div class="description" style="display: none">
                    <div class="px-default-text" style='display: <%= (showDescription) ? "block" : "none" %>'>
                        <%= item.Item.Description %></div>
                    <% if (level == 1)
                       { %>
                        <div style='display: <%= (showDescription) ? "block" : "none" %>;'>                            
                            <img class="fpimageLarge" alt="" delayedsrc="<%= ((PxUnit)item.Item).Thumbnail %>" />
                        </div>
                    <% } %>
                    <div style="clear:both"></div>
                </div>
            <% } %>


            <% if (item.Item is PxUnit && level == 1 && isInstructor && !disableEditing)
               { %>
                <div class="addContentBtn">
                    <div class="btn-wrapper gradient">
                        <a id="add-assignment-btn">Add to this Unit <span class="pxicon pxicon-down-open"></span></a>
                    </div>
                </div>
            <% } %>

            <% if (!isInstructor)
               { %>
            <% } %>
            <% ViewData["HiddenFromStudents"] = item.Item.HiddenFromStudents; %>
            <% if (!item.Item.ReadOnly)
               {%>
                <div class="faceplate-right-menu <%=sbManagementCardClass %>" id="faceplate-right-menu">
                </div>
                <%--<% Html.RenderPartial("~/Views/FacePlate/FacePlateGearboxMenu.ascx", item); %>--%>
            <% } %>
            <div style="clear: both">
            </div>
        </li>
    <% } %>
<% } %>