// Defines a singleton object that coordinates all client-side behavior of the AssignmentCenter page.
(function ($) {
    // static data
    var _static = {
        defaults: {
            mode: "syllabus",
            autoFocus: false
        },
        settings: {},
        modes: {
            assignmentPad: "assignment_pad",
            syllabus: "syllabus",
            noSyllabus: "nosyllabus"
        },
        // commonly used CSS classes
        css: {
        },
        // commonly used jQuery selectors
        sel: {
            leftNav: ".unit-subitems-wrapper-dialog",  // ".nav",
            viewMode: ".selViewMode",
            actionMenu: "#assignmentmenu .action-menu",
            autoFocusReturn: ".auto-focus-return"
        },
        // static functions
        fn: {
            // when the view mode is changed in the drop down we need to redraw the left nav
            onViewModeChanged: function (event) {
                var mode = $(event.target).val();
                $(_static.sel.leftNav).block({ message: null });
                $.get(PxPage.Routes.load_ac_navigation, { mode: mode }, function (response) {
                    $(_static.sel.leftNav).replaceWith(response);
                    $(_static.sel.leftNav).Navigation({ readOnly: (mode === _static.modes.assignmentPad) });
                });
            },
            // when something adds existing content to the assignment center we need to track the
            // new item
            onAddExistingContent: function (event, id, categoryId) {
                if (!id) {
                    return;
                }

                var args = {
                    nodeState: {
                        title: "",
                        id: id,
                        state: "barren",
                        visible: true
                    },
                    categoryId: categoryId
                };

                if (id.toLowerCase().indexOf("module_") >= 0) {
                    args.nodeState.state = "closed";
                }

                $(_static.sel.leftNav).FacePlateDialog("addItem", args);
                $(PxPage.switchboard).trigger("contentcreated", [id, true]);

            },

            // when a node is clicked, we want to update the menu
            onNodeClick: function (node, category) {
                alert("FacePlateDialogHost onNodeClick");
                _static.fn.onUpdateActionMenu(node);

                if (_static.settings.autoFocus) {
                    $(_static.sel.leftNav).FacePlateDialog("hideAllExcept", category, node);
                }
            },

            // when a node is hovered, we want to show the tool tip

            onNodeMouseEntered: function (node, category) {
                ContentWidget.ShowItemHover = true;
                PxPage.log('onNodeMouseEntered');

                ContentWidget.ItemHoverTimer = setTimeout(function () {
                    if (!ContentWidget.ItemHoverTimer) {
                        return;
                    }

                    $('.qtip-content').hide()
                    clearTimeout(ContentWidget.ItemHoverTimer);
                    ContentWidget.ItemHoverTimer = null;
                    var nodeId = node.ftattr("id");
                    var tooltip = node.find(".tooltip").first();
                    var tocItem = tooltip.siblings('#tocItemId').val();

                    var assigned = node.find('.assignment-due-date').first();
                    var isHiddenOnly = false;
                    if (assigned.length == 0 || assigned.html() == "") {
                        if (node.attr('data-ac-hidden') == true) {
                            isHiddenOnly = true;
                        }
                        else {
                            $('.qtip-content').hide();
                            return;
                        }
                    }

                    var $item = $("#" + nodeId);
                    if (tooltip.length) {
                        var self = node.find(".faux-tree-node-title").first();
                        var itemId = nodeId;
                        self.qtip({
                            content: { text: 'Loading' },
                            show: { delay: 2000, ready: true },
                            hide: { when: { event: 'mouseout'} },
                            position: { corner: { tooltip: "topLeft", target: "topRight"} },
                            style: { padding: 0, width: { min: 178, max: 300} },
                            api: {
                                onRender: function () {
                                    var current = this;

                                    if (isHiddenOnly) {
                                        current.updateContent('Hidden from students', false);
                                    }
                                    else {
                                        jQuery.post(
                                            PxPage.Routes.show_tocitem_tooltip,
                                            { 'assignmentId': itemId, 'isAssignmentCenter': false },
                                            function (theResponse) {   // Callback after the ajax request is complete
                                                if (jQuery.trim(theResponse) == '') {
                                                    current.updateContent('no tool tip', false);
                                                }
                                                else {
                                                    current.updateContent(theResponse, false);   // Replace current content
                                                }

                                                $('.qtip-content').show()
                                                return true;
                                            }
                                        );
                                    }
                                    PxPage.log('render tooltip');
                                },
                                onShow: function () {
                                    PxPage.log('removing timer id ' + ContentWidget.ItemHoverTimer);
                                    clearTimeout(ContentWidget.ItemHoverTimer);
                                    ContentWidget.ItemHoverTimer = null;
                                }
                            }
                        }).qtip("show");
                    }
                }, 3000); // this has to be greater than the delay for qtip
            },
            // when autofocus is returned to the starting view
            onAutoFocusReturn: function (event) {
                event.preventDefault();

                $(_static.sel.leftNav).FacePlateDialog("showAll");
            },
            // updates the action menu
            onUpdateActionMenu: function (item) {
                var menu = $(_static.sel.actionMenu),
                    config = {},
                    onAction,
                    addNew = {
                        name: "ac-add-new",
                        text: "Create new content"
                    },
                    addExisting = {
                        name: "ac-add-existing",
                        text: "Add existing content"
                    },
                    removeNode = {
                        name: "ac-remove-node",
                        text: "Remove item"
                    },
                    addCategory = {
                        name: "ac-add-category",
                        text: "Add Category"
                    },
                    editCategory = {
                        name: "ac-edit-category",
                        text: "Edit Category"
                    },
                    autoFocus = {
                        name: "ac-auto-focus",
                        text: "Enable Autofocus"
                    },
                    category = item && item.is(_static.sel.category) ? $(_static.sel.leftNav).FacePlateDialog("getCategory", item.fpnattr("id"), true) : null;

                if (menu.length > 0) {
                    config.id = "acactions";
                    config.options = [];

                    config.options.push(addNew);
                    config.options.push(addExisting);
                    config.options.push(removeNode);

                    if (category) {
                        config.options.push(editCategory);
                    }

                    config.options.push(addCategory);

                    if (_static.settings.autoFocus) {
                        autoFocus.text = "Disable Autofocus";
                    }

                    config.options.push(autoFocus);

                    onAction = function (event, action) {
                        event.preventDefault();

                        switch (action) {
                            case addNew.name:
                                PxPage.Loading();
                                FacePlateHost.createContent("create");
                                break;

                            case addExisting.name:
                                PxPage.Loading();
                                FacePlateHost.createContent("existing");
                                break;

                            case removeNode.name:
                                $(_static.sel.leftNav).FacePlateDialog("removeItem");
                                break;

                            case addCategory.name:
                                PxSyllabusCategory.ShowEditSyllabus('');
                                break;

                            case editCategory.name:
                                PxSyllabusCategory.ShowEditSyllabus(category.fpnattr("id"));
                                break;

                            case autoFocus.name:
                                _static.settings.autoFocus = !_static.settings.autoFocus;

                                if (_static.settings.autoFocus) {
                                    $(_static.sel.autoFocusReturn).show();
                                }
                                else {
                                    $(_static.sel.autoFocusReturn).hide();
                                    $(_static.sel.leftNav).FacePlateDialog("showAll");
                                }

                                _static.fn.onUpdateActionMenu();

                                break;
                        }
                    };

                    menu.ActionWidget({ menu: config, action: onAction });
                }
            },
            // handles what happens when an item is selected and needs to be created
            onItemFromTemplate: function (data) {
                PxContentTemplates.CreateItemFromTemplate(data.templateId, function (item) {
                    var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&isMultipartLessons=true&includeNavigation=false";
                    $.get(itemUrl, null, function (response) {
                        $(response).PxNonModal({
                            title: 'Create New',
                            onCompleted: function () {
                                var args = {
                                    nodeState: {
                                        title: "New " + data.type,
                                        id: item.id,
                                        state: "barren",
                                        visible: true
                                    },
                                    categoryId: data.filterId
                                },

                                newNode = $(_static.sel.leftNav).FacePlateDialog("addItem", args),
                                parentId = newNode.ftattr("parent"),
                                filterId = parentId,
                                type = data.templateId;

                                if (!parentId || parentId === "") {
                                    parentId = $(_static.sel.leftNav).FacePlateDialog("getCategory", newNode.ftattr("id")).fpnattr("id");
                                    filterId = parentId;
                                }

                                // change the parent id from PX_TEMP parent id, so that when clicking
                                // save button we can create a new item with this parent id.
                                $('#nonmodal-content').find('#Content_DefaultCategoryParentId').val(parentId);
                                $('#nonmodal-content').find('#Content_ParentId').val(parentId);
                                $('#nonmodal-content').find('#Content_Title').val('Untitled ' + $('#nonmodal-content').find('#Content_Title').val());

                                if (parentId == "PX_MULTIPART_LESSONS") {
                                    $('#nonmodal-content').find('#Content_SyllabusFilter').val(filterId);
                                }
                                else {
                                    $('#nonmodal-content').find('#Content_SyllabusFilter').val(parentId);
                                }

                                $('#Content_SourceTemplateId').val(type);

                            }
                        });
                        PxPage.Update();
                        PxPage.Loaded();
                    });
                });
            }
        }
    };

    // FacePlateHost is a singleton that lives on the window
    window.FacePlateDialogHost = {
        // initializes the sub-components like the left nav
        init: function (options) {
            $.extend(true, _static.settings, _static.defaults, options);
            // we need to initialize the content widget
            if (ContentWidget) {
                ContentWidget.Init();
            }
                    },
        // opens the AssignmentCenterContentDialog in the given mode
        createContent: function (mode) {
            $.fn.AssignmentCenterContentDialog("open", {
                mode: mode,
                onItemFromTemplate: _static.fn.onItemFromTemplate
            });
        },
        // called when an item is saved and we need to initialize the dialog to show the templates again
        onShowTemplates: function () {
            $.fn.AssignmentCenterContentDialog("open", {
                mode: "showmore",
                onItemFromTemplate: _static.fn.onItemFromTemplate
            });
        },
        // called when content is "saved"
        onSaveContent: function (response) {
            ContentWidget.ContentCreated(response, null, null, 'modal', PxTemplates.TemplateContexts.FacePlate.value, FacePlateHost.onShowTemplates);
        },
        // called when content is "saved and opened"
        onSaveAndOpenContent: function (response) {
            ContentWidget.ContentCreated(response, null, null, 'normal', PxTemplates.TemplateContexts.FacePlate.value);

        },
        // called when content is "saved and opened"
        onCancelAndShowCreate: function (id) {
            PxPage.CloseCreateNewScreen({ reason: 'cancel', id: id })
            ContentWidget.ContentCreated('cancel', null, null, 'modal', PxTemplates.TemplateContexts.FacePlate.value, FacePlateHost.onShowTemplates);
            return false;
        },
        createSyllabus: function () {
            PxSyllabusCategory.ShowSyllabusSetUp();
        },
        contentRemoved: function (event, itemId) {
            jQuery(PxPage.switchboard).trigger('contentremoved', [itemId]);
            PxSyllabusCategory.ShowActionLink(event);
        },
        addExistingContent: function (event, itemId) {
            jQuery(PxPage.switchboard).trigger('existingcontentadded', [itemId]);
            PxSyllabusCategory.ShowActionLink(event);
        }
    };
} (jQuery))