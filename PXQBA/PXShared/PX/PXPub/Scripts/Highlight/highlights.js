//jQuery.noConflict();

var PxComments = function ($) {

    // set up default options
    var defaults = {
        docFrameId: ".proxyFrame",
        docViewerId: "#document-viewer",
        docBodyId: ".document-body",
        fneWindowId: "#fne-content",
        newBlockId: "#highlight-block-0",
        defaultCommentText: "Enter comment here",
        defaultCommentLink: "http://",
        menu: null,
        contentItemWidthAuto: true,
        topNoteSpecificEditorOption: null
    };

    var postInit = null;

    var EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

    var getResponseBody = function (text) {
        var results = "";
        var inBody = false;

        HTMLParser(text, {
            start: function (tag, attrs, unary) {
                if (inBody) {
                    results += "<" + tag;

                    for (var i = 0; i < attrs.length; i++)
                        results += " " + attrs[i].name + '="' + attrs[i].escaped + '"';

                    results += (unary ? "/" : "") + ">";
                }
                if (tag == "body")
                    inBody = true;
            },
            end: function (tag) {
                if (inBody) {
                    results += "</" + tag + ">";
                }
                if (tag == "body")
                    inBody = false;
            },
            chars: function (text) {
                if (inBody) {
                    results += text;
                }
            },
            comment: function (text) {
                if (inBody) {
                    results += "<!--" + text + "-->";
                }
            }
        });

        return results;
    };

    //adds the noteSettings
    var addNoteSettings = function () {
        $('#noteSetting-Container').remove();
        $('.highlight-new-container').hide();
        $.post(PxPage.Routes.note_settings, null, function (response) {
            $('#highlight-container').append(response);
            PxPage.Update();
            $('#fne-content').animate({ scrollTop: 0 }, 'fast');
            $("#searchNote").focus();
        });
        $('#highlightList').hide();

    };

    var renderShowMenu = function (data) {

        var menu = $("#shared-data-menu");
        var notes = [];

        for (var n in data.Notes) {
            var ntext = '<input type="checkbox" value="' + data.Notes[n].UserId + '" class="note-type"';
            if (data.Notes[n].Active)
                ntext += ' checked="true"';

            ntext += "></input>" + data.Notes[n].UserName + ' (' + data.Notes[n].ItemCount + ')';

            notes.push({ name: "note-" + data.Notes[n].UserId, text: ntext });
        }

        //note that there was some code here to treat highlights and notes as their own entries in the menu.
        //this got axed because there was no way to make just a highlight shared in which case removing
        //highlight visibility via this menu made no sense. function was then recoded such that both
        //highlight and note visiblity was controlled by the same menu option. this may change in the
        //future if design team sees the light and decides on a way to make highlights shareable as separate
        //entities

        var callback = function (event) {
            if ($(this).is("li")) {
                var check = $(this).find('input[type="checkbox"]');
                var active = false;


                if (!$(event.target).is('input[type="checkbox"]')) {
                    if (check.is(":checked")) {
                        check.prop("checked", false);
                    }
                    else {
                        check.prop("checked", true);
                        active = true;
                    }
                }
                else {
                    active = check.is(":checked");
                }

                var sharerId = check.val();
                var data = {
                    sharerId: check.val()
                };

                data.highlights = active;
                data.notes = active;

                $.post(PxPage.Routes.update_note_settings, data, function (resposne) {
                    PxComments.Reload();
                });
            }
        };

        menu.ActionWidget({ menu: { id: "shareddata", options: notes }, action: callback });
    };

    //menuData.key is the key of the item being displayed
    //menuData.highlightId is the id of the highlight that is selected, if any
    var highlightMenu = function (menuData) {

        var actions = $(menuData.selector);

        var inFNE = $("#fne-window").is(":visible");

        if (actions.length) {
            var menu;
            $.ajax({
                url: PxPage.Routes.get_hightlight_menu,
                dataType: 'json',
                success: function (data) { menu = data; },
                async: false
            });

            if ($("#highlight-menu-container").length < 1) {
                $("#shared-data-menu").css("display", "none");
            } else {
                $("#shared-data-menu").show();
            }

            if (menu.options.length < 5 && $("#shared-data-menu").length > 0) {
                $("#shared-data-menu").css("display", "none");
            }


            if (menu) defaults.menu = menu;

            PxComments.ShowHighlightCount();

            if (!inFNE) {
                menu.options.splice(2, 0, { name: "add-note", text: "Add note" });
            }

            if (inFNE) {
                menu.options.splice(1, 0, { name: "collapse-all-notes", text: "Collapse All Notes" });
            }

            var callback = function (event, action) {
                switch (action) {
                    case "view-notes":
                        PxComments.OnViewNotes(event);
                        break;

                    case "collapse-all-notes":
                        $('.highlight-block').hide();
                        break;

                    case "add-top-note":
                        PxComments.OnAddTopNote(event);
                        break;
                        
                    case "clear-highlights":
                        PxComments.OnClearHighlights(event);
                        break;

                    case "delete-notes":
                        PxComments.OnClearNotes(event);
                        break;

                    case "note-settings":
                        PxComments.OnShowNoteSettings();
                        break;
                }
            };

            actions.ActionWidget({ menu: menu, action: callback });
        }
    };

    // Overwrite default options 
    // with user provided ones 
    // and merge them into "Settings".
    var Settings = defaults;

    return {

        ///	<summary>
        ///		Align the highlight-comment boxes with the appropriate highlights in the document.
        ///	</summary>
        SetHighlightPositions: function (event) {

            // Need to Add the Top Notes Here and then give the offset value to the next hightlights.
            var offSet = 0;

            // adjust the space between boxes
            var boxSpace = 10;

            // adjust the pointer position
            var pointerOffSet = 10;

            var topNoteSectionHeight = 0;
            
            var pageHighlights = new Array();


            $('.highlight-block').each(function (index) {
                if ($(this).hasClass('page')) {
                    var highlightId = this.id.replace("highlight-block-", "");
                    var linkId = "#highlight-" + highlightId;
                    if ($(Settings.docFrameId).contents().find(linkId).length) {
                    }
                    else {

                        var height = $(this).outerHeight();
                        var topValue = offSet + boxSpace;
                        offSet = topValue + height;

                        topValue = topValue + "px";

                        //show the popup message and hide with fading effect
                        $(this).css({ top: topValue }).show();
                        $(this).show();
                    }
                }
            });

            topNoteSectionHeight = offSet;
            $(Settings.docBodyId).css({ marginTop: topNoteSectionHeight });
            
            try {
                $(Settings.docFrameId).contents().find(".highlight").each(function (index) {
                    var highlightId = "#" + this.id;
                    var highlightBlockId = "#highlight-block-" + highlightId.replace("#highlight-", "");
                    if ($(highlightBlockId).length) {
                        var currentHighlightPosition = getIframeRelativePosition(highlightId, Settings.docViewerId, Settings.docFrameId);

                        //top position is pushed down by the top note diplay section.
                        currentHighlightPosition.top += topNoteSectionHeight;

                        //getting height and width of the message box
                        var height = $(highlightBlockId).outerHeight();

                        if (jQuery.inArray(highlightId, pageHighlights) == -1) {

                            //calculating offset for displaying popup message
                            var topVal = currentHighlightPosition.top - pointerOffSet;

                            if ((offSet > (currentHighlightPosition.top - pointerOffSet))) {
                                // if there is a box collision, place new box directly below
                                topVal = offSet + boxSpace;
                            }

                            offSet = topVal + height;

                            topVal = topVal + "px";

                            //show the popup message and hide with fading effect
                            $(highlightBlockId).css({ top: topVal }).show();

                            if ($(Settings.docFrameId).contents().find(highlightId).is(':visible')) {
                                $(highlightBlockId).show();
                            } else {
                                $(highlightBlockId).hide();
                            }

                            pageHighlights.push(highlightId);
                        }
                    }
                });
            } catch (e) {
                PxPage.log("iframe's content not accessible");
            }
        },

        AdjustHighlightPositions: function (blockId) {
            PxComments.SetHighlightPositions();
        },

        RemoveTinyControlForId: function (id) {
            try {
                if (tinyMCE.activeEditor) {
                    tinyMCE.remove(tinyMCE.activeEditor);
                    //tinyMCE.activeEditor.remove();
                }
            }
            catch (e) {
                //PxPage.log(e);
            }
        },

        LoadHighlightForm: function (strToHighlight, topPos, highlightId) {

            PxComments.ResetView();

            var shortText = (strToHighlight.length > 100) ? strToHighlight.substring(0, 99) + '...' : strToHighlight;

            PxComments.RemoveBindingForTinyControl();

            var htmlToAppend = '<textarea style="width: 100%; height: 100px; font-size: 0.9em; rows="2" name="CommentText"  maxlength="1024" id="CommentText" cols="20" class="commentTextbox zen-editor"></textarea>';
            $("#highlight-new-container").find('.highlight-comment-form').prepend(htmlToAppend);


            PxComments.RemoveTinyControlForId("");

            //don't show the lock option for top note.
            if (topPos == 0 && strToHighlight == "" && highlightId == "") {
                $(Settings.newBlockId).addClass('top-note');
            }
            else {
                $(Settings.newBlockId).removeClass('top-note');
            }
            
            PxComments.SetTinyMceEditorStyle($(Settings.newBlockId));

            //$("#highlight-new-container form textarea[name='CommentText']").val("");
            $("#highlight-new-container form input[name='CommentLink']").val("http://");
            $("#highlight-new-container form input[name='HighlightText']").val("");
            $("#highlight-new-container form input[name='HighlightId']").val(highlightId);
            $('#highlight-new-container').show();
            $('#highlight-block-0').show();

            //$(Settings.newBlockId + ' .highlight-text').html(shortText.toString());
            $(Settings.newBlockId + ' #HighlightText').val(strToHighlight.toString());

            $(Settings.newBlockId).css({ top: "" });
            $('#highlight-new-container').css({ top: (topPos) + "px" }).show();
            $('#fne-content').animate({ scrollTop: topPos }, 'fast');
            $(Settings.newBlockId).addClass("active");

            //Hide the lock option if not an instructor.
            if (PxPage.Context.IsInstructor != 'true') {
                $(Settings.newBlockId).find('span.block-controls > span.unlock').hide();
            }

            //hide the shared-icon for peer-review (PLATX-4337)
            if (bfw_highlightType == 3) {
                $(Settings.newBlockId).find('span.block-controls > span.share').hide();
            }

            if ($('.highlight-block').length) { }
            else {
                PxComments.Resize();
            }
            //move();

        },

        ShowHighlightCount: function () {
            var counter = 0;
            $('#highlightList').find('.highlight-block:not(.blank-notes)').each(function () {
                counter++;
            });

            //var highlightCount = $('#document-body-iframe').contents().find('.highlight').length;

            $(".highlight-count").text(counter);
            var viewNoteText = "View notes" + ' (' + counter + ')';
            if (defaults.menu) {
                defaults.menu.options[0].text = viewNoteText;
            }
            $('#highlightmenuactions .view-notes a').html(viewNoteText);
        },
        CreateHighlight: function (highlightBlockId) {
            /// <summary>
            /// Makes a call to the server to create the highlight and comment (if there is a comment)
            /// </summary>
            /// <param name="highlightBlockId" type="String">id (with # sign) of comment block to create</param>
            
            $.post(PxPage.Routes.create_highlight, $(highlightBlockId + " form").serialize(), function (response) {
                $('#highlightList').append(response);
                PxComments.SetHighlightPositions();
                PxComments.SetNoteCountForHighlight();
                PxComments.SetDeleteAccess();
                $(highlightBlockId + " form").find('input#CommentLink').val('http://');
                $(highlightBlockId).find('#Locked').val('');
                $(highlightBlockId).find('#Shared').val('');
                PxComments.RemoveBindingForTinyControl();
                PxComments.DisplayShowMenu();
                PxComments.ShowNoteDescription();
                PxComments.SetFirstComment();
                PxComments.AdjustHighlightPositions();
            });
        },
        PopulateNoteFormFields: function(highlightBlockId) {
            /// <summary>
            /// Gets properties from comment UI elements and sets them to hidden values in the comment block form
            /// </summary>
            /// <param name="highlightBlockId" type="String">id (with # sign) of comment block to operate on</param>
            /// <returns type="bool">True if the comment form was fully updated. False if there was an error</returns>

            //TODO: Is it safe to assume there will always only be one CommentText mce editor?
            var commentTxt = tinyMCE.get('CommentText').getContent();
            $(highlightBlockId + " form").find('textarea[name=CommentText]').val(commentTxt);

            var commentLnk = $(highlightBlockId + " form").find('input#CommentLink').val();
            if ((commentTxt == '' || commentTxt == 'Enter comment here') && (commentLnk == '' || commentLnk == 'http://')) {
                return false;
            }

            if ($(highlightBlockId + ' form').find('.highlight-public-private').val() == '1') {
                $(highlightBlockId + ' form').find('input[name=Shared]').val('True');
            } else {
                $(highlightBlockId + ' form').find('input[name=Shared]').val('False');
            }
            
            return true;
        },
        GetNoteDataFromBlock: function (highlightBlockId) {
            /// <summary>
            /// Gets relevant note data from a highlight block
            /// </summary>
            /// <param name="highlightBlockId" type="String">Highlight block id (with #) to get the Note data from</param>
            /// <returns type="Object">Object containing necessary data to update/add a note</returns>

            var highlightBlock = $(highlightBlockId);
            var retval = {
                highlightId: '',
                highlightType: '',
                highlightBlockId: highlightBlockId,
                commentListDiv: {},
                noteId: ''
            };
            
            retval.highlightId = highlightBlock.attr("id").replace("highlight-block-", "");
            retval.highlightType = highlightBlock.find('.highlight-comment-form').children('#highlightType').val();
            retval.commentListDiv = $('#commentListDiv-' + retval.highlightId);
            retval.noteId = retval.commentListDiv.children('div.active').attr('id');
            return retval;
        },
        AddNote: function (noteData) {
            /// <summary>
            /// Adds a new note to an existing comment?
            /// </summary>
            /// <param name="noteData" type="Object">Contains fields relevant to saving a new note </param>

            var highlightBlock = $(noteData.highlightBlockId);
            //new comment inside an existing bubble.
            var mceControl = tinyMCE.get('CommentText');
            var commentTxt = mceControl == null ? '' : mceControl.getContent();
            var commentLnk = highlightBlock.find('#form-' + noteData.highlightId).find("#CommentLink").val();

            if ((commentTxt == '' || commentTxt == 'Enter comment here') && (commentLnk == '' || commentLnk == 'http://')) {
                PxComments.RemoveBindingForTinyControl();
                PxComments.ResetView();
                PxComments.AdjustHighlightPositions();
                return;
            }
            if (commentLnk !== undefined && commentLnk != '' && commentLnk != 'http://') {
                commentLnk = "<a href='" + commentLnk + "' target='_blank'> " + commentLnk + "</a>";
                var commandName = PxComments.GetCommandNameForEditor();
                mceControl.execCommand(commandName, false, commentLnk);
            }

            commentTxt = mceControl.getContent();
            highlightBlock.find('#form-' + noteData.highlightId).find("#CommentLink").val('http://');

            highlightBlock.find('#form-' + noteData.highlightId).find("textarea[name=CommentText]").val(commentTxt);
            PxComments.ResetView();
            PxComments.AdjustHighlightPositions();
            highlightBlock.find('#form-' + noteData.highlightId).find("#highlightId").val(noteData.highlightId);

            var url, args;
            if (highlightBlock.hasClass('page')) {
                url = PxPage.Routes.Highlight_AddCommentToTopNote;
                highlightBlock.find('#form-' + noteData.highlightId + ' #highlightId').attr('name', 'topNoteId');
                args = highlightBlock.find('#form-' + noteData.highlightId).serialize();
                
            } else {
                url = PxPage.Routes.add_comment;
                args = highlightBlock.find('#form-' + noteData.highlightId).serialize();
            }
            $.post(url, args, function (response) {
                noteData.commentListDiv.children('.highlight-comment').removeClass('lastComment');
                highlightBlock.find('#form-' + noteData.highlightId).find("#CommentLink").val('http://');
                noteData.commentListDiv.children('.note-description').hide();
                noteData.commentListDiv.append(response);

                //Remove the Binding with TinyMce.                    
                PxComments.RemoveBindingForTinyControl();
            });
        },
        UpdateNote: function (noteData) {
            /// <summary>
            /// Updates a note
            /// </summary>
            /// <param name="noteData" type="Object">Object containing information to update the note</param>

            //editing existing comment inside an existing bubble.
            noteData.noteId = noteData.noteId.replace('note-', '');
            var commentDiv = noteData.commentListDiv.children('div.active');
            var previousText = commentDiv.children('div.note').text();
            var editingMceID = "note-text-" + noteData.noteId;

            var commentLink = noteData.commentListDiv.parent().find("input[name=CommentLink]").val();
            if (commentLink != '' && $.trim(commentLink.toLowerCase()) != 'http://') {
                commentLink = " " + commentLink;
                var commandName = PxComments.GetCommandNameForEditor();
                tinyMCE.get(editingMceID).execCommand(commandName, false, commentLink);
            }
            var noteText = tinyMCE.get(editingMceID).getContent();

            if (previousText == noteText) {
                PxComments.ResetView();
                return;
            }

            $.ajax({
                url: PxPage.Routes.update_note,
                type: "POST",
                data: {
                    noteId: noteData.noteId,
                    noteText: noteText,
                    highlightId: noteData.highlightId,
                    highlightType: noteData.highlightType
                },
                success: function (response) {
                    commentDiv.children('div.note').html(noteText).show();
                    if (noteText == "") {
                        //Hide the highlightblock if the notes are empty.
                        commentDiv.addClass('blank-note');
                        commentDiv.hide();
                        PxComments.HideHighlightBlockIfNoNotes($(noteData.highlightId).parents('.highlight-block'));
                        PxComments.ShowHighlightCount();
                    }
                    commentDiv.removeClass('active');
                    PxComments.RemoveBindingForTinyControl();
                    PxComments.ShowNoteDescription();
                    PxComments.ResetView();
                    PxComments.SetFirstComment();
                }
            });
        },
        SaveNoteBlock: function (highlightBlockId) {
            /// <summary>
            /// Gets note data from the UI and makes a call to the server to add/update the note
            /// </summary>
            /// <param name="highlightBlockid" type="String">ID (with #) of the block to save.</param>

            var noteData = PxComments.GetNoteDataFromBlock(highlightBlockId);
            if (noteData.noteId == undefined || noteData.noteId == '') {
                PxComments.AddNote(noteData);
            } else {
                PxComments.UpdateNote(noteData);
            }
        },
        Reload: function () {
            PxComments.SetHighlightsAndNotes();
        },
        OnAddNote_loadForm: function (highlightText, event, highlightId) {
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }
            postInit = null;
            PxComments.LoadHighlightForm(highlightText, event.pageY, highlightId);
            PxComments.ShowHighlightCount();
        },
        OnAddNote: function (event, highlightId, highlightText) {
            if ($(PxPage.switchboard).data('events')["Highlighter-OnAddNote-override"]) {
                $(PxPage.switchboard).trigger("Highlighter-OnAddNote-override", [highlightId, highlightText, event, PxComments.OnAddNote_loadForm]);
                return;
            }

            if ($("#fne-window").is(":hidden")) {
                postInit = function () {
                    PxComments.OnAddNote_loadForm(highlightText, event, highlightId);
                };

                if ($.browser.msie) {
                    PxPage.openContent({ useLocal: true, onContentLoaded: function () {
                        PxComments.OnAddNote_loadForm(highlightText, event, highlightId);
                    }
                    });
                }
                else {
                    PxPage.openContent({ useLocal: true });
                }
            }
            else {
                PxComments.OnAddNote_loadForm(highlightText, event, highlightId);
            }
        },
        OnReplyNote_loadForm: function (highlightId, event) {
            var block = $("#highlight-block-" + highlightId);
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }
            $(block).removeClass('active');
            postInit = null;
            PxComments.OnHighlightBlockClicked.apply(block, [event]);
        },
        OnReplyNote: function (event, highlightId) {
            if ($(PxPage.switchboard).data('events')["Highlighter-OnReplyNote-override"]) {
                $(PxPage.switchboard).trigger("Highlighter-OnReplyNote-override", [highlightId, event, PxComments.OnReplyNote_loadForm]);
                return;
            }

            if ($("#fne-window").is(":hidden")) {
                postInit = function () {
                    PxComments.OnReplyNote_loadForm(highlightId, event);
                };
                PxPage.openContent({ useLocal: true });
            }
            else {
                PxComments.OnReplyNote_loadForm(highlightId, event);
            }
        },
        OnEditNote_loadForm: function (noteId, event) {
            postInit = null;
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }

            var noteBlock = $("#note-" + noteId);
            noteBlock.trigger('click');
        },
        OnEditNote: function (event, noteId) {
            if ($(PxPage.switchboard).data('events')["Highlighter-OnEditNote-override"]) {
                $(PxPage.switchboard).trigger("Highlighter-OnEditNote-override", [noteId, event, PxComments.OnEditNote_loadForm]);
                return;
            }

            var loadForm = function () {
                PxComments.OnEditNote_loadForm(noteId, event);
            };

            if ($("#fne-window").is(":hidden")) {
                postInit = loadForm;
                PxPage.openContent({ useLocal: true });
            }
            else {
                loadForm();
            }
        },

        DeleteTopNoteBlock : function(highlightBlock) {
            var noteId = highlightBlock.attr('id').replace('highlight-block-', '');
            $.post(PxPage.Routes.delete_note, { noteId: noteId }, function() {
                var blockId = highlightBlock.attr('id').replace('highlight-block-', '');
                highlightBlock.remove();
                PxComments.OnDeleteHighlight(blockId, false);
                PxComments.SetNoteCountForHighlight();
                PxComments.ShowHighlightCount();
                PxComments.AdjustHighlightPositions();
            });
        },
        
        DeleteRegularNoteBlock : function(highlightBlock) {
            var highlightId = highlightBlock.attr('id').replace('highlight-block-', '');
            $.post(PxPage.Routes.delete_notes, { highlightId: highlightId }, function () {
                var blockId = highlightBlock.attr('id').replace('highlight-block-', '');
                highlightBlock.remove();
                PxComments.OnDeleteHighlight(blockId, false);
                PxComments.SetNoteCountForHighlight();
                PxComments.ShowHighlightCount();
                PxComments.AdjustHighlightPositions();
            });
        },

        OnDeleteBlockClicked: function (event) {
            if (!confirm('Are you sure you want to delete this item?')) {
                event.stopPropagation();
                return;
            }
            var highlightBlock = $(this).parents('.highlight-block');

            if ($(highlightBlock).hasClass('page')) { //This is a top note, proceed to delete top note. The route is slightly different. 
                PxComments.DeleteTopNoteBlock(highlightBlock);
            } else { //This is a regular note.
                PxComments.DeleteRegularNoteBlock(highlightBlock);
            }
            event.stopPropagation();
        },

        OnDeleteHighlight: function (highlightId, confirmation) {
            confirmation = (typeof confirmation !== 'undefined') ? confirmation: true;
            var highlightBlock = $("#highlight-block-" + highlightId);

            if (confirmation && $(Settings.docFrameId).contents().find('#highlight-' + highlightId).hasClass('has-notes')) {
                if (!confirm('Deleting this highlight will also delete the attached note, Are you sure you want to delete?')) return;
            }
            $.ajax({
                url: PxPage.Routes.delete_highlight,
                type: "POST",
                data: {
                    highlightId: highlightId
                },
                success: function (response) {
                    highlightBlock.remove();
                    $.each($(Settings.docFrameId).contents().find('.highlight#highlight-' + highlightId), function (index) {
                        // $(this).replaceWith($(this).contents());
                        $(this).removeClass();
                    });
                    PxComments.ShowHighlightCount();
                    PxComments.AdjustHighlightPositions();
                }
            });
        },

        OnAddTopNote: function (event) {
            var loadForm = function () {
                if ($("#noteSetting-Container").is(":visible")) {
                    $("#noteSetting-Container").hide();
                }
                postInit = null;
                PxComments.LoadHighlightForm("", 0, "");
            };

            if ($("#fne-window").is(":hidden")) {
                postInit = loadForm;
                PxPage.openContent({ useLocal: true });
            }
            else {
                loadForm();
            }
        },

        OnShowNoteSettings: function (event, highlightId, highlightText) {
            var loadForm = function () {
                postInit = null;

                addNoteSettings();

            };

            if ($("#fne-window").is(":hidden")) {
                postInit = loadForm;
                PxPage.openContent({ useLocal: true });
            }
            else {
                loadForm();
            }
        },

        OnViewNotes: function (event) {
            if ($("#fne-window").is(":hidden")) {
                PxPage.openContent({ useLocal: true });
            }
            else {
                if ($("#noteSetting-Container").is(":visible")) {
                    $("#noteSetting-Container").hide();
                }
                $("#highlightList, #highlightList .highlight-block").show();
            }
        },

        OnClearHighlights: function (event) {
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }
            if (!confirm('Are you sure you want to clear your highlights?')) return;
            var itemId = $("#highlight-new-container form input[name='ItemId']").val(),
                secondaryId = $("#highlight-new-container form input[name='SecondaryId']").val(),
                reviewId = $("#highlight-new-container form input[name='PeerReviewId']").val(),
                highlightType = $("#highlight-new-container form input[name='HighlightType']").val();
            var cls = 'mine:not(.has-notes)';
            if (PxPage.Context.IsInstructor != 'true') {
                cls += ':not(.locked)';
            }
            $(Settings.docFrameId).contents().find('body').HighlightWidget("clear", cls);

            $.post(PxPage.Routes.clear_highlights, {
                itemId: itemId,
                secondaryId: secondaryId,
                reviewId: reviewId,
                highlightType: highlightType
            });
        },

        OnClearNotes: function (event) {
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }
            if (!confirm('Are you sure you want to clear your notes?')) return;
            var isInstructor = PxPage.Context.IsInstructor == 'true' ? true : false;
            var cls = '';
            if (!isInstructor) {
                cls += ':not(.locked)';
            }

            var itemId = $("#highlight-new-container form input[name='ItemId']").val(),
                secondaryId = $("#highlight-new-container form input[name='SecondaryId']").val(),
                reviewId = $("#highlight-new-container form input[name='PeerReviewId']").val(),
                highlightType = $("#highlight-new-container form input[name='HighlightType']").val();

            $('.highlight-block:not(' + Settings.newBlockId + ')').each(function (i, v) {
                var highlightId = $(this).attr('id').replace('highlight-block-', '');
                var isMine = $(this).hasClass('mine');
                var notes = $(this).find(".highlight-comment:has(input[name='note-user-id'])");
                var highlightElement = $(Settings.docFrameId).contents().find("span[id='highlight-" + highlightId + "']");
                if (isInstructor && isMine) { // Remove bubble and highlight if its own by instructor
                    $(this).remove();
                    highlightElement.each(function () {
                        $(this).removeClass().addClass("removedHL");
                    });

                }
                // if instructor: Remove just instructor's note, if student: remove just student's note if no reply.
                else if (isInstructor || notes.not('.mine').length < 1) {
                    notes.each(function (i, v) {
                        if ($(this).find("input[name='note-user-id']").val() == PxPage.Context.CurrentUserId) {
                            $(this).remove();
                        }
                    });
                    if ($(this).find(".highlight-comment").length == 0) {
                        $(this).remove();
                        highlightElement.each(function () {
                            $(this).removeClass().addClass("removedHL");
                        });
                    }
                }
                $(this).find('.highlight-comment:last').addClass('lastComment');
            });

            PxComments.ResetView();
            PxComments.SetNoteCountForHighlight();
            PxComments.ShowHighlightCount();

            $.post(PxPage.Routes.clear_notes, {
                itemId: itemId,
                secondaryId: secondaryId,
                reviewId: reviewId,
                highlightType: highlightType
            }, function (response) { });
        },

        ResetView: function () {
            ///	<summary>
            ///		Resets the highlight view to initial page load setup.
            ///	</summary>
            

            //remove all temp highlight styles
            $('span.tempHl').replaceWith($('span.tempHl').contents());

            //unblock the comment list
            $('#highlightList').unblock();

            //hide the new highlight form
            //$('#highlight-new-container').hide();

            $('.highlight-block').removeClass("active");
            $("#CommentLink").hide();
            $(".commentLibraryWrapper").hide();
            $(".dlCommentLibrary").hide();
            $(".linkLibraryWrapper").hide();
            $(".listLinkLibrary").hide();
            $('.highlight-block .highlight-comment').removeClass('active activeNavigationItem'); //.children('textarea.commentTextbox').hide();
            $('.highlight-block .highlight-comment').children('div.note').show();
            // $('.highlight-block .highlight-comment').children('span.delete').hide();
            $('#highlight-block-0').find(".highlight-public-private").val('1');

            PxComments.SetNoteCountForHighlight();
            PxComments.AdjustHighlightPositions();
        },

        SetNoteCountForHighlight: function () {
            $(Settings.docFrameId).contents().find('body .highlight').each(function (i, v) {
                var blockId = $(this).attr("id").replace("highlight-", "highlight-block-");
                var block = $("#" + blockId);

                if (block.length > 0 && !block.hasClass('blank-notes')) {
                    //if there is a block of notes, then make sure the highlight has the has-notes class on it
                    $(this).addClass("has-notes");
                }
                else {
                    //if there is no block of comments, make sure the highlight does NOT have the has-notes class on it
                    $(this).removeClass("has-notes");
                }
            });
        },

        SetFirstComment: function () {
            if (PxPage.Context.IsInstructor != 'true')
                return;

            $('.highlight-block').find('.delete:first').text('Delete Note');
            $('.highlight-block').find('.delete:first').unbind();
            $('.highlight-block').find('.delete:first').unbind('click').bind('click', PxComments.OnDeleteBlockClicked);

            $('.highlight-block').find('.lock:first, .unlock:first').css('display', 'block');

        },

        ShowNoteDescription: function (NoteListBox) {
            if (typeof NoteListBox === "undefined")
                NoteListBox = $('.note-list-box');

            NoteListBox.each(function (index, elem) {
                var commentLists = $(this).find('.highlight-comment');
                var firstCommet = commentLists.filter(":first");
                commentLists.not(':first-child').hide();

                var isInstructorLocked = $(this).closest('.highlight-block').hasClass('locked');
                var isReadOnly = PxPage.Context.IsInstructor !== "true" && isInstructorLocked;
                var tempStr = commentLists.length - 1;
                if (commentLists.length == 1)
                    tempStr = isReadOnly? "This discussion has been locked" : "Reply";
                else if (commentLists.length == 2)
                    tempStr += " Response";
                else
                    tempStr += " Responses";

                $(this).find('.note-description').text(tempStr).show();
                
            });
        },

        HideNoteDescription: function (event) {
            var isInstructorLocked = $(event.target).closest('.highlight-block').hasClass('locked');
            var isReadOnly = PxPage.Context.IsInstructor !== "true" && isInstructorLocked;
            if (isReadOnly) {
                $(event.target).closest('.note-list-box').find('.note-description').html('This discussion has been locked.');
            } else {
                $(event.target).closest('.note-list-box').find('.note-description').hide();
            }
            var commentLists = $(event.target).closest('.note-list-box').find('.highlight-comment');
            commentLists.show();
        },

        DisplayShowMenu: function () {

            if ($("#shared-data-menu").length == 0) {
                var showMenu = '<span id="shared-data-menu" class="action_menu">Show</span>';
                if ($('#fne-content .assignment-viewer').length) {
                    $('#assignmentDetails #highlight-links .action-links').append('<li>' + showMenu + '</li>');
                }
                else {
                    $("#fne-title-right-nav").append(showMenu);
                }
            }

            highlightMenu({ selector: "#fne-window #content_widget_highlight_menu" });

            var noteArgs = {
                itemId: $("#highlight-new-container form input[name='ItemId']").val(),
                reviewId: $("#highlight-new-container form input[name='PeerReviewId']").val(),
                enrollmentId: $("#highlight-new-container form input[name='SecondaryId']").val()
            };

            $.getJSON(PxPage.Routes.load_note_settings, noteArgs, function (data) {
                renderShowMenu(data);
            });

        },

        AllowComments: function () {
            var allowComments = $("#AllowComments");
            if (allowComments) {
                if (allowComments.val().toLowerCase() == "true") {
                    return true;
                }
            }
            return false;
        },

        IsExternalContent: function () {
            var isExternalContent = $("#IsExternalContent");
            if (isExternalContent.val()) {
                if (isExternalContent.val().toLowerCase() == "true") {
                    return true;
                }
            }
            return false;
        },

        Resize: function () {
            if ($(Settings.docFrameId).parents('#right').length && $(Settings.docFrameId).parents('#fne-content').length < 1) {
                $(Settings.docBodyId).css({ "height": "auto" }).show();
                $(Settings.docViewerId).css({ "height": "auto" }).show();
                $('#right .content').css({ "height": "auto" }).show();
                $(Settings.docBodyId).css({ "width": "" });
            }
            else if ($(Settings.docFrameId).parents('.assignment-viewer').length && Settings.contentItemWidthAuto) {
                $('#content-item').css({ "width": "auto" });
                var bodyWidth = $('#document-viewer').width() - 350;
                if ($('.highlight-block').length) {
                    bodyWidth = $('#document-viewer').width() - $('.highlight-block').width() - 100;
                }
                $(Settings.docBodyId).css({ "width": bodyWidth }).show();

            }

            else if ($(Settings.docFrameId).parents('#fne-content').length && Settings.contentItemWidthAuto) {
                $('#content-item').css({ "width": "auto" });
                $('.content').css({ "height": "" });

                $(Settings.docBodyId).css({ "width": "99%" }).show(); //fix for 8335
                $(Settings.docFrameId).css({ "width": "100%" }).show();
            }

            // iFrameAutoHeight() jQuery plugin resizes iframe to match its content
            // Offset = 20 compensates for the height of the iframe's horisontal scrollbar (PX-8107)
            //
            // Normally, the iframe's horizontal scrollbar shouldn't be there, but it happens under the following conditions:
            // 1) when the iframe content has fixed width element (coded in iframe css, see use case in PX-8107, checkout ".bodyHolder1" in bookstyles.css )  
            // 2) and only on the initial load
            // Note that iframe's horizontal scrollbar is "gone" after highlighting event
            // Don't get too excited because highlighting patches the problem by setting "overflow:hidden" to the iframe's body,
            // which potentially may result in loss of content.
            // Bottom line - resizing functionality needs to be revisited.

            $(Settings.docFrameId).iframeAutoHeight({ heightOffset: 20 });

            if ($(Settings.bodyElement).length) {
                var outerIframe = $(Settings.bodyElement).find('iframe');
                if (outerIframe.length) {
					// offset height changed to 0 since it causing the content area to grow
                    outerIframe.iframeAutoHeight({ heightOffset: 0 });
                }
            }
            
            $(PxPage.switchboard).trigger("iframe-resized", Settings.docFrameId);
            
            PxComments.SetHighlightMenuPosition();
            PxComments.SetHighlightPositions();


        },

        SetHighlightMenuPosition: function () {
            if ($('#fne-window').is(':visible') && $('#fne-content #document-viewer #highlight-container').length) {
                if ($('#fne-content .advanced-search').length) {
                    return;
                }

                var fneContentWidth = $('#fne-content').outerWidth(true);
                var fneRightWidth = fneContentWidth - $('#document-body').outerWidth(true);
                if ($('#fne-content .assignment-viewer').length) {
                    if ($('#fne-content .eportfolio-grade-viewer').length) return;
                    $('.assignmentLeftView').width(fneContentWidth - fneRightWidth - 26);
                    $('#assignmentDetails').width(fneRightWidth);
                }
                else if ($('body').hasClass('product-type-lms-faceplate') || $('body').hasClass('product-type-faceplate')) {
                    $('#fne-window #fne-title-left').css({ "width": "70%" });
                    $('#fne-window #fne-title-right').css({ "width": "30%" }).show();
                }
                else {
                    //$('#fne-window #fne-title-right').width(fneRightWidth).show();
                    //$('#fne-window #fne-title-left').width(fneContentWidth - fneRightWidth - 5);
                    $('#fne-window #fne-title-left').css({ "width": "80%" });
                    $('#fne-window #fne-title-right').css({ "width": "20%" }).show();
                }

            }
        },

        ResetFne: function () {
            if ($(Settings.docFrameId).parents('.assignment-viewer').length) {

                var data = {
                    itemId: bfw_itemId,
                    secondaryId: bfw_secondaryId,
                    reviewId: bfw_reviewId,
                    highlightType: bfw_highlightType,
                    highlightDesc: bfw_highlightDesc,
                    url: bfw_itemUrl,
                    commenterId: bfw_commenterId,
                    isCurrentUserContext: bfw_isCurrentUserView
                };

                $('#bottomView').load(PxPage.Routes.document_viewer + '/' + bfw_itemId, data, function () { });
            }
            else {
                ContentWidget.RefreshContentItem(bfw_itemId, 4, 'fne-content', function () { });
            }
            PxComments.Resize();
        },


        OnHighlightMenuClicked: function (event) {
            var url = ((this.href !== undefined) ? this : event).href;
            event.preventDefault();

            var $this = $(this);

            if ($this.hasClass('feedback')) {
                PxComments.LoadHighlightForm('');
            }
            else if ($this.hasClass('mine')) {
                $('.highlight-block').hide();
                $('.highlight-block.mine').show();
            }
            else if ($this.hasClass('open')) {
                $('.highlight-block').show();
            }
            else if ($this.hasClass('collapse')) {
                $('.highlight-block').hide();
            }
        },

        OnNoteDescriptionClicked: function (event) {
            PxComments.HideNoteDescription(event);
            var thisObj = $(this).closest('.highlight-block').get(0);
            PxComments.OnHighlightBlockClicked.call(thisObj, event);
        },

        SetTinyMceEditorStyle: function (block) {
            if ($(block).hasClass('page') || $(block).hasClass('top-note')) { //This is Top Note specific tinyMCE editor
                tinyMCE.init(Settings.topNoteSpecificEditorOption);
            } else {    //This is for regular note editor
                tinyMCE.init(PxPage.editableNote_editor_options);
            }
        },

        OnHighlightBlockClicked: function (event) {
            var highlightId = $(this).attr("id");
            highlightId = "#" + highlightId;
            var highlightNum = highlightId.replace("#highlight-block-", "");
            if ($(this).hasClass('active') && !$(this).find('div.highlight-comment').hasClass('active')) { }
            else {
                var isReadOnly = false;
                if (PxPage.Context.IsInstructor == 'true') {
                    isReadOnly = false;
                }
                else {
                    isReadOnly = $(Settings.docViewerId).hasClass('readonly') ||
                                 $(this).hasClass('locked');
                }

                if (!isReadOnly) {
                    if (event.target != '[object HTMLSelectElement]') {
                        if (event.target != '[object HTMLOptionElement]' && (event.target.name != 'CommentLink')) {
                            PxComments.SaveCurrentNote(event);
                            $(this).addClass('active');

                            //Add the textarea CommentText first and then bind.                        
                            PxComments.RemoveBindingForTinyControl();
                            $(this).find('.highlight-note-header').remove();
                            var noteHeaderText = $('#highlight-block-0').find('.highlight-note-header').clone().wrap('<p>').parent().html();
                            var htmlToAppend = noteHeaderText + '<textarea style="width: 100%; height: 100px; font-size: 0.9em; rows="2" name="CommentText" id="CommentText"  maxlength="1024" cols="20" class="commentTextbox zen-editor"></textarea>';
                            $(this).find('.highlight-comment-form').prepend(htmlToAppend);
                            //PxPage.Update();



                            PxComments.RemoveTinyControlForId("");
                            PxComments.SetTinyMceEditorStyle(this);

                            tinyMCE.onAddEditor.add(function (mgr, ed) {
                                PxComments.AdjustHighlightPositions(highlightNum);
                            });
                            PxComments.AdjustHighlightPositions(highlightNum);
                        }
                    }
                }
            }
            if (event)
                event.stopPropagation();
        },

        OnEditNoteClicked: function (event) {
            var thisObj = $(this).closest('.highlight-comment').get(0);
            PxComments.OnNoteClicked.call(thisObj, event);
        },

        OnNoteClicked: function (event) {
            //Should perfom no action if event.target is a tinyMCE button.
            if ($(event.target).hasClass('mceIcon')) {
                event.stopPropagation();
                return;
            }
            
            PxComments.HideNoteDescription(event);
            PxComments.SaveCurrentNote(event);
            var 
                isReadOnly = false,
                isInstructor = PxPage.Context.IsInstructor == 'true' ? true : false;

            if (!isInstructor) { 
               
                isReadOnly = $(Settings.docViewerId).hasClass('readonly') ||
                            $(this).hasClass('locked') ||
                            !$(this).hasClass('mine');
            }
            if (isReadOnly) return;

            var noteText = $(this).children('div.note').html();
            var editableNoteId = $(this).attr('id').replace('note-', 'note-text-');
            var highlightBlockId = '#' + $(this).parents('.highlight-block').attr('id');
            var highlightId = highlightBlockId.replace('#highlight-block-', '');
            var notes = $(highlightBlockId).find('.highlight-comment');
            $(highlightBlockId).addClass("active"); //.find("#CommentText").hide();

            $(this).children('div.note').hide();
            $(this).closest('.highlight-block').find('.highlight-note-header').remove();

            $('#highlight-container').find(".mceEditor").remove();
            if ($(this).children('textarea.commentTextbox').length) {
                $(this).addClass('active activeNavigationItem').children('textarea.commentTextbox').remove();
                PxComments.RemoveTinyControlForId(editableNoteId);
            }
            PxComments.RemoveBindingForTinyControl();

            $(this).addClass('active activeNavigationItem').append('<textarea class="commentTextbox zen-editor"  maxlength="1024" name="CommentText" id="' + editableNoteId + '" style = "width:100%; height:100px; font-size:0.9em;" >' + $.trim(noteText) + '</textarea>');

            PxComments.RemoveTinyControlForId("");
            PxComments.SetTinyMceEditorStyle($(highlightBlockId));

            var commandName = PxComments.GetCommandNameForEditor();
            try {
                tinyMCE.get(editableNoteId).execCommand(commandName, false, noteText);
            }
            catch (e) {
                PxPage.log(e);
            }

            if (!$(this).hasClass("mine") && isInstructor) {
                // Enable just delete to instructor if the note is owned by student.
                $(this).children('textarea.commentTextbox').attr("disabled", "disabled");
                tinyMCE.execCommand("contentReadOnly", true, editableNoteId);
            }
            //Allow delete only to instructor OR if no reply for first note added by owner of highlight.
            // if (isInstructor || $(this).index() > 0 || ($(this).index() == 0 && notes.not('.mine').length < 1)) {
            //    $(this).children('span.delete').show();
            //}
            PxComments.AdjustHighlightPositions(highlightId);


            event.stopPropagation();
        },

        OnNoteBlur: function (event) {
            /// <summary>
            /// No idea what the point of this function is.  Please comment if you do.
            /// </summary>
            /// <param name="event" type="Object">mouse click event handler object.</param>

            //firefox and chrome somehow pass in different event.target. 
            if ($(event.target).hasClass('ignore-autosave') || $(event.target).hasClass('highlight-public-private') ||
                $(event.target).parent().hasClass('highlight-public-private') ||
                $(event.target).parents('.ignore-autosave').length || $(event.target).parents().hasClass('mceButton')) {
                return;
            }
            else {
                PxComments.SaveCurrentNote(event);
            }

            PxComments.ShowNoteDescription();
            PxComments.Resize();
        },

        OnCommentLinkClicked: function (event) {
            var highlightId = $(this).parents('div.highlight-block').first().attr("id");
            var highlightNum = highlightId.replace("highlight-block-", "");
            highlightId = "#" + highlightId;
            $(highlightId).find(".linkLibraryWrapper").show();
            $(highlightId).find(".listLinkLibrary").show().focus();
            $(highlightId).find("#CommentLink").show().focus();
            $(highlightId).find("#CommentText").addClass("outline");
            $(highlightId).find("#CommentText").addClass("watermark");
            $(highlightId).find(".commentLibraryWrapper").hide();
            PxComments.AdjustHighlightPositions(highlightNum);
            event.stopPropagation();
        },

        GetEditingMceId: function () {
            var currentHighlightBlock = $('.highlight-block.active');
            var editingMceID;
            if (currentHighlightBlock.attr('id') == 'highlight-block-0') {
                editingMceID = 'CommentText';
            }
            else {
                var highlightId = currentHighlightBlock.attr("id").replace("highlight-block-", "");
                var commentListDiv = $('#commentListDiv-' + highlightId);
                var noteId = commentListDiv.children('div.active').attr('id');
                noteId = (noteId == undefined) ? '' : noteId.replace('note-', '');

                if (noteId == undefined || noteId == '') {
                    editingMceID = 'CommentText';
                }
                else {

                    editingMceID = "note-text-" + noteId;
                }
            }
            return editingMceID;

        },

        OnCommentLibraryClicked: function (event) {
            var highlightId = $(this).parents('div.highlight-block').first().attr("id");
            var highlightNum = highlightId.replace("highlight-block-", "");
            highlightId = "#" + highlightId;
            $(highlightId).find(".commentLibraryWrapper").show();
            $(highlightId).find(".dlCommentLibrary").show().focus();
            $(highlightId).find("#CommentText").addClass("outline");
            $(highlightId).find(".dlCommentLibrary").val('');
            $(highlightId).find(".linkLibraryWrapper").hide();
            PxComments.AdjustHighlightPositions(highlightNum);
            event.stopPropagation();
        },

        OnCancelClicked: function (event) {
            event.stopPropagation();
            $(this).parents('div.highlight-block').hide();
            PxComments.RemoveBindingForTinyControl();
            PxComments.ShowNoteDescription();
            PxComments.ResetView();
        },

        OnCommentLibraryChanged: function (event) {
            var highlightId = $(this).parents('div.highlight-block').first().attr("id");
            highlightId = "#" + highlightId;
            var text = $(highlightId).find(".dlCommentLibrary option:selected").val();
            if (text == "" || text == $(highlightId).find(".dlCommentLibrary > option:first").val()) {
                return false;
            }
            var editableNoteId = PxComments.GetEditingMceId();
            var prevText = tinyMCE.get(editableNoteId).getContent();


            if ($.trim(prevText) != "Enter comment here" || $.trim(prevText) != "") {
                text = " " + text;
            }
            if (text != '') {

                var commandName = PxComments.GetCommandNameForEditor();
                tinyMCE.get(editableNoteId).execCommand(commandName, false, text);

                $(highlightId).find(".commentLibraryWrapper").hide();
                $(highlightId).find(".dlCommentLibrary").hide();
            }
            event.stopPropagation();
        },

        OnLinkLibraryChanged: function (event) {
            var highlightId = $(this).parents('div.highlight-block').first().attr("id");
            highlightId = "#" + highlightId;
            var selectedText = $(highlightId).find(".listLinkLibrary option:selected").text();
            if (selectedText == $(highlightId).find(".listLinkLibrary > option:first").text()) {
                return false;
            }

            var text = "<a href='" + $(highlightId).find(".listLinkLibrary option:selected").val() + "' target='_blank'>" + selectedText + "</a>";
            var editableNoteId = PxComments.GetEditingMceId();
            var prevText = tinyMCE.get(editableNoteId).getContent();
            if ($.trim(prevText) != "Enter comment here" || $.trim(prevText) != "") {
                text = " " + text;
            }
            if (text != '') {
                var commandName = PxComments.GetCommandNameForEditor();
                tinyMCE.get(editableNoteId).execCommand(commandName, false, text);

                $(highlightId).find(".linkLibraryWrapper").hide();
                $(highlightId).find(".listLinkLibrary").hide();
            }
            event.stopPropagation();
        },

        OnRubricListChanged: function (event) {
            var highlightId = $(this).parents('div.highlight-block').first().attr("id");
            highlightId = "#" + highlightId;
            var selectedText = $(highlightId).find(".dlRubricsList option:selected").text();
            if (selectedText == $(highlightId).find(".dlRubricsList > option:first").text()) {
                return false;
            }

            var text = "\"Applied to Rubric: " + $(highlightId).find(".dlRubricsList option:selected").val() + "\"";
            var editableNoteId = PxComments.GetEditingMceId();
            var prevText = tinyMCE.get(editableNoteId).getContent();
            if ($.trim(prevText) != "Enter comment here" || $.trim(prevText) != "") {
                text = " " + text;
            }
            if (text != '') {
                var commandName = PxComments.GetCommandNameForEditor();
                tinyMCE.get(editableNoteId).execCommand(commandName, false, text);
            }
            event.stopPropagation();
        },

        OnCommentSubmitted: function (id) {
            ///	<summary>
            ///		After a comment is submitted, reset the textbox and put focus on it
            ///	</summary>
            var blockId = '#highlight-block-' + id;
            $(blockId + ' input[type=text]').val('');
            $(blockId + ' #CommentText').val('').focus();

            $(blockId + ' #CommentText').removeAttr("disabled");
            $(blockId + ' input[type=submit]').removeAttr("disabled");

            PxComments.AdjustHighlightPositions(id);
        },

        OnCommentSubmittedBegin: function (id) {
            ///	<summary>
            ///		After a comment is submitted, reset the textbox and put focus on it
            ///	</summary>
            var blockId = '#highlight-block-' + id;

            $(blockId + ' #CommentText').attr("disabled", "disabled");
            $(blockId + ' input[type=submit]').attr("disabled", "disabled");

            PxComments.AdjustHighlightPositions(id);
        },


        OnCloseHighlightClicked: function (event) {
            event.stopPropagation();
            $(this).parents('div.highlight-block').hide();
            PxComments.ResetView();
        },

        OnDeleteNoteClicked: function (event, curObject) {
            if (!confirm('Are you sure you want to delete this note?')) {
                event.stopPropagation();
                return false;
            }
            var noteBlock = $(event.target).parents('div.highlight-comment');
            var noteId = noteBlock.attr("id");

            //fix for top note not deleting.
            if (noteId == null) {
                noteBlock = $(this).parent('div.highlight-comment');
                noteId = noteBlock.attr("id");
            }

            noteId = noteId.replace('note-', '');
            $.ajax({
                url: PxPage.Routes.delete_note,
                type: "POST",
                data: {
                    noteId: noteId
                },
                success: function (response) {
                    var commentListBox = noteBlock.parent('.note-list-box');
                    noteBlock.remove();
                    if (commentListBox.children('div.highlight-comment').length) {
                        commentListBox.children('div.highlight-comment:last').addClass('lastComment');
                        PxComments.ResetView();
                    }
                    else {
                        commentListBox.parents('.highlight-block').remove();
                        PxComments.SetNoteCountForHighlight();
                    }
                }
            });
            event.stopPropagation();
            //return false;
        },

        OnToggleShareNoteClicked: function (event) {
            var highlightBlockId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            var notes = $(highlightBlockId + " .highlight-comment");
            var noteId = notes.first().attr('id').replace('note-', '');
            if (($(highlightBlockId).hasClass('locked') && PxPage.Context.IsInstructor != 'true') || // If locked and not instructor 
                !$(highlightBlockId).hasClass('mine') || // If this highlight is not current user's
                ($(this).hasClass('shared') && notes.not('.mine').length)) // If highlight block is shared and it has reply from other users
            {
                return;
            }

            var shareNotes = $(this).hasClass('share');
            $.ajax({
                url: PxPage.Routes.share_single_note,
                type: "POST",
                data: {
                    noteId: noteId,
                    isShared: shareNotes
                },
                success: function (response) {
                    if (shareNotes) {
                        $(highlightBlockId + ' span.share').removeClass('share').addClass('shared');
                        $(highlightBlockId + ' span.shared').attr('title', 'Note is Shared');
                    }
                    else {
                        $(highlightBlockId + ' span.shared').removeClass('shared').addClass('share');
                        $(highlightBlockId + ' span.share').attr('title', 'Share Note');
                    }
                }
            });
            event.stopPropagation();
        },

        OnToggleShareClicked2: function (event) {
            var highlightBlockId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            var highlightId = highlightBlockId.replace("#highlight-block-", "");
            var highlightBlockForm = highlightBlockId + " form";
            $(highlightBlockForm).find("input[id='Shared']").val($(this).val());
            var notes = $(highlightBlockId + " .highlight-comment");
            if (($(highlightBlockId).hasClass('locked') && PxPage.Context.IsInstructor != 'true') || // If locked and not instructor 
                !$(highlightBlockId).hasClass('mine') || // If this highlight is not current user's
                ($(this).hasClass('shared') && notes.not('.mine').length)) // If highlight block is shared and it has reply from other users
            {
                return;
            }

            //If highlight has class "page", this is a top note
            var url, args;
            var shareNotes = ($(this).val() == 1) ? "True" : "False";
            
            if ($(highlightBlockId).hasClass('page')) {
                url = PxPage.Routes.share_single_note;
                args = {
                    noteId: highlightId,
                    isShared: shareNotes
                };

            } else {
                url = PxPage.Routes.share_highlight;
                args = {
                    highlightId: highlightId,
                    isShared: shareNotes
                };
            }

            $.ajax({
                url: url,
                type: "POST",
                data: args,
                success: function (response) {
                    if (shareNotes) {
                        $(highlightBlockId + ' span.share').removeClass('share').addClass('shared');
                        $(highlightBlockId + ' span.shared').attr('title', 'Note is Shared');
                    }
                    else {
                        $(highlightBlockId + ' span.shared').removeClass('shared').addClass('share');
                        $(highlightBlockId + ' span.share').attr('title', 'Share Note');
                    }
                }
            });
            event.stopPropagation();
        },


        OnToggleShareClicked: function (event) {
            var highlightBlockId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            var highlightId = highlightBlockId.replace("#highlight-block-", "");
            var notes = $(highlightBlockId + " .highlight-comment");
            if (($(highlightBlockId).hasClass('locked') && PxPage.Context.IsInstructor != 'true') || // If locked and not instructor 
                !$(highlightBlockId).hasClass('mine') || // If this highlight is not current user's
                ($(this).hasClass('shared') && notes.not('.mine').length)) // If highlight block is shared and it has reply from other users
            {
                return;
            }

            var shareNotes = $(this).hasClass('share');
            $.ajax({
                url: PxPage.Routes.share_highlight,
                type: "POST",
                data: {
                    highlightId: highlightId,
                    isShared: shareNotes
                },
                success: function (response) {
                    if (shareNotes) {
                        $(highlightBlockId + ' span.share').removeClass('share').addClass('shared');
                        $(highlightBlockId + ' span.shared').attr('title', 'Note is Shared');
                    }
                    else {
                        $(highlightBlockId + ' span.shared').removeClass('shared').addClass('share');
                        $(highlightBlockId + ' span.share').attr('title', 'Share Note');
                    }
                }
            });
            event.stopPropagation();
        },

        OnNewBlockControlsClicked: function (event) {
            if ($(this).hasClass('delete')) {
                PxComments.OnCloseHighlightClicked.apply($(this), [event]);
                return;
            }
            $(Settings.newBlockId).find('#Locked').val($(this).hasClass('unlock'));
            $(Settings.newBlockId).find('#Shared').val($(this).hasClass('share'));
            PxComments.SaveNewComment();
        },

        OnHighlightClicked: function (e) {
            if ($("#noteSetting-Container").is(":visible")) {
                $("#noteSetting-Container").hide();
            }
            var elem = e.target || e.srcElement;
            var highlightId = elem.id;


            var pointerOffSet = 15;

            highlightNum = highlightId.replace("highlight-", "");
            highlightId = "#" + highlightId;

            var highlightBlockId = "#highlight-block-" + highlightNum;

            //getting height and width of the message box
            var height = $(highlightBlockId).height();
            var width = $(highlightBlockId).width();

            $(highlightBlockId).show();

            $('#highlightList').css({ bottom: "0px" }).show(); //.fadeOut(1500);

            var currentBlockPosition = getRelativePosition(highlightBlockId, Settings.docBodyId, Settings.docFrameId);

            //calculating offset for displaying popup message
            if (currentBlockPosition) {
                topVal = (currentBlockPosition.top - e.pageY + pointerOffSet) + "px";

                //show the popup message and hide with fading effect
                $('#highlightList').show().css({ bottom: topVal }); //.fadeOut(1500);
            }
            var isReadOnly = ($(Settings.docViewerId).hasClass('readonly'));
            if (!isReadOnly) {
                $('.highlight-block').removeClass("active");
                var highlightBlock = $("#highlight-block-" + highlightNum);
                if (highlightBlock != null) {
                    //$(highlightBlock).addClass('active');
                }
                PxComments.AdjustHighlightPositions(highlightNum);
            }
        },

        OnCommentFocus: function (event) {
            //alert('focus in:' + this.value + ' c:' + Settings.defaultCommentText);
            var highlightId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            //Check if user and enter into the textbox 
            //and remove the class we have assigned on focus event of the text Box
            if (this.value == Settings.defaultCommentText) {
                this.value = "";
                $(highlightId).find("#CommentText").removeClass("watermark"); //remove class when user focus on the textbox
            }
        },

        OnCommentBlur: function (event) {
            var highlightId = "#" + $(this).parents('div.highlight-block').first().attr("id");

            if ((this.value == "") || (this.value == Settings.defaultCommentText)) {
                this.value = Settings.defaultCommentText;
                $(highlightId).find("#CommentText").addClass("watermark"); // Add class to the textbox
            }
        },

        OnCommentLinkFocus: function (event) {
            var highlightId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            // If user did not enter any value in the text box 
            // then assign back the watermark value and assign the class
            if (this.value == "") { this.value = Settings.defaultCommentLink; }
            $(highlightId).find("#CommentLink").removeClass("watermark"); //remove class when user focus on the textbox
            return false;
        },

        OnCommentLinkBlur: function (event) {

            var highlightId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            if ((this.value == "") || (this.value == Settings.defaultCommentLink)) {
                this.value = Settings.defaultCommentLink;
                $(highlightId).find("#CommentLink").addClass("watermark"); // Add class to the textbox
            }
            var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;

            if (!RegExp.test(this.value)) {
                $("#spnLinkError").show();
                return true;
            }


            return false;
        },

        SaveNewComment: function () {
            $('#highlight-new-container').hide().children('.highlight-block').removeClass('active');
            //If the form can gets populated ok, go ahead and create the highlight
            if (PxComments.PopulateNoteFormFields('#highlight-block-0')) {
                PxComments.CreateHighlight('#highlight-block-0');
            }
        },

        OnCommentKeyPress: function (event) {
            if (event.which == 13) {
                PxComments.SaveCurrentNote(event);
                event.preventDefault();
            }
        },

        OnLinkSubmit: function (event) {
            //just append the text to the tinyMce.
            //PxComments.SaveCurrentNote(event);
            var currentHighlightBlock = $('.highlight-block.active');
            $("#spnLinkError").hide();
            if (currentHighlightBlock.length < 1) return false;
            var commentLnk = $(this).prev('#CommentLink').val();
            if (commentLnk != '' && commentLnk != 'http://') {

                var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;

                if (!RegExp.test(commentLnk)) {
                    $("#spnLinkError").show();
                    return false;
                }

                commentLnk = "<a href='" + commentLnk + "' target='_blank'> " + commentLnk + "</a>";
                var editingMceID;
                if (currentHighlightBlock.attr('id') == 'highlight-block-0') {
                    editingMceID = "CommentText";
                }
                else {
                    var highlightId = currentHighlightBlock.attr("id").replace("highlight-block-", "");
                    var highlightType = currentHighlightBlock.find('.highlight-comment-form').children('#highlightType').val();
                    var commentListDiv = $('#commentListDiv-' + highlightId);
                    var noteId = commentListDiv.children('div.active').attr('id');

                    if (noteId == undefined || noteId == '') {
                        editingMceID = "CommentText";
                    }
                    else {
                        noteId = noteId.replace('note-', '');
                        editingMceID = "note-text-" + noteId;
                    }
                }
                var commandName = PxComments.GetCommandNameForEditor();
                tinyMCE.get(editingMceID).execCommand(commandName, false, commentLnk);
                $(this).prev('#CommentLink').val('http://');
            }
            return false;
        },

        MoveTinyMCECursorToEnd: function (editor_id) {
            var inst = tinyMCE.activeEditor;
            //inst = tinyMCE.getInstanceById(editor_id);
            tinyMCE.execInstanceCommand(editor_id, "selectall", false, null);
            if (tinyMCE.isMSIE) {
                rng = inst.getRng();
                rng.collapse(false);
                rng.select();
            }
            else {
                sel = inst.getSel();
                sel.collapseToEnd();
            }
        },

        FocusTinyMce: function () {
            try {
                tinyMCE.activeEditor.focus();
            }
            catch (e) { }
        },


        RemoveBindingForTinyControl: function () {
            try {
                if ($("#highlight-container").find('textarea[name=CommentText]').length) {
                    
                    PxComments.RemoveTinyControlForId("CommentText");
                    $("#highlight-container").find('textarea[name=CommentText]').remove();
                    $('#highlight-container').find(".mceEditor").remove();
                }
            }
            catch (e) {
                //PxPage.log("Unbinding exception thrown for tinymce" + e); 
            }
        },

        GetCommandNameForEditor: function() {
            if (jQuery.browser.msie) return "mceInsertRawHTML";
            else if (jQuery.browser.mozilla) return "insertHtml";
            else return "insertHtml";
        },

        SaveCurrentNote: function (event) {
            /// <summary>
            /// Saves the current active note block
            /// </summary>
            /// <param name="event" type="Object">Mouse click event hander object.  Doesn't look like it is currently used.</param>
            
            var currentHighlightBlock = $('.highlight-block.active');
            if (currentHighlightBlock.length < 1) return;
            if (currentHighlightBlock.attr('id') == 'highlight-block-0') {
                //new comment inside a new bubble.
                PxComments.SaveNewComment();
            } else {
                //New comment inside existing bubble or existing comment inside existing bubble
                PxComments.SaveNoteBlock('#' + currentHighlightBlock.attr('id'));
            }
        },

        HideHighlightBlockIfNoNotes: function (highlightBlock) {
            //if the first note is empty we don't need to show the block at all.
            var notelistBlock = $(highlightBlock).find('.note-list-box');
            $(notelistBlock).find('.highlight-comment').each(function (i, v) {
                var noteText = $(v).children('div.note').html();
                if ($.trim(noteText) == "") {
                    $(v).addClass('blank-note');
                }
            });
            var firstComment = $(notelistBlock).find('.highlight-comment:first');
            var noteText = $(firstComment).children('div.note').html();
            if ($.trim(noteText) == "") {
                $(highlightBlock).addClass('blank-notes');
            }
        },

        OnToggleLockClicked2: function (event) {
            if ((PxPage.Context.IsInstructor != 'true') && !$(this).parents('div.highlight-block').first().hasClass('mine')) {
                return;
            }
            var highlightBlockId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            var highlightId = highlightBlockId.replace("#highlight-block-", "");
            var lockNotes = $(this).hasClass('unlock');
            var args, url;
            //If has class "page", this is a top note.
            if ($(highlightBlockId).hasClass('page')) {
                url = PxPage.Routes.Hightlight_ToggleLockTopNote;
                args = {
                    noteId: highlightId,
                    lockNotes: lockNotes
                };
            } else {
                url = PxPage.Routes.togglelock_highlight_notes;
                args = {
                    highlightId: highlightId,
                    lockNotes: lockNotes
                };
            }
            

            $.post(url, args, function (response) {
                var hSpan = $(Settings.docFrameId).contents().find('#highlight-' + highlightId);
                if (lockNotes) {
                    $(highlightBlockId + ' span.unlock').removeClass('unlock').addClass('lock');
                    $(highlightBlockId + ' span.lock').attr('title', 'Unlock Note');
                    $(highlightBlockId + ' span.lock').text('Unlock Note');
                    $(highlightBlockId + ' .highlight-comment').addClass('locked');
                    $(highlightBlockId + ' input[name="status"]').val('Locked');
                    $(hSpan).addClass('locked');
                }
                else {
                    $(highlightBlockId + ' span.lock').removeClass('lock').addClass('unlock');
                    $(highlightBlockId + ' span.unlock').attr('title', 'Lock Note');
                    $(highlightBlockId + ' span.unlock').text('Lock Note');
                    $(highlightBlockId + ' .highlight-comment').removeClass('locked');
                    $(highlightBlockId + ' input[name="status"]').val('Active');
                    $(hSpan).removeClass('locked');
                }
            });
            event.stopPropagation();
        },

        OnToggleLockClicked: function (event) {
            if ((PxPage.Context.IsInstructor != 'true') && !$(this).parents('div.highlight-block').first().hasClass('mine')) {
                return;
            }
            var highlightBlockId = "#" + $(this).parents('div.highlight-block').first().attr("id");
            var highlightId = highlightBlockId.replace("#highlight-block-", "");
            var lockNotes = $(this).hasClass('unlock');
            var args = {
                highlightId: highlightId,
                lockNotes: lockNotes
            };

            $.post(PxPage.Routes.togglelock_notes, args, function (response) {
                var hSpan = $(Settings.docFrameId).contents().find('#highlight-' + highlightId);
                if (lockNotes) {
                    $(highlightBlockId + ' span.unlock').removeClass('unlock').addClass('lock');
                    $(highlightBlockId + ' span.lock').attr('title', 'Unlock Note');
                    $(highlightBlockId + ' .highlight-comment').addClass('locked');
                    $(hSpan).addClass('locked');
                }
                else {
                    $(highlightBlockId + ' span.lock').removeClass('lock').addClass('unlock');
                    $(highlightBlockId + ' span.unlock').attr('title', 'Lock Note');
                    $(highlightBlockId + ' .highlight-comment').removeClass('locked');
                    $(hSpan).removeClass('locked');
                }
            });
            event.stopPropagation();
        },

        SetDeleteAccess: function () {
            $('#highlightList').find('.highlight-block').each(function () {
                var notes = $(this).find('.highlight-comment');
                var isLocked = $(this).hasClass("locked");
                if (PxPage.Context.IsInstructor == 'true' || (notes.not('.mine').length < 1 && !isLocked)) {
                    // Allow delete if instructor or a note has no reply from other users and its not locked.
                    $(this).find('span.block-controls > span.delete').show();
                }
                else {
                    $(this).find('span.block-controls > span.delete').hide();
                }
            });
        },

        AddtoNoteLibrary: function (event) {
            var editableNoteId = PxComments.GetEditingMceId();
            var prevText = tinyMCE.get(editableNoteId).getContent();
            prevText = prevText.replace(/<[^>]+>/g, '');

            var completeTitle = prevText.split(' ');
            //var completeTitle = jQuery(this).parents('div.highlight-block').find('textarea').filter(':visible:first').val().split(' ');
            var shortTitle = "";

            if (completeTitle != '') {
                PxPage.Loading('fne-content');
                for (var i = 0; i < 3 && i < completeTitle.length; i++) {
                    shortTitle += completeTitle[i];
                    if (i == 2)
                        shortTitle += "...";
                    else
                        shortTitle += " ";
                }

                $.ajax({
                    url: PxPage.Routes.create_note,
                    type: "POST",
                    data: {
                        Text: prevText,
                        //Text: jQuery(this).parents('div.highlight-block').find('textarea').filter(':visible:first').val(),
                        Title: shortTitle
                    },
                    success: function (response) {
                        $.get(PxPage.Routes.load_comment_library, null, function (response) {
                            $(".commentLibraryWrapper > .dlCommentLibrary").replaceWith(response);
                            $(".commentLibraryWrapper > .dlCommentLibrary").show();
                            PxPage.Loaded('fne-content');
                        });
                        PxComments.BeginOpenNoteLibrary(response);
                        //PxPage.Update();
                        PxNoteLibrary.Init();
                        PxNoteLibrary.DisplayTopNote();

                        //$(highlightBlockId + ' .save').hide();
                    }
                });

                event.stopPropagation();
            }
        },

        OpenNoteLibrary: function (event) {
            PxPage.Loading('fne-content');
            $.post(PxPage.Routes.open_note, null, function (response) {
                PxComments.BeginOpenNoteLibrary(response);
                PxNoteLibrary.Init();
                PxPage.Loaded('fne-content');
            });
            event.stopPropagation();
        },


        OpenLinkLibrary: function (event) {
            PxPage.Loading('fne-content');
            $.post(PxPage.Routes.open_link, null, function (response) {
                PxComments.BeginOpenLinkLibrary(response);
                PxLinkLibrary.Init();
                PxPage.Loaded('fne-content');
            });
            event.stopPropagation();
        },

        BeginOpenNoteLibrary: function (response) {
            // hide all the other highlight blocks except the one selected for already saved highlights.
            var highlights = $("#highlightList");
            highlights.children().hide().filter(".active").show();
            highlights.children('.active').attr('style', 'top:0px');
            highlights.children('.active').find('.commentLibraryWrapper').hide();

            // hide all the other highlight blocks except the one selected for extremely new highlight.
            if ($("#highlight-block-0").hasClass("active")) {
                var position = $("#highlight-new-container").css("top");
                var newContainer = $("#highlight-new-container");
                newContainer.children().hide().filter(".active").show();
                newContainer.attr('style', 'top:0px');
                newContainer.children('.active').attr('style', 'top:0px');
                newContainer.children('.active').find('.commentLibraryWrapper').hide();
                newContainer.append("<input id='newNotePosition' type='hidden' value='" + position + "'/>");
            }

            $('.highlight-bottom-menu').hide();
            $("#document-viewer").addClass('ignore-autosave');
            $('#noteLibrary').remove();
            $('.noElements').remove();
            $(".noteLibraryHeading").remove();
            $('.document-viewer-frame-host').hide();
            $('#notelibrarywrapper').remove();
            $('#document-body').append(response);
            $('#fne-content').animate({ scrollTop: 0 }, 'fast');
            $('#notelibrarywrapper').height($('#fne-content').height());

        },

        BeginOpenLinkLibrary: function (response) {
            // hide all the other highlight blocks except the one selected for already saved highlights.  
            var highlights = $("#highlightList");
            highlights.children().hide().filter(".active").show();
            highlights.children('.active').attr('style', 'top:0px');
            highlights.children('.active').find('.linkLibraryWrapper').hide();

            // hide all the other highlight blocks except the one selected for extremely new highlight.
            if ($("#highlight-block-0").hasClass("active")) {
                var newContainer = $("#highlight-new-container");
                var position = newContainer.css("top");
                newContainer.children().hide().filter(".active").show();
                newContainer.attr('style', 'top:0px');
                newContainer.children('.active').attr('style', 'top:0px');
                newContainer.children('.active').find('.linkLibraryWrapper').hide();
                newContainer.append("<input id='newNotePosition' type='hidden' value='" + position + "'/>");
            }

            $('.highlight-bottom-menu').hide();
            $("#document-viewer").addClass('ignore-autosave');

            $('#linklibrary').remove();
            //$('#content-nav').hide();
            $('.breadcrumb').hide();
            $('.content-title').hide();
            $('#document-body').append(response);
            $('.document-viewer-frame-host').hide();

            //$('#fne-window').animate({ scrollTop: 0 }, 'fast');
            $('#fne-content').animate({ scrollTop: 0 }, 'fast');
        },

        UseLinkOrNote: function (e, requestType, currentObject) {
            var wrapperType, text, preText;
            if (requestType == "link") {
                text = "<a href='" + $(currentObject).next('.linkUrl').val() + "' target='_blank'>" + $(currentObject).nextAll('.linkSearchedName').val() + "</a>";
                wrapperType = ".linkLibraryWrapper";
            }
            else if (requestType == "note") {
                text = $(currentObject).parent().find("input[name='item.Text']").val();
                wrapperType = ".commentLibraryWrapper";
            }

            var currentHighlightBlock = $('.highlight-block.active');
            if (currentHighlightBlock.length < 1) return;

            var editingMceID = PxComments.GetEditingMceId();

            preText = tinyMCE.get(editingMceID).getContent();
            if ($.trim(preText) != "Enter comment here" || $.trim(preText) != "") {
                text = " " + text;
            }

            var commandName = PxComments.GetCommandNameForEditor();
            tinyMCE.get(editingMceID).execCommand(commandName, false, text);

            var newNote = false;
            var topForNewNote;
            if ($("#highlight-block-0").hasClass("active")) {
                newNote = true;
                topForNewNote = $.trim($("#newNotePosition").val());
                $("#highlight-new-container").children('.active').find(wrapperType).show();
                $("#newNotePosition").remove();
            }
            if (requestType == "link") PxLinkLibrary.CloseLinks(e);
            else if (requestType == "note") PxNoteLibrary.CloseNotes(e);

            PxComments.SetHighlightPositions(e);
            if (!newNote) {
                $("#highlight-new-container").hide();
                $("#highlightList").children('.active').find(wrapperType).show();
            }
            else {
                $("#highlight-new-container").attr('style', 'top:' + topForNewNote);
                $("#highlight-block-0").attr('style', 'top:0px');
            }
            var top = newNote ? topForNewNote : $("#highlight-container").find('.active').css("top");
            //$('#document-viewer').scrollTop(parseInt(top, 10));
            $('#fne-content').animate({ scrollTop: top }, 'fast');
            e.stopPropagation();
        },

        // this function makes ajax call to get notes and highlights from server. 
        //Adds Notes to DOM and calls  applyPreviousHighlights on HighlightWidget to apply highlights
        SetHighlightsAndNotes: function () {
            var objBody = $(Settings.docFrameId).contents().find('body'),
            key = objBody.HighlightWidget("getDocumentKey");

            $.get(PxPage.Routes.load_notes, key, function (response) {
                if (!response) {
                    return;
                }
                if ($("#noteSetting-Container").is(":visible")) {
                    $("#noteSetting-Container").hide();
                }

                //PLATX-4180 fix.
                PxComments.SetDeleteAccess();
                var isNotesLoaded = ($("#highlightList div").length == 0) ? false : true;
                var inFne = !$("#fne-window").is(":hidden");
                var isFacePlate = $('body').hasClass('product-type-lms-faceplate') || $('body').hasClass('product-type-faceplate');
                if (inFne && PxComments.IsExternalContent() && isNotesLoaded && !isFacePlate) {
                    // in case of enernal content, do not set notes again in FNE window, becuase they are already set. Setting again caused js errors
                }
                else {
                    $("#highlightList").replaceWith(response.notesHtml).show(); // set notes

                }

                // $("#highlightList").replaceWith(response.notesHtml).show(); // set notes

                PxComments.SetFirstComment();

                objBody.HighlightWidget("applyPreviousHighlights", response.highlights); // apply highlights
                PxComments.ShowHighlightCount();

                PxComments.ShowNoteDescription();

                PxComments.Resize();

                //Don't show the HighlightBlock which has their first note as empty.
                PxComments.CheckForEmptyNotes();
            });

        },

        CheckForEmptyNotes: function () {
            $("#highlightList").find('.highlight-block').each(function (i, v) {
                PxComments.HideHighlightBlockIfNoNotes(v);
            });
            PxComments.ShowHighlightCount();
        },

        BindFrameControls: function () {
            //tinymce.relaxedDomain = document.domain;
            var isAuthenticated = (PxPage.Context.IsAuthenticated == 'true');
            var isReadOnly = ($(Settings.docViewerId).hasClass('readonly'));
            var inFne = !$("#fne-window").is(":hidden");            
            if (isAuthenticated) {
                if (!isReadOnly) {
                    $(Settings.docFrameId).contents().find('body').HighlightWidget("init", {
                        storeUrl: PxPage.Routes.save_highlight,
                        updateColorUrl: PxPage.Routes.save_highlight_color,
                        iframe: $(Settings.docFrameId).last()[0],
                        showNotesOnHover: !inFne,
                        onAddNote: PxComments.OnAddNote,
                        onReplyNote: PxComments.OnReplyNote,
                        onEditNote: PxComments.OnEditNote,
                        onDeleteHighlight: PxComments.OnDeleteHighlight,
                        onHighlightClicked: PxComments.OnHighlightClicked,
                        highlightKey: {
                            ItemId: $("#highlight-new-container form input[name='ItemId']").val(),
                            SecondaryId: $("#highlight-new-container form input[name='SecondaryId']").val(),
                            PeerReviewId: $("#highlight-new-container form input[name='PeerReviewId']").val(),
                            HighlightType: $("#highlight-new-container form input[name='HighlightType']").val(),
                            HighlightDescription: $("#highlight-new-container form input[name='HighlightDescription']").val()
                        }
                    });

                    PxComments.SetHighlightsAndNotes();
                    if (!inFne) {
                        highlightMenu({ selector: "#content_widget_highlight_menu" });
                    }
                    else {
                        //highlightMenu({ selector: $("#fne-window").find("#content_widget_highlight_menu") });
                        PxComments.DisplayShowMenu();
                    }

                }
            }
        },


        BindControls: function () {
            //tinymce.relaxedDomain = document.domain;
            var isReadOnly = $(Settings.docViewerId).hasClass('readonly') || $(this).hasClass('locked');
            if (!isReadOnly) {
                $('#fne-window #fne-content').live('click', PxComments.OnNoteBlur);
                $(Settings.docFrameId).contents().find('body').bind('click', PxComments.OnNoteBlur);
                $('.highlight-link').die().live('click', PxComments.OnHighlightMenuClicked);
                $(".highlight-block").die().live('click', PxComments.OnHighlightBlockClicked);
                $('.highlight-comment').die().live('click', PxComments.OnNoteClicked);
                $('.note-description').die().live('click', PxComments.OnNoteDescriptionClicked);


                $(".highlight-block #CommentText").live('focusin', PxComments.OnCommentFocus);
                $(".highlight-block #CommentText").live('focusout', PxComments.OnCommentBlur);
                $(".highlight-block #CommentLink").live('focusin', PxComments.OnCommentLinkFocus);
                $(".highlight-block #CommentLink").live('focusout', PxComments.OnCommentLinkBlur);

                // highlight block button events 
                $(".highlight-block .commentLink").die().live('click', PxComments.OnCommentLinkClicked);
                $(".highlight-block .commentLibrary").die().live('click', PxComments.OnCommentLibraryClicked);
                $(".highlight-block .cancel").die().live('click', PxComments.OnCancelClicked);
                $(".highlight-block .close").die().live('click', PxComments.OnCloseHighlightClicked);
                $(".linkLibraryWrapper > div .more_eBook_links").die().live('click', PxComments.OpenLinkLibrary);

                $(Settings.newBlockId + ' span.block-controls > span:not(.close)').die().live('click', PxComments.OnNewBlockControlsClicked);

                $(".highlight-block:not(" + Settings.newBlockId + ") .share, .highlight-block .shared").die().live('click', function (event) {
                    // This is a temporary solution, should change to better way to handle topnotes.
                    var highlightBlock = $(this).parents('.highlight-block');
                    if (highlightBlock.hasClass('page')) {
                        PxComments.OnToggleShareNoteClicked.apply($(this), [event]);
                    }
                    else {
                        PxComments.OnToggleShareClicked.apply($(this), [event]);
                    }
                });

                $('.highlight-block .comment-submit[type=button]').die().live('click', PxComments.SaveCurrentNote);
                //$(".highlight-block .lock, .highlight-block:not(" + Settings.newBlockId + ") .unlock").die().live('click', PxComments.OnToggleLockClicked);
                $('.highlight-block .commentTextbox').die().live({
                    'keypress': PxComments.OnCommentKeyPress,
                    'click': function () { return false; }
                });

                $('#CommentLink').die().live({
                    'keypress': function (event) { if (event.which == 13) return false; }
                });

                $('.highlight-comment-form .commentLinkSubmit').die().live('click', PxComments.OnLinkSubmit);

                $("#highlight-block-0 input[name='submitButton']").die().live('click', function () { PxComments.SaveNewComment(); return false; });

                // highlight block form events
                $(".commentLibraryWrapper >  .dlCommentLibrary").die().live($.browser.msie ? 'click' : 'change', PxComments.OnCommentLibraryChanged);
                //Add note library events.
                $(".commentLibraryWrapper > div .add-to-note-library").die().live('click', PxComments.AddtoNoteLibrary);
                $(".commentLibraryWrapper > div .open-note-library").die().live('click', PxComments.OpenNoteLibrary);

                $(".linkLibraryWrapper >  .listLinkLibrary").die().live($.browser.msie ? 'click' : 'change', PxComments.OnLinkLibraryChanged);

                $(".highlight-bottom-menu >  .dlRubricsList").die().live($.browser.msie ? 'click' : 'change', PxComments.OnRubricListChanged);


                $('.highlight-public-private').die().live('change', PxComments.OnToggleShareClicked2);

                $('.highlight-note-action-menu').find('.delete').unbind('click').die().live('click', PxComments.OnDeleteNoteClicked);
                $('.highlight-note-action-menu').find('.edit').unbind('click').die().live('click', PxComments.OnEditNoteClicked);
                $('.highlight-block').find('.lock:first, .unlock:first').css('display', 'block');
                $('.highlight-note-action-menu').find('.lock, .unlock').unbind('click').die().live('click', PxComments.OnToggleLockClicked2);

                $('.highlight-note-action-menu').die().live({
                    mouseenter: function () {
                        var highlightBlock = $(this).closest(".highlight-block");
                        var newZindex = parseInt(highlightBlock.css('z-index'), 10) + 1;
                        highlightBlock.css("z-index", newZindex);
                        
                        $(this).find('.highlight-note-action-menu-list').stop(true, true).slideDown("fast");
                        $(this).find('.highlight-note-action-menu-list').position({ my: 'right top', at: 'right bottom', of: $(this) });
                    },
                    mouseleave: function () {
                        var highlightBlock = $(this).closest(".highlight-block");
                        var newZindex = parseInt(highlightBlock.css('z-index'), 10) - 1;
                        highlightBlock.css("z-index", newZindex);
                        $(this).find('.highlight-note-action-menu-list').stop(true, true).slideUp("fast");
                    },
                    click: function (e) {
                        e.stopPropagation();
                    }
                });

                $('.highlight-block').die().live({
                    mouseenter: function () {
                        var targetHighlight = "span[id='" + $(this).attr("id").replace("highlight-block", "highlight") + "']";
                        var doc = $(Settings.docFrameId).get(0).contentWindow.document;
                        $(targetHighlight, doc).each(function () {
                            if (/color-\d+/.test(this.className)) {
                                var colorClass = this.className.match(/color-\d+/)[0];
                                $(this).removeClass(colorClass).addClass(colorClass + '-hover');
                            }
                        });

                        $(this).addClass("highlight-block-hightlighted");
                    },
                    mouseleave: function () {
                        var targetHighlight = "span[id='" + $(this).attr("id").replace("highlight-block", "highlight") + "']";
                        var doc = $(Settings.docFrameId).get(0).contentWindow.document;
                        $(targetHighlight, doc).each(function () {
                            if (/color-\d+-hover/.test(this.className)) {
                                var colorClass = this.className.match(/color-\d+-hover/)[0];
                                var origColorClass = colorClass.replace("-hover", "")
                                $(this).removeClass(colorClass).addClass(origColorClass);
                            }
                        });

                        $(this).removeClass("highlight-block-hightlighted");
                    }
                });
            }
        },

        OnFrameLoaded: function () {
            PxPage.UpdateFneSize();

            // loading css with relative path causes issue in IE because it tries to load css in the scope/url of frame
            // hence we load it with fully qualified path which is relative to current window url
            var cssPath = PxPage.CssRoutes.highlights_css,
            cssPath = window.location.protocol + '//' + window.location.host + cssPath;
            // PxPage.log("css -- " + cssPath);
            $(Settings.docFrameId).contents().find('head').append('<link href="' + cssPath + '" rel="stylesheet" type="text/css" />');

            //resize according to doc viewer position            
            //reset height of content
            $(Settings.docFrameId).contents().find("html").first().css("height", "auto");
            if ($(Settings.docFrameId).parents('#right').length) {
                $(Settings.docFrameId).iframeAutoHeight({ heightOffset: 20 });
                if ($(Settings.bodyElement).length) {
                    var outerIframe = $(Settings.bodyElement).find('iframe');
                    if (outerIframe.length) {
                        outerIframe.iframeAutoHeight({ heightOffset: 20 });
                    }
                }
            }

            $.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
            if ($.browser.chrome) {
                $(Settings.docViewerId).first().css("overflow", "visible");
                $(Settings.docFrameId).first().css("height", "100%");
            }

            // Move title to document body only on highlightable documents.
            if ($("#fne-window").is(":visible") && $('#fne-content h2.content-title:first:visible') && $('#fne-content .assignment-viewer').length < 1) {
                var title = PxPage.GetTitleFromContent();
                if (title != '') {
                    PxPage.SetFneTitle(title);
                }
                $('#fne-content h2.content-title:first').hide();
            }

            PxComments.Resize();

            $(Settings.docFrameId).contents().find("body").rebind("onresize", function () {
                PxComments.Resize();
            });

            if ($('#fne-window').is(':visible') && $('#fne-content #document-viewer #highlight-container').length) {
                //make comment section at least equal height of the document
                $('#highlightList').css({ "min-height": $('#document-body').height() }).show();
                if ($('#fne-content .assignment-viewer').length == 0) {
                    $('#fne-window #fne-title-left').css('border-top-right-radius', '0');
                }
            }

            PxComments.BindFrameControls();
        },

        OnFneLoaded: function () {
            if ($('#fne-window').is(':visible') && ($('#fne-content #document-viewer').hasClass('readonly') == false)) {
                $('#fne-content').addClass('hasNotes');
            }
        },
        Init: function (opts) {
            //tinymce.relaxedDomain = document.domain;
            tinyMCE.init(PxPage.editableNote_editor_options);

            $(Settings.docFrameId).unbind("load");

            //Clear previous settings after running an unbind.
            Settings = {};

            //Create new Settings object
            Settings = $.extend(true, {}, Settings, defaults, opts);

            //Clone the tinyMCE launchpad_editor_options so it won't affect other components that use this editor config.
            Settings.topNoteSpecificEditorOption = jQuery.extend(true, {}, PxPage.launchpad_editor_options);
            Settings.topNoteSpecificEditorOption.editor_selector = "zen-editor";
            Settings.topNoteSpecificEditorOption.mode = "specific_textareas";
            Settings.topNoteSpecificEditorOption.theme_advanced_resizing = false;
            Settings.topNoteSpecificEditorOption.setupcontent_callback = "setContentOk";
            Settings.topNoteSpecificEditorOption.width = "100%";

            var targetIframe = Settings.contentFrame ? $(Settings.contentFrame) : $(Settings.docFrameId);
            Settings.docFrameId = targetIframe;
            //Ensures that document.evaluate works for all browsers
            if (!Settings.docFrameId.length) {
                return;
            }
            wgxpath.install(Settings.docFrameId[0].contentWindow);

            Settings.docViewerId = Settings.viewerElement ? Settings.viewerElement : Settings.docViewerId;
            Settings.docBodyId = Settings.bodyElement ? Settings.bodyElement : Settings.docBodyId;

            this.BindControls();

            //            $(Settings.docFrameId).unbind("load").load(function () {
            //                PxComments.Init();
            //            });

            PxPage.FneInitHooks["document-viewer"] = function () {
                PxComments.OnFneLoaded();
                PxComments.Resize();
                if ($(Settings.docFrameId).length == 0) {
                    PxComments.DisplayShowMenu();
                } else {
                    //                    already executed by
                    //PxComments.OnFrameLoaded();
                }
            };
            PxPage.FneResizeHooks["document-viewer"] = PxComments.Resize;
            PxPage.FneCloseHooks["document-viewer"] = function () {
                PxPage.FneInitHooks["document-viewer"] = null;
                $('#shared-data-menu').remove();
                PxComments.Resize();
                $('#fne-content').removeClass('hasNotes');
            };

            // doc viewer frame events
            if ($(Settings.docFrameId).length) {
                $('#document-viewer #content-loading').hide();
                $('#document-viewer #document-body').show();
                PxComments.OnFrameLoaded();
            }

            //hide loading.. message
            //Fix done as part of 4927 and 4856
            PxPage.FneLoadedHooks["document-viewer"] = function () {
                $('#document-viewer #content-loading').hide();
            };
            //hide the new highlight form            
            $('.highlight-new-container').hide();

            PxComments.SetDeleteAccess();

            $(Settings.docFrameId).iframeAutoHeight({ heightOffset: 20 });

            if ($(Settings.bodyElement).length) {
                var outerIframe = $(Settings.bodyElement).find('iframe');
                if (outerIframe.length) {
                    outerIframe.iframeAutoHeight({ heightOffset: 20 });
                }
            }
            
            if (postInit) {
                postInit();
            }
        }
    }
} (jQuery);



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//      UTIL METHODS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// WINDOW SELECTION METHODS
///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function getSelectedText() {
    var t = '';
    if (window.getSelection) {
        t = window.getSelection();
    }
    else if (document.getSelection) {
        t = document.getSelection();
    }
    else if (document.selection) {
        t = document.selection.createRange().text;
    }
    return t;
}

