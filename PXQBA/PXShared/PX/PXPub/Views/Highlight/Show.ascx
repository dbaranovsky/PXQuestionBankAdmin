<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HighlightModel>" %>

<%
    var isTopNote = false;
    var highlightId = "";
    
    if (String.IsNullOrEmpty(Model.Text))
    { 
        isTopNote = true;
        // This is a temporary solution, should change to better way to handle topnotes.
        if(Model.Notes.Count > 0)
        {
            highlightId = Model.Notes.FirstOrDefault().NoteId;
        }
    }
    else
    {
        highlightId = Model.HighlightId;
    }
    
    var locked = Model.Status == Bfw.PX.Biz.DataContracts.HighlightStatus.Locked;
    var classes = Model.Public ? " public" : " private";
    classes += isTopNote ? " page" : "";
    classes += Model.IsUserHighlight ? " mine" : "";
    classes += locked ? " locked" : "";
    ViewData["HLM_HighlightType"] = Model.HighlightType;
    ViewData["HLM_Public"] = Model.Public;
    ViewData["CurrentUserId"] = ViewData["CurrentUserId"] ?? Model.UserId;
    ViewData["HLM_NoteType"] = Model.NoteType;
    ViewData["HLM_IsInstructor"] = Model.IsInstructor;
    ViewData["HLM_Locked"] = Model.Status == Bfw.PX.Biz.DataContracts.HighlightStatus.Locked;
    
%>
 <div id="highlight-block-<%= highlightId%>" class="highlight-block <%=classes %>">
     <div class="highlight-block-innerwrapper">
        <div id="commentListDiv-<%= highlightId %>" class="highlight-body note-list-box">
            <% Html.RenderPartial("CommentList", Model.Notes);  %>
            <div class="note-description" style="display:none"></div>
        </div>
        <% using (Html.BeginForm("AddComment", "Highlight", FormMethod.Post, new { id = "form-" + highlightId })) %>
        <% { %>

            <div class="highlight-comment-form">                
                <div class="commentLibraryWrapper"><% Html.RenderAction("CommentLibraryDropList", "Highlight", new { });  %>
                    <div><a href="#" class="add-to-note-library">Add to Note Library</a> | <a href="#" class="open-note-library">Open Note Library</a></div>
                </div>
                <div class="linkLibraryWrapper">
                    <% Html.RenderAction("QuickLinkList", "Highlight", new { });  %>
                    <div>
                        <a href="#" class="more_eBook_links">More eBook links</a>
                    </div>
                    <%= Html.TextBox("CommentLink","",new {@maxlength="160", @text="http://", @class="commentLinkTextbox", @style="display:none;" })%>
                    <input type="button"  name="commentLinkSubmit" title="Submit Link" class="commentLinkSubmit" value="OK" />
                    
                </div>
                <%= Html.Hidden("highlightId", highlightId)%>
                <%= Html.Hidden("isPublic", Model.Public)%>
                <%= Html.Hidden("status", Model.Status)%>
                <%= Html.Hidden("itemId", Model.ItemId) %>
                <%= Html.Hidden("secondaryId", Model.EnrollmentId) %>
                <%= Html.Hidden("reviewId", Model.ReviewId)%>
                <%= Html.Hidden("highlightType", Model.HighlightType)%>
                <%= Html.Hidden("highlightDescription", Model.Description)%>
                <%= Html.Hidden("color", Model.Color) %>
                
                <div class="highlight-middle-menu">

                   <% if (Model.HighlightType == Bfw.PX.Biz.DataContracts.PxHighlightType.GeneralContent && Model.AllowShareComments) { %>
                         <% if (Model.IsInstructor) { %>
                               <select class="highlight-public-private highlight-public-private-instructor">
                                    <option value="1" <%= (Model.Public)? "selected":"" %>>Public</option>
                                    <option value="0" <%= (Model.Public)? "":"selected" %>>Private</option>
                               </select>                        
                         <% } else if(Model.IsUserHighlight) {%>                            
                               <select class="highlight-public-private highlight-public-private-student">
                                    <option value="1" <%= (Model.Public)? "selected":"" %>>Public</option>
                                    <option value="0" <%= (Model.Public)? "":"selected" %>>Private</option>
                               </select> 
                         <% } %>                        
                   <% } %>               
                    
                </div>
                <div class="highlight-bottom-menu ignore-autosave">
                     <input type="button" title="Submit" name="submitButton" class="comment-submit button primary small" value="Post" />                
                    <span title="Cancel" class="cancel">Cancel</span><span class="commentLink" title="Add Link"></span><span class="commentLibrary" title="Add Comment"></span>
                    <div style="clear:both;"></div>
                </div>
            </div>
        <% } %>
    </div>
</div>