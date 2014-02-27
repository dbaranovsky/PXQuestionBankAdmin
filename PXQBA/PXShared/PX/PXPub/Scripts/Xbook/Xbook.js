// XBook

//add the xbook object to window so that intilaization can be automatically taken care by ProductScripts.ascx
window.XBOOK = {
    Init: function () {
        PxXbook.Init();
    }
};

var PxXbook = function ($) {
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
        productRouteRules: {
            tab: function (value, request, valuesObj) {
                if (!isNaN(value) || value == "state" || value == "/state" || value == "#state") {
                    return false;
                } 
                return true;
            }
        },
        selectedTocItem: '',
        selectedAssignmentFolder: '',
        folderForNewContent: '',
        controllers: {},
        //class added to containers after then have been loaded to avoid reload
        loadedClass: 'xloaded',
        //avoid binding the handler for overriding the menu click multiple times
        initialized: false,
        state: 'home',
        defaults: {
            readOnly: false
        },
        constant: {
            classActivity: 'ClassActivity',
            homeMenuItem: 'PX_MENU_ITEM_HOME',
            xbookMenuItem: 'PX_MENU_ITEM_XBOOK_TAB',
            assignmentsMenuItem: 'PX_MENU_ITEM_ASSIGNMENTS',
            gradesMenuItem: 'PX_MENU_ITEM_CLASS_ACTIVITY',
            homeHash: 'home',
            xbookHash: 'xbook',
            assignmentsHash: 'assignments',
            gradesHash: 'grades'
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
            TocDropArea: '.tocdrop',
            ProfileWidget_EditLink: '#PX_UserProfileWidget .editProfile-icon',
            ProfileWidget_HiddenEditLink: '#PX_UserProfileWidget .instructor-profile-links .edit-profile-btn a',
            carouselTocButton: '#PX_FeaturedContentWidget_TocButton .carouselTocButton',
            updatebutton: 'update_fne_btn',
            divFneWindow: '#fne-window',
            btnFneClose: '#fne-unblock-action-home',
            firstMenuTab: '#PX_MenuWidget li:first'
        },
        // private functions
        fn: {
            Init: function () {

                if (_static.initialized) {
                    PxPage.log('Tried to reinitialize PxXbook');
                    return;
                }

                // edit link on the user profile
                $(document).off('click', _static.sel.ProfileWidget_EditLink).on('click', _static.sel.ProfileWidget_EditLink, function () { $(_static.sel.ProfileWidget_HiddenEditLink).click() });

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

                //Handles the click of a carousel item
                $(PxPage.switchboard).bind("CarouselWidget.Itemclicked", _static.fn.carouselWidgetItemClicked);

                //Handles the click of Carousel TOC button
                $(document).off('click', _static.sel.carouselTocButton).on('click', _static.sel.carouselTocButton, _static.fn.carouselTocButtonClicked);

                //Handles the selection of the TOC item
                $(PxPage.switchboard).bind('toc_item_selected', function (event, itemid) {
                    _static.selectedTocItem = itemid;
                    _static.fn.openContent(itemid);
                });

                $(PxPage.switchboard).bind("Highlighter-OnAddNote-override", function (event, highlightId, highlightText, orig_event, func) {
                    func(highlightText, orig_event, highlightId);
                });
                $(PxPage.switchboard).bind("Highlighter-OnEditNote-override", function (event, noteId, orig_event, func) {
                    func(noteId, orig_event);
                });
                $(PxPage.switchboard).bind("Highlighter-OnReplyNote-override", function (event, highlightId, orig_event, func) {
                    func(highlightId, orig_event);
                });

                $(PxPage.switchboard).bind("BeingMetaResultsClicked", _static.fn.beingMetaResultsClicked);
                $(PxPage.switchboard).bind("FeaturedWidgetClicked", _static.fn.featuredWidgetClicked);

                //Listen to the content player navigation controls:
                $(PxPage.switchboard).bind(PxContentAreaWidget.events.ContentBack, PxXbookWidget.toc_prev);
                $(PxPage.switchboard).bind(PxContentAreaWidget.events.ContentForward, PxXbookWidget.toc_next);
                //$(PxPage.switchboard).bind(PxContentAreaWidget.events.ContentFullScreen, PxXbookWidget.mca_fullscreen);

                //Listen to the gradebooks view assignment event
                $(PxPage.switchboard).bind(PxGradebook.events.viewAssignment, function (event, itemId) {
                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/displayContentItem?itemid=' + itemId]);
                });

                //CREATE ASSIGNMENT EVENT HANDLES
                //Listen to the Create Assignment's "assign existing" event click
                $(PxPage.switchboard).bind('assignexisting', function (event) {
                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/']);
                    //                    PxXbookWidget.toc_displayAddAssignment();
                });
                //Listen to the Create Assignment's "folderselected" event click
                $(PxPage.switchboard).bind('folderselected', function (event, folder) {
                    _static.folderForNewContent = folder;
                });

                //Listen to xbook widget component hash changes
                $(PxPage.switchboard).bind('COMPONENT_HASH', function (event, component, func, args, silent) {
                    _static.fn.rerouteHash(component, func, args, false, silent);
                });

                //TODO: lot of repeat code. Clean this up
                // sets the width of the content iframe with in the Agilix MainContentArea
                $(PxPage.switchboard).bind("htmlquiz-loaded", function () {

                    var htmlquizIFrame = _static.fn.getHtmlQuizIFrames();
                    if (htmlquizIFrame === undefined)
                        return;

                    //If the content is already visible, we don't need to wait for the event, otherwise fire the event to calculate the width/height of
                    //the content
                    if (htmlquizIFrame.contentFrame[0].contentWindow.$('div[data-type="section"]:visible').length > 0) {
                        _static.fn.setContentFrameDimensions(htmlquizIFrame.contentFrame, htmlquizIFrame.parentFrames);
                    } else {
                        htmlquizIFrame.contentFrame[0].contentWindow.$('body').bind('df-content-rendered', function () {
                            _static.fn.setContentFrameDimensions(htmlquizIFrame.contentFrame, htmlquizIFrame.parentFrames);
                        });
                    }
                });

                //Handle sizing of proxy page iframe
                $(PxPage.switchboard).bind("document-body-iframe-loaded", function (event) {
                    var proxyFrame = $('iframe#document-body-iframe.proxyFrame');
                    var proxyContent = proxyFrame.last().contents()[0];
                    $('html', proxyContent).css({ 'overflow': 'hidden' });

                    if ($('div#content-item.externalcontent, div#document-viewer.allowComments').length > 0 &&
                             $('iframe#document-body-iframe.proxyFrame').length > 0) {
                        var proxyFrame = $('iframe#document-body-iframe.proxyFrame');

                        if (proxyFrame.length == 0)
                            return;

                        //If the content is already visible, we don't need to wait for the event, otherwise fire the event to calculate the width/height of
                        //the content
                        if (proxyFrame[0].contentWindow.$('div[data-type="section"]:visible').length > 0) {
                            _static.fn.setContentFrameDimensions(proxyFrame);
                        } else {
                            proxyFrame[0].contentWindow.$('body').bind('df-content-rendered', function () {
                                _static.fn.setContentFrameDimensions(proxyFrame);
                            });
                        }
                    }
                        // learning curve
                    else if ($('div#content-item.externalcontent').length > 0 &&
                             $('iframe#document-body-iframe').length > 0) {
                        var proxyFrame = $('iframe#document-body-iframe');
                        $(proxyFrame.last()[0]).css({ width: '910px' });
                    }
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
                PxPage.LargeFNE.Init();

                //Route initialization for xbook
                PxRoutes.Init(_static.fn.rerouteHash);
                // the regular express matches everything except "state"
                PxRoutes.AddProductRoute('{tab}/:component*:', _static.fn.handleRoute, 10, _static.productRouteRules);
                PxXbookWidget.init();
                PxGradebook.InitRoutes();
                
                //displayitem route (with path)
                PxRoutes.AddComponentRoute('item', ':path*:/{itemid}{?args}', function (state, path, itemid, args) {
                    var displayitemid = itemid;
                    //Hack that will select the next item in the toc if the current item doesn't have content (ie. folder)
                    if (window.PxTocNavStructure !== undefined) {
                        displayitemid = PxTocNavStructure.current(itemid);
                    }
                    
                    PxContentAreaWidget.DisplayContent(displayitemid, args);
                });

                //Listen for related content drag event to know when we need to display the div mask over the toc
                $(PxPage.switchboard).bind('dragstarted', function (event) {
                    _static.fn.activateTocMask();
                });
                $(PxPage.switchboard).bind('dragstopped', function (event) {
                    _static.fn.removeTocMask();
                });

                //initialize the persistent Qtips
                PxPersistingQtips.Init();

                //initialize new assignment widget to listen for routes
                PxCreateNewAssignment.Init();

                // initialize the navigation js
                PxTocNavStructure.init(
                {
                    navigation_structure: PxPage.Routes.navigation_structure,
                    top_level_item_id: 'PX_TOC',
                    excludeSingleItem: function (item) { return ((item.Href || "").length == 0) ? true : false; },
                    excludeFolderItem: function (item) { return ((item.BFW_Subtype || "") == "Related Content") ? true : false; }
                }, function () {
                    //load the content for the default tab (usually the first tab)
                    HashHistory.Init(_static.fn.getDefaultMenuTab());
                });

                _static.initialized = true;
            },
            getMaxWidth: function (elementNames, context) {
                var widths = $(elementNames, context).map(function () {
                    return $(this).offset().left + $(this).outerWidth();
                }).get();

                //Can't use array.filter because of IE8
                var filteredWidths = [];
                for( w in widths) {
                    if(!(widths[w] === undefined || widths[w] === null)) {
                        filteredWidths.push(widths[w]);
                    }
                }
                return Math.max.apply(null, filteredWidths);
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

                if (target.match(_static.constant.classActivity + "$")) // same as ends with
                    PxXbook.UpdateButton('<span class="doneEditing-btn-icon"></span>Done');
                else
                    PxXbook.UpdateButton('<span class="home-btn-icon"></span>Home');

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
                if (PxTocNavStructure && PxTocNavStructure != null) {
                    PxTocNavStructure.reload(null, function () {

                        PxXbookWidget.toc_refresh(contentItemId);

                        //Navigate to the newly created content on the XBook tab
                        $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/displayContentItem?itemid=' + contentItemId]);
                    });
                } else {
                    PxXbookWidget.toc_refresh(contentItemId);

                    //Navigate to the newly created content on the XBook tab
                    $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/displayContentItem?itemid=' + contentItemId]);
                }
            },

            //Handles the click of a carousel item
            carouselWidgetItemClicked: function (event, id, disableCarasoulDefault) {
                PxPage.log('CarouselImageClicked ItemID: ' + id);
                //take the user to the corresponding item in the TOC
                $(PxPage.switchboard).trigger('PX_SET_HASH', ['xbook/toc/displayContentItem?itemid=' + id]);
            },

            //Handles the click of Carousel TOC button
            carouselTocButtonClicked: function () {
                $(PxPage.switchboard).trigger('COMPONENT_HASH', ['toc', 'displayContentItem']);
            },

            //Handles a being meta result item click
            beingMetaResultsClicked: function (event, itemIds, indexid) {
                var itemId = itemIds[0];
                if (itemId === undefined || itemId == null)
                    itemId = '';
                //$(PxPage.switchboard).trigger('COMPONENT_HASH', ['displayContentItem', { itemid: itemId}]);
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

            //find the default tab
            getDefaultMenuTab: function () {
                var firstMenuItem = $(_static.sel.firstMenuTab).attr("id");
                var defaultTab = '';
                switch (firstMenuItem) {
                    case _static.constant.homeMenuItem:
                        defaultTab = _static.constant.homeHash;
                        break;
                    case _static.constant.xbookMenuItem:
                        defaultTab = _static.constant.xbookHash;
                        break;
                    case _static.constant.assignmentsMenuItem:
                        defaultTab = _static.constant.assignmentsHash;
                        break;
                    case _static.constant.gradesMenuItem:
                        defaultTab = _static.constant.gradesHash;
                        break;
                    default:
                        return;
                }
                return defaultTab;
            },

            //Removes the mask over the TOC for related content drag/drop
            removeTocMask: function () {
                $('.splitmask').remove();
            },
            //Reroute handler used in conjunction with PxRoutes to take the component hash and add the product portion
            rerouteHash: function (component, func, args, replace, silent) {
                replace = replace == null ? true : replace;
                if (component !== undefined) {
                    switch (component.toLowerCase()) {
                        case 'gradebook':
                            _static.state = 'grades';
                            break;
                        case 'assignments':
                        case 'assignmentlist':
                            _static.state = 'assignments';
                            break;
                        case 'contentarea':
                        case 'toc':
                        case 'xbook':
                            _static.state = 'xbook';
                            break;
                        case 'home':
                        case 'links':
                            _static.state = 'home';
                            break;
                        default:
                            break;
                    }
                }

                if (component !== 'item' || component === undefined) {
                    //Unblock any FNE's that may be open
                    if ($("#fne-window").is(":visible")) {
                        PxPage.UnBlockAction(null, false);
                    }
                }

                //Reroute grade to the assigned scores screen if not defined
                if (_static.state === "grades" && func === undefined) {
                    component = 'gradebook';
                    func = 'assignedScores';
                }

                var hash = _static.state + '/' + (func === undefined ? '' : component + '/' + func);
                if (args !== undefined) {
                    hash += '?' + $.map(args, function (a, b) { return b + '=' + a; }).join('&');
                }

                //Kind of a temporary hack for clicking on toc content. ill explain better
                if (silent) {
                    HashHistory.SetHashSilently(hash);
                } else {
                    $(PxPage.switchboard).trigger('PX_SET_HASH', [hash, replace]);
                }
            },
            // Handles a product parsing of hash and moves to appropriate tab
            handleRoute: function (tab, component) {
                // ignore route when you are being tranferred from the Dashboard to the Course
                // expected links will not be available
                if ($('.PX_DashboardWidget').length > 0) {
                    return;
                }

                switch (tab.toLowerCase()) {
                    case 'xbook':
                        _static.state = 'xbook';
                        _static.fn.changeTabs($(_static.sel.tabXBook), function () {
                            if (component === undefined) {
                                //To keep the url in sync with the state, if we go to xbook with a toc item selected we want to make sure
                                //we are at that proper url.
                                if (_static.selectedTocItem !== '') {
                                    _static.fn.openContent();
                                }
                            }
                            //Cleanup any dialogs that may have been leftover
                            if (PxCreateNewAssignment !== undefined) {
                                PxCreateNewAssignment.CleanupDialogs();
                            }
                        });
                        break;
                    case 'assignments':
                        _static.state = 'assignments';
                        _static.fn.changeTabs($(_static.sel.tabAssignments), function () {
                            //Cleanup any dialogs that may have been leftover
                            if (PxCreateNewAssignment !== undefined) {
                                PxCreateNewAssignment.CleanupDialogs();
                            }
                        });
                        break;
                    case 'grades':
                        _static.state = 'grades';
                        _static.fn.changeTabs($(_static.sel.tabGrades));
                        break;
                    case 'home':
                    default:
                        _static.state = 'home';
                        _static.fn.changeTabs($(_static.sel.tabHome), function () {
                            //Cleanup any dialogs that may have been leftover
                            PxXbookWidget.closeCourseLinks();
                        });
                        break;
                }
                $(PxPage.switchboard).trigger('TogglePersistentQtips', [tab.toLowerCase()]);
            },
            updateButton: function (text) {
                $(_static.sel.btnFneClose).html(text);
            },
            
            //Returns the frame of the HTML quiz
            getHtmlQuizIFrames: function () {
                //We gotta do this nonsense to get to the iframe to levels in.  Also need to check if we are in an FNE window because they can both be open.

                //Level 1
                var htmlQuizPlayerFrame = $('.htmlquiz iframe');
                if (htmlQuizPlayerFrame.length == 0)
                    htmlQuizPlayerFrame = $('#fne-MainContentArea iframe');
                if (htmlQuizPlayerFrame.length == 0)
                    return;
                var htmlQuizPlayerContent = htmlQuizPlayerFrame.contents()[0];

                //Level 2
                var contentFrame = $('#document-body-iframe', htmlQuizPlayerContent);
                if (contentFrame.length == 0) {
                    return;
                } else {
                    return {
                        contentFrame: contentFrame,
                        parentFrames: htmlQuizPlayerFrame
                    }
                }
            },
            setContentFrameDimensions: function (frame, parentFrames) {
                /// <summary>
                /// Sets the width dimensions of the document viewer based on the content in the iframe
                /// </summary>
                
                //Calculate  width
                var maxWidthCalc = _static.fn.getMaxWidth('div, span, table, ul, ol, img, iframe, p, body', $(frame).contents()[0]);
                var maxWidthScroll = frame[0].contentWindow.document.body.scrollWidth;
                var maxWidth = maxWidthScroll > maxWidthCalc ? maxWidthScroll : maxWidthCalc;

                $(frame).width(maxWidth + 'px');
                $(parentFrames).width(maxWidth + 'px');
            },
            openContent: function (itemid, inFne) {
                if (!itemid) {
                    itemid = _static.selectedTocItem;
                }
                if (!inFne) {
                    inFne = 'False';
                }
                //Always defaults to document viewer
                window.location.hash = 'state/item/' + itemid + '?mode=Preview&renderFNE=' + inFne + '&toc=syllabusfilter';
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
                Assignment: '#PX_AssignmentDefaultReload',
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
        UpdateButton: function (text) {
            _static.fn.updateButton(text);
        },
        Init: function () {
            _static.fn.Init();
        }
    };
} (jQuery);