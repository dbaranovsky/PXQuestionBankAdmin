/* ****************************************************************************
*
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/
/*
* Treeview 1.5pre - jQuery plugin to hide and show branches of a tree
* 
* http://bassistance.de/jquery-plugins/jquery-plugin-treeview/
* http://docs.jquery.com/Plugins/Treeview
*
* Copyright (c) 2007 Jörn Zaefferer
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Revision: $Id: jquery.treeview.js 5759 2008-07-01 07:50:28Z joern.zaefferer $
*
*/
/**
* Cookie plugin
*
* Copyright (c) 2006 Klaus Hartl (stilbuero.de)
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
*/

/**
* Create a cookie with the given name and value and other optional parameters.
*
* @example $.cookie('the_cookie', 'the_value');
* @desc Set the value of a cookie.
* @example $.cookie('the_cookie', 'the_value', {expires: 7, path: '/', domain: 'jquery.com', secure: true});
* @desc Create a cookie with all available options.
* @example $.cookie('the_cookie', 'the_value');
* @desc Create a session cookie.
* @example $.cookie('the_cookie', null);
* @desc Delete a cookie by passing null as value.
*
* @param String name The name of the cookie.
* @param String value The value of the cookie.
* @param Object options An object literal containing key/value pairs to provide optional cookie attributes.
* @option Number|Date expires Either an integer specifying the expiration date from now on in days or a Date object.
*                             If a negative value is specified (e.g. a date in the past), the cookie will be deleted.
*                             If set to null or omitted, the cookie will be a session cookie and will not be retained
*                             when the the browser exits.
* @option String path The value of the path atribute of the cookie (default: path of page that created the cookie).
* @option String domain The value of the domain attribute of the cookie (default: domain of page that created the cookie).
* @option Boolean secure If true, the secure attribute of the cookie will be set and the cookie transmission will
*                        require a secure protocol (like HTTPS).
* @type undefined
*
* @name $.cookie
* @cat Plugins/Cookie
* @author Klaus Hartl/klaus.hartl@stilbuero.de
*/

/**
* Get the value of a cookie with the given name.
*
* @example $.cookie('the_cookie');
* @desc Get the value of a cookie.
*
* @param String name The name of the cookie.
* @return The value of the cookie.
* @type String
*
* @name $.cookie
* @cat Plugins/Cookie
* @author Klaus Hartl/klaus.hartl@stilbuero.de
*/

jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        var path = options.path ? '; path=' + options.path : '';
        var domain = options.domain ? '; domain=' + options.domain : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

