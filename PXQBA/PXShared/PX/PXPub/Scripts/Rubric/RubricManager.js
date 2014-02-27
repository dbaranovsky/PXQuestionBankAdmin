// RubricManager
//
// This plugin is responsible for the client-side behaviors of the
// Rubric Management.
//
//
(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "RubricManager",
        dataKey: "RubricManager",
        bindSuffix: ".RubricManager",
        // default option values
        defaults: {
            expandDelay: 1500,
            readOnly: false,
            emptyTreeText: "There are no rubrics in this category."
        },
        settings: {},
        // class names used by the plug-in
        css: {
            emptyTree: "empty-tree"
        },
        // jquery selectors for commonly accessed elementsrubricTitle
        sel: {
            rubricItem: ".item-node",
            rubricLeftItemChechBox: "#allRubricsContainer .item-node input:checkbox",
            rubricRightItemChechBox: "#courseRubricsContainer .item-node input:checkbox",
            addRubricsButton: "#lnkAddRubrics",
            removeRubricsButton: ".remove-rubric",
            keepRubricsConfirmButton: ".keep-rubric",
            previewRubricButton: ".preview-rubric",
            editRubricButton: ".edit-rubric",
            deleteRubricButton: '.delete-rubric',
            submitRubricButton: ".submit-rubric",
            saveRubricButton: ".save-rubric",
            saveAsRubricButton: ".save-as-rubric",
            cancelRubricButton: ".cancel-rubric-edit",
            cancelSaveRubricButton: ".cancel-save-rubric",
            createRubricButton: ".create-rubric",
            viewRubricAlignmentsButton: ".view-rubric-alignments",
            editRubricAlignmentButton: ".edit-align-rubric",
            alignRubricButton: ".align-rubric",
            cancelAlignRubricButton: ".cancel-align-rubric",
            removeAlignedRubricButton: "#aligned-rubric .close",
            removeObjectiveButton: "#objectives-container .removeAlignment",
            assessmentContainer: ".assessment-container",
            selectedRubrics: "#allRubricsContainer input:checked",
            selectedCourseRubrics: "#courseRubricsContainer input:checked",
            allRubricNodes: "#allRubricsContainer li.item-node .title",
            courseRubricsList: "#courseRubricsContainer #activeRubrics",
            saveAsRubricDialog: "#save-as-rubric-dialog",
            removeRubricDialog: "#remove-rubric-dialog",
            deleteRubricDialog: '#delete-rubric-dialog',
            rubricDialogError: "#save-as-rubric-dialog #error-message",
            rubricTitleInput: "#save-as-rubric-dialog #txtTitle",
            rubricCreateTitleInput: ".rubric-title",
            rubricTitleBackUp: "#currentTitle",
            rubricCreateTitleError: "#error-message-title",
            rubricAlignmentSort: "#rubric-alignments .sort-option",
            rubricById: function (id) {
                return '.item-node[rel="' + id + '"]';
            },
            rubricByTitle: function (title) {
                return '.item-node .title:[contains="' + title + '"]';
            }
        },
        // private functions
        fn: {
            rubric_sort: function (a, b) {
                return ($(b).text()) < ($(a).text());
            },
            sortList: function (list) {
                list.find('li').sort(_static.fn.rubric_sort).appendTo(list);
            },
            reArrangeAlternateColor: function (list) {
                list.find('li').removeClass('even');
                list.find('li').removeClass('odd');
                list.find('li').each(function (index) {
                    $(this).addClass((index % 2 == 0) ? 'even' : 'odd');
                });
            },

            // grabs ids for all checked rubrics in publisher/myrubrics list and updates to course rubrics.
            onAddCourseRubricsClick: function (event) {
                var selectedRubrics = $(_static.sel.selectedRubrics),
                    selectedCourseRubrics = $(_static.sel.selectedCourseRubrics),
                    rubricList = [];

                if (selectedRubrics.length) {
                    PxPage.Loading('assessment-content');

                    selectedRubrics.each(function (index) {
                        $(this).attr('checked', false);
                        var id = $(this).val();
                        if ($(_static.sel.courseRubricsList).find(_static.sel.rubricById(id)).length == 0) {
                            rubricList.push(id);
                            var newRubric = $(this).closest('li').clone().addClass('fade-effect');
                            newRubric.find('.links').hide();
                            newRubric.appendTo(_static.sel.courseRubricsList);
                        }
                    });

                    var selectedLiRubrics = $(selectedRubrics).closest('li.item-node');
                    selectedLiRubrics.addClass("fade-effect");


                    if (rubricList.length > 0) {
                        _static.fn.sortList($(_static.sel.courseRubricsList));
                        _static.fn.reArrangeAlternateColor($(_static.sel.courseRubricsList));
                        var json = {
                            SelectedRubrics: rubricList
                        };

                        json = JSON.stringify(rubricList);
                        $.post(PxPage.Routes.add_course_rubrics + "?data=" + json, null, function (response) {
                            PxPage.Loaded('assessment-content');
                            PxPage.Fade();
                        });
                    }
                    else {
                        PxPage.Loaded('assessment-content');
                        PxPage.Fade();
                    }
                    $(_static.sel.addRubricsButton).removeClass("buttonSelectedColor");
                }
            },

            onRemoveRubricClick: function () {
                $(_static.sel.removeRubricDialog).dialog('open');
            },

            onKeepCourseRubricsClick: function (event) {
                PxPage.Loaded('assessment-content');
                $(_static.sel.removeRubricDialog).dialog('close');
            },

            // grabs ids for all checked rubrics in course rubrics and removes from list.
            onRemoveCourseRubricsClick: function (event) {
                var selectedRubrics = $(_static.sel.selectedRubrics),
                    selectedCourseRubrics = $(_static.sel.selectedCourseRubrics),
                    rubricList = [],
                    $this = $(this),
                    isConfirmed = false,
                    fromDialog = $this.closest(_static.sel.removeRubricDialog).length;

                if (selectedCourseRubrics.length) {
                    if (fromDialog) {
                        isConfirmed = true;
                    }
                    else {
                        isConfirmed = confirm("Are you sure you want to remove the selected rubrics?");
                    }
                    if (isConfirmed) {
                        PxPage.Loading('assessment-content');

                        selectedCourseRubrics.each(function (index) {
                            var id = $(this).val();
                            rubricList[index] = id;
                        });


                        var json = { SelectedRubrics: rubricList };
                        json = JSON.stringify(rubricList);
                        var args = { data: json, validate: true };

                        if ($this.closest(_static.sel.removeRubricDialog).length) {
                            args.validate = false;
                        }

                        $.post(PxPage.Routes.remove_course_rubrics, args, function (response) {
                            if (response.status != "fail") {
                                selectedCourseRubrics.each(function (index) {
                                    $(this).closest('li').remove();
                                });

                                PxPage.Loaded('assessment-content');

                                if ($this.closest(_static.sel.removeRubricDialog).length) {
                                    $(_static.sel.removeRubricDialog).dialog('close');
                                }
                                _static.fn.reArrangeAlternateColor($(_static.sel.courseRubricsList));
                            }
                            else {
                                $(_static.sel.removeRubricDialog).dialog({ width: 520, height: 160, minWidth: 520, minHeight: 120, modal: true, draggable: false, closeOnEscape: false, dialogClass: 'px-dialog-notitle',
                                    open: function (event, ui) {
                                        $(_static.sel.removeRubricDialog).find(".ui-dialog-titlebar").attr("style", "display:none;");
                                    }
                                });
                            }
                        });
                        $(_static.sel.removeRubricsButton).removeClass("buttonSelectedColor");
                    }
                }
            },

            onRubricAlignmentSortClick: function () {
                var val = $(this).text();
                PxPage.Loading("alignments-content-wrapper");
                if (val == "Rubric") {
                    $.post(PxPage.Routes.rubric_alignments_by_rubric, null, function (response) {
                        $('#alignments-content-wrapper').html(response);
                        PxPage.Loaded("alignments-content-wrapper");
                    });
                }
                else {
                    $.post(PxPage.Routes.rubric_alignments_by_assignment, null, function (response) {
                        $('#alignments-content-wrapper').html(response);
                        PxPage.Loaded("alignments-content-wrapper");
                    });
                }
            },

            showEditRubricLink: function (event) {
                var $this = $(this);
                var parentNode = $this.closest('li.item-node');
                parentNode.addClass('liHoverColor');
                parentNode.find('.edit-rubric').show();
                parentNode.find('.delete-rubric').show();
            },
            hideEditRubricLink: function (event) {
                var $this = $(this);
                var parentNode = $this.closest('li.item-node');
                parentNode.removeClass('liHoverColor');
                parentNode.find('.edit-rubric').hide();
                parentNode.find('.delete-rubric').hide();
            },

            onLeftItemNodeCheckboxChange: function (event) {
                event.stopImmediatePropagation();
                var singleItemSelected = false;
                $('#allRubricsContainer').each(function () {
                    $(this).find('li').each(function () {
                        var current = $(this);
                        var checked = current.find('input:checkbox').is(':checked');
                        if (checked) {
                            singleItemSelected = true;
                            return false;
                        }
                    });
                });
                if (singleItemSelected) {
                    $(_static.sel.addRubricsButton).addClass("buttonSelectedColor");
                }
                else {
                    $(_static.sel.addRubricsButton).removeClass("buttonSelectedColor");
                }
            },

            onRightItemNodeCheckboxChange: function (event) {
                event.stopImmediatePropagation();
                var singleItemSelected = false;
                $('#courseRubricsContainer').each(function () {
                    $(this).find('li').each(function () {
                        var current = $(this);
                        var checked = current.find('input:checkbox').is(':checked');
                        if (checked) {
                            singleItemSelected = true;
                            return false;
                        }
                    });
                });
                if (singleItemSelected) {
                    $(_static.sel.removeRubricsButton).addClass("buttonSelectedColor");
                }
                else {
                    $(_static.sel.removeRubricsButton).removeClass("buttonSelectedColor");
                }
            },


            onPreviewRubricClick: function (event) {
                // if the check box or edit button was clicked, the preview modal should pop up
                if (event.target.tagName == 'INPUT' || event.target.tagName == 'A') return;

                var $this = $(this);
                var parentNode = $this.closest('li.item-node');
                var id = parentNode.attr('rel');

                // the class needs to be available before the preview can be displayed
                if (!parentNode.hasClass('preview-rubric')) return;

                _static.fn.ShowRubricDialog(PxPage.Routes.preview_rubric + "?id=" + id, true, { title: "Preview Rubric", dialogClass: 'px-dialog', height: 600, width: 700 });
            },

            onEditRubricClick: function () {
                var $this = $(this);
                var id = $this.closest('li.item-node').attr('rel');
                var titleName = $this.text() + ' Rubric';
                _static.fn.ShowRubricDialog(PxPage.Routes.edit_rubric + "?id=" + id, true, { width: '96%', height: 650, title: titleName, dialogClass: 'px-dialog-title' });
            },

            onDeleteRubricClick: function () {
                var $this = $(this);
                var node = $this.closest('li.item-node');
                var id = node.attr('rel');
                $(_static.sel.deleteRubricDialog).dialog({
                    resizable: false,
                    height: 180,
                    width: 500,
                    modal: true,
                    buttons: {
                        'Delete': function () {
                            if (!id || id == '')
                                return false;

                            PxPage.Loading();

                            $.post(PxPage.Routes.delete_rubric, { id: id }, function (response) {
                                $(node).hide();
                                $(_static.sel.courseRubricsList).find(_static.sel.rubricById(id)).hide();
                                PxPage.Loaded();
                            });

                            $(this).dialog("close");

                        },

                        "Don't Delete": function () {
                            $(this).dialog("close");
                        }
                    }

                });

            },

            onCreateRubricClick: function () {
                _static.fn.ShowRubricDialog(PxPage.Routes.edit_rubric, true, { width: '96%', title: 'Create Rubric', dialogClass: 'px-dialog-title' });
            },

            onViewRubricAlignmentsClick: function () {
                _static.fn.ShowRubricDialog(PxPage.Routes.rubric_alignments, false, { title: 'View Rubric Alignments', dialogClass: 'px-dialog-title', width: 750, height: 600 });
            },

            onCancelRubricClick: function () {
                var $this = $(this);
                $this.parents('#px-dialog').dialog("close");
            },

            onCancelSaveRubricClick: function () {
                $(_static.sel.saveAsRubricDialog).dialog('close');
                $(_static.sel.rubricDialogError).hide();
                $(_static.sel.saveRubricButton).removeAttr("disabled");
            },

            onSubmitRubricClick: function () {
                var $this = $(this),
                    lo = "";

                PxRubricBuilder.SaveRubricText();

                if (PxPage.ValidateTitle(true)) {

                    $("#objectives-container li").each(function () {
                        var current = $(this);
                        var isFake = current.hasClass('fakeNode');
                        if (!isFake) {
                            lo += current.attr("data-ft-id") + '|';
                        }
                    });

                    var rubric =
                        {
                            Title: $('.rubric-title').val(),
                            Description: $('.rubric-desc').val(),
                            IsBeingEdited: $('.rubric-edited').val(),
                            Objectives: lo,
                            Id: $('.item-id').val(),
                            Rubric: PxRubricBuilder.GetRubricData()
                        };

                    if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                        rubric.Id = '';
                    }

                    PxPage.Loading('ui-dialog', true);

                    $.ajax({
                        url: PxPage.Routes.save_rubric_data,
                        type: 'POST',
                        dataType: 'json',
                        data: JSON.stringify(rubric),
                        contentType: 'application/json; charset=utf-8',
                        complete: function (response) {
                            var text = rubric.Title;
                            var val = response.responseText;

                            if ($('#fsRubric #selRubric').length) {
                                if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                                    $('#fsRubric #selRubric').append(
                                $('<option></option>').val(val).html(text)
                                );

                                    $('#fsRubric #selRubric').val(val);
                                }
                            }
                            else {
                                PxPage.Loaded('ui-dialog', true);
                                $this.parents('#px-dialog').dialog("close");
                                $('#assessment-modes #newRubricCreated').val(val);
                                $('#assessment-modes #RubricsItem a').click();
                            }
                        }
                    });
                }
            },

            onSaveRubricClick: function () {
                var $this = $(this);
                if (PxPage.ValidateTitle(true)) {

                    var isValid = _static.fn.ValidateTitle();
                    if (!isValid) return;

                    var rubric =
                        {
                            Title: $('#txtTitle').val(),
                            Description: $('.rubric-desc').val(),
                            IsBeingEdited: $('.rubric-edited').val(),
                            Id: $('.item-id').val(),
                            Rubric: PxRubricBuilder.GetRubricData()
                        };

                    if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                        rubric.Id = '';
                    }

                    PxPage.Loading('ui-resizable', true);

                    $.ajax({
                        url: PxPage.Routes.save_rubric_data,
                        type: 'POST',
                        dataType: 'json',
                        data: JSON.stringify(rubric),
                        contentType: 'application/json; charset=utf-8',
                        complete: function (response) {
                            var text = rubric.Title;
                            var val = response.responseText;

                            if ($('#fsRubric #selRubric').length) {
                                if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                                    $('#fsRubric #selRubric').append(
                                $('<option></option>').val(val).html(text)
                                );

                                    $('#fsRubric #selRubric').val(val);
                                }
                            }
                            else {
                                PxPage.Loaded('ui-resizable', false);
                                $(_static.sel.saveAsRubricDialog).dialog('close');
                                $('.rubric-editor').parents('#px-dialog').dialog("close");
                                $('#assessment-modes #newRubricCreated').val(val);
                                $('#assessment-modes #RubricsItem a').click();
                            }
                        }
                    });
                }
            },

            onSaveAsRubricClick: function () {
                PxRubricBuilder.SaveRubricText();
                $(_static.sel.saveAsRubricDialog).dialog('open');
            },

            onEditRubricAlignmentClick: function () {
                var $this = $(this),
                    itemId = $('#content-item-id').text(),
                    url = PxPage.Routes.edit_rubric_alignment + "?itemId=" + itemId;
                _static.fn.ShowRubricDialog(url, true, { title: "Align Rubric", dialogClass: 'px-dialog', height: 465, width: 420 });
            },

            onAlignRubricClick: function () {
                var $this = $(this),
                    itemId = $('#content-item-id').text(),
                    rubricId = $("#rubric-alignment input[type='radio']:checked").val();

                if (!itemId) {
                    return;
                }

                PxPage.Loading('ui-dialog', true);

                var args = {
                    itemId: itemId,
                    rubricId: rubricId,
                    folderItemId: itemId
                };

                $.post(PxPage.Routes.align_rubric, args, function (response) {
                    $("#alignment-rubrics").html(response);
                    $("#alignment-rubrics #aligned-rubric").addClass('fade-effect');
                    PxPage.Fade();
                    if ($("#alignment-objectives").length) {

                        $.post(PxPage.Routes.reload_manage_and_align_learning_objectives, args, function (response) {
                            $("#alignment-objectives").html(response);
                            PxEportfolioLearningObjective.BindControls();
                            PxPage.Loaded('ui-dialog', true);
                            $this.parents('#px-dialog').dialog("close");
                        });
                    }
                });


            },

            onCancelAlignRubricClick: function () {
                var $this = $(this);
                PxPage.Loaded('ui-dialog', true);
                $this.parents('#px-dialog').dialog("close");
            },

            onRemoveObjectiveClick: function () {
                var $this = $(this);
                $(this).closest('li').remove();
            },

            onRemoveAlignedRubricClick: function () {
                var itemId = $('#content-item-id').text();

                if (!itemId) {
                    return;
                }

                PxPage.Loading('alignment-rubrics');

                var args = {
                    itemId: itemId
                };

                $.post(PxPage.Routes.align_rubric, args, function (response) {
                    PxPage.Loaded('alignment-rubrics');
                    PxPage.CloseNonModal();
                    $("#alignment-rubrics").html(response);
                });
            },

            DupeNodeFound: function (title) {
                var found = false;
                var tempTitle = $.trim(title);
                $(_static.sel.allRubricNodes).each(function () {
                    var current = $(this);
                    var curTitle = $.trim(current.text());
                    if (curTitle.toLowerCase() == tempTitle.toLowerCase()) {
                        found = true;
                        return false;
                    }
                });

                return found;
            },

            onRubricTitleKeyUp: function (event) {
                event.stopImmediatePropagation();
                var isValid = _static.fn.ValidateTitle();
                if (!isValid) return;
            },

            onRubricCreateTitleKeyUp: function (event) {
                event.stopImmediatePropagation();
                var title = $.trim($(_static.sel.rubricCreateTitleInput).val());
                var preEditTitle = $.trim($(_static.sel.rubricTitleBackUp).val());

                $('#error-message-title').remove();

                if (_static.fn.DupeNodeFound(title) && preEditTitle != title) {
                    var errorDiv = $('<div id="error-message-title"><span id="error-icon"></span><span id="errorMessage">Duplicate rubric title, please select another.</span></div>');
                    errorDiv.insertAfter($(_static.sel.rubricCreateTitleInput));
                    $(_static.sel.submitRubricButton).attr("disabled", "disabled");
                }
                else {
                    $('#error-message-title').remove();
                    $(_static.sel.submitRubricButton).removeAttr("disabled");
                }
            },

            ValidateTitle: function () {
                var title = $(_static.sel.rubricTitleInput).val(),
                    isValid = true;
                if (_static.fn.DupeNodeFound(title)) {
                    $(_static.sel.rubricDialogError).show().text('Duplicate rubric title, Please select another.');
                    $(_static.sel.saveRubricButton).attr("disabled", "disabled");
                    $(_static.sel.submitRubricButton).attr("disabled", "disabled");
                    isValid = false;
                }
                else {
                    $(_static.sel.rubricDialogError).hide();
                    $(_static.sel.saveRubricButton).removeAttr("disabled");
                    $(_static.sel.submitRubricButton).removeAttr("disabled");
                }

                return isValid;
            },

            RemoveTinyControlForId: function (id) {
                try {
                    if (tinyMCE.activeEditor) {
                        tinyMCE.activeEditor.remove();
                    }
                }
                catch (e) {
                }
            },

            ShowRubricDialog: function (url, buildRubric, options) {
                options = $.extend({ modal: true, draggable: true, closeOnEscape: false, width: 'auto', height: 'auto', resizable: false, autoOpen: false, dialogClass: 'px-dialog-notitle' }, options || {});

                //This tag will the hold the dialog content.
                var tag = $("<div id='px-dialog'><div id='loadingBlock' style='padding-top:10px;padding-bottom:10px;'>Loading..</div></div>");

                tag.dialog({ modal: options.modal, title: options.title, dialogClass: options.dialogClass, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, height: options.height, resizable: options.resizable, autoOpen: options.autoOpen, close: function () {
                    if ($(this).find('.rubric-editor').length) {
                        $(_static.sel.saveAsRubricDialog).dialog('destroy').empty().remove();
                    }

                    $(this).dialog('destroy').empty().remove();
                }
                }).dialog('open');

                tag.load(url, {}, function (data, textStatus, XMLHttpRequest) {
                    tag.dialog("option", "position", "center");
                    if (buildRubric) {
                        var jsonData = $('#rubric-json-data').text();
                        PxRubricBuilder.Build(jsonData);

                        $("#rubric_learning_objectives").fauxtree({
                            debug: false,
                            showall: true,
                            readOnly: true
                        });

                        $(_static.sel.saveAsRubricDialog).dialog({ autoOpen: false, width: 520, height: 120, minWidth: 520, minHeight: 120, modal: true, draggable: false, closeOnEscape: true, dialogClass: 'px-dialog-notitle' });
                    }

                    $.isFunction(options.success) && (options.success)();
                });
            }
        }
    };
    // The public interface for interacting with this plugin.
    window.RubricManager = {
        // The init method sets up the data for the plugin using the given
        // option values to override the defaults.                
        init: function (options) {
            var settings = $.extend(true, _static.settings, _static.defaults, options),
                $this = $(this);

            $(_static.sel.createRubricButton).die('click');
            $(_static.sel.editRubricButton).die('click');
            $(_static.sel.deleteRubricButton).die('click');
            $(_static.sel.rubricItem).die('click');
            $(_static.sel.rubricItem).die('mouseenter');
            $(_static.sel.rubricItem).die('mouseleave');
            $(_static.sel.rubricLeftItemChechBox).die('change');
            $(_static.sel.rubricRightItemChechBox).die('change');
            $(_static.sel.addRubricsButton).die('click');
            $(_static.sel.removeRubricsButton).die('click');

            $(_static.sel.keepRubricsConfirmButton).die('click');
            $(_static.sel.viewRubricAlignmentsButton).die('click');
            $(_static.sel.submitRubricButton).die('click');
            $(_static.sel.saveRubricButton).die('click');
            $(_static.sel.editRubricAlignmentButton).die('click');
            $(_static.sel.alignRubricButton).die('click');
            $(_static.sel.cancelAlignRubricButton).die('click');
            $(_static.sel.removeAlignedRubricButton).die('click');
            $(_static.sel.removeObjectiveButton).die('click');
            $(_static.sel.cancelRubricButton).die('click');
            $(_static.sel.cancelSaveRubricButton).die('click');
            $(_static.sel.rubricTitleInput).die('keyup');
            $(_static.sel.rubricCreateTitleInput).die('keyup');
            $(_static.sel.rubricAlignmentSort).die('click');

            $(_static.sel.createRubricButton).live("click", _static.fn.onCreateRubricClick);
            $(_static.sel.editRubricButton).live("click", _static.fn.onEditRubricClick);
            $(_static.sel.deleteRubricButton).live('click', _static.fn.onDeleteRubricClick);
            $(_static.sel.rubricItem).live("click", _static.fn.onPreviewRubricClick);

            $(_static.sel.rubricLeftItemChechBox).live("change", _static.fn.onLeftItemNodeCheckboxChange);
            $(_static.sel.rubricRightItemChechBox).live("change", _static.fn.onRightItemNodeCheckboxChange);

            $(_static.sel.rubricItem).live("mouseenter", _static.fn.showEditRubricLink);
            $(_static.sel.rubricItem).live("mouseleave", _static.fn.hideEditRubricLink);
            $(_static.sel.addRubricsButton).live("click", _static.fn.onAddCourseRubricsClick);
            $(_static.sel.removeRubricsButton).live("click", _static.fn.onRemoveCourseRubricsClick);
            $(_static.sel.keepRubricsConfirmButton).live("click", _static.fn.onKeepCourseRubricsClick);
            $(_static.sel.viewRubricAlignmentsButton).live("click", _static.fn.onViewRubricAlignmentsClick);
            $(_static.sel.submitRubricButton).live("click", _static.fn.onSubmitRubricClick);
            $(_static.sel.saveRubricButton).live("click", _static.fn.onSaveRubricClick);
            $(_static.sel.saveAsRubricButton).live("click", _static.fn.onSaveAsRubricClick);
            $(_static.sel.editRubricAlignmentButton).live("click", _static.fn.onEditRubricAlignmentClick);
            $(_static.sel.alignRubricButton).live("click", _static.fn.onAlignRubricClick);
            $(_static.sel.cancelAlignRubricButton).live("click", _static.fn.onCancelAlignRubricClick);
            $(_static.sel.removeAlignedRubricButton).live("click", _static.fn.onRemoveAlignedRubricClick);
            $(_static.sel.removeObjectiveButton).live("click", _static.fn.onRemoveObjectiveClick);
            $(_static.sel.cancelRubricButton).live("click", _static.fn.onCancelRubricClick);
            $(_static.sel.cancelSaveRubricButton).live("click", _static.fn.onCancelSaveRubricClick);
            $(_static.sel.rubricTitleInput).live('keyup', _static.fn.onRubricTitleKeyUp);
            $(_static.sel.rubricCreateTitleInput).live('keyup', _static.fn.onRubricCreateTitleKeyUp);
            $(_static.sel.rubricAlignmentSort).live("click", _static.fn.onRubricAlignmentSortClick);
        }
    };
} (jQuery))

    