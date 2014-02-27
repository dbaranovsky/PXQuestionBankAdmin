var PxViewRender = function ($) {
    var _static = {
        defaults: {
            url: "http://localhost:8100/test/renderview",
            escapeScript: true
        },
        sel: {

        },
        fn: {
            renderView: function (app, folder, data, type, options) {
                var content = '';
                _static.defaults = $.extend({}, _static.defaults, options)
                if (!type)
                    type = "POST";

                var url = _static.defaults.url + '/' + app;
                if (folder !== undefined) {
                    url = url + '/' + folder;
                }
                $.ajax({
                    type: type,
                    data: data,
                    async: false,
                    url: url,
                    dataType: 'html',
                    error: function (error) {
                        content = error.statusText;
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
        RenderView: function (app, folder, data, type, options) {
            return _static.fn.renderView(app, folder, data, type, options);
        }
    };
}(jQuery);