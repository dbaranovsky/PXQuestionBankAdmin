<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentArea>" %>

<div id="xb-documentviewer">
    <div id="content-px">

    <%Html.RenderPartial("ContentArea", Model, ViewData);%>
    
    </div>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/ContentWidget/ContentWidget.js") %>',
                '<%= Url.ContentCache("~/Scripts/highlight.js") %>'], function () {
                    ContentWidget.Init();
                    $(PxPage.switchboard).trigger('InitPersistentQtips', []);
                });
        });
    })(jQuery);
</script>