<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div class="faceplate-browse-resources" >
    <% Html.RenderPartial("ResourceList"); %>
</div>


<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
                $('.faceplate-browse-resources').FacePlateBrowseMoreResources();
        });
    } (jQuery));    
    


</script>