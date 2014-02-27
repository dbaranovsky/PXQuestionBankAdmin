<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentToView>" %>
<div id="highlight-block-0" class="highlight-block">
    <div class="highlight-block-innerwrapper">
    <span id="spnLinkError" class="px-default-text" style="display: none;">Please enter a valid link</span> 
        <span class="block-controls">
        </span>
        <span class="pointer"></span>
        <div class="highlight-note-header"><span class="highlight-note-user"><%= ViewData["FormattedName"] %></span><span class="highlight-note-text"> says...</span> </div>
        <% using (Html.BeginForm("Create", "Highlight")) %>
        <% { %>
        <div class="highlight-comment-form">
            <div class="commentLibraryWrapper">
                <% Html.RenderAction("CommentLibraryDropList", "Highlight", new { });  %>
                <div>
                    <a href="#" class="add-to-note-library">Add to Note Library</a> | <a href="#" class="open-note-library">Open Note Library</a>
                </div>
            </div>
            <div class="linkLibraryWrapper">
                <% Html.RenderAction("QuickLinkList", "Highlight", new { }); %>
                <div>
                    <a href="#" class="more_eBook_links">More eBook links</a>
                </div>
                <%= Html.TextBox("CommentLink","",new {@maxlength="160", @text="http://", @class="commentLinkTextbox", @style="display:none;" })%>
                <input type="button"  name="commentLinkSubmit" class="commentLinkSubmit" value="OK" />                
            </div>
            <%= Html.Hidden("HighlightDescription", System.Web.HttpUtility.UrlDecode(Model.HighlightDescription)) %>
            <%= Html.Hidden("HighlightText", Model.HighlightText) %>
            <%= Html.Hidden("HighlightType", Model.HighlightType) %>
            <%= Html.Hidden("HighlightColor", Model.HighlightColor) %>
            <%= Html.Hidden("HighlightId", Model.HighlightId) %>
            <%= Html.Hidden("ItemId", Model.ItemId) %>
            <%= Html.Hidden("SecondaryId", Model.SecondaryId) %>
            <%= Html.Hidden("PeerReviewId", Model.PeerReviewId) %>
            <%= Html.Hidden("Locked", Model.Locked) %>
            <%= Html.Hidden("Shared", Model.Shared) %>
            <%= Html.Hidden("ShowRubrics", Model.ShowRubrics)%>
            <%= Html.Hidden("IsInstructor", Model.IsInstructor)%>
            <%
               var rubrics = (Model.RubricsGuide.IsNullOrEmpty()) ? string.Empty : string.Join("|",Model.RubricsGuide);
             %>
             <input type="hidden" id="RubricsGuide" value='<%=rubrics %>' name="RubricsGuide" />            
            <div class="highlight-middle-menu">
                <% if (Model.IsInstructor) { %>
                    <select class="highlight-public-private highlight-public-private-instructor">
                        <option value="1" <%= (Model.Shared)? "selected":"" %>>Public</option>
                        <option value="0" <%= (Model.Shared)? "":"selected" %>>Private</option>
                    </select>                        
                <% } else {%>
                    <select class="highlight-public-private highlight-public-private-student">
                        <option value="1" <%= (Model.Shared)? "selected":"" %>>Public</option>
                        <option value="0" <%= (Model.Shared)? "":"selected" %>>Private</option>
                    </select> 
                <% } %>                            
            </div>
            <div class="highlight-bottom-menu">                
                
                <%
               if (Model.ShowRubrics && Model.IsInstructor && (!Model.RubricsGuide.IsNullOrEmpty()))
               {
                   Html.RenderPartial("~/Views/EportfolioBrowser/RubricsList.ascx", Model.RubricsGuide);
               }%>                
                
                <input type="submit" title="Submit" name="submitButton" class="button primary small" value="Post" />                
                <span title="Cancel" class="cancel link button">Cancel</span><span class="commentLink" title="Add Link"></span><span class="commentLibrary" title="Add Comment"></span>              
                <div style="clear: both;"></div>
            </div>            
        </div>
        <% } %>
    </div>
</div>

