<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Submission>" %>
<div class="show-quiz submission-history">
    <input type="hidden" class="content-item-id" value="<%=Model.QuizId %>" />     
    <%  Html.RenderPartial("BhIFrameComponent", new BhComponent()
        {
            ComponentName = "SubmissionDetails",
            Parameters = new
            {
                EnrollmentId = Model.EnrollmentId,
                ItemId = Model.QuizId,
                Version = Model.Version > -1 ? Model.Version.ToString() : null
            }
        }); %>
</div>