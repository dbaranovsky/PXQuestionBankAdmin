/// <reference path="QuestionEditorMenu.js" />
// QuestionEditor
//
// This plugin is responsible for the client-side behaviors of the
// Question Editor.
//
//
(function ($) {
    var _static = {
        pluginName: "QuestionEditor",
        dataKey: "QuestionEditor",
        bindSuffix: ".QuestionEditor",
        dataAttrPrefix: "data-qe-",
        defaults: {
            readOnly: false
        },
        settings: {
            // A global bookmark for the specific instance named "p_editor"
            editor_bookmark: false
        },
        css: {},
        sel: {
            questionEditorTemplate: '#question-editor-step-templates',
            questionEditor: '#question-editor',
            textEditorWrapper: '#question-editor #text-editor-wrapper',
            htmlView: '#question-editor #html-view',
            xmlView: '#question-editor #xml-view',
            xmlViewBody: '#question-editor #xml-view textarea',
            contentWrapper: '#question-editor .content-wrapper',
            contentWrapperTableLeft: '#question-editor .content-wrapper-table-left',
            contentWrapperTableRight: '#question-editor .content-wrapper-table-right',
            contentWrapperLeft: '#question-editor .content-left',
            contentWrapperRight: '#question-editor .content-right',
            stepsPane: '.pane.steps',
            variablesPane: '.pane.steps',
            previewPane: '.pane.preview',
            settingsPane: '.pane.settings',
            steps: '#question-editor .step',

            //local selectors
            step: '.step',
            solution: '.solution',
            solutionEditorId: 'solution-editor',
            node: '.node',
            subnode: '.sub-node',
            toolbar: '.mceExternalToolbar',
            responseRef: 'img[hts-data-type="numeric"], img[hts-data-type="multi"], img[hts-data-type="math"], img[hts-data-type="text"]',
            responseHoverRef: '.hts-response-hover',
            responseMenu: '.hts-response-menu',
            responseHoverAction: '.hts-hover-list li',

            toggleButton: '.toggle-button',
            deleteButton: '.delete-button',
            previewButton: '.preview-button',
            addQuestionButton: '.add-question-button',
            addHintButton: '.add-hint-button',
            addCorrectFeedBackButton: '.add-correct-feedback-button',
            addIncorrectFeedBackButton: '.add-incorrect-feedback-button',
            questionStepMenu: '.node-menu',

            stepButtonByName: function (name) {
                return 'input.button[name="' + name + '"]';
            },

            nodeByType: function (type) {
                return '.node[data-qe-nodetype="' + type + '"]';
            },

            tiny_mce_default_value: "click here and start typing",
            lnkCustomProperties: ".customquestion-properties-hts",
            questionMetaTitleContainer: '#qmeta_title_customeditorcomponent-hts',
            customMetaTitleText: '#txtMetaTitle_customComponent'
        },
        //private functions
        fn: {
            //adds new functionality from the module into the main plugin
            addModule: function (module) {
                $.extend(true, _static, _static, module.static(_static));
                $.extend(true, api, api, module.api(_static));
                module.init();
            },

            discardChanges: function () {
                $(_static.sel.htmlView).show();
                $(_static.sel.xmlView).hide();
                $(_static.sel.addStepButton).show();
                $(_static.sel.addSolutionButton).show();
                $(_static.sel.showVariablesButton).show();
                var data = _static.settings.htsData;
                _static.fn.buildUI(data);
            },

            onToggleNodeClick: function () {
                var $this = $(this);
                var node = $this.closest(_pluginStatic.sel.node);
                if (node.hasClass('close')) {
                    node.removeClass('close');
                    node.children('.node-body').slideDown('slow');
                } else {
                    node.addClass('close');
                    node.children('.node-body').slideUp('slow');
                }
            },

            checkDefaultText: function (ed) {
                if (ed.editorId.match("step-[1-99]-") == null)
                    return;

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

            editorStoreBookmark: function (Id) {
                if (Id) {
                    var ed = tinyMCE.get(Id);
                    if (ed) {
                        _static.settings.editor_bookmark = _static.settings.editor_bookmark = ed.selection.getBookmark(1);
                    }
                }
            },

            editorRestoreBookmark: function (id) {
                if (_static.settings.editor_bookmark) {
                    var ed = tinyMCE.get(id);
                    if (ed) {
                        ed.selection.moveToBookmark(_static.settings.editor_bookmark);
                    }
                }
            },

            onAddQuestionClick: function (e) {
                var args = {},
                step = $(this).closest(_static.sel.step),
                subNodeList = step.find('.sub-nodes');

                args.title = "Question & Response";
                args.nodeType = "question";
                args.parentStep = step;
                args.appendTo = subNodeList;

                var ctr = _static.fn.newSubNode(args);

                // questions should not be deleted.
                ctr.find(_static.sel.deleteButton).hide();

                $(this).hide();
            },
            onAddHintClick: function (e) {
                var args = {},
                step = $(this).closest(_static.sel.step),
                subNodeList = step.find('.sub-nodes');

                args.title = "Hint";
                args.nodeType = "hint";
                args.parentStep = step;
                args.appendTo = subNodeList;

                var ctr = _static.fn.newSubNode(args);

                $(this).hide();
                _static.fn.hideStepMenu($(this).closest(_static.sel.questionStepMenu));
            },

            onAddCorrectFeedbackClick: function (e) {
                var args = {},
                step = $(this).closest(_static.sel.step),
                subNodeList = step.find('.sub-nodes');

                args.title = "Correct Feedback";
                args.nodeType = "correct";
                args.parentStep = step;
                args.appendTo = subNodeList;

                var ctr = _static.fn.newSubNode(args);
                $(this).hide();
                _static.fn.hideStepMenu($(this).closest(_static.sel.questionStepMenu));
            },

            onAddIncorrectFeedbackClick: function (e) {
                var args = {},
                step = $(this).closest(_static.sel.step),
                subNodeList = step.find('.sub-nodes');

                args.title = "Incorrect Feedback";
                args.nodeType = "incorrect";
                args.parentStep = step;
                args.appendTo = subNodeList;

                var ctr = _static.fn.newSubNode(args);
                $(this).hide();
                _static.fn.hideStepMenu($(this).closest(_static.sel.questionStepMenu));
            },

            onDeleteClick: function () {
                var ctr = $(this).closest(_static.sel.node);
                if (ctr.length) {
                    _static.fn.deleteContainer(ctr);
                }
            },

            onDeleteHover: function (event) {
                var $this = $(this),
                    parent = $this.parent(),
                    title = parent.find('span');

                if (event.type == 'mouseover') {
                    // do something on mouseover
                    $this.addClass("hover");
                    var tooltip = $('<div class="delete-tooltip"></div>').text('Delete This ' + title.text());
                    tooltip.appendTo($this);
                    tooltip.position({
                        my: 'right bottom',
                        at: 'right top',
                        of: $this,
                        offset: "5"
                    });

                } else {
                    // do something on mouseout
                    parent.find('.delete-tooltip').remove();

                }
            },

            getStepData: function (step) {
                var stepData = {},
                question = _static.fn.getNodeData(step, 'question'),
                hint = _static.fn.getNodeData(step, 'hint'),
                correct = _static.fn.getNodeData(step, 'correct'),
                incorrect = _static.fn.getNodeData(step, 'incorrect');

                if (question != null) stepData.Question = question;
                if (hint != null) stepData.Hint = hint;
                if (correct != null) stepData.Correct = correct;
                if (incorrect != null) stepData.Incorrect = incorrect;

                return stepData;
            },

            setStepData: function (step, type, data) {
                _static.fn.setNodeData(step, type, data);
            },

            getNodeData: function (step, nodeType) {
                var nodeData = {};

                var node = step.find(_static.sel.nodeByType(nodeType));
                if (node.length) {
                    var editorCtl = node.find('.body').first(),
                    editorId = editorCtl.attr('id');
                    nodeText = "";

                    if (editorId && tinyMCE.getInstanceById(editorId)) {
                        nodeText = tinyMCE.get(editorId).getContent({
                            format: 'html'
                        });
                    }

                    nodeData = $.trim(nodeText);
                }
                return nodeData;
            },

            setNodeData: function (step, nodeType, data) {
                var node = step.find(_static.sel.nodeByType(nodeType));
                if (node.length) {
                    var editorCtl = node.find('.body').first();

                    //setting body of the control
                    editorCtl.html(data);
                }
            },
            
            getMetaTitleData: function() {
                var metaTitleData = $(_static.sel.customMetaTitleText).val();
                return metaTitleData;
            },

            getSolutionData: function () {
                var solutionValue = "",
                solution = $(_static.sel.questionEditor).find(_static.sel.solution),
                editorCtl = solution.find('.body').first(),
                editorId = editorCtl.attr('id'),
                nodeText = null;

                if (editorId && tinyMCE.getInstanceById(editorId)) {
                    nodeText = tinyMCE.get(editorId).getContent({
                        format: 'html'
                    });

                    nodeText = $.trim(nodeText);
                    if (nodeText == "") nodeText = "_";
                }

                solutionValue = nodeText;

                return solutionValue;
            },

            getHtsData: function () {
                try {
                    var openDialog = $(".hts-dialog").dialog("isOpen");
                    if (openDialog.length > 0) {
                        //openDialog.find('iframe').contents().find('.hts-dialog-save').trigger('click');
                        return false;
                    }
                } catch (err) { }

                var htsData = {};
                var steps = [];
                $(_static.sel.steps).each(function (stepIdx) {
                    var stepCtr = $(this),
                    stepData = _static.fn.getStepData(stepCtr);

                    stepData.sequence = stepIdx;
                    steps.push(stepData);
                });
                htsData.MaxPoints = _static.settings.htsData.MaxPoints;
                htsData.Steps = steps;
                htsData.Solution = _static.fn.getSolutionData();
                htsData.QuestionTitle = _static.fn.getMetaTitleData();
                htsData.Responses = _static.settings.htsData.Responses;
                htsData.Variables = _static.settings.htsData.VariableLookup;
                htsData.VariableLookup = _static.settings.htsData.VariableLookup;

                var variableList = [];
                $.each(htsData.VariableLookup, function (idx, v) {
                    variableList.push(v);
                });

                htsData.Variables = variableList;

                htsData.QuestionId = _static.settings.QuestionId;
                htsData.EntityId = _static.settings.EntityId;
                htsData.EnrollmentId = _static.settings.EnrollmentId;
                htsData.QuizId = _static.settings.QuizId;

                htsData.FormulaEditorUrl = _static.settings.EquationImageUrl;
                htsData.PlayerUrl = _static.settings.PlayerUrl;

                return htsData;
            },

            hideStepMenu: function (currentMenu) {
                var hint = currentMenu.find(_static.sel.stepButtonByName('hint')).is(':visible'),
                correct = currentMenu.find(_static.sel.stepButtonByName('correct')).is(':visible'),
                incorrect = currentMenu.find(_static.sel.stepButtonByName('incorrect')).is(':visible');

                if (hint == false && correct == false && incorrect == false) {
                    $(currentMenu).hide();
                }
                hint = correct = incorrect = null;
            },

            saveError: function (message) {
                this.message = message;
                this.name = "UserException";
            },

            saveHtsData: function (fnCallback, hideConfirmation) {
                var htsData = _static.fn.getHtsData();
                var saveQuestionByUrl = _static.settings.SaveUrl;

                if (htsData === false) {
                    alert("Please close any open dialog box before save.");
                    throw new _static.fn.saveError("Response box is not closed");
                    return false;
                }

                if ($(_pluginStatic.sel.xmlView).is(':visible')) {
                    var variableList = [];
                    $.each(htsData.VariableLookup, function (idx, v) {
                        variableList.push(v);
                    });
                    htsData.Variables = variableList;
                    htsData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                    saveQuestionByUrl = _static.settings.SaveXmlUrl;
                }

                helper.saveDialogOpen();

                $.ajax({
                    url: saveQuestionByUrl,
                    type: 'POST',
                    data: JSON.stringify(htsData),
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    cache: false,
                    success: function (response) {
                        _pluginStatic.settings.RPC.questionSaved(htsData.QuestionId);
                        //helper.saveDialogClose();
                        if (hideConfirmation == undefined || hideConfirmation == null || hideConfirmation == false) {
                            //helper.confirmMessageDialog(response.message);
                        }

                        if ($.isFunction(fnCallback)) {
                            fnCallback();
                        }

                        // Candidate for componentisation
                        var jqxhr = $.getJSON(_pluginStatic.settings.LoadUrl, null, function (dataIn) {
                            _pluginStatic.settings.htsData = dataIn;
                            $(_pluginStatic.sel.xmlViewBody).val(vkbeautify.xml(dataIn.RawXML));
                        });

                        jqxhr.complete(function () {
                            helper.saveDialogClose();
                            console.log('save completed and xml refreshed');
                        });

                    },
                    error: function (error) {
                        console.log(error);
                        helper.saveDialogClose();
                        helper.confirmMessageDialog('Save failed.');
                    }
                });
            },

            newStep: function () {
                if ($(_static.sel.questionEditorTemplate).length) {
                    var stepCount = $(_static.sel.steps).length + 1,
                    tmpl = $(_static.sel.questionEditorTemplate),
                    ctr = tmpl.find(_static.sel.step).clone(true, true),
                    newTitle = "Step " + stepCount,
                    stepId = 'step-' + stepCount;

                    ctr.find('.title span').text(newTitle);
                    ctr.attr('id', stepId);

                    return ctr;
                }
            },

            newSolution: function () {
                if ($(_static.sel.questionEditorTemplate).length) {
                    var tmpl = $(_static.sel.questionEditorTemplate),
                    ctr = tmpl.find(_static.sel.solution).clone(true, true);

                    return ctr;
                }
            },

            deleteContainer: function (ctr) {
                var position = ctr.position(),
                    type = ctr.qeattr('nodeType') || 'step',
                    title = ctr.find('.title span').first().text();

                confirmText = "Are you sure you want to delete this " + title.toLowerCase() + "?";

                if (title.indexOf("Step") != -1) {
                    confirmText = "Are you sure you want to delete this step?";
                }

                if (confirm(confirmText)) {
                    // if deleting sub-node, show button to add it back.
                    if (ctr.hasClass('sub-node')) {
                        var parentStep = ctr.closest(_static.sel.step),
                        menu = parentStep.find(_static.sel.questionStepMenu),
                        button = menu.find(_static.sel.stepButtonByName(type));

                        button.show();
                        if ($(menu).is(':visible') == false) {
                            $(menu).show();
                        }
                    }

                    if (ctr.hasClass('solution')) {
                        $(_pluginStatic.sel.addSolutionButton).removeAttr('disabled');
                    }

                    ctr.find('.body').each(function () {
                        var editorId = $(this).attr('id');
                        if (editorId && tinyMCE.getInstanceById(editorId)) {
                            tinyMCE.execCommand('mceFocus', false, editorId);
                            tinyMCE.execCommand('mceRemoveControl', false, editorId);
                        }
                    });

                    ctr.remove();

                    if (ctr.hasClass('step')) {
                        _static.fn.sortSteps();
                    }
                }
            },

            newSubNode: function (args) {
                if ($(_static.sel.questionEditorTemplate).length) {
                    var tmpl = $(_static.sel.questionEditorTemplate),
                    ctr = tmpl.find(_static.sel.subnode).clone(true, true),
                    editorCtl = ctr.find('.body').first(),
                    editorId = args.editorId;

                    if (args.title != undefined) ctr.find('.title span').text(args.title);

                    if (args.nodeType != undefined) ctr.qeattr("nodeType", args.nodeType);

                    if (args.editorId != undefined) {
                        editorCtl.attr("id", args.editorId);
                    } else {
                        //editorId = args.parentStep.attr('id') + '-' + args.nodeType + '-editor';
                        var uniq = '-id' + (new Date()).getTime();
                        editorId = args.parentStep.attr('id') + '-editor-' + args.nodeType + uniq;
                        editorCtl.attr("id", editorId);
                    }

                    if (args.value) editorCtl.text(args.value);
                    if (editorId.match("step-[0-99]-editor") && (args.value == undefined || args.value == null || $.trim(args.value).length == 0)) {
                        editorCtl.text(_static.sel.tiny_mce_default_value);
                    }

                    if (args.appendTo != undefined) {
                        ctr.hide().appendTo(args.appendTo);
                        ctr.ToEditor();
                        ctr.show();
                    }

                    if (editorId && tinyMCE.getInstanceById(editorId)) tinymce.execCommand('mceFocus', false, editorId)

                    return ctr;
                }
            },

            htmlEncode: function (value) {
                return $('<div/>').text(value).html();
            },

            htmlDecode: function (value) {
                return $('<div/>').html(value).text();
            },

            initMCE: function () {
                if (tinyMCE) {
                    tinyMCE.init({
                        theme: "advanced",
                        skin: "hts",
                        mode: "none",
                        plugins: "inlinepopups,table,paste,nonbreaking,paste,advimage,hts,media, noneditable",
                        inlinepopups_skin: "hts-dialog",
                        theme_advanced_toolbar_location: "external",
                        theme_advanced_toolbar_align: "left",
                        theme_advanced_buttons1: "bold,italic,strikethrough,bullist,numlist,justifyleft,justifycenter,justifyright,link,unlink,image,|,htsFormulaEditor,htsEditorMenu",
                        theme_advanced_buttons2: "",
                        //theme_advanced_buttons3: "",
                        extended_valid_elements: "img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|iprof|hts-data-id|hts-data-type|hts-data-equation|hts-data-variable-type|hts-data-variable-index|function|datafile]",
                        valid_children : "+body[style]",
                        add_unload_trigger: false,
                        remove_linebreaks: false,
                        force_p_newlines: false,
                        convert_urls: false,
                        theme_advanced_buttons3_add: "pastetext,pasteword,selectall",
                        paste_auto_cleanup_on_paste: true,
                        debug: false,
                        width: "100%",
                        setupcontent_callback: "QuestionEditorHelper.setupEditor",
                        remove_instance_callback: "QuestionEditorHelper.removeEditor",
                        equation_image_path: _static.settings.EquationImageUrl,
                        server_path: _static.settings.ServerUrl,
                        entity_encoding: 'named',
                        entities: '160,nbsp',
                        setup: function (ed) {
                            ed.onClick.add(function (ed, e) {
                                $(_static.sel.textEditorWrapper).find(_static.sel.toolbar).hide();
                                var toolbarId = '#' + ed.editorId + "_external",
                                existingToolbar = $(_static.sel.textEditorWrapper).find(toolbarId);
                                existingToolbar.show();
                            });

                            ed.onInit.add(function (ed) {
                                if (tinymce.isIE) {
                                    tinymce.dom.Event.add(ed.getBody(), "dragenter", function (e) {
                                        return tinymce.dom.Event.cancel(e);
                                    });
                                }
                                else {
                                    tinymce.dom.Event.add(ed.getBody().parentNode, "drop", function (e) {
                                        tinymce.dom.Event.cancel(e);
                                        tinymce.dom.Event.stop(e);
                                    });
                                }
                            });
                            ed.onEvent.add(function (ed) {
                                _static.fn.editorStoreBookmark(ed.editorId);
                            });
                            ed.onMouseUp.add(function (ed) {
                                _static.fn.checkDefaultText(ed);
                            });
                            ed.onKeyPress.add(function (ed) {
                                _static.fn.checkDefaultText(ed);
                            });
                            ed.onBeforeExecCommand.add(function (ed, cmd, ui, val) {
                                if (cmd == "htsInsertVariableInstance") {
                                    _static.fn.checkDefaultText(ed);
                                }
                            });
                        },
                        paste_preprocess: function (pl, o) {

                            if (o.content.indexOf('img') > 0) {
                                var message = "Response/Variable can not be copied!";
                                var showAlert = false;
                                if (pl.editor.editorId.indexOf('question') > 0 && o.content.indexOf('type=mathResponse') > 0) {
                                    message = "Response can not be copied!";
                                    showAlert = true;
                                }
                                else if (!(pl.editor.editorId.indexOf('question') > 0)) {
                                    showAlert = true;
                                }
                                if (showAlert) {
                                    alert(message);
                                    o.content = "";
                                }
                            }

                        },

                        paste_postprocess: function (pl, o) {
                            //                            // Content DOM node containing the DOM structure of the clipboard
                            //                            alert(o.node.innerHTML);
                            //                            o.node.innerHTML = o.node.innerHTML + "\n-: CLEANED :-";
                        }
                    });
                } else {
                    alert('Text Editor(MCE) not loaded');
                }
            },

            initRPC: function () {

                var editorUrl = $(_static.sel.questionEditor).attr("hts-data-editorUrl");
                if (typeof easyXDM !== 'undefined') {
                    var rpc = new easyXDM.Rpc({}, {
                        local: {
                            saveQuestion: function (successFn) {
                                //$(_static.sel.saveButton).trigger('click', [successFn]); // [fnCallback]);
                                var result = _static.fn.saveHtsData(null, true);
                                if (result === false) { return false; }
                                return "true";
                            },
                            previewQuestion: function (entityId, successFn) {
                                var previewResponse = "";
                                var htsData = _static.fn.getHtsData();
                                htsData.EntityId = entityId;
                                if ($(_pluginStatic.sel.xmlView).is(':visible')) {
                                    htsData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                                }

                                $.ajax({
                                    url: _static.settings.PreviewUrl,
                                    cache: false,
                                    async: false,
                                    success: function (xml) {
                                        previewResponse = xml;
                                    },
                                    data: JSON.stringify(htsData),
                                    contentType: "application/json; charset=utf-8",
                                    type: "POST"
                                });
                                return previewResponse;
                            },
                            isDirty: function (successFn) {
                                var htsData = {};
                                if ($(_pluginStatic.sel.xmlView).is(':visible'))
                                {
                                    /*htsData = _pluginStatic.settings.htsData;
                                    var variableList = [];
                                    $.each(htsData.VariableLookup, function (idx, v) {
                                        variableList.push(v);
                                    });
                                    htsData.Variables = variableList;
                                    htsData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();*/
                                    var originalData = _pluginStatic.settings.htsData;
                                    var variableList = [];
                                    $.each(originalData.VariableLookup, function (idx, v) {
                                        variableList.push(v);
                                    });
                                    originalData.Variables = variableList;

                                    originalData.ViewAsXml = "xml";
                                    originalData.PendingXML = $(_pluginStatic.sel.xmlViewBody).val();
                                    htsData.htsData = originalData;
                                } else {
                                    htsData = _pluginStatic.fn.getHtsData();
                                    htsData.ViewAsXml = null;
                                }

                                $.ajax({
                                    url: _pluginStatic.settings.CheckQuestionIsSaved,
                                    type: 'POST',
                                    data: JSON.stringify(htsData),
                                    contentType: "application/json; charset=utf-8",
                                    async: false,
                                    cache: false,
                                    success: function (response) {
                                        if (response == "true") {
                                            result = false;
                                        } else {
                                            result = true;
                                        }
                                    },
                                    error: function (error) {
                                        console.log(error);
                                        result = true;
                                    }
                                });
                                return result;
                            }
                        },
                        remote: {
                            questionSaved: {}
                        }
                    });

                    _static.settings.RPC = rpc;
                    //PxPage.FrameAPI = rpc;
                }

            },

            resetUI: function () {
                var stepList = $(_static.sel.stepsPane),
                    solutionStep = $(_static.sel.questionEditor).find(_static.sel.solution);

                $(_static.sel.questionEditor).find('textarea.body').each(function (stepIdx) {
                    var stepCtr = $(this);
                    var editorId = stepCtr.attr('id');
                    if (editorId != undefined) {
                        if (tinyMCE.getInstanceById(editorId)) {
                            tinyMCE.execCommand('mceFocus', false, editorId);
                            tinyMCE.execCommand('mceRemoveControl', false, editorId);
                        }
                    }
                });
                stepList.empty();
                solutionStep.remove();

                $(_pluginStatic.sel.addSolutionButton).removeAttr('disabled');
            },

            // Retrieve/Parse xml document and update HTS ui for existing question data.
            buildUI: function (data, fromRawXMLSave) {

                if (fromRawXMLSave != "fromsave") {
                    $(_static.sel.questionEditor).show();
                }
                if (data) {
                    //reset UI
                    _static.fn.resetUI();

                    var xmlDoc = data,
                    stepList = $(_static.sel.stepsPane),
                    solutionStep = $(_static.sel.questionEditor).find(_static.sel.solution),
                    args = {},
                    stepCtr = null,
                    stepButton = null,
                    stepMenu = null,
                    stepSubNodes = null,
                    ctr = null;
                    if (data.ContentWithInvalidXml == "true") {
                        helper.confirmMessageDialog('This question contains invalid XML. Editing tools will not be available.');
                        _pluginStatic.fn.showXMLView(data.RawXML);
                    } else {
                        $.each(data.Steps, function (Idx, currStep) {
                            stepCtr = _static.fn.newStep();
                            stepCtr.appendTo(stepList);
                            stepSubNodes = stepCtr.find('.sub-nodes');
                            stepMenu = stepCtr.find(_static.sel.questionStepMenu);

                            args = {};
                            args.parentStep = stepCtr;
                            args.appendTo = stepSubNodes;

                            if (currStep.Question == null) {
                                currStep.Question = " ";
                            }

                            if (currStep.Question) {
                                args.title = "Question & Response";
                                args.nodeType = "question";
                                args.value = currStep.Question;
                                ctr = _static.fn.newSubNode(args);

                                // questions should not be deleted.
                                ctr.find(_static.sel.deleteButton).hide();

                                stepButton = stepMenu.find(_static.sel.stepButtonByName(args.nodeType));
                                stepButton.hide();
                            }

                            if (currStep.Hint) {
                                args.title = "Hint";
                                args.nodeType = "hint";
                                args.value = currStep.Hint;
                                ctr = _static.fn.newSubNode(args);

                                stepButton = stepMenu.find(_static.sel.stepButtonByName(args.nodeType));
                                stepButton.hide();
                            }

                            if (currStep.Correct) {
                                args.title = "Correct Feedback";
                                args.nodeType = "correct";
                                args.value = currStep.Correct;
                                ctr = _static.fn.newSubNode(args);

                                stepButton = stepMenu.find(_static.sel.stepButtonByName(args.nodeType));
                                stepButton.hide();
                            }

                            if (currStep.Incorrect) {
                                args.title = "Incorrect Feedback";
                                args.nodeType = "incorrect";
                                args.value = currStep.Incorrect;
                                ctr = _static.fn.newSubNode(args);

                                stepButton = stepMenu.find(_static.sel.stepButtonByName(args.nodeType));
                                stepButton.hide();
                            }

                            _static.fn.hideStepMenu(stepMenu);
                        });

                        if (data.Solution) {
                            if (solutionStep.length == 0) {
                                var solution = _static.fn.newSolution();
                                solution.insertAfter(stepList);
                                solution.find('.body').first().text(data.Solution);
                                solution.ToEditor();

                                $(_pluginStatic.sel.addSolutionButton).attr('disabled', 'disabled');
                            }
                        }
                        if (!$.isEmptyObject(data.VariableLookup) && fromRawXMLSave != "fromsave") {
                            _pluginStatic.fn.showVariables();
                        }
                        if (data.QuestionTitle) {
                            $(_static.sel.questionMetaTitleContainer).val(data.QuestionTitle);
                        }
                    }
                }
            },

            onResponseHoverAction: function (event) {
                var $this = $(this);
                var $parent = $this.closest('.hts-wrapper');
                var $response = $parent.find('img');

                if ($this.hasClass('edit')) {
                    var nodeType = $response.attr('hts-data-type');
                    //tinyMCE.activeEditor.selection.select(elm);
                    tinyMCE.activeEditor.selection.select($response.get(0));
                    switch (nodeType) {
                        case "math":
                            tinyMCE.activeEditor.execCommand('htsFormulaResponseEditor', $response);
                            break;
                        case "text":
                            tinyMCE.activeEditor.execCommand('htsTextResponseEditor', $response);
                            break;
                        case "multi":
                            tinyMCE.activeEditor.execCommand('htsMultiResponseEditor', $response);
                            break;
                        case "numeric":
                            tinyMCE.activeEditor.execCommand('htsNumericResponseEditor', $response);
                            break;
                        //                        case "variable":                                       
                        //                            tinyMCE.activeEditor.execCommand('htsInsertVariableInstance', null);                                       
                        //                            break;                                       
                        //                        case "formula":                                       
                        //                            tinyMCE.activeEditor.execCommand('htsFormulaEditor', $response);                                       
                        //                            break;                                       
                    }
                }

                if ($this.hasClass('delete')) {
                    helper.confirmDialog(
                    'Apply Changes',
                    'Are you sure you want to delete this answer?',
                    'Delete',
                    "Don't Delete",
                    function () {
                        //delete it
                        $parent.remove();
                        var position = $this.position();
                    },
                    function () {
                        $parent.find('.hts-hover-list').remove();
                    });

                    /*$parent.remove();
                    var position = $this.position();
                    var confirmText = "Are you sure you want to delete this ?";*/

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
                }
            },

            onResponseRefHover: function (event) {
                var $this = $(this);

                var $response = $(this);
                var responseId = $response.attr('hts-data-id');
                var responseHoverId = "hts-hover-" + responseId;

                var $wrapperDiv = $("<div></div>");
                var wrapperDivId = "hts-wrapper-" + responseId;
                $wrapperDiv.attr('id', wrapperDivId);
                $wrapperDiv.css('display', 'inline-block');
                $wrapperDiv.addClass('hts-wrapper');

                var $menuDiv = $("<ul><li class='edit'>Edit Response</li><li class='delete'>Delete Response</li></ul>");
                $menuDiv.attr('id', responseHoverId);
                $menuDiv.css('position', 'absolute');
                $menuDiv.addClass('hts-hover-list');

                if (event.type == 'mouseover') {
                    wrapperDivId = "#" + wrapperDivId;

                    if ($response.closest(wrapperDivId).length)
                    { }
                    else {

                        $response.wrap($wrapperDiv);
                        $wrapperDiv = $response.parent();

                        $menuDiv.insertAfter($response);
                        $menuDiv.position({
                            my: "right top",
                            at: "right bottom",
                            of: $response,
                            collision: "fit"
                        });
                        //                    $(this).qtip({
                        //                        overwrite: false, // Make sure another tooltip can't overwrite this one without it being explicitly destroyed
                        //                        content: 'I\'m a live qTip',
                        //                        show: {
                        //                            event: false,
                        //                            ready: true // Needed to make it show on first mouseover event
                        //                        },
                        //                        hide: false, //Don't specify a hide event.
                        //                        style: {
                        //                            classes: 'qtip-response'
                        //               w         }
                        //                    });

                        $wrapperDiv.bind("mouseenter mouseleave", function (event) {

                            if (event.type == 'mouseleave') {
                                // do something on mouseout
                                responseHoverId = "#" + responseHoverId;
                                //                    $(responseHoverId).remove();
                                $wrapperDiv.find(responseHoverId).remove();
                                $response.unwrap();
                            }

                        });
                    }


                }

                //                 else {
                //                    // do something on mouseout
                //                    responseHoverId = "#" + responseHoverId;
                //                    //                    $(responseHoverId).remove();
                //                    $wrapperDiv.find(responseHoverId).remove();
                //                    $response.unwrap();

                //                }
            },

            confirmDialog: function (titleText, alertText, okText, cancelText, okCallback, cancelCallback) {
                var div = $("<div></div>");
                div.html(alertText);
                $(div).dialog({
                    resizable: false,
                    dialogClass: 'dialog-confirm',
                    height: 200,
                    modal: true,
                    title: titleText,
                    open: function (event, ui) {
                        $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
                    },
                    buttons: [{
                        text: okText,
                        click: function () {
                            $(this).dialog("close");
                            okCallback();
                        }
                    }, {
                        text: cancelText,
                        click: function () {
                            $(this).dialog("close");
                            cancelCallback();
                        }
                    }]
                });
            },

            sortSteps: function () {
                $(_static.sel.steps).each(function (stepIdx) {
                    var title = "Step " + (stepIdx + 1);
                    $(this).find('.title span').first().text(title);
                    $(this).attr("id", "step-" + (stepIdx + 1));
                });
            },
            
            closeDialogForCustomProperties: function (event, ui) {
                var newTitle = $(_static.sel.questionMetaTitleContainer).val();
                $(_static.sel.customMetaTitleText).val(newTitle);
            },
            
            openDialogForCustomProperties: function () {
                $('#customquestion-meta-title-dialog-hts').dialog({
                    autoOpen: true,
                    show: {
                        effect: "fold",
                        duration: 500
                    },
                    hide: {
                        effect: "fold",
                        duration: 500
                    },
                    modal: true,
                    width: '50%' ,
                    title: "Question Properties",
                    dialogClass: "custom-properties-meta-title",
                    buttons:    [
                            {
                                text: "Close", click: function (event, ui) {
                                    _static.fn.closeDialogForCustomProperties(event,ui);
                                    $('#customquestion-meta-title-dialog-hts').dialog("close");
                                }
                            }
                    ],
                    open: function (event, ui) {
                        $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
                    },
                    close: function (event, ui) {
                        _static.fn.closeDialogForCustomProperties(event, ui);
                    },
                    appendTo: "#question-editor",
                    position: { my: "left top", at: "left bottom", of: ".customquestion-properties-hts" }
                    
                });
            },


            bindControls: function () {
                // Bind global toggle button
                $(_static.sel.toggleButton).die('click');
                $(_static.sel.toggleButton).live("click", _static.fn.onToggleNodeClick);

                // Bind Step/Solution menu buttons
                $(_static.sel.addQuestionButton).die('click');
                $(_static.sel.addHintButton).die('click');
                $(_static.sel.addCorrectFeedBackButton).die('click');
                $(_static.sel.addIncorrectFeedBackButton).die('click');
                $(_static.sel.deleteButton).die('click');

                $(_static.sel.addQuestionButton).live("click", _static.fn.onAddQuestionClick);
                $(_static.sel.addHintButton).live("click", _static.fn.onAddHintClick);
                $(_static.sel.addCorrectFeedBackButton).live("click", _static.fn.onAddCorrectFeedbackClick);
                $(_static.sel.addIncorrectFeedBackButton).live("click", _static.fn.onAddIncorrectFeedbackClick);

                $(_static.sel.deleteButton).die('mouseover mouseout');
                $(_static.sel.deleteButton).live('mouseover mouseout', _static.fn.onDeleteHover);
                $(_static.sel.deleteButton).live("click", _static.fn.onDeleteClick);
                $(_static.sel.lnkCustomProperties).die('click');
                $(_static.sel.lnkCustomProperties).live('click',_static.fn.openDialogForCustomProperties);
                
                $(_static.sel.stepsPane).sortable({
                    revert: true,
                    update: function (event, ui) {
                        _static.fn.sortSteps();
                    },
                    start: function (event, ui) {
                        var step = ui.item;
                        $(step).find('.node-body').hide();
                        $(step).find('.body').each(function () {
                            var editorId = $(this).attr('id');
                            if (editorId != undefined) {
                                if (tinyMCE.getInstanceById(editorId)) {
                                    tinyMCE.execCommand('mceFocus', false, editorId);
                                    tinyMCE.execCommand('mceRemoveControl', false, editorId);
                                }
                            }
                        });
                        $(step).addClass('dragging');
                    },
                    stop: function (event, ui) {
                        var step = ui.item;
                        $(step).find('.node-body').show();
                        $(step).find('.body').each(function () {
                            var editorId = $(this).attr('id');
                            if (editorId != undefined) {
                                var textbox = $('<textarea class="body"></textarea>');
                                textbox.html($(this).val());
                                textbox.attr('id', editorId);

                                $(this).parent().append(textbox);
                                $(this).remove();

                                tinyMCE.execCommand('mceAddControl', false, editorId);
                                $(this).sortable("refresh");
                            }
                        });

                        $(step).find('.node-body .body').first().click();
                        if ($(step).hasClass('close')) {
                            $(step).removeClass('close'); /*find(_static.sel.toggleButton).click();*/
                        }

                        $(step).find('.sub-node').each(function () {
                            if ($(this).hasClass('close')) {
                                $(this).removeClass('close'); /*.find(_static.sel.toggleButton).click();*/
                            }
                        });

                        $(step).removeClass('dragging');
                    }
                });

                $.ui.dialog.prototype.options.autoReposition = true;
                $(window).resize(function () {
                    $(".ui-dialog-content:visible").each(function () {
                        var dialog = $(this).data("dialog");
                        if (dialog.options.autoReposition) {
                            dialog.option("position", dialog.options.position);
                        }
                    });
                });
            }
        }
    },

    //public API
api = {
    // The init method sets up the data for the plugin using the given
    // option values to override the defaults.                
    init: function (options) {
        var settings = $.extend(true, _static.settings, _static.defaults, options),
            $this = $(this),
            data = $this.data(_static.dataKey);

        if (!data) {
            $this.data(_static.dataKey, {
                target: $this,
                settings: settings
            });
            data = $this.data(_static.dataKey);
        }

        _static.settings = settings;

        _static.fn.initMCE();
        _static.fn.initRPC();
        _static.fn.bindControls();

        //add modules
        _static.fn.addModule(QuestionEditorMenu());
        _static.fn.addModule(QuestionEditorVariablesPanel());
        _static.fn.addModule(QuestionEditorNumericVariableDialog());
        _static.fn.addModule(QuestionEditorNumericArrayDialog());
        _static.fn.addModule(QuestionEditorTextVariableDialog());
        _static.fn.addModule(QuestionEditorTextArrayDialog());
        _static.fn.addModule(QuestionEditorMathVariableDialog());
        _static.fn.addModule(QuestionEditorMathArrayDialog());
        

        _static.fn.buildUI(_static.settings.htsData);
    },
    build: function () {
        $.getJSON(_static.settings.LoadUrl, null, function (dataIn) {

            _static.settings.htsData = dataIn;
            _static.fn.buildUI(dataIn);
        });
    }
};

    //Handle the custom attributes
    $.fn.qeattr = function (name, value) {
        var target = this.first(),
        aName = _static.dataAttrPrefix + name;
        if (value)
            return target.attr(aName, value);

        return target.attr(aName);
    };

    //Convert any element to TinyMCE editor
    $.fn.ToEditor = function () {
        var target = $(this).find('.body').first(),
        targetId = $(target).attr('id');

        tinyMCE.execCommand('mceAddControl', false, targetId);

        return $(target);
    };

    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.QuestionEditor = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

    // The public interface for interacting with this plugin.
    window.QuestionEditorHelper = {

        initMCE: function () {
            _static.fn.initMCE();
        },

        setupEditor: function (editor_id, body, doc) {
            var toolbarId = '#' + editor_id + "_external",
            toolbarCount = $(_static.sel.textEditorWrapper).find(_static.sel.toolbar).length,
            existingToolbar = $(_static.sel.textEditorWrapper).find(toolbarId),
            newToolbar = $(_static.sel.contentWrapper).find(toolbarId),
            visibleToolbars = $(_static.sel.textEditorWrapper).find(_static.sel.toolbar + ':visible');

            // remove existing toolbar
            if (existingToolbar.length) {
                newToolbar.remove();
                newToolbar = existingToolbar;
            } else {
                // add new toolbar                
                newToolbar.appendTo(_static.sel.textEditorWrapper);
            }

            // adjust toolbar visibility
            if (visibleToolbars.length) {
                newToolbar.hide();
            }

            $(body).off('mouseover mouseout', _static.sel.variableRef);
            $(body).on('mouseover mouseout', _static.sel.variableRef, _static.fn.onVariableRefHover);

            $(body).off('mouseover mouseout', _static.sel.responseRef);
            $(body).on('mouseover mouseout', _static.sel.responseRef, _static.fn.onResponseRefHover);

            $(body).off('click', _static.sel.responseHoverAction);
            $(body).on('click', _static.sel.responseHoverAction, _static.fn.onResponseHoverAction);

            //$(body).off('mouseover mouseout', _static.sel.responseHoverRef);
            //$(body).on('mouseover mouseout', _static.sel.responseHoverRef, _static.fn.onResponseRefHover);
        },

        removeEditor: function (inst) {
            var toolbarId = '#' + inst.editorId + "_external",
            existingToolbar = $(_static.sel.textEditorWrapper).find(toolbarId);

            if (existingToolbar.length) {
                existingToolbar.remove();
            }
        },

        addResponseData: function (responseData) {
            var data = _static.settings.htsData;
            var responseId = responseData.ElementId;
            var found = false;
            var foundIndex = 0;

            $.each(data.Responses, function (i, v) {
                var response = $(this);
                if (v.ElementId == responseId) {
                    found = true;
                    foundIndex = i;
                    return false;
                }
                return;
            });

            if (found) {
                // if response exist, remove old one and add new one to array
                data.Responses.splice(foundIndex, 1);
                data.Responses.push(responseData);
            } else {
                // response is new, just add new one to array
                data.Responses.push(responseData);
            }
            return responseId;
        },

        getResponseData: function (responseId) {
            var data = _static.settings.htsData;
            var response = null;
            $.each(data.Responses, function (i, v) {
                if (v.ElementId == responseId) {
                    response = v;
                    return false;
                }
                return;
            });

            return response;
        },

        addVariableData: function (variableData, inMode) {
            var data = _static.settings.htsData;
            var foundIndex = 0,
                variables = data.VariableLookup;

            variables[variableData.Name] = variableData;

            if (variableData.OldName != variableData.Name && variableData.OldName != "") {
                _pluginStatic.fn.reload();
                variableData.OldName = "";
            }

            return variableData;
        },

        getVariableData: function (variableName) {
            var data = _static.settings.htsData;
            var variables = data.VariableLookup;

            return variables[variableName];
        },

        getTypeVariableData: function (type) {
            var data = _static.settings.htsData;
            var variables = data.VariableLookup;
            var numericVariable = {};
            $.each(variables, function (variableName) {
                if (variables[variableName].Type == type) {
                    numericVariable[variableName] = variables[variableName];
                }
            });

            return numericVariable;
        },

        removeVariableData: function (variableName) {
            var data = _static.settings.htsData;
            var variables = data.VariableLookup;

            if (variables[variableName]) {
                delete variables[variableName];
            }

            _pluginStatic.fn.reload();

            return variables[variableName];
        },
        isValidSubscript: function (variableName, idx) {
            var data = _static.settings.htsData;
            var variables = data.VariableLookup;
            var varObj = variables[variableName];
            var varLength = 0;

            if (varObj) {
                if (varObj.Constraints) {
                    var constraint = varObj.Constraints[0],
                       items = constraint.Inclusions.split(",");
                    varLength = items.length - 1;
                }
            }

            //return idx <= varLength;
            return true;
        },
        // returns a new generated response id based on the max value of the responses array.
        getNewResponseId: function () {
            var data = _static.settings.htsData;
            var maxId = 0;

            var responseIdArray = $.map(data.Responses, function (r) {
                return r.ElementId;
            });

            if (responseIdArray != undefined && responseIdArray.length > 0) {
                maxId = Math.max.apply(Math, responseIdArray);
            }

            return maxId + 1;
        },
        restoreBookMark: function (id) {
            _static.fn.editorRestoreBookmark(id);
        }

    };
} (jQuery))