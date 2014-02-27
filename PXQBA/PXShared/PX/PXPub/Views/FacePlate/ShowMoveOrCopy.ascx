<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Widget>>" %>

<div id="PX_LAUNCHPAD_MOVECOPY_WIDGET" class="widgetItem px-movecopy-dialog-tree">
<%var sourceId = ViewData["sourceId"];
  var vd = new ViewDataDictionary();
  vd["sourceId"] = sourceId;
  vd["isHideCopy"] = ViewData [ "isHideCopy"];%>
  <input type="hidden" id="sourceId" value="<%=sourceId%>" />

<%
    foreach (var widget in Model)
    {
        Html.RenderAction("Index", "LaunchpadTreeWidget", widget);
    }
%>


</div>
    <input type="button" value="Move" onclick="return PxManagementCard.moveItem();" class="btnMoveItem primary button large" /> 
    <input type="button" value="Copy" onclick="return PxFacePlate.ShowCopyItemDialog();" class="btnCopyItem primary button large" />