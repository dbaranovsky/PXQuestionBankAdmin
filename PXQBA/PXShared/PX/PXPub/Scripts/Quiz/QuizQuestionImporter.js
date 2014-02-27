//jQuery plugin for Quiz Questions Import functionality

(function ($) {
    var _static = {
        pluginName: "questionimport",
        dataKey: "questionlist",
        bindSuffix: "questionlist",
        isInitialized: false,
        defaults: {

        },
        sel: {
            fneContent: '#fne-content',
            dialogWin: ".question-pool-dialog-text",
            importerText: '#importer-text',
            importerHelpLink: ".importer-help-link",
            importerSampleText: '.importer-sample-text',
            importerSampleMultipleChoice: '.importer-sample-multiple-choice',
            importerSampleFillInBlank: '.importer-sample-fill-in-blank',
            importerHelpArea: '.importer-help-area',
            importerHelpSel: '.importer-help-sel',
            importerButtonImport: '.importer-button-import',
            importerButtonTest: '.importer-button-test',
            importerResponse: '.importer-response',
            itemId: '.item-id',
            tagId: '#importer-dialog',
            importerErrorClose: '.importer-error-close',
            tooltip: 'tooltip',
            quizEditor: '.quiz-editor'
        },
        //private functions
        fn: {
            ShowImporterDialog: function () {
                var tag = $('<div id="importer-dialog" class="question-pool-dialog-text"></div>');
                $.get(PxPage.Routes.import_dialog, function (response) {
                    tag.html(response);
                    tag.dialog({
                        title: "Import questions",
                        width: 600,
                        minWidth: 600,
                        height: 'auto',
                        minHeight: 400,
                        modal: true,
                        draggable: false,
                        resizable: false,
                        closeOnEscape: true,
                        close: function() {
                            $(this).dialog('destroy').remove();
                        },
                        buttons: [
                            { 
                                text: 'Import questions', 
                                'class': _static.sel.importerButtonImport.replace('.', ''), 
                                click: function() { _static.fn.ImportQuestions(tag); } 
                            },
                            {
                                text: 'Test questions', 
                                'class': _static.sel.importerButtonTest.replace('.', ''), 
                                click: function() { _static.fn.TestQuestions(tag);}
                            },
                            {
                                text: 'Cancel', 
                                'class': _static.sel.importerButtonTest.replace('.', ''), 
                                click: function() { tag.dialog("close"); }
                            }]
                    });

                    _static.fn.BindDialogControls();
                });

            },
            BindDialogControls: function () {
                $(_static.sel.importerHelpLink).bind('click', function () {
                    _static.fn.ShowHideSample();
                });

                $(_static.sel.importerSampleMultipleChoice).bind('click', function () {
                    _static.fn.ShowSampleText(this);
                });

                $(_static.sel.importerSampleFillInBlank).bind('click', function () {
                    _static.fn.ShowSampleText(this);
                });

                _static.fn.BindQtips();
            },
            BindQtips: function () {
                $('[' + _static.sel.tooltip + ']').qtip("destroy");

                var tooltips = $('[' + _static.sel.tooltip + ']');

                for (var i = 0; i < tooltips.length; i++) {
                    $(tooltips[i]).qtip({
                        content: {
                            text: $(tooltips[i]).attr(_static.sel.tooltip)
                        },
                        show: { ready: false, solo: true },
                        position: {
                            my: 'bottom center'
                        },
                        style: { padding: 0, width: { min: 140, max: 300} }
                    });
                }
            },
            ShowHideSample: function () {
                if ($(_static.sel.importerSampleText).html() == '') {
                    $(_static.sel.importerSampleMultipleChoice).click();
                }

                if ($(_static.sel.importerHelpArea).css('display') == 'none') {
                    $(_static.sel.importerHelpArea).css('display', 'block');
                    $(_static.sel.importerHelpSel).html('-');
                }
                else {
                    $(_static.sel.importerHelpArea).css('display', 'none');
                    $(_static.sel.importerHelpSel).html('+');
                }
            },
            ShowSampleText: function (link) {
                $(_static.sel.importerSampleMultipleChoice).removeClass('active');
                $(_static.sel.importerSampleFillInBlank).removeClass('active');

                $(link).addClass('active');

                if ($(link).hasClass(_static.sel.importerSampleMultipleChoice.replace('.', ''))) {
                    $(_static.sel.importerSampleMultipleChoice + "-placeholder").css('display', 'block');
                    $(_static.sel.importerSampleFillInBlank + "-placeholder").css('display', 'none');
                }

                if ($(link).hasClass(_static.sel.importerSampleFillInBlank.replace('.', ''))) {
                    $(_static.sel.importerSampleMultipleChoice + "-placeholder").css('display', 'none');
                    $(_static.sel.importerSampleFillInBlank + "-placeholder").css('display', 'block');
                }
            },
            ValidateContent: function (content) {
                if (content.length == 0) {
                    PxPage.Toasts.Error('Please provide text to parse!');
                    return false;
                }

                return true;
            },
            TestQuestions: function (tag) {
                var request = $(_static.sel.importerText).val();

                if (!_static.fn.ValidateContent(request)) {
                    return false;
                }

                PxPage.Loading(_static.sel.tagId);

                tag.find(_static.sel.importerResponse).html('');

                $.ajax({
                    dataType: "json",
                    cache: false,
                    type: 'POST',
                    url: PxPage.Routes.validate_respondus_questions,
                    data: { data: request },
                    complete: function () {
                        PxPage.Loaded(_static.sel.tagId);
                    },
                    success: function (response) {
                        _static.fn.DisplayErrors(tag, response);
                    }
                });
            },
            ImportQuestions: function (tag) {
                var request = $(_static.sel.importerText).val();

                if (!_static.fn.ValidateContent(request)) {
                    return false;
                }

                PxPage.Loading(_static.sel.tagId);

                $.ajax({
                    dataType: "json",
                    type: 'POST',
                    url: PxPage.Routes.import_respondus_questions,
                    data: { data: request, quizId: $(_static.sel.fneContent).find('.item-id').val() },
                    complete: function () {
                        PxPage.Loaded(_static.sel.tagId);
                    },
                    success: function (response) {
                        _static.fn.DisplayErrors(tag, response);

                        if (tag.find(_static.sel.importerErrorClose).length == 0) {
                            tag.dialog('destroy');
                            PxQuiz.UpdateQuestionList();
                        }
                    }
                });
            },
            DisplayErrors: function (tag, response) {
                var divRes = "<div class='importer-success-header'>No errors found</div>";

                if (response.length > 0 && response != "SUCCESS") {
                    divRes = "<div class='importer-error'><div class='importer-error-header'>Errors<div class='importer-error-close'>X</div></div>";
                    divRes += "<div class='importer-error-body' style='max-height: 70px;overflow-y: auto;'>";

                    for (var i = 0; i < response.length; i++) {
                        var error = response[i].split('|');

                        divRes += "<div style='float: left; padding-top: 5px; padding-bottom: 5px;'>" + error[1] + "</div><div style='float: right; padding-top: 5px; padding-bottom: 5px;'>line " + error[0] + "</div>";
                        divRes += "<div style='background-color: #f8e1e7;clear: both; padding-top: 5px; padding-bottom: 5px;'>" + error[2] + "</div>";
                    }

                    divRes += "</div></div>";
                }

                tag.find(_static.sel.importerResponse).html(divRes);

                $(_static.sel.importerErrorClose).bind('click', function () {
                    tag.find(_static.sel.importerResponse).html('');
                });
            }
        }
    };
    //public functions
    var api = {
        init: function () {
            if (!_static.isInitialized) {
                $(PxPage.switchboard).bind("showimporterdialog.questionimport", function () {
                    _static.fn.ShowImporterDialog();
                });

                _static.isInitialized = true;
            }
        }
    };

    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.questionimport = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };
} (jQuery));