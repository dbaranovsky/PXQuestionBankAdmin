//The NoteWidget represents 
(function($) {
    var defaults = {};
    var pluginName = "NoteWidget";
    var dataKey = "NoteWidget";
    var bindSuffix = ".NoteWidget";

    //templates    
    
    //private methods
    
    //public interface
    var intrface = {
        init: function(options) {
            return this.each(function() {
                var settings = $.extend(true, {}, defaults, options),
                $this = $(this),
                data = $this.data(dataKey);

                if (!data) {
                    $this.data(dataKey, {
                        target: $this,
                        settings: settings
                    });
                    data = $this.data(dataKey);

                    //event bindings
                }
            });
        },
        destroy: function() {
            return this.each(function() {
                $(this).unbind(bindSuffix);
            });
        }
    };

    $.fn.NoteWidget = function(method) {
        if (intrface[method]) {
            return intrface[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return intrface.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + pluginName);
        }
    };
} (jQuery))