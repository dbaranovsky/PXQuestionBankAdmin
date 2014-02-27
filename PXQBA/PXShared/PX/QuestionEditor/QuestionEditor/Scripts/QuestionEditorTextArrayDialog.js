// QuestionEditorVariables Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables.
//
//
(function ($) {
    window.QuestionEditorTextArrayDialog = function () {
        var _static = {
            varTypes: {
                inMode: "new"
            },
            varOldName: "",
            sel: {
                variableDialog: ".hts-text-array-dialog",
                variableDialogForm: ".hts-text-array-dialog form",
                variableDialogFormErrors: ".hts-text-array-dialog form .errors",
                variableDialogFormInputs: ".hts-text-array-dialog .form input",

                saveButton: ".hts-text-array-dialog .save-array-button",
                cancelButton: ".hts-text-array-dialog .cancel-array-button",
                variableList: ".hts-text-array-dialog .hts-variable-list",
                variableRows: ".hts-text-array-dialog .hts-variable-row",
                variableRowTemplate: ".hts-text-array-dialog .hts-variable-row-template",
                variableRowAddButton: '.hts-text-array-dialog .add-array-value-button',
                variableRowEditButtons: '.hts-text-array-dialog .hts-variable-row .hts-variable-edit',
                variableRowDeleteButtons: '.hts-text-array-dialog .hts-variable-row .hts-variable-delete',
                variableRowSaveButtons: '.hts-text-array-dialog .hts-variable-row .hts-variable-save',
                variableRowCancelButtons: '.hts-text-array-dialog .hts-variable-row .hts-variable-cancel',

                //pool local selectors
                variableRow: ".hts-variable-row",
                pendingVariableRow: '.hts-edit-row',
                variableRowEditButton: ".hts-variable-edit",
                variableRowDeleteButton: ".hts-variable-delete",
                variableRowSaveButton: ".hts-variable-save",
                variableRowCancelButton: ".hts-variable-cancel",
                variableValueText: ".hts-variable-value span",
                variableValueInput: ".hts-variable-value input",
                variableActionsMenu: '.hts-variable-actions-menu',
                variableIndex: ".hts-variable-index",
                variableArrayNameInput: ".hts-variable-array-name"
            },
            fn: {
                preload: function (variable) {
                    var dialog = $(_static.sel.variableDialog),
                        variableList = $(_static.sel.variableList);

                    dialog.find(_static.sel.variableArrayNameInput).val(variable.Name);

                    _static.varOldName = variable.Name;

                    if (variable.Constraints[0]) {
                        var c = variable.Constraints[0];

                        if (c.Inclusions) {
                            var inclusionValues = c.Inclusions.split(",");

                            $.each(inclusionValues, function (i, v) {
                                var row = _static.fn.getNewRow();
                                row.find(_static.sel.variableValueText).text(v);
                                row.appendTo(variableList);
                            });
                        }
                    }
                },

                load: function (variable) {
                    var variableDialog = $(_static.sel.variableDialog),
                        rightPane = $(_pluginStatic.sel.contentWrapperTableRight),
                        contentWrapper = $(_pluginStatic.sel.contentWrapper),
                        variableDialogBody = variableDialog.find('.body:first');

                    variableDialogBody.height(contentWrapper.height() - 110);
                    variableDialogBody.css('overflow-y', 'auto');
                    variableDialog.show();
                    _static.fn.reset();

                    _static.varTypes.inMode = "new";
                    if (variable) {
                        _static.varTypes.inMode = "edit";
                        _static.fn.preload(variable);
                    }

                    variableDialog.position({
                        my: "right top",
                        at: "left top",
                        of: rightPane,
                        collision: "fit"
                    });

                    $(_static.sel.variableArrayNameInput).focus();
                },
                save: function () {
                    if (_static.varTypes.inMode == "new") {
                        _static.varOldName = "";
                    }
                    var valid = true,
                        dialog = $(_static.sel.variableDialog),
                        variable = _static.fn.getVariableData(),
                        variableMem = QuestionEditorHelper.getVariableData(variable.Name);

                    switch (_static.varTypes.inMode) {
                        case "new":
                            if (variableMem) {
                                alert('Fail to save, duplicate variable found!');
                                valid = false;
                            }
                            break;
                        case "edit":
                            //                            if (!variableMem) {
                            //                                alert('Fail to save, variable not found!');
                            //                                valid = false;
                            //                            }
                            break;
                        default:
                            valid = false;
                    }

                    if (valid) {
                        //add variable to Memory variable list
                        QuestionEditorHelper.addVariableData(variable);

                        //add variable to UI variable list
                        _pluginStatic.fn.addVariableItem(variable);

                        _static.fn.reset();

                        //close dialog
                        dialog.hide();
                    }
                },

                reset: function () {
                    var variableRows = $(_static.sel.variableRows).not(_static.sel.variableRowTemplate);
                    variableRows.remove();

                    var formInputs = $(_static.sel.variableDialogFormInputs);
                    formInputs.val('');

                    _pluginStatic.fn.setDialogValidation($(_static.sel.variableDialog));

                    $.validator.addClassRules("hts-variable-array-name", {
                        nameRegex: /^[a-zA-Z]([a-zA-Z0-9_]*)$/
                    });
                },

                cancel: function () {
                    var dialog = $(_static.sel.variableDialog);

                    _static.fn.reset();

                    dialog.hide();
                },

                getVariableRowData: function (variableRow) {
                    var variableText = "";

                    if (!variableRow.hasClass('hts-variable-row-template')) {
                        var variableText = variableRow.find(_static.sel.variableValueText).text();
                    }

                    return variableText;
                },

                getVariableData: function () {
                    var dialog = $(_static.sel.variableDialog),
                        variableRows = $(_static.sel.variableRows).not('.hts-variable-row-template'),
                        type = "textarray";

                    var varArrayName = $.trim(dialog.find(_static.sel.variableArrayNameInput).val());

                    var variable = {};
                    variable.Type = type;
                    variable.Name = varArrayName;
                    variable.OldName = _static.varOldName;

                    variable.Constraints = [];
                    var constraint = {};
                    constraint.Inclusions = "";

                    $(variableRows).each(function (idx) {
                        var currRow = $(this);
                        var varText = _static.fn.getVariableRowData(currRow);
                        if ($.trim(varText) != "") {
                            if (idx != 0) {
                                constraint.Inclusions += ",";
                            }
                            constraint.Inclusions += varText;
                        }

                    });
                    variable.Constraints.push(constraint);

                    return variable;
                },

                onSaveClick: function () {
                    _static.fn.checkPendingRows();

                    if ($(_static.sel.variableDialogForm).valid()) {
                        _static.fn.save();
                    }
                },
                onCancelClick: function () {
                    _static.fn.cancel();
                },

                getNewRow: function () {
                    var rows = $(_static.sel.variableRows),
                        rowTemplate = $(_static.sel.variableRowTemplate),
                        newRow = rowTemplate.clone(),
                        variableAltClass = 'alt';

                    newRow.removeClass('hts-variable-row-template');

                    if (rows.length % 2 == 0) {
                        newRow.addClass(variableAltClass);
                    }
                    newRow.find(_static.sel.variableIndex).text(rows.length);
                    newRow.addClass('hts-variable-row');
                    return newRow;
                },

                onAddVariableRowClick: function () {
                    var variableList = $(_static.sel.variableList),
                        newRow = _static.fn.getNewRow();

                    _static.fn.checkPendingRows();

                    if (variableList.find(_static.sel.pendingVariableRow).length == 0) {
                        newRow.appendTo(variableList);
                        var newName = 'hts-variable-row' + variableList.length;
                        newRow.attr('name', newName);
                        newRow.find(_static.sel.variableRowEditButton).click();
                        newRow.find('.hts-variable-value-input').focus();
                    }
                    else {
                        $(_static.sel.variableDialogForm).valid();
                    }
                },

                onDeleteVariableRowClick: function () {
                    if (confirm('Are you sure you want to delete this variable?')) {
                        var variableRow = $(this).closest(_static.sel.variableRow);
                        variableRow.remove();

                        _static.fn.orderVariableRows();
                    }
                },

                onEditVariableRowClick: function () {
                    _static.fn.checkPendingRows();
                    var currentRow = $(this).closest(_static.sel.variableRow),
                    rowValueInput = currentRow.find(_static.sel.variableValueInput),
                    rowValueText = currentRow.find(_static.sel.variableValueText);

                    //add editrow class to current datarow for highlight
                    currentRow.addClass('hts-edit-row');

                    rowValueText.hide();
                    rowValueInput.val(rowValueText.text());
                    rowValueInput.show();

                    currentRow.find(_static.sel.variableRowEditButton).hide();
                    currentRow.find(_static.sel.variableRowDeleteButton).hide();
                    currentRow.find(_static.sel.variableRowSaveButton).show();
                    currentRow.find(_static.sel.variableRowCancelButton).show();
                },

                onSaveVariableRowClick: function () {
                    var currentRow = $(this).closest(_static.sel.variableRow),
                    rowValueInput = currentRow.find(_static.sel.variableValueInput),
                    rowValueText = currentRow.find(_static.sel.variableValueText);

                    if (rowValueInput.val()) {
                        //remove editrow class to current datarow for highlight
                        currentRow.removeClass('hts-edit-row');

                        rowValueInput.hide();
                        rowValueText.text(rowValueInput.val());
                        rowValueText.show();

                        currentRow.find(_static.sel.variableRowEditButton).show();
                        currentRow.find(_static.sel.variableRowDeleteButton).show();
                        currentRow.find(_static.sel.variableRowSaveButton).hide();
                        currentRow.find(_static.sel.variableRowCancelButton).hide();
                    }
                },

                onCancelVariableRowClick: function (variable) {
                    var currentRow = $(this).closest(_static.sel.variableRow),
                    rowValueInput = currentRow.find(_static.sel.variableValueInput),
                    rowValueText = currentRow.find(_static.sel.variableValueText);

                    //remove editrow class to current datarow for highlight
                    currentRow.removeClass('hts-edit-row');

                    rowValueInput.hide();
                    rowValueInput.val('');
                    rowValueText.show();

                    currentRow.find(_static.sel.variableRowEditButton).show();
                    currentRow.find(_static.sel.variableRowDeleteButton).show();
                    currentRow.find(_static.sel.variableRowSaveButton).hide();
                    currentRow.find(_static.sel.variableRowCancelButton).hide();
                },

                checkPendingRows: function () {
                    var variableList = $(_static.sel.variableList),
                        pendingRows = variableList.find(_static.sel.pendingVariableRow);

                    if (pendingRows.length) {
                        pendingRows.find(_static.sel.variableRowSaveButton).click();
                    }
                },

                orderVariableRows: function () {
                    $(_static.sel.variableRows).each(function (stepIdx) {
                        var variableIndex = stepIdx;
                        $(this).find(_static.sel.variableIndex).text(variableIndex);
                    });
                }
            }
        };
        return {
            _pluginStatic: {},
            //returns new static information to be added to a plugin
            static: function (pluginStatic) {
                _pluginStatic = pluginStatic;

                return {
                    sel: {
                    //                        numericVariableDialog: ".hts-text-array-dialog",                        
                },
                fn: {
                    loadTextArrayDialog: function (variable) {
                        _static.fn.load(variable);
                    },
                    saveTextArrayDialog: function () {
                        return _static.fn.save();
                    }
                }
            };
        },
        //returns new API calls for the plugin
        api: function (pluginStatic) {
            _pluginStatic = pluginStatic;

            return {
            //                    speak: function (message) {
            //                        $(_pluginStatic.sel.speakArea).html(message);
            //                    }
        };
    },

    init: function () {
        // Bind controls                

        $(_static.sel.saveButton).die('click');
        $(_static.sel.cancelButton).die('click');
        $(_static.sel.variableRowDeleteButtons).die('click');
        $(_static.sel.variableRowEditButtons).die('click');
        $(_static.sel.variableRowSaveButtons).die('click');
        $(_static.sel.variableRowCancelButtons).die('click');
        $(_static.sel.variableRowAddButton).die('click');

        $(_static.sel.saveButton).live("click", _static.fn.onSaveClick);
        $(_static.sel.cancelButton).live("click", _static.fn.onCancelClick);

        $(_static.sel.variableRowDeleteButtons).live("click", _static.fn.onDeleteVariableRowClick);
        $(_static.sel.variableRowEditButtons).live("click", _static.fn.onEditVariableRowClick);
        $(_static.sel.variableRowSaveButtons).live("click", _static.fn.onSaveVariableRowClick);
        $(_static.sel.variableRowCancelButtons).live("click", _static.fn.onCancelVariableRowClick);
        $(_static.sel.variableRowAddButton).live("click", _static.fn.onAddVariableRowClick);
    }
};
};
} (jQuery))








