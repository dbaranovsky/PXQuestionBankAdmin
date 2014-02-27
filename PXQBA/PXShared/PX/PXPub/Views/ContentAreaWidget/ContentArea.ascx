<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentArea>" %>
<div class="contentPlayerHeader faceplate-nav" id="contentPlayerHeader">
    <span id="content-courseb-title"><%=  Model != null ? Model.Content.Title : ViewData["CourseTitle"]%></span>
    <span id="content-fullscreen" class="xb-btn"></span>
    <span id="content-fwd" class="xb-btn"></span>
    <span id="content-back" class="xb-btn"></span>
    <div id="eLibrary">
    <%  if (Model != null)
        { 
            Html.RenderAction("GetRelatedContents", "ContentAreaWidget", new { itemId = Model.Content.Id }); 
        }
    %>
    </div>
</div>
<div id="content-viewer">
<% 
if (Model != null)
{
    Html.RenderAction("DisplayItem", "ContentWidget", new { id = Model.Content.Id, mode = ContentViewMode.Preview, includeNavigation = false, readOnly = true});
}
%>
</div>
<script type="text/javascript">
  (function ($) {
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/ContentWidget/ContentAreaWidget.js") %>'], function () {
      PxContentAreaWidget.Init();
    });
  })(jQuery);
</script>