; (function ($) {

    // TODO rewrite as a widget, removing all the extra plugins
    $.extend($.fn, {
        swapClass: function (c1, c2) {
            var c1Elements = this.filter('.' + c1);
            this.filter('.' + c2).removeClass(c2).addClass(c1);
            c1Elements.removeClass(c1).addClass(c2);
            return this;
        },
        replaceClass: function (c1, c2) {
            return this.filter('.' + c1).removeClass(c1).addClass(c2).end();
        },
        hoverClass: function (className) {
            className = className || "hover";
            return this.hover(function () {
                $(this).addClass(className);
            }, function () {
                $(this).removeClass(className);
            });
        },
        heightToggle: function (animated, callback) {
            animated ?
				this.animate({ height: "toggle" }, animated, callback) :
				this.each(function () {
				    jQuery(this)[jQuery(this).is(":hidden") ? "show" : "hide"]();
				    if (callback)
				        callback.apply(this, arguments);
				});
        },
        heightHide: function (animated, callback) {
            if (animated) {
                this.animate({ height: "hide" }, animated, callback);
            } else {
                this.hide();
                if (callback)
                    this.each(callback);
            }
        },
        prepareBranches: function (settings) {
            if (!settings.prerendered) {
                // mark last tree items
                this.filter(":last-child:not(ul)").addClass(CLASSES.last);
                // collapse whole tree, or only those marked as closed, anyway except those marked as open
                this.filter((settings.collapsed ? "" : "." + CLASSES.closed) + ":not(." + CLASSES.open + ")").find(">ul").hide();
            }
            // return all items with sublists
            return this.filter(":has(>ul)");
        },
        applyClasses: function (settings, toggler) {
            // TODO use event delegation
            this.filter(":has(>ul):not(:has(>a))").find(">span").unbind("click.treeview").bind("click.treeview", function (event) {
                // don't handle click events on children, eg. checkboxes
                if (this == event.target)
                    toggler.apply($(this).next());
            }).add($("a", this)).hoverClass();

            if (!settings.prerendered) {
                // handle closed ones first
                this.filter(":has(>ul:hidden)")
						.addClass(CLASSES.expandable)
						.replaceClass(CLASSES.last, CLASSES.lastExpandable);

                // handle open ones
                this.not(":has(>ul:hidden)")
						.addClass(CLASSES.collapsable)
						.replaceClass(CLASSES.last, CLASSES.lastCollapsable);

                // create hitarea if not present
                var hitarea = this.children("div." + CLASSES.hitarea);
                if (!hitarea.length)
                    hitarea = this.prepend("<div class=\"" + CLASSES.hitarea + "\"/>").children("div." + CLASSES.hitarea);
                hitarea.removeClass().addClass(CLASSES.hitarea).each(function () {
                    var classes = "";
                    $.each($(this).parent().attr("class").split(" "), function () {
                        classes += this + "-hitarea ";
                    });
                    $(this).addClass(classes);
                })
            }

            // apply event to hitarea
            this.find("div." + CLASSES.hitarea).unbind().click(toggler);
        },
        treeview: function (settings) {

            settings = $.extend({
                cookieId: "treeview"
            }, settings);

            if (settings.toggle) {
                var callback = settings.toggle;
                settings.toggle = function () {
                    return callback.apply($(this).parent()[0], arguments);
                };
            }

            // factory for treecontroller
            function treeController(tree, control) {
                // factory for click handlers
                function handler(filter) {
                    return function () {
                        // reuse toggle event handler, applying the elements to toggle
                        // start searching for all hitareas
                        toggler.apply($("div." + CLASSES.hitarea, tree).filter(function () {
                            // for plain toggle, no filter is provided, otherwise we need to check the parent element
                            return filter ? $(this).parent("." + filter).length : true;
                        }));
                        return false;
                    };
                }
                // click on first element to collapse tree
                $("a:eq(0)", control).click(handler(CLASSES.collapsable));
                // click on second to expand tree
                $("a:eq(1)", control).click(handler(CLASSES.expandable));
                // click on third to toggle tree
                $("a:eq(2)", control).click(handler());
            }

            // handle toggle event
            function toggler() {
                var myParent = $(this).parent();
                var opened = myParent.filter(":has(>ul:hidden)").length == 0;

                var root = myParent.parents('.treeview');
                var changeData = new MvcControlsToolkit_changeData(myParent, opened ? 'ItemClosing' : 'ItemOpening', null);
                root.trigger('itemChange', changeData);
                if (changeData.Cancel == true) return;
                myParent
                // swap classes for hitarea
					.find(">.hitarea")
						.swapClass(CLASSES.collapsableHitarea, CLASSES.expandableHitarea)
						.swapClass(CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea)
					.end()
                // swap classes for parent li
					.swapClass(CLASSES.collapsable, CLASSES.expandable)
					.swapClass(CLASSES.lastCollapsable, CLASSES.lastExpandable)
                // find child lists
					.find(">ul")
                // toggle them
					.heightToggle(settings.animated, settings.toggle);
                if (settings.unique) {
                    $(this).parent()
						.siblings()
                    // swap classes for hitarea
						.find(">.hitarea")
							.replaceClass(CLASSES.collapsableHitarea, CLASSES.expandableHitarea)
							.replaceClass(CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea)
						.end()
						.replaceClass(CLASSES.collapsable, CLASSES.expandable)
						.replaceClass(CLASSES.lastCollapsable, CLASSES.lastExpandable)
						.find(">ul")
						.heightHide(settings.animated, settings.toggle);
                }
                changeData = new MvcControlsToolkit_changeData(myParent, opened ? 'ItemClosed' : 'ItemOpened', null);
                root.trigger('itemChange', changeData);
            }
            this.data("toggler", toggler);

            function serialize() {
                function binary(arg) {
                    return arg ? 1 : 0;
                }
                var data = [];
                branches.each(function (i, e) {
                    data[i] = $(e).is(":has(>ul:visible)") ? 1 : 0;
                });
                $.cookie(settings.cookieId, data.join(""), settings.cookieOptions);
            }

            function deserialize() {
                var stored = $.cookie(settings.cookieId);
                if (stored) {
                    var data = stored.split("");
                    branches.each(function (i, e) {
                        $(e).find(">ul")[parseInt(data[i]) ? "show" : "hide"]();
                    });
                }
            }

            // add treeview class to activate styles
            this.addClass("treeview");

            // prepare branches and find all tree items with child lists
            var branches = this.find("li").prepareBranches(settings);

            switch (settings.persist) {
                case "cookie":
                    var toggleCallback = settings.toggle;
                    settings.toggle = function () {
                        serialize();
                        if (toggleCallback) {
                            toggleCallback.apply(this, arguments);
                        }
                    };
                    deserialize();
                    break;
                case "location":
                    var current = this.find("a").filter(function () {
                        return this.href.toLowerCase() == location.href.toLowerCase();
                    });
                    if (current.length) {
                        // TODO update the open/closed classes
                        var items = current.addClass("selected").parents("ul, li").add(current.next()).show();
                        if (settings.prerendered) {
                            // if prerendered is on, replicate the basic class swapping
                            items.filter("li")
							.swapClass(CLASSES.collapsable, CLASSES.expandable)
							.swapClass(CLASSES.lastCollapsable, CLASSES.lastExpandable)
							.find(">.hitarea")
								.swapClass(CLASSES.collapsableHitarea, CLASSES.expandableHitarea)
								.swapClass(CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea);
                        }
                    }
                    break;
            }

            branches.applyClasses(settings, toggler);

            // if control option is set, create the treecontroller and show it
            if (settings.control) {
                treeController(this, settings.control);
                $(settings.control).show();
            }

            return this;
        }
    });

    // classes used by the plugin
    // need to be styled via external stylesheet, see first example
    $.treeview = {};
    var CLASSES = ($.treeview.classes = {
        open: "open",
        closed: "closed",
        expandable: "expandable",
        expandableHitarea: "expandable-hitarea",
        lastExpandableHitarea: "lastExpandable-hitarea",
        collapsable: "collapsable",
        collapsableHitarea: "collapsable-hitarea",
        lastCollapsableHitarea: "lastCollapsable-hitarea",
        lastCollapsable: "lastCollapsable",
        lastExpandable: "lastExpandable",
        last: "last",
        hitarea: "hitarea"
    });

})(jQuery);

