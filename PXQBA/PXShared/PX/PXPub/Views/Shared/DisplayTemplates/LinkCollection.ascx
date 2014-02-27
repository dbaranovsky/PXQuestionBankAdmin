<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LinkCollection>" %>
<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
                var deps = ['<%= Url.ContentCache("~/Scripts/LinkCollection/LinkCollection.js") %>', '<%= Url.ContentCache("~/Scripts/Assignment/Assignment.js") %>'];
                PxPage.Require(deps, function () {
                    PxAssignment.Init();
                    PxLinkCollection.BindControls();
                });
            });
    } (jQuery));    
</script>

<%var hasParentLesson = ViewData["hasParentLesson"];%>
<div class="link-collection-content-view">
    <h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
    <% if (!Model.ReadOnly)
       { %>
        <div class="menu edit-link">
            <%
                var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                        {
                                            id = Model.Id,
                                            mode = ContentViewMode.Edit,
                                            hasParentLesson = hasParentLesson,
                                            includeNavigation = false,
                                            isBeingEdited = true
                                        });    
                %>
                <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
        </div>
    <% } %>
</h2>
    <div class="html-container description-content"><%= Model.Description %></div>
    <fieldset id="link-collection">
    <h3 class="sub-title">Links</h3>
    <div class="link-table">
    <% 
        foreach (var link in Model.Links.OrderBy(o => o.Title))
        {
    %>
        <div class = "link-wrapper">
            <div id="dropboxSubmission">
                <div id="dropboxSubmissionDesc">
                    <div>
                        <span><a href="<%= link.Url %>" target="_newtab" class="document-list-table-a"><%= link.Title %></a></span>
                    </div>
             
                    <div>
                        <span><a href="<%= link.Url %>" target="_newtab" class="document-list-table-a link"><%= link.Url %></a></span>
                    </div>
          
                </div>
            </div>                
        </div>
    <%          
        }
    %>
  </div>
</fieldset>
</div>