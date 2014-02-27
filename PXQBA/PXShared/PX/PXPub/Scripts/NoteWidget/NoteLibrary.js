var PxNoteLibrary = function ($) {

    var _updateActionMenu = function (nodeData) {
        var actions = $('#note_widget_action_menu');
        var noteOptions = [
                            { name: "new", text: "Create New Note" },
                            { name: "deletenote", text: "Delete" }
                        ];


        var options = [];

        options = noteOptions;

        if (actions.length) {
            var menu = {
                id: "noteactions",
                options: options
            };

            var callback = function (event, action) {
                switch (action) {
                    case "new":
                        PxNoteLibrary.DeSelectNotes();
                        PxNoteLibrary.LoadNotes();
                        break;
                    case "deletenote":
                        PxNoteLibrary.DeleteNotes(event);
                        break;

                }
            };
            actions.ActionWidget({ menu: menu, action: callback });
        }
    };

    return {

        DeSelectNotes: function (event) {
            $("#notelist > ul > li").removeClass("selected");
            $("#notelist > ul > li > div").removeClass("visible");
        },

        CloseForm: function (event) {
            var newNote = false;
            var topForNewNote;
            if ($("#highlight-block-0").hasClass("active")) {
                newNote = true;
                topForNewNote = $.trim($('#newNotePosition').val());
                $("#newNotePosition").remove();
                $("#highlight-new-container").children('.active').find('.commentLibraryWrapper').show();
            }
            PxNoteLibrary.CloseNotes(event);
            PxComments.SetHighlightPositions(event);

            if (!newNote) {
                $("#highlight-new-container").hide();
                $("#highlightList").children('.active').find('.commentLibraryWrapper').show();
            }
            else {
                $("#highlight-new-container").attr('style', 'top:' + topForNewNote);
                $("#highlight-block-0").attr('style', 'top:0px');
            }
            var top = newNote ? topForNewNote : $("#highlight-container").find('.active').css("top");
            // $('#document-viewer').scrollTop(parseInt(top, 10));
            $('#fne-content').animate({ scrollTop: top }, 'fast');

            //also place the cursor to the tinymce.
            PxComments.FocusTinyMce();

            event.stopPropagation();
        },

        CloseNotes: function (event) {
            $('#notelibrarywrapper').remove();
            $('.document-viewer-frame-host').show();
            $("#highlightList").children().show();
            $('.highlight-bottom-menu').show();
            $('.linkLibraryWrapper').hide();
        },

        DisplayTopNote: function (event) {
            $("#notelist > ul li:first").toggleClass("selected");
            $("#notelist > ul li:first").children(".editableForm").toggleClass("visible");
        },

        LoadNotes: function (event) {
            //Close all new notes before opening a new note.
            $("#notelist > ul > li.newnode").remove();
            var contents = "<li class='newnode'><div class='formElements'><form class='validationForm' method='post' action=''><label for='noteTitle'>Title:</label>";
            contents += "<input type='text' name='Title' style='width: 100%' />";
            contents += "<label for='noteText'> Note: </label>";
            contents += "<textarea style='width: 100%;' name='Text' id='Text' />"
            contents += "<div class='clear' /> <div class='saveAndSubmit'><input id='btnSubmitNew' type='submit' value='Save' />";
            contents += "<input id='btnCancelNew' name='cancel' class='btnCancelClass' type='button' value='Cancel' />";
            contents += "</div></form><div class='clear' /></div><li>";

            $('#notelist > ul').prepend(contents);
            $("#notelist > ul > li:not(.noteli,.newnode)").remove();
        },

        DeleteNotes: function (event) {
            var select = false;
            $('#notelist > ul > li').each(function (index) {
                if ($(this).hasClass('selected')) {
                    select = true;
                    if (confirm("Are you sure you want to delete the selected Note?")) {
                        $.ajax({
                            url: PxPage.Routes.delete_note_notelibrary,
                            type: "POST",
                            data: {
                                id: $(this).children().children(".formElements").find("input[name='item.NoteId']").val()
                            },
                            success: function (response) {
                                $.get(PxPage.Routes.load_comment_library, null, function (response) {
                                    $(".commentLibraryWrapper > .dlCommentLibrary").replaceWith(response);
                                    $(".commentLibraryWrapper > .dlCommentLibrary").show();
                                    PxComments.FocusTinyMce();
                                });
                                $('#notelist > ul').html(response);
                                PxPage.Update();
                            }
                        });
                    }
                }
            });
            if (!select) {
                PxPage.Toasts.Warning("Please select a note you want to delete.");
            }
        },

        OnNewNoteCreation: function (event) {
            var args = {
                Text: jQuery(this).parent().parent().find("textarea[name='Text']").val(),
                Title: jQuery(this).parent().parent().find("input[name='Title']").val()
            };
            $.post(PxPage.Routes.save_note, args, function (response) {
                $('#notelist > ul').html(response);
                PxPage.Update();
            });
            event.stopPropagation();
        },

        OnNoteUpdation: function (form) {
            $.ajax({
                url: PxPage.Routes.save_note,
                type: "POST",
                data: {
                    Text: $(form).find("textarea[name='Text']").val(),
                    Title: $(form).find("input[name='Title']").val(),
                    NoteId: $(form).parent().find("input[name='item.NoteId']").val(),
                    Sequence: $(form).parent().find("input[name='item.Sequence']").val(),
                    EntityId: $(form).parent().find("input[name='item.EntityId']").val()
                },
                success: function (response) {
                    $.get(PxPage.Routes.load_comment_library, null, function (response) {
                        $(".commentLibraryWrapper > .dlCommentLibrary").replaceWith(response);
                        $(".commentLibraryWrapper > .dlCommentLibrary").show();
                    });
                    $('#notelist > ul').html(response);
                    $('#noteLibrary').unblock();
                    PxPage.Update();
                    PxComments.FocusTinyMce();
                }
            });
        },

        OnNoteValidation: function (event) {
            $(".validationForm").each(function () {
                $(this).validate({
                    submitHandler: function (form) {
                        if (!$(this.submitButton).is(':disabled')) {
                            $('#noteLibrary').block({ message: 'Loading...' });
                            $(this.submitButton).attr('disabled',true);
                            PxNoteLibrary.OnNoteUpdation(form);
                        }

                    },
                    rules: {
                        Title: {
                            required: true,
                            maxlength: 50
                        },
                        Text: {
                            required: true,
                            maxlength: 200
                        }
                    },
                    messages: {
                        Title: {
                            required: "Please enter title of the note",
                            maxlength: "Maximum 50 characters allowed"
                        },
                        Text: {
                            required: "Please enter Text of the note",
                            maxlength: "Maximum 200 characters allowed"
                        }
                    }
                });
            });
            event.stopPropagation();
        },

        ShowFormControls: function (e) {
            e.preventDefault();
            if ($(this).parent().hasClass("selected")) {
                $(this).parent().toggleClass("selected");
            }
            else {
                $("#notelist > ul > li").removeClass("selected");
                $("#notelist > ul > li > div").removeClass("visible");
            }
            $(this).parent().toggleClass("selected");
            $(this).parent().children(".editableForm").toggleClass("visible");

        },

        UseNote: function (e) {
            PxComments.UseLinkOrNote(e, "note", this);
        },

        HideFormControls: function (e) {
            e.preventDefault;
            var originalText = $(this).closest(".formElements").find("input[name='item.Text']").val();
            var originalTitle = $(this).closest(".formElements").find("input[name='shortTitle']").val();
            $(this).closest(".formElements").find("textarea[name='Text']").val(originalText);
            $(this).closest(".formElements").find("input[name='Title']").val(originalTitle);

            //hide validation error message if any.
            $(this).closest(".formElements").find("label[for='Title']").hide();
            $(this).closest(".formElements").find("label[for='Text']").hide();

            $(this).closest(".editableForm").toggleClass("visible");
            PxComments.FocusTinyMce();
            return false;
        },

        BindControls: function () {

            $('#noteLibrary')
            .ajaxStart(function () { PxPage.Loading("fne-content"); })
            .ajaxStop(function () { PxPage.Loaded("fne-content"); });

            $(".noteTitle").die().live('click', PxNoteLibrary.ShowFormControls);

            $(".useText").die().live('click', PxNoteLibrary.UseNote);

            $('#fne-unblock-action').unbind('click', PxNoteLibrary.CloseNotes).bind('click', PxNoteLibrary.CloseNotes);

            $("#btnClose").die().live('click', function (e) {
                e.preventDefault();
                PxNoteLibrary.CloseForm(e);
            });

            $("#btnCancelNew").die().live('click', function (e) {
                $(this).closest('.formElements').closest('li.newnode').remove();
                PxComments.FocusTinyMce();
            });

            $(".btnCancelClass").die().live('click', PxNoteLibrary.HideFormControls);

            $("#btnSubmit").die().live('click', PxNoteLibrary.OnNoteValidation);
            $("#btnSubmitNew").die().live('click', PxNoteLibrary.OnNoteValidation);

        },

        Init: function () {
            var sortOptions = {
                axis: "y",
                cursor: "move",
                revert: true,
                refresh: true,
                scroll: true,
                placeholder: "ui-note-state-highlight",
                containment: "#notelibrarywrapper",
                cursorAt:{top:0,left:0},
                update: function (event, ui) {
                    var args = {
                        minSequence: $(ui.item).prev().find("input[name='item.Sequence']").val() ? $(ui.item).prev().find("input[name='item.Sequence']").val() : "",
                        maxSequence: $(ui.item).next().find("input[name='item.Sequence']").val() ? $(ui.item).next().find("input[name='item.Sequence']").val() : "",
                        id: $(ui.item).find("input[name='item.NoteId']").val(),
                        entityId: $(ui.item).find("input[name='item.EntityId']").val()
                    };

                    $("#notelist > ul").filter('.ui-sortable').sortable('refresh');
                    $.post(PxPage.Routes.reorder_notes, args, function (response) {
                        $('#notelist > ul').html(response);
                        PxPage.Update();
                    });
                }
            };
            $("#notelist > ul").sortable(sortOptions);

            this.BindControls();
            _updateActionMenu('');
            $("#notelist ul li:even").addClass('even');
        }
        //         
    };
} (jQuery);


