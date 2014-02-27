<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% var itemId = ViewData["itemId"]; %>
<div style="clear:both"></div>
<div id="objective-tags">
    <% Html.RenderAction("List", "LearningObjective", new { itemId = itemId }); %>
</div>

<a id="show-objective-links" class="correlatelink">CORRELATE LEARNING OBJECTIVES</a>

<div id="objective-form" style="display:none;">
<% Html.RenderAction("ObjectiveForm", "LearningObjective", new { itemId = itemId }); %>
</div>

<script type="text/javascript">
    PxPage.OnReady(function () {
            if (PxLearningObjectives) {
                PxLearningObjectives.Init();
            }        
    });
</script>