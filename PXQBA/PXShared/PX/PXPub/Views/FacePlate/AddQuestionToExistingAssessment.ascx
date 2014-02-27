<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Widget>>" %>

<div id="PX_LAUNCHPAD_ASSESSMENT_BROWSER_WIDGET" class="widgetItem px-assessment-dialog-tree">
<%var questionId = ViewData["questionId"];
  var vd = new ViewDataDictionary();
  vd["questionId"] = questionId;
  vd["isHideCopy"] = ViewData [ "isHideCopy"];%>
  <input type="hidden" id="question-id" value="<%=questionId%>" />
<%
    foreach (var widget in Model)
    {
        Html.RenderAction("Index", "LaunchpadTreeWidget", widget);
    }
%>

</div>
    <input type="button" value="Add" class="btnAddQuestion" style="display:none" /> 
<%--<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/FacePlate/FacePlateDialog.js") %>'], function () {
                PxFacePlate.InitDialog();
            });
        });

    } (jQuery));    
</script>--%>




