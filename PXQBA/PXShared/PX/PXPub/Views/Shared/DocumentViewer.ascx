<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentToView>" %>

<script type="text/javascript">
    $("#document-body-iframe").load(function () {
        $("#document-body").height($('#document-body-iframe').contents().height());
        $("#highlight-container").height($('#document-body-iframe').contents().height());
    });
</script>
<%
    if (Model.IsBinary)
    {//binary file (PDF, PPT, etc), load intesticial page
        %>
        <div id="document-viewer" class="document-viewer readonly">
            <%Html.RenderPartial("DocumentFileDownload", Model);%>   
        </div>
         <%   
        return;
    }
    
    
    if (Model.IsProductCourse)
    {
        Model.AllowComments = false; // comments are not allowed in product  course.
    }
    var isLearningCurveResourceDialog = ViewData["isLCResourceDialog"]; 
%>

<div id="document-viewer" class="document-viewer <%= (Model.IsReadOnly || !Model.AllowComments) ? " readonly":""%> <%=Model.AllowComments? " allowComments":"" %> ">
    <% if (Model.AllowComments)
       { %>
    <div id="highlight-container">
        <div id="highlight-new-container" class="highlight-new-container" style="display: none;">
            <% Html.RenderAction("NewHighlightForm", "Highlight", Model); %>
        </div>
        <div id="highlightList">
            <%--client call will get notes and highlights and add notes in this div--%>
        </div>
    </div>
    <% } %>
    <div id="document-body" class="document-body">
        <div id="content-loading" class="content-loading" style="display: none; margin-top: 10px; height: 20px;">
            <div id="loadingBlock"></div>
        </div>
        <%      
    
            Model.Url = HttpUtility.HtmlDecode(Model.Url);
            Model.ExternalUrl = HttpUtility.HtmlDecode(Model.ExternalUrl);

            var rand = new Random();
            var frameHostId = string.Format("document-viewer-frame-host-{0}", rand.Next());
            if (Model.AllowComments)
            {

                if (isLearningCurveResourceDialog == null)
                {%>
                    <div id="<%= frameHostId %>" rel="<%= Model.Url %>" class="document-viewer-frame-host"></div>
            <% }
                else
                { %>
                    <iframe id="lc-frame" src="<%= Model.Url %>" height="100%" width="100%"></iframe>
            <% } %>

        <% }
            else
            {
                if (Model.Url == "comingsoon.html") // if there is not preceeding slash, whole page loads in from with master page
                {
                    Model.Url = "/comingsoon.html";
                }
        
                if (Model.IsExernalContent)
                { %>
                    <div id="<%= frameHostId %>" rel="<%= Model.Url %>" class="document-viewer-frame-host"></div>
            <%  }
                else
                {
                    if (Model.Url.IsNullOrEmpty())
                    {
            %>
                        <div class="no-description" style="margin-top: 20px">You have not yet provided a URL to load. Click "Edit" and choose "Basic Info" to specify a URL.</div>
                <%
                    }
                    else
                    { 
                %>
                        <iframe id="document-body-iframe" src="<%= Model.Url %>" frameborder="0" style="width: 100%; height: 680px; min-height: 600px; overflow: auto" name="iframe" class="proxyFrame static-height"></iframe>
            <% 
                    }
                } 
            %>
        <% } %>
    </div>


    <input type="hidden" id="AllowComments" value="<%=Model.AllowComments%>" />
    <input type="hidden" id="IsExternalContent" value="<%=Model.IsExernalContent%>" />


    <script type="text/javascript">

    <%
        if (Model.AllowComments)
        {
    %>
        bfw_itemId = '<%= Model.ItemId %>';
        bfw_secondaryId = '<%= Model.SecondaryId %>';
        bfw_reviewId = '<%= Model.PeerReviewId %>';
        bfw_commenterId = '<%= Model.CommenterId %>';
        bfw_highlightType = '<%= Model.HighlightType %>';
        bfw_highlightDesc = "<%= System.Web.HttpUtility.UrlEncode(Model.HighlightDescription) %>";
        bfw_itemUrl = '<%= System.Web.HttpUtility.HtmlEncode(Model.Url) %>';
        bfw_isAssignmentView = '<%= Model.isAssignmentView %>';
        bfw_isCurrentUserView = '<%= Model.IsCurrentUserContext %>';
        bfw_enrollmentId = '<%= Model.CommenterId %>';
        bfw_highlightId = '<%= Model.HighlightId %>';
        bfw_ProxyUrl = '<%= Model.HighlightId %>';

    <%
        }
    %>

        (function ($) {
            PxPage.OnReady(function () {
                //safe to use $, safe to use PxPage

                var iframe = $("#document-body-iframe");
                var allowComments = $("#AllowComments").val();

                // this each loop ensures that  PxComments.Init() is called after the iframe is loaded, else it creates cross domain issue
                iframe.each(function () {
                    $(PxPage.switchboard).trigger("document-body-iframe-loaded");
                    
                    if (this.attachEvent) {
                        this.attachEvent("onload", function () {
                            if (allowComments && allowComments.toLowerCase() === "true") {
                                PxComments.Init('');
                            }
                        });
                    } else {
                        this.onload = function () {
                            if (allowComments && allowComments.toLowerCase() === "true") {
                                PxComments.Init('');
                            }
                        };
                    }
                });

            });
        }(jQuery));
    
    </script>
    <div style="clear: both;"></div>
</div>
