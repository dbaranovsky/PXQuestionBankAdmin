tinyMCEPopup.requireLangPack();

var MultiResponseEditorDialog = function ($) {
    var _static = {
        pluginName: "MultiResponseEditor",
        dataKey: "MultiResponseEditor",
        bindSuffix: ".MultiResponseEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {},
        css: {},
        sel: {
            responseDialog: ".hts-dialog-mc",
            responseDialogForm: ".hts-dialog-mc form",
            responseDialogFormErrors: ".hts-dialog-mc hts-form .errors",
            responseDialogFormInputs: ".hts-dialog-mc .form input",

            questionEditor: '#question-editor',
            gridTemplate: '.hts-grid-template',
            grid: '.hts-grid',
            gridWrapper: '.hts-grid-wrapper',
            gridBody: '.hts-grid-body',
            gridToolbar: '.hts-grid-toolbar',
            gridDataRows: '.hts-grid .data-row',
            gridDataRow: '.data-row',
            gridPendingRow: '.hts-edit-row',
            saveDialogButton: '.hts-dialog-save',
            cancelDialogButton: '.hts-dialog-cancel',
            addRowButton: '.hts-grid-addrow',
            deleteRowButton: '.hts-grid-deleterow',
            lockRowButton: '.hts-grid-lockrow',
            unlockRowButton: '.hts-grid-unlockrow',
            rowAnswerInput: '.hts-answer-input',
            rowAnswerSpan: '.hts-answer-text',
            rowAnswerCorrectInput: '.hts-answer-correct-input',
            rowAnswerCorrectLabel: '.hts-answer-correct-label',
            variableList: '.variable-list',
            deleteRowButton: '.hts-grid-deleterow',
            responseForm: '.hts-form',
            responseScramble: '.response-scramble',
            responseIncludeVariable: '.response-includevariables',
            responseAreaOfIncludeVariable: '.hts-variable-expression',
            responseColumns: '.response-columns',
            responseWeighting: '.response-weighting',
            toolbar: '.mceExternalToolbar',
            tiny_mce_default_value: "click to edit"
        },
        //private functions
        fn: {

            addVariableData: function (variableData) {
                var variableList = $(_static.sel.variableList);
                variableList.html('<option value="-1" title="select">select</option>');
                if (variableData != undefined) {
                    $.each(variableData, function (variableName) {
                        variableList.html(variableList.html() + '<option value="$' + variableName + '">' + variableName + '</option>');
                    });
                }
            },

            checkDefaultText: function (ed) {
                ed.focus();
                // set the focus
                var cont = ed.getContent();

                // get the current content
                slen = cont.length;
                cont = cont.substring(3, slen - 4);

                // cut off <p> and </p> to comply with XHTML strict
                // these can't be part of the default_value 
                is_default = (cont == _static.sel.tiny_mce_default_value);

                // compare those strings
                if (!is_default)
                    return;

                // nothing to do
                ed.selection.select(ed.dom.select('p')[0]);
            },

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

            getCorrectLabel: function () {
                $(_static.sel.gridDataRows).each(function (idx) {
                    idx++; /* Incrementing by 1 to start index by 1 in place of 0 for PX-1281 */
                    var row = $(this),
                        rowAnswerCorrectLabel = row.find(_static.sel.rowAnswerCorrectLabel);

                    rowAnswerCorrectLabel.html(idx);
                });
            },

            getNewRow: function () {
                var gridTemplate = $(_static.sel.gridTemplate),
                        gridRowTemplate = gridTemplate.find(_static.sel.gridDataRow),
                        newGridRow = gridRowTemplate.clone();

                return newGridRow;
            },

            onAddRowClick: function () {
                var grid = $(_static.sel.grid),
                    gridRows = $(_static.sel.gridDataRows),
                    gridRowsCount = gridRows.length + 1,
                    newGridRow = _static.fn.getNewRow(),
                    newGridRowAnswerInput = newGridRow.find(_static.sel.rowAnswerInput),
                    newGridRowAnswerColumn = newGridRowAnswerInput.closest('td'),
                    newGridRowEditorId = "";

                newGridRowEditorId = "hts-mc-editor-" + gridRowsCount;
                newGridRowAnswerInput.remove();

                var textbox = $('<textarea class="hts-answer-input required" name="hts-answer-input"></textarea>');
                //textbox.html($(this).html());
                textbox.attr('id', newGridRowEditorId);
                textbox.html(_static.sel.tiny_mce_default_value);
                newGridRowAnswerColumn.append(textbox);

                var newName = 'hts-answer-input' + gridRowsCount;
                textbox.attr('name', newName);

                newGridRow.appendTo(grid);
                _static.fn.convertToMCE(newGridRowEditorId);
                _static.fn.getCorrectLabel();
            },

            onUnlockRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow);
                if (confirm('Are you sure you want to lock this choice?')) {
                    currentGridRow.find(_static.sel.lockRowButton).show();
                    currentGridRow.find(_static.sel.unlockRowButton).hide();
                }
            },

            onLockRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow);
                if (confirm('Are you sure you want to unlock this choice?')) {
                    currentGridRow.find(_static.sel.lockRowButton).hide();
                    currentGridRow.find(_static.sel.unlockRowButton).show();
                }
            },

            onDeleteRowClick: function () {
                var currentGridRow = $(this).closest(_static.sel.gridDataRow);
                helper.confirmDialog(
                    'Apply Changes',
                    'Are you sure you want to delete this choice?',
                    'Delete',
                    "Don't Delete",
                    function () {
                        currentGridRow.remove();
                        _static.fn.getCorrectLabel();
                    },
                    function () { });
                /*if (confirm('Are you sure you want to delete this choice?')) {
                currentGridRow.remove();
                }*/
            },
            onMulitDeleteHover: function (event) {
                var $this = $(this),
                parent = $this.parent(),
                    title = "Delete this correct answer?";

                if (event.type == 'mouseover') {
                    // do something on mouseover
                    $this.addClass("hover");

                    var tooltip = $('<div class="delete-tooltip delete-tooltip-adjust"></div>').text(title);
                    //style = "white-space:nowrap;text-align:right;position:absolute;"
                    tooltip.appendTo(parent);

                    tooltip.position({
                        my: 'right bottom',
                        at: 'right top',
                        of: $this,
                        offset: "1"
                    });

                } else {
                    // do something on mouseout
                    parent.find('.delete-tooltip').remove();

                }
            },
            onScrambleClick: function () {
                var isScrambleChecked = $(this).is(':checked'),
                    grid = $(_static.sel.grid),
                    lockButtons = $(_static.sel.lockRowButton),
                    unlockButtons = $(_static.sel.unlockRowButton);

                if (isScrambleChecked) {
                    if (confirm('Are you sure you want to scramble choices?')) {
                        lockButtons.hide();
                        unlockButtons.show();
                    }
                }
                else {
                    if (confirm('Are you sure you do not want to scramble choices?')) {
                        lockButtons.hide();
                        unlockButtons.hide();
                    }
                }
            },

            onSaveDialogClick: function () {
                if (_static.fn.getPendingRows()) {
                    if ($(_static.sel.responseDialogForm).valid()) {
                        var response = _static.fn.getResponseData();

                        if (response.Correct == "-1") {
                            if (response.IncludeVariable == false) {
                                alert('Please select correct response!');
                            } else {
                                alert('Please select correct choice defined by variable');
                            }
                            return;
                        }

                        var formulaImgBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path');
                        var imgBaseUrl = formulaImgBaseUrl.replace("geteq", "geticon");
                        var caption = "";
                        for (var i = 0; i < response.Choices.length; i++) {
                            caption += $(response.Choices[i].Text).text() + "|";
                        }

                        caption = encodeURIComponent(caption);

                        var imgSrc = imgBaseUrl + "?caption=" + caption + "&type=" + response.Type + "Response";

                        var responseImage = $('<img />');
                        responseImage.attr('src', imgSrc);
                        responseImage.attr('hts-data-type', 'multi');
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
                else
                    alert("Invalid choice data");
            },

            onCancelDialogClick: function () {
                _static.fn.reset();
                tinyMCEPopup.close();

            },

            onResponseIncludeVariable: function (event) {
                var checkBox = $(this);
                if ($(checkBox).is(':checked')) {
                    if ($(_static.sel.variableList).find('option').length <= 1) {
                        alert('Please define at least one numeric variable in variable collection.');
                        event.stopPropagation(true);
                        $(checkBox).attr('checked', false);
                        return;
                    }
                    _static.fn.toggleOptionWithLabel(true);
                } else {
                    _static.fn.toggleOptionWithLabel(false);
                }
            },

            toggleOptionWithLabel: function (includeVariableSelected, correctResponse) {
                if (includeVariableSelected == true) {
                    $(_static.sel.responseAreaOfIncludeVariable).show();
                    $(_static.sel.rowAnswerCorrectInput).hide();
                    $(_static.sel.rowAnswerCorrectLabel).show();
                } else {
                    $(_static.sel.responseAreaOfIncludeVariable).hide();
                    $(_static.sel.rowAnswerCorrectInput).show();
                    $(_static.sel.rowAnswerCorrectLabel).hide();
                }
                if (correctResponse != undefined) {
                    $(_static.sel.variableList).find('option[value=\\' + correctResponse + ']').attr('selected', 'selected');
                }
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
            },

            preloadDialog: function (response) {
                var grid = $(_static.sel.grid);
                var isScramble = "no";
                var responseCorrectIsNaN = isNaN(parseInt(response.Correct));

                if (response.Scramble == 'yes') {
                    isScramble = "yes";
                    $(_static.sel.responseScramble).attr('checked', 'checked');
                }

                if (response.IncludeVariable || responseCorrectIsNaN) {
                    $(_static.sel.responseIncludeVariable).attr('checked', 'checked');
                    _static.fn.toggleOptionWithLabel(true, response.Correct);
                } else {
                    _static.fn.toggleOptionWithLabel(false);
                }

                $(_static.sel.responseColumns).val(response.Columns);
                $(_static.sel.responseWeighting).val(response.Points);

                for (var i = 1; i <= response.Choices.length; i++) { /* Changed i=0 to 1 for PX-1281 */
                    var newGridRow = _static.fn.getNewRow(),
                    rowAnswerInput = newGridRow.find(_static.sel.rowAnswerInput),
                    rowAnswerInputColumn = rowAnswerInput.closest('td'),
                    rowCorrectCheck = newGridRow.find(_static.sel.rowAnswerCorrectInput),
                    rowLock = newGridRow.find(_static.sel.lockRowButton),
                    rowUnlock = newGridRow.find(_static.sel.unlockRowButton),
                    newGridRowEditorId = "hts-mc-editor-" + i;

                    rowAnswerInput.remove();

                    var answerText = response.Choices[i - 1].Text; /* Changed i=0 to 1 for PX-1281 */

                    var textbox = $('<textarea class="hts-answer-input"></textarea>');
                    //textbox.html($(this).html());
                    textbox.attr('id', newGridRowEditorId);
                    textbox.val(answerText);

                    rowAnswerInputColumn.append(textbox);

                    var choiceNumber = i;
                    if (choiceNumber == response.Correct) {
                        rowCorrectCheck.attr('checked', 'checked');
                    }

                    if (isScramble == "yes") {
                        if (response.Choices[i - 1].Fixed == "yes") { /* Changed i=0 to 1 for PX-1281 and PX-4018 */
                            rowLock.show();
                            rowUnlock.hide();
                        }
                        else {
                            rowLock.hide();
                            rowUnlock.show();
                        }
                    }

                    newGridRow.appendTo(grid);
                    _static.fn.convertToMCE(newGridRowEditorId);
                }
            },

            getPendingRows: function () {
                var bReturn = true;
                $(_static.sel.gridDataRows).each(function (idx) {
                    idx++; /* Incrementing ID by 1 so that choice can be 1 based in place of 0 based. PX-1281 */
                    var row = $(this),
                        rowAnswer = row.find(_static.sel.rowAnswerInput),
                        rowAnswerEditorId = rowAnswer.attr('id'),
                        nodeText = "";

                    if (rowAnswerEditorId && tinyMCE.getInstanceById(rowAnswerEditorId)) {
                        nodeText = tinyMCE.get(rowAnswerEditorId).getContent({ format: 'text' });
                    }

                    if ($.trim(nodeText) == "") {
                        bReturn = false;
                        return false; //exit loop
                    }
                });

                return bReturn;
            },

            getResponseData: function () {
                var weighting = $(_static.sel.responseWeighting).val(),
                    scrambleChoices = "no",
                    includeVariables = false,
                    columns = $(_static.sel.responseColumns).val(),
                    includeVariableResponse = $(_static.sel.variableList).find('option:selected').val();

                if ($(_static.sel.responseScramble).is(':checked')) {
                    scrambleChoices = "yes";
                }

                var response = {};
                response.ElementId = _static.settings.responseData.responseId;
                response.Type = "multi";
                response.Scramble = scrambleChoices;
                if ($(_static.sel.responseIncludeVariable).is(':checked')) {
                    includeVariables = true;
                    response.Correct = includeVariableResponse;
                } else {
                    response.Correct = "-1";
                }
                response.IncludeVariable = includeVariables;
                response.Columns = columns;
                response.Points = weighting;
                response.Choices = [];

                $(_static.sel.gridDataRows).each(function (idx) {
                    idx++; /* Incrementing by 1 to start index by 1 in place of 0 for PX-1281 */
                    var choice = {};
                    var row = $(this),
                        rowAnswer = row.find(_static.sel.rowAnswerInput),
                        isRowCorrect = row.find(_static.sel.rowAnswerCorrectInput).is(':checked'),
                        isRowFixed = row.find(_static.sel.unlockRowButton).is(':visible') ? "no" : "yes",
                        rowAnswerEditorId = rowAnswer.attr('id'),
                        nodeText = "";

                    if (rowAnswerEditorId && tinyMCE.getInstanceById(rowAnswerEditorId)) {
                        nodeText = tinyMCE.get(rowAnswerEditorId).getContent({ format: 'html' });
                    }

                    if ((includeVariables == false) && isRowCorrect) {
                        response.Correct = idx;
                    }

                    choice.ChoiceId = idx;
                    choice.Fixed = isRowFixed;
                    choice.Text = nodeText;

                    response.Choices.push(choice);
                });

                return response;
            },

            setupEditor: function (editor_id, body, doc) {
                var toolbarId = '#' + editor_id + "_external",
                toolbarCount = $(_static.sel.gridToolbar).find(_static.sel.toolbar).length,
                existingToolbar = $(_static.sel.gridToolbar).find(toolbarId),
                newToolbar = $(_static.sel.gridWrapper).find(toolbarId),
                visibleToolbars = $(_static.sel.gridToolbar).find(_static.sel.toolbar + ':visible');

                // remove existing toolbar
                if (existingToolbar.length) {
                    newToolbar.remove();
                    newToolbar = existingToolbar;
                }
                else {
                    // add new toolbar                
                    newToolbar.appendTo(_static.sel.gridToolbar);
                }

                // adjust toolbar visibility
                if (visibleToolbars.length) {
                    newToolbar.hide();
                }
                else {
                    newToolbar.show();
                }
            },

            removeEditor: function (inst) {
                var toolbarId = '#' + inst.editorId + "_external",
                existingToolbar = $(_static.sel.gridToolbar).find(toolbarId);

                if (existingToolbar.length) {
                    existingToolbar.remove();
                }
            },

            editorStoreBookmark: function (Id) {
                if (Id) {
                    var ed = tinyMCE.get(Id);
                    if (ed) {
                        _static.settings.editor_bookmark = _static.settings.editor_bookmark = ed.selection.getBookmark(1);
                    }
                }
            },            

            convertToMCE: function (editorId) {

                var windowMultiChoice = tinyMCEPopup.dom.doc.defaultView;

                if (windowMultiChoice == undefined) {
                    windowMultiChoice = tinyMCEPopup.dom.doc.parentWindow;
                }

                var args = {
                    element_id: editorId,
                    window: windowMultiChoice,
                    doc: tinymce.DOM.doc,
                    theme: "advanced",
                    mode: "none",
                    plugins: "inlinepopups,table,paste,nonbreaking,hts",
                    theme_advanced_toolbar_location: "external",
                    theme_advanced_toolbar_align: "left",
                    theme_advanced_buttons1: "bold,italic,underline,|,justifyleft,justifycenter,justifyright,|,link,unlink,|,bullist,numlist,|,htsFormulaEditor",
                    theme_advanced_buttons2: "",
                    theme_advanced_buttons3: "",
                    extended_valid_elements: "img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|iprof|hts-data-id|hts-data-type|hts-data-equation|hts-data-variable-type|hts-data-variable-index|function|datafile]",
                    add_unload_trigger: false,
                    remove_linebreaks: false,
                    equation_image_path: tinyMCEPopup.getWindowArg('equation_image_path'),
                    debug: false,
                    width: "100%",
                    height: "60",
                    setup: function (ed) {
                        ed.onPostRender.add(function (ed, cm) {
                            _static.fn.setupEditor(ed.id);
                        });
                        ed.onRemove.add(function (ed) {
                            _static.fn.removeEditor(ed);
                        });
                        ed.onEvent.add(function (ed) {
                            _static.fn.editorStoreBookmark(ed.editorId);
                        });
                        ed.onClick.add(function (ed, e) {

                            $(_static.sel.gridToolbar).find(_static.sel.toolbar).hide();
                            var toolbarId = '#' + ed.editorId + "_external",
                                                existingToolbar = $(_static.sel.gridToolbar).find(toolbarId);
                            existingToolbar.show();
                        });
                        ed.onMouseUp.add(function (ed) {
                            _static.fn.checkDefaultText(ed);
                        });
                        ed.onKeyPress.add(function (ed) {
                            _static.fn.checkDefaultText(ed);
                        });

                    }
                };

                tinyMCE.execCommand('mceAddFrameControl', true, args);
            },

            bindControls: function () {

                $(_static.sel.addRowButton).die('click');
                $(_static.sel.deleteRowButton).die('click');
                $(_static.sel.lockRowButton).die('click');
                $(_static.sel.unlockRowButton).die('click');
                $(_static.sel.responseScramble).die('click');

                $(_static.sel.saveDialogButton).die('click');
                $(_static.sel.cancelDialogButton).die('click');

                $(_static.sel.addRowButton).live("click", _static.fn.onAddRowClick);
                $(_static.sel.deleteRowButton).live("click", _static.fn.onDeleteRowClick);
                $(_static.sel.lockRowButton).live("click", _static.fn.onLockRowClick);
                $(_static.sel.unlockRowButton).live("click", _static.fn.onUnlockRowClick);
                $(_static.sel.responseScramble).live("click", _static.fn.onScrambleClick);

                $(_static.sel.deleteRowButton).die('mouseover mouseout');
                $(_static.sel.deleteRowButton).live('mouseover mouseout', _static.fn.onMulitDeleteHover);

                $(_static.sel.saveDialogButton).live("click", _static.fn.onSaveDialogClick);
                $(_static.sel.cancelDialogButton).live("click", _static.fn.onCancelDialogClick);

                $(_static.sel.responseIncludeVariable).die("click").live("click", _static.fn.onResponseIncludeVariable);
                _static.fn.toggleOptionWithLabel(false);

                $(window.parent.document).find('.mceClose').mousedown(function (e) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    e.stopPropagation();

                    tinyMCEPopup.close();
                });

                tinyMCEPopup.oldonclose = tinyMCEPopup.close; // preserve old onclose function
                tinyMCEPopup.close = _static.fn.onClose;

            },

            onClose: function () {

                // CUSTOM close handler
                // Needed because we are including a new tinyMCE instance in a tinyMCE plugin
                // Adding a new frame control to the parent resets the tinymce object to the parent 
                var args = {
                    element_id: '',
                    window: window.parent,
                    doc: window.oldDoc,
                    theme: "advanced"
                };
                tinyMCE.execCommand('mceAddFrameControl', true, args);

                // Reset the tinyMCE configuration to the base page configurations.
                window.parent.QuestionEditorHelper.initMCE();

                // execute old onclose function if there has been one
                if (typeof tinyMCEPopup.oldonclose == "function") {
                    tinyMCEPopup.oldonclose();
                }
            }
        }
    };

    return {
        init: function () {
            window.oldActiveEditor = tinyMCEPopup.editor;
            window.oldDoc = tinymce.DOM.doc;
        },
        build: function () {
            var responseData = tinyMCEPopup.getWindowArg("response_data");
            _static.settings.responseData = responseData;
            _static.fn.buildNewGrid();
            _static.fn.bindControls();

            if (responseData == undefined) {
            }
            else {
                _static.fn.addVariableData(responseData.numericVariable);
                if (responseData.mode == "new") {
                    _static.fn.setDefaults();
                    $(_static.sel.addRowButton).click();
                }
                if (responseData.mode == "edit") {
                    _static.fn.preloadDialog(responseData.response);
                }
                _static.fn.getCorrectLabel();
            }

            _static.fn.reset();
        }

    }
} (jQuery);

jQuery.fn.OuterHTML = function () {
    return $('<div></div>').append(this.clone()).html();
}


tinyMCEPopup.onInit.add(MultiResponseEditorDialog.init, MultiResponseEditorDialog);