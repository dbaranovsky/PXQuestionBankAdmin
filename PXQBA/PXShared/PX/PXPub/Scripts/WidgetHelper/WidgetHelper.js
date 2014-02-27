var PxWidgetHelper = function ($) {

    var _static = {

        pluginName: "PXWidgetHelper",
        dataKey: "WidgetHelper",
        bindSuffix: ".WidgetHelper",
        dataAttrPrefix: "data-wh-",
        defaults: {
        },
        settings: {},
        modes: {},
        // commonly used CSS classes
        css: {},
        // commonly used jQuery selectors
        sel: {
        },
        fn: {
            WidgetInputHelper: function (inputHelpers, widgetId) {
                var pageValue = '';
                var variableName = '';
                var useDefaultValue = '';
                var defaultValue = '';
                var helperCollection = {};
                var setValue = false;

                $.each(inputHelpers, function (key, valObj) {
                    pageValue = ''; setValue = false;
                    variableName = valObj.Name;
                    useDefaultValue = valObj.UseDefaultValue;
                    defaultValue = valObj.DefaultValue;

                    pageValue = $(valObj.Selector).val();
                    if (pageValue !== '' && pageValue !== null && pageValue !== undefined) {
                        helperCollection[variableName] = pageValue;
                        setValue = true;
                    }

                    if (setValue == false && useDefaultValue) {
                        helperCollection[variableName] = defaultValue;
                    }
                });

                return helperCollection;
            }

        }
    };

    return {
        WidgetInputHelper: function (inputHelpers, widgetId) {
            return _static.fn.WidgetInputHelper(inputHelpers, widgetId);
        }
    };

} (jQuery);