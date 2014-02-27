<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TreeWidgetRoot>" %>
<%
    TreeWidgetSettings settings = Model.Settings;
    bool isInstructor = (settings.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor);
    bool sortByDueDate = settings.SortByDueDate;
    bool splitAssigned = settings.SplitAssigned;
    var scrollOnOpen = settings.ScrollOnOpen;
    var closeAllOnOpen = settings.CloseAllOnOpen;
    var launchpadTitle = settings.Title;
    var allowPastDue = settings.AllowPastDue;
    var allowDueLater = settings.AllowDueLater;
    var dueSoonDays = settings.DueLaterDays;
    var pastDueCount = settings.PastDueCount == 0 ? string.Empty : settings.PastDueCount.ToString();
    var dueLaterCount = settings.DueLaterCount == 0 ? string.Empty : settings.DueLaterCount.ToString();
    var showCollapseUnassigned = settings.ShowCollapsedUnassigned;
    var showExpandIcon = settings.ShowExpandIconAtAllLevels;
    var collapseUnassigned = settings.CollapseUnassigned;
    var collapseDueLaterByDefault = settings.AllowDueLater;
    var course = settings.CourseId;
    var collapsePastDueByDefault = settings.AllowPastDue;
    bool grayOutPastDueLater = settings.GreyoutPastDue;
    bool showWidgetTitles = settings.ShowWidgetTitles;
    var disableDnd = settings.AllowDragDrop;
    var allowEditing = settings.AllowEditing;
    var triggerOpenContentOnClick = settings.OpenContentOnClick;
    var entityId = settings.EntityId;
    var widgetId = settings.WidgetId;
    var isReadOnly = (settings.UserAccess != Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor || !allowEditing);

    bool enableDragAndDrop = !disableDnd.Equals("true");

    if (!pastDueCount.IsNullOrEmpty())
    {
        pastDueCount = "(" + pastDueCount + ")";
    }
    if (!dueLaterCount.IsNullOrEmpty())
    {
        dueLaterCount = "(" + dueLaterCount + ")";
    }

    if (!allowPastDue)
    {
        var cookie = new System.Web.HttpCookie(entityId + "hide_past_due");
        cookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(cookie);
    }

    if (!allowDueLater)
    {
        var cookie = new System.Web.HttpCookie(entityId + "hide_due_later");
        cookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(cookie);
    }

    var pastDueChecked = "";
    if (Request.Cookies.AllKeys.Contains(course + "hide_past_due") && Request.Cookies[course + "hide_past_due"].Value == "1")
    {
        pastDueChecked = "checked='true'";
    }
    var dueLaterChecked = "";
    if (Request.Cookies.AllKeys.Contains(course + "hide_due_later") && Request.Cookies[course + "hide_due_later"].Value == "1")
    {
        dueLaterChecked = "checked='checked'";
    }
    var collapseAllChecked = "";
    if (Request.Cookies.AllKeys.Contains(course + "collapse_all") && Request.Cookies[course + "collapse_all"].Value == "1")
    {
        collapseAllChecked = "checked='checked'";
    }

    var showWidgetTitleClass = " ";
    if (!showWidgetTitles)
    {
        showWidgetTitleClass = " hide";
    }

    var chapters_assigned = Model.Items.Where(c => c.Item.IsAssigned);
    var chapters_unassigned = Model.Items.Where(c => !c.Item.IsAssigned);

