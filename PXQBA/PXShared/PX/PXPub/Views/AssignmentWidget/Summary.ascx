<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignmentWidget>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Models" %>

<%
    var courseType = ViewData["CourseType"].ToString();
    var isStart = true;

    //isStart = ViewContext.ParentActionViewContext != null && (ViewContext.ParentActionViewContext.RouteData.Values["action"] == "Index" || ViewContext.ParentActionViewContext.RouteData.Values["action"] == "IndexStart");

    if (isStart)
    { 
%>        
<div id="schedule" class="assignment-widget">
        <table class="assignmentwidget-table" cellspacing="0" cellpadding="5px" border="0" style="width: 100%">
        <% 
            for (int gi = 0; gi < Model.Groups.Count; gi++)
            {
                var group = Model.Groups[gi];               
                
                if (!group.Title.ToLower().Equals("important")) { 
        %>

        <tr class="group <%= group.Title %>">
        <% 
            var titleClass = string.Empty;
            var assignmentFullDisplay = string.Empty;
            var assignmentCollapseDisplay = string.Empty;

            if (group.Assignments != null && group.Assignments.Count() > 0)
            {
                titleClass = "noCollapse";
                assignmentFullDisplay = "assignmentShow";
                assignmentCollapseDisplay = "assignmentHide";
            }
            else
            {
                titleClass = "Collapse";
                assignmentFullDisplay = "assignmentHide";
                assignmentCollapseDisplay = "assignmentShow";
            }               
        %>
            <td class="group-title <%= titleClass %>">
                
                <span class="collapseIcon" col="yes"></span>
                <h3><%= string.Format("{0}{1}", group.Title.ToLower().Equals("today") ? "" : string.Empty, group.Title) %></h3>                
            </td>
            <td class="group-empty">
                <%
                    if (group.Assignments == null || group.Assignments.Count() == 0)
                    { 
                %>
                        <div class="item-date"><em>There are currently no assignments due <%= string.Format("{0}{1}", group.Title.ToLower().Equals("today") ? "" : string.Empty, group.Title) %></em></div>
                <%
                    }
                %>
            </td>
        </tr>
        <tr>
            <td class="tblAssignmentFull <%= assignmentFullDisplay %>" colspan="2">
                <div style="overflow: auto; max-height: 100px; width: 100%">
                    <table width="100%">
                    <%
                        if (group.Assignments != null)                        
                        {
                        foreach (var assignment in group.Assignments)
                        { 
                    %>

                        <tr style="width:100%">                            
                            <td class="item-title">&nbsp;
                            <% 
                                var title = string.IsNullOrEmpty(assignment.Title) ? "No Title" : assignment.Title; 

                                if (courseType == CourseType.FACEPLATE.ToString())
                                { 
                            %>

                            <%= Url.GetComponentLink(HttpUtility.HtmlDecode(title), "item", assignment.Id, new { mode = ContentViewMode.Preview, includeDiscussion = false, renderFNE = true }, new { @class = "faceplatefne-assignmentwidget" })%>
                            
                            <%
                                }
                               else
                               { 
                                    if (!string.IsNullOrEmpty(assignment.SubType) && assignment.SubType.ToLowerInvariant() == "eportfolio")
                                    {
                            %>
                            <%= Html.RouteLink(title, "MyEportfolios", new { folderid = assignment.Id }) %>
                            <%
                                    }
                                    else if (!string.IsNullOrEmpty(assignment.Type) && assignment.Type.ToLowerInvariant() == "rssfeed")
                                    {
                            %>
                            <%= Html.ActionLink(title, "ShowRssPopup", "RSSFeed", new { id = assignment.Id }, new { @class = "fne-link" }) %>
                            <%
                                    }
                                    else
                                    { 
                            %>
                            <%= Html.ActionLink(title, "Index", "AssignmentCenter", new { assignmentID = assignment.Id }, new { @class = "assignmentLink" }) %>
                            <%
                                    }
                                }
                            %>
                            <span class="item-date">
                            <%= assignment.DueDateDisplay %>
                            </span>
                            
                            </td>
                        </tr>
                    <% 
                        } 
                        }
                    %>
                    </table>
                </div>
            </td>

        <% 
                }
            } 
        %>
        </tr>
    </table>
    <div class="assignmentViewAll" style="">
        Open calendar: <a href='#state/calendar/list' class="listView">List view</a> | <a href='#state/calendar/month' class="monthView">Month view</a>
    </div>
    </div>
        <%
    }
    else
    {
%>
<div id="schedule" class = "assignment-widget">
    <table class="assignmentwidget-table" cellspacing="0" cellpadding="5px" border="0" style="width: 100%">
        <% for (int gi = 0; gi < Model.Groups.Count; gi++)
           {
               var group = Model.Groups[gi];               
                %>
        <tr class="group <%= group.Title %>">
            <% var titleClass = "";
               if (group.Assignments.IsNullOrEmpty() || group.Assignments.Count() == 1)
               {
                   titleClass = "noCollapse";
               }

               var assignmentFullDisplay = "";
               var assignmentCollapseDisplay = "";
               if (!group.Assignments.IsNullOrEmpty() && group.Assignments.Count() == 1)
               {
                   assignmentFullDisplay = "assignmentShow";
                   assignmentCollapseDisplay = "assignmentHide";
               }
               else
               {
                   assignmentFullDisplay = "assignmentHide";
                   assignmentCollapseDisplay = "assignmentShow";
               }
               
               %>
            <td class="group-title <%= titleClass %>">
                <span class="collapseIcon" col="yes"></span>
                <h3><%= group.Title%></h3>
            </td>
            <td class="group-body">
                <% if (!group.Assignments.IsNullOrEmpty())
                   {    
                %>
                <table class="tblAssignmentFull <%= assignmentFullDisplay%>"  width="100%">
                    <%
                       foreach (var assignment in group.Assignments)
                       { %>
                    <tr>
                        <td class="item-title">
                            <% var title = string.IsNullOrEmpty(assignment.Title) ? "No Title" : assignment.Title;  %>

                            <% if (courseType == CourseType.FACEPLATE.ToString())
                               { %>
                                <%= Url.GetComponentLink(HttpUtility.HtmlDecode(title), "item", assignment.Id, new { mode = ContentViewMode.Preview, includeDiscussion = false, renderFNE = true }, new { @class = "faceplatefne-assignmentwidget" })%>
                            <%}
                               else
                               { %>

                            <%if (!string.IsNullOrEmpty(assignment.SubType) && assignment.SubType.ToLowerInvariant() == "eportfolio")
                              { %>
                            <%= Html.RouteLink(title, "MyEportfolios", new { folderid = assignment.Id })%>
                            <%}
                              else if (!string.IsNullOrEmpty(assignment.Type) && assignment.Type.ToLowerInvariant() == "rssfeed")
                              {%>
                                 <%=Html.ActionLink(title, "ShowRssPopup", "RSSFeed", new { id = assignment.Id }, new { @class = "fne-link" })%>
                            <%}
                              else
                              { %>
                               <%=Html.ActionLink(title, "Index", "AssignmentCenter", new { assignmentID = assignment.Id }, new { @class = "assignmentLink" })%>
                            <%}
                               }%>
                        </td>
                    </tr>
                    <% } %>
                </table>
                <table class="tblAssignmentCollapsed <%= assignmentCollapseDisplay %>">
                    <tr>
                        <td class="item-title">
                            <%= group.Assignments.Count().ToString() %> assignments due
                        </td>
                       
                    </tr>
                </table>
                 
                <%}
                   else
                   {%>
                   <div><%= group.DefaultDisplay%></div>
                  <% } %>
            </td>
            <td class="group-date">
                <% if (!group.Assignments.IsNullOrEmpty())
                   {    
                %>

                <table class="tblAssignmentFull <%= assignmentFullDisplay%>" width="100%">
                    <%
                    foreach (var assignment in group.Assignments)
                    { %>
                    <tr>
                        <td class="item-date">
                            <span class="date">
                                <%= assignment.DueDateDisplay%></span>
                        </td>
                    </tr>
                    <% } %>
                </table>
                <table class="tblAssignmentCollapsed <%= assignmentCollapseDisplay %>">
                    <tr>
                        <td class="item-date">
                            <%= group.FirstDefaultDate %>
                        </td>
                    </tr>
                </table>
                
                <%}%>
            </td>
                   
        </tr>
        
        <% } %>

    </table>
    <div class="assignmentViewAll" style="">
    <%= Model.All.Count().ToString() %> assignments remaining <%= Html.ActionLink("View all>", "Index", "AssignmentCenter", new { }, new { @class = "assignmentViewAllLink" })%>
    </div>
    <%
        var currentDate = DateTime.Now.ToString("MM/dd/yyyy");
       var nextWeek = Model.Groups.Where(g => g.Type == "nextweek").FirstOrDefault();
       string[] nextWeekArray = {""};
       if(nextWeek != null && nextWeek.Assignments != null)
       {
           nextWeekArray = nextWeek.Assignments.Map(a => a.DueDate.ToString("MM/dd/yyyy")).ToArray();
           currentDate = nextWeek.Assignments.First().DueDate.ToString("MM/dd/yyyy");
       }

       var thisWeek = Model.Groups.Where(g => g.Type == "thisweek").FirstOrDefault();
       string[] thisWeekArray = { "" };
       if(thisWeek != null && thisWeek.Assignments != null)
       {
           thisWeekArray = thisWeek.Assignments.Map(a => a.DueDate.ToString("MM/dd/yyyy")).ToArray();
           currentDate = thisWeek.Assignments.First().DueDate.ToString("MM/dd/yyyy");
       }

       var today = Model.Groups.Where(g => g.Type == "today").FirstOrDefault();
       string[] todayArray = { "" };
       if (today != null && today.Assignments != null)
       {
           todayArray = today.Assignments.Map(a => a.DueDate.ToString("MM/dd/yyyy")).ToArray();
           currentDate = today.Assignments.First().DueDate.ToString("MM/dd/yyyy");
       }
%>
    <input type="hidden" id="nextWeek" value="<%=string.Join(",",nextWeekArray) %>" />
    <input type="hidden" id="thisWeek" value="<%=string.Join(",",thisWeekArray) %>" />
    <input type="hidden" id="today" value="<%=string.Join(",",todayArray) %>" />
    <input type="hidden" id="currentDate" value="<%=currentDate %>" />
    <input type="hidden" class="customValues" value="Upcoming Assignments" />
</div>
<div class="px-calendar assign-widget-calendar" id="assignWidgetCalendar" style="display:none;"></div>
<% } %>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnProductLoaded(function(){
            var deps = ['<%= Url.ContentCache("~/Scripts/AssignmentWidget/AssignmentWidget.js") %>'];

            PxPage.Require(deps, function () {
                PxAssignmentWidget.Init(<%= isStart.ToString().ToLower() %>);
            });
        });
    } (jQuery));  
</script>

<span class="clear"></span>
