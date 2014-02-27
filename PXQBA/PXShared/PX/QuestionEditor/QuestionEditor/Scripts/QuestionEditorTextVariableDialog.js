// QuestionEditorVariables Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables.
//
//
(function ($) {
    window.QuestionEditorTextVariableDialog = function () {
        var _static = {
                    varTypes: {
                inMode: "new"
            },
            varOldName: "",
            sel: {
                variableDialog: ".hts-text-variable-dialog",
                variableDialogForm: ".hts-text-variable-dialog form",
                variableDialogFormErrors: ".hts-text-variable-dialog form .errors",
                variableDialogFormInputs: ".hts-text-variable-dialog .form input",

                saveButton: ".hts-text-variable-dialog .save-variable-button",
                cancelButton: ".hts-text-variable-dialog .cancel-variable-button",
                addConditionalButton: ".hts-text-variable-dialog .add-conditional",
                poolList: ".hts-text-variable-dialog .hts-pool-list",
                poolTemplate: ".hts-text-variable-dialog .hts-pool-template",
                conditionalPool: ".hts-text-variable-dialog .hts-conditional-pool",
                defaultPool: ".hts-text-variable-dialog .hts-default-pool",
                variablePool: ".hts-text-variable-dialog .hts-pool",                
                variablePoolDeleteButton: '.hts-text-variable-dialog .hts-pool .delete-button',

                //pool local selectors
                variableNameInput: ".hts-variable-name",
                variablePoolInclusionBox: ".hts-variable-pool-inclusion",
                variablePoolValuesInputs: ".hts-variable-pool-values",
                variablePoolConditionInputs: '.hts-variable-pool-condition-inputs',
                variablePoolConditionType: '.hts-variable-pool-condition-type',
                variablePoolConditionExpressionLeft: '.hts-variable-pool-condition-left-expression',
                variablePoolConditionExpressionRight: '.hts-variable-pool-condition-right-expression'
            },
            fn: {
                preload: function (variable) {

                    var dialog = $(_static.sel.variableDialog);
                    dialog.find(_static.sel.variableNameInput).val(variable.Name);
                    _static.varOldName = variable.Name;

                    if(variable.Constraints) {
                        $.each(variable.Constraints, function (i, c) {
                            var pool = null;
                            if (i == 0) {
                                pool = _static.fn.addDefaultPool();
                            }
                            else {
                                pool = _static.fn.addConditionalPool();
                            }

                            if (c.Condition) {
                                pool.find(_static.sel.variablePoolConditionExpressionLeft).val(c.Condition.Expression1);
                                pool.find(_static.sel.variablePoolConditionExpressionRight).val(c.Condition.Expression2);
                                pool.find(_static.sel.variablePoolConditionType).val(c.Condition.Type);
                            }

                            if (c.Inclusions) {
                                pool.find(_static.sel.variablePoolValuesInputs).val(c.Inclusions);
                            }
                        });
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

                    //clear out existing values from dialog
                    _static.fn.reset();

                    if (variable) {
                        _static.varTypes.inMode = "edit";
                        _static.fn.preload(variable);
                    }
                    else {
                        _static.varTypes.inMode = "new";
                        _static.fn.addDefaultPool();
                    }

                    variableDialog.position({
                        my: "right top",
                        at: "left top",
                        of: rightPane,
                        collision: "fit"
                    });

                    $(_static.sel.variableNameInput).focus();
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
                    var pools = $(_static.sel.variablePool).not(_static.sel.poolTemplate);
                    pools.remove();

                    var formInputs = $(_static.sel.variableDialogFormInputs);
                    formInputs.val('');

                    _pluginStatic.fn.setDialogValidation($(_static.sel.variableDialog));

                    $.validator.addClassRules("hts-variable-name", {
                        nameRegex: /^[a-zA-Z]([a-zA-Z0-9_]*)$/
                    });
                },

                cancel: function () {
                    var dialog = $(_static.sel.variableDialog);
                    dialog.hide();
                    _static.fn.reset();
                },

                getVariablePoolData: function (varPool) {
                    var constraint = {};
                    
                    constraint.Inclusions = varPool.find(_static.sel.variablePoolValuesInputs).val();

                    var conditionInputs = varPool.find(_static.sel.variablePoolConditionInputs);
                    if (conditionInputs.is(':visible')) {
                        constraint.Condition = {};
                        constraint.Condition.Expression1 = "";
                        constraint.Condition.Expression2 = "";
                        constraint.Condition.Type = "";
                        constraint.Condition.Type = varPool.find(_static.sel.variablePoolConditionType).val();
                        constraint.Condition.Expression1 = varPool.find(_static.sel.variablePoolConditionExpressionLeft).val();
                        constraint.Condition.Expression2 = varPool.find(_static.sel.variablePoolConditionExpressionRight).val();

                    }

                    return constraint;
                },

                getVariableData: function () {
                    var dialog = $(_static.sel.variableDialog);
                    var name = $.trim(dialog.find(_static.sel.variableNameInput).val()),
                    type = "text";

                    var variable = {};
                    variable.Type = type;
                    variable.Name = name;
                    variable.OldName = _static.varOldName;

                    variable.Constraints = [];
                    $(_static.sel.variablePool).each(function (idx) {
                        var currPool = $(this);
                        if (!currPool.hasClass('hts-pool-template')) {
                            var constraint = _static.fn.getVariablePoolData(currPool);
                            variable.Constraints.push(constraint);
                        }
                    });

                    return variable;
                },

                onSaveClick: function () {
                    if ($(_static.sel.variableDialogForm).valid()) {
                        _static.fn.save();
                    }
                },
                onCancelClick: function () {
                    _static.fn.cancel();
                },
                onAddConditionalClick: function () {
                    _static.fn.addConditionalPool();
                },

                addDefaultPool: function () {
                    if ($(_static.sel.defaultPool).length == 0) {
                        var parentDialog = $(_static.sel.variableDialog),
                            poolList = $(_static.sel.poolList),
                            poolTemplate = $(_static.sel.poolTemplate),
                            currentPools = $(_static.sel.variablePool),
                            newPool = poolTemplate.clone();

                        newPool.removeClass('hts-pool-template');
                        newPool.addClass('hts-default-pool');

                        newPool.prependTo(poolList);

                        return newPool;
                    }

                    return;
                },

                addConditionalPool: function () {
                    var parentDialog = $(_static.sel.variableDialog),
                            poolList = $(_static.sel.poolList),
                            poolTemplate = $(_static.sel.poolTemplate),
                            currentPools = $(_static.sel.variablePool),
                            newPool = poolTemplate.clone();

                    newPool.removeClass('hts-pool-template');
                    newPool.addClass('hts-conditional-pool');

                    newPool.appendTo(poolList);

                     //update the pool input names for validation plugin
                    newPool.find('input').each(function (idx) {
                        var input = $(this),
                            inputName = input.attr('name') + currentPools.length;
                        input.attr('name', inputName);
                    });

                    _static.fn.orderConditionalPools();

                    return newPool;
                },

                onDeletePoolClick: function () {
                    if (confirm('Are you sure you want to delete this pool?')) {
                        var pool = $(this).closest(_static.sel.variablePool);
                        pool.remove();

                        _static.fn.orderConditionalPools();
                    }
                },

                orderConditionalPools: function () {
                    $(_static.sel.conditionalPool).each(function (stepIdx) {
                        var poolIndex = stepIdx + 1;
                        var poolHeaderTitle = "Conditional Values Pool " + poolIndex;
                        $(this).find('span').text(poolHeaderTitle);
                    });
                },

            }
        };
        return {
            _pluginStatic: {},
            //returns new static information to be added to a plugin
            static: function (pluginStatic) {
                _pluginStatic = pluginStatic;

                return {
                    sel: {
                        //                        textVariableDialog: ".hts-text-variable-dialog",
                        //                        savetextVariableDialog: ".hts-text-variable-dialog .save-variable-button",
                        //                        canceltextVariableDialog: ".hts-text-variable-dialog .cancel-variable-button",
                        //                        addConditionaltextVariableDialog: ".hts-text-variable-dialog .add-conditional"
                    },
                    fn: {
                        loadTextVariableDialog: function (variable) {
                            _static.fn.load(variable);
                        },
                        saveTextVariableDialog: function () {
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
                $(_static.sel.addConditionalButton).die('click');
                $(_static.sel.variablePoolDeleteButton).die('click');

                $(_static.sel.saveButton).live("click", _static.fn.onSaveClick);
                $(_static.sel.cancelButton).live("click", _static.fn.onCancelClick);
                $(_static.sel.addConditionalButton).live("click", _static.fn.onAddConditionalClick);
                $(_static.sel.variablePoolDeleteButton).live("click", _static.fn.onDeletePoolClick);
            }
        };
    };
} (jQuery))








