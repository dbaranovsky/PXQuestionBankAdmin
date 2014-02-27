<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    var parentStartDate = ViewData["parentStartDate"] != null ? ((DateTime)ViewData["parentStartDate"]) : DateTime.MinValue;
    var parentEndDate = ViewData["parentEndDate"] != null ? ((DateTime)ViewData["parentEndDate"]) : DateTime.MinValue;
    var itemStartDate = ViewData["itemStartDate"] != null ? ((DateTime)ViewData["itemStartDate"]) : DateTime.MinValue;
    var itemDueDate = ViewData["itemDueDate"] != null ?((DateTime)ViewData["itemDueDate"]) : DateTime.MinValue;
    var dateCategory = ViewData["dateCategory"] != null ? ViewData["dateCategory"] : "";
    
    var itemId = ViewData["itemId"];
    var type = ViewData["type"].ToString();

    var isDueDateSet = itemDueDate.Year > 1900;
    var userAccess = (Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["userAccess"];
%>
<div class="assign-set">
    
    <% if (userAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor) { 
        var showClockIcon = false;
    %>
        <input type="hidden" value="<%= parentStartDate.ToString("MM/dd/yyyy") %>" class="parent-start-date" />
        <input type="hidden" value="<%= parentEndDate.ToString("MM/dd/yyyy") %>" class="parent-end-date" />
        <input type="hidden" value="<%= itemStartDate.ToString("MM/dd/yyyy") %>" class="item-start-date" />
        <input type="hidden" value="<%= itemDueDate.ToString("MM/dd/yyyy") %>" class="item-due-date" />
        <input type="hidden" value="<%= DateTime.Now.ToString("MM/dd/yyyy") %>" class="current-date" />
        <input type="hidden" value="<%= itemId %>" class="item-id" />
        <input type="hidden" value="<%= type %>" class="item-type" />
        
        <a class="due-date-link">
        <% if (isDueDateSet)
        {
            var link = "";
            var start = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).StartOfDay();
            var end = DateTime.Now.EndOfWeek(DayOfWeek.Sunday).EndOfDay();
            showClockIcon = itemDueDate.InRange(start, end);
            if(type == "item")
            {
                link = itemDueDate.ToString("M/d");
            }
            else
            {
                link = itemStartDate.ToString("M/d") + "-" + itemDueDate.ToString("M/d");
            }
        %>
            <small> 
                <%=link%>
            </small>
        <% } else { %>
            <img src="<%= Url.Action("Index", "Style", new { path="images/b_calendar.png" }) %>" alt="Calendar" />
        <% } %>
        </a><% if (showClockIcon) { %><div class="assignclock" alt="Assigned with in the week" title="assigned"></div><% } %>
    <% } else { %>
    
        <%var submittedDate = ViewData["submittedDate"] == null ? DateTime.MinValue : (DateTime)(ViewData["submittedDate"]); %>
        <% if (submittedDate.Year > DateTime.MinValue.Year || dateCategory=="Completed")
           { %>
                <img alt="completed" style="height:15px;" src="<%= Url.Action("Index", "Style", new { path="images/Icon_Checkmark_Green_32.png" }) %>" />
        <%}
           else
           { %>
        <small> <%=Course.ShowBlankDateIfMinValue(itemDueDate)%></small>
        <%} %>
    <% } %>
</div>