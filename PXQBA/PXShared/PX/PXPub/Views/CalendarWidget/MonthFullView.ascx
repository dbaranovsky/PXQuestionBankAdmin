<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignmentWidget>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Models" %>

<%
    var viewType = (ViewData["ViewType"] == null) ? "Instructor" : ViewData["ViewType"].ToString().ToLower();
    var dueLater = (ViewData["DueLater"] == null) ? 0 : Convert.ToInt32(ViewData["DueLater"]);
%>

<div class="toolbox">
    <div class="next-back-container">
        <span id="nav-container">
            <span id="back" class="calendar-navigate-back">
                <span class="navbtn-back-icon"></span>
            </span> 
            <span id="next" class="calendar-navigate-next">
                <span class="navbtn-next-icon"></span>
            </span> 
        </span>
        <span id="currentSelection"></span>
        <a href="#" id="todaySelector">Today</a>
    </div>  
    <div class="assignments-nav">
        <% 
            if (viewType.ToLower() == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor.ToString().ToLower())
            {
                Html.RenderPartial("~/Views/Shared/GroupSelector.ascx", new ViewDataDictionary() { { "GroupSelectorLabel", "Schedule for" }, { "OnChangeEvent", "CalendarWidget.ChangeGroup(event)" } });
            }
        %>
        <a href="#state/calendar/list" class="link">List</a>
        <a href="#state/calendar/month" class="link active">Month</a>
    </div>
</div>

<div class="agenda-main">
    <div id="calendar"></div>
</div>

<script type="text/javascript">
    //(function ($) {
        PxPage.OnReady(function () {
            var deps = ['<%= Url.ContentCache("~/Scripts/CalendarWidget/CalendarWidget.js") %>', 
                '<%= Url.ContentCache("~/Scripts/fullcalendar/fullcalendar.js") %>', 
                '<%= Url.ContentCache("~/Scripts/jquery/jquery.fauxtree.js") %>'];

            PxPage.Require(deps, function () {
                CalendarWidget.Init("month", <%= dueLater %>, '<%= viewType %>');
            });
        });
    //} (jQuery));    
</script>