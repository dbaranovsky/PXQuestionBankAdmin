/// <reference path="helper.js" />
tinyMCEPopup.requireLangPack();

var NumericResponseEditorDialog = function ($) {
    var _static = {
        pluginName: "NumericResponseEditor",
        dataKey: "NumericResponseEditor",
        bindSuffix: ".NumericResponseEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {},
        css: {},
        sel: {
            responseDialog: ".hts-dialog-numeric",
            responseDialogForm: ".hts-dialog-numeric form",
            responseDialogFormErrors: ".hts-dialog-numeric hts-form .errors",
            responseDialogFormInputs: ".hts-dialog-numeric .form input",

            questionEditor: '#question-editor',
            gridTemplate: '.hts-grid-template',

            //grid
            grid: '.hts-grid',
            gridBody: '.hts-grid-body',
            gridDataRows: '.hts-grid .data-row',
            gridDataRow: '.data-row',
            gridPendingRow: '.hts-edit-row',

            //commands
            saveDialogButton: '.hts-dialog-save',
            cancelDialogButton: '.hts-dialog-cancel',
            addRowButton: '.hts-grid-addrow',
            editRowButton: '.hts-grid-editrow',
            deleteRowButton: '.hts-grid-deleterow',
            saveRowButton: '.hts-grid-saverow',
            cancelRowButton: '.hts-grid-cancelrow',

            //grid-row
            rowAnswerInput: '.hts-answer-input',
            rowAnswerSpan: '.hts-answer-text',
            rowSigFigsInput: '.hts-sigfigs-input',
            rowSigFigsSpan: '.hts-sigfigs-text',
            rowToleranceInput: '.hts-tolerance-input',
            rowToleranceSpan: '.hts-tolerance-text',
            rowToleranceTypeInput: '.hts-tolerance-type-input',
            rowToleranceTypeSpan: '.hts-tolerance-type-text',

            responseForm: '.hts-form',

            //radio button
            responseFullPrecision: '.response-precision',
            responseShow: '.response-precision',
            responseAuto: '.response-chars',
            responseNumber: '.response-chars',
            responseFullPreOption: '.response-precision[value=full]',
            responseShowOption: '.response-precision[value=show]',
            responseAutoOption: '.response-chars[value=auto]',
            responseNumberOption: '.response-chars[value=number]',

            responseWeighting: '.response-weighting',

            responseChk: '.response-check',
            responseText: '.response-text',

            responseSigFig: '.response-checksigfigs',
            responseSigFigTrue: '#checksigfigs-true',
            responseSigFigFalse: '#checksigfigs-false',

            //dropdown
            responseDecimalPlacesDropDown: '.response-decimal-places',
            responseCharactersInput: '.response-characters'
        },
        //private functions
        fn: {
            reset: function () {
                _static.fn.setDialogValidation($(_static.sel.responseDialog));
            },

            numericVariableTest: function (value) {
                var returnFlag = false;

                if ($.trim(value).substring(0, 1) === "$") {
                    /*var varName = value.substr(1, value.length);
                    var variableData = _static.settings.responseData.numericVariable;
                    if (variableData != undefined) {
                        $.each(variableData, function (variableName) {
                            if (varName === variableName)*/
                                returnFlag = true;
                        /*});
                    }*/
                }

                return returnFlag;
            },

            /* http://hcicrossroads.blogspot.com/2011/01/regular-expression-for-multiple-forms.html */
            scientificNumberTest: function (value) {
                return (/^[+|-]?\d\.?\d{0,}[E|e|X|x](10)?[\^\*]?[+|-]?\d+$/.test(value) || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value) || /^[.](?:\d+)/.test(value) || _static.fn.numericVariableTest(value) || _static.fn.mathsExpressionTest(value)); /* || /^[+|-](?:\.\d+)/.test(value) */
            },

            significantNumberTest: function (value) {
                var returnFlag = false;

                if (/^[+]?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value) || _static.fn.numericVariableTest(value)) {
                    returnFlag = true;
                }

                return returnFlag;
            },
            
            mathsExpressionTest: function (value) {
                try {
                    var math = mathjs();
                    math.parse(value);
                    return true;
                } catch(e) {
                    return false;
                } 
                
            },

            setDialogValidation: function () {
                $.validator.addMethod("greaterThanZero", function (value, element) {
                    return this.optional(element) || (parseFloat(value) > 0);
                }, "* Amount must be greater than zero");

                $.validator.addMethod("scientificNumber", function (value, element) {
                    return this.optional(element) || _static.fn.scientificNumberTest(value); 
                }, "* Please enter a valid number.");

                $.validator.addMethod("numberVariable", function (value, element) {
                    var returnFlag = false;
                    if (this.optional(element)) {
                        returnFlag = true;
                    }
                    else {
                        returnFlag = _static.fn.significantNumberTest(value);
                    }
                    return returnFlag;
                }, "* Please enter a valid positive number or number variable.");

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

            onWidthClicked: function (e) {
                switch ($(this).val()) {
                    case "number":
                        $(_static.sel.responseCharactersInput).removeAttr('disabled');
                        $(_static.sel.responseCharactersInput).focus();
                        break;
                    case "auto":
                        $(_static.sel.responseCharactersInput).val(""); //default width
                        $(_static.sel.responseCharactersInput).attr('disabled', 'disabled');
                        break;
                }
            },

            onPrecisionClicked: function (e) {
                switch ($(this).val()) {
                    case "show":
                        $(_static.sel.responseDecimalPlacesDropDown).removeAttr('disabled');
                        $(_static.sel.responseDecimalPlacesDropDown).focus();
                        break;
                    case "full":
                        $(_static.sel.responseDecimalPlacesDropDown).val(""); //default width
                        $(_static.sel.responseDecimalPlacesDropDown).attr('disabled', 'disabled');
                        break;
                }
            },

            getTypeId: function (strSelection) {
                switch (strSelection) {
                    case "Absolute":
                        return "1";
                    case "Relative":
                        return "2";
                    default:
                        return "0";
                }
            },
            getDescription: function (strId) {
                switch (strId) {
                    case "0":
                        return "(please select:)";
                    case "1":
                        return "Absolute";
                    case "2":
                        return "Relative";
                };
            },
            checkPendingRows: function () {
                var grid = $(_static.sel.grid),
                    gridPendingRows = grid.find(_static.sel.gridPendingRow);

                if (gridPendingRows.length) {
                    gridPendingRows.find(_static.sel.saveRowButton).click();
                    //                    if (confirm('You have unsaved changes.')) {
                    //                        gridPendingRows.find(_static.sel.saveRowButton).click();
                    //                        //gridPendingRows.find(_static.sel.cancelDialogButton).click();
                    //                    }
                }
            },
            onResponseChkClick: function () {
                if ($(this).attr("checked") == "checked") {
                    $(_static.sel.responseText).removeAttr("disabled");
                } else {
                    $(_static.sel.responseText).attr("disabled", "disabled");
                }
            },

            onResponseSigfigs: function () {
                if ($(this).attr("checked") == "checked") {
                    _static.fn.showHideSigFigsSection(true);
                } else {
                    _static.fn.showHideSigFigsSection(false);
                }
            },

            showHideSigFigsSection: function (sigfigs) {
                if (sigfigs == true) {
                    $(_static.sel.responseSigFigTrue).show();
                    $(_static.sel.responseSigFigFalse).hide();
                    $(_static.sel.rowSigFigsInput).removeAttr('disabled');
                    $(_static.sel.rowSigFigsInput).addClass('required');
                } else {
                    $(_static.sel.responseSigFigTrue).hide();
                    $(_static.sel.responseSigFigFalse).show();
                    $(_static.sel.rowSigFigsInput).attr('disabled', 'disabled');
                    $(_static.sel.rowSigFigsInput).val('');
                    $(_static.sel.rowSigFigsSpan).text('');
                    $(_static.sel.rowSigFigsInput).removeClass('required');
                }
            },

            getNewRow: function () {
                var gridTemplate = $(_static.sel.gridTemplate),
                        gridRowTemplate = gridTemplate.find(_static.sel.gridDataRow),
                        newGridRow = gridRowTemplate.clone();

                return newGridRow;
            },

            onAddRowClick: function () {
                var grid = $(_static.sel.grid),
                    newGridRow = _static.fn.getNewRow(),
                    numRows = $(_static.sel.gridDataRows).length;

                _static.fn.checkPendingRows();

                if (grid.find(_static.sel.gridPendingRow).length == 0) {
                    newGridRow.appendTo(grid);
                    var answerInput = newGridRow.find('.hts-answer-input');
                    answerInput.attr('name', 'hts-answer-input' + numRows);

                    var sigfigsInput = newGridRow.find('.hts-sigfigs-input');
                    sigfigsInput.attr('name', 'hts-sigfigs-input' + numRows);
                    if ($(_static.sel.responseSigFig).attr('checked') !== 'checked') {
                        sigfigsInput.attr('disabled', 'disabled');
                    } else {
                        sigfigsInput.addClass('required');
                    }

                    var toleranceInput = newGridRow.find('.hts-tolerance-input');
                    toleranceInput.attr('name', 'hts-tolerance-text' + numRows);

                    var toleranceTypeInput = newGridRow.find('.hts-tolerance-type-input');
                    toleranceTypeInput.attr('name', 'hts-tolerance-type-input' + numRows);

                    newGridRow.find(_static.sel.editRowButton).click();
                }
                else {
                    $(_static.sel.responseDialogForm).valid();
                }
            },

            onEditRowClick: function () {
                //save any pending rows
                _static.fn.checkPendingRows();

                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowAnswerInput = currentGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan),
                    rowSigFigsInput = currentGridRow.find(_static.sel.rowSigFigsInput),
                    rowSigFigsSpan = currentGridRow.find(_static.sel.rowSigFigsSpan),
                    rowToleranceInput = currentGridRow.find(_static.sel.rowToleranceInput),
                    rowToleranceSpan = currentGridRow.find(_static.sel.rowToleranceSpan),
                    rowToleranceTypeInput = currentGridRow.find(_static.sel.rowToleranceTypeInput),
                    rowToleranceTypeSpan = currentGridRow.find(_static.sel.rowToleranceTypeSpan);


                //swap span/textbox
                rowAnswerSpan.hide();
                rowAnswerInput.val(rowAnswerSpan.text());
                rowAnswerInput.show();

                rowSigFigsSpan.hide();
                rowSigFigsInput.val(rowSigFigsSpan.text());
                rowSigFigsInput.show();

                rowToleranceSpan.hide();
                rowToleranceInput.val(rowToleranceSpan.text());
                rowToleranceInput.show();

                rowToleranceTypeSpan.hide();
                rowToleranceTypeInput.val(_static.fn.getTypeId(rowToleranceTypeSpan.text()));
                rowToleranceTypeInput.show();

                //add editrow class to current datarow for highlight
                currentGridRow.addClass('hts-edit-row');

                //swap edit/delete icons for save/cancel icons
                currentGridRow.find(_static.sel.editRowButton).hide();
                currentGridRow.find(_static.sel.deleteRowButton).hide();
                currentGridRow.find(_static.sel.saveRowButton).show();
                currentGridRow.find(_static.sel.cancelRowButton).show();

                //put focus in textbox automatically
                rowAnswerInput.focus();

                //if textbox has input, select all
                if ($.trim(rowAnswerInput.val()).length) {
                    rowAnswerInput.select();
                }
            },

            onDeleteRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow);
                /*helper.confirmMessageDialog('test dialog', currentGridRow);
                return;*/
                helper.confirmDialog(
                    'Confirm',
                    'Are you sure you want to delete this row?',
                    'Delete',
                    "Don't Delete",
                    _static.fn.removeElement,
                    function () { },
                    currentGridRow);
            },

            removeElement: function (elementToRemove) {
                if (elementToRemove != null) {
                    elementToRemove.remove();
                }
            },

            onSaveRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowAnswerInput = currentGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan),
                    rowSigFigsInput = currentGridRow.find(_static.sel.rowSigFigsInput),
                    rowSigFigsSpan = currentGridRow.find(_static.sel.rowSigFigsSpan),
                    rowToleranceInput = currentGridRow.find(_static.sel.rowToleranceInput),
                    rowToleranceSpan = currentGridRow.find(_static.sel.rowToleranceSpan),
                    rowToleranceTypeInput = currentGridRow.find(_static.sel.rowToleranceTypeInput),
                    rowToleranceTypeSpan = currentGridRow.find(_static.sel.rowToleranceTypeSpan),
                    rowAnswerInputVal = $.trim(rowAnswerInput.val()),
                    rowToleranceInputVal = $.trim(rowToleranceInput.val());

                var checkSigFigs = true;
                if ($(_static.sel.responseSigFig).attr('checked') == 'checked')
                    checkSigFigs = _static.fn.significantNumberTest(rowSigFigsInput.val());

                if ((rowAnswerInputVal != "" && rowToleranceInputVal != "") && (_static.fn.scientificNumberTest(rowAnswerInputVal) == true && _static.fn.scientificNumberTest(rowToleranceInputVal) == true) && checkSigFigs == true) {
                    //swap span/textbox
                    rowAnswerInput.hide();
                    rowAnswerSpan.text($.trim(rowAnswerInput.val()));
                    rowAnswerSpan.show();

                    rowSigFigsInput.hide();
                    rowSigFigsSpan.text($.trim(rowSigFigsInput.val()));
                    rowSigFigsSpan.show();

                    rowToleranceInput.hide();
                    rowToleranceSpan.text($.trim(rowToleranceInput.val()));
                    rowToleranceSpan.show();

                    rowToleranceTypeInput.hide();
                    rowToleranceTypeSpan.text(_static.fn.getDescription($.trim(rowToleranceTypeInput.val())));
                    rowToleranceTypeSpan.show();

                    //add editrow class to current datarow for highlight
                    currentGridRow.removeClass('hts-edit-row');

                    //swap edit/delete icons for save/cancel icons
                    currentGridRow.find(_static.sel.editRowButton).show();
                    currentGridRow.find(_static.sel.deleteRowButton).show();
                    currentGridRow.find(_static.sel.saveRowButton).hide();
                    currentGridRow.find(_static.sel.cancelRowButton).hide();

                    if ($(_static.sel.responseDialogForm).valid()) {
                        _static.fn.reset();
                    }
                }
                else {
                    $(_static.sel.responseDialogForm).valid();
                }
            },

            onCancelRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowAnswerInput = currentGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan),
                    rowSigFigsInput = currentGridRow.find(_static.sel.rowSigFigsInput),
                    rowSigFigsSpan = currentGridRow.find(_static.sel.rowSigFigsSpan),
                    rowToleranceInput = currentGridRow.find(_static.sel.rowToleranceInput),
                    rowToleranceSpan = currentGridRow.find(_static.sel.rowToleranceSpan),
                    rowToleranceTypeInput = currentGridRow.find(_static.sel.rowToleranceTypeInput),
                    rowToleranceTypeSpan = currentGridRow.find(_static.sel.rowToleranceTypeSpan);

                //swap span/textbox
                rowAnswerInput.hide();
                rowAnswerInput.val('');
                rowAnswerSpan.show();

                rowSigFigsInput.hide();
                rowSigFigsInput.val('');
                rowSigFigsSpan.show();

                rowToleranceInput.hide();
                rowToleranceInput.val('');
                rowToleranceSpan.show();

                rowToleranceTypeInput.hide();
                rowToleranceTypeInput.val('');
                rowToleranceTypeSpan.show();

                //add editrow class to current datarow for highlight
                currentGridRow.removeClass('hts-edit-row');

                //swap edit/delete icons for save/cancel icons
                currentGridRow.find(_static.sel.editRowButton).show();
                currentGridRow.find(_static.sel.deleteRowButton).show();
                currentGridRow.find(_static.sel.saveRowButton).hide();
                currentGridRow.find(_static.sel.cancelRowButton).hide();
            },

            onSaveDialogClick: function () {

                _static.fn.checkPendingRows();

                if ($(_static.sel.gridDataRows).length == 0) {
                    alert('Response must have at least 1 correct answer.');
                    return;
                }

                if ($(_static.sel.responseDialogForm).valid()) {
                    var response = _static.fn.getResponseData();

                    if (response == false) {
                        alert('Please edit answer(s) with Significant Figure values.'); /* helper.confirmMessageDialog */
                        return;
                    }

                    var formulaImgBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path');
                    var imgBaseUrl = formulaImgBaseUrl.replace("geteq", "geticon");
                    var caption = response.Correct;
                    if (response.Format != "auto" && parseInt(response.Format) != NaN && caption.indexOf('e') == -1 && response.SigFigs == '') {
                        caption = _static.fn.format_number(caption, parseInt(response.Format));
                    }
                    caption = encodeURIComponent(caption);

                    var imgSrc = imgBaseUrl + "?caption=" + caption + "&type=" + response.Type + "Response";

                    var responseImage = $('<img />');
                    responseImage.attr('src', imgSrc);
                    responseImage.attr('hts-data-type', 'numeric');
                    responseImage.attr('hts-data-id', response.ElementId.toString());

                    window.parent.QuestionEditorHelper.restoreBookMark(tinyMCEPopup.editor.editorId);
                    var imgNode = tinyMCEPopup.getWindowArg('response_image');
                    if (imgNode) $(imgNode).remove();
                    tinyMCEPopup.editor.execCommand('mceInsertContent', false, responseImage.OuterHTML());
                    tinyMCEPopup.editor.execCommand('htsAddResponseData', response);

                    _static.fn.reset();

                    setTimeout(function () { tinyMCEPopup.close(); }, 500);
                }
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

            setDefaults: function () {
                $(_static.sel.responseWeighting).val("1");
                $(_static.sel.responseAutoOption).attr('checked', 'checked');
                $(_static.sel.responseCharactersInput).removeAttr('disabled');

                $(_static.sel.responseFullPreOption).attr('checked', 'checked');
                $(_static.sel.responseDecimalPlacesDropDown).attr('disabled', 'disabled');
            },
            preloadAnswerRow: function (answer) {
                var grid = $(_static.sel.grid);
                var newGridRow = _static.fn.getNewRow(),
                        rowAnswerSpan = newGridRow.find(_static.sel.rowAnswerSpan),
                        rowSigFigsSpan = newGridRow.find(_static.sel.rowSigFigsSpan),
                        rowToleranceSpan = newGridRow.find(_static.sel.rowToleranceSpan),
                        rowToleranceTypeSpan = newGridRow.find(_static.sel.rowToleranceTypeSpan),
                        tolerances = newGridRow.find(_static.sel.rowToleranceTypeInput);
                var correctTolerance = answer.ToleranceType;
                if (answer.Correct) {
                    var answerText = answer.Correct;
                    var sigfigsText = answer.SigFigs;
                    var toleranceText = answer.Tolerance;
                    var toleranceTypeText = answer.ToleranceType;
                    toleranceTypeText = answer.ToleranceType;
                    rowAnswerSpan.text(answerText);
                    rowSigFigsSpan.text(sigfigsText);
                    rowToleranceSpan.text(toleranceText);
                    rowToleranceTypeSpan.text(toleranceTypeText);

                    newGridRow.appendTo(grid);
                }
            },

            preloadDialog: function (response) {
                var grid = $(_static.sel.grid);

                if (response.Format == "auto") {
                    $(_static.sel.responseFullPreOption).attr('checked', 'checked');
                    $(_static.sel.responseDecimalPlacesDropDown).attr('disabled', 'disabled');
                }
                else {
                    $(_static.sel.responseShowOption).attr('checked', 'checked');
                    $(_static.sel.responseDecimalPlacesDropDown).removeAttr('disabled');
                    $(_static.sel.responseDecimalPlacesDropDown).val(response.Format);
                }

                if (response.Size == "auto") {
                    $(_static.sel.responseAutoOption).attr('checked', 'checked');
                    $(_static.sel.responseCharactersInput).attr('disabled', 'disabled');
                }
                else {
                    $(_static.sel.responseNumberOption).attr('checked', 'checked');
                    $(_static.sel.responseCharactersInput).val(response.Size);
                }

                if (response.CheckSyntax == "on") {
                    $(_static.sel.responseChk).attr('checked', 'checked');
                    $(_static.sel.responseText).removeAttr("disabled");
                }

                if (response.SigFigs != undefined && response.SigFigs.length > 0) {
                    $(_static.sel.responseSigFig).attr('checked', 'checked');
                    _static.fn.showHideSigFigsSection(true);
                } else {
                    _static.fn.showHideSigFigsSection(false);
                }

                $(_static.sel.responseWeighting).val(response.Points);

                $(_static.sel.responseText).val(helper.htmlDecode(response.AllowedWords));

                _static.fn.preloadAnswerRow(response);

                for (var i = 0; i < response.Answers.length; i++) {

                    _static.fn.preloadAnswerRow(response.Answers[i]);
                }
            },
            onNumericDeleteHover: function (event) {
                var $this = $(this),
                parent = $this.parent(),
                    title = "Delete this correct answer?";

                if (event.type == 'mouseover') {
                    // do something on mouseover
                    $this.addClass("hover");

                    var tooltip = $('<div class="delete-tooltip"></div>').text(title);
                    //style = "white-space:nowrap;text-align:right;position:absolute;"
                    tooltip.appendTo(parent);

                    tooltip.position({
                        my: 'right top',
                        at: 'right bottom',
                        of: $this,
                        offset: "1"
                    });

                    tooltip.addClass("delete-tooltip-adjust2");

                } else {
                    // do something on mouseout
                    parent.find('.delete-tooltip').remove();

                }
            },
            getResponseData: function () {
                var width = "",
                    type = "numeric",
                    format = "",
                    auto = false,
                    checkSyntax = "off",
                    weighting = $(_static.sel.responseWeighting).val(),
                    allowedWords = helper.htmlEncode($(_static.sel.responseText).val()),
                    makeResponseToFalse = false;

                if ($(_static.sel.responseFullPreOption).is(':checked')) {
                    format = "auto";
                }
                else {
                    format = $(_static.sel.responseDecimalPlacesDropDown).val();
                }

                if ($(_static.sel.responseAutoOption).is(':checked')) {
                    width = "auto";
                }
                else {
                    width = $(_static.sel.responseCharactersInput).val();
                }

                if ($(_static.sel.responseChk).is(':checked')) {
                    checkSyntax = "on";
                }

                var response = {};
                response.ElementId = _static.settings.responseData.responseId;
                response.Type = type;
                response.Points = weighting;
                response.Answers = [];
                response.Format = format;
                response.Size = width;
                response.CheckSyntax = checkSyntax;
                response.AllowedWords = allowedWords;
                $(_static.sel.gridDataRows).each(function (idx) {
                    var answer = {};
                    var row = $(this),
                        rowAnswer = row.find(_static.sel.rowAnswerSpan),
                        rowSigFigs = row.find(_static.sel.rowSigFigsSpan),
                        rowTolerance = row.find(_static.sel.rowToleranceSpan),
                        rowToleranceType = row.find(_static.sel.rowToleranceTypeSpan),
                        rowToleranceIndex = row.find(_static.sel.rowToleranceTypeInput).attr("hts-tolerance-index");

                    /* This is a special condition, if user selects SigFigs at last moment and did not provided value, stop saving */
                    if ($(_static.sel.responseSigFig).attr('checked') == 'checked' && $.trim(rowSigFigs.text()) == "")
                        makeResponseToFalse = true;

                    if (idx == 0) {
                        response.Correct = rowAnswer.text();
                        response.SigFigs = rowSigFigs.text();
                        response.Tolerance = rowTolerance.text();
                        response.ToleranceType = rowToleranceType.text();

                    }
                    else {
                        answer.Correct = rowAnswer.text();
                        answer.SigFigs = rowSigFigs.text();
                        answer.Tolerance = rowTolerance.text();
                        answer.ToleranceType = rowToleranceType.text();
                        response.Answers.push(answer);
                    }
                });

                if (makeResponseToFalse == true) {
                    return false;
                }

                return response;
            },

            bindControls: function () {

                $(_static.sel.addRowButton).die('click');
                $(_static.sel.editRowButton).die('click');
                $(_static.sel.deleteRowButton).die('click');
                $(_static.sel.saveRowButton).die('click');
                $(_static.sel.cancelRowButton).die('click');


                $(_static.sel.saveDialogButton).die('click');
                $(_static.sel.cancelDialogButton).die('click');
                $(_static.sel.responseAuto).die('click');
                $(_static.sel.responseFullPrecision).die('click');

                $(_static.sel.responseChk).die('click');

                $(_static.sel.responseAuto).live('click', _static.fn.onWidthClicked);
                $(_static.sel.addRowButton).live("click", _static.fn.onAddRowClick);
                $(_static.sel.editRowButton).live("click", _static.fn.onEditRowClick);
                $(_static.sel.deleteRowButton).live("click", _static.fn.onDeleteRowClick);
                $(_static.sel.saveRowButton).live("click", _static.fn.onSaveRowClick);
                $(_static.sel.cancelRowButton).live("click", _static.fn.onCancelRowClick);
                $(_static.sel.responseChk).live("click", _static.fn.onResponseChkClick);
                $(_static.sel.responseSigFig).die('click').live("click", _static.fn.onResponseSigfigs);
                _static.fn.showHideSigFigsSection(false);
                $(_static.sel.responseFullPrecision).live("click", _static.fn.onPrecisionClicked);

                $(_static.sel.saveDialogButton).live("click", _static.fn.onSaveDialogClick);
                $(_static.sel.cancelDialogButton).live("click", _static.fn.onCancelDialogClick);


                $(_static.sel.deleteRowButton).die('mouseover mouseout');
                $(_static.sel.deleteRowButton).live('mouseover mouseout', _static.fn.onNumericDeleteHover);
            },

            format_number: function (pnumber, decimals) {
                if (isNaN(pnumber)) { return 0 };
                if (pnumber == '') { return 0 };
                var negative = false;
                var snum = new String(pnumber);
                var sec = snum.split('.');
                var whole = parseFloat(sec[0]);
                if (isNaN(whole) == true) {
                    whole = 0;
                }
                if (pnumber < 0) {
                    negative = true;
                    whole = -(whole);
                }
                var result = '';
                if (sec.length > 1) {
                    var dec = new String(sec[1]);
                    dec = String(parseFloat(sec[1]) / Math.pow(10, (dec.length - decimals)));
                    dec = String(whole + Math.round(parseFloat(dec)) / Math.pow(10, decimals));
                    var dot = dec.indexOf('.');
                    if (dot == -1 && decimals > 0) {
                        dec += '.';
                        dot = dec.indexOf('.');
                    }
                    while (dec.length <= dot + decimals) { dec += '0'; }
                    result = dec;
                } else {
                    var dot;
                    var dec = new String(whole);
                    dec += '.';
                    dot = dec.indexOf('.');
                    while (dec.length <= dot + decimals) { dec += '0'; }
                    result = dec;
                }
                if (negative == true) {
                    result = '-' + result;
                }
                return result;
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
        }
    }
} (jQuery);

jQuery.fn.OuterHTML = function () {
    return $('<div></div>').append(this.clone()).html();
}

tinyMCEPopup.onInit.add(NumericResponseEditorDialog.init, NumericResponseEditorDialog);