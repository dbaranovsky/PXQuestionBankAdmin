<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.FeaturedWidget>" %>

<div class="featuredwidget" contenttype="<%= Model.ContentType %>">
    <div class="description">
        <%= Model.Description %>
    </div>
</div>

<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/FeaturedWidget/FeaturedWidget.js") %>'], function () {
                PxFeaturedWidget.Init();
            });
        });

    } (jQuery));
</script>