(function ($) {
    var CLASSES = $.treeview.classes;
    var proxied = $.fn.treeview;
    $.fn.treeview = function (settings) {
        settings = $.extend({}, settings);
        if (settings.add) {
            return this.trigger("add", [settings.add]);
        }
        if (settings.remove) {
            return this.trigger("remove", [settings.remove]);
        }
        return proxied.apply(this, arguments).bind("add", function (event, branches) {
            $(branches).prev()
				.removeClass(CLASSES.last)
				.removeClass(CLASSES.lastCollapsable)
				.removeClass(CLASSES.lastExpandable)
			.find(">.hitarea")
				.removeClass(CLASSES.lastCollapsableHitarea)
				.removeClass(CLASSES.lastExpandableHitarea);
            $(branches).find("li").andSelf().prepareBranches(settings).applyClasses(settings, $(this).data("toggler"));
        }).bind("add_treeview_node", function (event, branches) {
            $(branches).prev()
				.removeClass(CLASSES.last)
				.removeClass(CLASSES.lastCollapsable)
				.removeClass(CLASSES.lastExpandable)
			.find(">.hitarea")
				.removeClass(CLASSES.lastCollapsableHitarea)
				.removeClass(CLASSES.lastExpandableHitarea);
            $(branches).prepareBranches(settings).applyClasses(settings, $(this).data("toggler"));
        }).bind("remove", function (event, branches) {
            var prev = $(branches).prev();
            var parent = $(branches).parent();
            $(branches).remove();
            prev.filter(":last-child").addClass(CLASSES.last)
				.filter("." + CLASSES.expandable).replaceClass(CLASSES.last, CLASSES.lastExpandable).end()
				.find(">.hitarea").replaceClass(CLASSES.expandableHitarea, CLASSES.lastExpandableHitarea).end()
				.filter("." + CLASSES.collapsable).replaceClass(CLASSES.last, CLASSES.lastCollapsable).end()
				.find(">.hitarea").replaceClass(CLASSES.collapsableHitarea, CLASSES.lastCollapsableHitarea);
            if (parent.is(":not(:has(>))") && parent[0] != this) {
                parent.parent().removeClass(CLASSES.collapsable).removeClass(CLASSES.expandable)
                parent.siblings(".hitarea").andSelf().remove();
            }
        });
    };

})(jQuery);

