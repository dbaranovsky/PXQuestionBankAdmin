var ArgaApi = function ($) {
    return {
        argacomplete: function () {
            var itemid = $('#content-item .item-id').val();

            $(PxPage.switchboard).trigger('argacomplete', [{ itemid: itemid }]);
        }
    };
} (jQuery);
