// jQuery plugin that turns a flat ul into a tree like structure.
//
// I begged and pleaded not to have to write this code, but c'est la vie
(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "fauxtree",
        dataKey: "fauxtree",
        bindSuffix: ".fauxtree",
        dataAttrPrefix: "data-ft-",
        minLevel: 1,
        // default setting values
        defaults: {
            indent: 12,
            sortableThreshold: 12,
            sortableUseMouse: false,
            sortableInsideContainer: false,
            sortableDisableReadOnlyDrag: false,
            sortableDisableIndentation: false, //disables using left-position of mouse to determine location of item
            sortableNodeExpandDelay: 1000, //delay in ms before expanding a node on hover
            sortableEnableWindowScrollOnDrag: true,
            sortableItemsUndraggableSel: "",
            sortableHelperHtml: function (ui) {
                var elem = $("<div class='ui-fauxtree-helper'><span><div id='dragIcon' class='" + _static.css.iconSet + " " + _static.css.dropInto + " '></div>" + $(ui.srcElement).text() + "</span></div>");
                return elem;
            },
            allowRootLevelUnitNesting: true, //allow root-level units to reside within other units
            sortableRootLevelName: "",
            showDragLabel: false,
            useCustomText: false,
            dragLabelCustomText: "",
            showDragIcon: false,
            showExpandIcon: true,
            closeAllSiblingsOnOpen: false,
            animateOpenNode: true,
            nodeTitleCustomSel: "",
            nodeTitleCustomHtml: "",
            css: {
                evenClass: "even",
                oddClass: "odd"
            },
            debug: false,
            debugDragOutput: ".debug-drag-output",
            iconWidth: 18,
            readOnly: false,
            showall: false,
            showLastViewed: false,
            filterByTreeId: '',
            filterNodesByTree: false,
            defaultRootParent: "PX_MULTIPART_LESSONS"
        },
        states: {
            none: "none",
            barren: "barren",
            open: "open",
            closed: "closed"
        },
        dropTypes: {
            sibling: "sibling",
            siblingBelow: "siblingBelow",
            child: "child"
        },
        css: {
            placeholder: "faux-tree-placeholder",
            activeNode: "active",
            hoverNode: "hovered",
            icon: "icon",
            collapsed: "collapsed",
            expanded: "expanded",
            node: "faux-tree-node",
            nodeTitle: "faux-tree-node-title",
            fade: "fade-effect",
            dropInto: " pxicon-plus-circle ",
            dropInvalid: " pxicon-x-circle ",
            dropBetween: " pxicon-list ",
            iconSet: " pxicon "
        },
        // selectors for commonly accessed elements
        sel: {
            node: ".faux-tree-node",
            nodeById: function (id) {
                var selector = _static.sel.node + "[" + _static.dataAttrPrefix + "id=\"" + id + "\"]";

                if (_static.defaults.filterNodesByTree == true && _static.defaults.filterByTreeId != "") {
                    selector = "#" + _static.defaults.filterByTreeId + ' ' + selector;
                }

                return selector;
            },
            nodeByTreeAndId: function (destinationTree, id) {
                _static.defaults.filterByTreeId = destinationTree.attr("id");

                var selector = _static.sel.nodeById(id);

                settings.filterByTreeId = settings.filterByTreeId;

                return selector;
            },
            childOf: function (id) {
                var selector = _static.sel.node + "[" + _static.dataAttrPrefix + "parent=\"" + id + "\"]";

                if (_static.defaults.filterNodesByTree == true && _static.defaults.filterByTreeId != "") {
                    selector = "#" + _static.defaults.filterByTreeId + ' ' + selector;
                }

                return selector;
            },
            childOfNode: function (node) {
                var tree = _static.fn.getTree(node);
                var parentId = node.ftattr("id");

                //disable filterding nodes by tree ID
                var filterNodesByTree = _static.defaults.filterNodesByTree;
                _static.defaults.filterNodesByTree = false;
                var nodes = tree.find(_static.sel.childOf(parentId));
                //reset filternodesbytreeid
                _static.defaults.filterNodesByTree = filterNodesByTree;

                return nodes;
            },
            evenNodes: ".faux-tree-node:visible:even",
            oddNodes: ".faux-tree-node:visible:odd",
            firstLevelEvenNodes: ".faux-tree-node[data-ft-level=\"1\"]:even",
            firstLevelOddNodes: ".faux-tree-node[data-ft-level=\"1\"]:odd",
            visibleNotNode: function (id) {
                return ':visible:not([' + _static.dataAttrPrefix + 'id="' + id + '"])';
            },
            rootNodes: '.faux-tree-node[data-ft-level=1]',
            nodeTitle: ".faux-tree-node-title",
            placeholder: ".faux-tree-placeholder",
            tree: ".faux-tree",
            activeNode: ".faux-tree-node.active",
            nodeIcon: ".icon",
            nodeLink: ".faux-tree-node .faux-tree-link"
        },
        // set of private functions
        fn: {
            // returns the fauxtree instance that the given node is part of.
            //
            // node - node we are getting the tree for
            getTree: function (node) {
                var instance;

                if (typeof node === "string") {
                    instance = $(_static.sel.nodeById(node)).parents("ul");
                }
                else {
                    instance = $(node).parents("ul");
                }

                return instance;
            },
            // returns the settings of the instance the node is part of.
            //
            // node - node whose tree we are getting the settings object for
            getSettings: function (node) {
                return _static.fn.getTree(node).data(_static.dataKey).settings;
            },
            // creates an object representation of the given node.
            // this function is not recursive and therefore the children property is
            // always empty.
            //
            // node - the element representing the node being serialized.
            // callback - if given, function will be called to allow for custom data to be serialized: function(nodeElement, nodeObject)
            getNodeData: function (node, callback) {
                var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
                if (!target) {
                    throw new Error("Node not found");
                }
                var nodeData = {
                        level: target.ftattr("level"),
                        parentId: target.ftattr("parent"),
                        id: target.ftattr("id"),
                        sequence: target.ftattr("sequence"),
                        state: target.ftattr("state"),
                        visible: target.ftattr("visible"),
                        isvisibletostudents: target.ftattr("isvisibletostudents"),
                        //title: target.find(_static.sel.nodeTitle).html(),
                        children: []
                    };

                if (callback) {
                    callback(target, nodeData);
                }

                return nodeData;
            },
            // sets the data from a node back into the DOM
            setNodeData: function (nodeState, callback, node) {
                var target = node ? node : $(_static.sel.nodeById(nodeState.id));
                var settings = _static.fn.getSettings(target);

                target.addClass(_static.css.node);
                target.ftattr("level", nodeState.level);
                target.ftattr("parent", nodeState.parentId ? nodeState.parentId : "");
                target.ftattr("id", nodeState.id);
                target.ftattr("sequence", nodeState.sequence);
                target.ftattr("state", nodeState.state);
                target.ftattr("visible", nodeState.visible);
                target.ftattr("isvisibletostudents", nodeState.isvisibletostudents);

                if (nodeState.title) {
                    if (target.find(_static.sel.nodeTitle + settings.nodeTitleCustomSel).length == 0) {
                        if (settings.nodeTitleCustomHtml.length > 0) {
                            target.append(settings.nodeTitleCustomHtml);
                        } else {
                            target.append('<span class="' + _static.css.nodeTitle + '"></span>');
                        }
                    }

                    target.find(_static.sel.nodeTitle + settings.nodeTitleCustomSel).html(nodeState.title);
                }

                if (!_static.defaults.readOnly) {
                    if (target.find(".assignment-dates").length == 0) {
                        target.append('<a class="assignment-dates" href="#"><span class="assignment-no-dates" title="Set due date"></span></a>');
                    }
                }

                if (callback) {
                    callback(target, nodeState);
                }
            },
            // creates a new node which isn't connected to the DOM and returns it.
            createNode: function (state, showExpandIcon) {
                var node = $('<li><span class="' + _static.css.icon + '"' + (!showExpandIcon ? ' style="display:none" ' : "") + '></span></li>');

                return node;
            },
            // dumps node data into a json string and returns it
            dumpNode: function () {
                var node = $(this),
                    nodeData = {
                        level: node.ftattr("level"),
                        parentId: node.ftattr("parent"),
                        id: node.ftattr("id"),
                        seq: node.ftattr("sequence"),
                        state: node.ftattr("state"),
                        visible: node.ftattr("visible")
                    },
                    nodeDataString = JSON ? JSON.stringify(nodeData) : "",
                    debugElm = node.find(".debug"),
                    debugData = '<span class="debug">' + nodeDataString + '</span>';

                var debug = true;
                try {
                    debug = _static.fn.getSettings(node).debug;
                } catch (err) {
                }


                if (debug) {
                    if (debugElm.length > 0) {
                        debugElm.replaceWith(debugData);
                    }
                    else {
                        node.append(debugData);
                    }
                }
            },
            // dumps data about the node being dragged and where it would be dropped
            dumpDrag: function () {
                var node = $(this),
                    tree = _static.fn.getTree(node),
                    settings = tree.data(_static.dataKey).settings,
                    helper = $(settings.dragging_helper),
                    position = helper.position(),
                    offset = helper.offset(),
                    placeholder = $(settings.placeholder),
                    nodeAbove = placeholder.prevAll(_static.sel.visibleNotNode(node.ftattr("id"))).first(),
                    nodeBelow = placeholder.nextAll(_static.sel.visibleNotNode(node.ftattr("id"))).first(),
                    output = $(settings.debugDragOutput),
                    drop = _static.fn.calculateDrop(node, nodeAbove.length > 0 ? nodeAbove : null, nodeBelow.length > 0 ? nodeBelow : null, settings.sortableThreshold, settings.sortableUseMouse);

                if (settings.debug) {
                    output.empty();
                    output.append('<div>Node: ' + node.ftattr("id") + '</div>');
                    output.append('<div>Relative: { left: ' + position.left + ', top: ' + position.top + ' }</div>');
                    output.append('<div>Absolute: { left: ' + offset.left + ', top: ' + offset.top + ' }</div>');
                    output.append('<div>Above: ' + nodeAbove.ftattr("id") + '</div>');
                    output.append('<div>Below: ' + nodeBelow.ftattr("id") + '</div>');
                    output.append('<div>Drop: { type: ' + drop.type + ', parent: ' + (drop.parent ? drop.parent.ftattr("id") : 'null') + ', relative: ' + (drop.relative ? drop.relative.ftattr("id") : 'null') + ' }</div>');
                }
            },
            // applies the callback function to each descendant of the node
            //
            // node - node whose descendants we are going to apply callback to
            // callback - function to call for each descendant of node. can optionally take in
            //            a single parameter which is the parent of the node. callback function's context (i.e. this) is
            //            the node itself.
            applyToDescendants: function (node, callback) {
                $(_static.sel.childOfNode(node)).each(function () {
                    callback.apply($(this), [node]);
                    _static.fn.dumpNode.apply($(this));
                    _static.fn.applyToDescendants($(this), callback);
                });

            },
            // applies the callback function to each descendant of the node
            // Ignores tree id
            //
            // node - node whose descendants we are going to apply callback to
            // callback - function to call for each descendant of node. can optionally take in
            //            a single parameter which is the parent of the node. callback function's context (i.e. this) is
            //            the node itself.
            applyToDescendantsAllTrees: function (node, callback) {
                var filterNodesByTree = _static.defaults.filterNodesByTree;
                _static.defaults.filterNodesByTree = false;
                $(_static.sel.childOf(node.ftattr("id"))).each(function () {
                    callback.apply($(this), [node]);
                    _static.fn.dumpNode.apply($(this));
                    _static.fn.applyToDescendantsAllTrees($(this), callback);
                });
                _static.defaults.filterNodesByTree = filterNodesByTree;
            },

            //check to see if the his node has Children;
            //
            //node - node to check if it has children
            hasChildren: function (node) {
                return ($(_static.sel.childOfNode(node)).length > 0);
            },
            //return all the children of this node
            getChildren: function (node) {
                return $(_static.sel.childOfNode(node));
            },
            getLastSequence: function (node) {
                var parentId = node.ftattr("id");
                var children = ($(_static.sel.childOf(parentId)));
                var sequences = $(children).map(function () { return $(this).attr("data-ft-sequence"); }).get();
                return $(sequences.sort()).last().get(0);
            },
            // applies the callback function to all ancestors of the node
            //
            // node - node whose ancestor we are going to apply callback to
            // callback - function to call for all ancestors of node. can optionally take in
            //            a single parameter which is the parent of the node. callback function's context (i.e. this) is
            //            the node itself.
            applyToAncestors: function (node, callback, targetNode) {
                if (node != null && node != undefined) {
                    var parentId = node.ftattr("parent");
                    var parentNode = _static.sel.nodeById(parentId);
                    if (parentId != null && parentId != undefined && parentId != "") {
                        callback.apply(node, [parentNode, targetNode]);
                        _static.fn.applyToAncestors($(parentNode), callback, targetNode);
                    }
                }
            },
            // gets an array of all parents for a node
            getAllParents: function (node) {
                var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
                var parents = new Array();

                while (target != null && target.length > 0) {
                    var parentId = target.ftattr("parent");
                    target = $(_static.sel.nodeById(parentId));
                    if (target.length > 0) {
                        parents.push($(_static.sel.nodeById(parentId)));
                    }
                }

                return parents.reverse();
            },
            // applies the callback function to all siblings of the node
            //
            // node - node whose sibling we are going to apply callback to
            // callback - function to call for all siblings of node. can optionally take in
            //            a single parameter which is the parent of the node. callback function's context (i.e. this) is
            //            the node itself.
            applyToSiblings: function (node, callback) {
                if (node != null && node != undefined) {
                    var parentId = node.ftattr("parent");

                    var siblingNodes;
                    if (parentId == null || parentId == undefined || parentId == "" || parentId == "PX_MULTIPART_LESSONS") {
                        siblingNodes = node.siblings(_static.sel.rootNodes);
                    } else {
                        siblingNodes = node.siblings(_static.sel.childOf(parentId));
                    }
                    siblingNodes.each(function () {
                        callback.apply($(this), [node]);
                    });

                }
            },
            // returns the last descendant of node, or node itself if node is empty or barren
            // 
            // node - node to get last descendant of
            // descendants - tail-recursive parameter that can either be undefined or an empty array on first invocation
            lastDescendantOrSelf: function (node, descendants) {
                if (!node) {
                    return null;
                }

                if (!descendants) {
                    descendants = [];
                }

                descendants.push(node);

                _static.fn.applyToDescendants(node, function () {
                    descendants.push(this);
                });

                return descendants.pop();
            },

            //returns the topmost ancestor of this node
            topAncestorOrSelf: function (node) {
                if (!node || node.length == 0)
                    return null;

                if ($(node).ftattr("level") == 1)
                    return $(node);
                else {
                    var parentNode = $.fn.fauxtree("getNode", $(node).ftattr("parent"));
                    return _static.fn.topAncestorOrSelf(parentNode);
                }

            },
            // traverse an entire tree of state and applies it to the DOM elements.
            traverseState: function (state, callback) {
                $.each(state, function () {
                    var node = null;
                    if ($(_static.sel.nodeById(this.id)).length == 0) {
                        node = _static.fn.createNode(this);
                        _static.fn.lastDescendantOrSelf($(_static.sel.nodeById(this.parentId))).after(node);
                    }

                    _static.fn.setNodeData(this, callback, node);

                    if (this.children) {
                        _static.fn.traverseState(this.children, callback);
                    }
                });
            },
            // applies the correct even and odd styles to all nodes in the tree
            updateNodeStyle: function () {
                settings = $(this).data(_static.dataKey).settings;
                $(this).find(_static.sel.evenNodes).removeClass(settings.css.oddClass).addClass(settings.css.evenClass);
                $(this).find(_static.sel.oddNodes).removeClass(settings.css.evenClass).addClass(settings.css.oddClass);
            },
            //sorts the tree using the comparison function provided.
            //each node and its subchildren are sorted individually
            //also reorders items based on location of parent
            //comparer returns -1, 0, 1
            sortTree: function (nodes, comparator, recursive) {
                if (nodes.length == 0) {
                    return;
                }
                if (nodes == null) {
                    nodes = $(_static.sel.rootNodes);
                }
                else if (nodes.is("ul")) {
                    nodes = nodes.find(_static.sel.rootNodes);
                }
                if (nodes.length == 0) {
                    return;
                }
                var settings = _static.fn.getSettings(nodes);
                _static.defaults.filterNodesByTree = settings.filterNodesByTree;
                _static.defaults.filterByTreeId = settings.filterByTreeId;

                nodes.sortElements(comparator);

                nodes.each(function (n, node) {
                    //move child nodes to below current item
                    $(node).after(_static.fn.getChildren($(node)));
                    //sort child nodes
                    if (recursive) {
                        _static.fn.sortTree(_static.fn.getChildren($(node)), comparator, recursive);
                    } else {
                        _static.fn.orderBasedOnParent(_static.fn.getChildren($(node)));
                    }
                });

                //_static.fn.scanTree.apply($(_static.sel.tree));
            },
            //orders the items in the tree based on parent-child heirarchy
            orderBasedOnParent: function (nodes) {
                if (nodes == null) {
                    nodes = $(_static.sel.rootNodes);
                }
                nodes.each(function (n, node) {
                    //move child nodes to below current item
                    var children = _static.fn.getChildren($(node));
                    if (children.length > 0) {
                        $(node).after(children);
                        //sort child nodes
                        _static.fn.orderBasedOnParent(_static.fn.getChildren($(node)));
                    }
                });
            },
            // scans the tree and applies the style necessary based on
            // the state and other attributes of each node.
            scanTree: function (nodes) {
                var tree = $(this);

                if (tree == null || tree.length == 0 || !tree.data) return;

                var settings = tree.data(_static.dataKey).settings;
                _static.defaults.filterByTreeId = tree.attr("id"); //settings.filterByTreeId;
                var filterNodes = _static.defaults.filterNodesByTree;
                _static.defaults.filterNodesByTree = true;

                var isFullTree = nodes == null || nodes.length < 1;
                if (isFullTree) {
                    nodes = tree.find(_static.sel.node);
                }
                $(nodes).each(function() {
                    var node = $(this),
                        parent = node.ftattr("parent"),
                        level = node.ftattr("level"),
                        state = node.ftattr("state"),
                        visible = node.ftattr("visible"),
                        parentNode,
                        indent = settings.indent;

                    if (!parent) {
                        // if there is no parent, that means that this is a first-level node
                        level = _static.minLevel;
                    } else {
                        // there is a parent, and thus we need to calculate the level of this node based on the
                        // parent node's level.
                        parentNode = $(_static.sel.nodeById(parent));
                        if (parentNode.length > 1) {
                            parentNode.each(function() {
                                if ($(this).parent().attr('class') == $(tree).attr('class')) {
                                    parentNode = $(this);
                                }
                            });
                        }

                        if (parentNode != null && parentNode.length > 0) {
                            level = parseInt(parentNode.ftattr("level")) + 1;
                        } else {
                            level = 1;
                        }
                    }

                    if (!state) {
                        // when a node has no state we should assume it can bear children and is initially closed.
                        state = _static.states.closed;
                    }
                    
                    if (state == _static.states.open && !isFullTree) {
                        //if we aren't scanning the full tree, scan the children
                        _static.fn.scanTree.apply(tree, [_static.fn.getChildren(node)]);
                    }

                    if (settings.onScanItem) {
                        settings.onScanItem.apply(node, [{
                            sender: tree
                        }]);
                    }

                    if ((!parent || parentNode.length == 0 || settings.showall) && visible != 'false') {
                        // if no visibility is explicitly set and this node has no parent, it should be
                        // visible by default.
                        visible = "true";
                    }
                    else {
                        //if the parent is open, this node should be visible
                        if (parentNode && parentNode.ftattr("state") == _static.states.open && parentNode.ftattr('visible') != 'false') {
                            visible = "true";
                        } else {
                            // on an initial load like this, all nodes other than the first-level nodes are 
                            // hidden.
                            visible = "false";
                        }
                    }

                    // calculate the indentation in pixels
                    if (level > 0) {
                        indent = level * indent;
                    }
                    // save all state attributes as they may have changed.
                    node.ftattr("level", level);
                    node.ftattr("state", state);
                    node.ftattr("visible", visible);
                    node.css("paddingLeft", indent + "px");

                    node.removeClass("unitrowlevel1")
                        .removeClass("unitrowlevel2")
                        .removeClass("unitrowlevel3")
                        .removeClass("unitrowlevel4");

                    var indentLevelClass = "unitrowlevel" + level;
                    node.addClass(indentLevelClass);

                    // visibility is not rendered
                    if (visible == "true") {
                        if (!node.is(":visible")) {
                            if (settings.animateOpenNode) {
                                if (node.is(':animated')) {
                                    $(node).stop(true, true);
                                }
                                $(node).slideDown();
                                
                            } else {
                                node.show();
                            }
                        }
                    }
                    else {
                        if (node.is(":visible")) {
                            if (settings.animateOpenNode) {
                                if (node.is(':animated')) {
                                    $(node).stop(true, true);
                                }
                                $(node).slideUp();
                            } else {
                                node.hide();
                            }
                        }
                    }

                    if (settings.debug) {
                        _static.fn.dumpNode.apply(node);
                    }
                });

                if (tree.not(":visible")) {
                    // if a category has not been expanded, then we need to set the first-level nodes styles a little differently
                    tree.find(_static.sel.firstLevelEvenNodes).removeClass(settings.css.oddClass).addClass(settings.css.evenClass);
                    tree.find(_static.sel.firstLevelOddNodes).removeClass(settings.css.evenClass).addClass(settings.css.oddClass);
                }

                _static.fn.updateNodeStyle.apply(tree);

                _static.defaults.filterNodesByTree = filterNodes;
            },
            // close all nodes except for node and its parents
            closeAllExcept: function (node) {

                _static.fn.applyToSiblings(node, function () {
                    if ($(this).ftattr("state") == _static.states.open) {
                        _static.fn.closeNode.apply($(this));
                    }
                });

                node.show();
                _static.fn.setTreeVisibility(node, true);
            },
            // opens all immediate child nodes of this node if they are closed
            openNode: function () {
                var root = $(this),
                    rootId = root.ftattr("id"),
                    placeholder = $(_static.sel.placeholder),
                    tree = _static.fn.getTree(root),
                    destinationTree = _static.fn.getTree(placeholder),
                    data = tree.data(_static.dataKey);
                var settings = data.settings,
                    isDragging = settings.is_dragging;

                if (destinationTree.length > 0 && destinationTree != tree) {
                    tree = destinationTree;
                }

                _static.defaults.filterByTreeId = tree.attr("id");
                if (data.settings.closeAllSiblingsOnOpen && !isDragging) {
                    _static.fn.closeAllExcept(root);
                }

                root.show();
                root.ftattr("state", _static.states.open);

                root.find("." + _static.css.icon)
                        .toggleClass(_static.css.collapsed)
                        .toggleClass(_static.css.expanded);
                
                var doOpenNode = function () {
                    _static.fn.applyToDescendants(root, function (parent) {
                        var child = $(this),
                            state = child.ftattr("state"),
                            parentId = parent.ftattr("id"),
                            parentState = parent.ftattr("state");

                        if (parentId == rootId || parentState === _static.states.open) {
                            child.ftattr("visible", "true");
                        } else if (state === _static.states.open) {
                            child.ftattr("visible", "true");
                        }
                    });

                    _static.fn.scanTree.apply(tree, [_static.fn.getChildren(root)]);

                    if (data.settings.onOpenNode) {
                        data.settings.onOpenNode.apply(root, [{ sender: tree}]);
                    }
                };

                if ((data.settings.onLoadNodes && !_static.fn.hasChildren(root) && root.ftattr("has-children") != "false") ||
                    root.ftattr("forceloadnode") == "true") {

                    if (root.ftattr("forceloadnode") == "true") {
                        $(_static.sel.childOfNode(root)).remove();
                        $(root).ftattr("forceloadnode", "false");
                    }

                    data.settings.onLoadNodes.apply(root, [{
                        sender: tree,
                        callback: function () {
                            if (_static.fn.hasChildren(root)) {
                                doOpenNode();
                            }

                            if (settings.showLastViewed) {
                                $(_static.sel.nodeLink).unbind("click", _static.fn.clickNodeLink);
                                $(_static.sel.nodeLink).bind("click", _static.fn.clickNodeLink);
                            }
                            if (isDragging) {
                                $('.faux-tree.ui-sortable').sortable("refresh");
                            }
                        }
                    }]);
                } else {
                    doOpenNode();
                }
            },
            // closes the node and hides all descendants
            closeNode: function (node) {
                var parent;

                if (node) {
                    parent = node;
                } else {
                    parent = $(this);
                }
                var tree = _static.fn.getTree(parent),
                    data = tree.data(_static.dataKey);

                _static.defaults.filterByTreeId = tree.attr("id");

                parent.find("." + _static.css.icon)
                        .toggleClass(_static.css.expanded)
                        .toggleClass(_static.css.collapsed);
                
                _static.fn.applyToDescendants(parent, function (parentId) {
                    var child = $(this);
                    child.ftattr("visible", "false");
                });

                parent.ftattr("state", _static.states.closed);
                _static.fn.scanTree.apply(tree, [parent]);
                _static.fn.scanTree.apply(tree, [_static.fn.getChildren(parent)]);

                if (data.settings.onCloseNode) {
                    data.settings.onCloseNode.apply(parent, [{
                        sender: tree
                    }]);
                }
            },
            // toggles whether a node is open or closed
            toggleNode: function () {
                var node = $(this),
                    state = node.ftattr("state");

                if (state === _static.states.open) {
                    _static.fn.closeNode.apply(node);
                }
                else if (state === _static.states.closed) {
                    _static.fn.openNode.apply(node);
                }
            },
            // handles case when a node is clicked as opposed to toggled
            clickNode: function (event) {
                var node = $(this),
                    tree = _static.fn.getTree(node),
                    data = tree.data(_static.dataKey);

                if (data) {
                    _static.defaults.filterByTreeId = data.settings.filterByTreeId;
                    _static.defaults.filterNodesByTree = data.settings.filterNodesByTree;
                }

                $(_static.sel.node).removeClass(_static.css.activeNode);
                node.addClass(_static.css.activeNode);

                if (data.settings.onNodeClick) {
                    data.settings.onNodeClick.apply(node, [{
                        sender: tree
                    }]);
                }
            },
            // handles when a link inside a node is clicked 
            clickNodeLink: function (event) {
                var node = $(this),
                    tree = _static.fn.getTree(node),
                    data = tree.data(_static.dataKey);
                
                $('.last-viewed').removeClass("last-viewed");

                data.settings.LastViewed = node.closest(".faux-tree-node");
                data.settings.LastViewed.addClass("last-viewed");

                set_cookie("last_viewed_" + $("#CourseId").val(), data.settings.LastViewed.ftattr('id'), '0', '/');
            },
            // handles case when a node is mouse entered as opposed to toggled
            mouseEnterNode: function (event) {
                var node = $(this),
                    tree = _static.fn.getTree(node),
                    data = tree.data(_static.dataKey);

                $(_static.sel.node).removeClass(_static.css.hoverNode);
                node.addClass(_static.css.hoverNode);

                if (data.settings.onNodeMouseEntered) {
                    data.settings.onNodeMouseEntered.apply(node, [{
                        sender: tree
                    }]);
                }
            },
            // handles case when a node mouseleave event is fired
            mouseLeaveNode: function (event) {
                var node = $(this),
                    tree = _static.fn.getTree(node),
                    data = tree.data(_static.dataKey);

                $(_static.sel.node).removeClass(_static.css.hoverNode);

                if (data.settings.onNodeMouseLeave) {
                    data.settings.onNodeMouseLeave.apply(node, [{
                        sender: tree
                    }]);
                }
            },
            // sets the visibility state of the tree starting at the root
            //
            // root - node at which to start updating visiblity
            // visible - whether the nodes in the tree should be shown or not
            setTreeVisibility: function (root, visible) {
                _static.fn.applyToDescendants(root, function () {
                    if (visible && $(this).ftattr("visible") == "true") {
                        $(this).show();
                    }
                    else {
                        $(this).hide();
                    }
                });
            },
            // moves the specified subtree and recalculates the level and other parameters
            //
            // node - the node being moved
            // parent - the parent of node, which could be different from its current parent
            // relative - the item below which node should be moved. typically this is the same as
            //            parent, but there are cases where we need to move node below parent's lastDescendant.
            moveTree: function (node, parent, relative) {
                var level = _static.minLevel,
                    parentId = "",
                    settings = _static.fn.getSettings(node);

                if (parent && parent.length > 0) {
                    level = parseInt(parent.ftattr("level")) + 1;
                    parentId = parent.ftattr("id");
                    node.insertAfter(relative);
                } else {
                    //no parentId was specified, this is a root node
                    node.insertAfter(relative);
                    //set the parentId to the relatives parent id
                    if (relative != null && relative.length > 0) {
                        //find root-level node that's a parent of teh relative, use it's parent id
                        if (relative.filter(_static.sel.rootNodes).length > 0) {
                            //relative is at the root level
                            parentId = relative.ftattr("parent");
                        } else {
                            //find root-level node above the relative, use it's parentid
                            parentId = relative.prevAll(_static.sel.rootNodes).first().ftattr("parent");
                        }

                    } else {
                        //if there is no relative, this is the first node, set parentid to node below
                        var below = node.nextAll(_static.sel.visibleNotNode(node.ftattr("id"))).first();
                        if (below != null && below.length > 0) {
                            parentId = below.ftattr("parent");
                        } else {
                            //this is the only node in the tree, set root parent to default
                            parentId = settings.defaultRootParent;
                        }
                    }
                }
                node.ftattr("parent", parentId);
                node.ftattr("level", level);

                _static.fn.applyToDescendantsAllTrees(node, function (parent) {
                    _static.fn.moveTree($(this), parent, parent);
                });
            },
            // Prepares and object that contains all of the information necessary to move
            // the currently dragged node in case the user performs a drop. 
            //
            // node - the node being dragged
            // nodeAbove - the node that will be above the node being dragged if it were to be
            //             dropped.
            // nodeBelow - the node that will be below the node being dragged if it were to be
            //             dropped.
            // threshold - if the node is dragged more than this many pixels to the right of 
            //             the nodeAbove, then the user's intent is that node be a child of 
            //             nodeAbove.
            calculateDrop: function (node, nodeAbove, nodeBelow, threshhold, useCursorPosition) {
                var aboveTitle,
                    nodeTitle,
                    aboveLeft,
                    nodeLeft,
                    delta,
                    drop = {
                        type: _static.dropTypes.sibling,
                        relative: null,
                        parent: null,
                        node: node,
                        expandNodeTimeout: null
                    },
                    placeholder = $(_static.sel.placeholder),
                    tree = _static.fn.getTree(node),
                    destinationTree = _static.fn.getTree(placeholder),
                    data = tree.data(_static.dataKey),
                    aboveState,
                    relative = nodeAbove,
                    aboveReadOnly = false;

                var setNodeAsChild = function (node, nodeAbove, relative, drop) {
                    drop.type = _static.dropTypes.child;
                    drop.relative = relative ? relative : nodeAbove;
                    drop.parent = nodeAbove;
                    return drop;
                };
                if (nodeAbove) {
                    aboveReadOnly = nodeAbove.udattr && nodeAbove.udattr("read-only") == "True";
                    aboveTitle = nodeAbove.find(_static.sel.nodeTitle);
                    aboveState = nodeAbove.ftattr("state");
                    nodeTitle = node.find(_static.sel.nodeTitle);

                    if (aboveTitle.length > 0 && nodeTitle.length > 0) {

                        if (data.settings.sortableDisableIndentation) {
                            //Dont use the horizontal position of the mouse to determine location
                            //user vertical position: if the mouse is above the placeholder, use node above
                            var useNodeAbove = (window.mouseY) < ($(nodeAbove).offset().top + $(nodeAbove).height()) || nodeBelow == null;
                            var nodeAboveRange = {
                                topRange: $(nodeAbove).offset().top + $(nodeAbove).height() / 4,
                                bottomRange: $(nodeAbove).offset().top + $(nodeAbove).height() * 3 / 4
                            };
                            //PxPage.log($.format("Node Above: top: {0}, bottom:{1}, mouse: {2}", nodeAboveRange.topRange, nodeAboveRange.bottomRange, window.mouseY));
                            var dragOverNodeAbove = (window.mouseY) >= nodeAboveRange.topRange
                                && (window.mouseY) <= nodeAboveRange.bottomRange;

                            var nodeBelowRange = {
                                topRange: $(nodeBelow).length > 0 ? $(nodeBelow).offset().top + $(nodeBelow).height() / 4 : 0,
                                bottomRange: $(nodeBelow).length > 0 ? $(nodeBelow).offset().top + $(nodeBelow).height() * 3 / 4 : 0
                            };
                            //PxPage.log($.format("Node Below: top: {0}, bottom:{1}, mouse: {2}", nodeBelowRange.topRange, nodeBelowRange.bottomRange, window.mouseY));
                            var dragOverNodeBelow = (window.mouseY) >= nodeBelowRange.topRange
                                && (window.mouseY) <= nodeBelowRange.bottomRange;

                            if (!dragOverNodeAbove && !dragOverNodeBelow) {
                                placeholder.show();
                                $(".ui-fauxtree-dragover").removeClass("ui-fauxtree-dragover");
                                $(".ui-fauxtree-helper #dragIcon").removeClass(_static.css.dropInvalid + _static.css.dropInto).addClass(_static.css.dropBetween);
                            }
                            if (useNodeAbove && aboveState != _static.states.open) {
                                if (aboveState == _static.states.closed && dragOverNodeAbove) {
                                    //the node above is closed, we should be able to drop an item into it
                                    placeholder.hide();
                                    $(placeholder).insertAfter(nodeAbove);
                                    nodeAbove.addClass("ui-fauxtree-dragover");
                                    $(".ui-fauxtree-helper #dragIcon").removeClass(_static.css.dropInvalid + _static.css.dropBetween).addClass(_static.css.dropInto);
                                    drop = setNodeAsChild(node, nodeAbove, false, drop);
                                } else {
                                    // in this case we assume that node is meant to be a sibling of nodeAbove
                                    drop.type = _static.dropTypes.sibling;
                                    drop.relative = nodeAbove;
                                    drop.parent = $(_static.sel.nodeByTreeAndId(destinationTree, nodeAbove.ftattr("parent")));
                                }
                            }
                            else if (useNodeAbove && aboveState == _static.states.open) {
                                //the node above is open, this should be a child of the node above
                                relative = $(_static.sel.placeholder).prevAll(_static.sel.visibleNotNode(node.ftattr("id"))).first();
                                drop = setNodeAsChild(node, nodeAbove, relative, drop);

                            } else {
                                if (nodeBelow.ftattr("state") == _static.states.closed && dragOverNodeBelow) {
                                    //the node below is closed, we should be able to drop an item into it
                                    placeholder.hide();
                                    $(placeholder).insertAfter(nodeBelow);
                                    nodeBelow.addClass("ui-fauxtree-dragover");
                                    $(".ui-fauxtree-helper #dragIcon").removeClass(_static.css.dropInvalid + _static.css.dropBetween).addClass(_static.css.dropInto);
                                    drop = setNodeAsChild(node, nodeBelow, false, drop);
                                } else {
                                    // in this case we assume that node is meant to be a sibling of nodeBelow
                                    drop.type = _static.dropTypes.siblingBelow;
                                    drop.relative = nodeBelow;
                                    drop.parent = $(_static.sel.nodeByTreeAndId(destinationTree, nodeBelow.ftattr("parent")));
                                }
                            }

                        } else {
                            if (useCursorPosition) {
                                nodeLeft = window.mouseX;
                            } else {
                                nodeLeft = nodeTitle.offset().left;
                            }
                            aboveLeft = aboveTitle.offset().left;

                            delta = nodeLeft - aboveLeft;
                            if (delta < 0 || aboveState == _static.states.barren) {
                                // in this case, we need to recurse up the tree and calculate the drop based on
                                // the parent of nodeAbove because nodeAbove is either not allowed to have children or
                                // the user has dragged node to the left of nodeAbove.
                                drop = _static.fn.calculateDrop(node, $(_static.sel.nodeById(nodeAbove.ftattr("parent"))), nodeBelow, threshhold, useCursorPosition);
                            } else if (delta >= threshhold) {
                                // in this case, node has been dragged to the right of nodeAbove to a degree where we
                                // think the user wants node to be a child of nodeAbove.
                                drop.type = _static.dropTypes.child;
                                relative = $(_static.sel.placeholder).prevAll(_static.sel.visibleNotNode(node.ftattr("id"))).first();
                                drop.relative = relative ? relative : nodeAbove;
                                drop.parent = nodeAbove;
                            } else {
                                // in this case we assume that node is meant to be a sibling of nodeAbove
                                drop.type = _static.dropTypes.sibling;
                                drop.relative = nodeAbove;
                                drop.parent = $(_static.sel.nodeById(nodeAbove.ftattr("parent")));
                            }
                        }
                    }
                } else {
                    //there is no node above, this is either first node or the only node, insert at the top
                    placeholder.show();
                    $(".ui-fauxtree-dragover").removeClass("ui-fauxtree-dragover");
                    $(".ui-fauxtree-helper #dragIcon").removeClass(_static.css.dropInvalid + _static.css.dropInto).addClass(_static.css.dropBetween);
                    
                    if (nodeBelow) {
                        // in this case we assume that node is meant to be a sibling of nodeBelow
                        drop.type = _static.dropTypes.siblingBelow;
                        drop.relative = nodeBelow;
                        drop.parent = $(_static.sel.nodeByTreeAndId(destinationTree, nodeBelow.ftattr("parent")));
                    }
                }

                //get parent title and set dragging label
                var parentTitle = $(drop.parent).find(".faux-tree-node-title .fptitle").text().trim();
                if (parentTitle.length == 0) {
                    parentTitle = $(drop.parent).find(".faux-tree-node-title .unitfptitle").text().trim();
                }
                if (parentTitle.length == 0) {
                    parentTitle = data.settings.sortableRootLevelName;
                }
                if (data.settings.useCustomText) {
                    $(_static.sel.placeholder).find(".drag-label").text(data.settings.dragLabelCustomText);
                } else {
                    $(_static.sel.placeholder).find(".drag-label").text("Move to " + parentTitle);
                }

                _static.fn.updatePlaceholder(drop, aboveReadOnly);
                data.settings.current_drop = drop;
                destinationTree.data(_static.dataKey, data);

                return drop;
            },
            // sets the visible indent on the ui-placeholder to give the user some visual indication
            // where a node will be dropped. This lets the user know if the node will be a child or 
            // a sibling, and of what node.
            //
            // drop - object that contains the details of the drop on which to base the display style of
            //        the placeholder
            updatePlaceholder: function (drop, aboveReadOnly) {
                var level = _static.minLevel,
                    settings = _static.fn.getSettings(drop.node),
                    indent = settings.indent;

                if (drop.type == _static.dropTypes.child) {
                    level = parseInt(drop.parent.ftattr("level")) + 1;
                }
                else if (drop.relative && (drop.type == _static.dropTypes.sibling || drop.type == _static.dropTypes.siblingBelow)) {
                    level = drop.relative.ftattr("level");
                }

                indent = (level * settings.indent) + settings.iconWidth;

                if (settings.sortableDisableReadOnlyDrag && aboveReadOnly) {
                    $(_static.sel.placeholder).css("background-color", "red");
                }
                else {
                    $(_static.sel.placeholder).css("background-color", "");
                    $(_static.sel.placeholder).css("margin-left", indent + "px");

                    $(_static.sel.placeholder).removeClass("unitrowlevel1")
                    .removeClass("unitrowlevel2")
                    .removeClass("unitrowlevel3")
                    .removeClass("unitrowlevel4")
                    .addClass("unitrowlevel" + level);
                }

            },
            // when a node starts sorting, we start watching its position
            onSortStart: function (event, ui) {
                var tree = _static.fn.getTree(ui.item),
                    data = tree.data(_static.dataKey);

                _static.fn.setTreeVisibility($(ui.item), false);


                if (!settings.allowRootLevelUnitNesting && $(ui.helper).ftattr("level") <= 1 && $(ui.helper).ftattr("state") != _static.states.barren) {
                    //dragged element is a root level element - close all other root-level elements
                    _static.fn.closeAllExcept($(data.settings.dragging_elm));
                }

                data.settings.is_dragging = true;
                data.settings.dragging_elm = ui.item;
                data.settings.dragging_helper = ui.helper;
                data.settings.placeholder = ui.placeholder;
                if (data.settings.showDragIcon) {
                    $(ui.placeholder).append("<div class='icon'></div>");
                }
                if (data.settings.showDragLabel) {
                    $(ui.placeholder).append("<div class='drag-label'></div>");
                }
                tree.data(_static.dataKey, data);

                if (data.settings.onNodeStartDrag) {
                    $(window).bind('keydown.fauxtree', function (e) {
                        if (27 == e.keyCode) {
                            data.settings.current_drop = null;
                            $(document).trigger("mouseup");
                            $(window).unbind('keydown.fauxtree');
                        }
                    });
                    $('body').bind("mousemove.fauxtree", function (e) {
                        var outsideTheTree = _static.fn.dragIsOutsideTree(e, tree);
                        if (outsideTheTree) {
                            $(".ui-fauxtree-helper #dragIcon").removeClass(_static.css.dropBetween + _static.css.dropInto).addClass(_static.css.dropInvalid);
                        }
                    });
                    data.settings.onNodeStartDrag.apply(data.settings.dragging_elm);
                }
            },
            dragIsOutsideTree: function (event, tree) {
                var inTree = false;
                //check if item is in ANY faux-tree
                $(_static.sel.tree).each(function () {
                    var tree = $(this);

                    if (event.pageX >= tree.offset().left && event.pageY >= tree.offset().top - 30
                        && event.pageX <= tree.offset().left + tree.width() && event.pageY <= tree.offset().top + tree.height() + 30)
                        inTree = true;
                });

                return !inTree;
            },
            // when a node stops sorting, we stop watching its position
            onSortStop: function (event, ui) {
                var tree = _static.fn.getTree(ui.item),
                    data = tree.data(_static.dataKey);

                $(window).unbind('keydown');

                var outsideTheTree = _static.fn.dragIsOutsideTree(event, tree);
                if (outsideTheTree) {
                    data.settings.current_drop = null;
                    $(tree).filter('.ui-sortable').sortable('cancel');
                }
                $('body').unbind("mousemove.fauxtree");

                // since we have hidden any previously displayed children to prevent a node from
                // being moved inside itself, we should reshow them if applicable once the sorting
                // has stopped.
                _static.fn.setTreeVisibility($(ui.item), true);

                if (data.settings.current_drop) {
                    var aboveReadOnly = (data.settings.current_drop.relative != null) &&
                        data.settings.current_drop.relative.udattr &&
                        data.settings.current_drop.relative.udattr("read-only") == "True";
                    if (aboveReadOnly && settings.sortableDisableReadOnlyDrag) {
                        //node is read only (aka "add more resources" button), set node to node above
                        tree.sortable('cancel');
                    }
                    else {
                        // if sorting stops and a drop has been prepared, that means
                        // something moved. technically we haven't gauranteed that a node's 
                        // order was changed, but this is a safe redundancy.
                        _static.fn.onNodeDropped(event, ui);
                    }
                } else {
                    $(tree).filter('.ui-sortable').sortable('cancel');
                }
                $(".ui-fauxtree-dragover").addClass(_static.css.fade);
                $(".ui-fauxtree-dragover").removeClass("ui-fauxtree-dragover");

                data.settings.is_dragging = false;
                data.settings.dragging_elm = null;
                data.settings.dragging_helper = null;
                data.settings.placeholder = null;
                data.settings.current_drop = null;
                tree.data(_static.dataKey, data);
            },
            // updates debugging information when a node is being moved
            onNodeDragging: function (event) {
                var settings = this.data(_static.dataKey).settings,
                    node = settings.is_dragging ? $(settings.dragging_elm) : null,
                    helper = settings.is_dragging ? $(settings.dragging_helper) : null,
                    parent;

                window.mouseX = event.pageX;
                window.mouseY = event.pageY;

                if (node) {
                    _static.fn.dumpDrag.apply(node);
                    settings = this.data(_static.dataKey).settings;
                    if (settings.sortableEnableWindowScrollOnDrag) {
                        var scrollspeed = 100;
                        var scrollableElem = 'body,html,document';
                        var ytop = $(window).scrollTop();
                        var ybottom = $(window).scrollTop() + $(window).height();

                        if (window.mouseY > ytop - 20 && window.mouseY < (ytop + 20)) {
                            //top edges
                            $(scrollableElem).stop().animate({
                                scrollTop: window.mouseY - scrollspeed
                            },
                                400);
                        } else if (window.mouseY > ybottom - 20 && window.mouseY < ybottom + 20) {
                            //bottom edges
                            $(scrollableElem).stop().animate({
                                scrollTop: window.mouseY - $(window).height() + scrollspeed
                            }, 400);
                        } else {
                            $(scrollableElem).stop();
                        }
                    }

                    if (settings.sortableDisableIndentation) {
                        //if we aren't using indentation, apply a delay on node expansion
                        if (settings.current_drop) {
                            //check if a node is a root level unit with childNodes
                            var isUnit = $(settings.current_drop.node).ftattr("level") == 1 && $(settings.current_drop.node).ftattr("state") != _static.states.barren;

                            //expand node with a timeout
                            if (settings.current_drop.expandNodeTimeout == null && (!isUnit || settings.allowRootLevelUnitNesting)) {
                                var curNode = this;
                                var nodeToOpen = curNode.data(_static.dataKey).settings.current_drop.relative;
                                if (nodeToOpen != null && nodeToOpen.ftattr("level") != 1) {//do not automatically expand chapter nodes
                                    state = nodeToOpen.ftattr("state");
                                    //expand only closed nodes
                                    if (state == _static.states.closed) {
                                        settings.current_drop.expandNodeTimeout =
                                            window.setTimeout(function () {
                                                if (settings.current_drop) {
                                                    //only expand if user is still dragging
                                                    settings.current_drop.expandNodeTimeout = null;
                                                    var currentNode = curNode.data(_static.dataKey).settings.current_drop.relative;
                                                    if (nodeToOpen == currentNode) {
                                                        _static.fn.openNode.apply(currentNode);
                                                    }
                                                }
                                            }, settings.sortableNodeExpandDelay);
                                    }
                                }
                            }
                        }
                    } else {
                        if (settings.current_drop && (settings.current_drop.type == _static.dropTypes.child)) {
                            // if the user's intent is to make the dragged node a child of an item,
                            // then the potential parent should be opened.
                            parent = settings.current_drop.parent;

                            if (parent && parent.ftattr("state") == _static.states.closed) {
                                _static.fn.openNode.apply(parent);
                            }
                        }
                    }
                }
            },
            // handles the case where a node is moved in the tree
            onNodeDropped: function (event, ui) {
                var node = ui.item,
                    $this = _static.fn.getTree(node),
                    tree = ui.sender ? ui.sender : $this,
                    data = tree.data(_static.dataKey),
                    drop = data.settings.current_drop;

                if (data.settings.current_drop != null) {
                    if (drop.type == _static.dropTypes.sibling) {
                        // this ensures that the dropped element is placed after the last descendant of the
                        // new parent node. this is important to take into account because the underlying elements
                        // are linear.
                        drop.relative = _static.fn.lastDescendantOrSelf(drop.relative);
                        if (drop.node.is(drop.relative)) {
                            drop.relative = drop.relative.prev();
                        }
                    }
                    else if (drop.type == _static.dropTypes.siblingBelow) {
                        //sibling of nodeBelow, place element right above it
                        drop.relative = drop.relative.prev();
                        if (drop.node.is(drop.relative)) {
                            drop.relative = drop.relative.prev();
                        }
                    }

                    _static.fn.moveTree(drop.node, drop.parent, drop.relative);
                    // we need to scan both trees here as their data attributes could have been affected by the 
                    // move operation, most notably the even|odd style.
                    _static.fn.scanTree.apply(tree);
                    if (tree != $this) {
                        _static.fn.scanTree.apply($this);
                    }

                    // we must remove the current_drop because this method is used to handle both the general
                    // moving of nodes in the same tree and between trees. setting current_drop to null will thus
                    // prevent the onSortStop method from moving the node a second time.
                    data.settings.current_drop = null;
                    tree.data(_static.dataKey, data);

                    //close the node if its currently open
                    if (node.ftattr("state") == _static.states.open) {
                        _static.fn.toggleNode.apply(node);
                    }

                    $(node).addClass(_static.css.fade);

                    if (data.settings.onNodeDropped) {
                        data.settings.onNodeDropped.apply(node, [{
                            type: drop.type,
                            sender: $this,
                            fromTree: ui.sender ? ui.sender : null,
                            above: node.prevAll(_static.sel.childOf(node.ftattr("parent"))).first(),
                            below: node.nextAll(_static.sel.childOf(node.ftattr("parent"))).first(),
                            callback: function () {
                                PxPage.Fade();
                            }
                        }]);
                    }
                }
            }
        }
    },
    // The public interface for interacting with this plugin.
    api = {
        // initializes a new instance of the plugin
        init: function (options) {
            return this.each(function () {
                var settings = $.extend(true, {}, _static.defaults, options),
                    $this = $(this),
                    data = $this.data(_static.dataKey);

                if (!data) {
                    $this.data(_static.dataKey, {
                        target: $this,
                        settings: settings
                    });
                    data = $this.data(_static.dataKey);
                }

                // attach handlers and initialize the tree
                _static.fn.scanTree.apply($this);

                $this.find(_static.sel.nodeIcon).bind("click", function (event) {
                    event.stopPropagation();
                    _static.fn.toggleNode.apply($(this).parents(_static.sel.node), [event]);
                });

                $this.find(_static.sel.nodeTitle).unbind().bind("click", function (event) {
                    if (!$(event.target).hasClass('node-disabled')) {
                        _static.fn.clickNode.apply($(this).parents(_static.sel.node), [event]);
                    }
                });

                if (!PxPage.TouchEnabled()) {
                    $this.find(_static.sel.node).bind("mouseenter", function (event) {
                        _static.fn.mouseEnterNode.apply($(this), [event]);
                    });
                    $this.find(_static.sel.node).bind("mouseleave", function (event) {
                        _static.fn.mouseLeaveNode.apply($(this), [event]);
                    });
                }

                if (!settings.readOnly && !PxPage.TouchEnabled()) {
                    $this.sortable({
                        placeholder: _static.css.placeholder,
                        forcePlaceholderSize: true,
                        connectWith: _static.sel.tree,
                        receive: _static.fn.onNodeDropped,
                        delay: 250,
                        distance: 20,
                        containment: false,
                        cursor: "pointer",
                        cursorAt: { left: -15, top: -15 }, //$('.nav-category'), //settings.sortableInsideContainer ? $this : false,
                        cancel: settings.sortableItemsUndraggableSel,
                        helper: settings.sortableHelperHtml,
                        revert: true,
                        scroll: false
                    });

                    // the following events are what allow us to track potential drop scenarios in real-time
                    $this.bind("sortstart", _static.fn.onSortStart);
                    $this.bind("sortupdate", _static.fn.onSortStop);
                    $(_static.sel.tree).bind("mousemove", function (event) {
                        _static.fn.onNodeDragging.apply($this, [event]);
                    });
                }
                if (settings.showLastViewed) {
                    settings.LastViewed = null;
                    $this.find(_static.sel.nodeLink).unbind("click", _static.fn.clickNodeLink);
                    $this.find(_static.sel.nodeLink).bind("click", _static.fn.clickNodeLink);
                }
            });
        },
        // serializes the tree into a JSON array containing the state of each node.
        // if a node is given, then only that node is serialized.
        //
        // this = fauxtree
        // callback - if given, will be executed for each node: function(nodeElement, nodeObject)
        // node - node to serialize.
        serialize: function (callback, node, includeChildrenOfNode) {
            var $this = this.first(),
                allNodes = {},
                rootNodes = [],
                roots = $this.find(_static.sel.rootNodes);

            if (node && includeChildrenOfNode == null) {
                return _static.fn.getNodeData(node, callback);
            }
            else if (node && includeChildrenOfNode) {
                roots = node;

            }
            var data = $this.data(_static.dataKey);
            _static.defaults.filterByTreeId = data.settings.filterByTreeId;

            roots.each(function () {
                //we need to serialize each root and all of its descendants
                var node = $(this),
                    nodeData = _static.fn.getNodeData(node, callback);

                allNodes[nodeData.id] = nodeData;
                rootNodes.push(nodeData);


                _static.fn.applyToDescendants(node, function (parent) {
                    //every descendant of a root node must be serialized
                    var parentData = allNodes[parent.ftattr("id")],
                        nodeData = _static.fn.getNodeData($(this), callback);

                    allNodes[nodeData.id] = nodeData;
                    parentData.children.push(nodeData);
                });
            });

            return rootNodes;
        },
        // updates all nodes in the DOM with the state given
        //
        // state - if an array, assumed to be the state of an entire tree, otherwise, the state of a single node
        // callback - callback that allows caller to set custom data on each DOM element
        deserialize: function (state, callback) {
            if ($.isArray(state)) {
                _static.fn.traverseState(state, callback);
            }
            else {
                var array = [state];
                _static.fn.traverseState(array, callback);
            }
        },
        // makes the next node active.
        nextNode: function () {
            var active = $(_static.sel.activeNode),
                next,
                parent,
                parentId;

            if (active.length > 0) {
                next = active.next(_static.sel.node);

                if (next.length > 0) {
                    parentId = next.ftattr("parent");

                    if (parentId) {
                        parent = $(_static.sel.nodeById(parentId));

                        if (parent.length > 0 && parent.ftattr("state") == _static.states.closed) {
                            _static.fn.openNode.apply(parent);
                        }
                    }

                    _static.fn.clickNode.apply(next);
                }
            }
        },
        // makes the previous node active.
        previousNode: function () {
            var active = $(_static.sel.activeNode),
                prev,
                parent,
                parentId;

            if (active.length > 0) {
                prev = active.prev(_static.sel.node);

                if (prev.length > 0) {
                    parent = prev;
                    parentId = prev.ftattr("parent");

                    // expand all immediate ancestors
                    while (parentId) {
                        parent = $(_static.sel.nodeById(parentId));

                        if (parent && parent.ftattr("state") == _static.states.closed) {
                            _static.fn.openNode.apply(parent);
                        }
                        parentId = parent.ftattr("parent");
                    }

                    _static.fn.clickNode.apply(prev);
                }
            }
        },
        // adds a new node to the tree using the given state information.
        //
        // If there is an active node, then the given node is added as a child of
        // that node. If the active node is barren, then the given node becomes the
        // last child of the active node's parent.  If the active node has no parent,
        // then the node becomes the last item at the root level.
        //
        // state - state of the node to create
        // callback - callback function to allow callers to modify the resulting node
        //            in order to add custom properties and markup.
        // tree - the caller can optionally specify the tree that the user wants the
        //        node to be added to.
        //
        // returns: newNode, or null if a node couldn't be added.
        addNode: function (state, callback, givenTree) {

            var tree = givenTree ? givenTree : $(this).has(_static.sel.activeNode).first(),
                settings = $(tree).data(_static.dataKey).settings,
                active = tree.length > 0 ? tree.find(_static.sel.activeNode) : null,
                activeState = active ? _static.fn.getNodeData(active) : null,
                newNode = null;

            if (givenTree) {
                // we were given a tree that doesn't have any nodes, or any active nodes so
                // the new node must be appended to the tree

                state.parentId = state.parentId ? state.parentId : "";
                state.level = state.level ? state.level : _static.minLevel;
                newNode = _static.fn.createNode(state, settings.showExpandIcon);

                if (state.insertBefore && state.insertBefore.length > 0) {
                    state.insertBefore.before(newNode);
                }
                else {
                    var parentNode = $(_static.sel.nodeById(state.parentId));
                    if (parentNode.length > 0) {
                        _static.fn.lastDescendantOrSelf(parentNode).after(newNode);
                    } else {
                        //if the parent doesn't exist, add to root of the tree

                        tree.append(newNode);
                    }
                }
                _static.fn.setNodeData(state, callback, newNode);
            }
            else if (!tree || (tree.length == 0)) {
                // there is no tree, so we can't do anything
                return newNode;
            }
            else if (activeState && !activeState.parentId && activeState.parentId !== "") {
                // active node has no parent, so newNode should be the last node in the
                // tree.
                state.level = _static.minLevel;
                state.parentId = "";
                newNode = _static.fn.createNode(state);
                tree.append(newNode);
                _static.fn.setNodeData(state, callback, newNode);
            }
            else if (activeState && activeState.state === _static.states.barren) {
                // active node can't have children, so we make the newNode be the last sibling
                // of the active node.
                state.level = activeState.level;
                state.parentId = activeState.parentId;
                newNode = _static.fn.createNode(state);

                _static.fn.lastDescendantOrSelf($(_static.sel.nodeById(state.parentId))).after(newNode);
                _static.fn.setNodeData(state, callback, newNode);
            }
            else if (activeState) {
                // active node must be able to have children, so the newNode becomes the last
                // child of the active node.

                state.parentId = state.parentId ? state.parentId : activeState.id;
                state.level = state.level ? state.level : activeState.level;
                newNode = _static.fn.createNode(state);

                if (state.insertBefore) {
                    state.insertBefore.before(newNode);
                }
                else {
                    var parentNode = $(_static.sel.nodeById(state.parentId));
                    if (parentNode.length > 0) {
                        _static.fn.lastDescendantOrSelf(parentNode).after(newNode);
                    } else {
                        //if the parent doesn't exist, add to root of the tree
                        tree.append(newNode);
                    }
                }
                _static.fn.setNodeData(state, callback, newNode);
            }

            if (newNode) {
                _static.fn.scanTree.apply(tree);
            }

            if (settings.showLastViewed) {
                $(_static.sel.nodeLink).unbind("click", _static.fn.clickNodeLink);
                $(_static.sel.nodeLink).bind("click", _static.fn.clickNodeLink);
            }

            return newNode;
        },
        // toggles whether a node is open or closed
        toggleNode: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
            if (target.length > 0) {
                _static.fn.toggleNode.apply(target);
            }
        },

        // opens a node and sets to active
        setActiveNode: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
            if (target.length > 0) {
                _static.fn.clickNode.apply(target);
            }
        },

        // removes the node and all of it's descendants from the tree
        removeNode: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node,
                tree = target.parents("ul");

            if (target.length > 0) {
                _static.fn.applyToDescendants(target, function () {
                    $(this).remove();
                });

                target.remove();
                _static.fn.scanTree.apply(tree);
            }
        },

        // removes all of a nodes descendants from the tree
        removeChildren: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node,
                tree = target.parents("ul");

            if (target.length > 0) {
                _static.fn.applyToDescendants(target, function () {
                    $(this).remove();
                });

                //target.remove();
                _static.fn.scanTree.apply(tree);
            }
        },
        // returns the tree that the node is a part of
        getTree: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node,
                tree = target.parents("ul");

            return tree;
        },
        // returns the node if an id is given
        getNode: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;

            if (target == null || target.length == 0) {
                //get node from all trees
                var filterNodesByTree = _static.defaults.filterNodesByTree;
                _static.defaults.filterNodesByTree = false;
                target = $(_static.sel.nodeById(node));
                _static.defaults.filterNodesByTree = filterNodesByTree;
            }

            return target;
        },
        // returns the node if an id is given
        getNodeFromTree: function (tree, node) {
            var nodeSelect = _static.sel.nodeById(node);
            var target = typeof (node) === "string" ? $('#' + tree + ' ' + _static.sel.nodeById(node)) : node;
            return target;
        },
        // returns the next node
        getNextNode: function (hideClass, hideClass2, folderExpandClass) {
            var activeNode = $(_static.sel.activeNode);
            if (activeNode.length != 0) {
                var next = activeNode.next(_static.sel.node);
                //var id = next.ftattr("id");
                //$.fn.fauxtree("setActiveNode", id);

                // showing only non-hidden nodes
                if (hideClass) {
                    while (next.length > 0) {
                        if (next.hasClass(hideClass)) {
                            if (folderExpandClass && next.hasClass(folderExpandClass) && (next.ftattr("state") == "closed")) {
                                _static.fn.toggleNode.apply(next);
                            }
                            next = next.next(_static.sel.node);
                        } else {
                            if (next.hasClass(hideClass2)) {
                                next = next.next(_static.sel.node);
                            } else {
                                break;
                            }
                        }
                    }
                }
                if (next.length == 0) {
                    return;
                }

                $(_static.sel.node).removeClass(_static.css.activeNode);
                next.addClass(_static.css.activeNode);
                return next;
            }
        },
        // returns the previous node
        getPreviousNode: function (hideClass, hideClass2) {
            var activeNode = $(_static.sel.activeNode);
            if (activeNode.length > 0) {
                var prev = activeNode.prev(_static.sel.node);
                // showing only non-hidden nodes
                if (hideClass) {
                    while (prev.length > 0) {
                        if (prev.hasClass(hideClass)) {
                            prev = prev.prev(_static.sel.node);
                        } else {
                            if (prev.hasClass(hideClass2)) {
                                prev = prev.prev(_static.sel.node);
                            } else {
                                break;
                            }
                        }
                    }
                }
                if (prev.length == 0) {
                    return;
                }
                $(_static.sel.node).removeClass(_static.css.activeNode);
                prev.addClass(_static.css.activeNode);
                return prev;
            }
        },
        // returns wether the node has next node
        hasNextNode: function (hideClass, hideClass2) {
            var activeNode = $(_static.sel.activeNode);
            if (activeNode.length != 0) {
                var next = activeNode.next(_static.sel.node);
                if (hideClass) {
                    while (next.length > 0) {
                        if (next.hasClass(hideClass)) {
                            next = next.next(_static.sel.node);
                        } else {
                            if (next.hasClass(hideClass2)) {
                                next = next.next(_static.sel.node);
                            } else {
                                return true;
                            }
                        }
                    }
                }
                if (next.length == 0) {
                    return null;
                }
                else {
                    return true;
                }
            }
            return null;
        },
        // returns the previous node
        hasPreviousNode: function (hideClass, hideClass2) {
            var activeNode = $(_static.sel.activeNode);
            if (activeNode.length > 0) {
                var prev = activeNode.prev(_static.sel.node);
                if (hideClass) {
                    while (prev.length > 0) {
                        if (prev.hasClass(hideClass)) {
                            prev = prev.prev(_static.sel.node);
                        } else {
                            if (prev.hasClass(hideClass2)) {
                                prev = prev.prev(_static.sel.node);
                            } else {
                                return true;
                            }
                        }
                    }
                }
                if (prev.length == 0) {
                    return null;
                } else {
                    return true;
                }
            }
            return null;
        },
        // gets the active node, if there is one
        getActiveNode: function () {
            return $(_static.sel.activeNode);
        },
        // gets the number of nodes in the tree
        getNodeCount: function () {
            var tree = $(this).first();

            return tree.find(_static.sel.node).length;
        },
        // hides all nodes except for node and its descendants
        hideAllExcept: function (node) {
            $(_static.sel.rootNodes).each(function () {
                _static.fn.setTreeVisibility($(this), false);
                $(this).hide();
            });

            node.show();
            _static.fn.setTreeVisibility(node, true);
        },
        // close all nodes except for node and its parents
        closeNode: function (node) {
            _static.fn.applyToSiblings(node, function () {
                _static.fn.closeNode.apply($(this));
            });
        },
        // shows all nodes
        showAll: function (node) {
            $(_static.sel.rootNodes).each(function () {
                _static.fn.setTreeVisibility($(this), true);
                $(this).show();
            });
        },
        // destroys the plugin instances
        destroy: function () {
            return this.each(function () {
                $(this).unbind(_static.bindSuffix);
            });
        },

        showTree: function (node) {
            _static.fn.setTreeVisibility($(node), true);
            var tree = _static.fn.getTree(node);
            tree.show();
        },

        applyToAncestors: function (node, callback) {
            _static.fn.applyToAncestors(node, callback, node);
        },

        getAllParents: function (node) {
            return _static.fn.getAllParents(node);
        },

        applyToDescendants: function (node, callback) {
            _static.fn.applyToDescendants(node, callback);
        },

        setParentEnable: function (node, callback) {
            _static.fn.applyToImmediateAncestor(node, callback);
        },
        sortTree: function (comparator, recursive) {
            _static.fn.sortTree($(this), comparator, recursive);
        },
        applyToSiblings: function (node, callback) {
            _static.fn.applyToSiblings(node, callback);
        },
        scanTree: function (tree) {
            _static.fn.scanTree.apply(tree);
        },
        scanNode: function (node) {
            _static.fn.scanTree.apply($(this), [node]);
        },
        moveTree: function (node, newParent, relative) {
            _static.fn.moveTree(node, newParent, relative);
        },

        hasChildren: function (node) {
            return _static.fn.hasChildren(node);
        },
        //item has children that have not been loaded
        isNotLoaded: function (node) {
            //return false if jquery object contains nothing
            return node && node.length > 0 && !_static.fn.hasChildren(node) && node.ftattr("has-children") != "false";
        },
        topAncestorOrSelf: function (node) {
            return _static.fn.topAncestorOrSelf(node);
        },
        isDragging: function () {
            var settings = this.data(_static.dataKey).settings;

            return settings.is_dragging == true;
        },
        getChildren: function (n) {
            var node = $.fn.fauxtree("getNode", n);
            return _static.fn.getChildren(node);
        },

        getLastSequence: function (n) {
            var node = $.fn.fauxtree("getNode", n);
            return _static.fn.getLastSequence(node);
        },
        openNode: function (node) {
            return _static.fn.openNode.apply(node);
        },

        getOldestAncestors: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
            var retval = target;

            while (target != null && target.length > 0) {
                retval = target;
                var parentId = target.ftattr("parent");
                target = $(_static.sel.nodeById(parentId))
            }

            return retval;
        },

        openAllParentNodes: function (node) {
            var target = typeof (node) === "string" ? $(_static.sel.nodeById(node)) : node;
            var parents = new Array();

            while (target != null && target.length > 0) {
                var parentId = target.ftattr("parent");
                target = $(_static.sel.nodeById(parentId));
                if (target.length > 0) {
                    parents.push($(_static.sel.nodeById(parentId)));
                }
            }

            parents = parents.reverse();

            for (var i = 0; i < parents.length; i++) {
                var node = parents[i];
                _static.fn.openNode.apply(node);
            }
        },

        closeAllRootNodes: function () {
            var $this = $(this);
            var nodes = $this.find(_static.sel.rootNodes);

            nodes.each(function () {
                if ($(this).ftattr("state") == _static.states.open) {
                    _static.fn.closeNode.apply($(this));
                }
            });
        },

        setFilterNodesByTree: function (value) {
            _static.defaults.filterNodesByTree = value;
        },

        setTreeId: function (value) {
            _static.defaults.filterByTreeId = value;
        },

        getNodeData: function (node, callback) {
            return _static.fn.getNodeData(node, callback);
        }
    };

    // Simple extension to ease getting the data attributes for this
    // plugin.
    $.fn.ftattr = function (name, value) {
        var target = this.first(),
            aName = _static.dataAttrPrefix + name;

        if (value != null)
            target.attr(aName, value);
        return target.attr(aName);
    };

    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.fauxtree = function (method) {
        var rtn = undefined;
        if (api[method]) {
            rtn = api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            rtn = api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
        return rtn;
    };

    $.fn.fauxtreeObj = function() {
        return {
            _static: _static,
            api: api
        };
    };
} (jQuery))