<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    IEnumerable<ContentItem> assignments = new List<ContentItem>();
    if (ViewData["assignments"] != null) 
    { 
        assignments = (IEnumerable<ContentItem>)ViewData["assignments"]; 
    }
%>

<div id="assignContent">
    <div class="content">
        <h2>Create Assignment</h2><br />
        <form>
            <input type="radio" name="rdAddignment" value="assign">Assign existing content <br />
            <input type="radio" name="rdAddignment" value="create">Create and assign a new content item <br />
        </form>

        <p class="lblDialog">Location</p>
        <select id="assignmentsDropDown">
            <option value="new" selected="selected">New Assignment</option>
        <%
            foreach( var assignment in assignments)
            {
                %>
                <option value="<%=assignment.Id%>"><%=assignment.Title%></option>
                <%
            }    
        %>
        </select>
    </div>
    <div class="button-wrap">
        <button id="btnNext" class="primary" ref="">Next</button>
        <button id="btnCancel" ref="#state/">Cancel</button>
    </div>
</div>

