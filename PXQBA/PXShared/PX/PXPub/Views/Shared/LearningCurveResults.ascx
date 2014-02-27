<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LearningCurveActivity>" %>

<div>
    <h2 class="content-title" style="display: none;"><%= HttpUtility.HtmlDecode(Model.Title)%></h2>
</div>
<div class="learningCurveHorizontalRule"><hr /></div>
<% Html.RenderPartial("LearningCurvePlayer", Model); %>