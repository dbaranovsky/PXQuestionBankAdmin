var PxRubric = function ($) {
    return {
        Init: function () {
            PxRubric.BindControls();
        },
        BindControls: function () {
            PxPage.FneInitHooks['rubric-editor'] = PxRubric.EditorLoaded;
            PxPage.FneInitHooks['rubric-selector'] = PxRubric.EditorLoaded;
        },
        BindEditorControls: function () {
            $('.submit-rubric').die('click');
            $('.cancel-rubric-edit').die('click');
            $('.submit-rubric').live('click', PxRubric.SubmitRubric);
            $('.cancel-rubric-edit').live('click', PxPage.CloseFne);

            $('.select-rubric').unbind().bind('click', PxPage.CloseFne);
            $('.copy-rubric').unbind().bind('click', PxRubric.CopyRubric);
        },
        EditorLoaded: function () {
            PxRubric.BindEditorControls();
        },
        CopyRubric: function () {
            var objValue = $('.rubric-selector #rubric-id').val();
            var data = {
                url: PxPage.Routes.copy_rubric + "?id=" + objValue,
                title: 'Copy Rubric',
                minimize: false
            };
            PxPage.openContent(data);
        },
        SubmitRubric: function () {
            if (PxPage.ValidateTitle(true)) {
                var rubric =
                        {
                            Title: $('.rubric-title').val(),
                            Description: $('.rubric-desc').val(),
                            IsBeingEdited: $('.rubric-edited').val(),
                            Id: $('.item-id').val(),
                            Rubric: PxRubricBuilder.GetRubricData()
                        };

                if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                    rubric.Id = '';
                }
                $.ajax({
                    url: PxPage.Routes.save_rubric_data,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(rubric),
                    contentType: 'application/json; charset=utf-8',
                    complete: function (response) {
                        var text = rubric.Title;
                        var val = response.responseText;

                        if ($('#fsRubric #selRubric').length) {
                            if (rubric.IsBeingEdited.toLowerCase() == 'false') {
                                $('#fsRubric #selRubric').append(
                                $('<option></option>').val(val).html(text)
                                );

                                $('#fsRubric #selRubric').val(val);
                            }
                        }
                        else {
                            ContentWidget.ShowContentItem(val, "Preview");
                        }
                    }
                });

                PxPage.CloseFne();
            }
        }
    };
} (jQuery);