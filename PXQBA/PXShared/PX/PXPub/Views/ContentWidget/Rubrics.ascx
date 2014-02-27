<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<% 
    ContentItem contentItem = Model.Content;

%>

<div id="content-assessment-tab">
<div id="assessment-content">
<div id="alignment-rubrics">
<% string selRubric = (ViewData["viewMode"] == null) ? "" : ViewData["viewMode"].ToString();
   if (!selRubric.IsNullOrEmpty() || Model.Content.IsItemLocked)
   {
      
        Html.RenderAction("RubricAlignment", "Rubric", new { id = Model.Content.Id, viewMode = "readonly" });
       
   }
   else
   {
       
        Html.RenderAction("RubricAlignment", "Rubric", new { id = Model.Content.Id });
       
   }%>
    
</div>

</div>
</div>

 <div class="rubricAlignModal">
        <div class="placeHolderRubric">
        </div>
    </div>


<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/Rubric/RubricManager.js") %>', , '<%= Url.ContentCache("~/Scripts/Rubric/RubricBuilder.js") %>', '<%= Url.ContentCache("~/Scripts/EportfolioLearningObjective/EportfolioLearningObjective.js") %>', '<%= Url.ContentCache("~/Scripts/jquery/jquery.fauxtree.js") %>'], function () {
                PxEportfolioLearningObjective.BindControls();
                PxRubric.Init();
            });
        });
    } (jQuery));
</script>

