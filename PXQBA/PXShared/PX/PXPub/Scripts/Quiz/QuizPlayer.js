//jQuery plugin for Quiz Preview functionality

(function ($) {
    var _static = {
        pluginName: "quizplayer",
        dataKey: "quizplayer",
        bindSuffix: ".quizplayer",
        dataAttrPrefix: "data-qp-",
        //plugin defaults
        defaults: {
            
        },
        //css settings
        css: {
            
        },
        //selectors for commonly accessed elements
        sel: {

        },
        //private functions
        fn: {
            
        },
    };
    
    var api = {
        init: function(options) {
            
        },
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.quizplayer = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

} (jQuery));