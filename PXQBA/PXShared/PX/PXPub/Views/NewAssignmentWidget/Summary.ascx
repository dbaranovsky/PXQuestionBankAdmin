<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Widget>" %>

<div id="newAssignment" class="newassignmentwidget">
<%  if (Convert.ToBoolean(ViewData["DisplayButton"]))
    { %>
    <div class="createassignment">
        <div id="createassignmentbutton" class="createassignmentbutton" ref="<%= Url.GetComponentHash("content", "assign") %>">
            Create Assignment
        </div>
    </div>
<%  } %>
</div>
<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/contentwidget.js") %>',
                            '<%= Url.ContentCache("~/Scripts/AssignmentWidget/CreateAssignmentWidget.js") %>'], function () {
                PxCreateNewAssignment.Init();
            });
        });

    }(jQuery));
</script>
