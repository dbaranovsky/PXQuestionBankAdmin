// XBookWidget
var PxXbookMediaWidget = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        initialized: false,
        pluginName: "XBookWidget",
        dataKey: "XBookWidget",
        tocWidthAdjustment: 188,
        xbapi: {},
        events: {
            widgetRpcLoaded: 'widget_rpc_loaded',
            postTabChange: 'MenuWidget.PostRequestTabChange',
            tocItemSelected: 'toc_item_selected'
        },
        defaults: {
            readOnly: false
        },
        state: {
            assignmentItemId: '',
            itemId: '',
            selectedTocItem: '',
            isExpanded: false
        },
        sel: {
        },
        // private functions
        fn: {
            handleRoute: function (state, func, args) {
                if (XbookRoutes[func] !== undefined) {
                    XbookRoutes[func].apply(this, [args]);
                }
            },
            getRpc: function (remoteUrl, container) {
                return new easyXDM.Rpc({
                    remote: remoteUrl,
                    onReady: function () {
                        PxPage.log(container + ' is ready for communication');
                    },
                    container: container,
                    props: {
                        id: "xb-component-frame",
                        scrolling: "no"
                    }
                }, {
                    local: {
                        /** Course Links VIEW-EDITABLE **/
                        CourseLinkEdit: function (actionName, url) {
                            if (actionName === 'EDIT LINKS') {
                                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['links', 'courseLinks']);
                            } else if (typeof (url) !== 'function') {
                                window.open(url);
                            }
                        },
                        CourseLinkSaveComplete: function () {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['links']);
                            PxXbookWidget.courselinks_reload();
                        },
                        /** CourseLinks  VIEW-NOT EDITABLE**/
                        CourseLinkClose: function (buttonName) {
                            if (buttonName.toUpperCase() === 'CANCEL') {
                                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['links']);
                            } else if (buttonName.toUpperCase() === 'SAVE') {
                                $(PxXbook.sel.widgets.EditCourseLinks).dialog('close');
                            }
                        },

                        /** TOC **/
                        TocItemClicked: function (itemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['contentarea', 'displayContentItem', { itemid: itemId}]);
                        },
                        GearboxItemClicked: function (selectedOption, itemId) {
                            switch (selectedOption.toUpperCase()) {
                                case 'ASSIGN':
                                    PxXbookWidget.toc_displayAddAssignment('', itemId);
                                    break;
                                case 'HIDE-SHOW':
                                    PxXbookWidget.toc_displayShowHide();
                                    break;
                                case "ADD":
                                    $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayAddContentDialog', { itemid: _static.state.selectedTocItem}]);
                                    break;
                            }
                        },
                        AssignItemButtonClicked: function (buttonName) {
                            switch (buttonName.toUpperCase()) {
                                case 'DONE':
                                    PxXbookWidget.toc_hideExpansion();
                                    $(PxPage.switchboard).trigger('COMPONENT_HASH', ['assignments']);
                                    break;
                                case 'CLOSE':
                                default:
                                    PxXbookWidget.toc_hideExpansion();
                                    break;
                            }
                        },
                        HideShowButtonClicked: function (buttonName) {
                            PxXbookWidget.toc_hideExpansion();
                        },
                        DeleteIconClicked: function (itemId) {
                            PxPage.log('DeleteIconClicked: ' + itemId);
                        },
                        AssignmentSaveComplete: function () {
                            PxPage.log('AssignmentSaveComplete');
                        },
                        AssignmentDeleted: function () {
                            PxXbookWidget.toc_assignment_reload();
                            //                            PxGradebook.Reload();
                        },
                        /** MainContentArea **/
                        NavigationButtonClicked: function (direction) {
                            if (direction === 'PREV') {
                                PxXbookWidget.toc_prev();
                            } else if (direction === 'NEXT') {
                                PxXbookWidget.toc_next();
                            }
                        },
                        FnEButtonClicked: function () {
                            PxXbookWidget.mca_fullscreen();
                        },
                        ContentItemClicked: function (itemId, fragmentId) {
                            //Not sure what this is for.
                            PxPage.log('ContentItemClicked: ' + itemId + ' ' + fragmentId);
                        },
                        RelatedContentDetailsState: function (state) {
                            //Not sure what this is for. 
                            PxPage.log('RelatedContentDetailsState: ' + state);
                        },
                        /** RecentlyViewedSnapshot **/
                        RecentlyViewedClicked: function (dataItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'toc_navigateTo', { itemid: dataItemId}]);
                        },
                        /** Carousel **/
                        PageJumperActivated: function (itemId, pageNumber) { },
                        CarouselImageClicked: function (itemId) {
                            PxPage.log('CarouselImageClicked ItemID: ' + itemId);
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'toc_navigateTo', { itemid: itemId}]);
                        },
                        SimpleError: function (errorMessage) {

                        },
                        TocClicked: function () {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'toc_navigateTo']);
                        },

                        /** AssignmentList **/
                        AssignmentListItemClicked: function (assignmentId, itemId) {
                            PxPage.log('AssignmentListItemClicked: ' + assignmentId + ' ' + itemId);
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'toc_navigateTo', { itemid: itemId}]);
                        },
                        AssignmentListEditClicked: function (actionName, assignmentItemId, itemId) {
                            _static.state.assignmentItemId = assignmentItemId;
                            if (actionName.toUpperCase() === 'EDIT') {
                                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc']);
                                PxXbookWidget.toc_displayAddAssignment(assignmentItemId);
                            }
                        },

                        /** AssignmentSummary **/
                        AssignmentSummaryClicked: function (dataItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['assignments']);
                        },

                        /** RecentActivity **/
                        RecentActivityClicked: function (dataItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['gradebook']);
                        },

                        /** LearningCurve **/
                        LearningCurveClicked: function () {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['xbook']);
                            PxXbookWidget.toc_filterLearningCurve();
                        },

                        /** RecentSubmissions **/
                        RecentSubmissionsClicked: function (state, enrollmentId, assignmentItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['gradebook']);
                        },
                        CommentRequested: function (type, open) {
                            if ($(PxXbook.sel.widgets.TOC).is(':visible'))
                                $(PxXbook.sel.widgets.TOC).hide();

                            PxXbookWidget.showComment(true);
                        }
                    },
                    remote: {
                        /** TOC **/
                        TOC_NavigateTo: function (itemId) { },
                        TOC_Reload: function (itemId) { },
                        TOC_Assignments_Reload: function () { },
                        TOC_Next: function () { },
                        TOC_Prev: function () { },
                        TOC_DisplayAssignment: function (assignmentId, itemId) { },
                        TOC_DisplayHideShow: function () { },
                        TOC_DisplayNormal: function () { },
                        TOC_LearningCurve: function () { },
                        TOC_GetSelectedItem: function () { },
                        TOC_AddElibraryItem: function (itemId, parentId, sequence, clone) { },

                        /*** Main Content Area ***/
                        UpdateContent: function (itemId, fragmentId) { },
                        ShowComment: function (show) { },

                        /*** Course Links ***/
                        CourseLinks_Reload: function () { },

                        /*** Assignments ***/
                        AssignmentList_Reload: function () { },

                        /*** Carousel ***/
                        Hide_PageJumper: function () { },
                        Carousel_Reload: function () { },

                        /*** Recently Viewed Snapshot ***/
                        RecentlyViewed_Reload: function () { },

                        /*** Class Activity Snapshot ***/
                        RecentSubmissions_Reload: function () { },

                        /*** Class Assignments Snapshot ***/
                        AssignmentSummary_Reload: function () { }


                    }
                });
            },
            reloadComponent: function (index, componentId) {
                var xb_component = $("#" + arguments[1] + " .xb-component");

                if (xb_component == undefined || xb_component == null) {
                    PxPage.log('Could not find component element');
                    return;
                }

                var xb_component_id = xb_component.attr('id');

                if (xb_component_id == undefined || xb_component_id == null) {
                    PxPage.log('Could not find component id');
                    return;
                }

                var rpc = _static.xbapi[xb_component_id];

                if (rpc == undefined || rpc == null) {
                    PxPage.log('Could not find RPC Object');
                    return;
                }

                switch (componentId) {
                    case 'PX_RecentlyViewedWidget':
                        rpc.RecentlyViewed_Reload();
                        break;
                    case 'PX_RecentSubmissionsWidget':
                        rpc.RecentSubmissions_Reload();
                        break;
                    case 'PX_AssignmentSummaryWidget':
                        rpc.AssignmentSummary_Reload();
                        break;
                    case 'PX_CourseLinksWidget':
                        rpc.CourseLinks_Reload();
                        break;
                    case 'PX_TOCWidget':
                        var itemid = rpc.TOC_GetSelectedItem();
                        rpc.TOC_Reload(itemid);
                        break;
                    case 'PX_AssignmentListWidget':
                        rpc.AssignmentList_Reload();
                        break;
                    case 'PX_ClassSubmissionWidget':
                        break;
                };
            },

            // determine what components exits on each tab
            reloadComponents: function (event, requiredLoad, componentIds) {
                // If the components were just loaded there is no reason for the component to be reloaded
                if (!requiredLoad)
                    return;

                $.each(componentIds || [], _static.fn.reloadComponent);

            },

            //Handles the initialization of the easyXDM iframes for agilix components
            initWidget: function () {
                $(PxPage.switchboard).unbind("MenuWidget.PostRequestTabChange").
                                        bind("MenuWidget.PostRequestTabChange", _static.fn.reloadComponents);
                var target = $(this);
                if (target.length == 0) {
                    PxPage.log("no xbapi targets");
                    return;
                }

                if (target.length > 0) {
                    target.each(function (i, e) {
                        PxPage.log("xbapi target: " + $(e).attr("rel"));
                        var frame = $(e).find("#xb-component-frame");
                        if (frame.length === 0) {
                            var componentid = $(e).attr("id");
                            if (_static.xbapi[componentid] !== undefined) {
                                _static.xbapi[componentid].destroy();
                            }
                            _static.xbapi[componentid] = _static.fn.getRpc($(e).attr("rel"), componentid);
                            $(this).trigger(_static.events.widgetRpcLoaded, [$(this).parentsUntil('.zone').last().attr('id')]);
                        }
                        else {
                            PxPage.log("xbapi already initialized");
                        }
                    });
                }
            }
        }
    };
    // The public interface for interacting with this plugin.
    $.fn.initXbookWidget = function () {
        //stop editcourselinks widget from initializing outside of this js file.  Needs to be initialized in the open of the dialog.
        if ($(this).parents(PxXbook.sel.widgets.EditCourseLinks).length == 0)
            return _static.fn.initWidget.apply(this, arguments);
    };

    return {
        init: function () {
            if (_static.initialized) {
                PxPage.log('Tried to reinitialize PxXbookWidget');
                return;
            }
            PxRoutes.AddComponentRoute('toc', null, _static.fn.handleRoute);
            PxRoutes.AddComponentRoute('links', null, _static.fn.handleRoute);
            _static.initialized = true;
        },
        displayAddContentDialog: function (parentid) {
            //Open the new content widget.
            PxCreateNewAssignment.DisplayCreateContentDialog(parentid);
        },
        toc_displayAddAssignment: function (assignmentId, itemId) {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocWidget.attr('id')].TOC_DisplayAssignment(assignmentId, itemId);
            if (!_static.state.isExpanded) {
                $(PxXbook.sel.widgets.TOC).css('width', $(PxXbook.sel.widgets.TOC).width() + _static.tocWidthAdjustment + 'px');
                $('#PX_XBOOK_TAB_ZONE_2').css('padding-left', parseInt($('#PX_XBOOK_TAB_ZONE_2').css('padding-left').replace('px', '')) + _static.tocWidthAdjustment + 'px');
                _static.state.isExpanded = true;
            }
        },
        toc_assignment_reload: function () {
            var tocComponent = $(PxXbook.sel.widgets.Assignments + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_Assignment_Reload();
        },
        toc_displayShowHide: function () {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_DisplayHideShow();
            if (!_static.state.isExpanded) {
                $(PxXbook.sel.widgets.TOC).css('width', $(PxXbook.sel.widgets.TOC).width() + _static.tocWidthAdjustment + 'px');
                $('#PX_XBOOK_TAB_ZONE_2').css('padding-left', parseInt($('#PX_XBOOK_TAB_ZONE_2').css('padding-left').replace('px', '')) + _static.tocWidthAdjustment + 'px');
                _static.state.isExpanded = true;
            }
        },
        toc_hideExpansion: function () {
            if (_static.state.isExpanded) {
                var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
                _static.xbapi[tocComponent.attr('id')].TOC_DisplayNormal();
                $(PxXbook.sel.widgets.TOC).css('width', $(PxXbook.sel.widgets.TOC).width() - _static.tocWidthAdjustment + 'px');
                $('#PX_XBOOK_TAB_ZONE_2').css('padding-left', parseInt($('#PX_XBOOK_TAB_ZONE_2').css('padding-left').replace('px', '')) - _static.tocWidthAdjustment + 'px');
                _static.state.isExpanded = false;
            }
        },
        toc_filterLearningCurve: function () {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_LearningCurve();
        },
        toc_refresh: function () {
            if ($("#PX_TOCWidget .xb-component").attr('id')) {
                var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
                _static.xbapi[tocComponent.attr('id')].TOC_Reload();
            }
        },
        toc_navigateTo: function (itemId) {
            if ($("#PX_TOCWidget .xb-component").attr('id')) {
                var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
                _static.xbapi[tocComponent.attr('id')].TOC_NavigateTo(itemId);
            }
        },
        toc_prev: function () {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_Prev();
        },
        toc_next: function () {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_Next();
        },
        toc_addElibraryItem: function (itemId, parentId, sequence, clone) {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_AddElibraryItem(itemId, parentId, '', clone);
            PxXbookWidget.toc_refresh();
        },
        mca_fullscreen: function () {
            if ($(PxXbook.sel.widgets.TOC).is(':visible')) {
                $(PxXbook.sel.widgets.TOC).hide();
                PxXbookWidget.showComment(true);
            }
            else {
                PxXbookWidget.showComment(false);
                $(PxXbook.sel.widgets.TOC).show();
            }
        },
        //TODO: we could probably move this, along with the state of the selected item in toc to Xbook. Think this over.
        displayContent: function (itemid) {
            _static.state.selectedTocItem = itemid;
            $(PxPage.switchboard).trigger(_static.events.tocItemSelected, [itemid]);
        },
        carousel_hidePageJumper: function () {
            var carouselComponent = $(PxXbook.sel.widgets.Carousel + ' .xb-component');
            _static.xbapi[carouselComponent.arrt('id')].Hide_PageJumper();
            _static.xbapi[carouselComponent.arrt('id')].Carousel_Reload();
        },
        courselinks_reload: function () {
            var courseLinkWidget = $(PxXbook.sel.widgets.CourseLinks + ' .xb-component');
            _static.xbapi[courseLinkWidget.attr('id')].CourseLinks_Reload();
        },
        showComment: function (show) {
            //            var tocComponent = $(PxXbook.sel.widgets.MainContentArea + ' .xb-component');
            //            _static.xbapi[tocComponent.attr('id')].ShowComment(show);
        },
        editCourseLinks: function () {
            var dialogDiv = $(PxXbook.sel.widgets.EditCourseLinks);
            $(dialogDiv).dialog({
                width: 650,
                height: 300,
                minWidth: 650,
                minHeight: 300,
                modal: true,
                draggable: false,
                closeOnEscape: true,
                open: function (event, ui) {
                    _static.fn.initWidget.apply($(PxXbook.sel.widgets.EditCourseLinks + ' .xb-component'));
                    $('.ui-dialog-titlebar-close').click(function () {
                        $(PxPage.switchboard).trigger('COMPONENT_HASH', ['links']);
                    });
                },
                close: function (event, ui) {
                }
            });
        },
        closeCourseLinks: function () {
            if ($(PxXbook.sel.widgets.EditCourseLinks).parent().hasClass('ui-dialog')) {
                $(PxXbook.sel.widgets.EditCourseLinks).dialog('destroy');
                var editCourseLinksComponent = $(PxXbook.sel.widgets.EditCourseLinks + ' .xb-component');
                _static.xbapi[editCourseLinksComponent.attr('id')].destroy();
            }
        }
    };
} (jQuery);