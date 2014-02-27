<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GroupSetEditor>" %>

<div class="edit-groups">
<%
    var parameters = new Dictionary<string, string>();
        parameters.Add("EnrollmentId", Model.EnrollmentId);

        if (Model.GroupSetEditType == GroupSetEditor.EditType.Edit || Model.GroupSetEditType == GroupSetEditor.EditType.Clone)
        {
            parameters.Add("SetId", Model.GroupSetId.ToString());
        }

        if (Model.GroupSetEditType == GroupSetEditor.EditType.Clone)
        {
            parameters.Add("Clone", "true");
        }

    BhComponent component = new BhComponent()
    {
        ComponentName = "GroupSetup",
        Id = "frameviewitem",
        Parameters = parameters
    };
    Html.RenderPartial("BhIFrameComponent", component);
    %>

    <script type="text/javascript" language="javascript">
        (function ($) {
            PxPage.OnReady(function () {
                if (PxPage.FrameAPI) {
                    PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
                }
            });
        } (jQuery));   
    </script>

</div>