///////////////////////////////////TREEVIEWS Edit Extension/////////////////////////
var MvcControlsToolkit_TreeView_ButtonModePostfix = '_ButtonMode';
var MvcControlsToolkit_TreeView_SaveDisplayPostfix = '_SaveDisplay';
var MvcControlsToolkit_TreeView_SaveEditPostfix = '_SaveEdit';
var MvcControlsToolkit_TreeView_ContainerDisplayPostfix = '___Choice1___flattened_ItemsContainer';
var MvcControlsToolkit_TreeView_ContainerEditPostfix = '___Choice2___flattened_ItemsContainer';
var MvcControlsToolkit_TreeView_ToggleEditPostfix = '_ToggleEditButton';
var MvcControlsToolkit_TreeView_IsEditPostFix = '___IsChoice2';
var MvcControlsToolkit_TreeView_RootNamePostfix = "_RootNamePostfix";
var MvcControlsToolkit_TreeView_ClosedPostfix = "___Closed";
var MvcControlsToolkit_TreeView_ItemsCountPostfix = "___ItemsCount";
var MvcControlsToolkit_TreeView_TemplatesPostfix = "_Templates";
var MvcControlsToolkit_TreeView_TemplateSymbolPrefix = '_TemplateSymbol';
var MvcControlsToolkit_TreeView_TemplateSriptPrefix = '_TemplateSript';
var MvcControlsToolkit_TreeView_TemplateHtmlPrefix = '_TemplateHtml';
var MvcControlsToolkit_TreeView_CanSortPrefix = '_CanSort';
var MvcControlsToolkit_TreeView_ItemsContainerPrefix = '_ItemsContainer';
var MvcControlsToolkit_TreeView_ContainerPrefix = '_Container';
var MvcControlsToolkit_TreeView_Open = 1;
var MvcControlsToolkit_TreeView_Close = 2;
var MvcControlsToolkit_TreeView_Toggle = 0;
var MvcControlsToolkit_TreeView_Updated = false;

function MvcControlsToolkit_TreeView_ChangeNodeState(node, operation) {
    var itemName = node;
    if (typeof (itemName) != 'string') itemName = MvcControlsToolkit_TreeView_ItemName(itemName);
    var place = itemName.lastIndexOf("___");
    if (place < 0) return;
    itemName = itemName.substring(0, place);

    var item = $(document.getElementById(itemName + MvcControlsToolkit_TreeView_ContainerPrefix));
    var hitharea = item.find(">.hitarea");
    var opened = item.find(':has(>ul:hidden)').length == 0;
    if (operation == MvcControlsToolkit_TreeView_Toggle) hitharea.click();
    else if (operation == MvcControlsToolkit_TreeView_Close && opened) hitharea.click();
    else if (operation == MvcControlsToolkit_TreeView_Open && !opened) hitharea.click();
}

