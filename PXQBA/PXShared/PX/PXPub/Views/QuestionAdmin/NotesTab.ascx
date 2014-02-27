<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<QuestionNotes>" %>
<div id="NotesTab">
    <%
        if (Model.NoteList.Count > 0)
        {    
    %>
    <ul id="QBA-NoteList">
    <% foreach (var note in Model.NoteList)
       {           
    %>   
        <li>
            <%= note.FirstName%> <%= note.LastName%> added a note on <%= note.Created.ToString("MMM d, yyyy @ hh:mm tt") %>
            <br />
            <%= note.Text%>
        </li>   
    <%
       }
    %>
    </ul>
    <%
        }
        else
        {
    %>
        <div class="norecord">Currently there are no notes for the question.
        <br />
        Use Add Note button to add new note.</div>

    <%
        }
    %>
    <br />
    <br />
    <% 	Html.RenderPartial("AddNotes", new QuestionNote() { QuestionId = ViewData["questionId"].ToString() }); %>
</div>
