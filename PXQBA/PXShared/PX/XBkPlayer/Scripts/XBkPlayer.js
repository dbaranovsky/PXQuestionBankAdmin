// XBkPlayer
//
// This plugin is responsible for the client-side behaviors of the
// XBook Player.
//
//
(function ($) {
    var _static = {
        pluginName: "XBkPlayer",
        dataKey: "XBkPlayer",
        bindSuffix: ".XBkPlayer",
        dataAttrPrefix: "data-qp-",
        defaults: {},
        settings: {},
        css: {},
        sel: {
            questionPlayer: '#question-player',
            contentWrapper: '#question-player .content-wrapper',
            contentMain: '#question-player .content-main'
        },
        fn: {
            displaySetup: function (data) {
                $(_static.sel.questionPlayer).show();
                $(_static.sel.contentMain).html(data);
            }
        }
    },

    api = {
        init: function (options) {
            var settings = $.extend(true, _static.settings, _static.defaults, options),
            $this = $(this),
            data = $this.data(_static.dataKey);

            if (!data) {
                $this.data(_static.dataKey, {
                    target: $this,
                    settings: settings
                });

                data = $this.data(_static.dataKey);
            }

            _static.settings = settings;

            _static.fn.displaySetup(_static.settings.XBkPlayerData);
        },

        build: function () {
            $.get(_static.settings.LoadUrl, function (dataIn) {
                _static.fn.displaySetup(dataIn);
            });
        }
    };

    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.XBkPlayer = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

    // The public interface for interacting with this plugin.
    window.XBkPlayerHelper = {

};
} (jQuery))