var StartWelcomeWidget = function ($) {
    // static data
    var _static = {
        defaults: {
        },
        settings: {
            widgetid: "PX_FacePlate_StartWelcome_Widget",
            editUrl: "../StartWelcomeWidget/Edit"
        },
        sel: {
            welcomeDiv: ".facePlate-start_welcome"
        },
        // private functions
        fn: {
            BindControls: function () {
                $('#Title').bind('keypress', function (e) {
                    if (e.keyCode == 13) {
                        $('.ui-button-text').click();
                        return false;
                    }
                });
            }

        }
    };
    // static functions
    return {
        Init: function () {
            _static.fn.BindControls();
        }
    };
} (jQuery)

