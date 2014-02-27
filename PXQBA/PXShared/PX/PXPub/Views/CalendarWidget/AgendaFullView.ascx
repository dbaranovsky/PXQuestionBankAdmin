<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignmentWidget>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Models" %>

<%
    var viewType = (ViewData["ViewType"] == null) ? "Instructor" : ViewData["ViewType"].ToString().ToLower();
    var timeZone = ViewData["timeZone"].ToString();
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
        <a href="#state/calendar/list" class="link active">List</a>
        <a href="#state/calendar/month" class="link">Month</a>
    </div>
</div>

<div class="agenda-main">

<%
    var lastMonth = DateTime.MinValue;
    var lastDay = Model.ReferenceDate.AddDays(-1);
    var i = 0;
    
    foreach (var group in Model.Groups)
    {
        var date = DateTime.Parse(group.Type);
        var isMonthBeginning = date > lastMonth;
        lastMonth = date;

        if (isMonthBeginning)
        {
            i++;
        }
%>
        <table class="agenda-full">
        <tr>            
            <td class="title daylabel" <%= isMonthBeginning ? String.Format("id={0}", group.Title.Replace(" ", "_")) : string.Empty %> monthYear="<%= group.Type %>" <%= isMonthBeginning ? String.Format("sequence={0}", i.ToString()) : string.Empty %>>                
                <h3><%= group.Title.ToUpper() %></h3>                
            </td>
            <td class="item-date-col">
                </td>
                <td class="item-link-col">
                </td>
         
        </tr>
<%     
        if (group.Assignments == null)
        {
%>
            <tr class="item-row singleDay" <%= Model.Groups.First() == group ? "style='background-color: #FAFFCF;'" : "" %>>            
                <td class="daylabel">
                    <%= Model.Groups.First() == group ? "Today" : string.Empty %>
                </td>     
                <td class='item-date-col <%= Model.Groups.First() == group ? "emptycell" : "emptycell" %>'>
                    <span class="item-date"><%= Model.Groups.First() == group ? "There are currently no assignments due today." : group.DefaultDisplay%></span>
                </td>
                <td class="item-link-col">
                </td>
                <td class="item-type-col">                    
                </td>   
            </tr>
<%
        }
        else
        {            
            foreach (var assignment in group.Assignments)
            {
                var q = group.Assignments.Where(a => a.DueDate.ToString("MM/dd/yyyy").Equals(assignment.DueDate.ToString("MM/dd/yyyy"))).Count();

                var className = "";

                if (q == 1)
                {
                    className = " singleDay";
                }
                else if (group.Assignments.Where(a => a.DueDate.Equals(assignment.DueDate)).First().Id == assignment.Id)
                {
                    className = " multiDay top";
                }
                else if (group.Assignments.Where(a => a.DueDate.Equals(assignment.DueDate)).Last().Id == assignment.Id)
                {
                    className = " multiDay bottom";
                }
                else
                {
                    className = " multiDay middle";
                }
            
%>                
            <tr class="item-row<%= className %>" <%= Model.Groups.First() == group && DateTime.Now.ToShortDateString().Equals(assignment.DueDate.ToShortDateString()) ? "style='background-color: #FAFFCF;'" : "" %>>
                <td class="daylabel">
                    <%= 
                        lastDay.ToShortDateString() == assignment.DueDate.ToShortDateString() ? string.Empty :
                        DateTime.Now.ToShortDateString().Equals(assignment.DueDate.ToShortDateString()) ? "Today" : String.Format("{0}, {1}", assignment.DueDate.DayOfWeek.ToString().Substring(0, 3), assignment.DueDate.ToString("MMM d"))
                    %>
                </td>     

                <td class="item-date-col">
                <%
                    TimeSpan ts = assignment.DueDate.Subtract(DateTime.Now.GetCourseDateTime());
                    var dueDateDisplayList = "";
                    if (ts.Days < 1 && ts.Hours >= 1)
                    {
                        dueDateDisplayList = string.Format("in {0} hours", ts.Hours);
                    }
                    else if (ts.Days < 1 && ts.Minutes >= 1)
                    {
                        dueDateDisplayList = string.Format("in {0} minutes", ts.Minutes);
                    }
                    else if (ts.Days < 1 && ts.Seconds >= 1)
                    {
                        dueDateDisplayList = string.Format("in {0} seconds", ts.Seconds);
                    }
                    else
                    {
                        dueDateDisplayList = String.Format("{0} {1}", assignment.DueDate.ToString("h:mm tt"), timeZone);
                    }
                     %>
                    <span class="item-date"><%= assignment.Title == null ? string.Empty : dueDateDisplayList %></span>
                </td>    
                        
                <td class="item-link-col">
<%
                var title = string.IsNullOrEmpty(assignment.Title) ? "&nbsp;" : assignment.Title.Truncate("...", 0, 100);                               
%>
                <a href='javascript:' class="tooltip" points="<%= assignment.MaxPoints %>" due="<%= assignment.DueDate %>" id="<%= assignment.Id %>" type="<%= assignment.Type %>" category="<%= assignment.Category %>"><%= title %></a>

                    <span class="tooltipLink" style="display: none" id="<%= assignment.Id %>">
                        <%= Url.GetComponentLink(title, "item", assignment.Id, new { mode = ContentViewMode.Preview, includeDiscussion = false, renderFNE = true })%>
                    </span>
                     <span class="editLink" style="display: none" id="<%= assignment.Id %>">
                    <%= Url.GetComponentLink(title, "item", assignment.Id, new { mode = ContentViewMode.Assign, includeDiscussion = false, renderFNE = true })%>
                </span>

                <span class="item-date-col item-date"><%= assignment.FriendlyNameSourceType %></span>                                 

                </td>  
            </tr>
<%
                lastDay = assignment.DueDate;
            }
        }
%>        
        </table>
<%
    }    
%>    

</div>

<script type="text/javascript">
    (function ($) {
        
        PxPage.OnReady(function () {
            var deps = (['<%= Url.ContentCache("~/Scripts/CalendarWidget/CalendarWidget.js") %>', '<%= Url.ContentCache("~/Scripts/jquery/jquery.fauxtree.js") %>']);
            
            PxPage.Require(deps, function () {
                CalendarWidget.Init("agenda", 0, '<%= viewType %>');
            });
        });
    } (jQuery));    
</script>