var PxLinkCollection = function ($) {
    // static data
    var _static = {
        defaults: {
        },
        settings: {
        },
        sel: {
            linkUrl: "#linkUrl",
            linkTitle: "#linkTitle",
            linkUrlFilled: "#linkUrl:filled",
            linkTitleFilled: "#linkTitle:filled",
            saveItem: "#saveItem",
            contentItem: "#content-item",
            uiPopup: ".ui-dialog .ui-dialog-content",
            popupTitle: ".divPopupTitle",
            popupLink: '.linkOpenPopup',
            currentPopup: '.divPopupWin:visible',
            savelink: ".savelink",
            linkError: "#spnLinkError",
            formsubmit: '#formsubmit'
        },
        fn: {
            operationLinkBegin: function () {
                PxPage.Loading('fne-content');

                if (!$(_static.sel.contentItem).is(':visible')) {
                    $(_static.sel.contentItem).attr('id', _static.sel.contentItem.replace('#', 'tmp_'));
                }
            },
            operationLinkComplete: function () {
                if ($('#tmp_' + _static.sel.contentItem.replace('#', '')).length > 0) {
                    $('#tmp_' + _static.sel.contentItem.replace('#', '')).attr('id', _static.sel.contentItem.replace('#', ''));
                }

                PxPage.Loaded('fne-content');
            },
            showLinkOpenPopupAsDialog: function (event) {
                $(_static.sel.savelink).show();

                var options = { modal: true, draggable: false, closeOnEscape: true, width: '500px', height: '400px', resizable: false, autoOpen: false };
                var tag = $("#" + event.target.rel); 

                var args = {
                    filterid: '',
                    syllabustype: '',
                    isReadOnly: false
                };

                $(_static.sel.linkTitle).val("");
                $(_static.sel.linkUrl).val("http://");

                tag.dialog({
                    modal: options.modal, title: options.title, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, resizable: options.resizable, autoOpen: options.autoOpen,
                    close: function () {
                        $(_static.sel.linkError).hide();
                        $(_static.sel.linkTitle).val("");
                        $(_static.sel.linkUrl).val("http://");
                    }
                }).dialog('open');

                tag.dialog().parent().css('top', ($(window).height() - 200) / 2);

                $(_static.sel.popupTitle).hide();
            },
            getValidator: function () {
                $(_static.sel.saveItem).validate({
                    rules: {
                        linkTitle: {
                            //required: true,
                            maxlength: 50
                        },
                        linkUrl: {
                            //required: true,
                            maxlength: 200,
                            url: true
                        }
                    },
                    messages: {
                        linkTitle: {
                            required: "Please enter title",
                            maxlength: "Maximum 50 characters allowed"
                        },
                        linkUrl: {
                            required: "Please enter URL",
                            maxlength: "Maximum 200 characters allowed",
                            url: "Please enter a valid URL"
                        }
                    }
                });
            },
            onClickSave: function (OnSuccess, IsOpen) {
                _static.fn.getValidator();

                var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;

                if (PxPage.ValidateTitle(true) && PxPage.ValidateUrl(_static.sel.linkUrl)) {
                    if (!IsOpen) {
                        PxContentTemplates.SetTemplateReloadMode('modal');
                    }

                    PxPage.OnFormSubmit('Processing...', true, {
                        form: _static.sel.saveItem, data: { behavior: 'Save' }, externalData: ContentWidget.CreateAndAssign,
                        rules: {
                            linkTitle: { required: _static.sel.linkUrlFilled },
                            linkUrl: {
                                required: _static.sel.linkTitleFilled,
                                regex: RegExp
                            }
                        },
                        updateSelector: _static.sel.contentItem, success: OnSuccess
                    }, function () {
                        if (IsOpen) {
                            ContentWidget.NavigateAway()
                        }
                    });
                }
                else {
                    return false;
                }

                return true;
            },
            onAddLinkSave: function (event) {
                var popup = $(_static.sel.currentPopup);
                var linkTitle = popup.find(_static.sel.linkTitle);
                var linkError = popup.find(_static.sel.linkError);
                var linkUrl = popup.find(_static.sel.linkUrl);
                var submit = popup.find(_static.sel.formsubmit);

                if ($.trim(linkTitle.val()) == '') {
                    linkError.text('Title cannot be empty.');
                    linkError.show();
                    linkTitle.focus();

                    return false;
                }

                var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;
                var link = $.trim(linkUrl.val());

                linkError.text('Please enter a valid link.');

                if (link == '' || !RegExp.test(link)) {
                    linkError.show();
                    linkUrl.focus();
                    event.returnValue = false;

                    return false;
                }
                else {
                    PxPage.OnFormSubmit();
                    submit.click();
                    _static.fn.closeAddResourse();

                    return true;
                }
            },
            closeAddResourse: function () {
                $(_static.sel.uiPopup).dialog('close');
            }
        }
    };
    return {
        AddLink: function () {
            return _static.fn.onAddLinkSave();
        },
        OperationLinkBegin: function () {
            return _static.fn.operationLinkBegin();
        },
        OperationLinkComplete: function () {
            return _static.fn.operationLinkComplete();
        },
        BindControls: function () {
            $(_static.sel.popupLink).die().live('click', PxLinkCollection.ShowLinkOpenPopupAsDialog);
        },
        ShowLinkOpenPopupAsDialog: function (event) {
            return _static.fn.showLinkOpenPopupAsDialog(event);
        },

        OnClickSave: function (OnSuccess, IsOpen) {
            return _static.fn.onClickSave(OnSuccess, IsOpen);
        },
        CloseAddResourse: function () {
            return _static.fn.closeAddResourse();
        }
    };
} (jQuery);