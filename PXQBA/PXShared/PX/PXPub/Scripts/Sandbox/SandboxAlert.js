var PxSandboxAlert = function ($) {
    // set up default options
    var defaults = {
        PublishChangesButton: '#sandbox_publish_button',
        RevertChangesButton: '#sandbox_revert_button',
        HelpLink: '#sandbox_help_link',
        PublishModal: '#sandbox_notice_modal',
        RevertModal: '#sandbox_revert_modal',
        HelpModal: '#sandbox_helpbox',
        CancelPublish: "#CancelPublish",
        CancelRevert: "#CancelRevert",
        CloseHelp: "#CloseHelp",
        RevertActionLink: "#RevertActionLink",
        RevertWait: ".revert-wait",
        PublishActionLink: "#PublishActionLink",
        PublishWait: ".publish-wait",
        AlertContainer: "#sandbox_alert_widget",
        ReloadButtom: "#reload-alertbox"

    };

    return {
        Init: function () {
            $(document).off('click', defaults.PublishChangesButton).on("click", defaults.PublishChangesButton, PxSandboxAlert.PublishChangesModal);
            $(document).off('click', defaults.RevertChangesButton).on("click", defaults.RevertChangesButton, PxSandboxAlert.RevertChangesModal);
            $(PxPage.switchboard).unbind("refereshsandboxalert", PxSandboxAlert.RefreshSandboxAlert);
        },
        InitHelp: function () {
            $(document).off('click', defaults.HelpLink).on("click", defaults.HelpLink, PxSandboxAlert.HelpModal);
            $(document).off('click', defaults.ReloadButtom).on("click", defaults.ReloadButtom, PxSandboxAlert.ReloadAlertBox);
            $(PxPage.switchboard).unbind("refereshsandboxalert").bind("refereshsandboxalert", PxSandboxAlert.RefreshSandboxAlert);
            $(PxPage.switchboard).unbind("fneclosing").bind("fneclosing", PxSandboxAlert.RefreshSandboxAlert);
        },
        PublishChangesModal: function () {
            var DialogID = defaults.PublishModal;
            if ($(DialogID).length > 0) {
                $(DialogID).dialog({
                    resizable: false,
                    width: 450,
                    height: 260,
                    modal: true,
                    dialogClass: 'no-title-bar'
                });
                $(DialogID).find(defaults.CancelPublish).unbind().bind("click", PxSandboxAlert.ClosePublishChangesModal);
                $(DialogID).find(defaults.PublishWait).hide();
                $(DialogID).find(defaults.PublishActionLink).unbind().bind("click", function () {
                    $(DialogID).find(defaults.PublishWait).show();
                    $(this).hide();
                    $(DialogID).find(defaults.CancelPublish).hide();
                    document.location = $(this).attr("ref");
                });
            }
        },
        RevertChangesModal: function () {
            var DialogID = defaults.RevertModal;
            if ($(DialogID).length > 0) {
                $(DialogID).dialog({
                    resizable: false,
                    width: 450,
                    height: 260,
                    modal: true,
                    dialogClass: 'no-title-bar'
                });
                $(DialogID).find(defaults.CancelRevert).unbind().bind("click", PxSandboxAlert.CloseRevertChangesModal);
                $(DialogID).find(defaults.RevertWait).hide();
                $(DialogID).find(defaults.RevertActionLink).unbind().bind("click", function () {
                    $(DialogID).find(defaults.RevertWait).show();
                    $(this).hide();
                    $(DialogID).find(defaults.CancelRevert).hide();
                    document.location = $(this).attr("ref");
                });
            }
        },
        HelpModal: function () {
            var DialogID = defaults.HelpModal;
            if ($(DialogID).length > 0) {
                $(DialogID).dialog({
                    resizable: false,
                    width: 500,
                    height: 350,
                    modal: true,
                    dialogClass: 'no-title-bar'
                });
                $(DialogID).find(defaults.CloseHelp).unbind('click').bind("click", PxSandboxAlert.CloseHelpModal);
            }
        },
        ReloadAlertBox: function () {
            var routeURL = $(this).attr("ref");
            if (routeURL != undefined) {
                var bodyPostContent = $.ajax({
                    type: "POST",
                    url: routeURL, /* URL */
                    data: {},
                    beforeSend: function (jqXHR, settings) {
                        PxPage.Loading($(defaults.AlertContainer).attr("id")); /* Blocks the UI on Ajax call */
                    },
                    success: function (data) {
                        if (data.Status != undefined && data.Status == "error")
                            PxPage.log('Unexpected error occurred while loading.');
                        else {
                            var dataHtml = $(data).html();
                            $(defaults.AlertContainer).html(dataHtml);
                            if ($(defaults.AlertContainer).find(defaults.HelpModal).length)
                                PxSandboxAlert.InitHelp();
                            else
                                PxSandboxAlert.Init();
                        }
                    },
                    complete: function (jqXHR, textStatus) {
                        PxPage.Loaded($(defaults.AlertContainer).attr("id")); /* Unblocks the UI after Ajax call */
                    }
                });
            }
        },
        RefreshSandboxAlert: function () {            
            if ($(defaults.ReloadButtom).length) {
                $(defaults.ReloadButtom).click();
            }
        },
        ClosePublishChangesModal: function () {
            var DialogID = defaults.PublishModal;
            $(DialogID).dialog("close");
        },

        CloseRevertChangesModal: function () {
            var DialogID = defaults.RevertModal;
            $(DialogID).dialog("close");
        },

        CloseHelpModal: function () {
            var DialogID = defaults.HelpModal;
            $(DialogID).dialog("close");
        }

    }
} (jQuery);