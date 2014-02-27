var PxCustomWidget = function ($) {
    return {
        BindControls: function () {
            for (editor in tinyMCE.editors) {
                tinyMCE.editors[editor].destroy();
            }

            $('.widgetName').each(function () {
                if ($(this).find("#Title").val() == '') {
                    $(this).find("#Title").addClass('customWidgetWaterMarkedTextBox');
                    $(this).find("#Title").val('Enter widget title here');
                }
            });

            $('.widgetName').focusout(function (event) {
                event.stopImmediatePropagation();
                if ($(this).find("#Title").val() == '') {
                    $(this).find("#Title").addClass('customWidgetWaterMarkedTextBox');
                    $(this).find("#Title").val('Enter widget title here');
                }
            });

            $('.widgetName').click(function (event) {
                event.stopImmediatePropagation();
                var bool = $(this).find("#Title").hasClass('customWidgetWaterMarkedTextBox');
                if (bool) {
                    $(this).find("#Title").val('');
                    $(this).find("#Title").removeClass('customWidgetWaterMarkedTextBox');
                }
            });

            //            $("#btnCustomWidgetSave").hover(
            //               function () {
            //                   $(this).addClass("btnCustomWidgetSavehover");
            //               },
            //                function () {
            //                    $(this).removeClass("btnCustomWidgetSavehover");
            //                }
            //            );
        },
        SubmitFromButton: function (zoneId, widgetTemplateId, widgetId) {
            $('#btnCustomWidgetSave').attr('disabled', true);

            if ($('#saveItem').find("#Title").hasClass("customWidgetWaterMarkedTextBox")) {
                $('#saveItem').find("#Title").val("");
                $('#saveItem').find("#Title").removeClass("customWidgetWaterMarkedTextBox")
            }
            if (PxCustomWidget.ValidateName(true)) {
                var belowSeq = $('#' + zoneId).find('div[templateid]').first().not('[id=' + widgetId + ']').attr('sequence');
                if (belowSeq == undefined) {
                    belowSeq = '';
                }

                PxPage.Loading("saveItem");
                PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save', ZoneId: zoneId, belowSequence: belowSeq },
                    success: function () {
                        var widgetId = $("#saveItem").find("#Id").val();
                        var mode = $("#saveItem").find("#Mode").val();

                        if (mode == "ADD") {
                            $(".pageContainer").PageLayout("addWidget", widgetTemplateId, zoneId, widgetId);
                        }
                        else if (mode == "EDIT") {
                            $(".pageContainer").PageLayout("reloadWidget", widgetId, widgetTemplateId);
                            PxPage.Loaded(zoneId);
                        }
                        $('#btnCustomWidgetSave').attr('disabled', false);
                        $('#saveItem').remove();
                        $('#btnCustomWidgetSave').closest(".ui-dialog").remove();
                    }
                });
                PxPage.Loading("saveItem");
            }
            else {
                $('#btnCustomWidgetSave').attr('disabled', false);
                if ($('#saveItem').find("#Title").hasClass("customWidgetWaterMarkedTextBox")) {
                    $('#saveItem').find("#Title").removeClass("customWidgetWaterMarkedTextBox")
                }
                return false;
            }
        },

        ValidateName: function () {
            var sel = '.title';

            if ($(sel).val().indexOf('<') != -1 ||
                    $(sel).val().indexOf('>') != -1) {
                PxPage.Toasts.Error('Widget name cannot contain html tags.');
                $(sel).focus();
                return false;
            }

            var title = $(sel).val();
            title = jQuery.trim(title);
            if (title == '') {
                PxPage.Toasts.Error('Widget name cannot be blank');
                $(sel).focus();
                return false;
            }

            return true;
        }
    }
} (jQuery);