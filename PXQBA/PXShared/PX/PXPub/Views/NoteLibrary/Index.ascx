<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Note>>" %>


 <%if (Model != null) { %>
<div id="notelibrarywrapper"> 
<div class="noteLibraryHeading">
    <div id="note_widget_action_menu" class="action_menu">
    </div>
    <label for="NoteLibrary" id="notetitle" >
        Note Library</label><br />
    <label for="NoteLibrary" id="selectprompt">
        Select to edit, drag to reorder.</label>
    <div style="height: 10px;">
    </div>    
    <div id="note_new">
    </div>
</div>
<div id="noteLibrary">
    <span id="notelist">
        <ul>
            <% Html.RenderPartial("NoteList", Model); %>           
        </ul>
    </span>
     <div style="float: right; margin-top: 15px; margin-bottom:15px;">
        <input type="button" id="btnClose" value="close" />
    </div>
</div>
<%} else { %>
    <div class="noElements">        
            Notes can not be created in this course.
    </div>
<% }%>   
</div>