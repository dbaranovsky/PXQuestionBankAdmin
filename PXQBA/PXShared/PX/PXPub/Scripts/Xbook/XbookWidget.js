// XBookWidget
var PxXbookWidget = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        initialized: false,
        pluginName: "XBookWidget",
        dataKey: "XBookWidget",
        tocWidthAdjustment: 188,
        xbapi: {},
        widgetQueue: {},
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
            EditCourseLinks: '#PX_CourseLinksEditWidget',
            assignmentList: '#assignmentList'
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
                    onReady: function (blah) {
                        PxPage.log(container + ' is ready for communication');
                        $(PxPage.switchboard).trigger('xb-component-loaded', [container]);

                        if(container !== 'AssignmentListWidget'){
                            _static.fn.runWidgetQueue(container);
                        }

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
                                $(_static.sel.EditCourseLinks).dialog('close');
                            }
                        },

                        /** TOC **/
                        TOC_Loaded: function () {
                            //Run through any queued functions now that the TOC is loaded.
                            _static.fn.runWidgetQueue(PxXbook.sel.widgets.TOC.replace('#', ''));
                        },
                        TocItemClicked: function (itemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem', { itemid: itemId }, true]);
                            PxXbookWidget.displayContent(itemId);
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
                                    $(PxPage.switchboard).trigger('COMPONENT_HASH', ['content', 'create', { parentid: _static.state.selectedTocItem}]);
                                    break;
                            }
                        },
                        AssignItemButtonClicked: function (buttonName, assignmentId) {
                            switch (buttonName.toUpperCase()) {
                                case 'DONE':
                                    PxXbookWidget.toc_hideExpansion();
                                    PxXbookWidget.toc_refresh();
                                    $(PxPage.switchboard).trigger('COMPONENT_HASH', ['assignmentlist', 'displayAssignment', { assignmentid: assignmentId, reload: true }]);
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
                        AssignmentDeleted: function () {
                            PxXbookWidget.toc_assignment_reload();
                            //                            PxGradebook.Reload();
                        },
                        /** RecentlyViewedSnapshot **/
                        RecentlyViewedClicked: function (dataItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem', { itemid: dataItemId}]);
                        },
                        /** Carousel **/
                        PageJumperActivated: function (itemId, pageNumber) { },
                        CarouselImageClicked: function (itemId) {
                            PxPage.log('CarouselImageClicked ItemID: ' + itemId);
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem', { itemid: itemId}]);
                        },
                        SimpleError: function (errorMessage) {

                        },
                        TocClicked: function () {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem']);
                        },

                        /** AssignmentList **/
                        AssignmentListItemClicked: function (assignmentId, itemId) {
                            PxPage.log('AssignmentListItemClicked: ' + assignmentId + ' ' + itemId);
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem', { itemid: itemId, assignmentid: assignmentId}]);
                        },
                        AssignmentListEditClicked: function (actionName, assignmentItemId, itemId) {
                            _static.state.assignmentItemId = assignmentItemId;
                            if (actionName.toUpperCase() === 'EDIT') {
                                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc']);
                                PxXbookWidget.toc_displayAddAssignment(assignmentItemId);
                            }
                        },
                        AssignmentList_Loaded: function () {
                            PxPage.log('AssignmentList_Loaded');
                            _static.fn.runWidgetQueue('AssignmentListWidget');
                        },
                        /** AssignmentDetails **/
                        AssignmentItemClicked: function(enrollmentId, itemId){
                            
                        },

                        /** AssignmentSummary **/
                        AssignmentSummaryClicked: function (dataItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['assignmentlist', 'displayAssignment', {assignmentid: dataItemId}]);
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
                        RecentScoresClicked: function (state, enrollmentId, assignmentItemId) {
                            $(PxPage.switchboard).trigger('COMPONENT_HASH', ['item', assignmentItemId, { renderFNE: 'True', assigned: 'True', mode: 'Grading' }]);
                        },
                        CommentRequested: function (type, open) {
                            PxXbookWidget.mca_fullscreen();
                        },
                        assignmentList_openFolder:function(){
                            PxXbookWidget.assignmentList_openFolder();
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
                        TOC_ChangeDropdown: function (assignmentId) { },

                        /*** Main Content Area ***/
                        UpdateContent: function (itemId, fragmentId) { },
                        ShowComment: function (show) { },

                        /*** Course Links ***/
                        CourseLinks_Reload: function () { },

                        /*** Assignments ***/
                        AssignmentList_Reload: function () { },
                        AssignmentList_OpenClicked: function(){},
                        //GetDefaultAssignmentId: function(aId) { }

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
                        //                        var itemid = rpc.TOC_GetSelectedItem();
                        //                        rpc.TOC_Reload(itemid);
                        break;
                    case 'PX_AssignmentListWidget':
                        //rpc.AssignmentList_Reload();
                        break;
                    case 'PX_ClassSubmissionWidget':
                        break;


                    case 'PX_AssignmentDefaultReload':
                        rpc.AssignmentList_OpenClicked();
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
                        var widgetname = $(this).parentsUntil('.zone').last().attr('id');
                        var componentid = $(e).attr("id");
                        if (_static.xbapi[componentid] !== undefined) {
                            _static.xbapi[componentid].destroy();
                        }
                        _static.xbapi[componentid] = _static.fn.getRpc($(e).attr("rel"), componentid);

                        //Initialized a queue object with a queue and a field to determine if the control has been initialized.
                        if (widgetname !== undefined && _static.widgetQueue[widgetname] === undefined) {
                            _static.widgetQueue[widgetname] = { initialized: false, queue: [] };
                        }
                        $(this).trigger(_static.events.widgetRpcLoaded, [widgetname]);
                    });
                }
            },
            //Checks to see if the widget has initialized.  If not, adds the function to a queue to be run once initialized.
            validateOrQueue: function (widgetname, callback, callbackargs) {

                if (_static.widgetQueue[widgetname] === undefined) {
                    _static.widgetQueue[widgetname] = { initialized: false, queue: [] };
                }
                var queueobj = _static.widgetQueue[widgetname];
                if (!queueobj.initialized) {
                    queueobj.queue.push({ func: callback, args: callbackargs });
                    return false;
                }

                return true;
            },
            runWidgetQueue: function (widgetname) {
                if (_static.widgetQueue[widgetname] === undefined) {
                    _static.widgetQueue[widgetname] = { initialized: false, queue: [] };
                }
                var queueobj = _static.widgetQueue[widgetname];
                
                queueobj.initialized = true;
                while (queueobj.queue.length > 0) {
                    var callback = queueobj.queue.shift();
                    callback.func.apply(this, callback.args);
                }
            }
        }
    };
    // The public interface for interacting with this plugin.
    $.fn.initXbookWidget = function () {
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
            PxRoutes.AddComponentRoute('assignmentlist', null, _static.fn.handleRoute);
            _static.initialized = true;
        },
        displayAddContentDialog: function (parentid) {
            //Open the new content widget.
            PxCreateNewAssignment.DisplayCreateContentDialog(parentid);
        },
        toc_displayAddAssignment: function (assignmentId, itemId) {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_displayAddAssignment, [assignmentId, itemId])) {
                _static.xbapi[tocWidget.attr('id')].TOC_DisplayAssignment(assignmentId, itemId);
                if (!_static.state.isExpanded) {
                    $(PxXbook.sel.widgets.TOC + ', #PX_XBOOK_TAB_ZONE_2').addClass('sidecomponent-visible');
                    _static.state.isExpanded = true;
                }
            }
        },
        toc_assignment_reload: function () {
            var tocComponent = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            _static.xbapi[tocComponent.attr('id')].TOC_Assignments_Reload();
        },
        toc_displayShowHide: function () {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_displayShowHide, null)) {
                _static.xbapi[tocWidget.attr('id')].TOC_DisplayHideShow();
                if (!_static.state.isExpanded) {
                    $(PxXbook.sel.widgets.TOC + ', #PX_XBOOK_TAB_ZONE_2').addClass('sidecomponent-visible');
                    _static.state.isExpanded = true;
                }
            }
        },
        toc_hideExpansion: function () {
            if (_static.state.isExpanded) {
                var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
                if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_displayShowHide, null)) {
                    _static.xbapi[tocWidget.attr('id')].TOC_DisplayNormal();
                    $(PxXbook.sel.widgets.TOC + ', #PX_XBOOK_TAB_ZONE_2').removeClass('sidecomponent-visible');
                    _static.state.isExpanded = false;
                }
            }
        },
        toc_filterLearningCurve: function () {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_filterLearningCurve, null)) {
                _static.xbapi[tocWidget.attr('id')].TOC_LearningCurve();
            }
        },
        toc_refresh: function (itemid) {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');

            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_refresh, [itemid])) {
                _static.xbapi[tocWidget.attr('id')].TOC_Reload(itemid);
            }
        },
        toc_navigateTo: function (itemId, assignmentId) {
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_navigateTo, [itemId, assignmentId])) {
                var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
                if (assignmentId && assignmentId != null && assignmentId.length > 0) {
                    _static.xbapi[tocWidget.attr('id')].TOC_ChangeDropdown(assignmentId);
                }
                _static.xbapi[tocWidget.attr('id')].TOC_NavigateTo(itemId);
            }
        },
        toc_prev: function () {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_prev, null)) {
                _static.xbapi[tocWidget.attr('id')].TOC_Prev();
            }
        },
        toc_next: function () {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_next, null)) {
                _static.xbapi[tocWidget.attr('id')].TOC_Next();
            }
        },
        toc_addElibraryItem: function (itemId, parentId, sequence, clone) {
            var tocWidget = $(PxXbook.sel.widgets.TOC + ' .xb-component');
            if (_static.fn.validateOrQueue(PxXbook.sel.widgets.TOC.replace('#', ''), PxXbookWidget.toc_addElibraryItem, [itemId, parentId, sequence, clone])) {
                _static.xbapi[tocWidget.attr('id')].TOC_AddElibraryItem(itemId, parentId, '', clone);
                PxXbookWidget.toc_refresh(itemId);
            }
        },
        mca_fullscreen: function () {
            if ($(PxXbook.sel.widgets.TOC).is(':visible')) {
                $(PxXbook.sel.widgets.TOC + ', #PX_SearchWidget, #PX_XBOOK_TAB_ZONE_2').addClass('comments-visible');
                PxXbookWidget.showComment(true);
            }
            else {
                PxXbookWidget.showComment(false);
                $(PxXbook.sel.widgets.TOC + ', #PX_SearchWidget, #PX_XBOOK_TAB_ZONE_2').removeClass('comments-visible');
            }
        },
        //Fires an event so that the display of the content can be handled outside of the widget.
        displayContent: function (itemid) {
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
        editCourseLinks: function () {
            var dialogDiv = $(_static.sel.EditCourseLinks);
            $(dialogDiv).dialog({
                width: 650,
                height: 300,
                minWidth: 650,
                minHeight: 300,
                modal: true,
                draggable: false,
                closeOnEscape: true,
                open: function (event, ui) {
                    _static.fn.initWidget.apply($(_static.sel.EditCourseLinks + ' .xb-component'));
                    $('.ui-dialog-titlebar-close').click(function () {
                        $(PxPage.switchboard).trigger('COMPONENT_HASH', ['links']);
                    });
                },
                close: function (event, ui) {
                }
            });
        },
        closeCourseLinks: function () {
            if ($(_static.sel.EditCourseLinks).parent().hasClass('ui-dialog')) {
                $(_static.sel.EditCourseLinks).dialog('destroy');
                var editCourseLinksComponent = $(_static.sel.EditCourseLinks + ' .xb-component');
                _static.xbapi[editCourseLinksComponent.attr('id')].destroy();
            }
        },
        assignmentList_openFolder: function (aId) {
            if (_static.fn.validateOrQueue('AssignmentListWidget', PxXbookWidget.assignmentList_openFolder, [aId])) {
                PxPage.log('assignmentList_openFolder');
                _static.xbapi['AssignmentListWidget'].AssignmentList_OpenClicked(aId);
            }
        },
        //Forces the assignment list xbookapp component to reload
        assignmentList_reload: function(){
            var AssignmentsWidget = $(PxXbook.sel.widgets.Assignments + ' .xb-component');
            if (_static.fn.validateOrQueue(AssignmentsWidget.attr('id'), PxXbookWidget.assignmentList_reload)) {
                //This forces any commands to assignment list to wait until the reload is completed
                _static.widgetQueue['AssignmentListWidget'].initialized = false;
                _static.xbapi['AssignmentListWidget'].AssignmentList_Reload();
                PxPage.log('assignmentList_reloaded');
            }
        }
    };
} (jQuery);