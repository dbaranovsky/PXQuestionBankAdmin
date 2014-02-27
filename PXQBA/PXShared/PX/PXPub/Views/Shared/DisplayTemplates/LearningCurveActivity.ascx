<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LearningCurveActivity>" %>
<%@ Import Namespace = "Bfw.PX.Biz.ServiceContracts"  %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {

            var deps = ['<%= Url.ContentCache("~/Scripts/LearningCurve/LearningCurve.js") %>','<%= Url.ContentCache("~/Scripts/LearningCurve/LearningCurveMoreResources.js") %>'].concat(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>);            
            PxPage.Require(deps, function () {
                PxQuiz.Init();
                $(".learningcurve-content").LearningCurve({modelId:'<%=Model.Id %>'});
            });
        });

    } (jQuery)); 
</script>

<div class="learningcurve-content">
    <div class="related-content-dialog">
        <div class="learning-curve-related-content">
            
        </div>
    </div>
    <% var isResultsView = ViewBag.Results ?? false;
       if (isResultsView)
       {
           Html.RenderPartial("LearningCurveResults", Model);
       }
       else
       {
    

            var hasParentLesson = ViewData["hasParentLesson"];
            var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                    {
                                        id = Model.Id,
                                        mode = ContentViewMode.Edit,
                                        hasParentLesson = hasParentLesson,
                                        includeNavigation = false,
                                        isBeingEdited = true
                                    });

            var showPlayerStudentView = ViewData["isStartActiviy"] == null ? false : (bool)ViewData["isStartActiviy"];
            double targetScore;
            double.TryParse(Model.TargetScore, out targetScore);
            var dueDatePassed = Model.DueDate < DateTime.Now;
           
            Html.RenderPartial("LearningCurvePlayer", Model);
        }
      %>  
</div>