%>
<div class="unit-content-wrapper" id="launchpad-widget-<%=widgetId%>">

    <%
        if (Model != null && Model.Items.Count == 0 && !Model.Settings.ShowAssignmentUnitWorkflow)
        {
            if (Model.Settings.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
            {%>
    <div class="empty-launchpad">
        <div class="launchpad-msg">You have not yet added any content to this unit.</div>
        <div class="xbook-msg">
            <div id="empty-greeting">Hi! It looks like I have no assignments yet</div>
            <ul id="empty-steps">
                <li class="step">
                    <i>1</i>
                    <p>To get started create an assignment above then go to the X-Book tab</p>
                </li>
                <li class="step">
                    <i>2</i>
                    <p>Hover your mouse over an item in the table of contents and click Assign</p>
                </li>
                <li class="step">
                    <i>3</i>
                    <p>You can assign the content to a new assignment or youre previously created assignment.</p>
                </li>
            </ul>
        </div>
    </div>
    <%  }
            else if (Model.Settings.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
            {%>
    <div class="empty-launchpad">
        The Instructor has not added any assignments to this Course.
    </div>
    <%}
        } %>

    <div id="faceplate-unit-nav" class="unit-subitems-wrapper <%= (isInstructor) ? "instructor-view" : "student-view" %>">
        <div class="nav-category active" data-ud-id="PX_MULTIPART_LESSONS" style="float: left">
            <div class="launchpad-settings <%= splitAssigned && chapters_assigned.Any()? "":"hidden" %>">
                <% if (splitAssigned)
                   { %>
                <div class="launchpad-title fptitle <%=showWidgetTitleClass %>">Assigned</div>
                <% }
                   else if (!launchpadTitle.IsNullOrEmpty())
                   {%>
                <div class="launchpad-title fptitle <%=showWidgetTitleClass %>">
                    <%=launchpadTitle%>
                </div>
                <%} %>
                <% 
                    if (splitAssigned && allowPastDue)
                    { 
                %>
                <div class="past-due-switch">
                    <input type="checkbox" id="hide_past_due" <%= pastDueChecked %> />
                    <label data-on="Hide past due<%= pastDueCount %>" data-off="Show past due <%= pastDueCount %>"></label>
                </div>
                <%
                    } 
                %>
            </div>
            <ul id="faceplate-unit-subitems-<%=widgetId%>" class="faceplate-unit-subitems faux-tree unit-subitems-widget ebook faceplateitemlist faceplatefne">
                <% if (Model != null)
                   {
                       foreach (var child in splitAssigned ? chapters_assigned : Model.Items)
                       {
                           Html.RenderPartial("~/Views/LaunchpadTreeWidget/DisplayItem.ascx", new List<TreeWidgetViewItem>() { child });
                       }
                   } %>
            </ul>
            <%
                if (splitAssigned)
                {
            %>
            <div class="launchpad-settings <%= splitAssigned  && chapters_assigned.Any()? "":"hidden" %>">
                <% 
                    if (allowDueLater)
                    {
                %>
                <div class="due-later-switch">
                    <input type="checkbox" id="hide_due_later" <%=dueLaterChecked %> />
                    <label data-on="Hide due later<%=dueLaterCount %>" data-off="Show due later<%=dueLaterCount %>"></label>
                </div>
                <% 
                    }
                %>
            </div>
            <div class="launchpad-settings <%= splitAssigned  && chapters_assigned.Any()? "":"hidden" %>">
                <div class="launchpad-title fptitle">
                    <% if (!launchpadTitle.IsNullOrEmpty())
                       {%>
                    <div class="launchpad-title fptitle <%=showWidgetTitleClass %>">
                        <%=launchpadTitle%>
                    </div>
                    <%} %>
                </div>
                <div class="collapse-all-switch">
                    <input type="checkbox" id="collapse_all" <%=collapseAllChecked %> /><label data-on="Hide all unassigned"
                        data-off="Show all unassigned"></label>
                </div>
            </div>
            <ul id="faceplate-unit-subitems-unassigned-<%=widgetId%>" class="faceplate-unit-subitems-unassigned faux-tree unit-subitems-widget launchpad faceplateitemlist faceplatefne">
                <%
                    if (Model != null)
                    {
                        foreach (var child in chapters_unassigned)
                        {
                            Html.RenderPartial("~/Views/LaunchpadTreeWidget/DisplayItem.ascx", new List<TreeWidgetViewItem>() { child });
                        }
                    } %>
            </ul>
            <% } %>
        </div>
    </div>

</div>
<div class="faceplate-right-menu" id="faceplate-right-menu">
</div>

<script type="text/javascript">
    
    (function ($) {
        PxPage.Loading("#launchpad-widget-<%=widgetId%>", false, null, "Please Wait");
        PxPage.OnProductLoaded(function () {
            PxPage.Loaded("#launchpad-widget-<%=widgetId%>");
            $('#launchpad-widget-<%=widgetId%>').ContentTreeWidget({
                    readOnly: <%=isReadOnly.ToString().ToLowerInvariant() %>,
                    togglePastDue: <%=allowPastDue.ToString().ToLowerInvariant() %>,
                    toggleDueLater: <%=allowDueLater.ToString().ToLowerInvariant() %>,
                    enableDragAndDrop: <%=enableDragAndDrop.ToString().ToLowerInvariant() %>,
                    showCollapseUnassigned: <%=showCollapseUnassigned.ToString().ToLowerInvariant() %>,
                    collapseUnassigned: <%=collapseUnassigned.ToString().ToLowerInvariant() %>,
                    collapseDueLaterByDefault: <%=collapseDueLaterByDefault.ToString().ToLowerInvariant() %>,
                    collapsePastDueByDefault: <%=collapsePastDueByDefault.ToString().ToLowerInvariant() %>,
                    grayOutPastDueLater: <%=grayOutPastDueLater.ToString().ToLowerInvariant() %>,
                    dueSoonDays: <%=dueSoonDays.ToString().ToLowerInvariant() %>,
                    sortByDueDate: <%=sortByDueDate.ToString().ToLowerInvariant() %>,
                    courseNumber: <%=course %>,
                    splitAssignedUnassigned: <%= splitAssigned.ToString().ToLowerInvariant() %>,
                    triggerOpenContentOnClick: <%= triggerOpenContentOnClick.ToString().ToLowerInvariant() %>,
                    scrollOnOpen: <%= scrollOnOpen.ToString().ToLowerInvariant() %>,
                    closeAllOnOpen: <%= closeAllOnOpen.ToString().ToLowerInvariant() %>,
                    showExpandIcon: <%= showExpandIcon.ToString().ToLowerInvariant() %>,
                    toc: "<%= Model.Settings.TOC.ToLowerInvariant() %>",
                    assignmentToc: "<%= Model.Settings.AssignmentTOC.ToLowerInvariant() %>",
                    showAssignmentUnitFlow: <%= Model.Settings.ShowAssignmentUnitWorkflow.ToString().ToLowerInvariant() %>,
                    removeOnUnassign: <%= Model.Settings.RemoveOnUnassign.ToString().ToLowerInvariant() %>,
                    widgetId: "<%=widgetId %>"
                });
        });
    }(jQuery));
   
</script>
