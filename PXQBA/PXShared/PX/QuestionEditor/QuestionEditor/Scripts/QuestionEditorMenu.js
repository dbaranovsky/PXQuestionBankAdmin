/// <reference path="tiny_mce/plugins/hts/js/helper.js" />
// QuestionEditorVariablesPanel Module
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor Variables Panel.
//
//
(function ($) {
    window.QuestionEditorMenu = function () {
        var _static = {
            sel: {
                toggleViewButton: '.toggle-view-button',
                addStepButton: '.add-step-button',
                addSolutionButton: '.add-solution-button',
                showVariablesButton: '.show-variables-button',
                hideVariablesButton: '.hide-variables-button',
                saveButton: '.save-button',
                revertButton: '.revert-button'
            },
            fn: {
                onSaveClick: function (e, fnCallback) {
                    var saveMethod = _pluginStatic.fn.saveHtsData;

                    if ($(_pluginStatic.sel.xmlView).is(':visible'))
                        saveMethod = _static.fn.saveHtsXML;

                    saveMethod();
                    /*_pluginStatic.fn.confirmDialog(
                    'Apply Changes',
                    'Are you sure you want to save question?',
                    'Save',
                    'Cancel',
                    saveMethod,
                    function () { });*/
                },

                onShowVariablesClick: function () {
                    _pluginStatic.fn.loadVariableItems();
                    $(_pluginStatic.sel.contentWrapperTableRight).show();
                    $(_pluginStatic.sel.showVariablesButton).hide();
                    $(_pluginStatic.sel.hideVariablesButton).show();
                },
                onHideVariablesClick: function () {
                    $(_pluginStatic.sel.contentWrapperTableRight).hide();
                    $(_pluginStatic.sel.showVariablesButton).show();
                    $(_pluginStatic.sel.hideVariablesButton).hide();
                },

                onAddSolutionClick: function () {
                    if ($(_pluginStatic.sel.questionEditor).find(_pluginStatic.sel.solution).length == 0) {
                        var solution = _pluginStatic.fn.newSolution();
                        var steps = $(_pluginStatic.sel.stepsPane);
                        solution.insertAfter(steps);
                        solution.ToEditor();

                        if (_pluginStatic.sel.solutionEditorId && tinyMCE.getInstanceById(_pluginStatic.sel.solutionEditorId)) {
                            tinymce.execCommand('mceFocus', false, _pluginStatic.sel.solutionEditorId);
                        }

                        $(this).attr('disabled', 'disabled');
                    }
                },

                onAddStepClick: function () {
                    var stepList = $(_pluginStatic.sel.stepsPane);
                    var step = _pluginStatic.fn.newStep();
                    step.appendTo(stepList);
                    step.find(_pluginStatic.sel.addQuestionButton).click();
                },

                onRevertClick: function () {
                    var isCurrentModeXml = $(_pluginStatic.sel.xmlView).is(':visible');

                    $.getJSON(_pluginStatic.settings.LoadUrl, null, function (dataIn) {
                        _pluginStatic.settings.htsData = dataIn;

                        if (isCurrentModeXml) {
                            _static.fn.revertRawXMLData();
                        }
                        else {
                            var data = _pluginStatic.settings.htsData;
                            _pluginStatic.fn.buildUI(data);
                        }
                    });
                },

                revertRawXMLData: function () {
                    var rawXML = _pluginStatic.settings.htsData.RawXML;
                    $(_pluginStatic.sel.xmlViewBody).val(vkbeautify.xml(rawXML))
                },

                onToggleViewClick: function () {
                    var $this = $(this),
                        isCurrentModeXml = $(_pluginStatic.sel.xmlView).is(':visible'),
                        isXmlrequest = $this.hasClass('xml-view');

                    if (isXmlrequest && !isCurrentModeXml) {
                        _static.fn.toggleXML();
                    }
                    else if (!isXmlrequest && isCurrentModeXml) {
                        var pendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                        if (pendingXML != _pluginStatic.settings.htsData.RawXML) {
                            var div = $("<div></div>");
                            div.html('You have made changes to the question XML. Would you like to apply these changes?');
                            $(div).dialog({
                                resizable: false,
                                dialogClass: 'dialog-confirm',
                                height: 200,
                                modal: true,
                                title: 'Apply Changes',
                                open: function (event, ui) {
                                    $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
                                },
                                buttons: [{
                                    text: 'Apply',
                                    click: function () {
                                        _static.fn.updateHtsXML();
                                        $(this).dialog("close");
                                    }
                                }, {
                                    text: 'Discard',
                                    click: function () {
                                        _static.fn.toggleUI();
                                        $(this).dialog("close");
                                    }
                                }, {
                                    text: 'Cancel',
                                    click: function () {
                                        $(this).dialog("close");
                                    }
                                }]
                            });
                        }
                        else {
                            _static.fn.toggleUI();
                        }
                    }
                },

                showXMLView: function (response) {
                    $(_pluginStatic.sel.xmlView).show();
                    $(_pluginStatic.sel.htmlView).hide();
                    var formattedXML = vkbeautify.xml(response);
                    $(_pluginStatic.sel.xmlViewBody).val(formattedXML);
                    _pluginStatic.settings.htsData.RawXML = formattedXML;
                    $(_static.sel.addStepButton).hide();
                    $(_static.sel.addSolutionButton).hide();
                    $(_static.sel.showVariablesButton).hide();
                    $(_static.sel.hideVariablesButton).hide();
                    $(_pluginStatic.sel.contentWrapperTableRight).hide();
                },

                toggleXML: function () {
                    var htsData = _pluginStatic.fn.getHtsData();
                    $.ajax({
                        url: _pluginStatic.settings.XmlEditorUrl,
                        type: 'POST',
                        data: JSON.stringify(htsData),
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            _static.fn.showXMLView(response);
                        },
                        error: function (error) {
                            console.log(error);
                            alert(error.responseText);
                            //_static.fn.xmlConversionError();
                        }
                    });
                },

                updateHtsXML: function () {
                    var htsData = _pluginStatic.settings.htsData;
                    var variableList = [];
                    $.each(htsData.VariableLookup, function (idx, v) {
                        variableList.push(v);
                    });
                    htsData.Variables = variableList;

                    var data = {};
                    htsData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                    data.originalData = htsData;

                    $.ajax({
                        url: _pluginStatic.settings.UpdateXMLUrl,
                        type: 'POST',
                        data: JSON.stringify(data),
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            _pluginStatic.settings.htsData = response;
                            _pluginStatic.settings.htsData.RawXML = $(_pluginStatic.sel.xmlViewBody).val(); /* To Update the Raw XML*/
                            _static.fn.toggleUI(response);
                        },
                        error: function (error) {
                            _static.fn.xmlConversionError();
                        }
                    });
                },

                reload: function () {
                    var htsData = _pluginStatic.fn.getHtsData();

                    $.ajax({
                        url: _pluginStatic.settings.ReloadUrl,
                        type: 'POST',
                        data: JSON.stringify(htsData),
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            _pluginStatic.settings.htsData = response;
                            _static.fn.toggleUI(response);
                        },
                        error: function (error) {
                            alert('Failed to reload Question!');
                        }
                    });
                },

                saveHtsXML: function (fnCallback) {
                    var data = _pluginStatic.settings.htsData;
                    var variableList = [];
                    $.each(data.VariableLookup, function (idx, v) {
                        variableList.push(v);
                    });
                    data.Variables = variableList;
                    var htsData = {};
                    data.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                    htsData.htsData = data;

                    helper.saveDialogOpen();

                    $.ajax({
                        url: _pluginStatic.settings.SaveXmlUrl,
                        type: 'POST',
                        data: JSON.stringify(htsData),
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            helper.saveDialogClose();
                            _pluginStatic.settings.htsData = response;
                            _pluginStatic.settings.RPC.questionSaved(data.QuestionId);
                            if ($.isFunction(fnCallback)) {
                                fnCallback();
                            }
                            _pluginStatic.fn.buildUI(response, "fromsave");
                        },
                        error: function (error) {
                            console.log(error);
                            //alert('Save failed.');
                            helper.saveDialogClose();
                            helper.confirmDialog("", "The Advanced Question Editor could not interpret the XML. There may be an error in the XML or you may be using a feature that is not yet recognized by the editor. If you save, then you will be limited to Raw XML view for this question.?",
                                "Save Question", "Cancel", function () { _static.fn.saveHtsInvalidXML(fnCallback) }, null, null,
                                "Discard Changes", function () { _static.fn.revertRawXMLData(); });
                            //helper.confirmMessageDialog('Save failed.');
                        }
                    });
                },

                saveHtsInvalidXML: function (fnCallback) {
                    var htsData = _pluginStatic.settings.htsData;
                    var variableList = [];
                    $.each(htsData.VariableLookup, function (idx, v) {
                        variableList.push(v);
                    });
                    htsData.Variables = variableList;
                    htsData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();

                    helper.saveDialogOpen();

                    $.ajax({
                        url: _pluginStatic.settings.SaveInvalidXmlUrl,
                        type: 'POST',
                        data: JSON.stringify(htsData),
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            helper.saveDialogClose();
                            _pluginStatic.settings.htsData = response;
                            _pluginStatic.settings.RPC.questionSaved(htsData.QuestionId);
                            if ($.isFunction(fnCallback)) {
                                fnCallback();
                            }
                        },
                        error: function (error) {
                            console.log(error);
                            //alert('Save failed.');
                            helper.saveDialogClose();
                            helper.confirmMessageDialog('Save failed.');
                        }
                    });
                },

                toggleUI: function (htsData) {
                    $(_pluginStatic.sel.htmlView).show();
                    $(_pluginStatic.sel.xmlView).hide();
                    $(_static.sel.addStepButton).show();
                    $(_static.sel.addSolutionButton).show();
                    $(_static.sel.showVariablesButton).show();

                    if (htsData) {
                        _pluginStatic.fn.buildUI(htsData);
                    }
                },

                xmlConversionError: function () {
                    var div = $("<div></div>");
                    div.html('You have made changes to the question XML that cannot be interpreted by the editor.');
                    $(div).dialog({
                        resizable: false,
                        dialogClass: 'dialog-confirm',
                        height: 240,
                        width: 380,
                        modal: true,
                        title: 'Error',
                        open: function (event, ui) {
                            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
                        },
                        buttons: [{
                            text: 'Discard Changes',
                            click: function () {
                                _static.fn.toggleUI();
                                $(this).dialog("close");
                            }
                        }, {
                            text: 'Continue In Raw XML View',
                            click: function () {
                                $('.toggle-view-button.html-view').removeAttr('checked');
                                $('.toggle-view-button.xml-view').attr('checked', 'checked');
                                $(this).dialog("close");
                            }
                        }]
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

                        toggleViewButton: _static.sel.toggleViewButton,
                        toggleButton: _static.sel.toggleButton,
                        addStepButton: _static.sel.addStepButton,
                        addSolutionButton: _static.sel.addSolutionButton,
                        showVariablesButton: _static.sel.showVariablesButton,
                        hideVariablesButton: _static.sel.hideVariablesButton,
                        saveButton: _static.sel.saveButton,
                        revertButton: _static.sel.revertButton
                    },
                    fn: {
                        reload: function () {
                            _static.fn.reload();
                            //_static.fn.toggleUI();
                        },

                        showVariables: function () {
                            _static.fn.onShowVariablesClick();
                        },

                        showXMLView: function (response) {
                            _static.fn.showXMLView(response);
                        }
                    }
                };
            },
            //returns new API calls for the plugin
            api: function (pluginStatic) {
                _pluginStatic = pluginStatic;
                return {
                };
            },

            init: function () {
                // Bind question editor menu buttons                      
                $(_static.sel.toggleViewButton).die('click');
                $(_static.sel.toggleViewButton).live("click", _static.fn.onToggleViewClick);

                $(_static.sel.addStepButton).die('click');
                $(_static.sel.addSolutionButton).die('click');
                $(_static.sel.addStepButton).live("click", _static.fn.onAddStepClick);
                $(_static.sel.addSolutionButton).live("click", _static.fn.onAddSolutionClick);

                $(_static.sel.showVariablesButton).die('click');
                $(_static.sel.hideVariablesButton).die('click');
                $(_static.sel.saveButton).die('click');
                $(_static.sel.revertButton).die('click');
                $(_static.sel.showVariablesButton).live("click", _static.fn.onShowVariablesClick);
                $(_static.sel.hideVariablesButton).live("click", _static.fn.onHideVariablesClick);
                $(_static.sel.saveButton).live("click", _static.fn.onSaveClick);
                $(_static.sel.revertButton).live("click", _static.fn.onRevertClick);
            }
        };
    };
} (jQuery))