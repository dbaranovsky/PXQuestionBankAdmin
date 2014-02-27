<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.LearningObjective>>" %>
 
 <%
 
     bool isItemLocked = ViewData["isItemLocked"] != null ? Convert.ToBoolean(ViewData["isItemLocked"]) : false;
 %>

 <% 
     var learningObjectiveUrl = "<a href='#' id='LearningObjectiveTabLinkId' >Align Learning Objectives.</a>";
     var learningObjectiveUrlForLockedItem = "<a href='#' id='LearningObjectiveTabLinkId' >View Learning Objectives.</a>";  
 %>

<% if (Model.IsNullOrEmpty()) { %>
		This assignment does not yet have a learning objective.
        <% if(!isItemLocked) {%>
            <%= learningObjectiveUrl %>
        <%} %>
<% } else if (Model.Count() == 1 ){ %>
		There is <%= Model.Count() %> Learning Objective for this assignment.
        <% if(!isItemLocked) {%>
            <%= learningObjectiveUrl %>
        <%} %>
        <% else {%>
            <%= learningObjectiveUrlForLockedItem%>
        <%} %>
<% } %>

<% else if (Model.Count() > 1 ){ %>
		There are <%= Model.Count() %> Learning Objectives for this assignment.
        <% if(!isItemLocked) {%>
            <%= learningObjectiveUrl %>
        <%} %>
        <% else {%>
            <%= learningObjectiveUrlForLockedItem%>
        <%} %>
<% } %>
