
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RSSFeedWidget>" %>

<div class="customEditWidget"  dialogWidth="400" dialogHeight="225" dialogMinWidth="400" dialogMinHeight="225" dialogTitle="RSS Feed">
    <div class="feedURLContainer">
        <div class="dialogLabel">Paste feed URL here: </div>
        <div class="FeedURL">
            <input class = "InputForControllerAction" type = "text" name="RssFeedUrl" id="RssFeedUrl" size = "60"  value="<%= Model.RSSUrl %>"/>
        </div>
    </div>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
                PxRssArticle.BindControls();
            });
        });

    } (jQuery));    
</script>
