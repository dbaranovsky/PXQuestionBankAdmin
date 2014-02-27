// XBook

//add the xbook object to window so that intilaization can be automatically taken care by ProductScripts.ascx
window.XBOOK = {
    Init: function () {
        PxXbook.Init();
    }
};

var PxXbookMedia = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "XBook",
        dataKey: "XBook",
        events: {
            tabLinkClickedEvent: 'tabLink_clicked',
            tabMenuClickOverride: 'MenuClickOverride',
            fullScreenChanged: 'fullscreenchange webkitfullscreenchange mozfullscreenchange',
            xbiFrameLoading: 'XBook_IFrame_Loading',
            eLibraryContentDragged: 'elibrary_content_dragged'
        },
        selectedTocItem: '',
        folderForNewContent: '',
        controllers: {},
        tabChangeFunctions: ['toHome', 'toXbook', 'toAssignments', 'toGrades'],
        //class added to containers after then have been loaded to avoid reload
        loadedClass: 'xloaded',
        //avoid binding the handler for overriding the menu click multiple times
        initialized: false,
        defaults: {
            readOnly: false
        },
        sel: {
            tabHome: '#PX_MENU_ITEM_HOME',
            tabXBook: '#PX_MENU_ITEM_XBOOK_TAB',
            tabAssignments: '#PX_MENU_ITEM_ASSIGNMENTS',
            tabGrades: '#PX_MENU_ITEM_CLASS_ACTIVITY',
            contentAreaWidget: '#PX_MainContentAreaWidget',
            pxcontentAreaWidget: '#PX_MainContentAreaSwitchWidget',
            editCourseLinksWidget: '#PX_CourseLinksEditWidget',
            carouselWidget: '#PX_XbookCarouselWidget',
            XbookZone2: '#PX_XBOOK_TAB_ZONE_2',
            TocWidget: '#PX_TOCWidget',
            SplitBar: '.splitbarV',
            SplitMask: '.splitmask',
            eLibraryItem: '.eLibraryItem',
            SplitMainContent: '#splitMainContent',
            TocDropArea: '.tocdrop'
        },
        // private functions
        fn: {
            Init: function () {

                if (_static.initialized) {
                    PxPage.log('Tried to reinitialize PxXbook');
                    return;
                }

                //Hide all the containers to avoid z-index issues. They get displayed after their tab is clicked.
                $('.widgetItem.PX_MenuWidget:not([sequence="a"])').hide();

                //Bind our events and mark as initialized.
                //                    $(PxPage.switchboard).bind(_static.events.tabMenuClickOverride, _static.fn.onMenuClick);
                $(PxPage.switchboard).bind(_static.events.fullScreenChanged, function (event) {
                    if (document.webkitFullscreenElement || document.mozFullscreenElement) {
                        $(_static.sel.contentAreaComponent).css('width', '80%');
                    }
                });
                $(PxPage.switchboard).bind('contentcreated', _static.fn.contentCreated);

                $(PxPage.switchboard).bind('toc_item_selected', function (event, itemid) {
                    _static.selectedTocItem = itemid;
                });

                $(PxPage.switchboard).bind("BeingMetaResultsClicked", _static.fn.beingMetaResultsClicked);
                $(PxPage.switchboard).bind("FeaturedWidgetClicked", _static.fn.featuredWidgetClicked);

                //Listen to the gradebooks view assignment event
                $(PxPage.switchboard).bind(PxGradebook.events.viewAssignment, function (event, itemId) {
                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/toc_navigateTo?itemid=' + itemId]);
                });

                //Listen to the Create Assignment's "assign existing" event click
                $(PxPage.switchboard).bind('assignexisting', function (event) {
                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/']);
//                    PxXbookWidget.toc_displayAddAssignment();
                });
                //Listen to the Create Assignment's "folderselected" event click
                $(PxPage.switchboard).bind('folderselected', function (event, folder) {
                    _static.folderForNewContent = folder;
                });

                //FNE related hooks, listeners, etc.
                $(PxPage.switchboard).bind('fne-done-link', function () {
                    PxPage.UnBlock();
                });

                // before the FNE screen loads convert the underlying Content Item display id so they will not conflict
                $(PxPage.switchboard).bind('fneprep', function () {
                    $('#content-item').attr('id', 'content-item-back');
                });

                //Code to run when the FNE window closes
                PxPage.FneCloseHooks[''] = function () {
                    // when the FNE is closing convert the content area id back to normal
                    if ($('#content-item').attr('id') === 'content-item-back') {
                        $('#content-item').attr('id', 'content-item');
                    }
                    PxXbookWidget.toc_refresh();
                    PxXbookWidget.toc_navigateTo();
                };

                $(PxPage.switchboard).bind('COMPONENT_HASH', function(event, component, func, args) {
			        _static.fn.rerouteHash(component, func, args, false);
		        });

                PxRoutes.Init(_static.fn.rerouteHash);
                // the regular express matches everything except "state"
                PxRoutes.AddProductRoute('{tab}/:component*:', _static.fn.handleRoute, 10);
                PxXbookWidget.init();
                PxGradebook.InitRoutes();
                PxContentAreaWidget.Init('#PX_XbookContentWidget_XBook');
                //Listen for related content drag event to know when we need to display the div mask over the toc
                $(PxPage.switchboard).bind('dragstarted', function (event) {
                    _static.fn.activateTocMask();
                });
                //Listen for related content stop drag event to remove mask
                $(PxPage.switchboard).bind('dragstarted', function (event) {
                    _static.fn.activateTocMask();
                });

                PxPage.LargeFNE.Init();

                //initialize the persistent Qtips
                PxPersistingQtips.Init();

                // initialize the navigation js
                PxTocNavStructure.init(
                {
                    navigation_structure: PxPage.Routes.navigation_structure,
                    top_level_item_id: 'PX_TOC',
                    excludeSingleItem: function (item) { return ((item.Href        || "").length == 0                ) ? true : false; },
                    excludeFolderItem: function (item) { return ((item.BFW_Subtype || "")        == "Related Content") ? true : false; },
                }, function() {
                    _static.fn.changeTabs($(_static.sel.tabXBook));
                    HashHistory.Init('home');
                });

                _static.initialized = true;
            },
            //Handles styling and tagging to when a menu item is clicked
            itemFocused: function (menuItem, containerItemId) {
                if (menuItem.length) {
                    var parentSiblings = $(menuItem).siblings('li.menu-item[mw-index=-1]');
                    parentSiblings.removeClass('active');
                    menuItem.addClass('active');
                }

                if (containerItemId !== undefined) {
                    var containerSiblings = $(containerItemId).siblings('.notTab');
                    containerSiblings.fadeOut();
                    $(containerItemId).fadeIn();
                }
            },
            //Overrides clicks on Navigation tabs 
            changeTabs: function (tab, callback) {
                var menuItem = tab.closest('li.menu-item[mw-index=-1]');
                var target = tab.find('a').attr("rel");
                var sourceUrl = tab.find('a').attr("ref");
                var containerItemId = target.replace('container_', '');
                var requiredLoad = false;

                //Check if class exists to determine if content needs to be loaded. 
                //If not then load the content and add the class, then continue on to hide other containers and expand selected one.
                if (!($(menuItem).hasClass(_static.loadedClass)) && $(target).length) {
                    PxPage.Loading(target);
                    $.ajax({
                        url: sourceUrl,
                        data: {
                            //itemId: id
                        },
                        type: "GET",
                        success: function (response) {

                            $(target).html(response);
                            //                            $(PxPage.switchboard).trigger("MenuWidget.ItemLoaded", [currentNode, true]);
                            PxPage.Loaded(target);

                            //Temporary fix until the cssclass proeprty on the widget works.
                            $(_static.sel.editCourseLinksWidget).hide();
                            if (callback !== undefined) {
                                callback();
                            }
                        }
                    });
                    $(menuItem).addClass(_static.loadedClass);
                    $(containerItemId).show();
                } else {
                    if (callback !== undefined) {
                        callback();
                    }
                    requiredLoad = true;
                }

                //There wasn't a way to select the tab containers without the tabs themselves, so I added this class to make the selector easier.
                if (!($(containerItemId).hasClass('notTab'))) {
                    $(containerItemId).addClass('notTab');
                }

                //hide and show appropriate containers
                if (!menuItem.hasClass('active')) {
                    _static.fn.itemFocused(menuItem, containerItemId);

                    var componentIds = _static.fn.getListOfXBookComponentsOnTab(requiredLoad, menuItem, containerItemId);
                    $(PxPage.switchboard).trigger("MenuWidget.PostRequestTabChange", [requiredLoad, componentIds]);
                }
            },
            getListOfXBookComponentsOnTab: function (requiredLoad, menuItem, containerItemId) {
                // if the items are or will be loaded shortly, there is no reason for the items to be reloaded
                if (!requiredLoad)
                    return [];

                // obtain a list of component item ids to return to calling method
                var components = [];
                $.each($(containerItemId + " .widgetItem"), function (index, value) {
                    components.push($(value).attr('itemid'));
                });
                return components;
            },
            //Marks the navigation tab as loaded so the entire div doesn't need to be loaded multiple times
            markItemAsLoaded: function (menuItem, containerItem) {
                $(menuItem).addClass(_static.loadedClass);
                $(containerItem).show();
                $(containerItem).addClass('notTab');
            },
            //Xbook specific handling of new content created by an instructor
            contentCreated: function (event, contentItemId, mode, parent, callback) {

                //If an assignment folder was selected when creating content, set the folder on the item in dlap
                if (_static.folderForNewContent !== undefined && _static.folderForNewContent !== '') {
                    $.post(
                        PxPage.Routes.set_assignment_folder,
                        {
                            itemId: contentItemId,
                            folderId: _static.folderForNewContent
                        },
                        function (data, status) {

                        }
                    );

                    //Set this to empty after grabbing the value so it doesn't accidentaly get used again
                    _static.folderForNewContent = '';
                }

                //Navigate to the newly created content on the XBook tab
                $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/toc_navigateTo?itemid=' + contentItemId]);
            },
            //Handles a being meta result item click
            beingMetaResultsClicked: function (event, itemIds, indexid) {
                var itemId = itemIds[0];
                if (itemId === undefined || itemId == null)
                    itemId = '';
                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc_navigateTo', { itemid: itemId}]);
            },
            featuredWidgetClicked: function (event, contentType) {
                switch (contentType) {
                    case 'LearningCurve':
                        $(PxPage.switchboard).trigger('PX_SET_HASH', ['#state/toc']);
                        PxXbookWidget.toc_filterLearningCurve();
                        break;
                    default:
                        return;
                }
            },
            //Creates an invisible div tag over the toc drag/drop between related content and toc can function
            activateTocMask: function () {
                var tocwidth = $(PxXbook.sel.widgets.TOC).width();
                var tocheight = $(PxXbook.sel.widgets.TOC).height();
                var mask = $('<div class="splitmask"><div class="tocdroplbl">Drop to add content</div><div style="height:' + tocheight + 'px; width:' + tocwidth + 'px;" class="tocdrop promoteToc"></div></div>');
                $(PxXbook.sel.widgets.TOC).before(mask);

                //Make div tag over TOC droppable.
                $(_static.sel.TocDropArea).droppable({
                    drop: function (event, ui) {
                        //Add the item to the TOC, close the related content drop down, remove the mask over the toc.
                        PxXbookWidget.toc_addElibraryItem($(ui.draggable[0]).attr('data-item-id'), 'PX_TOC', 'a', true);
                        PxRelatedContent.toggleDropDown();
                        _static.fn.removeTocMask();
                    }
                });
            },
            //Removes the mask over the TOC for related content drag/drop
            removeTocMask: function () {
                $('.splitmask').remove();
            },
            //Reroute handler used in conjunction with PxRoutes to take the component hash and add the product portion
            rerouteHash: function (component, func, args, replace) {
                var tab = '';
                replace = replace == null ? true : replace;
                switch (component.toLowerCase()) {
                    case 'gradebook':
                        tab = 'grades';
                        if(func === undefined) {
                            func = 'defaultView';
                        }
                        break;
                    case 'assignments':
                        tab = 'assignments';
                        break;
                    case 'contentarea':
                    case 'toc':
                    case 'xbook':
                        tab = 'xbook';
                        break;
                    case 'home':
                    case 'links':
                    default:
                        tab = 'home';
                        break;
                }
                var hash = tab + '/' + (func === undefined ? '' : component + '/' + func);
                if (args !== undefined) {
                    hash += '?' + $.map(args, function (a, b) { return b + '=' + a; }).join('&');
                }
                $(PxPage.switchboard).trigger('PX_SET_HASH', [hash, replace]);
            },
            // Handles a product parsing of hash and moves to appropriate tab
            handleRoute: function (tab, component) {      
                switch (tab.toLowerCase()) {
                    case 'xbook':
                        _static.fn.changeTabs($(_static.sel.tabXBook), function () {
                            if(component === undefined) {
                                //To keep the url in sync with the state, if we go to xbook with a toc item selected we want to make sure
                                //we are at that proper url.
                                if (_static.selectedTocItem !== '') {
                                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/contentarea/displayContentItem?itemid=' + _static.selectedTocItem]);
                                }
                            }
                        });
                        break;
                    case 'assignments':
                        _static.fn.changeTabs($(_static.sel.tabAssignments))
                        break;
                    case 'grades':
                        _static.fn.changeTabs($(_static.sel.tabGrades));
                        break;
                    case 'home':
                    default:
                        _static.fn.changeTabs($(_static.sel.tabHome), function () {
                            PxXbookWidget.closeCourseLinks();
                        });
                        break;
                }
                $(PxPage.switchboard).trigger('TogglePersistentQtips', [tab.toLowerCase()]);
            }
        }
    };

    // The public interface for interacting with this plugin.
    return {
        sel: {
            widgets: {
                Carousel: _static.sel.carouselWidget,
                CourseLinks: '#PX_CourseLinksWidget',
                EditCourseLinks: _static.sel.editCourseLinksWidget,
                UserProfile: '#PX_UserProfileWidget',
                StudentRegistration: '#PX_StudentRegistrationWidget',
                RecentlyViewed: '#PX_RecentlyViewedWidget',
                AssignmentSnapshot: '#PX_AssignmentSummaryWidget',
                ClassActivitySnapshot: '#PX_RecentSubmissionsWidget',
                LearningCurveSnapshot: '#PX_RecentActivitiesWidget',
                TOC: '#PX_TOCWidget',
                MainContentArea: _static.sel.contentAreaWidget,
                PXMainContentArea: _static.sel.pxcontentAreaWidget,
                Assignments: '#PX_AssignmentListWidget',
                CreateAssignment: '#PX_NewAssignmentWidget',
                ClassActivity: '#PX_ClassSubmissionWidget',
                XbookZone2: '#PX_XBOOK_TAB_ZONE_2',
                SplitMask: _static.sel.SplitMask
            }
        },
        state: {
            isDragging: false
        },
        events: {
            tabLinkClickedEvent: _static.events.tabLinkClickedEvent,
            tabMenuClickOverride: _static.events.tabMenuClickOverride,
            eLibraryContentDragged: _static.events.eLibraryContentDragged
        },
        ChangeTabs: function (event, tabSelector) {
            _static.fn.changeTabs(event, tabSelector);
        },
        Init: function () {
            _static.fn.Init();
        }
    };
} (jQuery);