function MvcControlsToolkit_TreeView_StartDrag(item, jQueryRoot) {
    MvcControlsToolkit_TreeView_Updated = false;
    var failure = true;
    var currClass = jQueryRoot.attr("class");
    if (currClass != null) {
        var currClass = currClass.split(' ');
        if (currClass != null && currClass.length > 0) {
            currClass = currClass[0];
            if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) != "_")
                return;
            else {
                currClass = currClass.substring(0, currClass.length - 1);
                var selectedElement = $('.' + currClass);
                if (selectedElement.length > 0) {
                    var res = MvcControlsToolkit_FormContext$_isElementInHierarchy(item[0], selectedElement[0]);
                    if (res) {
                        selectedElement.removeClass(currClass);
                        selectedElement.attr("class", currClass + "__ " + selectedElement.attr("class"));
                    }
                }
            }
        }
    }

    jQueryRoot.sortable('option', 'items', '');
    jQueryRoot.sortable('refresh');

}

function MvcControlsToolkit_TreeView_StopDrag(item, jQueryRoot) {
    MvcControlsToolkit_TreeView_Updated = false;
    var currClass = jQueryRoot.attr("class");
    if (currClass != null) {
        var currClass = currClass.split(' ');
        if (currClass != null && currClass.length > 0) {
            currClass = currClass[0];
            if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) == "_")
                currClass = currClass + '_';
            else {
                currClass = currClass + '__';
            }
            var selectedElement = $('.' + currClass);
            if (selectedElement.length > 0) {
                selectedElement.removeClass(currClass);
                currClass = currClass.substring(0, currClass.length - 2);
                selectedElement.attr("class", currClass + ' ' + selectedElement.attr("class"));
            }
        }
    }
    jQueryRoot.sortable('option', 'items', '> *');
    jQueryRoot.sortable('refresh');
}

function MvcControlsToolkit_TreeView_SelectLevel(target, selector) {
    var root = $(target).parent().find('>.mvcct-items-container');
    var currClass = root.attr("class");
    var arr = null;
    if (currClass != null) {
        var arr = currClass.split(' ');
        if (arr != null && arr.length > 0) {
            currClass = arr[0];
        }
        else {
            arr = new Array();
            arr.push(currClass);
        }
        currClass = arr[0];
    }
    if (target.checked) {
        if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) == "_")
            currClass = currClass.substring(0, currClass.length - 1);

    }
    else {

        if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) != "_")
            currClass = currClass + '_';
    }
    for (var i = 1; i < arr.length; i++) {
        currClass = currClass + ' ' + arr[i];
    }
    root.attr("class", currClass);
    if (target.checked) {
        $(selector).each(function () {
            if (this == target) return;
            if (!this.checked) return;
            this.checked = false;
            MvcControlsToolkit_TreeView_SelectLevel(this, selector);
        });
    }
}

function MvcControlsToolkit_TreeView_PrepareTemplates(root, templatesId) {
    eval(root + MvcControlsToolkit_TreeView_TemplateSriptPrefix + ' = new Array();');
    eval(root + MvcControlsToolkit_TreeView_TemplateHtmlPrefix + ' = new Array();');

    for (var i = 0; i < templatesId.length; i++) {
        var templateId = templatesId[i];
        var templateElement = $('#' + templateId);
        var allJavascript = CollectAllScriptsInelement(templateId);
        eval(root + MvcControlsToolkit_TreeView_TemplateSriptPrefix + '[i] = allJavascript;');

        templateElement.find('script').remove();

        var temp = null;
        if (templateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = templateElement.text();
        }
        else {
            temp = $('<div>').append(templateElement.children().clone()).remove().html();
        }
        eval(root + MvcControlsToolkit_TreeView_TemplateHtmlPrefix + '[i] = temp;');




    }
    $('#' + root + MvcControlsToolkit_TreeView_TemplatesPostfix).remove();


}

