/*
Plugin: iframe autoheight jQuery Plugin
Author: original code by NATHAN SMITH; converted to plugin by Jesse House
File: jquery.iframe-auto-height.plugin.js
Description: when the page loads set the height of an iframe based on the height of its contents
Remarks: original code from http://sonspring.com/journal/jquery-iframe-sizing
Version: 1.0.0 - see README: http://github.com/house9/jquery-iframe-auto-height
*/
(function($) {
    $.fn.iframeAutoHeight = function(options) {
        // set default option values
        var options = $.extend({
            heightOffset: 0,
            newHeight: 0,
            isResized: false
        }, options);

        // iterate over the matched elements passed to the plugin
        $(this).each(function () {

            //removed some code here that was checking for whether the browser was webkit based.
            //this may not be necessary anymore and was removed as it did not function properly in chrome.
            resizeHeight(this);
            // For other browsers.
            $(this).load(function() {
                resizeHeight(this);
            });

            // chrome hack to resize iframe only once
            function isResized(iframe) {
                // Set inline style to equal the body height of the iframed content plus a little
                var newHeight = iframe.contentWindow.document.body.offsetHeight + options.heightOffset;
                newHeight = newHeight + 'px';
                return (iframe.style.height >= newHeight);
            }

            // resizeHeight
            function resizeHeight(iframe) {
                try {
                    if (iframe.contentDocument) {
                        var doc = iframe.contentDocument;
                    } else if (iframe.contentWindow) {
                        var doc = iframe.contentWindow.document;
                    } else {
                        var doc = iframe.document;
                    }

                    if (doc.body) {
                        var height = $(iframe).contents().height();
                        if (height > 0) {
                            var newHeight = height + options.heightOffset;
                        } else {
                            var newHeight = doc.body.offsetHeight + options.heightOffset;
                        }

                        var scrollHeight = iframe.contentWindow.document.body.scrollHeight;
                        if (scrollHeight > newHeight) {
                            newHeight = scrollHeight;
                        }
                        iframe.style.height = newHeight + 'px';
                        options.isResized = true;
                    }
                }
                catch(ex) {
                    
                }
            }
        }); // end
    }
})(jQuery);