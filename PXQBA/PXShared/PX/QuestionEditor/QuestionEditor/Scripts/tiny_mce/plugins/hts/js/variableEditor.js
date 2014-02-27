tinyMCEPopup.requireLangPack();

var VariableEditorDialog = function ($) {
    var _static = {
        pluginName: "VariableEditor",
        dataKey: "VariableEditor",
        bindSuffix: ".VariableEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {},
        css: {},
        sel: {
            questionEditor: '#question-editor',
            variableDialog: '.hts-dialog-variable',
            saveDialogButton: '.hts-dialog-save',
            cancelDialogButton: '.hts-dialog-cancel',
            removeVariableButton: '.hts-dialog-remove-variable',
            variableArrayInputs: '.hts-variable-array-inputs',
            variableInputs: '.hts-variable-inputs',
            variableIndexInput: '.hts-variable-index-input',
            variableName: '.hts-variable-name',
            variableForm: '.hts-form'
        },
        //private functions
        fn: {

            onSaveDialogClick: function () {
                var variableDialog = $(_static.sel.variableDialog),
                    variableIndexInput = variableDialog.find(_static.sel.variableIndexInput);

                var variableNode = tinyMCEPopup.getWindowArg("variable_image_node"),
                    variableType = variableNode.attr('hts-data-variable-type'),
                    variableIndex = variableNode.attr('hts-data-variable-index'),
                    isVariableArray = (variableType.indexOf('array') > -1),
                    variableName = variableNode.attr('hts-data-id'),
                    variableImageBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path'),
                    imgBaseUrl = variableImageBaseUrl.replace("geteq", "geticon"),
                    variableImgSrc = imgBaseUrl + "?caption=";
                    

                if (isVariableArray) {
                    var newIndex = variableIndexInput.val();
                    if ($.isNumeric(newIndex) == false) {
                        //if (newIndex.substring(0, 1) != "~") newIndex = "~" + newIndex + "\\\\";
                    }
                    if (window.parent.QuestionEditorHelper.isValidSubscript(variableName, newIndex)) {
                        variableNode.attr('hts-data-variable-index', newIndex);
                        variableImgSrc += variableName + '[' + newIndex + ']&type=' + variableType + '_var';
                        variableNode.attr({ 'src': variableImgSrc, 'alt': variableImgSrc, 'data-mce-src': variableImgSrc, 'hts-data-equation': variableImgSrc });

                        if (newIndex != variableIndex) {
                            window.parent.QuestionEditorHelper.restoreBookMark(tinyMCEPopup.editor.editorId);
                            tinyMCEPopup.editor.execCommand('mceInsertContent', false, variableNode.OuterHTML());
                        }

                        tinyMCEPopup.close();
                    }
                    else {
                        alert('Invalid variable index');
                    }
                }
            },

            setTitle: function (type) {
                var sTitle = "";
                switch (type) {
                    case "numeric":
                        sTitle = "Numeric Variable";
                        break;
                    case "text":
                        sTitle = "Text Variable";
                        break;
                    case "math":
                        sTitle = "Math Variable";
                        break;
                    case "numarray":
                        sTitle = "Index Numeric Array Variable";
                        break;
                    case "textarray":
                        sTitle = "Index Text Array Variable";
                        break;
                    case "matharray":
                        sTitle = "Index Math Array Variable";
                        break;
                    default:
                        sTitle = "Variable";
                }

                $(document).attr("title", sTitle);
            },

            onCancelDialogClick: function () {
                tinyMCEPopup.close();
            },

            onRemoveVariableClick: function () {
                if (confirm("Are you sure you want to delete this variable instance?")) {
                    var variableNode = tinyMCEPopup.getWindowArg("variable_image_node");
                    window.parent.QuestionEditorHelper.restoreBookMark(tinyMCEPopup.editor.editorId);
                    tinyMCEPopup.editor.execCommand('mceInsertContent', false, "");
                }

                tinyMCEPopup.close();
            },

            bindControls: function () {

                $(_static.sel.saveDialogButton).die('click');
                $(_static.sel.cancelDialogButton).die('click');
                $(_static.sel.removeVariableButton).die('click');

                $(_static.sel.saveDialogButton).live("click", _static.fn.onSaveDialogClick);
                $(_static.sel.cancelDialogButton).live("click", _static.fn.onCancelDialogClick);
                $(_static.sel.removeVariableButton).live("click", _static.fn.onRemoveVariableClick);
            }
        }
    };

    return {
        init: function () {
            //        var f = document.forms[0];

            //        // Get the selected contents as text and place it in the input
            //        f.someval.value = tinyMCEPopup.editor.selection.getContent({ format: 'text' });
            //        f.somearg.value = tinyMCEPopup.getWindowArg('some_custom_arg');

            _static.fn.bindControls();

        },
        build: function () {
            var variableNode = tinyMCEPopup.getWindowArg("variable_image_node"),
                variableType = variableNode.attr('hts-data-variable-type'),
                variableIndex = variableNode.attr('hts-data-variable-index'),
                variableName = variableNode.attr('hts-data-id');

            var isEditMode = tinyMCEPopup.getWindowArg("variable_dialog_edit");

            var variableDialog = $(_static.sel.variableDialog),
                variableIndexInput = variableDialog.find(_static.sel.variableIndexInput),
                variableArrayInputs = variableDialog.find(_static.sel.variableArrayInputs),
                variableInputs = variableDialog.find(_static.sel.variableInputs),
                variableNames = variableDialog.find(_static.sel.variableName);

            _static.fn.setTitle(variableType);

            if (variableNode) {
                variableNames.text(variableName);
                var isVariableArray = (variableType.indexOf('array') > -1);

                if (isVariableArray) {
                    variableArrayInputs.show();

                    if (variableIndex == undefined) {
                        variableIndex = "1";
                    }

                    if (variableIndex) {
                        $(_static.sel.variableIndexInput).val(variableIndex);
                    }
                }
                else {
                    variableInputs.show();
                }

                if (isEditMode) {
                    $(_static.sel.removeVariableButton).show();
                }
            }
            else {
                alert('invalid variable!');
                tinyMCEPopup.close();
            }
        }
    }
} (jQuery);

jQuery.fn.OuterHTML = function () {
    return $('<div></div>').append(this.clone()).html();
}


tinyMCEPopup.onInit.add(VariableEditorDialog.init, VariableEditorDialog);