<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%= Html.Label("Assignment:") %>
<%
    List<string[,]> assignments = new List<string[,]>();
    if (ViewData["assignments"] != null) { assignments = (List<string[,]>)ViewData["assignments"]; }
%>


    <%
        if (assignments.Count > 0)
        {
            %>
        <select id="dlAssignments">
        <%            
            foreach (string[,] assignment in assignments)
            {
                for (int i = 0; i < assignment.GetUpperBound(1); i++)
                {
                    %>
                    <option value="<%=assignment[i, 0]%>"><%=assignment[i, 1]%></option>
                    <%
                }
            }
            %>
        </select>
        <% } else { %>
        <select id="dlAssignments" disabled="disabled">
        <option value="" selected="selected">None</option>
        </select>
        <% } %>
