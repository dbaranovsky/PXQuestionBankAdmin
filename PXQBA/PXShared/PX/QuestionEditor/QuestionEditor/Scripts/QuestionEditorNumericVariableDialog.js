// QuestionEditorVariables Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables.
//
//
(function ($) {
    window.QuestionEditorNumericVariableDialog = function () {
        var _static = {
            varTypes: {
                inMode: "new"
            },
            varOldName: "",
            sel: {
                variableDialog: ".hts-numeric-variable-dialog",
                variableDialogForm: ".hts-numeric-variable-dialog form",
                variableDialogFormErrors: ".hts-numeric-variable-dialog form .errors",
                variableDialogFormInputs: ".hts-numeric-variable-dialog .form input",

                saveButton: ".hts-numeric-variable-dialog .save-variable-button",
                cancelButton: ".hts-numeric-variable-dialog .cancel-variable-button",
                addConditionalButton: ".hts-numeric-variable-dialog .add-conditional",
                poolList: ".hts-numeric-variable-dialog .hts-pool-list",
                poolTemplate: ".hts-numeric-variable-dialog .hts-pool-template",
                conditionalPool: ".hts-numeric-variable-dialog .hts-conditional-pool",
                defaultPool: ".hts-numeric-variable-dialog .hts-default-pool",
                variablePool: ".hts-numeric-variable-dialog .hts-pool",
                variablePoolExcludeCheck: ".hts-numeric-variable-dialog .hts-variable-pool-exclude",
                variablePoolType: ".hts-numeric-variable-dialog .hts-variable-pool-type",
                variablePoolDeleteButton: '.hts-numeric-variable-dialog .hts-pool .delete-button',

                //pool local selectors
                variableNameInput: ".hts-variable-name",
                variableDecimalPlacesInput: ".hts-variable-decimal-places",
                variablePoolExcludeCheckLocal: ".hts-variable-pool-exclude",
                variablePoolInclusionBox: ".hts-variable-pool-inclusion",
                variablePoolExclusionBox: ".hts-variable-pool-exclusion",
                variablePoolRangeInputs: ".hts-variable-pool-range-inputs",
                variablePoolListInputs: ".hts-variable-pool-list-inputs",
                variablePoolConditionInputs: '.hts-variable-pool-condition-inputs',
                variablePoolTypeGroup: ".hts-variable-pool-type",
                variablePoolRangeLeftType: '.hts-variable-pool-range-left-type',
                variablePoolRangeLeftExpression: '.hts-variable-pool-range-left-expression',
                variablePoolRangeRightType: '.hts-variable-pool-range-right-type',
                variablePoolRangeRightExpression: '.hts-variable-pool-range-right-expression',
                variablePoolConditionType: '.hts-variable-pool-condition-type',
                variablePoolConditionExpressionLeft: '.hts-variable-pool-condition-left-expression',
                variablePoolConditionExpressionRight: '.hts-variable-pool-condition-right-expression'
            },
            fn: {
                validObj: function () {
                    msg: "";
                },
                preload: function (variable) {

                    var dialog = $(_static.sel.variableDialog);
                    dialog.find(_static.sel.variableNameInput).val(variable.Name);
                    dialog.find(_static.sel.variableDecimalPlacesInput).val(variable.Format);

                    _static.varOldName = variable.Name;

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

                        if (c.Ranges) {
                            if (typeof c.Ranges[0] !== 'undefined' && c.Ranges[0] !== null) {
                                pool.find(_static.sel.variablePoolRangeLeftExpression).val(c.Ranges[0].Expression);
                                pool.find(_static.sel.variablePoolRangeLeftType).val(c.Ranges[0].Type);
                            }

                            if (typeof c.Ranges[1] !== 'undefined' && c.Ranges[1] !== null) {
                                pool.find(_static.sel.variablePoolRangeRightExpression).val(c.Ranges[1].Expression);
                                pool.find(_static.sel.variablePoolRangeRightType).val(c.Ranges[1].Type);
                            }
                        }

                        if (c.Exclusions) {
                            pool.find(_static.sel.variablePoolExclusionBox).val(c.Exclusions);
                            pool.find(_static.sel.variablePoolExcludeCheckLocal).attr('checked', 'checked');
                            pool.find(_static.sel.variablePoolExclusionBox).removeAttr('disabled');
                        }

                        if (c.Inclusions) {
                            pool.find(_static.sel.variablePoolInclusionBox).val(c.Inclusions);
                            pool.find(_static.sel.variablePoolTypeGroup + '[value="list"]').click();
                        }
                    });
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
                    var validChk = new _static.fn.validObj();
                    validChk.msg = "";

                    var valid = true,
                        dialog = $(_static.sel.variableDialog),
                        variable = _static.fn.getVariableData(validChk),
                        variableMem = QuestionEditorHelper.getVariableData(variable.Name);

                    if (validChk.msg != "") {
                        valid = false;
                        alert(validChk.msg);
                    }
                    else {
                        switch (_static.varTypes.inMode) {
                            case "new":
                                if (variableMem) {
                                    alert('Fail to save, duplicate variable found!');
                                    valid = false;
                                }
                                break;
                            case "edit":
                                break;
                            default:
                                valid = false;
                        }
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
                    $(_static.sel.variableDecimalPlacesInput).val("0");

                    _pluginStatic.fn.setDialogValidation($(_static.sel.variableDialog));

                    //                    $.validator.addClassRules("hts-variable-pool-exclusion", {
                    //                        regex: /^([0-9])+(,[0-9]+)*$/
                    //                    });

                    $.validator.addClassRules("hts-variable-name", {
                        nameRegex: /^[a-zA-Z]([a-zA-Z0-9_]*)$/
                    });
                },

                cancel: function () {
                    var dialog = $(_static.sel.variableDialog);
                    _static.fn.reset();

                    dialog.hide();
                },

                getVariablePoolData: function (varPool, validChk) {
                    var constraint = {};
                    var constraintType = varPool.find(_static.sel.variablePoolTypeGroup + ":checked").val();

                    validChk.msg = "";
                    if (constraintType == "range") {
                        constraint.Ranges = [];

                        var rangeLeft = {};
                        rangeLeft.Type = varPool.find(_static.sel.variablePoolRangeLeftType).val();
                        rangeLeft.Expression = varPool.find(_static.sel.variablePoolRangeLeftExpression).val();

                        var rangeRight = {};
                        rangeRight.Type = varPool.find(_static.sel.variablePoolRangeRightType).val();
                        rangeRight.Expression = varPool.find(_static.sel.variablePoolRangeRightExpression).val();

                        switch (rangeLeft.Type) {
                            case "lt":
                            case "le":
                                if (rangeRight.Type != "gt" && rangeRight.Type != "ge")
                                    validChk.msg = "No Range minimum has been defined!";
                                else if (parseInt(rangeLeft.Expression) < parseInt(rangeRight.Expression))
                                    validChk.msg = "Invalid Range expressions!";
                                break;
                            case "gt":
                            case "ge":
                                if (rangeRight.Type != "lt" && rangeRight.Type != "le")
                                    validChk.msg = "No Range maximum has been defined!";
                                else if (parseInt(rangeLeft.Expression) > parseInt(rangeRight.Expression))
                                    validChk.msg = "Invalid Range expressions!";
                                break;
                        }

                        constraint.Ranges.push(rangeLeft);
                        constraint.Ranges.push(rangeRight);
                    }
                    else {
                        constraint.Inclusions = varPool.find(_static.sel.variablePoolInclusionBox).val();
                    }

                    var excludeCheck = varPool.find(_static.sel.variablePoolExcludeCheckLocal);
                    if (excludeCheck.is(':checked')) {
                        constraint.Exclusions = varPool.find(_static.sel.variablePoolExclusionBox).val();
                    }

                    var conditionInputs = varPool.find(_static.sel.variablePoolConditionInputs);
                    if (conditionInputs.is(':visible')) {
                        constraint.Condition = {};
                        constraint.Condition.Type = varPool.find(_static.sel.variablePoolConditionType).val();
                        constraint.Condition.Expression1 = varPool.find(_static.sel.variablePoolConditionExpressionLeft).val();
                        constraint.Condition.Expression2 = varPool.find(_static.sel.variablePoolConditionExpressionRight).val();
                    }

                    return constraint;
                },

                getVariableData: function (validChk) {
                    var dialog = $(_static.sel.variableDialog);
                    var name = $.trim(dialog.find(_static.sel.variableNameInput).val()),
                    decimalPlaces = $.trim(dialog.find(_static.sel.variableDecimalPlacesInput).val()),
                    type = "numeric";

                    var variable = {};
                    variable.Type = type;
                    variable.Name = name;
                    variable.OldName = _static.varOldName;

                    variable.Format = decimalPlaces;

                    variable.Constraints = [];
                    $(_static.sel.variablePool).each(function (idx) {
                        var currPool = $(this);
                        if (!currPool.hasClass('hts-pool-template')) {
                            var constraint = _static.fn.getVariablePoolData(currPool, validChk);

                            //error found
                            if (validChk.msg != "") return;

                            variable.Constraints.push(constraint);
                        }
                    });

                    return variable;
                },

                onSaveClick: function (event) {
                    if ($(_static.sel.variableDialogForm).valid()) {
                        _static.fn.save();
                        event.stopPropagation();
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
                            newPool = poolTemplate.clone(),
                            newPoolTypeGroup = newPool.find(_static.sel.variablePoolTypeGroup),
                            newPoolTypeGroupName = 'hts-variable-pool-type-' + currentPools.length;

                        newPool.removeClass('hts-pool-template');
                        newPool.addClass('hts-default-pool');

                        // change the radio button group name
                        newPoolTypeGroup.attr('name', newPoolTypeGroupName);

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
                            newPool = poolTemplate.clone(),
                            newPoolTypeGroup = newPool.find(_static.sel.variablePoolTypeGroup),
                            newPoolTypeGroupName = 'hts-variable-pool-type-' + currentPools.length;

                    newPool.removeClass('hts-pool-template');
                    newPool.addClass('hts-conditional-pool');

                    // change the radio button group name
                    newPoolTypeGroup.attr('name', newPoolTypeGroupName);

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

                onVariablePoolExcludeClick: function () {
                    var $this = $(this),
                        parentPool = $this.closest(_static.sel.variablePool),
                        exclusionBox = parentPool.find(_static.sel.variablePoolExclusionBox);

                    if ($this.is(':checked')) {
                        exclusionBox.removeAttr('disabled');
                        exclusionBox.focus();
                    }
                    else {
                        exclusionBox.val('');
                        exclusionBox.attr('disabled', 'disabled');
                    }
                },

                onVariablePoolTypeClick: function () {

                    var $this = $(this),
                        parentPool = $this.closest(_static.sel.variablePool),
                        exclusionBox = parentPool.find(_static.sel.variablePoolExclusionBox),
                        selectedValue = $this.val(),
                        rangeInputs = parentPool.find(_static.sel.variablePoolRangeInputs),
                        listInputs = parentPool.find(_static.sel.variablePoolListInputs);

                    rangeInputs.hide();
                    listInputs.hide();
                    if (selectedValue == 'range') {
                        rangeInputs.show();
                    }
                    else {
                        listInputs.show();
                    }
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
                        //                        numericVariableDialog: ".hts-numeric-variable-dialog",
                        //                        saveNumericVariableDialog: ".hts-numeric-variable-dialog .save-variable-button",
                        //                        cancelNumericVariableDialog: ".hts-numeric-variable-dialog .cancel-variable-button",
                        //                        addConditionalnumericVariableDialog: ".hts-numeric-variable-dialog .add-conditional"
                    },
                    fn: {
                        loadNumericVariableDialog: function (variable) {
                            _static.fn.load(variable);
                        },
                        saveNumericVariableDialog: function () {
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
                $(_static.sel.variablePoolExcludeCheck).die('click');
                $(_static.sel.variablePoolTypeGroup).die('click');


                $(_static.sel.saveButton).live("click", _static.fn.onSaveClick);
                $(_static.sel.cancelButton).live("click", _static.fn.onCancelClick);
                $(_static.sel.addConditionalButton).live("click", _static.fn.onAddConditionalClick);
                $(_static.sel.variablePoolDeleteButton).live("click", _static.fn.onDeletePoolClick);

                $(_static.sel.variablePoolExcludeCheck).live("click", _static.fn.onVariablePoolExcludeClick);
                $(_static.sel.variablePoolType).live("click", _static.fn.onVariablePoolTypeClick);
            }
        };
    };
} (jQuery))








