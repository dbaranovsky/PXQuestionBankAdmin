<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>

<select id="selassignmentunits" name="selassignmentunits" class="selassignunits">
    <option value="selassignment" selected="selected">Select Assignment</option>
    
    <%
        if (Model.AssignmentUnits != null)
        {
            foreach (var unit in Model.AssignmentUnits)
            {
                var unitSelection = (unit.Selected) ? "selected='selected'" : String.Empty;
    %>

    <option value="<%= unit.Id %>" <%= unitSelection %> data-category="<%= unit.CategoryId %>"><%= unit.Title %></option>
    
   <%
            }
        }
       %>
</select>