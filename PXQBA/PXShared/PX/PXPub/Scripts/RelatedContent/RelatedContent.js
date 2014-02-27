// RelatedContent
var PxRelatedContent = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "RelatedContent",
        dataKey: "RelatedContent",
        insideClick: false,
        isDragging: false,
        defaults: {
            readOnly: false
        },
        events: {
            dragStarted: 'dragstarted',
            dragStopped: 'dragstopped'
        },
        sel: {
            editor: "#eLibraryEditor",
            content: "#eLibraryContent",
            link: "#eLibraryButton",
            related_content: ".related-content",
            relateditem: '.relatedItem',
            relatedContentButton: '#eLibraryButton .relatedContentButton',
            dragParent: ''
        },
        fn: {
            //Sets up the contentdragged event for all items in the related content control
            initRelatedItems: function () {
                $(document).off('click', _static.sel.relatedContentButton).on('click', _static.sel.relatedContentButton, _static.fn.toggleDropDown);

                $(_static.sel.relateditem).draggable({
                    helper: function (event) {
                        var clone = $(this).clone().addClass('dragging');
                        $(_static.sel.dragParent).append(clone);
                        return clone;
                    },
                    //                    iframeFix: true,
                    revert: 'invalid',
                    revertDuration: 100,
                    zIndex: 2001,
                    delay: 50,
                    start: function (event, ui) {
                        _static.isDragging = true;
                        $(PxPage.switchboard).trigger(_static.events.dragStarted);
                    },
                    stop: function (event, ui) {
                        _static.isDragging = false;
                        $(PxPage.switchboard).trigger(_static.events.dragStopped);
                    }
                });
            },
            toggleDropDown: function (event) {
                if (event) {
                    event.preventDefault();
                }
                var count = 0,
                    linkText = $("#eLibraryButton").text();

                if ((linkText.indexOf("(") > 0) || linkText.indexOf(")") > 0) {
                    count = linkText.split("(")[1].split(")")[0];
                }

                if (!$(_static.sel.editor).is(':visible')) {
                    if (count != "0") {
                        if ($(_static.sel.relatedContentButton).is(":disabled")) $(_static.sel.relatedContentButton).removeClass("disabled");
                        $(_static.sel.related_content).show();
                        $(_static.sel.editor).show();
                        _static.fn.registerMouseLeave();

                        //show the Related Content Persitent Tooltip
                        $(PxPage.switchboard).trigger('ToggleRelatedContentToolTip', [true]);
                    }
                    else {
                        if (!$(_static.sel.relatedContentButton).is(":disabled")) $(_static.sel.relatedContentButton).addClass("disabled");
                    }
                }
                else {
                    $(_static.sel.editor).hide();
                    _static.fn.unregisterMouseLeave();
                    //hide the Related Content Persitent Tooltip
                    $(PxPage.switchboard).trigger('ToggleRelatedContentToolTip', [false]);
                }
            },
            //Handlers mouseleave event to check if the related content area should be closed.
            checkContentClose: function (event) {
                //If we are in the middle of a drag drop, don't close until dragging is done 
                if (!_static.isDragging) {
                    _static.fn.toggleDropDown();
                } else {
                    $(PxPage.switchboard).bind(_static.events.dragStopped, _static.fn.closeOnDragStop);
                }
            },
            //Handles closing of related content area when cursor has left the related content area while dragging an item.
            closeOnDragStop: function (event) {
                _static.fn.toggleDropDown();
                $(PxPage.switchboard).unbind(_static.events.dragStopped, _static.fn.closeOnDragStop);
            },
            //Removes all event handlers setup to listen for a click outside the related content area.
            unregisterMouseLeave: function () {
                $(_static.sel.content).unbind('mouseleave', _static.fn.checkContentClose);
            },
            //Sets up event listeners that will close the related content window if the mouse leaves the related content area.
            registerMouseLeave: function () {
                $(_static.sel.content).bind('mouseleave', _static.fn.checkContentClose);
            },
            getRelatedContent: function () {
                var args = { itemId: $(".item-id").val() };

                //ajax call to get related items
                $.post(PxPage.Routes.get_related_items, args, function (response) {
                    $(_static.sel.content).html(response);
                });
            }
        }
    };

    return {
        DragStarted: _static.events.dragStarted,
        DragStopped: _static.events.dragStopped,
        //Init: sets the dragparentSelector.
        // dragParentSelector: when the drag event is fired, a copy of the dragged element will be appened to this selector.
        Init: function (dragParentSelector) {
            _static.sel.dragParent = dragParentSelector;
        },
        //InitRelatedItems: setup the related items to be draggable.
        InitRelatedItems: function () {
            _static.fn.initRelatedItems();
        },
        getRelatedContent: function () {
            _static.fn.getRelatedContent();
        },
        toggleDropDown: function () {
            _static.fn.toggleDropDown();
        }
    };
} (jQuery);
