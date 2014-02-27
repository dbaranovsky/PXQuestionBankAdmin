<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<%  
    var courseId = (ViewData["courseId"] == null ? Model.Content.GroupId : ViewData["courseId"].ToString());    
    var isAssigned = (Model.Content.DueDate.Year > DateTime.MinValue.Year);
    var btnAssignDisplay = (isAssigned) ? "none" : "inline";
    var btnSaveChangesDisplay = (isAssigned) ? "inline" : "none";
    // we hide the unassigned button when assigning specific group/individual
    var btnUnassignDisplay = (isAssigned && (Model.Content.GroupId == "groupIdValue" || Model.Content.GroupId == "" || courseId == Model.Content.GroupId)) ? "inline" : "none";
    %>
    
       <% if (Model.AssignedItem.IsReadOnly)%>
       <% { %>
	    <li class="assign-buttons">
            <br />
            <input type="button" id="btnAssign" value="Assign" class="assign btnAssignReadOnly" disabled="disabled" style="display: <%= btnAssignDisplay %>" />
            <input type="button" id="btnSaveChanges" class="assign btnSaveChangesReadOnly" value="Save Changes" style="display: <%= btnSaveChangesDisplay %>" disabled="disabled" />
            &nbsp;
            <input type="button" id="btnUnassign" class="assign btnUnassignReadOnly" value="Unassign" disabled="disabled" style="display: <%= btnUnassignDisplay %>" />
        </li>
    <% } 
       else if (!Model.AssignedItem.IsContentCreateAssign)
      { %>
    <li class="assign-buttons">
        <br />
        <input type="button" id="btnAssign" value="Assign" class="assign btnAssign submit-action" disabled="disabled" style="display: <%= btnAssignDisplay %>" />
        <input type="button" id="btnSaveChanges" class="assign btnSaveChanges submit-action" value="Save Changes" style="display: <%= btnSaveChangesDisplay %>" disabled="disabled" />
        &nbsp;
        <input type="button" id="btnUnassign" class="assign btnUnassign" value="Unassign" style="display: <%= btnUnassignDisplay %>" />
    </li>
    <% } %>
