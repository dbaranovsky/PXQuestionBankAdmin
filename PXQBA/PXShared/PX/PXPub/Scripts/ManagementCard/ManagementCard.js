var PxManagementCard = function ($) {
    ///<summary>
    /// Assign an item. If unitId is valid then assign an item into provided unit
    ///</summary>
    ///<param name="itemId">Id of the TOC item</param>
    ///<param name="unit">Id of the selected Assignment Unit</param>
    function assign(itemId, unit) {
        var node = $.fn.fauxtree("getNode", itemId);
        var type = node.attr("data-ud-itemtype");
        var isContinue = true;

        if (type == "PxUnit") {
            var date = $("#facePlateAssignDueDate").val();
            var message = "Are you sure you want to assign a new due date to all items in this unit? \n\nIf any items in the unit are already assigned, this action will override those due dates. \n";
            isContinue = confirm(message);
            if (!isContinue) {
                return;
            }
        }

        var points = $('#txtGradePoints').val();
        var contentWrapper = node.find('.contentwrapper');
        var dueDate = $("#facePlateAssignDueDate").val() + " " + $("#facePlateAssignTime").val();
        
        var gradebookCategory = contentWrapper.find('#selgradebookweights option:selected').val();
        
        dueDate = jQuery.trim(dueDate);

        var isValid = true;
        if (type == "PxUnit") {
            if (dueDate.length < 3) {
                isValid = false;
            }
        }
        else {
            if (dueDate.length < 3 && (points == "0" || points == "")) {
                isValid = false;
            }
            else {
                var validDueDate = new Date(dueDate);
                if (isNaN(validDueDate.getTime())) {
                    isValid = false;
                }
            }
        }

        if (isValid == false) {
            PxPage.Toasts.Error("No date selected.");
            return;
        }

        PxPage.log('management card : assign with date : ' + dueDate);
        
        var tempDate = dueDate;
        if (!tempDate) {
            tempDate = '1/1/0001';
        }

        if ((points == "0" || points == "")) {
            var completedStudents = 0;
            var htmlNode = $(".faux-tree-node[data-ft-id=\"" + node.ftattr("id") + "\"] .faceplate-student-completion-stats").html();
            if (htmlNode) {
                completedStudents = htmlNode[0];
            }
            
            var msg = "";
            if (completedStudents > 0) {
                msg = "Setting the points to zero removes the gradebook entry for this item. Existing gradebook contains a student grade.";
            }
            else {
                msg = "You have chosen to assign a due date without putting this item in the gradebook. Are you sure this is what you want to do? To have the item appear in the gradebook, enter gradebook points and choose a gradebook category.";
            }

            if (!confirm(msg)) {
                return;
            }
        }
        else if (points > 100) {
            PxPage.Toasts.Error("Points should be between 0 and 100");
            return;
        }
        
        var onSuccess = function () {
        };

        ContentWidget.IsAssignDateValid(tempDate, function () { ContentWidget.ContentAssignedAssignmentCenter(itemId, unit, dueDate, points, gradebookCategory, onSuccess, null); });

        return;
    }
    
    ///<summary>
    /// Get an Id of the selected Assignment Unit of the opened TOC item's management card
    ///</summary>
    /// <param name="elem">Clicked object</param>
    function getSelectedUnit(elem) {
        //Check for dropdown
        var dropdownWrapper = $(elem).parents().find(".faceplate-assign-dropdown-container");
        if (!dropdownWrapper) {
            return null;
        }
        
        //Check for selected option
        var selectedOption = $(dropdownWrapper).find('#selassignmentunits option:selected');
        if (!selectedOption) {
            return null;
        }
        
        //Grad values and ensure they are valid
        var unitId = selectedOption.val();
        var categoryId = selectedOption.data("category");
        if (!unitId || unitId === '' || !categoryId || categoryId === '') {
            return null;
        }
        return {
            unitId: unitId,
            categoryId: categoryId
        };
    }
    
    function verifySelectedUnit(elem) {
        /// <summary>
        /// Returns false if a valid assignment unit hasn't been selected.  True otherwise
        /// </summary>

        var dropdownWrapper = $(elem).parents().find(".faceplate-assign-dropdown-container");
        
        //If no wrapper exists than we arn't using the assignment unit workflow so return true
        if (dropdownWrapper.length > 0) {
            var unit = getSelectedUnit(elem);
            hideValidators(dropdownWrapper);

            if (!unit) {
                var messages = ["Please choose an assignment."];
                showValidators(dropdownWrapper, messages);
                return false;
            }
        }
        return true;
    }

    ///<summary>
    /// Called before assigning an item
    ///</summary>
    /// <param name="elem">Clicked object</param>
    /// <param name="itemId">Id of the item to be assigned</param>
    /// <param name="needsDueDate">Boolean value to determine if the due date is a must</param>
    function onAssign(elem, needsDueDate, itemId, unit) {
        var dueDateSelected = $("#facePlateAssignDueDate").val().length > 0;
        var dueTimeSelected = $("#facePlateAssignTime").val().length > 0;
        var toValidateDate = needsDueDate || dueDateSelected;

        var isValid = true;

        if (toValidateDate) {
            isValid = dueDateSelected && dueTimeSelected && parseDateTime(true); // validate the picked date and time
        }

        if (isValid) {
            if (needsDueDate) {
                PxManagementCard.assign(itemId, unit);
            } else {
                //TODO: call method that assign item without due date
            }
        }
        else {
            if (!(dueDateSelected && dueTimeSelected)) {
                if ($(".management-card-unassign").length > 0) {
                    PxPage.log('management card : unassign triggered');
                    $(".management-card-unassign").click();
                    return;
                }
            }

            PxPage.Toasts.Error("To assign this item, you must select a due date.");
        }
    }

    function unAssign(itemId) {
        var node = $.fn.fauxtree("getNode", itemId);
        var title = ((node.find(".fptitle").length > 0) ? "&quot;" + node.find(".fptitle").text().trim() + "&quot;" : "this content");
        var isContinue = true;

        var message = "<strong>Are you sure you want to unassign " + title + "?</strong> <br><br>";
        message += "This action will remove the gradebook entry for the assigned item, including existing student grades.";

        $("#unassign-item").die().live("click", function () {
            $(PxPage.switchboard).trigger("contentunassigned", [{
                id: itemId,
                date: '',
                startdate: '',
                points: '',
                keepInGradebook: false,
                category: "",
                gradebookcategory: ''
            }]);
            $(".ui-dialog-titlebar-close").click();
        });
        $(".closeUnassignDialog").die().live("click", function () {
            $(".ui-dialog-titlebar-close").click();
        });

        var options = {
            modal: true,
            draggable: false,
            closeOnEscape: true,
            width: '500px',
            height: '250px',
            resizable: false,
            autoOpen: false,
            title: 'Unassign'
        };
        var tag = $("<div id='px-dialog' class='px-dialog px-default-text'></div>");
        tag.append("<p>" + message + "</p><br /><hr/>");
        tag.append("<input type='button' id='unassign-item' value='Unassign' /> | <a href='#' class='closeUnassignDialog'>Cancel</a>");
        tag.dialog({
            modal: options.modal,
            title: options.title,
            draggable: options.draggable,
            closeOnEscape: options.closeOnEscape,
            width: options.width,
            resizable: options.resizable,
            autoOpen: options.autoOpen,
            zIndex: 2001,
            close: function () {
                $(this).dialog('destroy').remove();
            }
        });
        PxModal.CenterDialog(tag);
        $('#selgradebookweightsHidden').val('');

        return false;
    }

    function populateAssignmentUnits(item) { /* used in PxUnit.ascx */
        PxPage.log("populateAssignmentUnits: refreshing the assignment units");

        var managementCard = $(".FacePlateAsssign.contentwrapper");
        if (!managementCard) {
            PxPage.log("populateAssignmentUnits: cannot find management card");
            return;
        }

        var dlAssignments = managementCard.find("#selassignmentunits");
        if (!dlAssignments) {
            PxPage.log("populateAssignmentUnits: cannot find dropdown list for assignment units");
            return;
        }
        
        if (item
            && item.id && item.name
            && item.id.length && item.name.length)
        {
            var selectOption = "<option value='" + item.id + "' data-category='" + item.categoryId + "'>" + item.name + "</option>";
            $(".FacePlateAsssign.contentwrapper #selassignmentunits option:eq(0)").after(selectOption);
            $(".FacePlateAsssign.contentwrapper #selassignmentunits").val(item.id);

            PxPage.log("populateAssignmentUnits: added a new assignment unit, " + item.name);
        }

        PxPage.CloseNonModal();
    }

    function getDate(d) {
        var text,
            date = Date.parse(d);
        if (date == null) {
            text = date;
            $(".invaliddate").show();
        } else {
            text = date.format('mm/dd/yyyy');
            $(".invaliddate").hide();
        }
        return text;
    }

    //// Internal functions ////

    function getTime(d) {
        var input = d;
        if (/^\d+$/.test(input)) {
            input = input.toNumber();
        }
        var text, dt = Date.parse(input);
        if (dt == null) {
            text = 'Invalid time.';
        } else {
            text = dt.format('hh:MM TT');
        }
        return text;
    }

    function hideValidators(elem) {
        if ($(elem).hasClass("required")) {
            $(elem).toggleClass("required");
        }

        var clsRequiredDynamic = $(elem).find(".required.dynamic");
        $.each(clsRequiredDynamic, function (ind) {
            $(this).remove();
        });
    }

    function showValidators(elem, messages) {
        if (!$(elem).hasClass("required")) {
            $(elem).toggleClass("required");
        }

        if ($.isArray(messages)) {
            $.each(messages, function (ind, val) {
                $(elem).append("<span class='required dynamic'>" + val + "</span>");
            });
        }
    }

    function init(contentItemId, assignmentSettingClass, hiddenFromStudents) {
        initEvents(contentItemId, assignmentSettingClass, hiddenFromStudents);
        initParseDate();
    }

    function initEvents(contentItemId, assignmentSettingClass, hiddenFromStudents) {
        $.fn.fauxtree("setFilterNodesByTree", false);

        if (hiddenFromStudents == 'False') {
            $(".managementcard_students_show").show();
            $(".managementcard_students_hide").hide();
        }
        else {
            $(".managementcard_students_show").hide();
            $(".managementcard_students_hide").show();
        }

        var assignmentSettingsClass = assignmentSettingClass;
        PxPage.log("assignmentSettingClass = " + assignmentSettingClass);
        
        //No need to run FNE size when calling from Management card
        ContentWidget.InitAssign(assignmentSettingsClass, false, false);

        var itemId = contentItemId,
            callback = null;
        //Initilize placeholder for date input in managment card
        $('#facePlateAssignDueDate').placeholder();

        $('.collapse-menu.assign-showCalendar-close').click(function () {
            var elem = this;

            if ($(elem).parents().find(".management-card-tracker").val() != "changed") {
                $(elem).parents().find("#managementcard-close").click();
            }

            var needsDueDate = true;

            onAssign(elem, needsDueDate, itemId);
        });

        $(".clearDateField").click(function () {
            $(".facePlateAssignDueDate").val("");
            $(".facePlateAssignTime").val("");
            $('.invaliddate').text('');
            $('#cal-box #assignment-calendar#assignment-calendar').DatePickerSetDate('');
            $('.placeholderWrap').removeClass('placeholder-changed');
            $(".management-card-tracker").val("changed");
            return false;
        });

        $('.txtGradePoints').change(function () {
            if ($(this).val() != $(this).parents().find(".txtGradePointsHidden").val()) {
                $(this).parents().find(".management-card-tracker").val("changed");
            } else {
                $(this).parents().find(".management-card-tracker").val("");
            }
        });

        $('.facePlateAssignDueDate').change(function () {
            if ($(this).val() != $(this).parents().find(".facePlateAssignDueDateHidden").val() && $(this).val().length > 0) {
                $(this).parents().find(".management-card-tracker").val("changed");
            } else if ($(this).parents().find('.facePlateAssignTime').val() == $(this).parents().find(".facePlateAssignTimeHidden").val()) {
                $(this).parents().find(".management-card-tracker").val("");
            }
        });

        $('.facePlateAssignTime').change(function () {
            if ($(this).val() != $(this).parents().find(".facePlateAssignTimeHidden").val()) {
                $(this).parents().find(".management-card-tracker").val("changed");
            } else {
                $(this).parents().find(".management-card-tracker").val("");
            }
        });

        $('.selgradebookweights').change(function () {
            if ($.trim($("option:selected", this).text()) != $(this).parents().find(".selgradebookweightsHidden").val()) {
                $(this).parents().find(".management-card-tracker").val("changed");
            } else {
                $(this).parents().find(".management-card-tracker").val("");
            }
        });

        //save changes button
        $('.btnSaveChanges').click(function () {
            var contentWrapper = $(this).closest('.contentwrapper');
            ContentWidget.ContentAssigned('assign', itemId, callback, contentWrapper);
        });

        $('.assignment-selection-btn').click(function () {
            var contentWrapper = $(this).closest('.contentwrapper');
            var templateId = contentWrapper.find(".assignmentUnitTemplateId").val();
            var toc = contentWrapper.find(".assignmentUnitToc").val();
            ContentWidget.CreateAssignmentUnit(templateId, toc);
        });

        $('.chkAssignmentGradeable').change(function () {
            var contentWrapper = $(this).closest('.contentwrapper');
            ContentWidget.AssignmentGradeable(contentWrapper);
        });

        $('.chkMarkAsComplete').change(function () {
            var contentWrapper = $(this).closest('.contentwrapper');
            ContentWidget.MarkAsCompleted(contentWrapper);
        });

        $('.chkDueDate').change(function () {
            var contentWrapper = $(this).closest('.contentwrapper');
            var chkDueDate = $(this);
            ContentWidget.DueDateChanged(contentWrapper, chkDueDate);
        });

        var args = {
            assignmentId: contentItemId
        };

        $.post(PxPage.Routes.get_submission_status_management_card, args, function (response) {
            $(".faceplate-student-completion-stats").html(response);
        });
    }
    
    function initParseDate() {
        if ($("#facePlateAssignTime").length > 0) {
            $("#facePlateAssignTime").ptTimeSelect();
        }
        $('#facePlateAssignDueDate').focusout(function () {
            parseDateTime();
        });
        $('#facePlateAssignTime').focusout(function () {
            parseDateTime();
        });
    }

    function parseDateTime(isManual) {
        if ($('#facePlateAssignDueDate').parent().hasClass('placeholder-focus') && !$('#facePlateAssignDueDate').parent().hasClass('placeholder-changed')) {
            $('#cal-box #assignment-calendar').DatePickerSetDate('');
            $('#facePlateAssignDueDate').val("");
            $('.invaliddate').text('');
            return false;
        }

        if (isManual != true) {
            var txtValue = $.trim($('#facePlateAssignDueDate').val());
            var dateval = (txtValue == "") ? getDate() : getDate(txtValue);

            if (dateval == null) {
                dateval = 'invalid date';
            }

            $('#facePlateAssignDueDate').val(dateval);

            if ((dateval.toString().toLowerCase() == 'invalid date.') || (dateval.toString().toLowerCase() == 'invalid date')) {
                $('#cal-box #assignment-calendar').DatePickerSetDate('');
                ContentWidget.AssignmentDateSelected('');
                return false;
            }
            if ($.trim($('#facePlateAssignTime').val()).length < 1) {
                $('#facePlateAssignTime').val('11:59 PM');
            } else {
                var time = getTime(dateval + " " + $.trim($('#facePlateAssignTime').val()));
                $('#facePlateAssignTime').val(time);
            }
            $('#cal-box #assignment-calendar').DatePickerSetDate(dateval, true);
            ContentWidget.AssignmentDateSelected(dateval);
        }

        return true;
    }

    // checks server for gradebook entries, prepares for removeItem
    function checkForSubmissionAndRemoveItem(itemId, tocs) {
        var args = {
            contentItemId: itemId,
            removeFrom: tocs
        };

        $.post(PxPage.Routes.ItemHeirachyHasSubmission, args, function (response) {
            args.hasSubmission = response && response.toLowerCase() === "true";

            removeItem(args);
        });
    }

    function removeItem(item) {
        var node = $.fn.fauxtree("getNode", item.contentItemId);
        var type = node.attr("data-ud-itemtype");
        var message = "";

        if (type == "PxUnit") {
            message = " Are you sure you want to remove this unit and all\n of its contents from your course?<br><br>This action will remove the gradebook entry for the assigned item, including any existing student grades. You can always re-add this activity from ’Removed content' within 'Resources'.";
        }
        else {
            message = "Are you sure you want to remove this activity from your course?<br><br>This action will remove the gradebook entry for the assigned item, including any existing student grades. You can always re-add this activity from ’Removed content' within 'Resources'.";
        }

        if (item.hasSubmission) {
            message += "<br/><br/>This action will remove gradebook entries for the assigned ";
            message += "content, including existing student grades. <br/><br/>";
            message += "You can always re-add this activity from 'Removed items'.  <br/><br/>";
        }

        var options = {
            modal: true,
            draggable: false,
            closeOnEscape: true,
            width: '500px',
            height: '200px',
            resizable: false,
            autoOpen: false,
            title: 'Remove Activity'
        };
        var tag = $("<div id='px-dialog' class='px-dialog'></div>"); //This tag will the hold the dialog content.
        tag.append("<p class='remove-message'>" + message + "</p><br />");
        tag.append("<input type='button' class='remove' value='Remove' /> <a href='#' class='dialogClose'>Cancel</a>");
        tag.dialog({
            modal: options.modal,
            title: options.title,
            draggable: options.draggable,
            closeOnEscape: options.closeOnEscape,
            width: options.width,
            resizable: options.resizable,
            autoOpen: options.autoOpen,
            close: function() {
                $(this).dialog('destroy').remove();
            }
        });
        PxModal.CenterDialog(tag);

        $('#px-dialog input.remove').die().live("click", function () {
            $(PxPage.switchboard).trigger("onremoveitem", [item]);
            tag.dialog('destroy').remove();
            return false;
        });
        $('#px-dialog a.dialogClose').die().live("click", function () {
            tag.dialog('destroy').remove();
            return false;
        });
    }

    // opens a jqui dialog for edit actions, server call is overloaded to mode
    function showEditContentTitleDialog (sourceId, parentId, title, source, mode) {
        if (title == undefined) {
            title = "";
        }            

        if (parentId == "") {
            title = "Edit title and directions";
        }

        if (parentId == null || parentId.length == 0) {
            parentId = "PX_MULTIPART_LESSONS";
        }

        var node = $.fn.fauxtree("getNode", parentId);
        var level = node.attr("data-ft-level");

        var options = {
            modal: true,
            draggable: false,
            closeOnEscape: true,
            width: $.fn.fauxtree("getNode", sourceId).hasClass('item-type-pxunit') ? '730px' : '530px',
            height: '400px',
            resizable: true,
            autoOpen: false,
            title: title
        };

        var tag = $("<div id='px-dialog-editcontent'></div>"); //This tag will the hold the dialog content.

        if (source == undefined) {
            source = "dialog";
        }

        if ($("#fne-window").is(":visible")) {
            source = "fne";
        }

        var args = {
            contentId: sourceId,
            newParentId: parentId,
            level: level,
            source: source,
            mode: mode
        };

        tag.dialog({
            modal: options.modal,
            title: options.title,
            draggable: options.draggable,
            closeOnEscape: options.closeOnEscape,
            width: options.width,
            resizable: options.resizable,
            autoOpen: options.autoOpen,
            close: function() {
                PxPage.TriggerHtmlSave();
                $(this).dialog('destroy').remove();
            }
        });

        $.ajax({
            type: "GET",
            data: args,
            url: PxPage.Routes.show_faceplate_editcontent,
            success: function (data) {
                tag.html(data);
                PxModal.CenterDialog(tag);
            }
        });
    }

    // called directly from the `Move or Copy` link
    function showMoveCopyDialog (sourceId) {
        var options = {
            modal: true,
            draggable: false,
            closeOnEscape: true,
            width: '700px',
            height: '400px',
            resizable: false,
            autoOpen: false,
            title: 'Move or Copy to...'
        };
        var tag = $("<div id='px-dialog' class='px-dialog movecopy-dialog'></div>"); //This tag will the hold the dialog content.

        var args = {
            sourceId: sourceId
        };

        tag.dialog({
            modal: options.modal,
            title: options.title,
            draggable: options.draggable,
            closeOnEscape: options.closeOnEscape,
            width: options.width,
            resizable: options.resizable,
            autoOpen: options.autoOpen,
            dialogClass: 'dlg-movecopy',
            close: function() {
                $(this).find(".unit-subitems-wrapper").ContentTreeWidget("destroy");
                $(this).dialog('destroy').remove();
            }
        });

        var parentId = $('.faux-tree').children('li[data-ft-id=' + sourceId + ']').prevAll('li[data-ft-level=1]').attr('data-ft-id');

        $.get(PxPage.Routes.show_faceplate_moveorcopy, args, function (response) {
            tag.html(response);
            

            $('#launchpad-widget-PX_LAUNCHPAD_MOVECOPY_WIDGET .faux-tree-node').live('click', function () {
                var tree = $(tag).find('.faux-tree');
                var activeChapter = tree.fauxtree('getNode', $(this));

                if (activeChapter.ftattr('state') == 'closed') {
                    activeChapter.removeClass('active');
                }
                else {
                    activeChapter.addClass('active');
                }
            });

            var node = $(tag).find('.faux-tree').find('li[data-ft-id=' + parentId + ']');
            node.parent().fauxtree('toggleNode', node);
            node.trigger('click');
            PxModal.CenterDialog(tag, 600);
        });
    }

    function ShowValidationMessage (event) {
        var val = event.currentTarget.value;
        var regex = /^[0-9]+$/;
        if (regex.test(val)) {
            $(".points-error-message").hide();
        } else {
            $(".points-error-message").show();
        }
    }

    function moveItem () {
        var sourceId = $("#sourceId").val();
        var sourceSelector = $(".faux-tree-node[data-ft-id='" + sourceId + "']");
        var sourceParentId = $(sourceSelector).ftattr("parent");
        var targetSelector = $(".movecopy-dialog .faux-tree-node.active");
        var targetId = targetSelector.attr("data-ud-id");
        $(".px-dialog").hide();
        $(".movecopy-dialog").dialog("close");
        $(".movecopy-dialog .faux-tree").remove();
        $(PxPage.switchboard).trigger("movenode", [sourceId, targetId, sourceParentId, null]);
    }

    return {
        assign: assign,
        GetSelectedUnit: getSelectedUnit,
        Init: init,
        OnAssign: onAssign,
        ParseDateTime: parseDateTime,
        unAssign: unAssign,
        PopulateAssignmentUnits: populateAssignmentUnits,
        checkForSubmissionAndRemoveItem: checkForSubmissionAndRemoveItem,
        showEditContentTitleDialog: showEditContentTitleDialog,
        showMoveCopyDialog: showMoveCopyDialog,
        moveItem: moveItem,
        ShowValidationMessage: ShowValidationMessage,
        VerifySelectedUnit: verifySelectedUnit
    };

}(jQuery);