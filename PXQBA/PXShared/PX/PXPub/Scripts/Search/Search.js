var PxSearch = function ($) {
    return {
        BindControls: function () {
            $(document).off('focus', '#SearchBox').on('focus', '#SearchBox', function () {
                if ($('#SearchBox').val() == "Search course") {
                    $('#SearchBox').val("");
                }
            });

            $(document).off('focusout', '#SearchBox').on('focusout', '#SearchBox', function () {
                if ($('#SearchBox').val() == "") {
                    $('#SearchBox').val("Search course");
                }
            });
            
        },
        Init: function () {
            PxSearch.BindControls();
        }
    };
} (jQuery);