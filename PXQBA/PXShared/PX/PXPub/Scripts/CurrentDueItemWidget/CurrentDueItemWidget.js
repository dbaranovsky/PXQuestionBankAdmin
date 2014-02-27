// PxCurrentItemDue
//
// This plugin is responsible for the client-side behaviors of the
// Px Unit Detail children
//
// This plugin requires the fauxtree plugin as the fauxtree is used
// to display the items inside a Unit.
//
(function ($) {
    var _static = {
        pluginName: "PxCurrentItemDue",
        dataKey: "PxCurrentItemDue",
        bindSuffix: ".PxCurrentItemDue",
        dataAttrPrefix: "data-ud-",
        // default option values
        defaults: {
            //determines how long a dragged item has to be held over an expandable item
            //in order to trigger expansion
            expandDelay: 1500,
            readOnly: false
        },
        // class names used by the plug-in
        css: {
        },

        sel: {
        },

        fn: {
            onclickOpenItem: function () {


                /*
                $.post(PxPage.Routes.show_assignment_widget, function (response) {
                $("#view-all-items-launch-pad").html(response);
                $("#view-all-items-launch-pad").dialog({
                width: 500,
                height: 375,
                minWidth: 500,
                minHeight: 375,
                modal: true,
                dialogClass:"upcoming-faceplate",
                draggable: false,
                title: "Upcoming Assignments",
                resizable: false,
                modal: true
                });

                });
                */
            },
            load: function () {
                PxPage.Loading("PX_LaunchpadAssignmentsWidget", false);
                var args = {};

                $.post(PxPage.Routes.show_current_due_item_widget, args, function (response) {
                    if ($("#PX_LaunchpadAssignmentsWidget").length) {
                        $("#PX_LaunchpadAssignmentsWidget").replaceWith(response);
                    } else if ($(".PX_LaunchpadAssignmentsWidget").length) {
                        $(".PX_LaunchpadAssignmentsWidget").replaceWith(response);
                    } else {
                        $("#LaunchPadAssignmentsWidget").replaceWith(response);
                    }
                    PxPage.Loaded();
                });
            }
        },
        //clean up
        destroy: function () {
            return this.each(function () {
                $(this).unbind(_static.bindSuffix);
                $("#fne-window").removeClass("OpenCalendarView");
            });
        }
    };

    // The public interface for interacting with this plugin.
    var api = {

        // The init method sets up the data for the plugin using the given
        // option values to override the defaults.                
        init: function (options) {
            $(PxPage.switchboard).unbind("loadcurrentdueitem");
            $(PxPage.switchboard).bind("loadcurrentdueitem", function () {
                _static.fn.load();
            });

            $(PxPage.switchboard).bind("fneclosing", function () {
                $("#fne-window").removeClass("OpenCalendarView");
            });
        },

        destroy: function () {
            return this.each(function () {
                $(this).unbind(_static.bindSuffix);
                $("#fne-window").removeClass("OpenCalendarView");
            });
        }

    };

    //Handle the custome attributes
    $.fn.udattr = function (name, value) {
        var target = this.first(),
            aName = _static.dataAttrPrefix + name;
        if (value != null)
            target.attr(aName, value);
        return target.attr(aName);
    };

    // Associate the plugin with jQuery
    $.fn.PxCurrentItemDue = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        }
        else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    }

} (jQuery))