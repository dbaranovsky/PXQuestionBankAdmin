// Adds ability to asynchronously load the options in a dropdown menu.
// options:
// loadingClass - class applied to the select element when loading starts.
// loadedClass - class applied to select element after loading completes.
// waitClass - class applied to the special option injected while waiting for loading to complete.
// loadData - function that will load the options. Should call callback method passing in array of { value, text } objects.
(function($) {
	
	var _render = null;
	var _log = null;
	
    // privately scoped data used by the plugin
	var _private = {
		pluginName: "DropDown",
		dataKey: "DropDown",
		bindSuffix: ".DropDown",
		defaults: {
			loadingClass: 'dropdown-loading',
			loadedClass: 'dropdown-loaded',
			waitClass: 'dropdown-wait',
			loadData: function(callback) {
				callback(null);
			}
		}
	};

    // Acts as the callback parameter to loadData. Injects the option elements needed
    // to represents the new dropdown items.
	_render = function(instance, options) {
		var data = instance.data(_private.dataKey);
		var settings = data.settings;
		
		if(options) {
			var wait = data.target.find('.' + settings.waitClass);
			$.each(options, function(index, value) {
				wait.before('<option value="' + value.value + '" >' + value.text + '</option>');
			});			
			wait.remove();
		}
		
		data.target.addClass('loaded');
		data.target.removeClass('loading');
	};
	
    // public interface of the plugin
    var intrface = {
        // initializes the plugin
		init: function(options) {
			return this.each(function() {
                var settings = $.extend(true, {}, _private.defaults, options),
                $this = $(this),
                data = $this.data(_private.dataKey);

                if (!data) {
                    $this.data(_private.dataKey, {
                        target: $this,
                        settings: settings
                    });
                    data = $this.data(_private.dataKey);
                    var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);
                    var bindEvent = isSafari ? 'mousedown' : 'click';
                   
                    $this.unbind(bindEvent).bind(bindEvent, function () {
						if(!$this.hasClass(settings.loadedClass) && !$this.hasClass(settings.loadingClass)) {
							$this.append('<option class="' + settings.waitClass + '"></option>');
							$this.addClass(settings.loadingClass);
							
							settings.loadData(function(loadedData) { _render($this, loadedData); });
						}
					});
                }
            });
        },
        // destroys the plugin instances
		destroy: function() {
            return this.each(function() {
                $(this).unbind(_private.bindSuffix);
            });
        }
	};

    // registers the plugin
	$.fn.DropDown = function(method) {
		if (intrface[method]) {
            return intrface[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return intrface.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + pluginName);
        }
	};
}(jQuery))