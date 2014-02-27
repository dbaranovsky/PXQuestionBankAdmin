var PXMceAddLink = function ($) {
    return {
        BindControls: function () {

            PXMceAddLink.Init();

            //to populate the "Link Text" textbox when clicked on the URL in TOC
            $('#addLinkDialog #MLPTOC a').die().live("click", function (e) {
                var lnk = $(this);
                PXMceAddLink.AddLinkSelect(lnk);
                return false;
            });

            //cancel the dialog box
            $('.mceAddLinkClose').live('click', function (event) {
                $('#addLinkDialog').dialog('close');
                return false;
            });

            //add the mce link to the tinyMCE editor
            $('#btnAddLink').die().live('click', function (event) {

                //stop the click event from firing twice
                event.stopImmediatePropagation();

                //Build the link and insert it into the tinyMCE editor
                //validate the title and the URL
                if (PXMceAddLink.ValidateTitle('addLinkDialog #Title') && PXMceAddLink.ValidateURL('addLinkDialog #tocURL')) {

                    var title = $.trim($("#addLinkDialog #Title").val());
                    var itemURL = $("#addLinkDialog #tocURL").val();
                    //var itemURL = PxPage.Routes.eBookBrowser + "?itemId=" + $("#addLinkDialog #tocURL").val();
                    var isSelectedText = $('#addLinkDialog #hdnIsSelectedText').val();
                    var selectedText = $('#addLinkDialog #hdnSelectedText').val();

                    //if the link title is selected in the tinyMCE and not edited in the 'Add Link Dialog'
                    if (isSelectedText == 'true' && selectedText == title) {
                        title = $('#addLinkDialog #hdnSelectedHTML').val();
                    }

                    var link = '<a target="_blank" href="' + itemURL + '">' + title + '</a>';

                    //insert the link in the tinyMCE editor
                    var commandName = PxPage.GetCommandNameForEditor();
                    //When u have more than 1 editors open, you need to check for the right editor.
                    if (tinyMCE.editors.length == 1) {
                        tinyMCE.editors[0].execCommand(commandName, false, link);
                    } else {
                        for (var i = 0; i < tinyMCE.editors.length; i++) {
                            if (tinyMCE.editors[i].selection) {
                                tinyMCE.editors[i].execCommand(commandName, false, link);
                            }
                        }
                    }
                    $('#addLinkDialog').dialog('close');
                }
            });

            //'Choose from course contents' radio button is clicked
            $('.rdoItemContent').live("change", function (e) {
                $('.rdoItemContent').prop('checked', true);
                $('#ContentEditor').show();
                $('#ManualEditor').hide();
                return true;
            });

            //'Create my own link' radio button is clicked
            $('.rdoItemCreateMyOwn').live("change", function (e) {
                $('.rdoItemCreateMyOwn').prop('checked', true);
                $('#ContentEditor').hide();
                $('#ManualEditor').show();
                return true;
            });

            //add the custom link in the tinyMCE editor
            $('#btnAddCustomLink').live("click", function (event) {
                //stop the click event from firing twice
                event.stopImmediatePropagation();

                if (PXMceAddLink.IsValidData('ManualEditor #Title', 'ManualEditor #Url', true)) {
                    var title = $('#ManualEditor #Title').val();
                    var url = $('#ManualEditor #Url').val();

                    //prefix could be ftp://, sftp://, http://, https://
                    var urlRegEx = new RegExp("[a-zA-Z]{3,5}://");

                    if (!urlRegEx.test(url)) {
                        url = 'http://' + url;
                    }

                    if (title == undefined || title == null || title == "") {
                        title = url;
                    }

                    var link = '<a target="_blank" href="' + url + '">' + title + '</a> &nbsp;';

                    //insert the link in the tinyMCE editor
                    var commandName = PxPage.GetCommandNameForEditor();
                    //When u have more than 1 editors open, you need to check for the right editor.
                    if (tinyMCE.editors.length == 1) {
                        tinyMCE.editors[0].execCommand(commandName, false, link);
                    } else {
                        for (var i = 0; i < tinyMCE.editors.length; i++) {
                            if (tinyMCE.editors[i].selection) {
                                tinyMCE.editors[i].execCommand(commandName, false, link);
                            }
                        }
                    }
                    //close the dialog
                    $('#addLinkDialog').dialog('close');
                }
            });

            //content editor link in the tab navigation
            $('.lnkContentEditor').die().live('click', function (e) {

                //prevent the default functionality of the anchor tag click               
                e.preventDefault();
                PxPage.Loading('ContentEditor'); //Load the spinner

                $('#divCourseMaterialsEditor').html('');
                $('#divCourseMaterialsEditor').hide();
                $('#divEbookEditor').html('');
                $('#divEbookEditor').hide();
                //mark the clicked link as selected
                $('.addlinkdialog .divPopupTitle a').removeClass('selectedlink');
                $(this).addClass('selectedlink');

                var contentEditor = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.get_mce_contenteditor,
                    data: {},
                    success: function (response) {
                        $('#divContentEditor').html(response);
                        PXMceAddLink.ApplyFauxTree();
                        $('#divContentEditor').show();
                        //set the title with selected text when dialog refreshes
                        PXMceAddLink.SetSelectedText();
                        //set the selected link active during the tab refresh
                        PXMceAddLink.SetSelectedLink();

                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    },
                    error: function (req, status, error) {
                        alert('ERROR_GET_MCE_CONTENT_EDITOR');
                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    }
                });

            });

            //e-book link in the tab navigation
            $('.lnkEbookEditor').die().live('click', function (e) {

                e.preventDefault();
                PxPage.Loading('ContentEditor'); //Load the spinner
                $('#divContentEditor').html('');
                $('#divContentEditor').hide();
                $('#divCourseMaterialsEditor').html('');
                $('#divCourseMaterialsEditor').hide();

                //mark the clicked link as selected
                $('.addlinkdialog .divPopupTitle a').removeClass('selectedlink');
                $(this).addClass('selectedlink');

                var courseMaterials = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.get_mce_ebookeditor,
                    data: {},
                    success: function (response) {
                        $('#divEbookEditor').html(response);
                        PXMceAddLink.ApplyFauxTree();
                        $('#divEbookEditor').show();
                        //set the title with selected text when dialog refreshes
                        PXMceAddLink.SetSelectedText();
                        //set the selected link active during the tab refresh
                        PXMceAddLink.SetSelectedLink();

                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    },
                    error: function (req, status, error) {
                        alert('ERROR_GET_EBOOK_EDITOR');
                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    }
                });

            });

            //course materials link in the tab navigation
            $('.lnkCourseMaterials').die().live('click', function (e) {

                e.preventDefault();
                PxPage.Loading('ContentEditor'); //Load the spinner

                $('#divContentEditor').html('');
                $('#divContentEditor').hide();
                $('#divEbookEditor').html('');
                $('#divEbookEditor').hide();

                //mark the clicked link as selected
                $('.addlinkdialog .divPopupTitle a').removeClass('selectedlink');
                $(this).addClass('selectedlink');

                var courseMaterials = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.get_mce_coursematerials,
                    data: {},
                    success: function (response) {
                        $('#divCourseMaterialsEditor').html(response);
                        $('#divCourseMaterialsEditor').show();
                        //set the title with selected text when dialog refreshes
                        PXMceAddLink.SetSelectedText();
                        //set the selected link active during the tab refresh
                        PXMceAddLink.SetSelectedLink();

                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    },
                    error: function (req, status, error) {
                        alert('ERROR_GET_EBOOK_EDITOR');
                        PxPage.Loaded('ContentEditor'); //unload the spinner
                    }
                });
            });
        },

        Init: function () {
            $("#divContent").show();
            $("#divEbookEditor").hide();
            $("#divCourseMaterialsEditor").hide();
            $("#ManualEditor").hide();
            PXMceAddLink.ApplyFauxTree();
            //mark the Content tab as selected            
            $('.addlinkdialog .divPopupTitle .lnkContentEditor').addClass('selectedlink');
        },

        AddLinkSelect: function (lnk) {

            var text = $.trim($(lnk).text());

            //mark the selected item 'active'
            $('#addLinkDialog #MLPTOC a').removeClass('active');
            $(lnk).addClass('active');
            $('#addLinkDialog #hdnSelectedId').val($(lnk).attr('id'));
            $('#addLinkDialog #hdnTitle').val(text);

            var tocItemId = $(lnk).siblings('#tocItemId').val();
            var tocItemUrl = $(lnk).siblings('#tocItemUrl').val();

            var titleValue = $.trim($("#addLinkDialog #Title").val());

            if (titleValue == '') {
                $('#addLinkDialog #hdnIsSelectedText').val('false');
            }

            if ($('#addLinkDialog #hdnIsSelectedText').val() != 'true') {
                $("#addLinkDialog #Title").val(text);
            }

            $("#addLinkDialog #tocURL").val(tocItemUrl);

        },

        ApplyFauxTree: function () {
            //implement the faux tree for the toc navigation in the content editor tab
            $('.mcecontenteditor').fauxtree({
                debug: false,
                indent: 20,
                showall: false,
                readOnly: true
            });
            //implement the faux tree for the toc navigation in the ebook editor tab
            $('.mceebookeditor').fauxtree({
                debug: false,
                indent: 20,
                showall: false,
                readOnly: true
            });
        },

        ValidateTitle: function (objTitle) {
            var title = $('#' + objTitle).val();
            title = jQuery.trim(title);
            if (title == '') {
                PxPage.Toasts.Warning('Please enter Link Text');
                $('#' + objTitle).focus();
                return false;
            }
            return true;
        },

        ValidateURL: function (objurl) {
            var url = $('#' + objurl).val();
            url = $.trim(url);
            if (url == '') {
                PxPage.Toasts.Warning('Please select a content item');
                return false;
            }
            return true;
        },

        IsValidData: function (obj1, obj2, isAllowEmpty) {

            var title = $('#' + obj1).val();
            title = jQuery.trim(title);

            var retval = true;
            if (isAllowEmpty == false) {
                if (title == '') {
                    $("#titleError").show();
                    $('#' + obj1).focus();
                    retval = false;
                }
                else {
                    $("#titleError").hide();
                }
            }


            var url = $('#' + obj2).val();
            url = jQuery.trim(url);

            //prefix could be ftp://, sftp://, http://, https://
            var urlRegEx = new RegExp("[a-zA-Z]{3,5}://");

            if (!urlRegEx.test(url)) {
                url = 'http://' + url;
            }

            var regex = /^(https?|ftp|sftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;
            if (url == '' || !regex.test(url)) {
                $("#urlError").show();
                retval = false;
            }
            else {
                $("#urlError").hide();
            }

            return retval;
        },

        //set the title with selected text when dialog refreshes
        SetSelectedText: function () {
            var isSelectedText = $('#addLinkDialog #hdnIsSelectedText').val();

            if (isSelectedText == 'true') {
                var title = $('#addLinkDialog #hdnSelectedText').val();
                $("#addLinkDialog #Title").val(title);
            }
        },

        //set the selected link active during the tab refresh
        SetSelectedLink: function () {
            var selectedItemId = $('#addLinkDialog #hdnSelectedId').val();

            if (selectedItemId != null && selectedItemId != '') {
                var linkSelector = '#addLinkDialog a#' + selectedItemId;
                $(linkSelector).addClass('active');
                var titleText = $('#addLinkDialog #hdnTitle').val();
                $("#addLinkDialog #Title").val(titleText);
            }
        }
    }
} (jQuery);