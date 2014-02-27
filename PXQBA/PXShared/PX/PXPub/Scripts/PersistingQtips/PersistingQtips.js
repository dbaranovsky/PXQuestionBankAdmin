//Script file for controlling actions associated with the BeingMeta Search Widget
var PxPersistingQtips = function ($) {
    var _static = {
        sel: {
            webTourElements: '.webtour',
            closeLnk: '.qtipClose',
            toolTipId: 'toolTipId',
            persistingToolTip: '.persistingtooltip',
            qTip: '.qtip',
            relatedContentTooltip: '.webtour[tooltipid=PX_RelatedContent_Tooltip]'
        },

        fn: {

            init: function () {
                //close the qtip and create a flag on the user object in dlap to mark the qtip as closed
                $(document).off('click', _static.sel.closeLnk).on('click', _static.sel.closeLnk, _static.fn.closeLnkClicked);
                // toggle the persistent qtips when the user switches between the tabs
                $(PxPage.switchboard).bind('TogglePersistentQtips', _static.fn.togglePersistentQtips);
                //init the persistent qtips
                $(PxPage.switchboard).bind('InitPersistentQtips', _static.fn.initPersistentQtips);
                //show or hide the related content qtip
                $(PxPage.switchboard).bind('ToggleRelatedContentToolTip', _static.fn.toggleRelatedContentToolTip);
                //hide the persistent qtips during an fne mode (fne mode shows up when the instructor tries to activate a course)
                $(PxPage.switchboard).bind("fneprep", _static.fn.togglePersistentQtips)
            },

            closeLnkClicked: function () {
                var toolTipId = $(this).attr(_static.sel.toolTipId);
                //ajax call to create a flag on the user object in dlap to mark the qtip as closed
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.close_persistent_qtip,
                    data: { tooltipId: toolTipId },
                    success: function (response) {
                        var qTipElement = _static.sel.webTourElements + '[toolTipId="' + toolTipId + '"]'; //gets the target element on which the qTip was built
                        //destroy the qtip and remove the webtour element
                        $(qTipElement).qtip('destroy');
                        $(qTipElement).remove();
                    },
                    error: function (req, status, error) {
                        PxPage.log('Error on request for tooltip close: toolTipId="' + toolTipId + '"');
                    }
                });
            },

            //Toggle intializes the persistent Qtips when on Xbook and destroys the tooltip when moved to any other tab
            togglePersistentQtips: function (event, tabName) {
                var persistingQtips = $(_static.sel.persistingToolTip).closest(_static.sel.qTip);
                if (tabName == 'xbook') {
                    //initializes the persistent qtips when navigating to the xbook tab for the first time (initializing the Qtip while on the home tab is causing positioning issues)
                    _static.fn.initPersistentQtips();
                }
                else {
                    _static.fn.destroyPersistentQtips();
                }
            },

            //destroy the persistent qtips
            destroyPersistentQtips: function () {
                $(_static.sel.webTourElements).qtip('destroy');
            },

            // Initializes the Qtips for all the elements with 'webtour' class
            initPersistentQtips: function () {
                $(_static.sel.webTourElements).each(function () {
                    //if there is already a qTip, skip the initialization
                    if ($(this).data('qtip')) {
                        return;
                    }
                    // Qtip initilization
                    var toolTipId = $(this).attr('toolTipId');
                    var toolTip = $('#' + toolTipId); //Tooltip item containing the description
                    var showToolTip = !$.parseJSON(toolTip.attr('isToolTipHidden').toLowerCase());
                    var qtipTooltipPosition = toolTip.attr('qtipTooltipPosition');
                    var qtipTargetPosition = toolTip.attr('qtipTargetPosition');
                    var qtipTarget = toolTip.attr('qtipTarget');

                    $(this).qtip({
                        content: {
                            text: toolTip.html()
                        },
                        position: {
                            target: $(qtipTarget), //target element
                            my: qtipTooltipPosition, //position of the qtip
                            at: qtipTargetPosition //position of the target
                        },
                        show: { ready: showToolTip }, //show the Qtip once it is ready
                        hide: false, //this makes the Qtip into a persistent tooltip
                        style: {
                            tip: true,
                            classes: toolTipId //adding a custom class to the Qtip. This will be used for the custom styling of each Qtip.
                        }
                    });
                });
            },

            //show or hide the related content qtip
            toggleRelatedContentToolTip: function (event, showToolTip) {
                if (showToolTip) {
                    $(_static.sel.relatedContentTooltip).qtip("show");
                }
                else {
                    $(_static.sel.relatedContentTooltip).qtip("hide");
                }
            }

        }
    };

    return {

        Init: function () {
            _static.fn.init();
        }
    };
} (jQuery);