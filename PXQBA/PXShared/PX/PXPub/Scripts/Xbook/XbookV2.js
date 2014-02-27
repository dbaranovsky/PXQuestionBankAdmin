var PxXbookV2 = function($) {

    var _static = {

        _initialized: false,
        _contentInitialized: false,
        
        // Tracks a new assignment unit created from the assignment tab
        _unitCreated: undefined,
        
        // tracks url state for xbook tab
        router: {},

        //Current state of the application (what tab we are on).
        _state: 'xbook',
        _states: ['xbook', 'assignments', 'gradebook'],

        //Selectors for the tabs in the xbook application
        _tabs: {
            xbook: '#PX_MENU_ITEM_XBOOK',
            assignments: '#PX_MENU_ITEM_ASSIGNMENTS',
            grades: '#PX_MENU_ITEM_GRADES'
        },

        _xbclasses: {
            loaded: 'xloaded'
        },

        _productRouteRules: {
            tab: function(value, request, valuesObj) {
                if (!isNaN(value) || value == "state" || value == "/state" || value == "#state") {
                    return false;
                }
                return true;
            }
        },

        // cached ui elements for bindings
        _ui: {
            vent: $(PxPage.switchboard),
            root: $('.product-type-xbookv2'),
            content: '#content-viewer',
            home: '#fne-unblock-action-home',
            activetab: 'li.menu-item.active',
            fneNav: '#nav-container',
            fneWindow: '#fne-window',
            browseResourcesLink: '.faceplate-nav .faceplate-add-content-menu #browse',
            firstTitleInToc: '.faux-tree-node[data-ft-chapter=]' // skips user-generated content
        },

        // map _ui to _fn functions
        _bindControls: function() {
            $(document).off('click', _static._ui.home).on('click', _static._ui.home, PxPage.UnBlock);
        },

        //_loadStickyHeaders initializes the functionality of the fixed headers for the TOC in the xbook2 app
        _loadStickyHeaders: function() {
            $("#PX_TOC").sticky({ topSpacing: 0 });
        },
        
        handleProductRoute: function(tab, component) {
            /// <summary>
            /// Handles a product parsing of hash and moves to appropriate tab
            /// </summary>
            /// <param name="tab" type="String">The tab that was parsed out of the hash url</param>

            // ignore route when you are being tranferred from the Dashboard to the Course
            // expected links will not be available
            if ($('.PX_DashboardWidget').length > 0) {
                return;
            }

            switch (tab.toLowerCase()) {
            case 'assignments':
                _static._state = 'assignments';
                _static.changeTabs($(_static._tabs.assignments));
                break;
            case 'gradebook':
                _static._state = 'gradebook';
                _static.changeTabs($(_static._tabs.grades));
                break;
            case 'xbook':
            default:
                _static._state = 'xbook';
                _static.changeTabs($(_static._tabs.xbook), _static._loadStickyHeaders);
                break;
            }
        },

        redirect: function(component, func, args) {
            /// <summary>
            /// Reroute handler used in conjunction with PxRoutes to take the component hash and add the product portion
            /// </summary>

            //If the component is just a state, we are only doing a state change for now.
            var hash = '';
            if ($.inArray(component, _static._states) > -1 || component === undefined) {

                //Unblock any FNE's that may be open
                if ($("#fne-window").is(":visible")) {
                    PxPage.UnBlockAction(null, false);
                }

                if (component !== undefined) {
                    _static._state = component;
                }
                hash = _static._state;
            } else {
                hash = _static._state + '/' + (func === undefined ? '' : component + '/' + func);
                if (args !== undefined) {
                    hash += '?' + $.map(args, function(a, b) { return b + '=' + a; }).join('&');
                }
            }
            $(PxPage.switchboard).trigger('PX_SET_HASH', [hash, true]);
        },

        changeTabs: function(tab, callback) {
            /// <summary>
            /// Grabs information off of the html markup to make the right ajax call to load the zones
            /// </summary>
            /// <param name="tab" type="Object">jQuery object containing the tab</param>
            /// <param name="callback" type="Function">A callback function to run after the ajax call has finished</param>

            var activeTab = $(_static._ui.activetab);
            var menuItem = tab.closest('li.menu-item');

            //If the tab is aleady active we don't need to reload.
            if (activeTab.attr('id') === menuItem.attr('id')) {
                return;
            }

            var target = tab.find('a').attr("rel");
            var sourceUrl = tab.find('a').attr("ref");
            if (!target || target.length < 1 || !menuItem || !sourceUrl || sourceUrl.length < 1) {
                PxPage.log('Cannot change xbook tab.  Tab missing information');
                return;
            }

            //Check if class exists to determine if content needs to be loaded. 
            //If not then load the content and add the class, then continue on to hide other containers and expand selected one.
            if ($(target).length) {
                PxPage.Loading(target);
                $.ajax({
                    url: sourceUrl,
                    data: {},
                    type: "GET",
                    success: function(response) {
                        $(target).html(response);
                        PxPage.Loaded(target);

                        if (callback !== undefined) {
                            callback();
                        }
                    },
                    error: function(error) {
                        if (callback !== undefined) {
                            callback();
                        }
                    }
                });
            } else {
                if (callback !== undefined) {
                    callback();
                }
            }

            //hide and show appropriate containers
            if (!menuItem.hasClass('active')) {
                _static.itemFocused(menuItem);
            }
        },

        itemFocused: function(menuItem) {
            /// <summary>
            /// Handles styling and tagging to when a menu item is clicked
            /// </summary>
            /// <param name="menuItem" type="Object">jQuery object containing the px menu item</param>
            if (menuItem.length) {
                var parentSiblings = $(menuItem).siblings('li.menu-item');
                parentSiblings.removeClass('active');
                menuItem.addClass('active');
            }
        },

        /// <summary>
        /// Keeps TOC in synch with document viewer by grabbing data from _static.router
        /// </summary>
        synchToc: function () {

            PxPage.log('toc : synchronizing > ');
            PxPage.log(_static.router);

            // open a path if found
            if (_static.router.path !== undefined) {
                var node = $('li[data-ud-id=' + _static.router.path + ']');
                $.fn.fauxtree('openNode', node);
                _static._ui.vent.bind('onNodesLoaded', activateItem);
            }

            function activateItem() {
                if (_static.router.id !== undefined) {
                    var node = $('li[data-ud-id=' + _static.router.id + ']'),
                        link = node.find('.faux-tree-link').click();
                    $.fn.fauxtree('openNode', node);
                    $.fn.fauxtree('setActiveNode', node);
                    PxXbookV2.contentNav.updateNavigation();
                    // set hash without triggering change
                    hasher.changed.active = false;
                    hasher.setHash(link.attr('href'));
                    hasher.changed.active = true;
                }
                _static._ui.vent.unbind('onNodesLoaded');
            }

            activateItem();

        },

        // fne callbacks

        _fneUrl: window.location.href.replace(/fne=true+/gi, 'FNE=False'),
        _docViewerUrl: window.location.href,

        // toggles css classes on fne open and close
        // @param {boolean} toggle : add or remove classes
        fneToggleClasses: function (toggle) {
            var cls = $('.faux-tree-node.active').attr('data-ud-itemtype') || '';
            $('body').toggleClass('is-fne', toggle);
            // appends currently selected node type to fne window element as class
            $(_static._ui.fneWindow).toggleClass(cls, toggle);
            // needed for sticky plugin
            !toggle && $('body').css('overflow', 'visible');
        },

        _fneOpened: function() {
            PxPage.log('fne : open callback');
            _static._fneUrl = window.location.href.replace(/fne=true+/gi, 'FNE=False');
            _static.fneToggleClasses(true);
        },

        // load content into page using router or by clicking the current (inactive) node
        _fneClosed: function() {
            if (PxXbookV2.getCurrentNode().attr('data-ud-islc').toLowerCase() === 'true') {
                PxPage.log('fne : reload content with doc viewer url : ' + _static._docViewerUrl);
                window.location.href = _static._docViewerUrl;
                _static.synchToc();
            } else {
                PxPage.log('fne : reload content with fne url : ' + _static._fneUrl);
                window.location.href = _static._fneUrl;
            }
            _static.fneToggleClasses(false);
        },

        // stops infinite load for LC quiz
        _quizLoaded: function () {
            $('.htmlquiz').unblock();
        },

        // for all events bound to pubsub (PxPage.switchboard)
        _bindSubs: function() {
            _static._ui.vent
                .bind('fneclosed.xbookv2', _static._fneClosed)
                .bind('fneloaded.xbookv2', _static._fneOpened)
                .bind('contentloaded.xbookv2', _static._initContent)
                .bind('contentloaded.xbookv2', _static.checkGradability)
                //Since content doesn't reload after submitting an answer in LC we listen for the argacomplete event 
                //which fires after and question has been answered to check if we need to make the item gradable.
                .bind('argacomplete.xbookv2', _static.checkGradability)
                .bind('contentsaved.xbookv2', _static.contentSaved)
                .bind('contentcreated.xbookv2', _static.contentCreated)
                .bind('contentgradable.xbookv2', _static.contentGradable)
                .bind('htmlquiz-loaded', _static._quizLoaded)
                .bind('htmlquiz-submit-complete', _static.quizSubmissionComplete)
                .bind('componentcancelled', PxPage.LargeFNE.CloseFNE) // published from gradebook
                .on("click", _static._ui.browseResourcesLink, _static._loadBrowseMoreResources);
        },


        // route handling

        _initRoutes: function() {

            PxRoutes.Init(_static.redirect);

            // the regular express matches everything except 'state'
            PxRoutes.AddProductRoute('{tab}/:component*:', _static.handleProductRoute, 10, _static._productRouteRules);
            PxRoutes.AddComponentRoute('item', ':path*:/{itemid}{?args}', function (state, path, itemid, args) {
                PxContentAreaWidget.DisplayContent(itemid, args);

                if (!args.renderFNE || args.renderFNE.toLowerCase() === 'false') {
                    _static._docViewerUrl = window.location.href;
                    // track path and item to keep toc in sync for xbook tab
                    if (_static._state === 'xbook') {
                        _static.router.path = path;
                        _static.router.id = itemid;
                    }
                }
            });

            PxPage.OnProductLoaded(function() {
                HashHistory.Init('xbook');
            });
        },

        // loads first content at unit level if it contains a link
        // or loads child content if no link is found
        // @param {array} node : non user created node in toc
        loadFirstTocItem: function (node) {

            PxPage.log('toc : loading first content entry...');

            var fauxTreeLink = node.find('.faux-tree-link');

            if (fauxTreeLink.length) {
                fauxTreeLink[0].click();
            }
            else {
                _static._ui.vent.bind('onNodesLoaded.xbook', function () {
                    _static._ui.vent.unbind('onNodesLoaded.xbook');
                    node.next().find('.faux-tree-link')[0].click();
                });
                $.fn.fauxtree('openNode', node);
            }

        },

        _initToc: function() {

            var hash = window.location.hash.split('/'),
                firstTitle = $(_static._ui.firstTitleInToc).eq(0);

            if (hash.length > 2) {

                // handle url with path and item id
                var path = hash[3],
                    itemid = hash[4],
                    node = $('#PX_TOC').find('li[data-ft-id=' + path + ']');

                if (node.length && node.data('ft-state') === 'closed') {

                    PxPage.log('toc : found closed node related to path, opening...');

                    $.fn.fauxtree('setActiveNode', node);
                    node.find('.faux-tree-link').click();

                    // fires from contenttreewidget after node is set
                    _static._ui.vent.bind('onNodesLoaded', function () {
                        var child = node.nextAll('li[data-ft-id=' + itemid + ']');
                        $.fn.fauxtree('setActiveNode', child);
                        window.setTimeout(function() { // todo: bind this to vent instead of timeout
                            PxXbookV2.contentNav.updateNavigation();
                        }, 400);
                        _static._ui.vent.unbind('onNodesLoaded');
                    });

                }

            }
            else if (firstTitle.length) {
                _static.loadFirstTocItem(firstTitle);
            }

        },

        // initializes other parts of script when content is loaded
        _initContent: function() {

            if ($('#xb-documentviewer').has('.blockOverlay')) {
                PxPage.Loaded();
            }
            
            if (_static._contentInitialized === true) {
                return false;
            }

            // below will only run once

            _static._bindControls();
            _static._initToc();
            _static._contentInitialized = true;
            PxXbookV2.contentNav.init();
            PxXbookV2.managementCard.init();

        },
        
        quizSubmissionComplete: function (event, status) {
            PxPage.log("htmlquiz-submit event has been triggered ------- " + status);
            if (status !== "ERROR") {
                PxPage.Toasts.Success("Response submitted successfully. Click here to view your submission.");
                _static.reloadContent();
            } else {
                PxPage.Toasts.Error("An Error has occurred.");
            }
        },
        
        reloadContent: function() {
            /// <summary>
            /// Resets hash which will trigger a content reload on the xbook or assignments tab.
            /// </summary>
            
            window.location.hash = window.location.hash.replace('xbook', 'state').replace('assignments', 'state');
        },
        
        contentCreated: function(event, item) {
            /// <summary>
            /// Handles content creation functionality specific to xbook
            /// </summary>

            if (item.unitType && item.unitType.toLowerCase() === "assignmentunit") {
                
                //We don't call saveNavigationState when an assignment unit is created in xbook tab so we need to do
                //an item operation
                if (_static._state === 'xbook') {
                    _static.saveNewAssignmentUnit(item);
                } else {
                    _static._unitCreated = item;
                }
            }            
        },
        
        contentSaved: function() {
            /// <summary>
            /// Handles creation of an assignment unit from the assignment tab because we don't want to save the item until
            /// saveNavigationState has been called.
            /// </summary>
            
            if (_static._unitCreated) {
                
                //In case createGradebookCategory fires the saveNavigation event, make sure we set the state to undefined
                //before calling it.
                var newUnit = _static._unitCreated;
                _static._unitCreated = undefined;
                _static.createGradebookCategory(newUnit);
            }
        },
        
        contentGradable: function() {
            /// <summary>
            /// Updates the html markup after a content item has been made gradable
            /// </summary>

            $('#content-item  input#Content_BHParentId').val('PX_MANIFEST');
            $('#content-item  input#Content_IsGradable').val('True');
            $('#content-item  input#Content_MaxPoints').val('1');
        },

        checkGradability: function () {
            /// <summary>
            /// Handles checking if an item that overrides the due date requirement is setup properly to show in the gradebook.
            /// </summary>
            
            //Grad the override field from the content.  This should only be setup on ExternalContent, Quiz and HtmlQuiz.
            var override = $('#content-item  input#Content_OverrideDueDateReq').val();
            if (override && override.toLowerCase() === 'true') {
                
                //If the item is setup to be gradable without a due date, grab the fields needed to ensure it'll show up in the BH
                //gradebook and make sure they are set to the correct values.
                var parent = $('#content-item  input#Content_BHParentId').val(),
                    isGradable = $('#content-item  input#Content_IsGradable').val(),
                    maxPoints = $('#content-item  input#Content_MaxPoints').val(),
                    itemId = $('#content-item #content-item-id').text();
                
                if ((parent && parent !== 'PX_MANIFEST') ||
                    (isGradable && isGradable.toLowerCase() !== 'true') ||
                    (maxPoints && maxPoints < 1)) {
                    
                    //One of the fields isn't the value it needs to be to show up in the gradebook, so fix.
                    ContentWidget.MakeContentItemGradable(itemId, true);
                }
            }
        },
        
        saveNewAssignmentUnit: function (item) {
          /// <summary>
          /// Handles adding a new assignment unit to the assignment tree structure.
          /// </summary>

            var url = PxPage.Routes.launchpad_item_operation;
            var dataType = "json";

            var data = {
                itemId: item.id,
                targetId: item.parentId,
                operation: 'NewItem',
                toc: item.toc
            };
            $.ajax({
                url: url,
                type: "POST",
                data: JSON.stringify(data),
                dataType: dataType,
                contentType: "application/json; charset=utf-8",
                success: function(response) {
                    _static.createGradebookCategory(item);
                }
            });
        },
        createGradebookCategory: function(item) {
            ContentWidget.CreateNewGradebookCategory(item, function (categoryId, unitId) {
                ContentWidget.AddGradebookCategoryToUnit(categoryId, unitId, function () {
                    //Assignment unit dropdown is only displayed on xbook tab, so only update the drop down
                    //if we are on the xbook tab
                    if (_static._state === 'xbook') {
                        item.categoryId = categoryId;
                        PxManagementCard.PopulateAssignmentUnits(item);
                        // provide link to new unit in assignments tab
                        var msg = $.format('<a href="#/assignments/item/{0}">{1}</a>', [unitId, item.name]);
                        PxPage.Toasts.Success('New assignment unit, ' + msg + ' , has been successfully created.', null, {timeOut: 10000});
                    }
                });
            });
        },

        _loadBrowseMoreResources: function (event) {
            $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', 'none', 'coming soon...');
        }
    };
    
    return {

        vent: _static._ui.vent,

        getCurrentNode: function () {
            return $.fn.fauxtree('getActiveNode');
        },

        getNextNode: function () {
            return $('.last-viewed').nextAll(PxXbookV2.contentNav.validNextNode()).not('.hide-in-fne').eq(0);
        },

        getPrevNode: function () {
            return $('.last-viewed').prevAll(PxXbookV2.contentNav.validPrevNode()).not('.hide-in-fne').eq(0);
        },

        Init: function () {

            if (_static._initialized === false) {

                // check for dependencies
                var reqs = [$.fn.ContentTreeWidget, window.PxContentAreaWidget, window.HashHistory, window.PxRoutes],
                    i = 0;

                for (; i < reqs.length; i += 1) {
                    if (reqs[i] === undefined) {
                        PxPage.log('!! xbookv2 : init error : ' + i);
                        return false;
                    }
                }

                PxPage.log('!! xbookv2 : init');
                
                // toasts
                PxPage.Toasts.AppName("X-Book");
                
                // init components
                _static._initRoutes();
                _static._bindSubs();

                // init px modules
                PxPage.LargeFNE.ShowItemInFNE = function () {}; // we already do this, and it doesn't work properly in LP
                PxPage.LargeFNE.InitStudentTimer = function () { }; // causes issues as student, need to check if required in xbv2
                PxPage.LargeFNE.Init();

                _static._initialized = true;

                return this;

            }

        },
        PrivateTest: function() {
            return _static;
        }
    };
}(jQuery);

