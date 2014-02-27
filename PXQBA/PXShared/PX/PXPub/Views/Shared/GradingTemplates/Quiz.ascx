<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>
<% 
    var quizComponent = new BhComponent();

    quizComponent.ComponentName = Model.IsInstructor ? "ItemDetails" : "SubmissionDetails";

    quizComponent.Parameters = new { EnrollmentId = Model.EnrollmentId, ItemId = Model.Id, entityid = Model.CourseInfo.Id };

    Html.RenderPartial("BhIFrameComponent", quizComponent);    
%>