var PxUpload = function ($) {
    return {
        Init: function () {
            PxUpload.BindControls();
        },


        BindControls: function () {
            $(document).on('click', '.upload-link', function () {
                PxUpload.ShowUploadAsDialog();
                return false;
            });
            $(document).on('click', '.uploadandsubmit-link', PxUpload.ShowUploadAndSubmitAsDialog);
            $(document).on('click', '.delete-link', PxUpload.OnDeleteLinkClick);
            $(document).on('click', '.upload-window .divPopupClose, .upload-window input[name="cancel"]', function () {
                $('body').unblock();
            });
            $(document).off('click', '.removeStudentSubmittedFile').on('click', '.removeStudentSubmittedFile', PxUpload.RemoveStudentSubmittedFile);

        },

        ShowUploadAndSubmitAsDialog: function () {
            var itemID = $('#divComposeItems').attr("itemid");
            var showUploadandSubmitModal = $('.showUploadandSubmitModal');

            var onCompleteScript = null;
            if ($('#onCompleteScript')) {
                onCompleteScript = $('#onCompleteScript').val();
            }

            $(showUploadandSubmitModal).html('<div id="loadingBlock" style="padding-top:10px;padding-bottom:10px;">Loading..</div>');
            $(showUploadandSubmitModal).attr('id', 'showUploadandSubmitModal');
            $(showUploadandSubmitModal).dialog({ width: 740, height: 280, minWidth: 740, minHeight: 280, modal: true, draggable: false, resizable: false, closeOnEscape: true,
                open: function (event, ui) {
                    $(showUploadandSubmitModal).closest(".ui-dialog-content").attr("style", "font-size:12px;font-family:arial;font-weight:normal;width: 740px;height: 310px;overflow:auto;");
                    $(showUploadandSubmitModal).closest(".ui-dialog").find(".ui-dialog-titlebar").attr("style", "display:block;");
                    $(showUploadandSubmitModal).closest(".ui-dialog").find(".ui-dialog-title").html("File to submit");
                },
                close: function (event, ui) {
                    if (tinyMCE.activeEditor) {
                        try {
                            tinyMCE.activeEditor.remove();
                        }
                        catch (ex) {
                        }
                    }
                    $(showUploadandSubmitModal).html('');
                    $(showUploadandSubmitModal).attr('style', '');
                }
            });
            
            var bodyPostContent = $.ajax({
                type: "POST",
                url: PxPage.Routes.show_upload_and_submit,
                data: { parentId: itemID, onCompleteScript: onCompleteScript },
                success: function (dataShow) {
                    $(".ui-dialog-titlebar").attr("style", "display:block;");
                    //$(".ui-dialog").attr("style", "display:block;z-index:9999;font-size:12px;width: 770px;height: 350px;top: 900px;left: 245.5px;");
                    $(showUploadandSubmitModal).html(dataShow);
                },
                error: function (req, status, error) {
                    alert("ERROR_UPLOAD_AND_SUBMIT");
                }
            });
            return false;
        },

        RemoveStudentSubmittedFile: function () {
            if (confirm('Are you sure that you want to remove submitted file?')) {
                $(".currentSubmittedFile").hide()
                $(".RetainOriginalSubmittedFile").val('false');
            }
        },      

        ShowUploadAsDialog: function () {
            var options = { modal: true, draggable: false, closeOnEscape: true, width: '560px', height: '400px', resizable: false, autoOpen: false };
            var tag = $("#doc-upload-window").last(); //This tag will the hold the dialog content.
            tag.unblock(); //this will make sure that dialog content is always unblocked while opening.
            tag.find('.field-validation-error').hide();
            tag.find('#uploadFile').val('');
            var args = {
                filterid: '',
                syllabustype: '',
                isReadOnly: false
            };

            tag.dialog({ modal: options.modal, title: options.title, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, resizable: options.resizable, autoOpen: options.autoOpen, close: function () {
                $("#uploadTitle").val("");
            }
            }).dialog('open');

            tag.dialog().parent().css('top', $(window).height() / 2 - 200);
            tag.dialog().parent().css('position', 'fixed');

            if ($('#titleOverride').length > 0) {
                $(".ui-dialog-title").html($('#titleOverride').val());
            }
            else {
                $(".ui-dialog-title").html("Upload Course Document");
            }

            $(".divPopupTitle").hide();
        },

        CloseDialog: function(event) {

            var isDocumentCollection = $("#fne-window .doc-collection-content-view, #nonmodal .doc-collection-content-view").length > 0;

            if (isDocumentCollection) {
                $("#doc-upload-window").dialog("close");
                $('div.ui-dialog:visible').find(".ui-dialog-titlebar-close.ui-corner-all").click();
            } else {

                if (tinyMCE.activeEditor) {
                    try {
                        tinyMCE.activeEditor.remove();
                    }
                    catch (ex) {
                    }
                }
                $("#doc-upload-window").dialog("close");
                PxPage.Loaded('ui-dialog', true)
                $(".ui-icon-closethick").click();
                $("#doc-upload-and-submit").remove();
            }

            if(event!=null) {
                event.stopPropagation();
                event.preventDefault(); 
            }

        },

        OnUploadLinkClick: function (event) {
            if ($('#fne-content #content-item').length > 0) {
                if ($('#fne-unblock-action').length > 0) {
                    $('#fne-unblock-action').click();
                }
            }

            PxUpload.ClearUploadFields();
            $('body').block({
                message: $('#doc-upload-window'),
                css: {
                    padding: 0,
                    margin: 0,
                    width: '35%',
                    top: '40%',
                    left: '35%',
                    textAlign: 'center'
                }
            });
            event.preventDefault ? event.preventDefault() : event.returnValue = false;
        },

        OnDeleteLinkClick: function (event) {
            if (confirm('Delete selected items?')) {
                PxPage.Loading("assignmentViewContent");
                var selectedIds = $.map($("tr.jqgrow[aria-selected='true']"), function (i) { return $(i).attr('id'); });
                var eid = $('#content-item-id').text();
                $.post(PxPage.Routes.assignment_delete_docs, { eid: eid, resourceIdList: selectedIds.join(',') }, function (response) {
                    PxPage.Loaded();
                    $("#gridStudentsSubmissions, #fne_gridStudentsSubmissions").trigger("reloadGrid");
                });
            }
            event.preventDefault ? event.preventDefault() : event.returnValue = false;
        },

        ClearUploadFields: function () {
            $('#uploadDocMessage').hide();
            $('#doc-upload-window span.buttons').show();
            $('#uploadTitle').val('');
            if ($.browser.msie) {
                $("#uploadFile").replaceWith("<input type='file' id='uploadFile' name='uploadFile' />");
            }
            else {
                $('#uploadFile').val('');
                PxUpload.AddWaterMark();
            }
        },

        OnUploadBegin: function (event, uploadFileType, downloadOnlyDocumentTypes) {
            var uploadButton = $('#doc-upload-window .btnUploadDoc');
            var form = $(uploadButton).closest('form').last(); 
            var uploadTitle = form.find('#uploadTitle');
            var uploadDocMessage =  form.find('#uploadDocMessage');
            var uploadFile = form.find('#uploadFile');
            
            if(uploadTitle==null) {
                uploadTitle = $('#uploadTitle'); 
            }
            if(uploadDocMessage==null) {
                uploadDocMessage = $('#uploadDocMessage'); 
            }
            if(uploadFile==null) {
                uploadFile = $('#uploadFile'); 
            }

            if ($.trim(uploadTitle.val()) == '' || $.trim(uploadTitle.val()) == 'Enter the document title') {
                uploadDocMessage.text('Document title cannot be empty.').show();
                uploadTitle.val('').focus();
                event.preventDefault ? event.preventDefault() : event.returnValue = false;
                return;
            }
            uploadDocMessage.hide();
            if (!PxUpload.ValidateTitle(uploadTitle)){
                return false;
            }
            
            if (!PxUpload.ValidateFileControl(uploadFile, uploadFileType == 'Any', downloadOnlyDocumentTypes)) {
                event.preventDefault ? event.preventDefault() : event.returnValue = false;
            }
            else {
                // If cross domain, put the domain to the form and handle response without using easyXDM.
                if (tinyMCE && tinyMCE.relaxedDomain) {
                    form.find('#domain').val(tinyMCE.relaxedDomain);
                    form.prop('target', 'postFrame');
                    $('#postFrame').one('load', function(){
                        var body = $(this).contents().find('body');
                        if(body && body.find('#EasyXDM_response').length > 0){
                            var content = body.find('#EasyXDM_response')[0];
                            var response =  content.innerHTML || content.innerText;
                            parent.PxUpload.OnUploadComplete(response, form.find('#onCompleteScript').val());
                        }

                    });
                } else {
                    form.find('#domain').val('');
                }

                var uploadingFunc = function() {                    
                        //PxPage.Loading();
                        PxPage.Loading("doc-upload-window");
                };
                if($(PxPage.switchboard).data("events")["openactivenode"] != undefined) {
                    $(PxPage.switchboard).trigger("openactivenode", uploadingFunc);
                } else {
                    uploadingFunc();
                }
            }
        },

        //PLATX-9910 - add validation for special characters to match the edit screen
        ValidateTitle: function (titleSelector) {
            var sel = 'input.uploadTitle';
            if (titleSelector)
                sel = titleSelector;


            var title = $(sel).val();
            title = jQuery.trim(title);
            if (title == '' || title == null) {
                PxPage.Toasts.Warning('Title cannot be blank');
                $(sel).focus();
                return false;
            }
            if ($(sel).val().indexOf('<') != -1 ||
                    $(sel).val().indexOf('>') != -1) {
                PxPage.Toasts.Error('Title cannot contain html tags.');
                $(sel).focus();
                return false;
            }

            var validTitleRegexp = /^[A-Za-z0-9 \-.:'/"()]+$/;
            if (!validTitleRegexp.test($(sel).val())) {
                PxPage.Toasts.Error('Title cannot contain special characters.');
                $(sel).focus();
                return false;
            }
            return true;
        },

        OnImageUploadBegin: function (event, uploadFileType) {
            if (!PxUpload.ValidateImageControl($('#uploadFile'), uploadFileType == 'Any')) {
                event.preventDefault ? event.preventDefault() : event.returnValue = false;
            }
            else {
                var uploadButton = $('#doc-upload-window span.buttons');
                var form = $(uploadButton).closest('form');
                var fileButton = form.find('#uploadFile');
                PxPage.Loading();
                $(uploadButton).hide();
                $(fileButton).hide();
                $('#uploadDocMessage').show();
                $('#uploadDocMessage').text('Please wait, uploading...');

                // If cross domain, put the domain to the form and handle response without using easyXDM.
                if (tinyMCE && tinyMCE.relaxedDomain) {
                    form.find('#domain').val(tinyMCE.relaxedDomain);
                        form.prop('target', 'postFrame');
                        $('#postFrame').one('load', function(){
                            var body = $(this).contents().find('body');
                            if(body && body.find('#EasyXDM_response').length > 0){
                                var content = body.find('#EasyXDM_response')[0];
                                var response =  content.innerHTML || content.innerText;
                                PxUpload.OnUploadComplete(response, form.find('#onCompleteScript').val());
                                $(uploadButton).show();
                                $(fileButton).show();
                                $('#uploadDocMessage').hide();
                            }

                        });
                } 


            }
        },

        OnUploadComplete: function (response, callback) {
            var data = null;
            try{
                response = $(response).html();
            }catch(exc) { }
            var data = $.parseJSON(response);
            $('#uploadDocMessage').text('');
            if (data.Status == "error") {
                $('#doc-upload-window').show();
                $('#uploadDocMessage').show();
                $('#uploadDocMessage').text(data.ErrorMessage);
                $('#doc-upload-window span.buttons').show();
                PxPage.Loaded('ui-dialog', true);
                return;
            }

            var isEdit = true;
            var title = '';
            var subTitle = '';
            var description = '';

            // check if request is coming from fne-window
            var contentItem = $("#content-item");
            // check if request is coming from nonmodal popup
            if (!contentItem.is(":visible")) {
                isEdit = false;
                title = $('#nonmodal-content').find('#Content_Title').val();
                subTitle = $('#nonmodal-content').find('#Content_SubTitle').val();
                if (tinyMCE) {
                    if (tinyMCE.activeEditor) {
                        description = tinyMCE.activeEditor.getContent();
                    }
                }

                contentItem = $('#nonmodal-content').find("#content-item");
            }

            if (data.OnSuccessActionUrl && data.OnSuccessActionUrl != '' && contentItem.is(":visible")) {
            var url = data.OnSuccessActionUrl + (data.OnSuccessActionUrl.indexOf('?') == -1 ? '?' : '&') + 'resourceId=' + data.ResourceId + 
                '&isEdit=' + isEdit + '&title=' + title + '&subTitle=' + subTitle + '&description=' + description;

                $.get(url, function (resp) {
                    if ($.isFunction(callback)) {
                        callback(resp);
                        PxPage.Loaded("doc-upload-window");
                        //$(PxPage.switchboard).trigger("contentcreated", [data.ResourceId]);
                        PxPage.Loaded();
                        return;
                    }else{
                        //Try to find the function from string
                        var temp = callback.split('.');
                        if (temp.length > 0) {
                            var func = window[temp[0]];
                            for(var i = 1; i < temp.length; i++) {
                                func = func[temp[i]];
                            }
                            func(resp);
                            PxPage.Loaded("doc-upload-window");
                            PxPage.Loaded();
                        }
                    }
                   
                });
                }
                else
                {
                    if($.isFunction(callback)){
                        callback(response);
                        var uploadButton = $('#doc-upload-window span.buttons');
                        var form = $(uploadButton).closest('form');
                        var fileButton = form.find('#uploadFile');
                        $(uploadButton).show();
                        $(fileButton).show();
                        PxPage.Loaded("doc-upload-window");
                        PxPage.Loaded();
                        $("#doc-upload-window").dialog("close");
                    } else {

                        //Try to find the function from string
                        var temp = callback.split('.');
                        if (temp.length > 0) {
                            var func = window[temp[0]];
                            for(var i = 1; i < temp.length; i++) {
                                func = func[temp[i]];
                            }
                            func(response);
                            PxPage.Loaded("doc-upload-window");
                            $("#doc-upload-window").dialog("close");
                        }
                    }
                }
        },

        ValidateImageControl: function (control, skipFileExtValidation) {
            var filename = $(control).val().toLowerCase();
            var validExtensions = {
                'jpg': 1
            };

            if (filename == '') {
                $('#uploadDocMessage').show();
                $('#uploadDocMessage').text('You must select a file to upload.');
                return false;
            }
            if (skipFileExtValidation) {
                $('#uploadDocMessage').hide();
                return true;
            }
            var ext = filename.split('.').pop();
            if (validExtensions[ext]) {
                $('#uploadDocMessage').hide();
                return true;
            } else {
                $('#uploadDocMessage').show();
                $('#uploadDocMessage').html('Invalid file.<br/>Only documents (.jpg) are allowed.');
                return false;
            }
        },

        DocumentLoaded: function () {
            ContentWidget.ContentLoaded();
            PxPage.Loaded();
            return true;
        },

        ValidateFileControl: function (control, skipFileExtValidation, downloadOnlyDocumentTypes) {
            var filename = $(control).val().toLowerCase();
            var validExtensions = {
                'doc': 1,
                'docx': 2,
                'rtf': 3
            };

            var validExtensionsDownloadOnly = new Array();
            if(downloadOnlyDocumentTypes != undefined) {
                validExtensionsDownloadOnly = downloadOnlyDocumentTypes.split('|');
            }
            

            if (filename == '') {
                $('#uploadDocMessage').show();
                $('#uploadDocMessage').text('You must select a file to upload.');
                return false;
            }
            if (skipFileExtValidation) {
                $('#uploadDocMessage').hide();
                return true;
            }
            var ext = filename.split('.').pop();
            if (validExtensions[ext] || jQuery.inArray(ext,validExtensionsDownloadOnly) > -1) {
                $('#uploadDocMessage').hide();
                return true;
            } 
            else {
                $('#uploadDocMessage').show();
                var validDocuments = '(' + '.doc, .docx, .rtf' ;
                $.each(validExtensionsDownloadOnly, function(index, value) {
                    if(value != "")
                    {
                        validDocuments += ', .' + value;
                    }
                });
                validDocuments += ')';
                $('#uploadDocMessage').html('Invalid file.<br/>Only documents ' + validDocuments + ' are allowed.');
                return false;
            }
        },

        //Adds the watermark to the document title textbox 
        AddWaterMark: function() {
            var uploadTitle = $('#uploadTitle'); //cache the jquery object

            //if the textbox is empty and has no watermark class implement watermark on it
            if (uploadTitle.val() == '') {
                uploadTitle.val('Enter the document title');
                uploadTitle.addClass("watermark");
            }
        },

        //removes the watermark from the document title textbox
        ClearWaterMark: function() {
            var uploadTitle = $('#uploadTitle'); //cache the jquery object
            var hasWaterMark = uploadTitle.hasClass("watermark");
            if(hasWaterMark)
            {
                uploadTitle.val('');
                uploadTitle.removeClass("watermark");
            }
        }
    };
} (jQuery);