/*
CONTENT NAVIGATION / XBOOK PLAYER CONTROLS
FOR ON-PAGE AND FNE WINDOW
*/

PxXbookV2.contentNav = (function($) {

    var _static = {
        // references to ui elements
        _ui: {
            vent: PxXbookV2.vent,
            el: '.contentPlayerHeader',
            prev: '#content-back',
            fwd: '#content-fwd',
            full: '#content-fullscreen',
            fne: {
                el: '#nav-container',
                prev: '#back',
                fwd: '#next'
            }
        },

        // how xbook determins what a valid next node would be within a chapter (top level node)
        _validNextChapterNode: function() {
            return '[data-ft-chapter!=""][data-ft-chapter=' + $('.last-viewed').ftattr('chapter') + '],[data-ft-chapter=' + $('.last-viewed').ftattr('id') + ']'
        },

        _validPrevChapterNode: function() {
            return '[data-ft-chapter!=""][data-ft-chapter=' + $('.last-viewed').ftattr('chapter') + '],[data-ft-id=' + $('.last-viewed').ftattr('chapter') + ']'
        },

        // callbacks for _ui elements
        _fn: {
            // TODO: check for `data-ud-isvisibletostudents` var True/False, skip and get next/prev if True

            prevClick: function() {
                if ($(this).hasClass('disabled')) return false;
                PxPage.log('nav : loading previous document');
                _static._ui.vent.trigger('fneclickPreviousNodeTitle.launchpad', _static._validPrevChapterNode());
            },
            nextClick: function() {
                if ($(this).hasClass('disabled')) return false;
                PxPage.log('nav : loading next document');
                _static._ui.vent.trigger('fneclickNextNodeTitle.launchpad', _static._validNextChapterNode());
            },

            // loads next or previous node in fne depending on buttons class attribute contents
            navigateFullscreen: function(event) {

                if ($(this).hasClass('disabled')) return false;

                var direction = $(event.target).attr('class').toString().indexOf('back'),
                    node = (direction > 0) ? PxXbookV2.getPrevNode() : PxXbookV2.getNextNode(),
                    link = node.find('a').eq(0),
                    nodeSelector = (direction > 0) ? _static._validPrevChapterNode() : _static._validNextChapterNode(),
                    publishEvent = (direction > 0) ? 'fneclickPreviousNodeTitle.launchpad' : 'fneclickNextNodeTitle.launchpad';

                // mock link href to open fne
                var href = link.attr('href').replace('FNE=False', 'FNE=True');
                link.attr('href', href);

                // trigger move
                _static._ui.vent.trigger(publishEvent, nodeSelector);

                // make sure link doesnt retain the mocked href
                (function(link) {
                    window.setTimeout(function() {
                        var href = link.attr('href').replace('FNE=True', 'FNE=False');
                        link.attr('href', href);
                    }, 1000);
                }(link));

            }
        },

        // disables or enables nav buttons depending on node selection

        _updateNavigation: function() {

            PxPage.log('nav : updating navigation');

            //If the current node is an unloaded parent, wait until the children have been loaded
            //before updating the navigation controls
            var currentNode = PxXbookV2.getCurrentNode();
            if ($.fn.fauxtree('isNotLoaded', currentNode)) {
                $(_static._ui.vent).bind('onNodesLoaded', _static._updateNavigation);
            } else {

                $(_static._ui.vent).unbind('onNodesLoaded', _static._updateNavigation);

                var nextNode = PxXbookV2.getNextNode(),
                    prevNode = PxXbookV2.getPrevNode();

                // reset nav state
                $(_static._ui.el).add(_static._ui.fne.el).find('.disabled').removeClass('disabled');

                // enable previous if node is found
                $(_static._ui.prev).add(_static._ui.fne.prev).toggleClass('disabled', !prevNode.length);

                // enable next if node is found
                $(_static._ui.fwd).add(_static._ui.fne.fwd).toggleClass('disabled', !nextNode.length);
            }
        },

        // map _ui to _fn functions

        _bindControls: function () {
            $(document).off('click', '.navigate-next');
            $(document).off('click', '.navigate-back');
            $(document).on('click', _static._ui.prev, _static._fn.prevClick);
            $(document).on('click', _static._ui.fwd, _static._fn.nextClick);
            $(document).on('click', _static._ui.fne.prev, _static._fn.navigateFullscreen);
            $(document).on('click', _static._ui.fne.fwd, _static._fn.navigateFullscreen);
        },

        // for all events bound to pubsub (PxPage.switchboard)

        _bindSubs: function() {
            _static._ui.vent
                .bind('contentloaded', _static._updateNavigation);
        },

        _init: function() {
            _static._bindControls();
            _static._bindSubs();
            _static._updateNavigation();
        }
    };
    return {
        init: _static._init,
        updateNavigation: _static._updateNavigation,
        validNextNode: _static._validNextChapterNode,
        validPrevNode: _static._validPrevChapterNode,
        staticTest: _static
    };

}(jQuery));