function getIframeSelectionText(iframe) {
    var win = iframe.contentWindow;
    var doc = win.document;

    if (win.getSelection) {
       // PxPage.log(win.getSelection());
        return win.getSelection().toString();
    } else if (doc.selection && doc.selection.createRange) {
        //PxPage.log(doc.selection);
        return doc.selection.createRange().text;
    }
}

//getRelativePosition
// selector: 
function getRelativePosition(selector, parentSelector, contentFrame) {

    var $parentElem = $(parentSelector);

    var $element = $(selector);
    
    if ($element.length == 0) {
        return null;
    }
        
    var elementPosition = $element.offset();
    var parentPosition = $parentElem.offset();

    return {
        top: elementPosition.top - parentPosition.top,
        left: elementPosition.left - parentPosition.left
    };
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// POSITIONING METHODS
///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function getIframeRelativePosition(selector, parentSelector, frameSelector) {

    var $parentElem = $(parentSelector);
    var $element = $(frameSelector).contents().find(selector);

    var elementPosition = $element.offset();
    var parentPosition = $parentElem.offset();
    return {
        top: elementPosition.top,
        left: elementPosition.left
    };
}


/// For TinyMCE Cursor to end.

var contentOk = false;
var initComplete = false;

function setContentOk(editor_id, body, doc) {
    try {
        contentOk = editor_id;
        move();
    }
    catch (e) { }
}

function move() {    
    moveCursorToEnd(contentOk); initComplete = true;
}

// This is the function that moves the cursor to the end of content
function moveCursorToEnd(editor_id) {
    //inst = tinyMCE.getInstanceById(editor_id);
    inst = tinyMCE.activeEditor;
    tinymce.execCommand('mceFocus', false, editor_id);
    tinyMCE.execInstanceCommand(editor_id, "selectall", false, null);
    if (tinyMCE.isMSIE) {
        rng = inst.getRng();
        rng.collapse(false);
        //rng.select();
    }
    else {
        try {
            if (!inst.getSel == undefined) {
                sel = inst.getSel();
                sel.collapseToEnd();
            }
        }
        catch(e) {
           // PxPage.log(e);
        }
    }
}