<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AllProgress>" %>

<%
var parameters = new Dictionary<string, string>();
parameters.Add("EnrollmentId", Model.EnrollmentId);
parameters.Add("DisableWhatIfSwitch", "true");


BhComponent component = new BhComponent()
{
    ComponentName = "StudentGrades",
    Id = "gradebook-component",
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