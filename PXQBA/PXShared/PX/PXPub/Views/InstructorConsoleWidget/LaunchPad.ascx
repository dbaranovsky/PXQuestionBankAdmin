<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LaunchPadSettings>" %>
<% 
    var viewType = (ViewData["ViewType"] == null) ? "" : ViewData["ViewType"].ToString().ToLower();
%>

<div class="settingNav-links" style="float: left; width: 10%">        
    <ul>
        <li>
        <%
            if (ViewData["View"].ToString() == "General")
            {
        %>
            <%= ViewData["View"].ToString() %>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/general" class="link">General</a>
        <%
            }
        %>
        </li><br />
        <li>
        <%
            if (ViewData["View"].ToString() == "Navigation")
            {
        %>
            <%= ViewData["View"].ToString() %>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/navigation" class="link">Navigation</a>
        <%
            }
        %>
        </li><br />
        <li>
        <%
            if (ViewData["View"].ToString() == "Launchpad")
            {
        %>
            <%= new MvcHtmlString("Launch Pad")%>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/launchpad" class="link">Launch Pad</a>
        <%
            }
        %>
        </li><br />
    </ul>
</div>


<div id="ConsoleRegion" class="instructor-console-settings" style="float: right; width: 80%">
    <div class="Title">
        <%= ViewData["View"].ToString() == "Launchpad" ? "Launch Pad" : ViewData["View"].ToString() %>
    </div>
        

     <% using (Ajax.BeginForm("UpdateLaunchPadSettings", "UpdateLaunchPadSettings", new AjaxOptions() { UpdateTargetId = "instructor-console-wrapper" }, new { id = "launchpadForm" }))
      { %>
        <%= Html.ValidationSummary(true)%>
        <%= Html.Hidden("View", ViewData["View"].ToString())%>

        <ul>
            <div class="launchpad-settings-view">
            <li class="title">Launch Pad units</li>
            <li><%= Boolean.Parse(ViewData["launchpadItemsIncluded"].ToString()) ? "You are using the publisher-provided Launch Pad units." : "You are not using the default publisher-provided units." %></li>
            <li class="category-name"><a id="launchpadItemsLink" href='' class='link'></a></li>
            
            <li class="title">Assigned items</li>
            <li><%= Html.CheckBoxFor(m => m.SortByDueDate) %> <span>Sort by due date (items within units will not be reordered)</span></li>
            
            <li class="second title">Past-due items items</li>
            <li class="second"><%= Html.CheckBoxFor(m => m.CollapsePastDue) %> <span>Collapse past-due items by default</span></li>
            
            <li class="second title">Items due later</li>
            <li class="second due-later" ><%= Html.CheckBoxFor(m => m.CollapseDueLater)%> <span>Collapse items due more than <%= Html.TextBoxFor(m => m.DueLaterDays)%> days in future by default</span></li>
            
            <li class="title">Unassigned items</li>
            <li ><%= Html.CheckBoxFor(m => m.CollapseUnassigned) %> <span>Collapse unassigned items by default</span></li>
            <li class="category-name"><span>Category name: <%= Html.TextBoxFor(m => m.CategoryName)%></span></li>
            
            </div>
            <%= Html.HiddenFor( m => m.ShowCollapseUnassigned)%>
            <%= Html.HiddenFor( m => m.GrayoutPastDueLater)%>
            <%= Html.HiddenFor( m => m.SplitAssigned)%>
            <%= Html.HiddenFor( m => m.CollapseUnassignedItems)%>
    
        </ul>

    <% } %>
    <div class="settingsSubmit-wrapper">
    <input type="submit" value="Save" id="submitlaunchPadForm" />
    <a href="#state/instructorconsole" class="link">Cancel</a>
    </div>
    </div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/CourseForm/CourseForm.js") %>'], function () {
                CourseForm.Init('0');
            });
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init('<%= Boolean.Parse(ViewData["launchpadItemsIncluded"].ToString()) %>');
            });
        });
    } (jQuery));    
</script>
    

