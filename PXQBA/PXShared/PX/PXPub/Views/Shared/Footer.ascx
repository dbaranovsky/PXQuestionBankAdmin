<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    (function ($) {
        $(function () {
            $('.fne-footer-link').click(function () {
                var href = $(this).attr('href');
                $('<div><p class="popup-content"></p></div>').dialog({
                    autoOpen: true,
                    modal: true,
                    height: 400,
                    width: 600,
                    open: function () {
                        $(this).find('.popup-content').load(href);
                        $('.ui-dialog-title').text('');
                    },
                    close: function () { $(this).dialog('destroy'); },
                    overlay: { opacity: 0.5, background: 'black' }
                });
                return false;
            });
        });
    } (jQuery));
</script>

<div id="footer">
     <a  href="http://www.macmillanhighered.com/Catalog/other/privacy" target="_blank" >Privacy Policy</a>
     <a href="http://www.macmillanhighered.com/catalog/other/terms" target="_blank" >Terms of Use</a>
     <a href="http://www.macmillanhighered.com/catalog/other/accessibility" target="_blank" >Accessibility</a>
     <a href=" http://support.bfwpub.com/index.php?View=entry&EntryID=18 " target="_blank" >Refund Policy</a>
     <a class="system-requirements" href="http://cmg.screenstepslive.com/s/3918/m/11562/l/121193-what-are-the-minimum-system-requirements-for-macmillan-media" target="_blank" >System Requirements</a>
     <a class="customer-service" href="http://cmg.screenstepslive.com/s/3918/m/11562/l/131292-how-do-i-get-in-touch-with-tech-support" target="_blank" >Customer Support</a>
    <span class="footer-copyright">© 2013 Macmillan. All rights Reserved</span>
        <div id="platformX-logo"></div>
</div>
