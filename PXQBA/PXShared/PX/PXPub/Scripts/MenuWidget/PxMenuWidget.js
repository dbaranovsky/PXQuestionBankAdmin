// Defines a singleton object that coordinates all client-side behavior of the AssignmentCenter page.
(function ($) {
    // static plugin values
    var _static = {
        pluginName: "PxMenuWidget",
        dataKey: "PxMenuWidget",
        bindSuffix: ".MenuWidget",
        dataAttrPrefix: "data-cw-",
        defaults: {},
        sel: {
            AssignDialog: "#assigndialog"

        },
        fn: {
            onMenuItemClick: function (currentNode) {

                var target = currentNode.attr("rel");
                var sourceUrl = currentNode.attr("href");

                if (currentNode.hasClass('disabledmenu')) {
                    return;
                }

                _static.fn.updatePaginationIndex(currentNode);

                if (undefined != $._data($(PxPage.switchboard)[0], 'events')['MenuClickOverride']) {
                    PxPage.log('Application has over ridden the standard menu click');
                    $(PxPage.switchboard).trigger('MenuClickOverride', [currentNode]);
                    return;
                }

                if ($(target).length) {
                    var originalHeight = $(target).height();
                    $(target).children().fadeOut(500);

                    PxPage.Loading(target);

                    $.ajax({
                        url: sourceUrl,
                        data: {
                            //itemId: id
                        },
                        type: "GET",
                        success: function (response) {
                            PxPage.Loaded(target);

                            $(target).html($(response));
                            $(target).children().hide().fadeIn(500);
                            var fullHeight = $(target).css("height", "auto").height();
                            $(target).css("height", originalHeight + "px");
                            $(target).animate({ height: fullHeight }, 500, function() {
                                $(target).css("height", "auto");
                            });
                            
                            _static.fn.itemloaded(currentNode);
                            $(PxPage.switchboard).trigger("MenuWidget.ItemLoaded", [currentNode, true]);

                        }
                    });
                }
                else {

                    window.location = sourceUrl;

                }
            },


            itemloaded: function (currentNode) {
                var parentNode = currentNode.closest('li.menu-item[mw-index=-1]');
                if (parentNode.length) {
                    var parentSiblings = $(parentNode).siblings('li.menu-item[mw-index=-1]');
                    parentSiblings.removeClass('active');
                    parentNode.addClass('active');
                }


            },


            onMenuNavClick: function (currentNode) {
                var parentPagination = currentNode.parent(),
                    parentMenu = currentNode.closest('.PX_Menu'),
                    total = parentPagination.find('.total').text(),
                    activeIndex = parentPagination.find('.active-index').text(),
                    selectedIndex = -1;

                if (currentNode.hasClass('disabledmenu')) {
                    return;
                }

                if (currentNode.hasClass('left-open')) {
                    selectedIndex = parseInt(activeIndex) - 1;
                }
                else {
                    if (parseInt(activeIndex) < parseInt(total)) {
                        selectedIndex = parseInt(activeIndex) + 1;
                    } else {
                        return;
                    }
                }

                var selectedNode = parentMenu.find('.menu-item[mw-index=' + selectedIndex + ']');
                if (selectedNode.length) {
                    selectedNode.find('.menu-link').click();
                }

                parentPagination.find('.active-index').text(selectedIndex);

            },

            updatePaginationIndex: function (currentNode) {
                if (currentNode.parent().hasClass('disabledmenu')) {
                    return;
                }

                var parentPagination = currentNode.parent(),
                    parentMenuDiv = $('#project-overlay-content-nav'),
                    parentMenu = parentMenuDiv.find('.menu-pagination'),
                    total = parentMenu.find('.total').text(),
                    activeIndex = parentPagination.attr('mw-index');

                var search = 'li.menu-item[mw-index=' + activeIndex + ']';

                selectedIndex = parseInt(activeIndex);
                parentMenuDiv.find('li').removeClass('active');

                var childrenNodeOf = parentMenuDiv.find(search).closest('li.menu-item[mw-index=-1]');
                if (childrenNodeOf.length) {
                    childrenNodeOf.addClass('active');
                }
                else {
                    parentMenuDiv.find(search).addClass('active');
                }
                //parentPagination.addClass('active');

                if (selectedIndex > 0) {
                    parentMenu.find('.start-button').hide();
                    parentMenu.find('.menu-numeric-seq').show();
                    parentMenu.find('.active-index').text(selectedIndex);
                }
                else {
                    parentMenu.find('.start-button').show();
                    parentMenu.find('.menu-numeric-seq').hide();
                    parentMenu.find('.active-index').text(selectedIndex);
                }
            }

        }
    };

    // The public interface for interacting with this plugin.
    var api = {
        // The init method sets up the data for the plugin using the given
        // option values to override the defaults.   
        init: function (options) {

            return this.each(function () {
                var settings = $.extend(true, {}, _static.defaults, options),
                $this = $(this),
                data = $this.data(_static.dataKey);
                var currentItem = "";

                if (settings.usehash === undefined || settings.usehash.toLowerCase() != "true") {
                    $(".PX_MenuWidget_Wrapper .menu-link").die();

                    $(".PX_MenuWidget_Wrapper .menu-link").live("click", function (event) {
                        event.preventDefault();
                        _static.fn.onMenuItemClick($(this));
                    });

                    $(".PX_MenuWidget_Wrapper .nav-button").die();

                    $(".PX_MenuWidget_Wrapper .nav-button").live("click", function (event) {
                        event.preventDefault();
                        _static.fn.onMenuNavClick($(this));
                    });


                    $(".PX_MenuWidget_Wrapper .start-button").die();
                    $(".PX_MenuWidget_Wrapper .start-button").live("click", function (event) {
                        event.preventDefault();
                        _static.fn.onMenuNavClick($(this));
                    });

                    $(".PX_MenuWidget_Wrapper .menu-numeric-seq").hide();

                    PxPage.OnProductLoaded(function () {
                        $(PxPage.switchboard).trigger("MenuWidget.MenuLoaded");
                    });
                }
            });
        },
        // the destroy method cleans up any plugin related data for the instance
        // on which it is called.
        destroy: function () {
            return this.each(function () {
                $(this).unbind(_static.bindSuffix);
            });
        }
    };

    // Associate the plugin with jQuery
    $.fn.PxMenuWidget = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        }
        else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };
} (jQuery))





