var PxAnnouncementWidget = function ($) {
    return {
        BindControls: function () {

            $('.announcementPin').each(function () {
                var pinSortOrder = $(this).closest('.announcementItem').attr('pinsortorder');
                if (pinSortOrder != '' && pinSortOrder != null && pinSortOrder != 'undefined') {
                    $(this).removeClass('UnPin').addClass('Pin');
                }
            });

            //highlight the annoucement when hover over it
            $('.announcementItem').mouseenter(function () {
                var announcementEdit = $(this).find('.announcementEdit');
                announcementEdit.css('display', 'inline-block');

                var announcementDelete = $(this).find('.announcementDelete');
                announcementDelete.css('display', 'inline-block');
            });

            //make the annoucement display normal when the mouse leaves the announcement
            $('.announcementItem').mouseleave(function () {
                var announcementEdit = $(this).find('.announcementEdit');
                announcementEdit.css('display', 'none');

                var announcementDelete = $(this).find('.announcementDelete');
                announcementDelete.css('display', 'none');
            });

            //archived elements should not have the edit and pin functionalities
            $('.announcementItem').each(function () {
                isArchived = $.parseJSON($(this).attr('isarchived'));
                if (isArchived) {
                    announcementPin = $(this).find('.announcementPin');
                    announcementEdit = $(this).find('.announcementEdit');
                    $(announcementPin).remove();
                    $(announcementEdit).remove();
                }
            });

            //enable the water mark for all the Announcement textboxes
            $('.txtPostAnnouncement').each(function () {
                PxAnnouncementWidget.EnableWaterMark(this);
            });

            $('.txtPostAnnouncement').click(function (event) {
                event.stopImmediatePropagation();
                var bool = $(this).hasClass('watermark');
                if (bool) {
                    $(this).val('');
                    $(this).removeClass('watermark');
                    var btnPostAnnouncement = $(this).closest('.containerPostAnnouncement').siblings('.btnPostAnnouncement');
                    btnPostAnnouncement.css('background-color', '');
                    btnPostAnnouncement.removeAttr('disabled');
                }
            });

            //enable the watermark if there is no text in the Announcement textbox
            $('.txtPostAnnouncement').blur(function () {
                if ($(this).val() == '') {
                    PxAnnouncementWidget.EnableWaterMark(this);
                }
            });

            //dynamic resize of the textarea
            $('.txtPostAnnouncement, .txtEditAnnouncement').keyup(function (event) {
                event.stopImmediatePropagation();
                //dynamic resize of the textarea
                var txtCopy = $(this).siblings('.textCopy');
                PxAnnouncementWidget.autoSize(this, txtCopy);
            });

            // NEW : post the announcement if the enter key is hit in the announcement textarea
            $('.txtPostAnnouncement').keydown(function (event) {
                event.stopImmediatePropagation();

                //post the announcement if the enter key is hit (without holding the shift key)
                if (event.keyCode == 13 && !event.shiftKey) {
                    var btnPostAnnouncement = $(this).closest('.containerPostAnnouncement').siblings('.btnPostAnnouncement');
                    var announcement = $(btnPostAnnouncement).siblings('.containerPostAnnouncement').find('.txtPostAnnouncement').val();
                    PxAnnouncementWidget.PostAnnouncement(btnPostAnnouncement, announcement);
                }
            });

            // NEW : post announcement button
            $('.btnPostAnnouncement').click(function (event) {
                //stop the event propagation
                event.stopImmediatePropagation();
                btnPostAnnouncement = $(this);
                var announcement = $(btnPostAnnouncement).siblings('.containerPostAnnouncement').find('.txtPostAnnouncement').val();
                PxAnnouncementWidget.PostAnnouncement(btnPostAnnouncement, announcement);
            });

            // EDIT : post the announcement if the enter key is hit in the announcement textarea
            $('.txtEditAnnouncement').keydown(function (event) {
                event.stopImmediatePropagation();
                //post the announcement if the enter key is hit (without holding the shift key) 
                if (event.keyCode == 13 && !event.shiftKey) {
                    var editButton = $(this).closest('.editWrapper').find('.saveEditAnnouncement');
                    PxAnnouncementWidget.SaveEditAnnouncement(editButton);
                }
            });

            // EDIT :save edited announcement button
            $('.saveEditAnnouncement').click(function () {
                var editButton = $(this);
                PxAnnouncementWidget.SaveEditAnnouncement(editButton);
            });

            //view all announcements modal dialog
            $('.announcementViewAllLink').click(function (event) {

                event.stopImmediatePropagation();

                var announcementModal = $('.announcementModal').first();

                var widgetBody = $(this).closest('.announcementWidget');
                var widgetBodyID = $(widgetBody).attr('id');

                $(announcementModal).dialog({ width: 1000, height: 600, minWidth: 1000, minHeight: 600, modal: true, draggable: false, title: 'Announcements',
                    create: function (event, ui) {
                        var anmtDialog = $(this).closest('.ui-dialog');
                        $(anmtDialog).attr('id', 'anmtDialog');
                        PxPage.Loading('anmtDialog'); //Load the spinner
                        PxAnnouncementWidget.ReloadViewAllWidget(announcementModal);
                    },
                    close: function (event, ui) {
                        $(announcementModal).remove();
                        PxPage.Loading(widgetBodyID); //Load the spinner
                        PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                    }
                });
            });

            //delete announcement
            $('.announcementDelete').click(function () {
                var bool = confirm('Do you want to remove the announcement?');
                if (!bool) {
                    return;
                }
                var announcementItem = $(this).closest('.announcementItem');
                var announcementID = $(this).attr('announcementID');
                var location = $(announcementItem).closest('.announcementWidget').attr('location');
                if (location == 'summary') {
                    var widgetBody = $(announcementItem).closest('.announcementWidget');
                    var widgetBodyID = $(widgetBody).attr('id');
                    PxPage.Loading(widgetBodyID); //Load the spinner                    
                }
                else {
                    var anmtModal = $(announcementItem).closest('.announcementModal');
                    var anmtDialog = $(anmtModal).closest('.ui-dialog');
                    var anmtDialogID = $(anmtDialog).attr('id');
                    PxPage.Loading(anmtDialogID); //Load the spinner                    
                }

                var removeAnnouncementURL = PxPage.Routes.removeAnnouncement;

                //ajax call to remove the Announcement from Agilix
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: removeAnnouncementURL,
                    data: { announcementID: announcementID },
                    success: function (msg) {
                        if (location == 'summary') {
                            PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                        }
                        else {
                            PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                        }
                    },
                    error: function (req, status, error) {
                        alert('ERROR_DELETE_ANNOUNCEMENT');
                    }
                });
            });

            //make the announcement editable
            $('.announcementEdit').click(function () {
                var announcementItem = $(this).closest('.announcementItem');
                var displayWrapper = $(announcementItem).find('.displayWrapper');
                var editWrapper = $(displayWrapper).siblings('.editWrapper');
                displayWrapper.hide();
                editWrapper.show();
            });

            //change from edit mode to display mode
            $('.cancelEditAnnouncement').click(function () {
                var editWrapper = $(this).closest('.editWrapper');
                var displayWrapper = $(editWrapper).siblings('.displayWrapper');
                var announcementText = $.trim($(displayWrapper).find('.announcementBody').html());
                announcementText = (announcementText).replace(/<br>/g, '\n');
                $(editWrapper).find('.txtEditAnnouncement').val(announcementText);
                editWrapper.hide();
                displayWrapper.show();
            });

            //implement drag and drop for Announcements
            $('.announcementList').sortable({
                items: "li[isarchived='false'][pinsortorder='']",
                stop: function (event, ui) {
                    sortableList = $(this);
                    PxAnnouncementWidget.MoveAnnouncement(sortableList, ui);
                }
            });

            //pin an announcement
            $('.announcementPin').click(function (event) {
                event.stopImmediatePropagation(); //to stop the event from firing twice
                announcementPin = $(this);
                PxAnnouncementWidget.PinAnnouncement(announcementPin);

            });

            //Archive the announcement            
            $('.announcementArchiveButton').click(function (event) {
                event.stopImmediatePropagation(); //to stop the event from firing twice
                var announcementItem = $(this).closest('.announcementItem');
                PxAnnouncementWidget.ArchiveAnnouncement(announcementItem);
            });

            //Repost the announcement
            $('.announcementRepostButton').click(function (event) {
                event.stopImmediatePropagation(); //to stop the event from firing twice
                var announcementText = $(this).parents('.announcementItem ').find('.announcementBody').html();
                announcementText = (announcementText).replace(/<br>/g, '\n');
                var btnPostAnnouncement = $(this).closest('.announcementModal').find('.btnPostAnnouncement');
                PxAnnouncementWidget.PostAnnouncement(btnPostAnnouncement, announcementText);
            });

            //Done button for Announcement Dialog
            $('.announcementModalDone').click(function () {
                announcementModal = $(this).closest('.announcementModal');
                $(announcementModal).dialog('close');
            });

            //Close the edit dialog
            $('.closeEditAnnouncement').click(function () {
                $('.ui-icon-closethick').click();
            });
        },

        SaveEditAnnouncement: function (editButton) {
            editWrapper = $(editButton).closest('.editWrapper');
            txtEditAnnouncement = $(editWrapper).find('.txtEditAnnouncement');
            announcementID = $(txtEditAnnouncement).attr('announcementID');
            announcementText = $.trim(txtEditAnnouncement.val());
            creationDate = $(txtEditAnnouncement).attr('creationDate');
            announcementItem = $(editButton).closest('.announcementItem')
            sequence = $(announcementItem).attr('sequence');
            pinSortOrder = $(announcementItem).attr('pinsortorder');
            editAnnouncementURL = PxPage.Routes.editAnnouncement;

            //validate the edit announcement texbox            
            if (announcementText == '') {
                alert('Announcement cannot be empty!');
                return;
            }

            var location = $(editButton).closest('.announcementWidget').attr('location');
            if (location == 'summary') {
                var widgetBody = $(editButton).closest('.announcementWidget');
                var widgetBodyID = $(widgetBody).attr('id');
                PxPage.Loading(widgetBodyID); //Load the spinner                
            }
            else {
                var anmtModal = $(editButton).closest('.announcementModal');
                var anmtDialog = $(anmtModal).closest('.ui-dialog');
                var anmtDialogID = $(anmtDialog).attr('id');
                PxPage.Loading(anmtDialogID); //Load the spinner
            }

            //post the edited announcement
            var bodyPostContent = $.ajax({
                type: 'POST',
                url: editAnnouncementURL,
                data: { announcementID: announcementID, announcementText: announcementText, creationDate: creationDate, sequence: sequence, pinSortOrder: pinSortOrder },
                success: function (msg) {
                    if (location == 'summary') {
                        PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                    }
                    else {
                        PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                    }
                },
                error: function (req, status, error) {
                    alert('ERROR_EDIT_ANNOUNCEMENT');
                }
            });
        },

        PinAnnouncement: function (announcementPin) {
            announcementItem = $(announcementPin).closest('.announcementItem');
            txtEditAnnouncement = $(announcementItem).find('.txtEditAnnouncement');
            announcementID = $(txtEditAnnouncement).attr('announcementID');
            announcementText = txtEditAnnouncement.val();
            creationDate = $(txtEditAnnouncement).attr('creationDate');

            var location = $(announcementItem).closest('.announcementWidget').attr('location');
            if (location == 'summary') {
                var widgetBody = $(announcementItem).closest('.announcementWidget');
                var widgetBodyID = $(widgetBody).attr('id');
                PxPage.Loading(widgetBodyID); //Load the spinner                
            }
            else {
                var anmtModal = $(announcementItem).closest('.announcementModal');
                var anmtDialog = $(anmtModal).closest('.ui-dialog');
                var anmtDialogID = $(anmtDialog).attr('id');
                PxPage.Loading(anmtDialogID); //Load the spinner
            }

            //pin the announcement
            if ($(announcementPin).hasClass('UnPin')) {
                pinSortOrder = $(announcementItem).index() + 1;
                sequence = '';
                pinAnnouncementURL = PxPage.Routes.pinAnnouncement;

                //pin the sequence of the announcement in Agilix
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: pinAnnouncementURL,
                    data: { announcementID: announcementID, announcementText: announcementText, creationDate: creationDate, sequence: sequence, pinSortOrder: pinSortOrder },
                    success: function (msg) {
                        if (location == 'summary') {
                            PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                        }
                        else {
                            PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                        }
                    },
                    error: function (req, status, error) {
                        alert('ERROR_PIN_ANNOUNCEMENT');
                    }
                });
            }
            else { //Unpin the announcement
                pinSortOrder = '';
                //find sequence of the immediate unpinned announcements                
                prevItems = $(announcementItem).prevAll();
                nextItems = $(announcementItem).nextAll();
                prevSequence = '';
                nextSequence = '';
                $.each(prevItems, function (index, value) {
                    var item = prevItems[index];
                    pinSortOrder = $(item).attr('pinsortorder');
                    if (pinSortOrder == '') {
                        prevSequence = $(item).attr('sequence');
                        return false;
                    }
                });
                $.each(nextItems, function (index, value) {
                    var item = nextItems[index];
                    pinSortOrder = $(item).attr('pinsortorder');
                    isArchived = $.parseJSON($(item).attr('isarchived'));
                    if (isArchived) { //there is no need to loop through archived items
                        return false;
                    }
                    if (pinSortOrder == '') {
                        nextSequence = $(item).attr('sequence');
                        return false;
                    }
                });

                unpinAnnouncementURL = PxPage.Routes.unpinAnnouncement;

                //un-pin the sequence of the announcement in Agilix
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: unpinAnnouncementURL,
                    data: { announcementID: announcementID, announcementText: announcementText, creationDate: creationDate, prevSequence: prevSequence, nextSequence: nextSequence, pinSortOrder: pinSortOrder },
                    success: function (msg) {
                        if (location == 'summary') {
                            PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                        }
                        else {
                            PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                        }
                    },
                    error: function (req, status, error) {
                        alert('ERROR_PIN_ANNOUNCEMENT');
                    }
                });
            }
        },

        ArchiveAnnouncement: function (announcementItem) {
            var txtEditAnnouncement = $(announcementItem).find('.txtEditAnnouncement');
            var announcementID = $(txtEditAnnouncement).attr('announcementID');
            var announcementText = txtEditAnnouncement.val();
            var creationDate = $(txtEditAnnouncement).attr('creationDate');
            var sequence = $(announcementItem).attr('sequence');
            var pinSortOrder = '';
            var archiveAnnouncementURL = PxPage.Routes.archiveAnnouncement;

            var location = $(announcementItem).closest('.announcementWidget').attr('location');
            if (location == 'summary') {
                var widgetBody = $(announcementItem).closest('.announcementWidget');
                var widgetBodyID = $(widgetBody).attr('id');
                PxPage.Loading(widgetBodyID); //Load the spinner                
            }
            else {
                var anmtModal = $(announcementItem).closest('.announcementModal');
                var anmtDialog = $(anmtModal).closest('.ui-dialog');
                var anmtDialogID = $(anmtDialog).attr('id');
                PxPage.Loading(anmtDialogID); //Load the spinner                
            }

            //archive the announcement on the server
            var bodyPostContent = $.ajax({
                type: 'POST',
                url: archiveAnnouncementURL,
                data: { announcementID: announcementID, announcementText: announcementText, creationDate: creationDate, sequence: sequence, pinSortOrder: pinSortOrder },
                success: function (msg) {
                    if (location == 'summary') {
                        PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                    }
                    else {
                        PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                    }
                },
                error: function (req, status, error) {
                    alert('ERROR_ARCHIVE_ANNOUNCEMENT');
                }
            });
        },

        MoveAnnouncement: function (sortableList, ui) {
            announcementItem = ui.item;

            //check if the item is an Archived announcement -- it is not possible to move an archived item
            IsArchived = $.parseJSON($(announcementItem).attr('isarchived'));

            if (IsArchived) {
                $(sortableList).filter('.ui-sortable').sortable('cancel');
                return;
            }

            //check if the immediate items are archived -- we cannot move an announcement in between archived announcements
            isPrevArchived = ui.item.prevAll('.announcementItem').first().attr('isarchived');
            isNextArchived = ui.item.nextAll('.announcementItem').first().attr('isarchived');
            if (isPrevArchived == undefined) {
                isPrevArchived = 'false';
            }
            isPrevArchived = $.parseJSON(isPrevArchived);

            if (isNextArchived == undefined) {
                isNextArchived = 'false';
            }
            isNextArchived = $.parseJSON(isNextArchived);

            if (isPrevArchived || isNextArchived) {
                $(sortableList).filter('.ui-sortable').sortable('cancel');
                return;
            }

            //check if the immediate next items are pinned
            prevPinSortOrder = ui.item.prevAll('.announcementItem').first().attr('pinsortorder');
            nextPinSortOrder = ui.item.nextAll('.announcementItem').first().attr('pinsortorder');
            if (prevPinSortOrder == undefined) {
                prevPinSortOrder = '';
            }
            if (nextPinSortOrder == undefined) {
                nextPinSortOrder = '';
            }
            if (prevPinSortOrder != '' && nextPinSortOrder != '') {
                $(sortableList).filter('.ui-sortable').sortable('cancel');
                return;
            }

            //find sequence of the immediate unpinned announcements
            prevItems = ui.item.prevAll('.announcementItem');
            nextItems = ui.item.nextAll('.announcementItem');
            prevSequence = ''
            nextSequence = ''
            $.each(prevItems, function (index, value) {
                var item = prevItems[index];
                pinSortOrder = $(item).attr('pinsortorder');
                if (pinSortOrder == '') {
                    prevSequence = $(item).attr('sequence');
                    return false;
                }
            });
            $.each(nextItems, function (index, value) {
                var item = nextItems[index];
                pinSortOrder = $(item).attr('pinsortorder');
                if (pinSortOrder == '') {
                    nextSequence = $(item).attr('sequence');
                    return false;
                }
            });

            txtEditAnnouncement = $(announcementItem).find('.txtEditAnnouncement');
            announcementID = $(txtEditAnnouncement).attr('announcementID');
            announcementText = txtEditAnnouncement.val();
            creationDate = $(txtEditAnnouncement).attr('creationDate');
            sequence = $(announcementItem).attr('sequence');
            pinSortOrder = $(announcementItem).attr('pinsortorder');

            //it is not possible to move a pinned item
            if (pinSortOrder != '') {
                $(sortableList).filter('.ui-sortable').sortable('cancel');
                return;
            }

            moveAnnouncementURL = PxPage.Routes.moveAnnouncement;

            var location = $(announcementItem).closest('.announcementWidget').attr('location');
            if (location == 'summary') {
                var widgetBody = $(announcementItem).closest('.announcementWidget');
                var widgetBodyID = $(widgetBody).attr('id');
                PxPage.Loading(widgetBodyID); //Load the spinner                
            }
            else {
                var anmtModal = $(announcementItem).closest('.announcementModal');
                var anmtDialog = $(anmtModal).closest('.ui-dialog');
                var anmtDialogID = $(anmtDialog).attr('id');
                PxPage.Loading(anmtDialogID); //Load the spinner
            }

            //update the sequence of the announcement in Agilix
            var bodyPostContent = $.ajax({
                type: 'POST',
                url: moveAnnouncementURL,
                data: { announcementID: announcementID, announcementText: announcementText, creationDate: creationDate, prevSequence: prevSequence, nextSequence: nextSequence, pinSortOrder: pinSortOrder },
                //data: {},
                success: function (msg) {
                    if (location == 'summary') {
                        PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                    }
                    else {
                        PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                    }
                },
                error: function (req, status, error) {
                    alert('ERROR_MOVE_ANNOUNCEMENT');
                }
            });
        },

        PostAnnouncement: function (btnPostAnnouncement, announcement) {
            //trim the announcement text
            announcement = $.trim(announcement);
            //validate the post announcement texbox            
            if (announcement == '' || announcement == 'Enter a new announcement') {
                alert('Announcement cannot be empty!');
                return;
            }

            var prevSequence = ''; //always place the announcement at top of the stack                
            var nextSequence = '';
            var location = $(btnPostAnnouncement).closest('.announcementWidget').attr('location');
            if (location == 'summary') {
                var widgetBody = $(btnPostAnnouncement).closest('.announcementWidget');
                nextItems = $(widgetBody).find('.announcementItem');
                var widgetBodyID = $(widgetBody).attr('id');
                PxPage.Loading(widgetBodyID); //Load the spinner
            }
            else {
                var anmtModal = $(btnPostAnnouncement).closest('.announcementModal');
                nextItems = $(anmtModal).find('.announcementItem');
                var anmtDialog = $(anmtModal).closest('.ui-dialog');
                var anmtDialogID = $(anmtDialog).attr('id');
                PxPage.Loading(anmtDialogID); //Load the spinner
            }

            $.each(nextItems, function (index, value) {
                var item = nextItems[index];
                pinSortOrder = $(item).attr('pinsortorder');
                isArchived = $.parseJSON($(item).attr('isarchived'));
                if (isArchived) { //there is no need to loop through archived items
                    return false;
                }
                if (pinSortOrder == '') {
                    nextSequence = $(item).attr('sequence');
                    return false;
                }
            });

            //add the posted announcement
            var addAnnouncementURL = PxPage.Routes.addAnnouncement;

            var bodyPostContent = $.ajax({
                type: 'POST',
                url: addAnnouncementURL,
                data: { announcementText: announcement, prevSequence: prevSequence, nextSequence: nextSequence },
                success: function (msg) {
                    if (location == 'summary') {
                        PxAnnouncementWidget.ReloadAnnouncementWidget(widgetBody);
                    }
                    else {
                        PxAnnouncementWidget.ReloadViewAllWidget(anmtModal);
                    }
                },
                error: function (req, status, error) {
                    alert('ERROR_ADD_ANNOUNCEMENT');
                }
            });
        },

        ReloadAnnouncementWidget: function (widgetBody) {
            reloadAnnouncementURL = PxPage.Routes.reloadAnnouncement;
            // ajax call to get widget from server
            var bodyPostContent = $.ajax({
                type: "POST",
                url: reloadAnnouncementURL,
                data: {},
                success: function (widgetHtml) {
                    var widgetBodyID = $(widgetBody).attr('id');
                    PxPage.Loaded(widgetBodyID);
                    $(widgetBody).html(widgetHtml);
                },
                error: function (req, status, error) {
                    var widgetBodyID = $(widgetBody).attr('id');
                    PxPage.Loaded(widgetBodyID);
                    alert("ERROR_SUMMARY_ANNOUNCMENTS");
                }
            });
        },

        ReloadViewAllWidget: function (anmtModal) {
            reloadAnnouncementURL = PxPage.Routes.viewAllAnnoucements;
            var anmtDialog = $(anmtModal).closest('.ui-dialog');
            var anmtDialogID = $(anmtDialog).attr('id');
            // ajax call to get widget from server
            var bodyPostContent = $.ajax({
                type: "POST",
                url: reloadAnnouncementURL,
                data: {},
                success: function (widgetHtml) {
                    PxPage.Loaded(anmtDialogID); //unload the spinner
                    $(anmtModal).html(widgetHtml);
                },
                error: function (req, status, error) {
                    PxPage.Loaded(anmtDialogID); //unload the spinner
                    alert("ERROR_VIEW_ALL_ANNOUNCMENTS");
                }
            });
        },

        EnableWaterMark: function (txtPostAnnouncement) {
            //enable the watermark
            $(txtPostAnnouncement).addClass('watermark');
            $(txtPostAnnouncement).val('Enter a new announcement');

            //grey out and disable the post button 
            var btnPostAnnouncement = $(txtPostAnnouncement).closest('.containerPostAnnouncement').siblings('.btnPostAnnouncement');
            btnPostAnnouncement.css('background-color', '#717D7D');
            btnPostAnnouncement.attr('disabled', 'disabled');
        },

        autoSize: function (txtPostAnnouncement, txtCopy) {
            // Copy textarea contents; browser will calculate correct height of copy,
            // which will make overall container taller, which will make textarea taller.
            var text = $(txtPostAnnouncement).val().replace(/\n/g, '<br/>');
            $(txtCopy).html(text);
        }
    }
} (jQuery);
