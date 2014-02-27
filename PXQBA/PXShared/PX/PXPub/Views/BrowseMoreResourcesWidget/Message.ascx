<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div class="faceplate-browse-resources" >
    <div id="browseResultsPanel">
        <div class="close">
        </div>
        <div id="browseResults" class="px-default-text browseResults">
        </div>
    </div>
</div>


<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources();
        });
    } (jQuery));    
    


</script>