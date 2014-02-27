PxPage.Loader = (function ($) {
    var _static = {
        defaults: {},
        css: {},
        sel: {}


    };


    return {
        HideFullScreenLoader: function () {
            if ($.active > 1 || $.ajax.active > 1) {
                $("body").ajaxStop(function () {
                    if ($.active == 0 || $.ajax.active == 0 && $(".loading-overlay").length) {
                        $(".loading-overlay").fadeOut(1000, function () {
                            $(".single-column").fadeIn(1000);
                            $(".loading-overlay").remove();
                        });
                    }
                });
            } else {
                $(".loading-overlay").fadeOut(1000, function () {
                    $(".single-column").fadeIn(1000);
                });
            }
        },
        Init: function () {
            if (HashHistory) {
                $(PxPage.switchboard).one("hashinitialized", function() {
                    PxPage.Loader.HideFullScreenLoader();
                });
            } else {
                PxPage.Loader.HideFullScreenLoader();
            }
        }
    };
})(jQuery);