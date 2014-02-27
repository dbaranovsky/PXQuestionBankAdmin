<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Widget>>" %>

<% 
    List<SelectListItem> templates = ViewData["templates"] == null ? new List<SelectListItem>() : ViewData["templates"] as List<SelectListItem>;    
    var questionId = ViewData["questionId"] == null ? "" : ViewData["questionId"].ToString();    
%>

<div>    
    <%= Html.Hidden("assessment-questionid", ViewData["questionId"]) %>
    <%= Html.DropDownList("assessment-type", templates) %>
    <input type="text" id="assessment-name" />
</div>

<div id="PX_LAUNCHPAD_MOVECOPY_WIDGET" class="widgetItem px-movecopy-dialog-tree">

<%
    foreach (var widget in Model)
    {
        Html.RenderAction("Index", "LaunchpadTreeWidget", widget);
    }
%>


</div>
    <input type="button" value="Save" onclick="return PxQuiz.AddToNewAssessment();" class="btnSave primary button large" /> 
    <a href="javascript:" onclick="return PxQuiz.CloseShowAddToNewAssessment();" class="btnCancel link">Cancel</a>

<%--<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/FacePlate/FacePlateDialog.js") %>'], function () {
                PxFacePlate.InitDialog();
            });
        });

    } (jQuery));    
</script>--%>




