(function () {

    var DOM = tinymce.DOM, Element = tinymce.dom.Element, Event = tinymce.dom.Event, each = tinymce.each, is = tinymce.is, t = this;

    // Creates a new plugin class and a custom listbox
    // This is PX FILE
    tinymce.create('tinymce.plugins.LaunchpadPlugin', {
        createControl: function (n, cm) {
            switch (n) {
                case 'sizebutton':
                    var c = cm.createSplitButton('sizebutton', {
                        title: 'Font Size',  
						'class': 'mce_styleprops',
						onclick: function () {
						    tinyMCE.activeEditor.execCommand('FontSize', false, 'medium');
                        }
                    });

                    c.onRenderMenu.add(function (cc, m) {
                        m.add({
                            title: 'Small text', onclick: function () {
                                tinyMCE.activeEditor.execCommand('FontSize', false, 'xx-small');
                        }
                        });

                        m.add({
                            title: 'Normal text', onclick: function () {
                                tinyMCE.activeEditor.execCommand('FontSize', false, 'medium');
                        }
                        });

                        m.add({
                            title: 'Large text', onclick: function () {
                                tinyMCE.activeEditor.execCommand('FontSize', false, 'xx-large');
                            }
                        });
                    });

                    // Return the new splitbutton instance
                    return c;

                case 'mymorestylesbutton':
                    var c = cm.createSplitButton('mymorestylesbutton', {
                        title: 'More styles',   
						'class' : 'mce_italic',                    
                        onclick: function () {
                            tinyMCE.activeEditor.selection.setContent('<i>' + tinyMCE.activeEditor.selection.getContent() + '</i>');
                        }
                    });

                    c.onRenderMenu.add(function (c, m) {
                        m.add({
                            title: 'Bold', onclick: function () {
                                tinyMCE.activeEditor.execCommand('Bold', false);
                        }
                        });

                        m.add({ title: 'Italic', onclick: function () {
                            tinyMCE.activeEditor.execCommand('Italic', false);
                        }
                        });

                        m.add({ title: 'Underline', onclick: function () {
                            tinyMCE.activeEditor.execCommand('Underline', false);
                        }
                        });

                        m.add({ title: 'Strikethrough', onclick: function () {
                            tinyMCE.activeEditor.execCommand('Strikethrough', false);
                        }
                        });

                        m.add({ title: 'Superscript', onclick: function () {
                            tinyMCE.activeEditor.execCommand('superscript', false);
                        }
                        });

                        m.add({ title: 'Subscript', onclick: function () {
                            tinyMCE.activeEditor.execCommand('subscript', false);
                        }
                        });
                    });

                    // Return the new splitbutton instance
                    return c;

                case 'filebutton':
                    if (PxPage != undefined) {
                        var c = cm.createMenuButton('filebutton', {
                            title: 'Upload document',
                            'class': 'mce_newdocument',
                            onclick: function () {
                                $.ajax({
                                    url: PxPage.Routes.Upload_TinyMceUploadDocumentForm,
                                    data: {},
                                    type: "GET",
                                    success: function (response) {
                                        if ($('#file_browse').length == 0) {
                                            $("body").append("<div id='file_browse' Title='Select file to upload'>" + response + "</div>");
                                        }

                                        PxUpload.ClearUploadFields();
                                        bindUploadControl();

                                        $("#file_browse").dialog({
                                            height: 200,
                                            width: 500,
                                            modal: true
                                        });
                                    }
                                });
                            }
                        });
                        return c;
                    }
            }

            return null;
        }
    });

    // Register plugin with a short name
    tinymce.PluginManager.add('launchpad', tinymce.plugins.LaunchpadPlugin);

})();

function bindUploadControl() {
    $('#file_browse #submit-upload-file').unbind();
    $('#file_browse #submit-upload-file').click(function () {
        if ($('#file_browse #uploadFile').val() == '') {
            $('#file_browse #uploadDocMessage').show();
            return false;
        } else {
            $('#file_browse #uploadDocMessage').hide();
        }
        $('#file_browse #upload-document-target').unbind('load');
        $('#file_browse #upload-document-target').load(function () {
            var response = $(this).contents().find('body #EasyXDM_response').html();
            OnUploadComplete(response);
        });
    });
    
}

function OnUploadComplete(response) {
    if (PxPage != undefined) {
        if (!response)
            return false;
        var result = JSON.parse(response);
        if (result.Status == 'error') {
            var errorMessage = result.ErrorMessage ? result.ErrorMessage : 'Upload cannot be completed';
            PxPage.Toasts.Error(errorMessage);
            return false;
        }
        $("#file_browse").dialog('close');

        
        var url = PxPage.Routes.document_download + "?id=" + result.ResourceId + "&name=" + result.FileName;
        tinyMCE.activeEditor.execCommand('mceInsertContent', false, ' <a href="' + url + '">' + result.FileName + '</a>', { format: 'bbcode' });
    }
}

