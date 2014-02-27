<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Note>>" %>
<%
    var isInstructor = (bool) ViewData["HLM_IsInstructor"];
    var currentUserId = (string)ViewData["CurrentUserId"];
    var locked = (bool) ViewData["HLM_Locked"];
    var count = 0;
    foreach (var hc in Model) 
    { 
        count++;
        var classes = hc.IsUserNote ? " mine" : "";
        classes += locked ? " locked" : "";  
%>
<div class="highlight-comment<%=(Model.Count() == count) ? " lastComment" : ""%><%=classes %>" id="note-<%=hc.NoteId %>">
    <input name="note-user-id" type="hidden" value="<%= hc.UserId %>" />
        <div class="block-controls">
            <%if (isInstructor)
              {%>
                    <div class="highlight-note-action-menu action pxicon pxicon-gear">
                        <div class="highlight-note-action-menu-list" style="display:none;">
                            <span class="highlight-note-action-menu-item edit">Edit</span>
                            <%if (locked)
                              { %>
                                <span class="highlight-note-action-menu-item lock" style="display:none">Unlock Note</span>
                            <%}
                              else
                              { %>
                                <span class="highlight-note-action-menu-item unlock" style="display:none">Lock Note</span>
                            <%} %>
                            <span class="highlight-note-action-menu-item delete">Delete</span>                    
                        </div>
                    </div>                
            <%}
              else
              { %>
                  <%if (locked)
                  {%>
                    <span class="lockedIcon" title="locked"></span>
                <%}
                    else if (hc.UserId == currentUserId)
                  { %>
                    <div class="highlight-note-action-menu action pxicon pxicon-gear">
                        <div class="highlight-note-action-menu-list" style="display:none;">
                            <span class="highlight-note-action-menu-item edit">Edit</span>
                            <span class="highlight-note-action-menu-item delete">Delete</span>                    
                        </div>
                    </div>
                <%} %> 
            <%} %>
       
        </div>
    <span class="author"><%= hc.FirstName %> <%= hc.LastName %> </span>
    <span class="note-date">
        <% 
            DateTime tempDateTime = (hc.ModifiedDate == DateTime.MinValue) ? DateTime.Now : hc.ModifiedDate;
            DateTime courseDateTime = tempDateTime.GetCourseDateTime();
            String DisplayDateTime = (tempDateTime.Date == DateTime.Today.Date) ? "Today, " + courseDateTime.ToString("t") : courseDateTime.ToString("g"); 
        %>
        <%= DisplayDateTime %>
    </span>

    <div class="note"><%= hc.Text %></div>
</div>
<% } %>