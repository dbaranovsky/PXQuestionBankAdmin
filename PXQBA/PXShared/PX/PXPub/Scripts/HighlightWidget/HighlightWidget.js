//The HighlightWidget governs when and how highlights are created inside of a container
(function ($) {
    var defaults = {
        minSelection: 10,
        colorClasses: ['color-1', 'color-2', 'color-3', 'color-4'],
        Context: { IsInstructor: 'false' },
        highlightKey: {}
    };

    if (window.PxPage) {
        defaults.Context = PxPage.Context;
    }

    var pluginName = "HighlightWidget";
    var dataKey = "HighlightWidget";
    var bindSuffix = ".HighlightWidget";
    var menuSel = "#highlight-widget-menu";
    var menuId = "highlight-widget-menu";
    var nextHid = 0;

    //private methods
    var _getXPath,
        _getRangeDetail,
        _getRangeInfo,
        _getRangeString,
        _createHighlight,
        _storeHighlight,
        _createHighlightMenuOldHighlight,
        _createHighlightMenuNewHighlight,
        _menuItemClicked,
        _getActiveColor,
        _setActiveColor,
        _changeHighlightColor,
        _highlightClicked,
        _storeHighlightColor,
        _highlightHoverIn,
        _highlightHoverOut,
        _clearHighlights;

    //Get the XPath of the node passed in
    _getXPath = function (node) {
        var comp, comps = [];
        var parent = null;
        var xpath = '';
        var getPos = function (node) {
            var position = 1, curNode;
            if (node.nodeType == 2) { // 2=Node.ATTRIBUTE_NODE
                return null;
            }
            for (curNode = node.previousSibling; curNode; curNode = curNode.previousSibling) {
                if (curNode.nodeName == node.nodeName) {
                    ++position;
                }
            }
            return position;
        }

        //Apparently IE8 doesn't use Document as the type
        if ((typeof Document !== 'undefined' && node instanceof Document) || (typeof HTMLDocument !== 'undefined' && node instanceof HTMLDocument)) {
            return '/';
        }

        for (; node && node.tagName != "BODY"; node = node.nodeType == 2 ? node.ownerElement : node.parentNode) { // 2 = Node.ATTRIBUTE_NODE
            comp = comps[comps.length] = {};
            switch (node.nodeType) {
                case 3: //Node.TEXT_NODE:
                    comp.name = 'text()';
                    break;
                case 2: //Node.ATTRIBUTE_NODE:
                    comp.name = '@' + node.nodeName;
                    break;
                case 7: //Node.PROCESSING_INSTRUCTION_NODE:
                    comp.name = 'processing-instruction()';
                    break;
                case 8: //Node.COMMENT_NODE:
                    comp.name = 'comment()';
                    break;
                case 1: //Node.ELEMENT_NODE:
                    comp.name = node.nodeName;
                    break;
            }
            comp.position = getPos(node);
        }

        for (var i = comps.length - 1; i >= 0; i--) {
            comp = comps[i];
            xpath += '/' + comp.name;
            if (comp.position != null) {
                xpath += '[' + comp.position + ']';
            }
        }
        return xpath;
    }

    _getRangeDetail = function (range) {

        var rangeDetail = new Object();

        if (range.startContainer) {
            rangeDetail.start = _getXPath(range.startContainer);
            rangeDetail.end = _getXPath(range.endContainer)
            rangeDetail.startOffset = range.startOffset;
            rangeDetail.endOffset = range.endOffset;
            rangeDetail.range = range;
            if (range.text)
                rangeDetail.text = range.text;
            else
                rangeDetail.text = range.toString();
        }

        return (rangeDetail);
    }

    //returns the information of text selected by the user
    _getRangeInfo = function (iframe) {
        var userSelection = null;
        var range = null;
        var text = "";
        var win = window;
        var doc = document;
        var rangeDetail;

        try {
            $("#spnLinkError").hide();
            if (iframe) {
                win = iframe.contentWindow;
                doc = win.document;
            }

            var htmlText = null;
            var clonedSelection = null;
            
            if (rangy.getSelection) {
                userSelection = rangy.getSelection(win);
            }

            if (userSelection.getRangeAt) {
                if (userSelection.rangeCount > 0) {
                    range = userSelection.getRangeAt(0);
                    if (range) {
                        clonedSelection = range.cloneContents();
                        userSelection.removeAllRanges();
                        userSelection.addRange(range);
                        rangeDetail = _getRangeDetail(range);
                        range.detach();
                    }
                }
            }

        }
        catch (e) {
        }
        return rangeDetail;
    };

    //stores a new highlight and modifies the necessary markup
    _createHighlight = function (event) {
        if ((PxPage.Context.IsProductCourse === "true") || $(event.target).is("input") || $(event.target).is("textarea")) {
            return;
        }
        var data = $(this).data(dataKey);
        var settings = data.settings;
        var rangeDetail = _getRangeInfo(data.settings.iframe);

        if (rangeDetail && rangeDetail.text.length >= settings.minSelection) {
            data.target.css('overflow', 'hidden');
            var menu = _createHighlightMenuNewHighlight(data, "", rangeDetail).show();
            var maximumRight = $(data.settings.iframe).width() - $(menu).width() - 310; // 310 is for padding and etc.                 

            $(menu).position({ my: "left bottom", at: "right top", of: event });

            if (event.clientX > maximumRight)
                $(menu).css("left", maximumRight);
        }
        else {
            $('#highlightList').css({ bottom: "0px" }).show();
            data.target.find(menuSel).hide();
        }
        // Shouldn't be setting this here
        // var highlightCount = data.target.find('.highlight').length;
        // $(".highlight-count").text(highlightCount);
    };

    //sends a post request to the given url passing the configured key values and the given search text. Server side
    //handler should create a new highlight attached the entity associated with the key values
    _storeHighlight = function (data, key, rangeDetail, color, onSave) {
        key.HighlightText = rangeDetail.text;
        key.HighlightColor = color;
        key.start = rangeDetail.start;
        key.startOffset = rangeDetail.startOffset;
        key.end = rangeDetail.end;
        key.endOffset = rangeDetail.endOffset;

        if (data.settings.storeUrl) {
            $.post(data.settings.storeUrl, key, function (response) {
                onSave(response);
            });
        }
        else {
            onSave(++nextHid);
        }
    };

    //sends a post request to the given url to update the highlight color
    _storeHighlightColor = function (data, highlightId, color) {
        if (data.settings.updateColorUrl) {
            $.post(data.settings.updateColorUrl, { HighlightId: highlightId, Color: color }, function (response) {
            });
        }
    };

    //Creates pop-up menu element for newly selected highlight
    _createHighlightMenuNewHighlight = function (data, id, rangeDetail) {
        var menu = data.target.find(menuSel);
        var activeColor = _getActiveColor(data);

        if (menu.length != 0) {
            $(menu).remove();
        }
            
        data.target.append('<div id="' + menuId + '" class="ignore-autosave"></div>');
        menu = data.target.find(menuSel);
        
        menu.append('<span id="highlight-widget-highlight" class="menu-item">Highlight</span>');

        $.each(data.settings.colorClasses, function (i, v) {
            var cls = "menu-item color";
            if (v === activeColor) {
                cls = cls + " active";
            }
            menu.append('<span id="highlight-widget-' + v + '" class="' + cls + '"><span style="display:none;">' + i + '</span></span>');
        });


        menu.append('<span id="highlight-widget-add-note" class="menu-item">Add Note</span>');
        menu.find(".menu-item").mouseup(function (e) { }).click(function (e) {
            var itemId = $(this).attr("id");
            _menuItemClicked(e, data, id, itemId, rangeDetail);
        });
        return menu;
    };

    //returns an element for previously existed highlights that acts as a menu that will appear on certain events
    _createHighlightMenuOldHighlight = function (data, id) {
        var menu = data.target.find(menuSel);
        var activeColor = _getActiveColor(data, id);

        if (menu.length != 0) {
            $(menu).remove();
        }

        data.target.append('<div id="' + menuId + '" class="ignore-autosave"></div>');
        menu = data.target.find(menuSel);

        menu.append('<span id="highlight-widget-id" style="display:none;">' + id + '</span>');

        var highlight = data.target.find("#highlight-" + id).first();
        var highlightBlock = $("#highlight-block-" + id);
        var notes = highlightBlock.find(".highlight-comment");
        var hasNotes = highlight.hasClass("has-notes");
        var isMine = highlight.hasClass("mine");
        var isLocked = highlight.hasClass("locked");
        var allowDelete = false, allowComment = false;

        if (PxPage.Context.IsInstructor == 'true' || (notes.not('.mine').length < 1 && !isLocked)) {
            // Allow delete if instructor or a note has no reply from other users and its not locked.
            allowDelete = true;
        }

        if (PxPage.Context.IsInstructor == 'true' || !isLocked) {
            // Allow comments if instructor or a note is not locked by instructor.
            allowComment = true;
        }

        // the extra span for highlight-widget-copy-container, causes the last color to give focues to copy, so the last color does not get applied
        //var copyElement = '<span id="highlight-widget-copy-container" style="position:relative;"><span id="highlight-widget-copy" class="menu-item">Copy</span></span>';
        var copyElement = '<span id="highlight-widget-copy" class="menu-item">Copy</span>';
        if (isLocked && !isMine && PxPage.Context.IsInstructor != 'true') {
            //menu.append(copyElement);
        }
        else {           
           // menu.append(copyElement);
            if (isMine) {
                $.each(data.settings.colorClasses, function (i, v) {
                    var cls = "menu-item color";
                    if (v === activeColor) {
                        cls = cls + " active";
                    }
                    menu.append('<span id="highlight-widget-' + v + '" class="' + cls + '"><span style="display:none;">' + i + '</span></span>');
                });
            }
            if (allowComment) {
                if (hasNotes && isMine && notes.length == 1) {
                    menu.append('<span id="highlight-widget-edit-note" class="menu-item existingHighlight">Edit Note</span>');
                }
                else if (hasNotes) {
                    menu.append('<span id="highlight-widget-reply-note" class="menu-item existingHighlight">Reply</span>');
                }
                else {
                    menu.append('<span id="highlight-widget-add-note" class="menu-item existingHighlight">Add Note</span>');
                }
            }
            if (allowDelete) {
                menu.append('<span id="highlight-widget-delete" class="menu-item"></span>');
            }
        }

        menu.find(".menu-item").mouseup(function (e) { }).click(function (e) {
            var itemId = $(this).attr("id");
            _menuItemClicked(e, data, id, itemId);
        });
        return menu;
    };

    //handles when an item in the highlight menu is clicked
    _menuItemClicked = function (e, data, menuId, itemId, rangeDetail) {
        var fromMenuForNewHighlight = (rangeDetail) ? true : false;
        var $this = data.target.find("#" + itemId);

        if ($this.hasClass("color")) {
            var index = $this.find("span").first().text();
            _setActiveColor(data, index);
            _changeHighlightColor(data, menuId, index);
            _storeHighlightColor(data, menuId, data.settings.colorClasses[index]);

            if (fromMenuForNewHighlight) {
                _storeHighlight(data, data.settings.highlightKey, rangeDetail, _getActiveColor(data), function (id) {
                    var cls = _getActiveColor(data) + " mine";
                    Highlighter.Highlight([{ rangeDetail: rangeDetail, id: id, className: cls, css: {}}], data.settings.iframe);
                });
            }
        }
        else if (itemId == "highlight-widget-add-note") {
            if (fromMenuForNewHighlight) {
                _storeHighlight(data, data.settings.highlightKey, rangeDetail, _getActiveColor(data), function (id) {
                    var cls = _getActiveColor(data) + " mine";
                    Highlighter.Highlight([{ rangeDetail: rangeDetail, id: id, className: cls, css: {}}], data.settings.iframe);

                    data.target.find(menuSel).hide();
                    if (data.settings.onAddNote && typeof (data.settings.onAddNote) === "function") {
                        data.settings.onAddNote(e, id, data.target.find('#highlight-' + id).text());
                    }

                });
            } else {
                data.target.find(menuSel).hide();
                if (data.settings.onAddNote && typeof (data.settings.onAddNote) === "function") {
                    data.settings.onAddNote(e, menuId, data.target.find('#highlight-' + menuId).text());
                }
            }
        }
        else if (itemId == "highlight-widget-reply-note") {
            data.target.find(menuSel).hide();
            if (data.settings.onReplyNote && typeof (data.settings.onReplyNote) === "function") {
                data.settings.onReplyNote(e, menuId);
            }
        }
        else if (itemId == "highlight-widget-edit-note") {
            data.target.find(menuSel).hide();
            var noteId = $("#highlight-block-" + menuId + " .highlight-comment.lastComment").attr("id").replace("note-", "");
            if (data.settings.onEditNote && typeof (data.settings.onEditNote) === "function") {
                data.settings.onEditNote(e, noteId);
            }
        }
        else if (itemId == "highlight-widget-copy") {
            data.target.find(menuSel).hide();
        }
        else if (itemId == "highlight-widget-delete") {
            data.target.find(menuSel).hide();
            if (data.settings.onDeleteHighlight && typeof (data.settings.onDeleteHighlight) === "function") {
                data.settings.onDeleteHighlight(menuId);
            }
        }
        else if (itemId == "highlight-widget-highlight") {
            _storeHighlight(data, data.settings.highlightKey, rangeDetail, _getActiveColor(data), function (id) {
                var cls = _getActiveColor(data) + " mine";
                Highlighter.Highlight([{ rangeDetail: rangeDetail, id: id, className: cls, css: {}}], data.settings.iframe);
            });
        }
        else {
            data.target.find(menuSel).hide();
        }
    };

    //returns the active color class.
    //if menuId is null and there is no menu, then the first colorClasses[0] is used
    //if menuId is null and there is a menu, then the existing active color class is used
    //if menuId is NOT null and the corresponding highlight has one of the colorClasses, then that color class is used
    _getActiveColor = function (data, menuId) {
        var menu = data.target.find(menuSel);
        var active = data.settings.colorClasses[0];

        if (menu.length > 0) {
            var index = menu.find(".menu-item.color.active span").text();
            active = data.settings.colorClasses[index];
        }

        if (menuId) {
            var highlight = data.target.find("#highlight-" + menuId).first();
            $.each(data.settings.colorClasses, function (i, v) {
                if (highlight.hasClass(v))
                    active = v;
            });
        }

        if (active == undefined || active == '' || active == null)
            active = data.settings.colorClasses[0];

        return active;
    };

    //changes the active color to be color at index, and returns the new active class
    _setActiveColor = function (data, index) {
        var menu = data.target.find(menuSel);
        menu.find(".menu-item.color.active").removeClass("active");

        var active = data.settings.colorClasses[index];

        menu.find("#highlight-widget-" + active).addClass("active");

        return active;
    };

    //changes the highlight color of every highlight attached to the menu
    _changeHighlightColor = function (data, menuId, index) {
        var color = data.settings.colorClasses[index];

        data.target.find("#highlight-" + menuId).each(function () {
            for (var i in data.settings.colorClasses) {
                if (!isNaN(i))
                    $(this).removeClass(data.settings.colorClasses[i]);
            }
            $(this).addClass(color);
        });
    };

    //when a highlight is clicked, the highlight menu is displayed
    _highlightClicked = function (e, data) {
        var highlightElement = $(this);
        var id = highlightElement.attr("id");
        id = id.substring(id.indexOf("-") + 1);
        var menu = _createHighlightMenuOldHighlight(data, id).show();
        var maximumRight = $(data.settings.iframe).width() - $(menu).width() - 310; // 310 is for padding and etc.                 
        $(menu).position({ my: "left bottom", at: "right top", of: e });

        if (e.clientX > maximumRight)
            $(menu).css("left", maximumRight);

        var settings = data.settings;

        if (data.settings.iframe) {
            data.target.css('overflow', 'hidden');
        }


        if (data.settings.onHighlightClicked && typeof (data.settings.onHighlightClicked) === "function") {
            data.settings.onHighlightClicked(e);
        }
    };

    //when a highlight is hovered over and it has notes, display the notes
    _highlightHoverIn = function (e, data) {

        if ($(this).hasClass("has-notes")) {
            var doc = $(data.settings.iframe).get(0).contentWindow.document;
            var currHighlights = $('span[id="' + this.id + '"]', doc);
            currHighlights.each(function () {
                if (/color-\d+/.test(this.className)) {
                    var colorClass = this.className.match(/color-\d+/)[0];
                    $(this).removeClass(colorClass).addClass(colorClass + '-hover');
                }
            });
            var targetNote = "div[id='" + $(this).attr("id").replace("highlight", "highlight-block") + "']";
            $(targetNote).addClass("highlight-block-hightlighted");
        }

        var inFne = !$("#fne-window").is(":hidden");
        if (inFne) {
            return;
        }

        if ($(this).hasClass("has-notes") && data.settings.showNotesOnHover) {
            var blockId = $(this).attr("id").replace("highlight", "commentListDiv");

            var block = data.target.find("#" + blockId);

            if (block.length == 0) {
                block = $("#" + blockId).clone().addClass("inline").appendTo(data.target).show();
            }
            block.position({ my: "center top", at: "center bottom", of: e, offset: "0 10", collision: "fit flip" });
        }
    };

    //when a highlight is no longer hovered and notes are being show, hide the notes
    _highlightHoverOut = function (e, data) {
        if ($(this).hasClass("has-notes")) {

            var doc = $(data.settings.iframe).get(0).contentWindow.document;
            var currHighlights = $('span[id="' + this.id + '"]', doc);
            currHighlights.each(function () {
                if (/color-\d+-hover/.test(this.className)) {
                    var colorClass = this.className.match(/color-\d+-hover/)[0];
                    var origColorClass = colorClass.replace("-hover", "")
                    $(this).removeClass(colorClass).addClass(origColorClass);
                }
            });

            var targetNote = "div[id='" + $(this).attr("id").replace("highlight", "highlight-block") + "']"; ;
            $(targetNote).removeClass("highlight-block-hightlighted");


            var blockId = $(this).attr("id").replace("highlight", "commentListDiv");
            var block = "#" + blockId;

            if (!(jQuery.browser.msie) && !(jQuery.browser.mozilla)) {
                if ($(block).length) {
                    //$(block).filter(
                    var delBlock = data.settings.iframe.contentWindow.$(block);
                    delBlock.remove();
                }
            }
            else {
                var block = data.target.find("#" + blockId);
                if (block.length)
                    block.remove();
            }

        }
    };

    //removes all highlights from the dom, is cls is not empty it is a class used to filter the highlights being removed.
    _clearHighlights = function (data, cls) {
        var selector = ".highlight";
        if (cls != "") {
            selector = selector + "." + cls;
        }

        data.target.find(selector).each(function (i, v) {
            $(this).removeClass().addClass("removedHL");
        });
    };


    //public interface
    var intrface = {
        init: function (options) {
            return this.each(function () {
                var settings = $.extend(true, {}, defaults, options),
                $this = $(this),
                data = $this.data(dataKey);

                //Initialize library that allows adds Range compatability to IE8
                if (rangy.getSelection === undefined) {
                    rangy.init();
                }
                if (!data) {
                    // alert("in init for hl widget");                   
                    $this.data(dataKey, {
                        target: $this,
                        settings: settings
                    });
                    data = $this.data(dataKey);

                    $this.bind("mouseup" + bindSuffix, _createHighlight);
                    $this.find(".highlight").live("click", function (e) {
                        e.stopPropagation();
                        _highlightClicked.apply(this, [e, data]);
                    }).live("mouseenter",
                        function (e) {
                            e.stopPropagation();
                            _highlightHoverIn.apply(this, [e, data]);
                        }).live("mouseleave",
                        function (e) {
                            e.stopPropagation();
                            _highlightHoverOut.apply(this, [e, data]);
                        }
                    );
                }
            });
        },
        // loads highlights from sever and applies to the content. This function is called from the init, and is execeuted only once to add existing highlights
        applyPreviousHighlights: function (highlights) {

            var $this = $(this),
            data = $this.data(dataKey);

            if (!highlights) {
                return;
            }

            var length = highlights.length;
            if (length == 0) {
                return;
            }

            var highlightSettings = [];
            for (var i = 0; i < length; i++) {
                if (highlights[i].Text != null) {
                    var rangeInfo = { start: highlights[i].Start, startOffset: highlights[i].StartOffset, end: highlights[i].End, endOffset: highlights[i].EndOffset, text: highlights[i].Text, status: highlights[i].Status };
                    highlightSettings.push({ rangeDetail: rangeInfo, id: highlights[i].HighlightId, className: highlights[i].ClassName, css: {} });
                }
            }

            intrface.clear.apply(this, [""]); // clear highlights before applying them
            if (data && data.target) {
                Highlighter.Highlight(highlightSettings, data.settings.iframe);
            }
        },
        clear: function (cls) {
            var $this = $(this),
            data = $this.data(dataKey);

            if (data) {
                _clearHighlights(data, cls);
            }
            else {
            }
        },
        getDocumentKey: function () {
            var key = {};

            key.ItemId = $("#highlight-new-container form input[name='ItemId']").val();
            key.SecondaryId = $("#highlight-new-container form input[name='SecondaryId']").val();
            key.PeerReviewId = $("#highlight-new-container form input[name='PeerReviewId']").val();
            key.HighlightType = $("#highlight-new-container form input[name='HighlightType']").val();
            key.ShowRubrics = $("#highlight-new-container form input[name='ShowRubrics']").val();
            key.IsInstructor = $("#highlight-new-container form input[name='IsInstructor']").val();
            key.RubricsList = $("#highlight-new-container form input[name='RubricsGuide']").val();
            return key;
        },
        destroy: function () {
            return this.each(function () {
                $(this).unbind(bindSuffix);
            });
        }
    };

    $.fn.HighlightWidget = function (method) {
        if (intrface[method]) {
            return intrface[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return intrface.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + pluginName);
        }
    };

} (jQuery));
