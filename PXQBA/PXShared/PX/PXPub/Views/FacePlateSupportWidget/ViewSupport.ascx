<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% 
   var details = (ViewData["details"] == null) ? "" : ViewData["details"].ToString();
   var detailsPopup = (ViewData["details_popup"] == null) ? "" : ViewData["details_popup"].ToString();
   detailsPopup= detailsPopup.Replace("form", "<a href='http://support.bfwpub.com/supportform/form.php?View=contact' target='_blank'>form</a>");
   if (!details.IsNullOrEmpty() && !detailsPopup.IsNullOrEmpty())
   { %>
    <div class="facePlate-support">
            <div class="tech-support-more"><%=details %></div>
            <div id="helpdialog" style="display:none;"><%=detailsPopup %></div>
    </div>
    <script type="text/javascript">
        (function ($) {
            PxPage.OnReady(function () {
                $(document).off('click', ".tech-support-more").on("click", ".tech-support-more", function () {
                    $("#helpdialog").dialog({
                        modal: true, title: "Technical Support Form", draggable: true, closeOnEscape: true, height: 200, width: 450, resizable: true, autoopen:true,
                        close: function () {
                            $(this).dialog("close");
                        }
                    });
                });
            });
        } (jQuery));    
    </script>
    <% } %>