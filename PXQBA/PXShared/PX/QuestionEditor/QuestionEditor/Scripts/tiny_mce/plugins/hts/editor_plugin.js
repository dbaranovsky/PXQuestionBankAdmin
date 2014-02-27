/**
* editor_plugin_src.js
*
* Copyright 2009, Moxiecode Systems AB
* Released under LGPL License.
*
* License: http://tinymce.moxiecode.com/license
* Contributing: http://tinymce.moxiecode.com/contributing
*/

(function () {
    // Load plugin specific language pack
    //tinymce.PluginManager.requireLangPack('hts');
    var _static = {
        urlText: "",
        server_path: ""
    };

    jQuery.fn.OuterHTML = function () {
        return $('<div></div>').append(this.clone()).html();
    };


    tinymce.create('tinymce.plugins.hts', {
        /**
        * Initializes the plugin, this will be executed after the plugin has been created.
        * This call is done before the editor instance has finished it's initialization so use the onInit event
        * of the editor instance to intercept that event.
        *
        * @param {tinymce.Editor} ed Editor instance that the plugin is initialized in.
        * @param {string} url Absolute URL to where the plugin is located.
        */

        loadResponseData: function (ed, responseType, responseImage) {
            var selectedNode = ed.selection.getNode();
            var responseData = {};
            var response = null;
            var isEditMode = false;
            if (responseImage) selectedNode = responseImage;
            if (selectedNode) {
                var selectedType = $(selectedNode).attr('hts-data-type');
                var selectedResponseId = $(selectedNode).attr('hts-data-id');

                if (typeof selectedType !== 'undefined' && selectedType == responseType) {
                    if (typeof selectedResponseId !== 'undefined' && selectedResponseId !== false) {
                        isEditMode = true;
                    }
                }
            }

            if (isEditMode) {
                response = QuestionEditorHelper.getResponseData(selectedResponseId);
                responseData.mode = "edit";
                responseData.response = response;
                responseData.responseId = response.ElementId;
            }
            else {
                responseData.mode = "new";
                responseData.response = null;
                responseData.responseId = QuestionEditorHelper.getNewResponseId();
            }

            responseData.numericVariable = QuestionEditorHelper.getTypeVariableData("numeric");

            return responseData;
        },

        loadVariableNode: function (ed) {
            var selectedNode = ed.selection.getNode();
            var variableNode = null;
            var isEditMode = false;

            if (selectedNode) {
                var selectedType = $(selectedNode).attr('hts-data-type');
                var selectedName = $(selectedNode).attr('hts-data-id');

                var isVariableTypeGiven = (typeof selectedType !== 'undefined' && selectedType == 'variable');
                var isVariableNameGiven = (typeof selectedName !== 'undefined' && selectedName !== false);
                if (isVariableTypeGiven && isVariableNameGiven) {
                    variableNode = selectedNode;
                }
            }

            return $(variableNode);
        },

        createVariableNode: function (variable) {
            var shortText = "";
            switch (variable.Type) {
                case 'text':
                    shortText = 'Tv';
                    break;
                case 'numeric':
                    shortText = 'Nv';
                    break;
                case 'math':
                    shortText = 'Mv';
                    break;
                case 'textarray':
                    shortText = 'TAv';
                    break;
                case 'numarray':
                    shortText = 'NAv';
                    break;
                case 'matharray':
                    shortText = 'MAv';
                    break;
            }

            if (variable.Type == 'math') {
                return _static.createMathNode(variable, shortText);
            }
            else {
                return _static.createNonMathNode(variable, shortText);
            }
        },

        createMathNode: function (variable, shortText) {
            var formulaImgBaseUrl = _static.equation_image_path;

            //equation renderer doesn't like underscores, replace with dash.
            var variableName = variable.Name;
            variableName = variableName.replace(/_/g, "-");

            var formulaText = encodeURIComponent("$" + variableName);
            var formulaImgSrc = formulaImgBaseUrl + "?eqtext=@T{" + formulaText + "}";

            var variableNode = $('<img />');
            variableNode.attr('src', formulaImgSrc);
            variableNode.attr('alt', formulaImgSrc);
            variableNode.attr('data-mce-src', formulaImgSrc);

            variableNode.attr('hts-data-type', 'formula');
            variableNode.attr('hts-data-variable-type', variable.Type);
            variableNode.attr('hts-data-equation', formulaText);
            variableNode.attr('hts-data-id', variable.Name);

            return variableNode;
        },

        createNonMathNode: function (variable, shortText) {
            var formulaImgBaseUrl = _static.equation_image_path;
            var imgBaseUrl = formulaImgBaseUrl.replace("geteq", "geticon");
            //equation renderer doesn't like underscores, replace with dash.
            var variableName = variable.Name;
            //variableName = variableName.replace(/_/g, "-");

            var imgSrc = imgBaseUrl + "?caption=" + variableName + "&type=" + variable.Type + "_var";

            var variableNode = $('<img />');
            variableNode.attr('src', imgSrc);
            variableNode.attr('hts-data-type', 'variable');
            variableNode.attr('hts-data-variable-type', variable.Type);
            variableNode.attr('hts-data-id', variable.Name);

            return variableNode;
        },

        init: function (ed, url) {
            _static.server_path = ed.settings.server_path;
            _static.equation_image_path = ed.settings.equation_image_path;
            _static.urlText = url;
            _static.getResponseData = this.loadResponseData;
            _static.getVariableNode = this.loadVariableNode;
            _static.createVariableNode = this.createVariableNode;
            _static.createMathNode = this.createMathNode;
            _static.createNonMathNode = this.createNonMathNode;

            // Register the command so that it can be invoked by using tinyMCE.activeEditor.execCommand('htsFormulaEditor');
            ed.addCommand('htsFormulaEditor', function (eqNode) {

                var imgNode = eqNode;
                if (imgNode == false) {
                    var selectedNode = ed.selection.getNode();
                    var selectedNodeType = selectedNode.getAttribute('hts-data-type');
                    if ((selectedNode.nodeName == 'IMG') && (selectedNodeType == 'math')) {
                        imgNode = selectedNode;
                    }
                }

                ed.windowManager.open({
                    file: url + '/formulaEditor.html',
                    width: 540 + parseInt(ed.getLang('example.delta_width', 0)),
                    height: 300 + parseInt(ed.getLang('example.delta_height', 0)),
                    resizable: false,
                    maximizable: false,
                    inline: 1
                }, {
                    plugin_url: url, // Plugin absolute URL                    
                    equation_image_path: ed.settings.equation_image_path,
                    equation_image: imgNode,
                    current_bookmark: ed.selection.getBookmark(1)
                });
            });

            ed.addCommand('htsFormulaResponseEditor', function (imgNode) {
                var response = _static.getResponseData(ed, 'math', imgNode);

                ed.windowManager.open({
                    file: url + '/formulaResponseEditor.html',
                    width: 540 + parseInt(ed.getLang('example.delta_width', 0)),
                    height: 332 + parseInt(ed.getLang('example.delta_height', 0)),
                    resizable: false,
                    maximizable: false,
                    inline: 1
                }, {
                    plugin_url: url, // Plugin absolute URL                    
                    equation_image_path: ed.settings.equation_image_path,
                    server_path: ed.settings.server_path,
                    response_data: response,
                    response_image: imgNode
                });
            });

            ed.addCommand('htsTextResponseEditor', function (imgNode) {
                var response = _static.getResponseData(ed, 'text', imgNode);
                ed.windowManager.open({
                    file: url + '/textResponseEditor.html',
                    width: 540 + parseInt(ed.getLang('example.delta_width', 0)),
                    height: 334 + parseInt(ed.getLang('example.delta_height', 0)),
                    resizable: false,
                    maximizable: false,
                    inline: 1
                }, {
                    plugin_url: url, // Plugin absolute URL                    
                    equation_image_path: ed.settings.equation_image_path,
                    server_path: ed.settings.server_path,
                    response_data: response,
                    response_image: imgNode
                });
            });

            ed.addCommand('htsMultiResponseEditor', function (imgNode) {
                var response = _static.getResponseData(ed, 'multi', imgNode);
                ed.windowManager.open({
                    title: 'Multiple Choice Response Editor',
                    file: url + '/multiResponseEditor.html',
                    width: 540 + parseInt(ed.getLang('example.delta_width', 0)),
                    height: 375 + parseInt(ed.getLang('example.delta_height', 0)),
                    resizable: false,
                    maximizable: false,
                    inline: 1
                }, {
                    plugin_url: url, // Plugin absolute URL                    
                    equation_image_path: ed.settings.equation_image_path,
                    server_path: ed.settings.server_path,
                    response_data: response,
                    response_image: imgNode
                });
            });

            ed.addCommand('htsNumericResponseEditor', function (imgNode) {
                var response = _static.getResponseData(ed, 'numeric', imgNode);
                ed.windowManager.open({
                    file: url + '/numericResponseEditor.html',
                    width: 520 + parseInt(ed.getLang('example.delta_width', 0)),
                    height: 350 + parseInt(ed.getLang('example.delta_height', 0)),
                    resizable: false,
                    maximizable: false,
                    inline: 1
                }, {
                    plugin_url: url, // Plugin absolute URL                    
                    equation_image_path: ed.settings.equation_image_path,
                    server_path: ed.settings.server_path,
                    response_data: response,
                    response_image: imgNode
                });
            });

            ed.addCommand('htsAddResponseData', function (responseData) {
                QuestionEditorHelper.addResponseData(responseData);
            });

            ed.addCommand('htsResetMCE', function (responseData) {
                QuestionEditorHelper.initMCE();
            });


            // Register commands
            ed.addCommand('mceAdvImage', function () {
                // Internal image object like a flash placeholder
                if (ed.dom.getAttrib(ed.selection.getNode(), 'class', '').indexOf('mceItem') != -1)
                    return;

                ed.windowManager.open({
                    file: url + '/../advimage/image.htm',
                    width: 480 + parseInt(ed.getLang('advimage.delta_width', 0)),
                    height: 340 + parseInt(ed.getLang('advimage.delta_height', 0)),
                    inline: 1
                }, {
                    plugin_url: url
                });
            });

            // Register editor button
            ed.addButton('htsFormulaEditor', {
                title: 'Insert Formula',
                cmd: 'htsFormulaEditor',
                image: url + '/img/formula.png'
            });

            ed.addCommand('htsInsertVariableInstance', function (variable) {
                var variableImageNode = null;
                var isEditMode = false;
                var isInsertMode = false;

                _static.server_path = ed.settings.server_path;
                _static.equation_image_path = ed.settings.equation_image_path;

                // load up an in memory image object to represent the variable
                if (variable) {
                    variableImageNode = _static.createVariableNode(variable);
                    isInsertMode = true;
                }
                else {
                    variableImageNode = _static.getVariableNode(ed);
                    isEditMode = true;
                }

                if (variableImageNode) {
                    var variableType = variableImageNode.attr('hts-data-variable-type'),
                        isVariableArray = (variableType.indexOf('array') > -1);

                    // if variable is not an array and we are inserting, just add the variable image to editor
                    if (!isVariableArray && isInsertMode) {
                        QuestionEditorHelper.restoreBookMark(ed.editorId);
                        ed.execCommand('mceInsertContent', false, variableImageNode.OuterHTML());
                    }
                    else {
                        ed.windowManager.open({
                            file: url + '/variableEditor.html',
                            width: 300 + parseInt(ed.getLang('example.delta_width', 0)),
                            height: 160 + parseInt(ed.getLang('example.delta_height', 0)),
                            resizable: false,
                            maximizable: false,
                            inline: 1
                        }, {
                            plugin_url: url, // Plugin absolute URL
                            server_path: ed.settings.server_path, // Custom argument                        
                            variable_image_node: variableImageNode,
                            variable_dialog_edit: isEditMode,
                            equation_image_path: ed.settings.equation_image_path
                        });
                    }
                }

            });


            // Add a node change handler, selects the button in the UI when a image is selected
            ed.onNodeChange.add(function (ed, cm, n, co) {
                if (ed.id.indexOf("question") > 0) {
                    cm.setDisabled('htsEditorMenu', false);
                }
                else {
                    cm.setDisabled('htsEditorMenu', true);
                }

                //                var nodeType = n.getAttribute('hts-data-type'),
                //                    formulaSelected = (n.nodeName == 'IMG') && (nodeType == 'formula'),
                //                    shortResponseSelected = (n.nodeName == 'IMG') && (nodeType == 'short'),
                //                    multiResponseSelected = (n.nodeName == 'IMG') && (nodeType == 'multi');

                //                cm.setActive('htsFormulaEditor', formulaSelected);
                //                cm.setActive('htsShortResponseEditor', shortResponseSelected);
                //                cm.setActive('htsMultiResponseEditor', multiResponseSelected);

                //                cm.setDisabled('htsFormulaEditor', !formulaSelected);
                //                cm.setDisabled('htsShortResponseEditor', !shortResponseSelected);
                //                cm.setDisabled('htsMultiResponseEditor', !multiResponseSelected);

            });

            ed.onDblClick.add(function (ed, e) {

                var nodeType = e.target.getAttribute('hts-data-type');

                switch (nodeType) {
                    case "math":
                        ed.execCommand('htsFormulaResponseEditor', e.target);
                        break;
                    case "text":
                        ed.execCommand('htsTextResponseEditor', e.target);
                        break;
                    case "multi":
                        ed.execCommand('htsMultiResponseEditor', e.target);
                        break;
                    case "numeric":
                        ed.execCommand('htsNumericResponseEditor', e.target);
                        break;
                    case "variable":
                        ed.execCommand('htsInsertVariableInstance', null);
                        break;
                    case "formula":
                        ed.execCommand('htsFormulaEditor', e.target);
                        break;
                }


            });
        },

        /**
        * Creates control instances based in the incomming name. This method is normally not
        * needed since the addButton method of the tinymce.Editor class is a more easy way of adding buttons
        * but you sometimes need to create more complex controls like listboxes, split buttons etc then this
        * method can be used to create those.
        *
        * @param {String} n Name of the control to create.
        * @param {tinymce.ControlManager} cm Control manager to use inorder to create new control.
        * @return {tinymce.ui.Control} New control instance or null if no control was created.
        */
        createControl: function (n, cm) {
            switch (n) {
                case 'htsEditorMenu':
                    var mnu = cm.createSplitButton('htsEditorMenu', {
                        title: 'Insert Response Area',
                        image: _static.urlText + '/img/response.png',
                        onclick: function () { }
                    });

                    mnu.onRenderMenu.add(function (mnu, m) {
                        //menu title, add if needed
                        //m.add({ title: 'Editors', 'class': 'mceMenuItemTitle' }).setDisabled(true);

                        m.add({ title: 'Numeric', 'class': 'numericResponse', cmd: 'htsNumericResponseEditor', image: _static.urlText + '/img/response.gif' });
                        m.add({ title: 'Text', 'class': 'textResponse', cmd: 'htsTextResponseEditor', image: _static.urlText + '/img/response.gif' });
                        m.add({ title: 'Formula', 'class': 'formulaResponse', cmd: 'htsFormulaResponseEditor', image: _static.urlText + '/img/response.gif' });
                        m.add({ title: 'Multiple Choice', 'class': 'multiResponse', cmd: 'htsMultiResponseEditor', image: _static.urlText + '/img/response.gif' });

                        // This really needs to be done with a callback
                        setTimeout(function () {
                            mnu.editor.nodeChanged();
                        }, 50);
                    });

                    return mnu;
            }

            return null;
        },

        /**
        * Returns information about the plugin as a name/value array.
        * The current keys are longname, author, authorurl, infourl and version.
        *
        * @return {Object} Name/value array containing information about the plugin.
        */
        getInfo: function () {
            return {
                longname: 'hts',
                author: 'BFW-NPS',
                authorurl: 'http://tinymce.moxiecode.com',
                infourl: 'http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/example',
                version: "1.0"
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('hts', tinymce.plugins.hts);
})();