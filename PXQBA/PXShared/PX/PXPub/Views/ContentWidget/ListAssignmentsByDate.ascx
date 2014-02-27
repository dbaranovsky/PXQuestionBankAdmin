<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var courseType = null != ViewData["CourseType"] ? ViewData["CourseType"].ToString() : string.Empty; %>
<div id="assignment-list">
    <div class="assignment-date-wrapper">
<% var assignedItems = ViewData["assignments"] as IEnumerable<AssignedItem>;
   var fromDate = DateTime.Parse(ViewData["fromDate"].ToString());
   var toDate = DateTime.Parse(ViewData["toDate"].ToString());          
    %>
   Other assignments due <%=fromDate.ToLongDateString() %>:
<%
if (assignedItems.IsNullOrEmpty()) { %> 
    </div>
     <ul class="assigned-items">   
        <span class="px-default no-assignment">No other assignments due.</span>  
    </li>
    </ul>  
    <%
     }
       else   
   { 
%>
    </div>
     <ul class="assigned-items">
     <% foreach (var item in assignedItems)
        {
            var assignmentTitle = HttpUtility.HtmlDecode(item.Title);
            assignmentTitle = Regex.Replace(item.Title, "<.*?>", string.Empty);
            %>
         <li>
         <%
            if (courseType.Equals("faceplate"))
            {
                var fnelink = Url.GetComponentLink(item.Title, "item", item.Id,
                                                               new
                                                               {
                                                                   mode = ContentViewMode.Preview,
                                                                   includeDiscussion = false,
                                                                   renderFNE = true
                                                               },
                                                               new
                                                               {
                                                                   @title = item.Title
                                                               });
             
              %>
              <%= fnelink%>
         <% }
            else
            { %>
                 <%= Html.ActionLink(assignmentTitle, "Index", "Content", new { id = item.Id }, new { target = "_blank" })%>
            <%} %>
            </li>
     <% } %>
     </ul>
<% } %>
</div>
<span style="display:block; clear: both;"></span>