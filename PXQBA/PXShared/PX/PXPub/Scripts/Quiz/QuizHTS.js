var PxQuizHts = function($) {
    return {        
        GetDataForGraphQuestion: function (questionId) {
            PxPage.log("entering GetDataForGraphQuestion");

            var xml = "";
            try {
                //Run some code here
                xml = document.getElementById("flash").getXML();
            }
            catch (err) {
                //Handle errors here
                PxPage.log("ending GetDataForGraphQuestion | could not retrieve XML for custom question:" + questionId);
            }

            PxPage.log("ending GetDataForGraphQuestion | returns xml");
            return xml;
        },
        StoreAndPreviewGraphQuestion: function (questionUrl) {
            var customXml = PxQuizHts.GetDataForGraphQuestion();
            $.ajax({
                url: PxPage.Routes.store_live_quiz,
                cache: false,
                success: function (questionId) {
                    PxQuizHts.LoadHTSPreview(questionId, questionUrl);
                },
                data: { customXML: customXml, customURL: questionUrl },
                type: "POST"
            });
        },
        LoadHTSPreview: function (response, customUrl) {
            //Save the question first if it is in edit mode.
            var previewDiv = $('<div></div>');
            var title = 'Advanced Question Preview';
            previewDiv.addClass('preview-question-dialog');

            var showQuiz = function () {
                $.ajax({
                    url: PxPage.Routes.show_quiz,
                    cache: false,
                    success: function (xml) {
                        $("#custom_preview_" + response).empty();
                        previewDiv.html(xml);
                    },
                    data: { Id: response },
                    type: "POST"
                });
            };
            
            var buttons = {};
            if (customUrl != 'FMA_GRAPH') {
                buttons["RegenerateVariables"] = {
                    text: "Regenerate Variables",
                    click: function () {
                        showQuiz();
                    }
                };
            } else {
                title = 'Graph Exercise Preview';
            }

            var w = $(window).width() - 50;
            var h = $(window).height() - 50;
            
            previewDiv.dialog({
                width: w,
                height: h,
                minWidth: 700,
                minHeight: 300,
                modal: true,
                draggable: false,
                closeOnEscape: true,
                title: title,
                buttons: buttons,
                close: function () {
                    $(this).dialog('destroy').empty().remove();
                }
            });
                
            showQuiz();

            return false;
        },
        SetHtsRpcHooks: function () {
            if ($('#custom-hts-editor').length) {
                var remoteUrl = $('#custom-hts-editor').attr('rel');

                var consumer = new easyXDM.Rpc({
                    remote: remoteUrl,
                    container: 'custom-hts-editor'
                },
                {
                    local: {
                        questionSaved: function (questionId, success, error) {
                            PxPage.Loading('fne-window');
                            $.post(
                                PxPage.Routes.remove_question_from_cache, {
                                    questionId: questionId
                                },
                                function (response) {
                                    $('.question-editor').questioneditor('saveNewQuestionPoints', function() {
                                        $('.question-editor').questioneditor('updateQuizEditorQuestions', null, function () { });
                                        PxPage.log('remove_question_from_cache : ' + response);
                                        if (success) {
                                            PxPage.Loaded('fne-window');
                                            success();
                                        }

                                    }, function() {
                                    });
                                }
                            );
                            PxPage.Toasts.Success('Question Saved.');

                        }
                    },
                    remote: {
                        saveQuestion: {},
                        previewQuestion: {},
                        isDirty: {}
                    }
                });
                PxQuiz.HTS_RPC = consumer;
            }
        },
    };
}(jQuery);