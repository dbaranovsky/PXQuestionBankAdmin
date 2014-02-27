var PxDiscussion = function($) {
    return {
        BindControls: function () {
            $('iframe').load(function () {
                if (PxPage.Context.IsInstructor == "true") {
                    if ($('iframe').contents().find('.dropboxScoreBox')) {
                        $('iframe').contents().find('.dropboxScoreBox').css('border', 'transparent');
                        $('iframe').contents().find('.dropboxScoreBox').css('background', 'transparent');
                    }
                    if ($('iframe').contents().find('#dropboxDetails_FrameContent')) {
                        $('iframe').contents().find('#dropboxDetails_FrameContent').parents('td').css('display', 'none');
                    }
                }
            });

            $('.divPopupClose, .divPopupWin input[name="cancel"]').bind('click', function() { $.unblockUI(); return false; });
            $('#lnkAttachment').bind('click', function(event) {               
                $.blockUI({
                    message: $("#divAttachmentPopup"),
                    css: {
                        padding: 0,
                        margin: 0,
                        width: '30%',
                        top: '40%',
                        left: '35%',
                        textAlign: 'center'
                    }
                });
                return false;
            });

            //select all checkboxes in the document list when selectAll is clicked
            $(document).off('click','#attachmentTable input[name="cbkSelectAll"]').on('click', '#attachmentTable input[name="cbkSelectAll"]', function(event) {
                if (event.target.checked == true)
                    $('#attachmentTable .cbResource').prop('checked', true);
                else
                    $('#attachmentTable .cbResource').prop('checked', false);
            });
        },

        ValidateDeleteResources: function() {
            if (!$('#attachmentTable .cbResource').is(':checked')) {
                alert("Please select atleast one resource to delete");
                return false;
            }
            else {
                PxPage.OnFormSubmit();
                return true;
            }
        },
        
        Init: function () {
            PxDiscussion.BindControls();
        }
    }
} (jQuery);