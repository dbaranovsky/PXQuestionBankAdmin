// QuestionEditorVariables Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables.
//
//
(function ($) {
    window.QuestionEditorMathArrayDialog = function () {
        var _static = {
            varTypes: {
                inMode: "new"
            },
            varOldName: "",
            sel: {
                variableDialog: ".hts-math-array-dialog",
                variableDialogForm: ".hts-math-array-dialog form",
                variableDialogFormErrors: ".hts-math-array-dialog form .errors",
                variableDialogFormInputs: ".hts-math-array-dialog .form input",

                //dialog buttons
                saveButton: ".hts-math-array-dialog .save-array-button",
                cancelButton: ".hts-math-array-dialog .cancel-array-button",
                addValueButton: '.hts-math-array-dialog .add-array-value-button',

                //row buttons
                variableRowEditButtons: '.hts-math-array-dialog .hts-variable-row .hts-variable-edit',
                variableRowDeleteButtons: '.hts-math-array-dialog .hts-variable-row .hts-variable-delete',
                variableRowSaveButtons: '.hts-math-array-dialog .hts-variable-row .hts-variable-save',
                variableRowCancelButtons: '.hts-math-array-dialog .hts-variable-row .hts-variable-cancel',

                variableList: ".hts-math-array-dialog .hts-variable-list",
                variableRows: ".hts-math-array-dialog .hts-variable-row",
                variableRowTemplate: ".hts-math-array-dialog .hts-variable-row-template",
                variableAnswerColumns: ".hts-math-array-dialog .hts-variable-answer",

                //pool local selectors
                variableRow: ".hts-variable-row",
                pendingVariableRow: '.hts-edit-row',
                variableRowEditButton: ".hts-variable-edit",
                variableRowDeleteButton: ".hts-variable-delete",

                variableActionsMenu: '.hts-variable-actions-menu',
                variableIndexColumn: ".hts-variable-index",
                variableAnswerColumn: ".hts-variable-answer",
                variableFormulaImage: ".hts-formula-input",
                variableArrayNameInput: ".hts-variable-array-name"
            },
            css: {
                variableFormulaImage: "hts-formula-input"
            },
            fn: {
                preload: function (variable) {
                    var dialog = $(_static.sel.variableDialog),
                        variableList = $(_static.sel.variableList),
                        equationImageUrl = $('#question-editor').attr("hts-data-equationimageUrl");

                    dialog.find(_static.sel.variableArrayNameInput).val(variable.Name);

                    _static.varOldName = variable.Name;

                    if (variable.Constraints[0]) {
                        var c = variable.Constraints[0];

                        if (c.Inclusions) {
                            var inclusionValues = c.Inclusions.split(",");

                            $.each(inclusionValues, function (i, v) {
                                var row = _static.fn.getNewRow();
                                var fText = v;// _pluginStatic.fn.htmlDecode(v);
                                var formulaImage = row.find(_static.sel.variableFormulaImage);
                                var imgSrc = equationImageUrl + "?eqtext=" + encodeURIComponent(fText);

                                formulaImage.attr('src', imgSrc);
                                formulaImage.attr('alt', imgSrc);
                                formulaImage.attr('hts-data-equation', v);
                                formulaImage.removeClass(_static.css.variableFormulaImage);

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
                    _static.varOldName = "";
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
                    var formulaText = "";

                    if (!variableRow.hasClass('hts-variable-row-template')) {
                        var rowAnswerColumn = variableRow.find(_static.sel.variableAnswerColumn),
                            formulaImage = rowAnswerColumn.find('img');

                        formulaText = decodeURIComponent(formulaImage.attr('hts-data-equation'));
                    }

                    return formulaText;
                },

                getVariableData: function () {
                    var dialog = $(_static.sel.variableDialog),
                        variableRows = $(_static.sel.variableRows).not('.hts-variable-row-template'),
                        type = "matharray";

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
                    if (_static.fn.checkPendingRows()) {
                        var varName = $.trim($(_static.sel.variableArrayNameInput).val());
                        var varNameValid =  !(varName === '' || (varName !== '' && varName === '<string>'));
                        if ($(_static.sel.variableDialogForm).valid() || varNameValid) {
                            _static.fn.save();
                        }
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
                    newRow.find(_static.sel.variableIndexColumn).text(rows.length);
                    newRow.addClass('hts-variable-row');

                    return newRow;
                },

                onAddVariableRowClick: function () {
                    if (_static.fn.checkPendingRows()) {
                        var variableList = $(_static.sel.variableList),
                        newRow = _static.fn.getNewRow();
                        newRow.appendTo(variableList);
                        var newName = 'hts-variable-row' + variableList.length;
                        newRow.attr('name', newName);
                        newRow.find(_static.sel.variableFormulaImage).click();
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
                    if (_static.fn.checkPendingRows()) {
                        var currentRow = $(this).closest(_static.sel.variableRow);
                        var imageCol = currentRow.find(_static.sel.variableAnswerColumn);
                        imageCol.click();
                    }
                },

                checkPendingRows: function () {
                    var variableList = $(_static.sel.variableList),
                        pendingRows = variableList.find(_static.sel.variableAnswerColumn).find('img[hts-data-equation=""]'),
                        totalRows = variableList.find(_static.sel.variableRow).not(_static.sel.variableRowTemplate);

                    if (pendingRows.length) {
                        alert('Invalid array values.');
                        return false;
                    }

                    return true;
                },

                orderVariableRows: function () {
                    $(_static.sel.variableRows).each(function (stepIdx) {
                        var variableIndex = stepIdx;
                        $(this).find(_static.sel.variableIndexColumn).text(variableIndex);
                    });
                },

                onVariableAnswerColumnHover: function (event) {
                    var $this = $(this),
                        parent = $this.parent()
                    editButton = parent.find(_static.sel.variableRowEditButton);

                    if (event.type == 'mouseover') {
                        // do something on mouseover
                        $this.addClass("hover");
                        editButton.addClass("hover");
                    } else {
                        // do something on mouseout
                        $this.removeClass("hover");
                        editButton.removeClass("hover");
                    }
                },

                onVariableAnswerColumnClick: function (event) {
                    var rowFormulaInput = $(this).find('img').get(0);
                    if (tinyMCE.activeEditor) {
                        tinyMCE.activeEditor.execCommand('htsFormulaEditor', rowFormulaInput);
                    }
                },

                onVariableArrayNameInputFocus: function (event) {
                    if ($(this).val() == '<string>')
                        $(this).val('');
                },

                onVariableArrayNameInputBlur: function (event) {
                    if ($(this).val().trim() == '')
                        $(this).val('<string>');
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
                        //                        numericVariableDialog: ".hts-math-array-dialog",                        
                    },
                    fn: {
                        loadMathArrayDialog: function (variable) {
                            _static.fn.load(variable);
                        },
                        saveMathArrayDialog: function () {
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
                $(_static.sel.addValueButton).die('click');
                $(_static.sel.saveButton).live("click", _static.fn.onSaveClick);
                $(_static.sel.cancelButton).live("click", _static.fn.onCancelClick);
                $(_static.sel.addValueButton).live("click", _static.fn.onAddVariableRowClick);

                $(_static.sel.variableRowDeleteButtons).die('click');
                $(_static.sel.variableRowEditButtons).die('click');
                $(_static.sel.variableRowDeleteButtons).live("click", _static.fn.onDeleteVariableRowClick);
                $(_static.sel.variableRowEditButtons).live("click", _static.fn.onEditVariableRowClick);

                $(_static.sel.variableAnswerColumns).die('mouseover mouseout');
                $(_static.sel.variableAnswerColumns).live('mouseover mouseout', _static.fn.onVariableAnswerColumnHover);

                $(_static.sel.variableAnswerColumns).die('click');
                $(_static.sel.variableAnswerColumns).live('click', _static.fn.onVariableAnswerColumnClick);

                $(_static.sel.variableArrayNameInput).die('focus').live('focus', _static.fn.onVariableArrayNameInputFocus);
                $(_static.sel.variableArrayNameInput).die('blur').live('blur', _static.fn.onVariableArrayNameInputBlur);
            }
        };
    };
} (jQuery))








