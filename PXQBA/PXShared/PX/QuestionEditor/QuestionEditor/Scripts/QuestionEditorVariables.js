// QuestionEditorVariablesPanel Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables Panel.
//
//
(function ($) {
    window.QuestionEditorVariablesPanel = function () {
        return {
            _pluginStatic: {},
            //returns new static information to be added to a plugin
            static: function (pluginStatic) {
                _pluginStatic = pluginStatic;

                return {
                    sel: {
                        addVariableButton: ".hts-variables-panel #add-variable-button",
                        addVariableIcon: ".hts-variables-panel .add-variable-wrapper",
                        variableItemTemplate: ".hts-variables-panel .hts-variable-item-template",
                        variablesList: ".hts-variables-panel .hts-variables-list",
                        variableItem: ".hts-variables-panel .hts-variables-list .hts-variable-item",
                        variableRef: 'img[hts-data-type="variable"]',
                        variablesMenu: '.hts-variables-panel .hts-variables-menu',
                        variablesMenuItem: '.hts-variables-panel .hts-variables-menu li',
                        variableInsertButton: '.hts-variables-panel .hts-variable-actions-menu .hts-variable-insert',
                        variableEditButton: '.hts-variables-panel .hts-variable-actions-menu .hts-variable-edit',
                        variableDeleteButton: '.hts-variables-panel .hts-variable-actions-menu .hts-variable-delete',
                        variableActionsMenu: '.hts-variable-actions-menu',
                        variableSideDialogs: '.hts-side-dialog',
                        variableDisplayLimit: 20
                    },
                    fn: {
                        htmlEncode: function (str) {
                            var div = document.createElement('div');
                            var text = document.createTextNode(str);
                            div.appendChild(text);
                            return div.innerHTML;
                        },

                        htmlDecode: function (str) {
                            var div = document.createElement('div');
                            div.innerHTML = str;
                            return div.firstChild.data;
                        },
                        onAddVariableClick: function () {
                            var button = $(_pluginStatic.sel.addVariableButton),
                                menu = $(_pluginStatic.sel.variablesMenu);
                            menu.show();
                            menu.position({
                                my: "right top",
                                at: "right bottom",
                                of: button,
                                collision: "fit"
                            });
                        },

                        onAddVariableDialogClick: function () {
                            var $this = $(this);
                            var varType = $this.attr('hts-variable-type');

                            var menu = $(_pluginStatic.sel.variablesMenu);
                            menu.hide();

                            _pluginStatic.fn.loadVariableDialog(varType);
                        },

                        onEditVariableClick: function () {
                            var $this = $(this);
                            var variableId = $this.closest('li').attr('hts-variable-id');
                            var variable = QuestionEditorHelper.getVariableData(variableId);
                            if (variable) {
                                _pluginStatic.fn.editVariableDialog(varType);
                            }
                        },

                        onVariablesMenuHoverOut: function () {
                            $(this).hide();
                        },

                        onVariableItemHover: function (event) {
                            var menu = $(this).find(_pluginStatic.sel.variableActionsMenu);
                            if (event.type == 'mouseover') {
                                // do something on mouseover
                                menu.show();
                                if ($(this).hasClass('hts-variable-type-matharray') || $(this).hasClass('hts-variable-type-math')) {
                                    var insertMenu = menu.find('.hts-variable-insert');
                                    insertMenu.addClass('hts-variable-insert-block');
                                }
                            } else {
                                // do something on mouseout
                                menu.hide();
                                if ($(this).hasClass('hts-variable-type-matharray') || $(this).hasClass('hts-variable-type-math')) {
                                    var insertMenu = menu.find('.hts-variable-insert');
                                    insertMenu.removeClass('hts-variable-insert-block');
                                }
                            }
                        },

                        onVariableInsertHover: function (event) {
                            var $this = $(this),
                                parent = $this.parent(),
                                li = $this.closest('li'),
                                mathVariable = false,
                                title = "Math variables can only be inserted into formulas. <br /> Please insert a formula and enter the variable there.";

                            if (li.hasClass('hts-variable-type-matharray') || li.hasClass('hts-variable-type-math')) {
                                mathVariable = true;
                            }

                            if (mathVariable == true) {
                                if (event.type == 'mouseover') {
                                    var tooltip = $('<div class="insert-variable-tooltip"></div>').html(title);
                                    tooltip.appendTo(parent);

                                    tooltip.position({
                                        my: 'top left',
                                        of: $this,
                                        offset: "20",
                                        collision: "none none"
                                    });

                                } else {
                                    parent.find('.insert-variable-tooltip').remove();
                                }
                            }
                        },

                        onVariableRefHover: function (event) {
                            var $this = $(this);
                            var position = $this.position();
                            var confirmText = "Are you sure you want to delete this ?";

                            //                            var dlg = $('<div>' + confirmText + '</div>').dialog({
                            //                                draggable: false,
                            //                                resizable: false,
                            //                                width: 250,
                            //                                dialogClass: 'dialog-delete-container',
                            //                                position: {
                            //                                    my: 'right bottom',
                            //                                    at: 'right top',
                            //                                    of: ctr.find(_static.sel.deleteButton).first(),
                            //                                    offset: "5"
                            //                                },
                            //                                buttons: {
                            //                                    OK: function () {
                            //                                        // if deleting sub-node, show button to add it back.
                            //                                        if (ctr.hasClass('sub-node')) {
                            //                                            var parentStep = ctr.closest(_static.sel.step),
                            //                                    menu = parentStep.find(_static.sel.questionStepMenu),
                            //                                    button = menu.find(_static.sel.stepButtonByName(type));

                            //                                            button.show();
                            //                                        }

                            //                                        if (ctr.hasClass('solution')) {
                            //                                            $(_static.sel.addSolutionButton).removeAttr('disabled');
                            //                                        }

                            //                                        ctr.find('div.body').each(function () {
                            //                                            var editorId = $(this).attr('id');
                            //                                            if (editorId && tinyMCE.getInstanceById(editorId)) {
                            //                                                tinyMCE.execCommand('mceFocus', false, editorId);
                            //                                                tinyMCE.execCommand('mceRemoveControl', false, editorId);
                            //                                            }
                            //                                        });

                            //                                        ctr.remove();

                            //                                        $(this).dialog('destroy');
                            //                                    },
                            //                                    Cancel: function () {
                            //                                        $(this).dialog('destroy');
                            //                                    }
                            //                                }
                            //                            });

                            if (event.type == 'mouseover') {
                                // do something on mouseover
                                //menu.show();
                            } else {
                                // do something on mouseout
                                //menu.hide();
                            }
                        },

                        onVariableInsertClick: function () {
                            var variableItem = $(this).closest(_pluginStatic.sel.variableItem),
                                variableName = variableItem.find('.name').text();

                            if ($(this).hasClass('hts-variable-insert-block') == false) {
                                var variable = QuestionEditorHelper.getVariableData(variableName);
                                if (variable) {
                                    tinyMCE.activeEditor.execCommand('htsInsertVariableInstance', variable);
                                }
                            }
                        },

                        onVariableEditClick: function () {

                            var variableItem = $(this).closest(_pluginStatic.sel.variableItem),
                                variableName = variableItem.find('.name').text();

                            var variable = QuestionEditorHelper.getVariableData(variableName);

                            if (variable) {
                                _pluginStatic.fn.editVariableDialog(variable);
                            }
                        },

                        onVariableDeleteClick: function () {
                            if (confirm('Deleting variable will delete all its references from Question!\r\nAre you sure you want to continue? ')) {
                                var variableItem = $(this).closest(_pluginStatic.sel.variableItem),
                                variableName = variableItem.find('.name').text();

                                $(this).closest(_pluginStatic.sel.variableItem).remove();
                                QuestionEditorHelper.removeVariableData(variableName);
                            }
                        },

                        loadVariableDialog: function (variableType) {
                            $(_pluginStatic.sel.variableSideDialogs).hide();

                            switch (variableType) {
                                case 'numeric':
                                    _pluginStatic.fn.loadNumericVariableDialog();
                                    break;
                                case 'text':
                                    _pluginStatic.fn.loadTextVariableDialog();
                                    break;
                                case 'math':
                                    _pluginStatic.fn.loadMathVariableDialog();
                                    break;
                                case 'numarray':
                                    _pluginStatic.fn.loadNumericArrayDialog();
                                    break;
                                case 'textarray':
                                    _pluginStatic.fn.loadTextArrayDialog();
                                    break;
                                case 'matharray':
                                    _pluginStatic.fn.loadMathArrayDialog();
                                    break;
                                default:
                                    alert('invalid variable type!');
                            }
                        },

                        editVariableDialog: function (variable) {
                            $(_pluginStatic.sel.variableSideDialogs).hide();
                            switch (variable.Type) {
                                case 'numeric':
                                    _pluginStatic.fn.loadNumericVariableDialog(variable);
                                    break;
                                case 'text':
                                    _pluginStatic.fn.loadTextVariableDialog(variable);
                                    break;
                                case 'math':
                                    _pluginStatic.fn.loadMathVariableDialog(variable);
                                    break;
                                case 'numarray':
                                    _pluginStatic.fn.loadNumericArrayDialog(variable);
                                    break;
                                case 'textarray':
                                    _pluginStatic.fn.loadTextArrayDialog(variable);
                                    break;
                                case 'matharray':
                                    _pluginStatic.fn.loadMathArrayDialog(variable);
                                    break;
                                default:
                                    alert('invalid variable type!');
                            }
                        },

                        setDialogValidation: function (dialog) {
                            //                            $.validator.addMethod(
                            //                                 "regex",
                            //                                    function (value, element, regexp) {
                            //                                        var re = new RegExp(regexp);
                            //                                        return re.test(value);
                            //                                    },
                            //                                    "Please check your input."
                            //                            );

                            $.validator.addMethod(
                                "nameRegex",
                                function (value, element, regexp) {
                                    var re = new RegExp(regexp);
                                    return re.test(value);
                                },
                                "Please check your input."
                            );

                            var dialogForm = dialog.find('form');
                            var validator = dialogForm.validate({
                                ignore: ':hidden',
                                errorClass: "invalid",
                                validClass: "valid",
                                errorPlacement: function (error, element) {
                                    //error.appendTo($(_static.sel.variableDialogFormErrors));
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
                                        dialogForm.find('.form .errors').html(message);
                                        dialogForm.find('.form .errors').show();
                                    } else {
                                        dialogForm.find('.form .errors').hide();
                                    }
                                }
                            });

                            dialogForm.find('.form .errors').hide();
                            validator.resetForm();
                        },

                        getVariableDetails: function (variable) {
                            var sDetails = "";
                            var constraint = {};

                            if (variable.Constraints) constraint = variable.Constraints[0];

                            if (constraint) {
                                switch (variable.Type) {
                                    case "numeric":
                                        var ranges = constraint.Ranges;
                                        if (ranges) {
                                            sDetails = "range[" + ranges[0].Expression + "," + ranges[1].Expression + "]";
                                        }

                                        if (constraint.Inclusions) {
                                            sDetails += sDetails != "" ? ", " : " ";
                                            sDetails += "list[" + constraint.Inclusions + "]";
                                        }

                                        if (constraint.Exclusions) {
                                            if (constraint.Exclusions) sDetails += sDetails != "" ? ", " : " ";
                                            sDetails += "exclude[" + constraint.Exclusions + "]";
                                        }
                                        break;
                                    case "text":
                                        if (constraint.Inclusions) sDetails = "{" + constraint.Inclusions + "}";
                                        break;
                                    case "numarray":
                                        if (constraint.Inclusions) sDetails = "array[" + constraint.Inclusions + "]";
                                        break;
                                    case "textarray":
                                        if (constraint.Inclusions) sDetails = "[" + constraint.Inclusions + "]";
                                        break;
                                    case "math":
                                        if (constraint.Inclusions) sDetails = "[" + decodeURIComponent(constraint.Inclusions) + "]";
                                        break;
                                    case "matharray":
                                        if (constraint.Inclusions) sDetails = "[" + decodeURIComponent(constraint.Inclusions) + "]";
                                        break;
                                }
                            }

                            if (sDetails.length > _pluginStatic.sel.variableDisplayLimit) {
                                sDetails = sDetails.substring(0, _pluginStatic.sel.variableDisplayLimit) + "...";
                            }

                            return sDetails;
                        },
                        addVariableItem: function (variable) {
                            var variablesList = $(_pluginStatic.sel.variablesList),
                                variableItems = $(_pluginStatic.sel.variableItem),
                                variableTemplate = $(_pluginStatic.sel.variableItemTemplate),
                                existingItem = variablesList.find('.hts-variable-item[hts-variable-id="' + variable.Name + '"]'),
                                variableTypeClass = 'hts-variable-type-' + variable.Type,
                                variableAltClass = 'alt';


                            if (existingItem.length) {
                                existingItem.find('.details').text(_pluginStatic.fn.getVariableDetails(variable));
                            }
                            else {
                                var newItem = variableTemplate.clone();
                                newItem.removeClass('hts-variable-item-template');
                                if (variableItems.length % 2 == 0) {
                                    newItem.addClass(variableAltClass);
                                }

                                newItem.addClass('hts-variable-item');
                                newItem.addClass(variableTypeClass);

                                newItem.attr('hts-variable-id', variable.Name);

                                newItem.find('.name').text(variable.Name);
                                newItem.find('.details').text(_pluginStatic.fn.getVariableDetails(variable));

                                newItem.appendTo(variablesList);
                            }
                        },

                        loadVariableItems: function () {
                            var data = _pluginStatic.settings.htsData,
                                variables = data.VariableLookup,
                                variableItems = $(_pluginStatic.sel.variableItem);
                            if (variables.length != variableItems.length) {
                                variableItems.remove();
                                $.each(data.VariableLookup, function (Idx, currVariable) {
                                    _pluginStatic.fn.addVariableItem(currVariable);

                                });
                            }
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
                $(_pluginStatic.sel.addVariableButton).die('click');
                $(_pluginStatic.sel.addVariableButton).live("click", _pluginStatic.fn.onAddVariableClick);

                $(_pluginStatic.sel.addVariableIcon).die('click').live('click', _pluginStatic.fn.onAddVariableClick);

                $(_pluginStatic.sel.variablesMenuItem).die('click');
                $(_pluginStatic.sel.variablesMenuItem).live("click", _pluginStatic.fn.onAddVariableDialogClick);


                $(_pluginStatic.sel.variablesMenu).hover(function () { }, _pluginStatic.fn.onVariablesMenuHoverOut);

                $(_pluginStatic.sel.variableItem).die('mouseover mouseout');
                $(_pluginStatic.sel.variableItem).live('mouseover mouseout', _pluginStatic.fn.onVariableItemHover);

                $(_pluginStatic.sel.variableInsertButton).die('click');
                $(_pluginStatic.sel.variableInsertButton).die('mouseover mouseout');
                $(_pluginStatic.sel.variableEditButton).die('click');
                $(_pluginStatic.sel.variableDeleteButton).die('click');

                $(_pluginStatic.sel.variableInsertButton).live("click", _pluginStatic.fn.onVariableInsertClick);
                $(_pluginStatic.sel.variableInsertButton).live('mouseover mouseout', _pluginStatic.fn.onVariableInsertHover);
                $(_pluginStatic.sel.variableEditButton).live("click", _pluginStatic.fn.onVariableEditClick);
                $(_pluginStatic.sel.variableDeleteButton).live("click", _pluginStatic.fn.onVariableDeleteClick);
            }
        };
    };
} (jQuery))