/*
MANAGEMENT CARD FUNCTIONS FOR XBV2 ONLY
*/

PxXbookV2.managementCard = (function ($) {

    // references to ui elements
    var _ui = {
        vent: PxXbookV2.vent,
        done: '.assign-showCalendar-close',
        close: '#managementcard-close',
        dateInput: '#facePlateAssignDueDate',
        timeInput: '#facePlateAssignTime',
        unAssign: '.management-card-unassign'
    };

    // callbacks for _ui elements
    var _fn = {

        doneClick: function (elem, itemId) {

            var needsDueDate = true; // TODO: this will be changed when assignment without due date functionality is implemented

            if (PxManagementCard.VerifySelectedUnit(elem)) {
                var unit = PxManagementCard.GetSelectedUnit(elem);
                PxManagementCard.OnAssign(elem, needsDueDate, itemId, unit);
            }
        }
    };

    // skips date checking and assigns an item (id)
    // ported from ManagementCard.js
    function _assignWithoutDueDate(id) {

        PxPage.log('management card : assign without date : ' + id);

        var node = $.fn.fauxtree('getNode', id);

        // prompt for mass unit assignment
        if (node.attr('data-ud-itemtype') === 'PxUnit') {
            var msg = 'Are you sure you want to assign a new due date to all items in this unit? \n\nIf any items in the unit are already assigned, this action will override those due dates.';
            if (!confirm(msg)) {
                return false;
            }
        }

        var points = $('#txtGradePoints').val();

        // validate points
        if (points === '0' || points === '') {

            var completedStudents = $('.faux-tree-node[data-ft-id="' + node.ftattr('id') + '"] .faceplate-student-completion-stats').html()[0],
                msg = 'You have chosen to assign a due date without putting this item in the gradebook. Are you sure this is what you want to do? To have the item appear in the gradebook, enter gradebook points and choose a gradebook category.';

            if (completedStudents > 0) {
                msg = 'Setting the points to zero removes the gradebook entry for this item. Existing gradebook contains a student grade.';
            }

            if (!confirm(msg)) {
                return false;
            }

        } else if (points > 100) {

            PxPage.Toasts.Error('Points should be between 0 and 100');
            return false;

        }

        var gradebookCategory = node.find('.contentwrapper').find('#selgradebookweights option:selected').val();

        // publish event with false date and set operation for assignment center
        _ui.vent.trigger('contentassigned', [{
            id: id,
            date: '01/01/0001',
            startdate: '01/01/0001',
            points: points,
            category: '',
            gradebookcategory: gradebookCategory,
            operation: 'NoDatesAssigned'
        }]);

    }

    // kill event listeners and add custom for xbookv2
    function _managementCardShow(event, id) {
        PxPage.log('management card : open');

        $(_ui.done).unbind('click').bind('click', function () {
            _fn.doneClick(this, id);
        });
        $(_ui.unAssign).unbind('click').bind('click', function () {
            PxManagementCard.unAssign(id);
        });

        // disable stickyness when management card is opened
        $.fn.sticky('toggle', false);
        $('body').addClass('has-management-card');
        $(window).scrollTop( $('.faux-tree-node.managementcard').offset().top - 180 );

    }

    function _managementCardHide(event, id) {
        $('body').removeClass('has-management-card');
        $.fn.sticky('toggle', true);
    }

    // for all events bound to pubsub (PxPage.switchboard)
    function _bindSubs() {
        _ui.vent
            .bind('managementcard.show', _managementCardShow)
            .bind('managementcard.hide', _managementCardHide);
    }

    function _init() {
        _bindSubs();
    }

    return {
        init: _init,
        assignWithoutDueDate: _assignWithoutDueDate,
        fn: _fn,
        ui: _ui
    };

}(jQuery));

// export to global
window.XBookV2 = PxXbookV2;