// LearningCurve
//
// This plugin is responsible for the client-side behaviors of the learning curve

(function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "LearningCurve",
        dataKey: "LearningCurve",
        locked: false,
        lcapi: [],
        learningCurveRpc: null,
        learningCurveRpcFunctions: {
            local: {
                updateTargetScoreInItem: function (entityId, itemId, targetScore) {
                    _static.fn.updateTargetScoreInItem(entityId, itemId, targetScore);
                },
                closeParentDialog: function(){
                    _static.fn.closeParentDialog();
                },
                displayContent: function (itemId) {
                    window.parent.parent.LearningCurve.displayContent(itemId);
                },
                getUserEmailAddress: function (enrollmentIds) {
                    _static.fn.getUserEmailAddress(enrollmentIds);
                }

            },
            remote: {
                setUserEmailAddress: {}
            }
        },
        defaults: {
            readOnly: false
        },
        sel: {
            autoTargetScore: "#AutoTargetScore",
            learningCurveTargetScore: "#LearningCurveTargetScore",
            targetScoreSettingsLi: ".target-score-settings",
            relatedContentDialog: ".related-content-dialog .learning-curve-related-content #content-item"
        },
        // private functions
        fn: {
            updateTargetScoreInItem: function (entityId, itemId, targetScore) {
                $.post(PxPage.Routes.LearningCurve_UpdateTargetScoreInItem, { entityId: entityId, itemId: itemId, targetScore: targetScore }, function (response) {
                    //Update LC activity url
                    var lcDiv = window.parent.parent.$('#main .itemRow[data-aw-id="' + itemId + '"] .colIndex1 span');
                    var url = _static.fn.updateQueryStringParameter(lcDiv.attr('url'), 'st', targetScore);
                    lcDiv.attr('url', url);
                    //Update LC report url
                    var lcReportDiv = window.parent.parent.$('#main .itemRow[data-aw-id="' + itemId + '"] .colIndex3 span');
                    var reportUrl = _static.fn.updateQueryStringParameter(lcReportDiv.attr('url'), 'st', targetScore);
                    lcReportDiv.attr('url', reportUrl);

                });

            },
            updateQueryStringParameter: function (uri, key, value) {
                var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
                separator = uri.indexOf('?') !== -1 ? "&" : "?";
                if (uri.match(re)) {
                    return uri.replace(re, '$1' + key + "=" + value + '$2');
                }
                else {
                    return uri + separator + key + "=" + value;
                }
            },
            initalizeEasyXdm: function (remoteUrl) {

                _static.learningCurveRpc = new easyXDM.Rpc({
                    remote: remoteUrl + '&platform=px'
                }, _static.learningCurveRpcFunctions);
            },
            initializeIframe: function (modelId) {
                var target = $(".lc_iframe-host");
                if (target.length <= 0) {
                    PxPage.log("no LCAPI targets");
                    return;
                }
                if (target.length > 0) {
                    target.each(function (i, e) {
                        PxPage.log("LCAPI target: " + $(e).attr("rel"));
                        var frame = $(e).find("#lc-body-iframe");
                        if (frame.length === 0) {
                            PxPage.log("LCAPI no iframe");

                            _static.lcapi.push(new easyXDM.Rpc({
                                remote: $(e).attr("rel"),
                                container: $(e).attr("id"),
                                props: {
                                    id: "lc-body-iframe"
                                }
                            }, {
                                local: {
                                    getLearningCurveData: function (ext_message, successFn) {
                                        //*
                                        var args = {
                                            Id: modelId,
                                            questionId: ext_message.qid
                                        };

                                        $.get(PxPage.Routes.LearningCurve_GetData, args, function (response) {
                                            successFn(response);
                                        });
                                        //*/

                                    },

                                    openContent: function (args) {
                                        _static.fn.initializeDialog(args);
                                    }
                                },
                                remote: {}
                            }));
                        }
                        else {
                            PxPage.log("LCAPI already initialized");
                        }
                    });
                }
            },
            getUserEmailAddress: function (enrollmentIds, cb) {
                $.ajax({
                    traditional: true,
                    type: "POST",
                    url: PxPage.Routes.LearningCurve_GetUserEmailAddressJsonResult,
                    data: { enrollmentIds: enrollmentIds },
                    success:function(data, textStatus, jqXHR) {
                        if (cb && typeof (cb) === 'function') {
                            cb(data);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (cb && typeof(cb) === 'function') {
                            cb(null);
                        }
                        console.log('error getUserEmailAddress(), status: ' + textStatus + ', thrown: ' + errorThrown);
                    }
                });
            },
            displayContent: function (itemId) {
                if ($('#fne-lc-dialog').length == 0)
                    $('#fne-window').append('<div id="fne-lc-dialog"><div id="fne-lc-dialog-content"></div></div>');

                $.post(PxPage.Routes.LearningCurve_GetEbookInfo, { itemId: itemId }, function (response) {

                    if (response && response.Status === 'Success') {
                        itemId = response.Id && response.Id != '' ? response.Id : itemId;
                        _static.fn.displayContentInDialog(itemId, response.Title);
                    } else {
                        if (response.ProductType && response.ProductType !== 'LearningCurve') {

                            _static.fn.displayMessageInDialog('', 'The specified content item could not be found in this course');
                        }
                    }
                });

            },
            //Get item content from dlap and display it in a dialog.
            displayContentInDialog: function (itemId, itemTitle) {
                var url = PxPage.Routes.display_content + '?id=' + itemId + '&mode=4&includeNavigation=false&includeToc=false&reaOnly=true&includeDiscussion=false';
                $.post(url, {}, function (response) {
                    var fneHeight = $('#fne-window').height() * 0.8;
                    var fneWidth = $('#fne-window').width() * 0.8;

                    try {
                        $('#fne-lc-dialog').html(response);
                    } catch (e) {
                        PxPage.log('error in LearningCureve.displayContentInDialog: ' + e);
                    }
                    var contentUrl = $('#fne-lc-dialog .document-viewer-frame-host,#fne-lc-dialog #MainContentArea').attr('rel');

                    if (contentUrl) {
                        $('<iframe />', {
                            name: 'content',
                            id: 'content',
                            src: contentUrl,
                            height: "100%",
                            width: "100%"

                        }).appendTo('#fne-lc-dialog');

                        $('#fne-lc-dialog #content-item').remove();
                    }
                    $('#fne-lc-dialog').dialog({
                        modal: true,
                        width: fneWidth,
                        height: fneHeight,
                        title: itemTitle,
                        setFocus: !$.browser.mozilla,
                        open: function () {
                            $('#fne-lc-dialog').css('overflow', 'hidden');
                        },
                        close: function () {
                            $('#fne-lc-dialog').empty();
                        }
                    });
                });
            },
            displayMessageInDialog: function (title, message) {
                $('#fne-lc-dialog').html(message);
                $('#fne-lc-dialog').dialog({
                    modal: true,
                    width: 500,
                    height: 150,
                    title: title,
                    close: function () {
                        $('#fne-lc-dialog').empty();
                    }
                });
            },
            initializeDialog: function (args) {
                $(_static.sel.relatedContentDialog).dialog("close");
                $(_static.sel.relatedContentDialog).dialog("destroy");
                if (args.id != undefined) {
                    url = PxPage.Routes.display_content + "?id=" + args.id + "&mode=4&includeNavigation=false&includeToc=false&reaOnly=true&includeDiscussion=false";
                }
                if (url != null) {
                    url = url.replace('Shortcut__1__', '');
                }
                $('.related-content-dialog .learning-curve-related-content').load(url, function () {
                    ContentWidget.ContentLoaded();
                    _static.fn.showRelatedContentAddedDialog();
                });

            },

            closeParentDialog: function(){
                 window.parent.parent.$('#fne-unblock-action').click();
            },

            showRelatedContentAddedDialog: function () {
                $(_static.sel.relatedContentDialog).dialog({
                    autoOpen: true,
                    resizable: false,
                    zIndex: 2000,
                    modal: true,
                    minHeight: 700,
                    minWidth: 700,
                    draggable: true,
                    closeOnEscape: true,
                    dialogClass: 'relatedContentContentDialog'
                });
            },


            autoTargetScore: function (event) {
                if (event.target.checked) {
                    var settings = $(event.target).closest(_static.sel.targetScoreSettingsLi);
                    var prevTargetValue = settings.find('.hdnTargetScore').val();
                    $(_static.sel.learningCurveTargetScore).val(prevTargetValue);
                    $(_static.sel.learningCurveTargetScore).attr("disabled", "disabled");
                }
                else {
                    $(_static.sel.learningCurveTargetScore).removeAttr("disabled");
                }
            }


        }
    },
    // The public interface for interacting with this plugin.
    api = {
        init: function (options) {
            return this.each(function () {
                _static.fn.initializeIframe(options.modelId);
                $(document).off('click', _static.sel.autoTargetScore).on('click', _static.sel.autoTargetScore, _static.fn.autoTargetScore);
            });

        },
        getLcRpcFunctions: function() {
            return _static.learningCurveRpcFunctions;
        },
        setLcRpc: function(rpc) {
            _static.learningCurveRpc = rpc;
        }
    };


    $.fn.LearningCurve = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery." + _static.pluginName);
        }
    };

    window.LearningCurve = {
        initalizeEasyXdm: function (remoteUrl) {
            _static.fn.initalizeEasyXdm(remoteUrl);
        },
        displayContent: function (itemId) {
            _static.fn.displayContent(itemId);
        },
        getUserEmailAddress: function (enrollmentIds, cb) {
            _static.fn.getUserEmailAddress(enrollmentIds, cb);
        },
        closeParentDialog: function () {
            _static.fn.closeParentDialog();
        },
        updateTargetScoreInItem: function(entityId, itemId, targetScore) {
            _static.fn.updateTargetScoreInItem(entityId, itemId, targetScore);
        }
    };

} (jQuery))