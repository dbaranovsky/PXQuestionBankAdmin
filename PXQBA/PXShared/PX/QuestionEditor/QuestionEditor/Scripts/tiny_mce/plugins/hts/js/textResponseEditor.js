/// <reference path="helper.js" />
tinyMCEPopup.requireLangPack();

var TextResponseEditorDialog = function ($) {
    var _static = {
        pluginName: "TextResponseEditor",
        dataKey: "TextResponseEditor",
        bindSuffix: ".TextResponseEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {},
        css: {},
        sel: {
            responseDialog: ".hts-dialog-text",
            responseDialogForm: ".hts-dialog-text form",
            responseDialogFormErrors: ".hts-dialog-text hts-form .errors",
            responseDialogFormInputs: ".hts-dialog-text .form input",

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
            rowAnswerInput: '.hts-answer-input',
            rowAnswerSpan: '.hts-answer-text',

            responseForm: '.hts-form',
            responseWidth: '.response-width', //value can be "auto" or "char"
            responseAutoWidth: '.response-width[value=auto]',
            responseCharWidth: '.response-width[value=char]',
            responseCharWidthInput: '.response-charWidth',
            responseWeighting: '.response-weighting'

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

            onlyNumber: function (e) {
                var code = e.which,
                chr = String.fromCharCode(code), // key pressed converted to s string
                cur = $(this).val(),
                newVal = parseFloat(cur + chr); // what the input box will contain after this key press

                // Only allow numbers, periods, backspace, tabs and the enter key
                if (code !== 190 && code !== 8 && code !== 9 && code !== 13 && !/[0-9]/.test(chr)) {
                    return false;
                }

                // If this keypress would make the number
                // out of bounds, ignore it
                if (newVal == NaN || (newVal < 0 || newVal > 10)) {
                    return false;
                }
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
                    case "char":
                        $(_static.sel.responseCharWidthInput).removeAttr('disabled');
                        $(_static.sel.responseCharWidthInput).focus();
                        break;
                    case "auto":
                        $(_static.sel.responseCharWidthInput).val(""); //default width
                        $(_static.sel.responseCharWidthInput).attr('disabled', 'disabled');
                        break;
                }
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

            getNewRow: function () {
                var gridTemplate = $(_static.sel.gridTemplate),
                        gridRowTemplate = gridTemplate.find(_static.sel.gridDataRow),
                        newGridRow = gridRowTemplate.clone();

                return newGridRow;
            },

            onAddRowClick: function () {
                var grid = $(_static.sel.grid),
                    newGridRow = _static.fn.getNewRow();

                _static.fn.checkPendingRows();

                if (grid.find(_static.sel.gridPendingRow).length == 0) {
                    newGridRow.appendTo(grid);
                    var newName = 'hts-answer-input' + grid.length;
                    newGridRow.attr('name', newName);
                    newGridRow.find(_static.sel.editRowButton).click();
                    //newGridRow.find('.hts-answer-input').focus();
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
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan);

                //swap span/textbox
                rowAnswerSpan.hide();
                rowAnswerInput.val(rowAnswerSpan.text());
                rowAnswerInput.show();

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
                helper.confirmDialog(
                    'Apply Changes',
                    'Are you sure you want to delete this answer?',
                    'Delete',
                    "Don't Delete",
                    function () {
                        currentGridRow.remove();
                    },
                    function () { });

                /*if (confirm('Are you sure you want to delete this answer?')) {
                    currentGridRow.remove();
                }*/
            },

            onSaveRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowAnswerInput = currentGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan);

                if (rowAnswerInput.val()) {
                    //swap span/textbox
                    rowAnswerInput.hide();
                    rowAnswerSpan.text($.trim(rowAnswerInput.val()));
                    rowAnswerSpan.show();

                    //add editrow class to current datarow for highlight
                    currentGridRow.removeClass('hts-edit-row');

                    //swap edit/delete icons for save/cancel icons
                    currentGridRow.find(_static.sel.editRowButton).show();
                    currentGridRow.find(_static.sel.deleteRowButton).show();
                    currentGridRow.find(_static.sel.saveRowButton).hide();
                    currentGridRow.find(_static.sel.cancelRowButton).hide();

                    if($(_static.sel.responseDialogForm).valid()) _static.fn.reset();
                }
                else {
                    $(_static.sel.responseDialogForm).valid();
                }
            },

            onCancelRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow),
                    rowAnswerInput = currentGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerSpan = currentGridRow.find(_static.sel.rowAnswerSpan);

                    //swap span/textbox
                    rowAnswerInput.hide();
                    rowAnswerInput.val('');
                    rowAnswerSpan.show();

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
                    var formulaImgBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path');
                    var imgBaseUrl = formulaImgBaseUrl.replace("geteq", "geticon");
                    var caption = response.Correct;
                    caption = encodeURIComponent(caption);

                    var imgSrc = imgBaseUrl + "?caption=" + caption + "&type=" + response.Type + "Response";

                    var responseImage = $('<img />');
                    responseImage.attr('src', imgSrc);
                    responseImage.attr('hts-data-type', 'text');
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
                $(_static.sel.responseAutoWidth).attr('checked', 'checked');
            },
            preloadAnswerRow: function (answer) {
                var grid = $(_static.sel.grid);
                var newGridRow = _static.fn.getNewRow(),
                    rowAnswerSpan = newGridRow.find(_static.sel.rowAnswerSpan);

                if (answer.Correct) {
                    var answerText = helper.htmlDecode(answer.Correct);
                    rowAnswerSpan.text(answerText);

                    newGridRow.appendTo(grid);
                }
            },

            preloadDialog: function (response) {
                var grid = $(_static.sel.grid);

                if (response.Size == 'auto') {
                    $(_static.sel.responseAutoWidth).attr('checked', 'checked');
                }
                else {
                    $(_static.sel.responseCharWidth).attr('checked', 'checked');
                    $(_static.sel.responseCharWidthInput).val(response.Size);
                }

                $(_static.sel.responseWeighting).val(response.Points);


                _static.fn.preloadAnswerRow(response);

                for (var i = 0; i < response.Answers.length; i++) {

                    _static.fn.preloadAnswerRow(response.Answers[i]);
                }

            },
            onTextDeleteHover: function (event) {
                var $this = $(this),
                parent = $this.parent(),
                    title = "Delete this correct answer?";

                if (event.type == 'mouseover') {
                    $this.addClass("hover");

                    var tooltip = $('<div class="delete-tooltip delete-tooltip-adjust"></div>').text(title);
                    tooltip.appendTo(parent);

                    tooltip.position({
                        my: 'right top',
                        at: 'right bottom',
                        of: $this,
                        offset: "1"
                    });

                } else {
                    parent.find('.delete-tooltip').remove();
                }
            },

            getResponseData: function () {
                var weighting = $(_static.sel.responseWeighting).val(),
                    size = $(_static.sel.responseCharWidthInput).val(),
                    type = "text";

                if ($(_static.sel.responseAutoWidth).is(':checked')) {
                    size = "auto";
                }

                var response = {};
                response.ElementId = _static.settings.responseData.responseId;
                response.Type = type;
                response.Size = size;
                response.Points = weighting;
                response.Answers = [];


                $(_static.sel.gridDataRows).each(function (idx) {
                    var answer = {};
                    var row = $(this),
                        rowAnswer = row.find(_static.sel.rowAnswerSpan);

                    if (idx == 0) {
                        response.Correct = helper.htmlEncode(rowAnswer.text());
                    }
                    else {

                        answer.Correct = helper.htmlEncode(rowAnswer.text());
                        //answer.Tolerance = rowAnswer.text();
                        //answer.Points = rowAnswer.text();
                        //answer.AnswerRule = rowAnswer.text();

                        response.Answers.push(answer);
                    }
                });

                return response;
            },

            bindControls: function () {

                $(_static.sel.addRowButton).die('click');
                $(_static.sel.editRowButton).die('click');
                $(_static.sel.deleteRowButton).die('click');
                $(_static.sel.saveRowButton).die('click');
                $(_static.sel.cancelRowButton).die('click');
                $(_static.sel.responseWidth).die('click');

                $(_static.sel.saveDialogButton).die('click');
                $(_static.sel.cancelDialogButton).die('click');

                $(_static.sel.addRowButton).live("click", _static.fn.onAddRowClick);
                $(_static.sel.editRowButton).live("click", _static.fn.onEditRowClick);
                $(_static.sel.deleteRowButton).live("click", _static.fn.onDeleteRowClick);
                $(_static.sel.saveRowButton).live("click", _static.fn.onSaveRowClick);
                $(_static.sel.cancelRowButton).live("click", _static.fn.onCancelRowClick);
                $(_static.sel.responseWidth).live('click', _static.fn.onWidthClicked);

                $(_static.sel.saveDialogButton).live("click", _static.fn.onSaveDialogClick);
                $(_static.sel.cancelDialogButton).live("click", _static.fn.onCancelDialogClick);

                $(_static.sel.deleteRowButton).die('mouseover mouseout');
                $(_static.sel.deleteRowButton).live('mouseover mouseout', _static.fn.onTextDeleteHover);

                $(_static.sel.responseCharWidthInput).die('keypress').live('keypress', _static.fn.onlyNumber);
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


tinyMCEPopup.onInit.add(TextResponseEditorDialog.init, TextResponseEditorDialog);