(function ($) {
    $.fn.ActionWidget = function (settings) {
        var defaults = { className: "contextMenu", openEvent: "click" };
        var options = defaults;
        if (settings != null && settings != undefined)
            options = $.extend(true, {}, defaults, settings);



        //template used to render a menu of actions
        var _menuTemplate = $.template(null, '<ul id="${id}" class="' + options.className + '">' +
                                                        '{{each options}}' +
                                                            '<li title="${$value.title}" class="${$value.name} ${$value.className}">' +
                                                                '<a href="javascript:" action_href="#${$value.name}" rel="${$value.relation}">{{html $value.text }}</a>' +
                                                            '</li>' +
                                                        '{{/each}}' +
                                                     '</ul>');
        //template used to render button user clicks on to show menu
        var _buttonTemplate = $.template(null, '<div class="gearbox"></div>');

        //registers event handler for when actions are selected
        var _attachActionHandler = function (instance) {
            instance.find("ul li").click(function (event) {
                if (options.action && typeof (options.action) === "function") {
                    var actionName = $(this).find("a").attr("action_href");
                    actionName = actionName.substring(actionName.lastIndexOf("#") + 1);
                    options.action.apply(this, [event, actionName, instance]);
                }
            });
        };

        //draws, or redraws, the menu items as needed
        var _renderMenu = function (instance) {
            var data = options.menu;
            if (instance.find("ul").length > 0) {
                instance.find("ul").remove();
            }
            if (options.menu && typeof(options.menu) === "function") {
                data = options.menu(instance);
            }
            $.tmpl(_menuTemplate, data).appendTo(instance);
            _attachActionHandler(instance);
        };

        return this.each(function () {
            var $this = $(this);

          

            //clicking on the icon displays the menu, and disables the opening event (until the menu is closed)
            var handleBodyClick = function () {
                $(".contextMenu").hide();
                $this.find(".gearbox").first().bind(options.openEvent, showMenu);
            };

            //render the control
            if (!$this.find(".gearbox").length) {
                $.tmpl(_buttonTemplate, null).appendTo($this);
            }
            
            var showMenu = function (event) {
                PxPage.log("action menu clicked");
                event.stopPropagation();
                
                _renderMenu($this);
                
                $this.find("ul").first().show().position({ of: $(this), my: "left top", at: "left bottom" });

                // unbind the triggering event
                $this.find(".gearbox").first().unbind(options.openEvent);
                $("body").rebind("click", handleBodyClick);

                //bug-fix :- PLATX-4424
                $("#content_widget_highlight_menu").bind("mouseleave", handleBodyClick);//TODO: This should not be here!

                event.stopImmediatePropagation();
            };

            $this.find(".gearbox").unbind(options.openEvent).bind(options.openEvent, showMenu);

        });
    };

    if ($("iframe").length > 0) {
        $("iframe").contents().find("body").click(function () { $(".contextMenu").hide(); });
    }

})(jQuery);