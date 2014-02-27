<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<%= Html.Hidden("courseFneTitle", "The course has been successfully activated!")%>

<% Html.RenderPartial("CourseActivationConfirmation", Model, ViewData); %>

<div class="activation-bookmark-btn">    <button id="unblock-action" class="divPopupClose">OK</button><span class="creation-url-descript">We recommend bookmarking this link to make it easy to return to your course.</span>  </div>


<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            $("#fne-title-action-right").show();
            //Standalone LC doesn't use LargeFNE
            if (PxPage.LargeFNE) {
                $('#unblock-action').on('click', PxPage.LargeFNE.CloseFNE);
            } else {
                $('#unblock-action').click(function() {
                    $('#fne-unblock-action').click();
                });
            }
            $('#fne-title-left #title-text').html('The course has been successfully activated.');
        }); 
    } (jQuery));    
</script>