// Content Tree Widget
//
// This plugin is responsible for the client-side behaviors of the
// Content Tree widget
//
// This plugin requires the fauxtree plugin as the fauxtree is used
// to display the items inside of a category.

var contentDataElem = [];

(function ($) {
    //privately scoped data and functions used by the plugin
    var _static = {
        pluginName: "ContentTree",
        dataKey: "ContentTree",
        bindSuffix: ".ContentTree",
        dataAttrPrefix: "data-ud-",
        minLevel: 1,
        listeners: [],
        operation: {
            assign: "DatesAssigned",
            invisible: "invisible",
            move: "move",
            moveAndAssign: "MoveAndAssign",
            newItem: "newitem",
            remove: "removed",
            removeOnUnassign: "RemoveOnUnassign",
            showOrHide: "ShowOrHideFromStudents",
            visible: "visible",
            unassign: "DatesUnAssigned"
        },
        //default option values
        defaults: {
            //determines how long a dragged item has to be held over an expandable item
            //in order to trigger expansion
            expandDelay: 1500,
            readOnly: false, //disable drag and drop, disable management card, disable add content (used for student view)
            showManagementCard: true, //enable management card
            showExpandIcon: false,
            enableAddContent: true,

            togglePastDue: false, // by default we don't show the past due toggle option
            toggleDueLater: false, // by default we don't show the due later toggle option
            showCollapseUnassigned: true, // by default we don't show the collapse all option
            collapseUnassigned: true, // by default we don't enable the collapse all option
            grayOutPastDueLater: false,
            dueSoonDays: 14,
            enableDragAndDrop: true,
            splitAssignedUnassigned: true,
            collapseDueLaterByDefault: false,
            collapsePastDueByDefault: false,
            courseNumber: "",
            triggerOpenContentOnClick: false,
            sortByDueDate: false,
            sortableRootLevelName: "Launchpad",
            scrollOnOpen: true, // scrolls viewport when node is expanded

            removeOnUnassign: false,

            containerId: "ContentBrowser", //default container ID to load items
            subContainerId: "PX_MULTIPART_LESSONS", //default subcontainer ID to load items

            emptyTreeText: "There are no items currently added. Please use the Add button to add content.",
            emptyTreeTextForStudent: "There are no items to view.",

            itemTemplate: "<div class='faux-tree-node-title col faceplate-hover-div showaslink'> \
                                                    <div class='fp-img-wrapper'                  \
                                                        <span class='fpimage'></span>            \
                                                    </div>                                       \
                                                    <span class='icon-placeholder'></span>       \
                                                    <span class='fptitle'>                       \
                                                        <a></a>                                  \
                                                    </span>                                      \
                                            </div>                                               \
                                            <div style='clear:both'></div>"

        },
        states: {
            none: "none",
            barren: "barren",
            open: "open",
            closed: "closed"
        },
        //class names used by the plug-in
        css: {
            fauxTree: "faux-tree", //faux-tree class
            emptyTree: "empty-tree", //empty tree class
            hidden: "hidden", //item is hidden
            itemExpanded: "nodeExpanded",
            item: "faux-tree-node",

            itemManagementCardOpen: "managementcard",
            newNode: "newNode"
        },
        //jquery selectors for commonly accessed elements
        sel: {
            assignmentDates: ".assignment-dates", //represents calendar shown on assigned launchpaditem
            startDate: ".assignment-start-date", //startdate of item
            endDate: ".assignment-due-date", //end date of item
            itemTitle: ".faux-tree-node-title", //title of the item (FN: nodeTitle)
            unitTitle: ".pxunit-title", //title of a unit (FN: pxUnitTitle)
            item: ".faux-tree-node", //individual launchpad item (FN: itemTitle)
            itemManagementCard: '.faceplate-right-menu',
            itemDescription: '.description',
            itemRowLevel1: '.unitrowlevel1',
            itemAddContentBtn: '.addContentBtn',
            itemButtons: '.faceplate-item-status', //calendar, assign, and gearbox buttons
            itemAssignButton: ".faceplate-item-assign",
            itemCalendar: ".faux-tree-node .pxunit-display-duedate",
            itemCalendarBox: ".pxunit-display-duedate",
            itemGearbox: ".view-pxunit-menu .gearbox",
            itemManagementCardPlaceholder: ".faceplate-right-menu",
            itemManagementCardClose: "#managementcard-close",
            itemCollapsedIcon: ".icon",
            itemPoints: ".pxunit-display-points",
            chapterUnselected: 'item-unselected', //grayout chapter nodes

            tree: ".faux-tree",
            assignedTree: ".faceplate-unit-subitems",
            unassignedTree: ".faceplate-unit-subitems-unassigned",
            emptyTree: ".empty-tree",
            category: ".nav-category",

            fneLink: ".faux-tree-link",
            hideForEbook: ".HideForEbook", //hide item in ebook view
            hidden: ".hidden", //item is not visible
            browseMoreResourcesLink: ".item-type-chapterresourceslinksfixed",

            cardMoveOrCopy: ".moveorcopy",

            lcDialog: '#fne-lc-dialog',

            addContentButton: "#add-assignment-btn",
            addContentMenu: ".faceplate-add-content-menu",
            addContentMenuLink: ".lnkCreateContent",
            
            newNode: ".newNode",
            addUnitButton: "#create-assignment",

            txtGradePoints: "#txtGradePoints",

            activeChapterId: "#activeChapterId",
            activeChapterName: "#activeChapterName",

            fptitle: '.fptitle',
            unitfptitle: '.unitfptitle',
            pxDefaultText: '.px-default-text',
            pxDefaultTextEmpty: '.px-default-text-empty',
            emptyFolder: '.emptyFolder',
            validChapterNode: function (node) {
                return "[data-ft-chapter=" + node.ftattr('chapter') + "]";
            },
            getItem: function (itemid) {
                return _static.sel.tree + " li.faux-tree-node[data-ft-id=\"" + itemid + "\"]";
            },
            getUnassignedItems: function () {
                return _static.sel.tree + ' li.faux-tree-node[data-ud-is-assigned="false"][data-ft-level="1"]';
            },
            childOf: function (parentid) {
                if (typeof (parentid) != "string") {
                    parentid = parentid.ftattr('id');
                }

                return _static.sel.tree + ' li.faux-tree-node[data-ft-parent="' + parentid + '"]';
            },
            isLevel: function (level) {
                return _static.sel.tree + "li.faux-tree-node[data-ft-level=" + level + "]";
            }
        },
        fn: {
            //---INIT functions
            initTrees: function (treeAssigned, treeUnassigned) {

                //initialize faux-trees and faux-tree related bindings
                var settings = $(this).data(_static.dataKey).settings;

                var fauxtreeReadOnly = settings.readOnly;
                if (!settings.enableDragAndDrop) {
                    fauxtreeReadOnly = true;
                }
                var undraggable = ".unitrowlevel1[data-ud-is-assigned='true']";
                if (settings.sortByDueDate == false) {
                    undraggable = "";
                }

                if (treeAssigned && treeAssigned.length) {
                    treeAssigned.fauxtree({
                        debug: false,
                        onNodeClick: _static.fn.onNodeClick,
                        onNodeDropped: _static.fn.onNodeDropped,
                        onNodeStartDrag: _static.fn.onNodeStartDrag,
                        onOpenNode: _static.fn.onOpenNode,
                        onCloseNode: _static.fn.onCloseNode,
                        onLoadNodes: _static.fn.onLoadNodes,
                        onScanItem: _static.fn.scanUnitOrFolder,
                        readOnly: fauxtreeReadOnly,
                        showLastViewed: true,
                        filterByTreeId: $(treeAssigned).attr("id"),
                        filterNodesByTree: true,
                        indent: 0,
                        sortableThreshold: 20,
                        sortableUseMouse: true,
                        sortableInsideContainer: true,
                        sortableDisableReadOnlyDrag: true,
                        sortableDisableIndentation: true,
                        sortableRootLevelName: settings.sortableRootLevelName,
                        sortableItemsUndraggableSel: undraggable,
                        showDragLabel: true,
                        showDragIcon: true,
                        showExpandIcon: settings.showExpandIcon,
                        sortableNodeExpandDelay: 1000,
                        allowRootLevelUnitNesting: false,
                        closeAllSiblingsOnOpen: settings.closeAllOnOpen,
                        nodeTitleCustomSel: " .fptitle a",
                        nodeTitleCustomHtml: settings.itemTemplate
                    });
                }
                if (treeUnassigned && treeUnassigned.length) {
                    treeUnassigned.fauxtree({
                        debug: false,
                        onNodeClick: _static.fn.onNodeClick,
                        onNodeDropped: _static.fn.onNodeDropped,
                        onNodeStartDrag: _static.fn.onNodeStartDrag,
                        onOpenNode: _static.fn.onOpenNode,
                        onCloseNode: _static.fn.onCloseNode,
                        onLoadNodes: _static.fn.onLoadNodes,
                        onScanItem: _static.fn.scanUnitOrFolder,
                        readOnly: fauxtreeReadOnly,
                        showLastViewed: true,
                        filterByTreeId: $(treeUnassigned).attr("id"),
                        filterNodesByTree: true,
                        indent: 0,
                        sortableThreshold: 20,
                        sortableUseMouse: true,
                        sortableInsideContainer: true,
                        sortableDisableReadOnlyDrag: true,
                        sortableDisableIndentation: true,
                        sortableRootLevelName: settings.sortableRootLevelName,
                        sortableItemsUndraggableSel: undraggable,
                        showDragLabel: true,
                        showDragIcon: true,
                        showExpandIcon: settings.showExpandIcon,
                        sortableNodeExpandDelay: 1000,
                        allowRootLevelUnitNesting: false,
                        closeAllSiblingsOnOpen: settings.closeAllOnOpen,
                        nodeTitleCustomSel: " .fptitle a",
                        nodeTitleCustomHtml: settings.itemTemplate
                    });
                }
            },

            initStudentUI: function () {
                //initialize student-specific ui elements
                // div overlay for student status:
                $(document).off('mouseenter', '.status-percentage').on('mouseenter', '.status-percentage', function () {
                    $(this).find(".chapter-student-status").show();
                });
                $(document).off('mouseleave', '.status-percentage').on("mouseleave", '.status-percentage', function () {
                    $(this).find(".chapter-student-status").hide();
                });
            },
            //--- END INIT Functions

            //---FAUX-TREE EVENTS
            onNodeClick: function (ui) {
                var settings = _static.fn.GetSettings();
                //faux-tree event callback: click node
                _static.fn.hideManagementCard();

                var itemType = $(this).udattr("itemtype");
                var hasChildren = $(this).ftattr("has-children");
                //open node if content is a folder, else open FNE
                //TODO: Remove check for "PxUnit" once we have confirmed hasChildren works for LP
                if (itemType == "PxUnit" || hasChildren == 'true') {
                    $(ui.sender).fauxtree("toggleNode", $(this));
                }
                // don't click the node if question dialog is open
                if ($(".add-question-to-existing-quiz-dialog").is(":visible")) {
                    $(".add-question-to-existing-quiz-dialog .faux-tree-node").removeClass("active");
                    $(this).addClass("active");
                    return;
                }
                if (settings.triggerOpenContentOnClick) {
                    var itemId = $(this).ftattr("id");
                    $(PxPage.switchboard).trigger("opencontent", itemId);
                }
            },
            onNodeDropped: function (drop) {
                //faux-tree event callback: drop node
                //_static.fn.onNodeDropped.apply($(this).parents(_static.sel.category), [$(this), drop]);
                var node = $(this);
                var data = {
                    category: node.parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: node,
                    above: drop.above,
                    below: drop.below,
                    operation: _static.operation.move,
                    loadView: false,
                    onSuccess: drop.callback
                };

                //restrict dropping unassigned items into the root of the assigned tree
                var cancelDrop = false;
                if (data.changed.udattr("is-assigned") == "false") {
                    if (data.above.length > 0 && data.above.ftattr('parent') == "PX_MULTIPART_LESSONS" && data.above.parent().hasClass('faceplate-unit-subitems')) {
                        cancelDrop = true;
                    } else if (data.below.length > 0 && data.below.ftattr('parent') == "PX_MULTIPART_LESSONS" && data.below.parent().hasClass('faceplate-unit-subitems')) {
                        cancelDrop = true;
                    }
                }
                if (cancelDrop) {
                    data.tree.filter('.ui-sortable').sortable('cancel');
                    data.changed.removeClass("fade-effect");
                    return false;
                }

                var parentChildrenNotLoaded = false;
                //we will recursively call this function until both children of node being dropped and children of target (parent) node are loaded
                var childrenNotLoaded = $(data.tree).fauxtree("isNotLoaded", node);
                var parent = $(_static.sel.getItem(node.ftattr("parent")));
                if (parent.length > 0) {
                    var parentChildren = $(_static.sel.childOf($(parent).ftattr("id"))).not(node);; //find parent's children besides the node being dropped
                    parentChildrenNotLoaded = parentChildren.length == 0 && $(parent).ftattr("has-children") == "true";
                }


                //if the element has children that have not been loaded, load the children first, then call savenavigationstate
                if (childrenNotLoaded) {
                    var ui = {
                        applySort: false,
                        skipChapterOpen: true,
                        callback: function () { _static.fn.onNodeDropped.apply(node, [drop]); }
                    };

                    _static.fn.onLoadNodes.apply(data.changed, [ui]);
                } //if the element's new parent has items that haven't been loaded, load those as well
                else if (parentChildrenNotLoaded) {
                    var ui = {
                        applySort: false,
                        skipChapterOpen: true,
                        callback: function () { _static.fn.onNodeDropped.apply(node, [drop]); }
                    };
                    _static.fn.onLoadNodes.apply(parent, [ui]);
                }
                else {
                    _static.fn.saveNavigationState(data);
                    if (node.ftattr("level") != 1) {
                        //if the dragged node is not on root level
                        _static.fn.closedChaptersFade();
                    }
                }
            },

            onNodeStartDrag: function (node) {
                //remove graying out when starting to drag and drop;
                _static.fn.closedChaptersShow();
                return true;
            },

            onOpenChapter: function (node, isAnimated, dontUpdateWindowLocation) {

                if (node.ftattr("level") == 1
                    && node.udattr("itemtype").toLowerCase() === "pxunit") {
                    //node is a chapter that has not been expanded out yet         

                    //Load images for content
                    node.find('img').each(function () {
                        $(this).attr('src', $(this).attr('delayedsrc'));
                    });

                    //close nodes of the opposite tree
                    var siblingTrees = node.parents(_static.sel.tree).siblings(_static.sel.tree);
                    $(siblingTrees).fauxtree("closeAllRootNodes");

                    //scroll to chapter
                    if (_static.fn.GetSettings().scrollOnOpen === true) {
                        if ($(".ui-dialog:visible").length == 0) {
                            window.setTimeout(function () {
                                var pos = node.offset().top + $('#main').scrollTop(); // -$('#main').positi().top;
                                $("body,html,document").animate({
                                    scrollTop: pos
                                });
                            }, 400);
                        }

                        //show description
                        node.find(_static.sel.pxDefaultText).show();
                        node.find(_static.sel.itemDescription + ',' + _static.sel.itemAddContentBtn).slideDown();
                    }
                    else {
                        node.find(_static.sel.itemDescription).show();
                        node.find(_static.sel.itemAddContentBtn).show();
                    }
                    //grey out all other chapters
                    var isDragging = $(_static.sel.tree).fauxtree("isDragging");
                    if (!isDragging) {
                        _static.fn.closedChaptersFade();
                    }
                    //set the currently active chapter
                    $(_static.sel.activeChapterId).val($(node).ftattr("id"));
                    $.trim($(_static.sel.activeChapterName).val($(node).find(".unitfptitle").first().text()));
                    $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");

                    if (!$(".fne-window").is(":visible") && !dontUpdateWindowLocation) {
                        var itemUrl = $.format("/item/{0}/", $(node).ftattr("id"));
                        if (window.location.hash.indexOf("/item/") > 0) {
                            window.location.hash = window.location.hash.replace(/\/item\/.*\//g, itemUrl);
                        } else {
                            window.location.hash = "state" + itemUrl;
                        }
                    }
                }
            },
            onOpenNode: function (ui, isAnimated, dontUpdateWindowLocation) {

                if (isAnimated === undefined) {
                    isAnimated = true;
                }

                var node = $(this);
                node.addClass(_static.css.itemExpanded);
                if ($(this).udattr('read-only') == "true") {
                    //read only nodes don't open
                    return;
                }

                _static.fn.onOpenChapter(node, isAnimated, dontUpdateWindowLocation);
            },
            onCloseNode: function (ui) {
                var node = $(this);
                node.removeClass(_static.css.itemExpanded);

                //hide description
                if (node.find(_static.sel.itemDescription + " div").is(":visible")) {
                    //scroll to chapter
                    if (_static.fn.GetSettings().scrollOnOpen === true) {
                        node.find(_static.sel.itemDescription).slideUp();
                    } else {
                        node.find(_static.sel.itemDescription).hide();
                    }
                }
                if (node.ftattr("level") == 1) {
                    //node is a chapter
                    node.find(_static.sel.itemAddContentBtn).hide();
                    //grey out all other chapters
                    _static.fn.closedChaptersShow();
                    //set location hash to state
                    window.location.hash = "state";
                    $(_static.sel.activeChapterId).val("");
                    $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");
                }
            },

            onLoadNodes: function (ui) {

                var node = this;
                var tree = $(this).parent(_static.sel.tree);
                var id = $(node).ftattr("id");
                var level = $(node).ftattr("level");
                var widgetId = $(node).closest(".widgetItem").attr("id");

                if (widgetId == "" || widgetId == undefined) {
                    widgetId = $("#PX_MenuWidget .PX_Menu li.active").attr("id");
                }

                var data = {
                    itemid: id,
                    level: level,
                    operation: "load",
                    widgetId: widgetId
                };
                if (!ui.skipChapterOpen) {
                    _static.fn.onOpenChapter(node, true);
                }

                _static.fn.Loading("onLoadNodes", true, id);

                $.ajax({
                    url: PxPage.Routes.load_faceplate_loadchildren,
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: "html",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        //response is a composition of li elements.
                        //however, if there is empty space between each </li>'space'<li>, $(response) will add #text
                        var filteredResult = $(response).not('#text');
                        _static.fn.Loaded("onLoadNodes");

                        // we need to make sure that we're still working with the same tree.
                        // when TOC tabs switched the old tree isn't available and the new one hasn't loaded yet
                        if ($('#' + tree.attr('id')).length > 0) {
                            $('.faux-tree-node[data-ft-chapter="' + id + '"]').remove();
                            $(filteredResult).each(function () {
                                var oldNode = tree.find('[data-ft-id=' + $(this).ftattr('id') + ']');
                                if (oldNode.length) {
                                    $(oldNode).remove();
                                }
                            });
                            // hide the elements because they will be displayed via fauxtree callback
                            filteredResult.hide().insertAfter($(node));

                            if (ui.applySort == undefined || ui.applySort) {
                                $(node).fauxtree('sortTree', _static.fn.compareNodesOnDateSequence, true);
                            }

                            tree.fauxtree('scanNode', node);
                            _static.fn.markLastViewed($(node), id);

                            // faux-tree callback method
                            if (ui.callback) {
                                ui.callback();
                            }

                            $(PxPage.switchboard).trigger('onNodesLoaded');
                        }
                    },
                    error: function () {
                        _static.fn.Loaded('onLoadNodes');
                    }
                });

            },

            openContentSubfolders: function (tree, itemId) {
                var node = tree.fauxtree('getNode', itemId);

                var parentFolder = $(tree).fauxtree("getNodeData", node.ftattr("parent"));

                if (parentFolder.state != 'open') {
                    var parents = $(tree).fauxtree("getAllParents", node);

                    for (var i = 0; i < parents.length; i++) {
                        if (parents[i].ftattr('state') != _static.states.open) {
                            $(tree).fauxtree("toggleNode", parents[i]);
                        }
                    }
                }
            },
            markLastViewed: function (tree, chapterid) {
                var lastViewed;
                if ($(".last-viewed").length == 0 && $("#fne-window").is(":visible") && $("#fne-content #content-item .item-id").length > 0) { //FNE window is already open, use that as last viewed and enable prev/next buttons
                    //item was likely loaded via deep linking
                    var itemid = $("#fne-content #content-item .item-id").val();
                    PxPage.LargeFNE.ShowItemInFNE(itemid);
                    lastViewed = {
                        id: itemid,
                        parentId: chapterid
                    };
                } else {
                    // opening last visited subfoldersfolder and applying last viewed class based on cookie
                    lastViewed = $(tree).fauxtree("getNodeData", get_cookie('last_viewed_' + $("#CourseId").val()));
                }

                if (lastViewed) {
                    var lastViewedNode = tree.fauxtree('getNode', lastViewed.id);
                    if (!$(lastViewedNode).hasClass('last-viewed')) {
                        $(lastViewedNode).addClass('last-viewed');
                    }
                    if ($(lastViewedNode).length > 0 && lastViewed.parentId != undefined && lastViewedNode.ftattr("chapter") == chapterid) {
                        _static.fn.openContentSubfolders(tree, lastViewedNode.ftattr("id"));
                    }
                }
            },

            //forces a click on the href inside a node
            forceClickNode: function (node) {
                if ($(node).find(_static.sel.fneLink).length > 0) {
                    PxPage.TriggerClick($(node).find(_static.sel.fneLink));
                } else { //if there is no href, toggle the node instead
                    var itemType = $(node).udattr("itemtype");
                    //open node if content is a folder, else open FNE
                    if (itemType == "PxUnit") {
                        $(node.parents(_static.sel.tree)).fauxtree("toggleNode", $(node));
                    }
                }
            },

            // opens parent folder if closed for next/prev navigation in FNE
            forceParentFolderOpen: function (node) {
                var parent = $.fn.fauxtree('getNode', node.ftattr('parent'));

                if ($.fn.fauxtree('getNode', parent.ftattr('parent')) != undefined &&
                    $.fn.fauxtree('getNode', parent.ftattr('parent')).length > 0 &&
                    $.fn.fauxtree('getNode', parent.ftattr('parent')).ftattr('id') != 'PX_MULTIPART_LESSONS' &&
                    $.fn.fauxtree('getNode', parent.ftattr('parent')).ftattr('state') == 'closed') {
                    _static.fn.forceParentFolderOpen(parent);
                }

                if (parent != undefined &&
                    parent.length > 0 &&
                    parent.ftattr('id') != 'PX_MULTIPART_LESSONS' &&
                    parent.ftattr('state') == 'closed') {
                    $.fn.fauxtree('toggleNode', parent);
                }
            },

            triggerFromLC: function () {
                alert('Navigation to different sections of the e-book isn\'t supported when the section is opened in a dialog.');
            },

            // shows the next node in faux tree
            triggerNextNode: function (event, nodeOverride) {

                var nodeSelector = nodeOverride ? nodeOverride : _static.sel.validChapterNode($('.last-viewed'));
                var nextNode = $('.last-viewed').nextAll(nodeSelector).not('.hide-in-fne').first();

                if (nextNode == null) {
                    return false;
                }
                else if ($(_static.sel.lcDialog).is(':visible')) {
                    _static.fn.triggerFromLC();
                    return false;
                }

                $('.last-viewed').removeClass('last-viewed');

                var hasNext = $.fn.fauxtree("hasNextNode", "hide-in-fne");

                _static.fn.forceParentFolderOpen(nextNode);
                _static.fn.forceClickNode(nextNode);

                return hasNext;
            },
            // shows the previous node in faux tree
            triggerPrevNode: function (event, nodeOverride) {

                var nodeSelector = nodeOverride ? nodeOverride : _static.sel.validChapterNode($('.last-viewed'));
                var prevNode = $('.last-viewed').prevAll(nodeSelector).not('.hide-in-fne').first();

                if (prevNode == null) {
                    return false;
                }
                else if ($(_static.sel.lcDialog).is(':visible')) {
                    _static.fn.triggerFromLC();
                    return false;
                }

                $('.last-viewed').removeClass('last-viewed');

                var hasPrev = $.fn.fauxtree("hasPreviousNode", "hide-in-fne");

                _static.fn.forceParentFolderOpen(prevNode);
                _static.fn.forceClickNode(prevNode);

                return hasPrev;
            },
            scanUnitOrFolder: function (ui) {
                var node = $(this);
                var tree = ui.sender;
                var state = node.ftattr("state");
                if (state != _static.states.barren) {
                    var level = node.ftattr("level");
                    if (level == _static.minLevel) {
                        //this is a unit                        
                        if (node.find(_static.sel.fptitle).length > 0) {
                            //we're changing a folder into a unit
                            node.find(_static.sel.itemCollapsedIcon).hide();
                            node.find(_static.sel.fptitle).removeClass("fptitle").addClass('unitfptitle');

                            //node.find(_static.sel.pxDefaultText).removeClass("emptyFolder");
                            node.find(_static.sel.itemDescription).slideDown();
                            //flag unit as having children in order to trigger loadchildren
                            node.ftattr("has-children", "true");

                            //add add button
                            node.find(_static.sel.itemAddContentBtn).remove();
                            node.find(_static.sel.itemDescription).after('<div class="addContentBtn"><div class="btn-wrapper gradient"><a id="add-assignment-btn">Add to this Unit</a></div></div>');
                            //force a reload of the children on expand
                            $(node).ftattr("forceloadnode", "true");
                            //toggle node to ensure node is open after drop, load children
                            if (state == _static.states.open) {
                                tree.fauxtree("toggleNode", $(node));
                                tree.fauxtree("toggleNode", $(node));
                            }
                        } else {
                            if (state == _static.states.open) {
                                if (tree.fauxtree("getChildren", $(node)).not('.item-type-chapterresourceslinksfixed').length == 0) {
                                    node.find(_static.sel.pxDefaultText).addClass("emptyFolder");
                                } else {
                                    node.find(_static.sel.pxDefaultText).removeClass("emptyFolder");
                                }
                            }
                        }
                    } else {
                        //this is a folder
                        if (tree.fauxtree("hasChildren", $(node))) {
                            //if folder has children
                            //remove browse more resource children
                            $(_static.sel.childOf(node.ftattr("id")) + "[data-ud-itemtype='ChapterResourcesLinksFixed']").remove();
                        }
                        if (!tree.fauxtree("hasChildren", $(node)) && state == _static.states.open) {
                            //if folder is empty and open, add emptyfolder class
                            node.find(_static.sel.pxDefaultText).addClass("emptyFolder");
                            //if folder is empty, flag as not having children
                            node.ftattr("has-children", "false");
                        } else {
                            node.find(_static.sel.pxDefaultText).removeClass("emptyFolder");
                            node.find(_static.sel.pxDefaultText).hide();
                            node.find(_static.sel.itemDescription).hide();
                        }

                        var nodefptitle = node.find(_static.sel.unitfptitle);
                        if (nodefptitle.length > 0) {
                            nodefptitle.removeClass("unitfptitle").addClass('fptitle');
                            if (nodefptitle.prev()) {
                                if (nodefptitle.prev().hasClass("icon-placeholder")) {
                                    nodefptitle.prev().removeClass("icon-placeholder").addClass("icon collapsed");
                                }
                                if (!nodefptitle.prev().hasClass("icon")) {
                                    nodefptitle.before("<span class='icon collapsed'></span>");
                                }
                            }
                        }
                    }
                    if (state == _static.states.open) {
                        if (node.find(_static.sel.emptyFolder).length > 0) {
                            if (node.find(_static.sel.itemDescription + " " + _static.sel.pxDefaultTextEmpty).length == 0) {
                                node.find(_static.sel.itemDescription).append("<div class='px-default-text-empty'></div>");
                            }
                            node.find(_static.sel.itemDescription + " " + _static.sel.pxDefaultTextEmpty).html("This " + (level == 1 ? "unit" : "folder") + " is empty.");
                            node.find(_static.sel.itemDescription + " " + _static.sel.pxDefaultTextEmpty).show();
                            node.find(_static.sel.itemDescription).show();
                        } else {
                            node.find(_static.sel.itemDescription + " " + _static.sel.pxDefaultTextEmpty).hide();
                        }
                    }
                    _static.fn.updatePastLaterToggles(node, $(_static.sel.childOf(node)));
                }
                _static.fn.updateDueDayStyles(node);

            },

            //---END FAUX-TREE EVENTS

            //-- Server-side actions

            ///Arguments:
            ///itemId
            ///targetId
            ///AssignmentItem - seralized faux tree item
            ///operation - "move", "assigndatetoitm", "unassigndatetoitem", "newitem"            
            itemOperation: function (args) {
                var data = {
                    itemId: args.itemId,
                    targetId: args.targetId,
                    assignedItem: args.assignedItem,
                    operation: args.operation,
                    entityId: args.entityId,
                    container: args.container,
                    toc: args.toc
                };

                url = PxPage.Routes.launchpad_item_operation;
                dataType = "json";

                $.ajax({
                    url: url,
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: dataType,
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        if (data.entityId == '' || data.entityId == $("#CourseId").val()) {
                            var tree = $(_static.sel.tree);

                            var corrected = [];
                            for (var i = 0; i < response.length; i++) {
                                if ($(_static.sel.getItem(response[i].id)).length > 0) {
                                    if (response[i].level == null) {
                                        response[i].level = $(_static.sel.getItem(response[i].id)).ftattr('level');
                                    }

                                    corrected.push(response[i]);
                                }
                            }
                            //disable tree filtering in fauxtree so that nodes can be deserialized into both trees
                            $.fn.fauxtree("setFilterNodesByTree", false);
                            $.fn.fauxtree("setFilterNodesByTree", false);
                            tree.fauxtree("deserialize", corrected, _static.fn.setCustomNodeData);

                            _static.fn.sortFauxTree();
                            _static.fn.scanFauxTree();
                        }

                        $(PxPage.switchboard).trigger("loadcurrentdueitem");
                        
                        if ($(_static.sel.getItem(data.itemId)).length > 0) {
                            var rawTitle = $(_static.sel.getItem(data.itemId)).find("span.fptitle, span.unitfptitle");
                            if (rawTitle.length) {
                                _static.fn.SaveNavigationToast(data.operation, rawTitle.text());
                            }
                        }

                        if (args.callback != null) {
                            args.callback();
                        }
                    }
                });
            },
            SaveNavigationToast: function (operation, rawtitle, foldertitle) {
                if (rawtitle == null || rawtitle.length == 0) {
                    rawtitle = "Item";
                }
                
                switch (operation) {
                    case _static.operation.assign:
                    case _static.operation.moveAndAssign:
                        // to stop showing toast message <!-- do not remove -->
                        break;
                    case _static.operation.remove:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.Remove, rawtitle));
                        break;
                    case _static.operation.removeOnUnassign:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.RemoveOnUnassign, rawtitle));
                        break;
                    case _static.operation.newItem:
                        if (foldertitle.length)
                        {
                            PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.NewItemTo, rawtitle, foldertitle));
                        } else {
                            PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.NewItemName, rawtitle));
                        }
                        break;
                    case _static.operation.move:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.Move, rawtitle));
                        break;
                    case _static.operation.visible:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.VisibleToStudents, rawtitle, operation));
                        break;
                    case _static.operation.invisible:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.InvisibleToStudents, rawtitle, operation));
                        break;
                    default:
                        PxPage.Toasts.Success($.format(PxPage.Toasts.Operation.Default, operation));
                }
            },

            ///Arguments:
            ///catagory - nav-category that will be serialized
            ///tree - faux-tree to serialize
            ///changed - jquery obj - node that has been changed 
            ///above - node above the moved item (used for move)
            ///below - node below the moved item (used for move)
            ///operation to perform - removed, newitem, move
            ///loadView - if true, returns the changed item HTML. if false, returns entire JSON tree
            ///onSuccess - callback to call when save completes successfully 
            saveNavigationState: function (args) {
                var settings = _static.fn.GetSettings();
                var toc = args.toc || settings.toc;
                var widgetid = settings.widgetId;
                var isSameToc = settings.toc === settings.assignmentToc;

                var data = {
                    category: _static.fn.getCategoryData(args),
                    changed: args.changed && args.changed.length > 0 ? args.tree.fauxtree("serialize", _static.fn.getCustomNodeData, args.changed) : null,
                    above: args.above && args.above.length > 0 ? args.tree.fauxtree("serialize", _static.fn.getCustomNodeData, args.above) : null,
                    below: args.below && args.below.length > 0 ? args.tree.fauxtree("serialize", _static.fn.getCustomNodeData, args.below) : null,
                    entityId: args.entityId,
                    operation: args.operation,
                    toc: toc,
                    removeFrom: args.removeFrom || toc,
                    unitContainer: args.unitContainer
                };

                _static.fn.Loading("saveNavigationState", true);

                var url, dataType;
                if (args.loadView) {
                    url = PxPage.Routes.save_launchpad_navigation_view;
                    dataType = "html";
                } else {
                    url = PxPage.Routes.save_launchpad_navigation;
                    dataType = "json";
                }

                $.ajax({
                    url: url,
                    type: "POST",
                    data: JSON.stringify({
                        state: data,
                        widgetId: widgetid,
                        toc: toc,
                        keepInGradebook: args.keepInGradebook
                    }),
                    dataType: dataType,
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        var operation = args.operation;
                        
                        if (args.loadView) {
                            if (isSameToc && args.operation == _static.operation.removeOnUnassign) // remove node only if the operation is run on the existing tree structure
                            {
                                $.fn.fauxtree("removeNode", data.changed.id);
                            } else {
                                var appendAll = args.operation !== _static.operation.unassign
                                    && args.operation !== _static.operation.removeOnUnassign
                                    && args.operation !== _static.operation.moveAndAssign;

                                _static.fn.saveNavigationStateParseViewResponse(response, appendAll);

                                $(PxPage.switchboard).trigger('contentsaved');
                            }
                        } else {
                            //response is the serialized tree
                            //disable tree filtering in fauxtree so that nodes can be deserialized into both trees
                            $.fn.fauxtree("setFilterNodesByTree", false);
                            args.tree.fauxtree("deserialize", response.category.items, _static.fn.setCustomNodeData);
                            args.tree.fauxtree("deserialize", response.changed, _static.fn.setCustomNodeData);
                            
                            if (settings.splitAssignedUnassigned) {
                                $(response.changes).each(function () {
                                    var item = $(_static.sel.getItem(this.id));
                                    var nodeLevel = $(item).ftattr("level");

                                    if (nodeLevel == 1) {
                                        if (new Date(parseInt(this.startDate.replace(/\/Date\((.*?)[+-]\d+\)\//i, "$1"))).getFullYear() <= 2 &&
                                            new Date(parseInt(this.startDate.replace(/\/Date\((.*?)[+-]\d+\)\//i, "$1"))).getFullYear() <= 2) {
                                            $(_static.sel.unassignedTree).append(item);
                                            $(_static.sel.unassignedTree).fauxtree("moveTree", item, null, $(item).prevAll(_static.sel.isLevel(nodeLevel)).first());
                                            $(_static.sel.unassignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, true);
                                        }
                                        else {
                                            item.find(_static.sel.itemAssignButton).hide();

                                            $(_static.sel.assignedTree).append(item);
                                            $(_static.sel.assignedTree).fauxtree("moveTree", item, null, $(item).prevAll(_static.sel.isLevel(nodeLevel)).first());
                                            $(_static.sel.assignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, true);
                                        }

                                        item.addClass("fade-effect");
                                    }
                                });
                            }

                            $.fn.fauxtree("setFilterNodesByTree", true);

                            if (args.changed) {
                                $(args.changed).addClass("fade-effect");
                            }

                            if (args.operation == _static.operation.showOrHide) {
                                if (data.changed.isvisibletostudents == "true") {
                                    operation = _static.operation.visible;
                                }
                                else {
                                    operation = _static.operation.invisible;
                                }
                            }
                            
                            $(PxPage.switchboard).trigger('contentsaved');
                        }

                        var newNode = $.fn.fauxtree("getNode", args.changed.ftattr("id"));
                        var title = newNode.find(".fptitle a").text();
                        _static.fn.SaveNavigationToast(operation, title, _static.fn.GetParentFolderTitle(args.changed));

                        if (!args.unitContainer) {
                            _static.fn.sortFauxTree(true);
                        }

                        _static.fn.scanFauxTree();

                        if (args.onSuccess) {
                            args.onSuccess(response);
                        }

                        _static.fn.UpdateDueCounts();
                        _static.fn.Loaded("saveNavigationState");
                        _static.fn.TitleVisibility();

                        PxPage.Fade();

                        if (PxPage.Context.IsSandboxCourse) {
                            $(PxPage.switchboard).trigger("refereshsandboxalert");
                        }

                        if (args.tree.find('.faux-tree-node').length == 0) {
                            $('.empty-launchpad').show();
                        }
                        else {
                            $('.empty-launchpad').hide();
                        }

                        $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");
                    },
                    error: function () {
                        args.category.unblock();
                    }
                });
            },
            //Response: list of HTML entities containing nodes to update in the fauxtree
            //AppendAll: true/false, appends new nodes to the tree if they do not exist (Default: false)
            saveNavigationStateParseViewResponse: function (response, appendAll) {
                var settings = _static.fn.GetSettings();
                if (appendAll == null) {
                    appendAll = false;
                }
				var needSortUnassignedTree = false, needSortAssignedtree = false;
                //response is a view, replace node with it
                $(response).each(function (i, newItem) {
                    var itemId = $(newItem).ftattr("id");
                    var oldItem = $(_static.sel.getItem(itemId));

                    if ($(newItem).udattr('itemtype') == "PxUnit" && $(oldItem).ftattr('level') == 1) {
                        $(newItem).ftattr("forceloadnode", "true");
                    }

                    var nodeState = $(oldItem).ftattr("state");
                    var nodeLevel = $(oldItem).ftattr("level");
                    if (oldItem != null && oldItem.length > 0) {
                        //if this is a root node, move to the correct assigned/unassigned tree if necessary
                        if (settings.splitAssignedUnassigned && nodeLevel == 1) {
                            //move assigned items up to the assigned UL
                            //move unassigned items down to the unassigned UL
                            if ($(newItem).udattr("is-assigned") === "true") {
                                $(_static.sel.assignedTree).append(oldItem);
                                $(_static.sel.assignedTree).fauxtree("moveTree", oldItem, null, null);
                                //sort tree to ensure items are in correct order after the move
                                needSortAssignedtree = true;
                            } else {
                                $(_static.sel.unassignedTree).append(oldItem);
                                $(_static.sel.unassignedTree).fauxtree("moveTree", oldItem, null, null);
                                //sort tree to ensure items are in correct order after the move
                                needSortUnassignedTree = true;
                            }
                        }

                        $(oldItem).replaceWith(newItem);
                        $(newItem).addClass("fade-effect");

                        if ($(oldItem).hasClass('active')) {
                            $(newItem).addClass("active");
                        }
                        if ($(oldItem).hasClass('last-viewed')) {
                            $(newItem).addClass("last-viewed");

                            set_cookie("last_viewed_" + $("#CourseId").val(), $(newItem).ftattr('id'), '0', '/');
                        }
                        if (nodeState == "open") {
                            $(newItem).ftattr("state", nodeState);
                            $(newItem).find(_static.sel.itemCollapsedIcon).attr("class", $(oldItem).find(_static.sel.itemCollapsedIcon).attr("class"));

                            _static.fn.onOpenNode.apply($(newItem), [null, false, true]); //dont update the window location hash when saving nav state
                        }
                        if (nodeLevel != $(newItem).ftattr("level")) {
                            $(newItem).ftattr("level", nodeLevel);
                        }
                    }
                    else {
                        if (appendAll) {
                            var parentNode = $(_static.sel.getItem($(newItem).ftattr("parent")));

                            if (parentNode.length <= 0) {
                                parentNode = $(_static.sel.getItem($(newItem).ftattr("chapter")));
                            }
                            if (parentNode.length <= 0) {
                                parentNode = $(_static.sel.tree + " li").first();
                            }
                            $(parentNode).after(newItem);
                            nodeLevel = $(parentNode).ftattr("level") + 1;
                            $(newItem).ftattr("level", nodeLevel);
                            $(newItem).addClass("fade-effect");
                        }
                    }

                });
				_static.fn.UpdateDueCounts();
				if (needSortUnassignedTree) {
					$(_static.sel.unassignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, true);
				}
				if (needSortAssignedtree) {
					$(_static.sel.assignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, true);
				}
            },
            // get category information
            ///Arguments:
            ///catagory - nav-category that will be serialized
            ///tree - faux-tree to serialize
            ///changed - jquery obj - node that has been changed 
            ///above - node above the moved item (used for move)
            ///below - node below the moved item (used for move)
            ///operation to perform - removed, newitem, move
            ///loadView - if true, returns the changed item HTML. if false, returns entire JSON tree
            ///onSuccess - callback to call when save completes successfully 
            // includeTree - include both trees in serialization process
            getCategoryData: function (args) {
                var category = args.category;

                var target = typeof (category) === "string" ? $(_static.sel.categoryById(category)) : category;

                //trim items by only including chapters related to changed/above/below
                var relevantItems = $.merge([], args.changed);
                if (args.above != null) {
                    $.merge(relevantItems, args.above);
                }
                if (args.below) {
                    $.merge(relevantItems, args.below);
                }

                if (args.operation == _static.operation.move) {
                    var sourceContainerId = args.changed.attr('data-ft-chapter');
                    if (sourceContainerId) {
                        var sourceContainerNode = $(_static.sel.getItem(sourceContainerId));
                        if (sourceContainerNode && sourceContainerNode.length) {
                            $.merge(relevantItems, sourceContainerNode);
                        }
                    }

                }

                //var relevantItems = $.merge($.merge($.merge([], args.changed), args.above), args.below);
                var items = [];
                var itemIds = {};
                $.each(relevantItems, function (index) {
                    var node = $(this);
                    var parentChapter = $(_static.sel.tree).fauxtree("topAncestorOrSelf", node);
                    if (itemIds[$(parentChapter).ftattr("id")] == null) {
                        itemIds[$(parentChapter).ftattr("id")] = parentChapter;

                        $.merge(items, args.tree.fauxtree("serialize", _static.fn.getCustomNodeData, parentChapter, true));
                    }
                });

                var data = {
                    id: target.udattr("id"),
                    items: items,
                    startDate: target.udattr("start-date"),
                    endDate: target.udattr("due-date")
                };

                return data;
            },
            // gets custom information that is stored on each fauxtree node.
            getCustomNodeData: function (node, obj) {

                if (node == null || node.length == 0) {
                    return obj;
                }

                if ($(node).find('#selCalculationTypeTrigger option:selected').length > 0) {
                    obj.submissiongradeaction = $(node).find('#selCalculationTypeTrigger option:selected').val();
                }
                else {
                    obj.submissiongradeaction = "-1";
                }

                var startDate = null;
                var dueDate = null;

                if (node.udattr("start-date") != undefined && node.udattr("start-date").substr(node.udattr("start-date").indexOf('/', 3) + 1) != "0001") {
                    startDate = dateFormat.dateConvertToUtc(new Date(node.udattr("start-date")), node.udattr("start-time"));
                }

                if (node.udattr("due-date") != undefined && node.udattr("due-date").substr(node.udattr("due-date").indexOf('/', 3) + 1) != "0001") {
                    dueDate = dateFormat.dateConvertToUtc(new Date(node.udattr("due-date")), node.udattr("due-time"));
                }

                obj.startDate = startDate;//node.udattr("start-date");
                obj.endDate = dueDate;//node.udattr("due-date");
                obj.points = node.udattr("points");
                obj.gradebookcategory = node.udattr("gradebookcategory");
                obj.previousparentid = node.udattr("previousparentid");
                obj.wasduedatemanuallyset = node.udattr("wasduedatemanuallyset");
                obj.defaultpoints = node.udattr("default-points");
                var title = null;
                if (node.find("span.fptitle").length > 0) {
                    title = $.trim(node.find("span.fptitle").text().replace('\n', ''));
                } else if (node.find("span.unitfptitle").length > 0) {
                    title = $.trim(node.find("span.unitfptitle").text().replace('\n', ''));
                }
                obj.rawtitle = title;
                obj.isvisibletostudents = node.udattr("isvisibletostudents");
                obj.type = node.udattr("itemtype");
                return obj;
            },
            // sets custom information that is stored on each fauxtree node.
            setCustomNodeData: function (node, obj) {
                var startDate = dateFormat.dateConvertFromCourseTimeZone(dateFormat.jsonDate(obj.startDate)),
                    endDate = dateFormat.dateConvertFromCourseTimeZone(dateFormat.jsonDate(obj.endDate)),
                    startFormatted = "01/01/0001",
                    endFormatted = "01/01/0001";
                if (node.udattr("date-mode") == undefined && obj.datemode != "") {
                    node.udattr("date-mode", obj.datemode);
                }
                try {
                    if (startDate != undefined) {
                        startFormatted = startDate.format("mm/dd/yyyy HH:MM:ss");
                    }
                } catch (err) {
                }

                try {
                    if (endDate != undefined) {
                        endFormatted = endDate.format("mm/dd/yyyy HH:MM:ss");
                    }
                } catch (err) {
                }

                _static.fn.setAssignmentDates(node, startFormatted, endFormatted, obj.points);
            },
            //-- END Server-side actions

            //-- Assignment-related functionality
            isDateTimeMin: function (value) {
                return value == "01/01/0001";
            },
            ///arguments:
            ///id - id of the item assigned
            ///date - due date assigned
            ///startdate - start date assigned 
            ///category - TOC category to use for assignment (ie: syllabusFilter)
            ///gradebookcategory - gradebook Category
            ///points - number of points
            ///operation:str - operation for content assignment helper
            onContentAssigned: function (argsParam) {
                var item = $(_static.sel.getItem(argsParam.id));
                var tree = $(item).parent(_static.sel.tree);

                if (item.udattr("itemtype") == "PxUnit" && item.ftattr("state") == "closed" && $(tree).fauxtree("isNotLoaded", item)) {
                    $(PxPage.switchboard).bind("onNodesLoaded", function () {
                        _static.fn.processContentAssignment(argsParam);

                        $(PxPage.switchboard).unbind("onNodesLoaded");
                    });
                    _static.fn.forceClickNode(item);
                } else {
                    _static.fn.processContentAssignment(argsParam);
                }
            },
            processContentAssignment: function (argsParam) {
                var settings = _static.fn.GetSettings();

                PxPage.log('content : processing assignment >');
                PxPage.log(argsParam);

                var item = $(_static.sel.getItem(argsParam.id));

                if (item == null || item.length == 0) {
                    var dueDate = dateFormat.dateConvertToUtc(argsParam.date);
                    var args = {
                        itemId: argsParam.id,
                        assignedItem: {
                            id: argsParam.id,
                            DueDate: dueDate,
                            EndDate: dueDate
                        },
                        operation: argsParam.operation || _static.operation.assign,
                        entityId: argsParam.entityId,
                        callback: argsParam.callback,
                        toc: settings.toc
                    };

                    if ($('#selCalculationTypeTrigger option:selected')) {
                        args.assignedItem.submissiongradeaction = $('#selCalculationTypeTrigger option:selected').val();
                    }
                    else {
                        args.assignedItem.submissiongradeaction = "-1";
                    }

                    _static.fn.itemOperation(args);

                    return;
                }

                item.udattr("due-date", dateFormat(argsParam.date, 'mm/dd/yyyy'));                

                if (argsParam.startdate != "" && argsParam.startdate != undefined) {
                    item.udattr("start-date", dateFormat(argsParam.startdate, 'mm/dd/yyyy'));
                }

                item.udattr("due-time", new Date(argsParam.date).format("h:MM TT"));

                if (argsParam.startdate != "" && argsParam.startdate != undefined) {
                    item.udattr("start-time", new Date(argsParam.startdate).format("h:MM TT"));
                }

                if (item.udattr("itemtype") != "PxUnit" && argsParam.points != undefined) {
                    item.udattr("points", argsParam.points);
                }

                item.udattr("gradebookcategory", argsParam.gradebookcategory);

                var data = {
                    category: $(item).parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: $(item),
                    above: null,
                    below: null,
                    operation: argsParam.operation || _static.operation.assign,
                    loadView: true,
                    entityId: argsParam.entityId,
                    toc: settings.toc,
                    unitContainer: {
                        toc: settings.assignmentToc,
                        unitId: (argsParam.unit) ? argsParam.unit.unitId : "",
                        categoryId: (argsParam.unit) ? argsParam.unit.categoryId : ""
                    },
                    onSuccess: function () {
                        _static.fn.hideManagementCard();
                        $(PxPage.switchboard).trigger("loadcurrentdueitem");
                    }
                };

                _static.fn.saveNavigationState(data);
            },
            onContentUnAssigned: function (argsParam) {
                var item = $(_static.sel.getItem(argsParam.id));
                var tree = $(item).parent(_static.sel.tree);

                if (item.udattr("itemtype") == "PxUnit" && item.ftattr("state") == "closed" && $(tree).fauxtree("isNotLoaded", item)) {
                    $(PxPage.switchboard).bind("onNodesLoaded", function () {
                        _static.fn.processContentUnAssignment(argsParam);

                        $(PxPage.switchboard).unbind("onNodesLoaded");
                    });
                    _static.fn.forceClickNode(item);
                } else {
                    _static.fn.processContentUnAssignment(argsParam);
                }
            },
            processContentUnAssignment: function (argsParam) {
                var item = $(_static.sel.getItem(argsParam.id));
                var data;
                if (item == null || item.length == 0) {
                    var dueDate = dateFormat.dateConvertToUtc(argsParam.date);
                    var startDate = dateFormat.dateConvertToUtc(argsParam.startdate);
                    data = {
                        itemId: argsParam.id,
                        assignedItem: {
                            id: argsParam.id,
                            StartDate: startDate,
                            EndDate: dueDate,
                            MaxPoints: argsParam.points
                        },
                        operation: argsParam.operation || _static.operation.unassign,
                        keepInGradebook: argsParam.keepInGradebook,
                        toc: argsParam.toc,
                        callback: argsParam.callback
                    };
                    _static.fn.itemOperation(data);

                    return;
                }

                item.udattr("due-date", '01/01/0001');
                item.udattr("start-date", '01/01/0001');
                item.udattr("points", argsParam.points == null ? "" : argsParam.points);
                item.udattr("gradebookcategory", "");

                _static.fn.hideManagementCard();
                data = {
                    category: $(item).parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: $(item),
                    above: null,
                    below: null,
                    operation: argsParam.operation || _static.operation.unassign,
                    keepInGradebook: argsParam.keepInGradebook,
                    toc: argsParam.toc,
                    onSuccess: function () {
                        $(PxPage.switchboard).trigger("loadcurrentdueitem");
                        if (typeof argsParam.callback === 'function') {
                            argsParam.callback();
                        }
                    },
                    loadView: true
                };
                _static.fn.saveNavigationState(data);
            },

            selectEventUnblock: function (enable) {
                if (enable) {
                    $(_static.sel.item).find('select').bind('mousedown', function (event) {
                        event.stopPropagation();
                    });
                }
                else {
                    $(_static.sel.item).find('select').unbind('mousedown');
                }
            },

            setAssignmentDates: function (node, start, end, points) {
                var settings = _static.fn.GetSettings();
                var startDate = new Date(start),
                    endDate = new Date(end),
                    dates = node.find(_static.sel.itemCalendarBox),
                    dateSet = false,
                    dateMode = node.udattr("date-mode");

                var isValidDate = (end != "01/01/0001") && ((new Date()).format("yyyy") - (new Date(end)).format("yyyy") < 4);

                if (dateMode === "nodates" || (end == "01/01/0001" && start == "01/01/0001") || !isValidDate) {
                    dates.hide();
                    node.find(_static.sel.itemAssignButton).show();
                    return;
                }

                if (dates.length == 0) {
                    node.find(_static.sel.nodeTitle).after('<a class="assignment-dates" href="#"></a>');
                    dates = node.find(_static.sel.assignmentDates);
                }


                var dateToDisplayMonth = "";
                var dateToDisplayDay = "";

                var displayStartDateMonth = startDate.format("mmm");
                var displayStartDateDay = startDate.format("dd");
                var displayEndDateMonth = endDate.format("mmm");
                var displayEndDateDay = endDate.format("dd");

                var today = new Date();
                var dueSoon = false;
                if (!_static.fn.isDateTimeMin(start) && dateMode === "range") {
                    //dates.append('<span class="assignment-start-date">' + startDate.format("mm/dd") + '-</span>');
                    node.udattr("start-date", startDate.format("mm/dd/yyyy"));
                    node.udattr("start-time", startDate.format("HH:MM:ss"));
                    if (displayStartDateMonth != displayEndDateMonth) {
                        dateToDisplayMonth = displayStartDateMonth + '-';
                    }
                    if (displayStartDateDay != displayEndDateDay) {
                        dateToDisplayDay = displayStartDateDay + '-';
                    }

                    dueSoon = ((startDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)) < settings.dueSoonDays;

                    dateSet = true;
                } else {
                    node.udattr("start-date", "01/01/0001");
                }

                if (!_static.fn.isDateTimeMin(end)) {
                    //dates.append('<span class="assignment-due-date">' + endDate.format("mm/dd") + '</span>');
                    dateToDisplayMonth += displayEndDateMonth;
                    dateToDisplayDay += displayEndDateDay;
                    node.udattr("due-date", endDate.format("mm/dd/yyyy"));
                    node.udattr("due-time", endDate.format("HH:MM:ss"));
                    dateSet = true;
                    if (!dueSoon) {
                        dueSoon = ((endDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)) < settings.dueSoonDays;
                    }
                } else {
                    node.udattr("due-date", "01/01/0001");
                    node.udattr("due-time", "12:00 AM");
                }


                if (!dateSet) {
                    dates.show();
                } else {
                    //dates.append(dateToDisplay);
                    dates.find(".dd_cal_month_inner").empty().append("<span>" + dateToDisplayMonth + "</span>");
                    dates.find(".dd_cal_date").empty().append("<span>" + dateToDisplayDay + "</span>");
                    //DEBUG TIME
                    //                    dates.find(".time").remove();
                    //                    dates.append("<div class='time'>" + node.udattr("due-time") + "</div>");
                    //END DEBUG TIME
                    if (dueSoon) {
                        $(dates).addClass("due_soon");
                    } else {
                        $(dates).removeClass("due_soon");
                    }
                    dates.show();
                    if (points != undefined && !node.hasClass("item-type-pxunit")) {
                        node.find(_static.sel.itemPoints).html(points + (Number(points) > 1 ? " pts" : " pt"));
                        node.find(_static.sel.itemAssignButton).hide();
                    }
                }

            },
            //-- END Assignment related functionality


            //---Management Card Functionality
            showManagementCard: function (item, mode) {
                //mode = "open" -- show calendar
                //mode = "close -- hide calendar
                //if item is already a node, use that, otherwise find parent node
                var node;
                if ($(item).hasClass(_static.css.item)) {
                    node = item;
                } else {
                    node = $(item).parents(_static.sel.item);
                }

                var rightDiv = node.find(_static.sel.itemManagementCardPlaceholder);
                if (rightDiv.length > 0 && !$(rightDiv).is(":visible")) {
                    //hide current management card
                    _static.fn.showAssignCalendar("close");
                    $(_static.sel.itemManagementCardPlaceholder).hide();
                    //position management card
                    var left = ($(node).parents(_static.sel.tree).position()).left + ($(node).parents(_static.sel.tree).outerWidth());
                    left = left - 12;
                    var ct = $(node).find(_static.sel.itemButtons).position().top;

                    var y = Math.round(rightDiv.height() / 2);
                    if ($(rightDiv).find('.assign-arrow').length > 0) {
                        y = $(rightDiv).find('.assign-arrow').css('top').replace('px', '');
                    }

                    ct = ct - y;

                    var rightDivArrowOffset = 0;
                    if (ct < -64) {
                        rightDivArrowOffset = -64 - ct;
                        ct = -64;
                    }
                    rightDiv.css("top", ct + "px");
                    rightDiv.css('left', left);
                    rightDiv.show("drop", { direction: "right" });

                    var contentItemId = node.attr('data-ud-id');

                    $(_static.sel.item).removeClass(_static.css.itemManagementCardOpen);
                    node.addClass(_static.css.itemManagementCardOpen);

                    $(_static.sel.itemManagementCardPlaceholder).empty();
                    $(_static.sel.itemManagementCardPlaceholder).block({
                        message: null
                    });

                    var widgetId = $(node).closest(".widgetItem").attr("id");
                    var args = {
                        contentItemId: contentItemId,
                        widgetId: widgetId
                    };

                    PxPage.log("Launchpad: show assign for: " + contentItemId);

                    $.post(PxPage.Routes.show_faceplate_managementcard, args, function (response) {
                        $(rightDiv).html(response);
                        $(_static.sel.itemManagementCardPlaceholder).unblock({
                            message: null
                        });
                        //hide move or copy for root-level items
                        if (node.attr("data-ft-level") == "1" && node.udattr("itemtype") == "PxUnit") {
                            node.find(_static.sel.cardMoveOrCopy).hide();
                        }
                        //position arrow
                        if (rightDivArrowOffset != 0) {
                            var arrowTop = $(".assign-arrow").css("top").replace(/[^-\d\.]/g, "");
                            arrowTop = (+arrowTop) - rightDivArrowOffset;
                            $(".assign-arrow").css("top", arrowTop + "px");
                        }
                        //show correct mode for the management card
                        _static.fn.showAssignCalendar(mode, contentItemId);
                        //bind close button
                        $(document).off('click', _static.sel.itemManagementCardClose).on('click', _static.sel.itemManagementCardClose, _static.fn.hideManagementCard);

                        // publish event with render delay
                        setTimeout(function () {
                            $(PxPage.switchboard).trigger('managementcard.show', contentItemId);
                        }, 100);

                        // disable jqui sortable when card is open
                        // since ie>8 cannot handle form select menus while <ul> has a mousedown event
                        if (PxPage.isIE()) {
                            _static.fn.selectEventUnblock(true);
                        }

                    });
                } else {
                    _static.fn.hideManagementCard();

                }
                return false;
            },
            hideManagementCard: function () {
                //hide all management cards
                $(_static.sel.itemManagementCard + ":visible").hide("drop", { direction: "right" }, "slow", function () {
                    $(_static.sel.itemManagementCard).empty();
                });
                $(_static.sel.item).removeClass(_static.css.itemManagementCardOpen);
                $(PxPage.switchboard).trigger('managementcard.hide');
                if (PxPage.isIE()) {
                    _static.fn.selectEventUnblock(false);
                }
            },
            disableManagementCard: function () {
                $(_static.sel.itemButtons).addClass(_static.css.hidden); //hide assignment buttons and dates
            },
            enableManagementCard: function () {
                $(_static.sel.itemButtons).removeClass(_static.css.hidden); //show assignment buttons and dates
            },

            showAssignCalendar: function (mode, itemId) {
                var node = $.fn.fauxtree("getNode", itemId);
                if (node == null || !node.length) {
                    node = $(_static.sel.itemManagementCardPlaceholder + ":visible").parent(_static.sel.item);
                }
                if (mode == "open") {
                    node.find(".assign-showCalendar-open").hide();
                    node.find(".faceplate-unit-calendar").show();

                    node.find(".faceplate-student-completion").addClass("faceplate-calendar-open");
                    node.find(".faceplate-student-completion").removeClass("faceplate-calendar-close");

                } else if (mode == "close") {
                    node.find(".assign-showCalendar-open").show();
                    node.find(".faceplate-unit-calendar").hide();

                    node.find(".faceplate-student-completion").removeClass("faceplate-calendar-open");
                    node.find(".faceplate-student-completion").addClass("faceplate-calendar-close");

                }
            },

            ToggleVisibility: function () {
                var nodeId = $(this).parents(_static.sel.item).ftattr("id");
                var hiddenFromStudent = true;
                if ($(this).hasClass("managementcard_students_show")) {
                    $(".managementcard_students_show").hide();
                    $(".managementcard_students_hide").show();
                    hiddenFromStudent = true;
                } else {
                    $(".managementcard_students_show").show();
                    $(".managementcard_students_hide").hide();
                    hiddenFromStudent = false;
                }

                var data = {
                    id: nodeId,
                    hiddenFromStudent: hiddenFromStudent
                };

                _static.fn.updateItemSettings(data);
                //$(PxPage.switchboard).trigger("settingsUpdate", [data]);

                return false;

            },
            //---END Management Card Functionality

            // ---- LaunchPad settings
            TogglePastDue: function () {
                var settings = _static.fn.GetSettings();
                if ($("#hide_past_due").is(":checked")) {
                    $(".past-due").each(function (i) {
                        var id = $(this).ftattr("id");
                        var node = $(".faux-tree-node[data-ft-id=\"" + id + "\"] ");
                        if ($(this).hasClass("nodeExpanded")) {
                            $(this).find(".unitfptitle").click();
                        }
                    });
                    $(".past-due").each(function (i) {
                        $(this).ftattr("visible", "false");
                    });
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "hide_past_due" + "=" + "1" + "; path=/; expires= 0"; // +expiry.toGMTString();
                } else {
                    $(".past-due").each(function (i) {
                        var id = $(this).ftattr("id");
                        var node = $(".faux-tree-node[data-ft-id=\"" + id + "\"] ");
                        if ($(this).hasClass("nodeExpanded")) {
                            $(this).find(".unitfptitle").click();
                        }
                    });
                    $(".past-due").each(function (i) {
                        $(this).ftattr("visible", "true");
                    });
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "hide_past_due" + "=" + "0" + "; path=/; expires= 0";
                }
                _static.fn.scanFauxTree();
            },

            ToggleDueLater: function () {
                var settings = _static.fn.GetSettings();
                if ($("#hide_due_later").is(":checked")) {
                    $(".due-later").each(function (i) {
                        var id = $(this).ftattr("id");
                        var node = $(".faux-tree-node[data-ft-id=\"" + id + "\"] ");
                        if ($(this).hasClass("nodeExpanded")) {
                            $(this).find(".unitfptitle").click();
                        }
                    });
                    $(".due-later").each(function (i) {
                        $(this).ftattr("visible", "false");
                    });
                    $(".due-later").addClass("hide");
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "hide_due_later" + "=" + "1" + "; path=/; expires= 0"; // +expiry.toGMTString();
                } else {
                    $(".due-later").each(function (i) {
                        var id = $(this).ftattr("id");
                        var node = $(".faux-tree-node[data-ft-id=\"" + id + "\"] ");
                        if ($(this).hasClass("nodeExpanded")) {
                            $(this).find(".unitfptitle").click();
                        }
                    });
                    $(".due-later").each(function (i) {
                        $(this).ftattr("visible", "true");
                    });
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "hide_due_later" + "=" + "0" + "; path=/; expires= 0";
                }
                _static.fn.scanFauxTree();
            },

            ToggleCollapseAllUnassigned: function () {
                var settings = _static.fn.GetSettings();
                if ($("#collapse_all").is(":checked")) {
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "collapse_all" + "=" + "1" + "; path=/; expires= 0"; // +expiry.toGMTString();
                    $(_static.sel.unassignedTree + " " + _static.sel.item).each(function (i) {
                        if ($(this).ftattr("state") == "open") {
                            $(_static.sel.pxUnitGrid).fauxtree("toggleNode", $(this));
                        }
                        if ($(this).ftattr("level") == "1") {
                            $(this).ftattr("visible", "false");
                        }
                    });

                } else {
                    var today = new Date();
                    var expiry = new Date(today.getTime() + 30 * 24 * 3600 * 1000);
                    document.cookie = settings.courseNumber + "collapse_all" + "=" + "0" + "; path=/; expires= 0";
                    $(_static.sel.unassignedTree + " " + _static.sel.item).each(function (i) {
                        if ($(this).ftattr("state") == "open") {
                            $(_static.sel.pxUnitGrid).fauxtree("toggleNode", $(this));
                        }
                        if ($(this).ftattr("level") == "1") {
                            $(this).ftattr("visible", "true");
                        }
                    });

                    if ($("#hide_past_due").is(":checked")) {
                        $(".faux-tree-node").each(function (i) {
                            if ($(this).hasClass("past-due")) {
                                $(this).ftattr("visible", "false");
                            }
                        });
                    }
                    if ($("#hide_due_later").is(":checked")) {
                        $(".faux-tree-node").each(function (i) {
                            if ($(this).hasClass("due-later")) {
                                $(this).ftattr("visible", "false");
                            }
                        });
                    }
                }
                _static.fn.scanFauxTree();
            },

            PastDueCheck: function (collapse) {
                var settings = _static.fn.GetSettings();
                var cookieName = settings.courseNumber + "hide_past_due";
                var cookieLength = cookieName.length;
                if (document.cookie.indexOf(cookieName) >= 0) {
                    var index = document.cookie.indexOf(cookieName);
                    var cookieVal = document.cookie.substring(index + cookieLength + 1, index + cookieLength + 2);
                    if (cookieVal != 0) {
                        $("#hide_past_due").prop("checked", true);
                        _static.fn.TogglePastDue();
                    }
                } else if (collapse) {
                    $("#hide_past_due").prop("checked", true);
                }
                _static.fn.UpdateDueCounts();
            },

            DueLaterCheck: function (collapse) {
                var settings = _static.fn.GetSettings();
                var cookieName = settings.courseNumber + "hide_due_later";
                var cookieLength = cookieName.length;
                if (document.cookie.indexOf(cookieName) >= 0) {
                    var index = document.cookie.indexOf(cookieName);
                    var cookieVal = document.cookie.substring(index + cookieLength + 1, index + cookieLength + 2);
                    if (cookieVal != 0) {
                        $("#hide_due_later").prop("checked", true);
                        _static.fn.ToggleDueLater();
                    }
                } else if (collapse) {
                    $("#hide_due_later").prop("checked", true);
                }
                _static.fn.UpdateDueCounts();
            },

            updateDueDayStyles: function (node) {
                var dueDate = node.udattr('due-date') + ' ' + node.udattr('due-time');

                if (dueDate.indexOf('/0001') > -1) {
                    return;
                }

                if (dateFormat.dateConvertToUtc(new Date()) > dateFormat.dateConvertToUtc(new Date(dueDate))) {
                    node.find('.due_date').addClass('grayout');
                }
            },

            updatePastLaterToggles: function (node, children) {
                if (node.hasClass('item-type-pxunit')) {
                    if (node.hasClass('due-later')) {
                        if (children.length > 0 && $(children).not('.is-not-assigned').length == 0) {
                            node.removeClass('due-later');
                        }
                    }

                    if (node.hasClass('past-due')) {
                        if (children.length > 0 && $(children).not('.is-not-assigned').length == 0) {
                            node.removeClass('past-due');
                        }
                    }
                }
            },

            UpdateDueCounts: function () {
                $(".past-due-switch label").attr("data-on", "Hide past due (" + $(".past-due").length + ")");
                $(".past-due-switch label").attr("data-off", "Show past due (" + $(".past-due").length + ")");
                $(".due-later-switch label").attr("data-on", "Hide due later (" + $(".due-later").length + ")");
                $(".due-later-switch label").attr("data-off", "Show due later (" + $(".due-later").length + ")");

                if ($(".due-later").length <= 0) {
                    if (!$(".due-later-switch").hasClass("hide")) {
                        $(".due-later-switch").addClass("hide");
                    }
                } else {
                    $(".due-later-switch").removeClass("hide");
                }
                if ($(".past-due").length <= 0) {
                    if (!$(".past-due-switch").hasClass("hide")) {
                        $(".past-due-switch").addClass("hide");
                    }
                } else {
                    $(".past-due-switch").removeClass("hide");
                }
                if ($(".faux-tree-node[data-ud-is-assigned=true]").length > 0) {
                    $(".launchpad-settings").removeClass("hidden").slideDown("slow");
                } else {
                    $(".launchpad-settings").slideUp("slow", function () {
                        $(".launchpad-settings").addClass("hidden");
                    });
                }
            },

            CollapseAllCheck: function (collapse) {
                var settings = _static.fn.GetSettings();
                var cookieName = settings.courseNumber + "collapse_all";
                var cookieLength = cookieName.length;
                if (document.cookie.indexOf(cookieName) > 0) {
                    var index = document.cookie.indexOf(cookieName);
                    var cookieVal = document.cookie.substring(index + cookieLength + 1, index + cookieLength + 2);
                    if (cookieVal != 0) {
                        $("#collapse_all").prop("checked", true);
                        //_static.fn.ToggleCollapseAllUnassigned();
                    } else {
                        $("#collapse_all").prop("checked", false);
                        //_static.fn.ToggleCollapseAllUnassigned();
                    }
                } else if (collapse) {
                    $("#collapse_all").prop("checked", true);
                }
            },
            // ---- END LaunchPad settings



            //--- Add Content Functionality
            showAddContentMenu: function ($this, event, settings) {
                
                event.stopPropagation();

                // show and position element
                $(_static.sel.addContentMenu)
                    .show()
                    .position({
                        my: "left top",
                        at: "left bottom",
                        of: $($this),
                        collision: "none"
                    });

                // default options
                var args = {
                    enableUnit: false,
                    enableFolder: false,
                    parentNode: null,
                    parentId: null
                };

                if ($(this).closest(".faceplate-nav").length > 0) { //create content called from faceplate main navigation (dont allow folders, create at root level)
                    args.enableUnit = true;
                    args.enableFolder = false;
                    args.parentNode = null;
                    if (settings != null) {
                        args.parentId = settings.subContainerId; //eg: PX_MULTIPART_LESSONS
                    }
                } else { //create content called inside faceplate unit (dont allow units)
                    args.enableUnit = false;
                    args.enableFolder = true;
                    args.parentNode = $($this).closest(_static.sel.item);
                    args.parentId = args.parentNode.udattr("id");
                }

                // default parent id
                if (args.parentId == null || args.parentId.length == 0) {
                    args.parentId = "PX_MULTIPART_LESSONS";
                }

                // click handler for `Create new` link
                $(_static.sel.addContentMenuLink).unbind('click').bind('click', function () {
                    PxContentTemplates.CreateItems(args.enableUnit, args.enableFolder, args.parentId, settings.toc);
                    $(_static.sel.addContentMenu).hide();
                    return false;
                });

                return false;
            },
            // when something adds existing content to the LaunchPad we need to track the
            // new item
            onAddExistingContent: function (id, parentId, operation, callback, toc) {
                if (!id) {
                    return;
                }

                var startDate = '';
                var endDate = '';
                var startTime = '';
                var endTime = '';
                var points = '';

                if (typeof id == "object") {
                    var start = new Date(parseInt(id.startDate.substr(6)));
                    var end = new Date(parseInt(id.endDate.substr(6)));

                    if (start.getFullYear() == 1) {
                        startDate = '01/01/0001';
                    }
                    else {
                        startDate = $.datepicker.formatDate('mm/dd/yy', start);
                        startTime = dateFormat(start, 'HH:MM:ss TT');
                        startDate += " " + startTime;
                    }

                    if (end.getFullYear() == 1) {
                        endDate = '01/01/0001';
                    }
                    else {
                        endDate = $.datepicker.formatDate('mm/dd/yy', end);
                        endTime = dateFormat(end, 'HH:MM:ss TT');
                        endDate += " " + endTime;
                    }

                    points = id.points;

                    id = id.id;
                }
                else if (typeof id == "string") {
                    var node = $(_static.sel.getItem(id));

                    startDate = node.udattr("start-date");
                    startTime = node.udattr("start-time");
                    endDate = node.udattr("due-date");
                    endTime = node.udattr("due-time");
                }

                if (operation == null) {
                    operation = _static.operation.newItem;
                }

                var stubData = {
                    type: 'existing',
                    parentId: parentId
                };

                var item = {
                    id: id,
                    title: 'Existing Resource Link'
                };

                var sourceContainerId = sourceContainerId = (node) ? node.attr('data-ft-chapter') : "";

                //remove item if it exists in the tree
                $(_static.sel.getItem(id)).remove();

                var newNode = _static.fn.injectStubItem(stubData, item);
                newNode.attr('data-ft-chapter', sourceContainerId);

                parentId = newNode.ftattr("parent");

                if (startDate != '' && startDate != '01/01/1') {
                    newNode.udattr("start-date", startDate);
                    newNode.udattr("start-time", startTime);
                }
                if (endDate != '' && endDate != '01/01/1') {
                    newNode.udattr("due-date", endDate);
                    newNode.udattr("due-time", endTime);
                }
                if (points != '') {
                    newNode.udattr("max-points", points);
                }

                var data = {
                    category: $(newNode).parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: $(newNode),
                    above: null, //get the parent node of the item
                    below: newNode.nextAll(_static.sel.childOf(parentId)).first(), //get the next child node of the item
                    operation: operation,
                    onSuccess: callback,
                    loadView: true,
                    toc: toc
                };
                _static.fn.saveNavigationState(data);

                return;
            },
            onSaveAndOpen: function (id, parentId, operation) {
                _static.fn.onAddExistingContent(id, parentId, operation, function () {
                    var node = $(_static.sel.getItem(id));
                    _static.fn.forceClickNode(node);
                });
            },
            onRemoveItem: function (item) {
                var nodeItem = $(_static.sel.getItem(item.contentItemId));
                if (nodeItem == null || nodeItem.length == 0) {
                    return;
                }

                nodeItem.addClass("fade-effect");
                PxPage.Fade();

                var data = {
                    category: $(nodeItem).parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: nodeItem,
                    above: null, //get the parent node of the item
                    below: null,
                    operation: _static.operation.remove,
                    removeFrom: item.removeFrom,
                    loadView: false,
                    onSuccess: function() {
                        $(PxPage.switchboard).trigger("faceplateUpdateResourceChapterId");

                        var setting = _static.fn.GetSettings();
                        if (setting) {
                            if ((setting.toc && setting.toc.length
                                             && this.removeFrom
                                             && this.removeFrom.indexOf(setting.toc) > -1)
                                || (!this.removeFrom))
                                $.fn.fauxtree("removeNode", nodeItem);
                        }
                    }
                };

                _static.fn.saveNavigationState(data);
            },

            updateItemAfterRenameAndOpen: function (response) {
                $.fn.ContentTreeWidget('updateItemAfterRename', response, true);
            },

            updateItemAfterRename: function (response, isOpen) {

                PxPage.log('content : updating item after rename');

                var returnValues = response.split("|");
                var id = returnValues[0];
                var title = returnValues[1];
                var description = returnValues[2];
                var thumbnail = returnValues[3];
                var subtitle = returnValues[4];
                var sourceWindow = returnValues[5];
                var node = $.fn.fauxtree("getNode", id);

                if (node != null) {
                    node.find('.fpimage').attr('src', thumbnail);
                    node.find('.fpimageLarge').attr('src', thumbnail);
                    node.find('.fpimageLarge').attr('delayedsrc', thumbnail);
                    node.find(".fne-link").html(title);
                    node.find(".item_subtitle").html(subtitle);
                    node.find(".description>:first-child").html(description);

                    if (node.find(".unitfptitle").length == 0) {
                        if (!!node.find('.fptitle a').length)
                            node.find('.fptitle a').html(title);
                        else
                            node.find(".fptitle").html(title);
                    }
                    else {
                        node.find(".unitfptitle").html(title);
                    }

                    if ($("#fne-window").is(":visible")) {
                        $.fn.ContentTreeWidget('hideManagementCard');
                        PxPage.Toasts.Success("Item was saved");
                        $('#fne-window').removeClass('require-confirm');
                        PxPage.Loaded("#content-item");
                        return false;
                    }

                    if (description.length) {
                        node.find(".description>:first-child").show();
                    }

                    if (isOpen == true) {
                        node.click();
                    }
                }

                $(".create-closecancel").click();

                if ($("#fne-unblock-action-home").is(":visible")) {
                    $("#fne-unblock-action-home").click();
                    $(".faux-tree-node.active .fne-link").click();
                }

                if (PxPage.Context.IsSandboxCourse) {
                    $(PxPage.switchboard).trigger("refereshsandboxalert");
                }

            },

            injectStubItem: function (data, item) {
                var insertBefore;
                var tree;

                if (data.parentId == null || data.parentId == 'PX_TEMP' || data.parentId == "undefined") { //if no parent, insert on top 
                    data.parentId = "PX_MULTIPART_LESSONS";
                }
                if (data.parentId == "PX_MULTIPART_LESSONS") {
                    insertBefore = $($(_static.sel.getUnassignedItems())[0]);
                } else {
                    //find the first node thats a child of the parent
                    insertBefore = $($(_static.sel.childOf(data.parentId))[0]);
                }

                if (insertBefore.length == 0) {
                    var parent = $(_static.sel.getItem(data.parentId));
                    if (parent.length) {
                        insertBefore = parent.next();
                    }
                }
                    
                tree = insertBefore.parents(_static.sel.tree);
                if (tree.length == 0) {
                    var treeUnassigned = $(_static.sel.tree + '.faceplate-unit-subitems-unassigned');
                    tree = (treeUnassigned.length)
                            ? treeUnassigned
                            : $(_static.sel.tree + '.faceplate-unit-subitems');
                }

                var nodeState = {
                    title: data.type == 'existing' ? "Adding existing content item" : "New " + data.type,
                    id: item.id,
                    state: "barren",
                    visible: true,
                    insertBefore: insertBefore,
                    parentId: data.parentId,
                    level: data.parentId ? 1 : null
                };

                //TODO: get root parent id (PX_MULTIPART_LESSONS) when added from nav, append to top
                //add item to faux-tree
                var newNode = $(tree).fauxtree("addNode", nodeState, null, tree);
                $.fn.fauxtree("showTree", newNode);
                newNode.addClass(_static.css.newNode);

                newNode.addClass("fade-effect");
                PxPage.Fade();

                return newNode;
            },


            //--- END Add Content Functionality



            //--- MISC settings

            onSetVisibility: function (args) {
                var node = $(_static.sel.getItem(args.id));

                var data = {
                    category: $(node).parents(_static.sel.category),
                    tree: $(_static.sel.tree),
                    changed: node,
                    above: null,
                    below: null,
                    operation: _static.operation.showOrHide,
                    onSuccess: function (response) {
                        //get all the nodes of the category and set the
                        // hidden-from-students accordingly.

                        $(_static.sel.item).each(function () {
                            var current = $(this);
                            var isvisibletostudents = current.udattr('isvisibletostudents');

                            if (isvisibletostudents != null) {
                                current.removeClass('hidden-from-students');

                                if (isvisibletostudents.toLowerCase() == "true") {
                                    //remove class;
                                } else {
                                    current.addClass('hidden-from-students');
                                }
                            }
                        });

                        // update parents if the child became visible
                        if (node.udattr('isvisibletostudents').toLowerCase() == 'true') {
                            $.each(response.changes, function () {
                                if (this.id != response.changed.id) {
                                    $(_static.sel.getItem(this.id)).removeClass('hidden-from-students');
                                }
                            });
                        }
                    },
                    loadView: false
                };
                _static.fn.saveNavigationState(data);
            },
            ///Arguments:itemid
            ///itemid: item to load
            refreshItem: function (itemid) {
                var node = $(_static.sel.getItem(itemid));
                if (node != null && node.length > 0) {
                    var data = {
                        itemId: itemid,
                        widgetId: node.closest(".widgetItem").attr("id")
                    };
                    if (node.attr("hide-student-data") == "True" || ($("#PX_MENU_ITEM_EBOOK").hasClass("active") && $(node).hasClass("student"))) {
                        return false;
                    }
                    _static.fn.Loading("refreshItem", true, itemid);
                    $.ajax({
                        url: PxPage.Routes.load_faceplate_item,
                        type: "POST",
                        data: JSON.stringify(data),
                        dataType: "html",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            _static.fn.saveNavigationStateParseViewResponse(response);

                            _static.fn.sortFauxTree();
                            _static.fn.scanFauxTree();
                            _static.fn.Loaded("refreshItem");

                            PxPage.Fade();
                        }

                    });
                }
            },
            ///Arguments:itemid
            ///itemid: item to refresh status of
            refreshItemStatus: function (itemId) {
                PxPage.Loading("fne-action-status", true);
                var data = {
                    itemId: itemId
                };

                $.ajax({
                    url: PxPage.Routes.update_faceplate_item_status,
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: "html",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        $(".fne-action-status").html(response);
                        PxPage.Loaded("fne-action-status", true);
                        $(".fne-action-status").addClass("fade-effect");
                        PxPage.Fade();
                    }

                });
            },

            ///Arguments:
            ///id: item id to update
            ///hiddenFromStudent: toggle indicating item is hiddent from student
            ///restrictedbydate: toggle restricted by date
            updateItemSettings: function (args) {
                var node = $(_static.sel.getItem(args.id));
                node.removeClass("hidden-from-students");
                if (args.hiddenFromStudent == true) {

                    //hide/show Node
                    node.udattr('isvisibletostudents', false);
                } else {
                    node.udattr('isvisibletostudents', true);
                }
                if (args.restrictedbydate == undefined) {
                    args.restrictedbydate = false;
                }

                _static.fn.onSetVisibility(args);

            },
            //--- END MISC settings

            //--- Utility Functions

            closedChaptersFade: function () {
                //gray out all root-level items
                $(".unitrowlevel1[data-ft-level='1']").each(function () {
                    $(this).addClass(_static.sel.chapterUnselected);
                });
                //show open chapters
                $(".unitrowlevel1[data-ft-state=\"open\"]").each(function () {
                    $(this).removeClass(_static.sel.chapterUnselected);
                });
            },
            closedChaptersShow: function () {
                //show all root level items
                $(".unitrowlevel1").each(function () {
                    $(this).removeClass(_static.sel.chapterUnselected);
                });
            },
            sortFauxTree: function (recursive) {
                if (recursive == undefined) {
                    recursive = false;
                }

                $(_static.sel.assignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, recursive);
                $(_static.sel.unassignedTree).fauxtree("sortTree", _static.fn.compareNodesOnDateSequence, recursive);
            },
            scanFauxTree: function () {
                var settings = _static.fn.GetSettings();
                $(_static.sel.assignedTree).fauxtree("scanTree", $(_static.sel.assignedTree));
                if (settings.splitAssignedUnassigned) {
                    $(_static.sel.unassignedTree).fauxtree("scanTree", $(_static.sel.unassignedTree));
                }
            },
            compareNodesOnDateSequence: function (a, b) {
                settings = _static.fn.GetSettings();

                var dateModeA = $(a).udattr("date-mode");
                var dateModeB = $(b).udattr("date-mode");
                var datea, dateb;
                if (dateModeA == "range") {
                    datea = new Date($(a).udattr("start-date") + " " + $(a).udattr("start-time"));
                } else {
                    datea = new Date($(a).udattr("due-date") + " " + $(a).udattr("due-time"));
                }
                if (dateModeB == "range") {
                    dateb = new Date($(b).udattr("start-date") + " " + $(b).udattr("start-time"));
                } else {
                    dateb = new Date($(b).udattr("due-date") + " " + $(b).udattr("due-time"));
                }

                //only sort chapter-nodes (level-1 nodes) by date, and only in launchpad mode,
                //sort everything else by sequence
                if (datea.getTime() == dateb.getTime() || $(a).ftattr("level") > 1 || !settings.sortByDueDate) {
                    //sort by sequence
                    var seqa = $(a).ftattr("sequence");
                    var seqb = $(b).ftattr("sequence");
                    if (seqa == seqb) {
                        //if sequences are equal, sort by title
                        var titlea = $.trim($(a).find(_static.sel.itemTitle).text() || $(a).find(_static.sel.unitTitle).text());
                        var titleb = $.trim($(b).find(_static.sel.itemTitle).text() || $(b).find(_static.sel.unitTitle).text());
                        return titlea.toLowerCase() > titleb.toLowerCase() ? 1 : -1;
                    }
                    if (seqa == undefined || seqb == undefined) {
                        return (seqb == undefined) ? -1 : 1;
                    }
                    if (seqa.length > 0 && seqb.length > 0) { //both nodes have a sequence
                        return seqa > seqb ? 1 : -1;
                    } else { //if one of the the nodes has no sequence, the node with a sequence should come first
                        return seqa.length > seqb.length ? -1 : 1;
                    }
                } else if (datea.getTime() == new Date("01/01/0001").getTime()) {
                    return 1;
                } else if (dateb.getTime() == new Date("01/01/0001").getTime()) {
                    return -1;
                } else if (datea < dateb) {
                    return -1;
                } else if (datea > dateb) {
                    return 1;
                } else {

                }
            },
            Loading: function (key, disableUI, itemId) {
                // removing a loading block if exists so there won't be a conflict
                $("#loadingBlock").remove();

                //create click overlady
                if (disableUI) {
                    if ($(".faceplate-overlay").length == 0) {

                        $('<div class="faceplate-overlay"> </div>').appendTo(document.body);
                    }
                    var overlay = $(".faceplate-overlay");
                    $(overlay).addClass(key);
                }

                if (itemId != null) {
                    var item = $(_static.sel.getItem(itemId));
                    PxPage.itemLoading = item;
                    PxPage.Loading(".faceplate_launchpad", null, null, null, {
                        css:
                        {
                            marginTop: $(PxPage.itemLoading).position().top - 55 + "px"
                        }
                    });

                    var animate = function () {
                        if (PxPage.itemLoading != null) {
                            $("#loadingBlock").parent().animate({ marginTop: $(PxPage.itemLoading).position().top - 55 + "px" }, "slow");
                            $(PxPage.itemLoading).effect("highlight", { color: "rgb(245, 255, 159)" }, 1000, animate);
                        }
                    };
                    animate();

                } else {
                    //position loading screen in center of screen for the user
                    PxPage.Loading(".faceplate_launchpad", null, null, null, {
                        css:
                        {
                            marginTop: $(window).height() / 2 + $(window).scrollTop() - 325 + "px"
                        }
                    });
                }
            },
            Loaded: function (key) {
                if ($(".faceplate-overlay").hasClass(key)) {
                    $(".faceplate-overlay").removeClass(key);
                }
                if ($(".faceplate-overlay").length == 0 || $(".faceplate-overlay").attr('class').split(/\s+/).length == 1) {
                    PxPage.Loaded(".faceplate_launchpad");
                    PxPage.Loaded();

                    if (PxPage.itemLoading) {
                        PxPage.itemLoading = null;
                        $("#loadingBlock").remove();
                    }
                    $(".faceplate-overlay").remove();
                }
            },
            TitleVisibility: function () {
                if ($(".faceplate-unit-subitems .faux-tree-node").length > 0) {
                    $(".launchpad-settings .launchpad-title").show();
                } else {
                    $(".launchpad-settings .launchpad-title").hide();
                }
            },
            GetSettings: function () {
                var element = contentDataElem.slice(-1)[0];

                return $(element).data(_static.dataKey).settings;

            },
            GetActiveChapterId: function () {
                return $(_static.sel.activeChapterId).val();
            },
            GetActiveChapterName: function () {
                return $(_static.sel.activeChapterName).val();
            },
            GetParentFolderTitle: function (node) {
                var appName = PxPage.Toasts.AppName();
                var foldertitle = (appName) ? appName : "Launchpad";
                if (node) {
                    var parentId = $(node).ftattr('parent');
                    var parentNode = $(_static.sel.getItem(parentId));
                    if (parentNode.length > 0) {
                        if ($.trim(parentNode.find('.fptitle').text()).length == 0) {
                            foldertitle = $.trim(parentNode.find('.unitfptitle').text());
                        } else {
                            foldertitle = $.trim(parentNode.find('.fptitle').text());
                        }
                    }
                }
                return foldertitle;
            },
            //--- END utility functions

            //---BEGIN route listeners
            InitRoutes: function () {
                $(PxRoutes.GetProductRoutes()).each(function () {
                    var route = $(this).get(0);
                    route.matched.add(function (launchpadState, component) {
                        var chapter = launchpadState.split("-")[1];
                        if (chapter != null && chapter.length > 0 && chapter != $(PxPage.switchboard).triggerHandler("getActiveChapterId")) {
                            $(PxPage.switchboard).trigger("opencontent", chapter);
                        }
                    });
                });
            }
            //---END Route listeners
        }


    };
    //The public interface for interacting with this plugin
    var api = {
        init: function (options) {
            $('#PX_MENU_ITEM_LAUNCHPAD').bind('click', function () {
                $('.moreResourcesAction').css('visibility', 'visible');
            });

            $('#PX_MENU_ITEM_EBOOK').bind('click', function () {
                $('.moreResourcesAction').css('visibility', 'hidden');
            });

            $(document).off('mouseenter', '.faux-tree-node .dd_cal').on('mouseenter', '.faux-tree-node .dd_cal', function () {
                var qTipTime = new Date('01/01/0001 ' + $(this).parents('.faux-tree-node').udattr('due-time')).format('hh:MMtt') + " " + $('#TimeZoneAbbreviation').val();

                $(this).qtip({
                    content: {
                        text: qTipTime
                    },
                    show: {
                        ready: true, solo: true
                    }
                });
            });

            return this.each(function () {
                var settings = $.extend(true, {}, _static.defaults, options),
                    $this = $(this),
                    data = $this.data(_static.dataKey),
                    treeAssigned = $this.find("ul.faceplate-unit-subitems"),
                    treeUnassigned = $this.find("ul.faceplate-unit-subitems-unassigned");

                if (!data) {
                    $this.data(_static.dataKey, {
                        target: $this,
                        settings: settings
                    });
                    data = $this.data(_static.dataKey);
                    contentDataElem.push(this);
                }

                $(PxPage.switchboard).unbind(".launchpad");

                //register events for the widget
                if (settings.splitAssignedUnassigned) {
                    _static.fn.initTrees.apply($this, [treeAssigned, treeUnassigned]);
                } else {
                    _static.fn.initTrees.apply($this, [treeAssigned]);
                }
                _static.fn.initStudentUI.apply($this);

                if (settings.readOnly) {
                    settings.showManagementCard = false;
                }

                if (settings.showManagementCard) {
                    $(document).off('click', _static.sel.itemAssignButton).on("click", _static.sel.itemAssignButton, function (event) {
                        if ($(this).hasClass("sandbox-inactive")) {
                            return;
                        }
                        event.preventDefault();
                        _static.fn.showManagementCard(this, 'open');
                    });
                    $(document).off('click', _static.sel.itemCalendar).on('click', _static.sel.itemCalendar, function (event) {
                        event.preventDefault();
                        _static.fn.showManagementCard(this, 'open');
                    });
                    $(document).off('click', _static.sel.itemGearbox).on('click', _static.sel.itemGearbox, function (event) {
                        event.preventDefault();
                        _static.fn.showManagementCard(this, 'open');
                    });
                }

                if (settings.enableAddContent) {
                    $(document).off('click.launchpad', _static.sel.addContentButton).on('click.launchpad', _static.sel.addContentButton, function (event) {
                        _static.fn.showAddContentMenu(this, event, settings);
                    });
                }

                //Handle the click of the "Create Assignment" button for assignment unit workflow
                $(_static.sel.addUnitButton).click(function (event) {
                    var button = $(_static.sel.addUnitButton);
                    var templateId = button.find(".assignmentUnitTemplateId").val();
                    var toc = button.find(".assignmentUnitToc").val();
                    ContentWidget.CreateAssignmentUnit(templateId, toc);
                });


                $('html').click(function () {
                    $(_static.sel.addContentMenu).hide();
                });

                $(PxPage.switchboard).bind("removenewnode.launchpad", function (event, close) {
                    if (close && close.reason === "cancel") {
                        $.fn.fauxtree("removeNode", close.id);
                    }
                    $(_static.sel.newNode).each(function () {
                        $.fn.fauxtree("removeNode", $(this));
                    });
                });

                //disable sorting on read-only items
                $(document).off('mouseenter.launchpad', '[data-ud-read-only="true"]').on("mouseenter.launchpad", '[data-ud-read-only="true"]', function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('disable');
                });

                $(document).off('mouseleave.launchpad', '[data-ud-read-only="true"]').on("mouseleave.launchpad", '[data-ud-read-only="true"]', function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('enable');
                });

                //add existing node to tree
                $(PxPage.switchboard).bind("addexistingcontent.launchpad", function (event, sourceId, targetId) {
                    _static.fn.onAddExistingContent(sourceId, targetId, _static.operation.newItem);
                });
                //save new content Trigger
                $(PxPage.switchboard).bind("onSaveContent.launchpad", function (event, sourceId, targetId) {
                    _static.fn.onAddExistingContent(sourceId, targetId, _static.operation.newItem);
                });

                $(PxPage.switchboard).bind("onSaveAndOpen.launchpad", function (event, itemid, existing, parent) {
                    _static.fn.onSaveAndOpen(itemid, parent, _static.operation.newItem);
                });

                $(PxPage.switchboard).bind("movenode.launchpad", function (event, sourceId, targetId, sourceRootId, targetRootId, callback) {
                    _static.fn.onAddExistingContent(sourceId, targetId, _static.operation.move, callback);
                });

                $(PxPage.switchboard).bind("contentcreated.launchpad", function (event, item, existing, parent, callback) {
                    if ($(_static.sel.tree).length > 0) {
                        var itemid = null;
                        if (item && item.id) {
                            itemid = item.id;
                        }

                        if (item.id && $(_static.sel.getItem(item.id)).length > 0) {
                            //if this item already exists, we're calling create content from an edit screen -- simply refresh the item
                            _static.fn.refreshItem(itemid);
                        } else {
                            //First make sure the content is being added to this tree
                            if (!item.toc || item.toc === settings.toc) {
                                //being added to this tree but does not exist, we're creating it.
                                _static.fn.onAddExistingContent(itemid, parent, _static.operation.newItem, callback, item.toc);
                            }
                        }
                    }
                });

                $(PxPage.switchboard).bind("contentcopied.launchpad", function (event, itemid, existing, parent, callback) {
                    _static.fn.onAddExistingContent(itemid, parent, _static.operation.newItem, callback);
                });

                if (!settings.triggerOpenContentOnClick) {
                    //bind open content (there should be only 1 opencontent binding per page)
                    var openContentFunc = function (event, itemid) {
                        var node = $this.find(_static.sel.getItem(itemid));
                        if (node.length > 0) {
                            _static.fn.openContentSubfolders(node.closest(_static.sel.tree), itemid);
                            _static.fn.forceClickNode(node);
                        }
                    };
                    $(PxPage.switchboard).unbind("opencontent").bind("opencontent", openContentFunc);
                }

                $(PxPage.switchboard).unbind("openparents").bind("openparents", function (event, itemid) {
                    var node = $this.find(_static.sel.getItem(itemid));
                    _static.fn.openContentSubfolders(node.closest(_static.sel.tree), itemid);
                });

                $(PxPage.switchboard).bind("onremoveitem.launchpad", function (event, item) {
                    _static.fn.onRemoveItem(item);
                });

                $(PxPage.switchboard).bind("contentassigned.launchpad", function (event, args) {
                    if (settings.showAssignmentUnitFlow) {
                        args.operation = _static.operation.moveAndAssign;
                        args.unitValue = "Launchpad";
                    }
                    _static.fn.onContentAssigned(args);
                });

                $(PxPage.switchboard).bind("contentunassigned.launchpad", function (event, args) {
                    if (settings.removeOnUnassign) {
                        args.operation = _static.operation.removeOnUnassign;
                    }

                    args.toc = settings.assignmentToc.length ? settings.assignmentToc : settings.toc;
                    _static.fn.onContentUnAssigned(args);
                });

                //disable dragging of browse more resources link
                $(document).off("mouseenter.launchpad", _static.sel.browseMoreResourcesLink).on("mouseenter.launchpad", _static.sel.browseMoreResourcesLink, function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('disable');
                });

                $(document).off("mouseleave.launchpad", _static.sel.browseMoreResourcesLink).on("mouseleave.launchpad", _static.sel.browseMoreResourcesLink, function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('enable');
                });

                //disable dragging of management card
                $(document).off("mouseenter.launchpad", _static.sel.itemManagementCard).on("mouseenter.launchpad", _static.sel.itemManagementCard, function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('disable');
                });

                $(document).off("mouseleave.launchpad", _static.sel.itemManagementCard).on("mouseleave.launchpad", _static.sel.itemManagementCard, function () {
                    $(_static.sel.tree).filter('.ui-sortable').sortable('enable');
                });

                $(PxPage.switchboard).unbind('fneclickNextNodeTitle').bind("fneclickNextNodeTitle.launchpad", _static.fn.triggerNextNode);
                $(PxPage.switchboard).unbind("fneclickPreviousNodeTitle").bind("fneclickPreviousNodeTitle.launchpad", _static.fn.triggerPrevNode);

                //toggle student visibility
                $(PxPage.switchboard).bind("settingsUpdate.launchpad", function (event, args) {
                    //_static.fn.updateItemSettings(args);
                    _static.fn.refreshItem(args.id);
                });

                //refresh FNE item status
                $(PxPage.switchboard).bind("argacomplete", function (event, args) {
                    _static.fn.refreshItemStatus(args.itemid);
                });

                $(document).off('click', ".managementcard_students_show").on('click', ".managementcard_students_show", _static.fn.ToggleVisibility);
                $(document).off('click', ".managementcard_students_hide").on("click", ".managementcard_students_hide", _static.fn.ToggleVisibility);

                $(document).off('click', "#collapse_all").on("click", "#collapse_all", _static.fn.ToggleCollapseAllUnassigned);
                _static.fn.CollapseAllCheck(settings.collapseUnassigned);

                $(document).off('click', "#hide_past_due").on("click", "#hide_past_due", _static.fn.TogglePastDue);
                _static.fn.PastDueCheck(settings.collapsePastDueByDefault);

                $(document).off('click', "#hide_due_later").on("click", "#hide_due_later", _static.fn.ToggleDueLater);
                _static.fn.DueLaterCheck(settings.collapseDueLaterByDefault);

                if (settings.grayOutPastDueLater) {
                    $(".unitrowlevel1.past-due .due_date").addClass("grayout");
                    //$(".unitrowlevel1.due-later").addClass("grayout");
                }

                //TODO: move this to management card js
                $(document).off('click', _static.sel.txtGradePoints).on('click', _static.sel.txtGradePoints, function () {
                    $(this).select();
                });

                //Gets the currently active chapter, to be used with $.triggerHandler()
                $(PxPage.switchboard).bind("getActiveChapterId.launchpad", _static.fn.GetActiveChapterId);
                $(PxPage.switchboard).bind("getActiveChapterName.launchpad", _static.fn.GetActiveChapterName);

                $(PxPage.switchboard).bind("refreshitem.launchpad", function (event, args) {
                    _static.fn.refreshItem(args.itemId);
                });

                _static.fn.scanFauxTree();
                _static.fn.markLastViewed($(_static.sel.tree), "");

                _static.fn.InitRoutes();

            });
        },
        destroy: function () {
            contentDataElem.pop();
        },
        showAssignCalendar: _static.fn.showAssignCalendar,
        hideManagementCard: _static.fn.hideManagementCard,
        updateItemAfterRenameAndOpen: _static.fn.updateItemAfterRenameAndOpen,
        updateItemAfterRename: _static.fn.updateItemAfterRename
    };

    //Handle the custome attributes
    $.fn.udattr = function (name, value) {
        var target = this.first(),
            aName = _static.dataAttrPrefix + name;
        if (value != null)
            target.attr(aName, value);
        return target.attr(aName);
    };

    $.fn.ContentTreeWidget = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
        return null;
    };

    $.fn.ContentTreeWidgetObj = function () {
        return {
            _static: _static,
            api: api
        };
    };
}(jQuery));