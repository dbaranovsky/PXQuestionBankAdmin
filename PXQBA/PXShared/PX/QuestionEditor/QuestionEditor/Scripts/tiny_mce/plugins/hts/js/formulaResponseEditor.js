tinyMCEPopup.requireLangPack();

// call from eqEditor to initialize eqEditor text
function eqEditorInitText() {
    return FormulaResponseEditorDialog.getFormulaSelection();
};

function eqEditorInitVars() {
    return "";
}


// callback from eqEditor flash movie for submit button
function eqSubmit(value) {
    FormulaResponseEditorDialog.updateFormulaSelection(value);
    FormulaResponseEditorDialog.closeFormulaEditor();
};

// callback from eqEditor flash movie for cancel button
function eqCancel() {
    FormulaResponseEditorDialog.closeFormulaEditor();
};

function setVariables() {
    //FormulaEditorDialog.close();
};

function isPageReady() {
    return true;
};

var FormulaResponseEditorDialog = function ($) {
    var _static = {
        pluginName: "FormulaResponseEditor",
        dataKey: "FormulaResponseEditor",
        bindSuffix: ".FormulaResponseEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {},
        css: {},
        sel: {
            responseDialog: ".hts-dialog-formula",
            responseDialogForm: ".hts-dialog-formula form",
            responseDialogFormErrors: ".hts-dialog-formula hts-form .errors",
            responseDialogFormInputs: ".hts-dialog-formula .form input",

            questionEditor: '#question-editor',
            gridTemplate: '.hts-grid-template',
            grid: '.hts-grid',
            gridBody: '.hts-grid-body',
            gridDataRows: '.hts-grid .data-row',
            gridDataRow: '.data-row',
            gridPendingRow: '.hts-edit-row',
            saveDialogButton: '.hts-dialog-save',
            cancelDialogButton: '.hts-dialog-cancel',
            addRowButton: '.hts-grid-addrow',
            editRowButton: '.hts-grid-editrow',
            deleteRowButton: '.hts-grid-deleterow',
            saveRowButton: '.hts-grid-saverow',
            cancelRowButton: '.hts-grid-cancelrow',

            //grid-row
            rowFormulaInput: '.hts-formula-input',
            rowAnswerRuleInput: '.hts-answer-rule-input',
            rowAnswerRuleSpan: '.hts-answer-rule-text',
            rowAnswerColumns: ".hts-grid .data-row .hts-answer",

            responseForm: '.hts-form',
            responseWeighting: '.response-weighting',

            formulaEditorDialogTemplate: '#formula-editor-dialog-template'
        },
        css: {
            rowFormulaInput: "hts-formula-input"
        },
        //private functions
        fn: {
            reset: function () {
                _static.fn.setDialogValidation($(_static.sel.responseDialog));
            },

            setDialogValidation: function () {
                $.validator.addMethod("greaterThanZero", function (value, element) {
                    return this.optional(element) || (parseFloat(value) > 0);
                }, "* Amount must be greater than zero");

                var dialogForm = $(_static.sel.responseDialog).find('form');
                var validator = dialogForm.validate({
                    ignore: ':hidden',
                    errorClass: "invalid",
                    validClass: "valid",
                    errorPlacement: function (error, element) {
                        //error.appendTo($(_static.sel.responseDialogFormErrors));
                    },
                    highlight: function (element, errorClass, validClass) {
                        $(element).addClass(errorClass).removeClass(validClass);
                    },
                    unhighlight: function (element, errorClass, validClass) {
                        $(element).removeClass(errorClass).addClass(validClass);
                    },
                    invalidHandler: function (form, validator) {
                        var errors = validator.numberOfInvalids();
                        if (errors) {
                            var message = errors == 1
                                        ? 'Invalid field. It has been highlighted'
                                        : 'Invalid ' + errors + ' fields. They have been highlighted';
                            dialogForm.find('.errors').html(message);
                            dialogForm.find('.errors').show();
                        } else {
                            dialogForm.find('.errors').hide();
                        }
                    }
                });

                dialogForm.find('.errors').hide();
                validator.resetForm();
            },

            step: function (id) {
                this.id = id;
                this.Hint = "";
                this.Incorrect = "";
                this.Correct = "";
                this.Question = "";
            },

            checkPendingRows: function () {
                var grid = $(_static.sel.grid),
                    gridPendingRows = grid.find(_static.sel.gridPendingRow).find('img[hts-data-equation=""]');

                if (gridPendingRows.length == 0) {
                    gridPendingRows = grid.find(_static.sel.gridPendingRow).find('img:not([hts-data-equation])');
                }

                if (gridPendingRows.length) {
                    alert("Invalid data values");
                    return false;
                }
                return true;
            },

            getDisplayText: function (description) {
                switch (description) {
                    case "any":
                        return "Any";
                    case "exact":
                        return "Exact";
                    case "similar":
                        return "Similar";
                    case "nodecimal":
                        return "No Decimal";
                    case "list":
                        return "List";
                    case "ordered list":
                        return "Ordered List";
                }
            },

            getValueText: function (description) {
                switch (description) {
                    case "Any":
                        return "any";
                    case "Exact":
                        return "exact";
                    case "Similar":
                        return "similar";
                    case "No Decimal":
                        return "nodecimal";
                    case "List":
                        return "list";
                    case "Ordered List":
                        return "ordered list";
                }
            },

            getNewRow: function () {
                var gridTemplate = $(_static.sel.gridTemplate),
                        gridRowTemplate = gridTemplate.find(_static.sel.gridDataRow),
                        newGridRow = gridRowTemplate.clone();

                return newGridRow;
            },
            onRowFormulaClick: function () {
                var rowFormulaInput = $(this).find('img'),
                    imgNode = rowFormulaInput.get(0),
                    eqText = rowFormulaInput.attr('hts-data-equation');

                if (!rowFormulaInput.attr('disabled')) {
                    _static.fn.showFormulaEditorDialog(imgNode);

                }
            },
            onAddRowClick: function () {
                var grid = $(_static.sel.grid),
                    newGridRow = _static.fn.getNewRow();

                newGridRow.addClass('hts-new-row');

                if (_static.fn.checkPendingRows()) {
                    $(_static.sel.saveRowButton).click();
                    newGridRow.appendTo(grid);

                    newGridRow.find(_static.sel.editRowButton).click();
                }
            },

            onEditRowClick: function () {
                //save any pending rows
                if (_static.fn.checkPendingRows()) {

                    var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowFormulaInput = currentGridRow.find('img'),
                    rowAnswerRuleInput = currentGridRow.find(_static.sel.rowAnswerRuleInput),
                    rowAnswerRuleSpan = currentGridRow.find(_static.sel.rowAnswerRuleSpan);

                    //disable image
                    rowFormulaInput.removeAttr('disabled');
                    rowFormulaInput.show();

                    rowAnswerRuleInput.val(_static.fn.getValueText(rowAnswerRuleSpan.text()));
                    rowAnswerRuleSpan.hide();
                    rowAnswerRuleInput.show();

                    //add editrow class to current datarow for highlight
                    currentGridRow.addClass('hts-edit-row');

                    //swap edit/delete icons for save/cancel icons
                    currentGridRow.find(_static.sel.editRowButton).hide();
                    currentGridRow.find(_static.sel.deleteRowButton).hide();
                    currentGridRow.find(_static.sel.saveRowButton).show();
                    currentGridRow.find(_static.sel.cancelRowButton).show();
                }
            },

            onDeleteRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow);

                helper.confirmDialog(
                    'Apply Changes',
                    'Are you sure you want to delete this row?',
                    'Delete',
                    "Don't Delete",
                    function () {
                        //delete it
                        currentGridRow.remove();
                    },
                    function () { });
            },

            onSaveRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowFormulaInput = currentGridRow.find('img'),
                    rowAnswerRuleInput = currentGridRow.find(_static.sel.rowAnswerRuleInput),
                    rowAnswerRuleSpan = currentGridRow.find(_static.sel.rowAnswerRuleSpan);

                if (_static.fn.checkPendingRows()) {
                    if ($(_static.sel.responseDialogForm).valid()) {
                        //rowFormulaInput.disabled();
                        rowFormulaInput.attr('disabled', 'disabled');

                        rowAnswerRuleInput.hide();
                        rowAnswerRuleSpan.text($.trim(rowAnswerRuleInput.find('option:selected').text()));
                        rowAnswerRuleSpan.show();

                        //add editrow class to current datarow for highlight
                        currentGridRow.removeClass('hts-edit-row');
                        currentGridRow.removeClass('hts-new-row');

                        //swap edit/delete icons for save/cancel icons
                        currentGridRow.find(_static.sel.editRowButton).show();
                        currentGridRow.find(_static.sel.deleteRowButton).show();
                        currentGridRow.find(_static.sel.saveRowButton).hide();
                        currentGridRow.find(_static.sel.cancelRowButton).hide();

                        _static.fn.reset();
                    }
                }
            },

            onCancelRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowFormulaInput = currentGridRow.find('img'),
                    rowAnswerRuleInput = currentGridRow.find(_static.sel.rowAnswerRuleInput),
                    rowAnswerRuleSpan = currentGridRow.find(_static.sel.rowAnswerRuleSpan);


                if (currentGridRow.hasClass('hts-new-row')) {
                    currentGridRow.remove();
                    return;
                }
                else {
                    //disable image
                    rowFormulaInput.attr('disabled', 'disabled');

                    rowAnswerRuleInput.hide();
                    rowAnswerRuleInput.val('');
                    rowAnswerRuleSpan.show();

                    //add editrow class to current datarow for highlight
                    currentGridRow.removeClass('hts-edit-row');

                    //swap edit/delete icons for save/cancel icons
                    currentGridRow.find(_static.sel.editRowButton).show();
                    currentGridRow.find(_static.sel.deleteRowButton).show();
                    currentGridRow.find(_static.sel.saveRowButton).hide();
                    currentGridRow.find(_static.sel.cancelRowButton).hide();
                }
            },

            onSaveDialogClick: function () {
                if (_static.fn.checkPendingRows()) {

                    if ($(_static.sel.gridDataRows).length == 0) {
                        alert('Response must have at least 1 correct answer.');
                        return;
                    }

                    if ($(_static.sel.responseDialogForm).valid()) {
                        var response = _static.fn.getResponseData();
                        var formulaImgBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path');

                        var imgBaseUrl = formulaImgBaseUrl.replace("geteq", "geticon");
                        var caption = response.Correct;
                        caption = encodeURIComponent(caption);

                        var imgSrc = imgBaseUrl + "?caption=" + caption + "&type=" + response.Type + "Response";

                        var responseImage = $('<img />');
                        responseImage.attr('src', imgSrc);
                        responseImage.attr('hts-data-type', 'math');
                        responseImage.attr('hts-data-id', response.ElementId.toString());

                        window.parent.QuestionEditorHelper.restoreBookMark(tinyMCEPopup.editor.editorId);

                        var imgNode = tinyMCEPopup.getWindowArg('response_image');
                        if (imgNode) $(imgNode).remove();
                        tinyMCEPopup.editor.execCommand('mceInsertContent', false, responseImage.OuterHTML());
                        tinyMCEPopup.editor.execCommand('htsAddResponseData', response);

                        _static.fn.reset();

                        setTimeout(function () { tinyMCEPopup.close(); }, 500);
                    }
                }
            },

            showFormulaEditorDialog: function (imgNode) {
                tinyMCEPopup.editor.execCommand('htsFormulaEditor', imgNode);
            },


            onCancelDialogClick: function () {
                _static.fn.reset();
                tinyMCEPopup.close();
            },

            buildNewGrid: function () {
                var gridTemplate = $(_static.sel.gridTemplate),
                    gridBody = $(_static.sel.gridBody),
                    newGrid = gridTemplate.clone();

                newGrid.removeClass('hts-grid-template').addClass('hts-grid');
                newGrid.find(_static.sel.gridDataRow).remove();
                newGrid.appendTo(gridBody);
            },

            preloadAnswerRow: function (answer) {
                var equationImageUrl = tinyMCEPopup.getWindowArg('equation_image_path');
                var grid = $(_static.sel.grid);
                var newGridRow = _static.fn.getNewRow(),
                            rowFormulaInput = newGridRow.find(_static.sel.rowFormulaInput),
                            rowAnswerRuleSpan = newGridRow.find(_static.sel.rowAnswerRuleSpan);

                if (answer.Correct) {
                    var fText = encodeURIComponent(answer.Correct); //_static.fn.htmlDecode(answer.Correct);
                    var imgSrc = equationImageUrl + "?eqtext=" + (fText);
                    rowFormulaInput.attr('disabled', 'disabled');
                    rowFormulaInput.attr('src', imgSrc);
                    rowFormulaInput.attr('alt', imgSrc);
                    rowFormulaInput.attr('hts-data-equation', answer.Correct);
                    rowFormulaInput.removeClass(_static.css.rowFormulaInput);

                    var answerRuleText = _static.fn.getDisplayText(answer.AnswerRule);
                    rowAnswerRuleSpan.text(answerRuleText);

                    newGridRow.appendTo(grid);
                }
            },

            setDefaults: function () {
                $(_static.sel.responseWeighting).val("1");
            },

            preloadDialog: function (response) {

                var grid = $(_static.sel.grid);

                if (response) {
                    $(_static.sel.responseWeighting).val(response.Points);

                    _static.fn.preloadAnswerRow(response);

                    for (var i = 0; i < response.Answers.length; i++) {

                        _static.fn.preloadAnswerRow(response.Answers[i]);
                    }
                }
            },

            onFormulaDeleteHover: function (event) {
                var $this = $(this),
                parent = $this.parent(),
                    title = "Delete this correct answer?";

                if (event.type == 'mouseover') {
                    $this.addClass("hover");

                    var tooltip = $('<div class="delete-tooltip"></div>').text(title);
                    tooltip.appendTo(parent);

                    tooltip.position({
                        my: 'right top',
                        at: 'right bottom',
                        of: $this,
                        offset: "1"
                    });

                    tooltip.addClass("delete-tooltip-adjust2");

                } else {
                    parent.find('.delete-tooltip').remove();

                }
            },

            getResponseData: function () {
                var weighting = $(_static.sel.responseWeighting).val(),
                    type = "math";

                var response = {};
                response.Type = type;
                response.Points = weighting;
                response.Answers = [];
                response.ElementId = _static.settings.responseData.responseId;

                $(_static.sel.gridDataRows).each(function (idx) {
                    var answer = {};
                    var row = $(this),
                        rowAnswerRule = row.find(_static.sel.rowAnswerRuleSpan),
                        rowAnswerFormulaImage = row.find('img'),
                        rowAnswerFormulaText = rowAnswerFormulaImage.attr('hts-data-equation'),
                        saveButton = row.find(_static.sel.saveRowButton)

                    if (saveButton && saveButton.is(":visible")) {
                        saveButton.click();
                        rowAnswerRule = row.find(_static.sel.rowAnswerRuleSpan);
                    }

                    if (idx == 0) {
                        response.Correct = decodeURIComponent(rowAnswerFormulaText);
                        response.AnswerRule = _static.fn.getValueText(rowAnswerRule.text());
                    }
                    else {
                        answer.Correct = decodeURIComponent(rowAnswerFormulaText);
                        answer.AnswerRule = _static.fn.getValueText(rowAnswerRule.text());

                        response.Answers.push(answer);
                    }
                });

                return response;
            },

            htmlDecode: function (str) {
                var div = document.createElement('div');
                div.innerHTML = str;
                return div.firstChild.data;
            },

            bindControls: function () {

                $(_static.sel.addRowButton).die('click');
                $(_static.sel.editRowButton).die('click');
                $(_static.sel.deleteRowButton).die('click');
                $(_static.sel.saveRowButton).die('click');
                $(_static.sel.cancelRowButton).die('click');

                $(_static.sel.saveDialogButton).die('click');
                $(_static.sel.cancelDialogButton).die('click');
                $(_static.sel.rowAnswerColumns).die('click');

                $(_static.sel.rowAnswerColumns).live("click", _static.fn.onRowFormulaClick);
                $(_static.sel.addRowButton).live("click", _static.fn.onAddRowClick);
                $(_static.sel.editRowButton).live("click", _static.fn.onEditRowClick);
                $(_static.sel.deleteRowButton).live("click", _static.fn.onDeleteRowClick);
                $(_static.sel.saveRowButton).live("click", _static.fn.onSaveRowClick);
                $(_static.sel.cancelRowButton).live("click", _static.fn.onCancelRowClick);

                $(_static.sel.saveDialogButton).live("click", _static.fn.onSaveDialogClick);
                $(_static.sel.cancelDialogButton).live("click", _static.fn.onCancelDialogClick);

                $(_static.sel.deleteRowButton).die('mouseover mouseout');
                $(_static.sel.deleteRowButton).live('mouseover mouseout', _static.fn.onFormulaDeleteHover);
            }
        }
    };

    return {
        init: function () {
        },
        build: function () {
            var responseData = tinyMCEPopup.getWindowArg("response_data");
            _static.settings.responseData = responseData;
            _static.fn.buildNewGrid();
            _static.fn.bindControls();

            if (responseData == undefined) {
            }
            else {
                if (responseData.mode == "new") {
                    _static.fn.setDefaults();
                    $(_static.sel.addRowButton).click();
                }
                if (responseData.mode == "edit") {
                    _static.fn.preloadDialog(responseData.response);
                }
            }

            _static.fn.reset();
        },
        getFormulaSelection: function () {
            var editRow = $(_static.sel.gridPendingRow),
                equationText = "";

            if (editRow.length) {
                var imgFormula = editRow.find(_static.sel.rowFormulaInput);
                equationText = imgFormula.attr('hts-data-equation');
            }
            return equationText;
        },
        updateFormulaSelection: function (value) {
        },
        closeFormulaEditor: function () {
        }
    }
} (jQuery);

jQuery.fn.OuterHTML = function () {
    return $('<div></div>').append(this.clone()).html();
};

tinyMCEPopup.onInit.add(FormulaResponseEditorDialog.init, FormulaResponseEditorDialog);
