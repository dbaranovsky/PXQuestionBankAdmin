<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>
<%--    <%  Html.RenderAction("GetHeader", "Grading", new { itemid = Model.Id, enrollmentid = Model.EnrollmentId }); %>
    <br />
    <div style="clear: both;">
    </div>--%>
<% 
    var bh1 = new BhComponent();
    if (Model.IsInstructor)
        bh1.ComponentName = "ItemDetails";
    else
        bh1.ComponentName = "SubmissionDetails";
    
    bh1.Parameters = new { EnrollmentId = Model.EnrollmentId, ItemId = Model.Id, entityid = Model.CourseInfo.Id };
    Html.RenderPartial("BhIFrameComponent", bh1);    
%>