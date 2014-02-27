<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Student>>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/NoteWidget/NoteSetting.js") %>"></script>

 <%if (Model != null) { %>
  
<div id="noteSetting-Container">
    <div class="returnToNotes"><a id="retNotes" href="#">Return to Notes</a></div>    
    <div id="noteSettingList">
        <div class="searchContainer">
            <div class="shareTitle"><b>SHARE MY NOTES WITH:</b></div>            
            <input type="text" name="searchNote" id="searchNote"  style="width: 100%" />
            <input type="hidden" id="studentId" />
        </div>
        <div id="shareListContainer">
            <%
                if (Model.Count() > 0)
                {
                    Html.RenderPartial("ShareList", Model);
                }
                else
                {
            %>
                <label for="notShared">Your notes are not shared with anyone.</label> 
            <%} %>
        </div>
    </div>
</div>

<%} else { %>
    <div class="error">        
            Notes can not be shared in this course.
    </div>
<% }%>   