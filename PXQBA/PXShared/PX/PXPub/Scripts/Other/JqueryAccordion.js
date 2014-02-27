
var PxAssignmentCenterAccordion = function($) {
    return {
        Init: function() {
            $("html").addClass("js");
            $.fn.accordion.defaults.container = false;

            $("#acc2").accordion({
                    obj: "div",
                    wrapper: "div",
                    el: ".h",
                    head: "h4, h5",
                    next: "div:not(.calendar-container)",
                    initShow: "div.shown"
                });
            $("html").removeClass("js");
        }
    };
} (jQuery);

PxPage.OnReady(function () {
    PxAssignmentCenterAccordion.Init();
});