var PxViewRender = function ($) {
    var _static = {
        defaults: {
            url: "http://192.168.78.92:8100/test/renderview",
            escapeScript: true
        },
        sel: {

        },
        fn: {
            renderView: function (data, type, options) {
                var content = '';
                _static.defaults = $.extend({}, _static.defaults, options)
                if (!type)
                    type = "GET";
                
                $.ajax({
                    type: type,
                    data: data,
                    async: false,
                    url: _static.defaults.url,
                    dataType: 'html',
                    error: function (error) {
                        content = "error";
                    },
                    success: function (html) {
                        if (_static.defaults.escapeScript) {
                            content = _static.fn.escapeSpecialSymbols(html);
                        } else {
                            content = html;
                        }
                    }
                });

                return content;
            },
            escapeSpecialSymbols: function (html) {
                html = html.replace(/<script/g, "<!--script");
                html = html.replace(/<\/script>/g, "<\/script-->");
                //html = html.replace(/<script.*>[\w\W]{1,}(.*?)[\w\W]{1,}<\/script>/gi, "");

                return html;
            }
        }
    };
    return {
        RenderView: function (data, type, options) {
            return _static.fn.renderView(data, type, options);
        }
    }
}(jQuery);