function MvcControlsToolkit_TreeView_AddNewChoice(rootName, templateChosen, item, after) {

    if (eval("typeof  " + rootName + MvcControlsToolkit_TreeView_RootNamePostfix + " === 'undefined'")) return;
    var root = eval(rootName + MvcControlsToolkit_TreeView_RootNamePostfix);
    var rootElement = $('#' + root + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', templateChosen);
    rootElement.trigger('itemChange', changeData);
    if (changeData.Cancel == true) return;
    var elementNumber = parseInt(document.getElementById(root + MvcControlsToolkit_TreeView_ItemsCountPostfix).value);

    var templateSymbol = new RegExp(eval(root + MvcControlsToolkit_TreeView_TemplateSymbolPrefix).source + templateChosen, 'g');

    var allJavascript = eval(root + MvcControlsToolkit_TreeView_TemplateSriptPrefix + '[templateChosen]').replace(templateSymbol, elementNumber + '');
    var allHtml = eval(root + MvcControlsToolkit_TreeView_TemplateHtmlPrefix + '[templateChosen]').replace(templateSymbol, elementNumber + '');

    var canSort = eval(root + MvcControlsToolkit_TreeView_CanSortPrefix);


    document.getElementById(root + MvcControlsToolkit_TreeView_ItemsCountPostfix).value = (elementNumber + 1) + '';



    var result = null;
    if (item == null)
        result = $(allHtml).appendTo('#' + rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    else {
        if (after != true) result = $(allHtml).insertBefore(item);
        else result = $(allHtml).insertAfter(item);
    }

    var initFields = result.find('.MvcCT_init_info_' + root).detach().children();
    initFields.insertAfter('#' + root + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    result.find('.level-select_' + root).change(
        function (event) {

            MvcControlsToolkit_TreeView_SelectLevel(event.target, '.level-select_' + root);
        }
    );
    jQuery.globalEval(allJavascript);
    if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {
        jQuery.validator.unobtrusive.parseExt('#' + result[0].id)
    }

    rootElement.treeview({
        add: result
    });
    MvcControlsToolkit_TreeView_UpdateFather(result[0], rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    if (canSort) {

        $('#' + rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix).sortable("refresh");
    }
    changeData = new MvcControlsToolkit_changeData(result, 'ItemCreated', templateChosen);
    rootElement.trigger('itemChange', changeData);
    return result;
}

function MvcControlsToolkit_TreeViewToggle(item) {
    if (item == null) return;
    var closedId = item.id.substring(0, item.id.lastIndexOf('_')) + MvcControlsToolkit_TreeView_ClosedPostfix;
    var closedStore = document.getElementById(closedId);
    if (closedStore == null) return;
    closedStore.value = $(item).hasClass($.treeview.classes.expandable) ? "True" : "False";

}

function MvcControlsToolkit_TreeView_UpdatePermutations(item, senderId) {
    if (item == null || item.length == 0) return;
    var nodeName = item.attr('id');
    if (nodeName == null) return;
    var place = nodeName.lastIndexOf("_");
    if (place < 0) return;
    nodeName = nodeName.substring(0, place);
    var rootName = document.getElementById(nodeName + '___FatherOriginalId');
    rootName = rootName.value.replace(/[\$\.]/g, '_');
    var oldFather = $('#' + rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    MvcControlsToolkit_TreeView_UpdateFather(item[0], rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    MvcControlsToolkit_TreeView_UpdateFather(item[0], null);
    if (MvcControlsToolkit_TreeView_Updated) return;
    var root = eval(rootName + MvcControlsToolkit_TreeView_RootNamePostfix);
    var rootElement = $('#' + root + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    var changeData = new MvcControlsToolkit_changeData(item, 'ItemMoved', oldFather);
    rootElement.trigger('itemChange', changeData);
    MvcControlsToolkit_TreeView_Updated = true;
}

function MvcControlsToolkit_TreeView_AdjustToggleButton(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit) {

    var button = $('#' + id + MvcControlsToolkit_TreeView_ToggleEditPostfix);
    if (button.length == 0) return;

    if (eval("typeof " + id + MvcControlsToolkit_TreeView_ButtonModePostfix + " === 'undefined'"))
        return;

    var buttonMode = eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix);
    if (buttonMode == 0) {
        button.removeClass(cssClassUndoEdit);
        button.removeClass(cssClassRedoEdit);
        button.addClass(cssClassEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlEdit);
    }
    else if (buttonMode == 1) {
        button.removeClass(cssClassUndoEdit);
        button.removeClass(cssClassEdit);
        button.addClass(cssClassRedoEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlRedoEdit);
    }
    else {
        button.removeClass(cssClassRedoEdit);
        button.removeClass(cssClassEdit);
        button.addClass(cssClassUndoEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlUndoEdit);
    }

}

function MvcControlsToolkit_TreeView_ToggleEdit(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit) {

    if (eval("typeof " + id + MvcControlsToolkit_TreeView_ButtonModePostfix + " === 'undefined'"))
        return;

    var buttonMode = eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix);


    if (buttonMode == 0 || buttonMode == 1) {
        var edit = eval(id + MvcControlsToolkit_TreeView_SaveEditPostfix);
        $('#' +
            id + MvcControlsToolkit_TreeView_ContainerDisplayPostfix).before(edit);
        var display = $('#' +
            id + MvcControlsToolkit_TreeView_ContainerDisplayPostfix).detach();
        eval(id + MvcControlsToolkit_TreeView_SaveDisplayPostfix + ' = display;');
        eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix + ' = 2;');


        document.getElementById(id + MvcControlsToolkit_TreeView_IsEditPostFix).value = 'True';
    }
    else {
        var display = eval(id + MvcControlsToolkit_TreeView_SaveDisplayPostfix);
        $('#' +
            id + MvcControlsToolkit_TreeView_ContainerEditPostfix).before(display);
        var edit = $('#' +
            id + MvcControlsToolkit_TreeView_ContainerEditPostfix).detach();
        eval(id + MvcControlsToolkit_TreeView_SaveEditPostfix + ' = edit;');
        eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix + ' = 1;');

        document.getElementById(id + MvcControlsToolkit_TreeView_IsEditPostFix).value = 'False';
    }
    MvcControlsToolkit_TreeView_AdjustToggleButton(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit);
}

function MvcControlsToolkit_TreeView_UpdateFather(item, fatherId) {
    if (item == null && fatherId == null) return;
    var root = null;
    var parentId = fatherId;
    if (parentId == null) {
        root = item.parentNode;
        parentId = root.getAttribute('id');
    }
    else {
        root = document.getElementById(parentId);
    }
    var place = parentId.lastIndexOf("_");
    if (place < 0) return;
    var parentName = parentId.substring(0, place);
    var itemsHandleName = parentName + "_handle";

    var countSonsField = document.getElementById(parentName + '___SonNumber');
    if (countSonsField == null) return;
    countSonsField.value = root.childNodes.length + '';

    var originaIdField = document.getElementById(parentName + '___OriginalId');
    if (originaIdField == null) return;
    var rootName = originaIdField.value;

    var nodeName = null;
    var placeAsSonField = null;
    var fatherNameField = null;
    for (i = 0; i < root.childNodes.length; i++) {

        var nodeId = root.childNodes[i].getAttribute('id');
        place = nodeId.lastIndexOf("_");
        if (place < 0) continue;
        nodeName = nodeId.substring(0, place);

        placeAsSonField = document.getElementById(nodeName + '___PositionAsSon');
        placeAsSonField.value = i + '';

        fatherNameField = document.getElementById(nodeName + '___FatherOriginalId');
        fatherNameField.value = rootName;
        if (root.childNodes[i] == item) {
            place = item.id.lastIndexOf("_");


            var innerContainer = $(document.getElementById(item.id.substring(0, place) + "___Item_SubContainer"));

            if (!innerContainer.hasClass(itemsHandleName)) {
                innerContainer.removeClass();
                innerContainer.addClass(itemsHandleName);
            }
            var current = $(item);
            if (i != root.childNodes.length - 1
                ) {
                current
                .removeClass($.treeview.classes.last)
				.removeClass($.treeview.classes.lastCollapsable)
				.removeClass($.treeview.classes.lastExpandable)
			    .find(">.hitarea")
				    .replaceClass($.treeview.classes.lastCollapsableHitarea, $.treeview.classes.collapsableHitarea)
				    .replaceClass($.treeview.classes.lastExpandableHitarea, $.treeview.classes.expandableHitarea);

            }
        }
    }
    if (root.childNodes.length > 0) {
        var last = $(root.childNodes[root.childNodes.length - 1]);
        if (!last.hasClass($.treeview.classes.last)) {
            last.addClass($.treeview.classes.last)
				.filter("." + $.treeview.classes.expandable).replaceClass($.treeview.classes.last, $.treeview.classes.lastExpandable).end()
				.find(">.hitarea").replaceClass($.treeview.classes.expandableHitarea, $.treeview.classes.lastExpandableHitarea).end()
				.filter("." + $.treeview.classes.collapsable).replaceClass($.treeview.classes.last, $.treeview.classes.lastCollapsable).end()
				.find(">.hitarea").replaceClass($.treeview.classes.collapsableHitarea, $.treeview.classes.lastCollapsableHitarea);
        }
    }
    if (root.childNodes.length > 1) {
        var semiLast = $(root.childNodes[root.childNodes.length - 2]);

        semiLast
            .removeClass($.treeview.classes.last)
				.removeClass($.treeview.classes.lastCollapsable)
				.removeClass($.treeview.classes.lastExpandable)
			    .find(">.hitarea")
				    .replaceClass($.treeview.classes.lastCollapsableHitarea, $.treeview.classes.collapsableHitarea)
				    .replaceClass($.treeview.classes.lastExpandableHitarea, $.treeview.classes.expandableHitarea);

    }


}

function MvcControlsToolkit_TreeView_ItemName(item) {
    var itemRoot = item.id;
    var place = itemRoot.lastIndexOf("_");
    if (place < 0) return null;

    return itemRoot.substring(0, place) + '___Item_Container';
}

function MvcControlsToolkit_TreeView_AddNew(root, templateChosen, item, after) {
    if (typeof (root) != 'string') root = MvcControlsToolkit_TreeView_ItemName(root);
    var place = root.lastIndexOf("___");
    if (place < 0) return;
    var root = root.substring(0, place);

    return MvcControlsToolkit_TreeView_AddNewChoice(root, templateChosen, item, after);



}

function MvcControlsToolkit_TreeView_Delete(node) {
    var itemName = node;
    if (typeof (itemName) != 'string') itemName = MvcControlsToolkit_TreeView_ItemName(itemName);
    var place = itemName.lastIndexOf("___");
    if (place < 0) return;
    itemName = itemName.substring(0, place);

    var item = document.getElementById(itemName + MvcControlsToolkit_TreeView_ContainerPrefix);
    var jItem = $(item);
    var rootName = document.getElementById(itemName + '___FatherOriginalId');
    rootName = rootName.value.replace(/[\$\.]/g, '_');
    var root = eval(rootName + MvcControlsToolkit_TreeView_RootNamePostfix);
    var rootElement = $('#' + root + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    var changeData = new MvcControlsToolkit_changeData(jItem, 'ItemDeleting', 0);
    rootElement.trigger('itemChange', changeData);
    if (changeData.Cancel == true) return;
    $(item).remove();
    MvcControlsToolkit_TreeView_UpdateFather(item, rootName + MvcControlsToolkit_TreeView_ItemsContainerPrefix);
    changeData = new MvcControlsToolkit_changeData(jItem, 'ItemDeleted', 0);
    rootElement.trigger('itemChange', changeData);

}

function MvcControlsToolkit_TreeView_Move(item, target, after) {
    if (after != true) $(item).insertBefore(target);
    else $(item).insertAfter(target);
    MvcControlsToolkit_TreeView_Updated = false;
    MvcControlsToolkit_TreeView_UpdatePermutations($(item), -1);
}

function MvcControlsToolkit_TreeView_MoveAppend(item, target) {
    $(item).appendTo($(target).find('>.mvcct-items-container'));
    MvcControlsToolkit_TreeView_Updated = false;
    MvcControlsToolkit_TreeView_UpdatePermutations($(item), -1);
}
