var PxInstructorConsoleWidget = function ($) {
    var HTMLSelector = {
        WidgetContainer: "#PX_InstructorConsoleWidget",
        AnnouncementsLink: "#CW_AnnouncementsLink",
        StudentViewLink: "#CW_StudentViewLink",
        settingsSave: "#settingsSave",
        courseSettingsSave: ".settingsSubmit-wrapper #submitForm",
        settingsForm: "#settingsForm",
        CW_ResourcesButton: "#CW_ResourcesButton",
        CW_ReturnButton: "#CW_ReturnButton",
        launchpadItemsLink: "#launchpadItemsLink",
        PX_MENU_ITEM_LAUNCHPAD: "#PX_MENU_ITEM_LAUNCHPAD",
        menuLink: ".menu-link",
        removeCategory: ".removeCategory",
        whatIsPassingScore: "#whatIsPassingScore",
        passingScoreExplanation: "#passingScoreExplanation",
        whatIsWeighted: "#whatIsWeighted",
        weightedExplanation: "#weightedExplanation",
        whatIsCategories: "#whatIsCategories",
        categoriesExplanation: "#categoriesExplanation",
        categoryItems: ".category-items",
        showAssignments: "#showAssignments",
        passingScore: "#passingScore",
        savePassingScore: "#savePassingScore",
        useWeightedCategories: "#useWeightedCategories",
        gradebookCategoriesList: "#gradebookCategoriesList",
        labelName: ".labelName",
        textName: ".textName",
        btnName: ".btnName",
        labelWeight: ".labelWeight",
        textWeight: ".textWeight",
        btnWeight: ".btnWeight",
        labelDrop: ".labelDrop",
        textDrop: ".textDrop",
        btnDrop: ".btnDrop",
        id: "id",
        category: "category",
        sequence: "sequence",
        categorySequence: ".categorySequence",
        itemSequence: ".itemSequence",
        highlightColor: "#CCFFCC",
        activeLaunchpadMenu: ".PX_Menu .parent .menu-item.active .menu-link",
        courseInfoWidget: '.widgetItem.PX_Course_Information',
        widgetEditLink: '.widgetEditLink',
        dueLaterDays: '#DueLaterDays',
        homeButton: "#fne-unblock-action-home",
        textboxCalendarToggle: '.textbox_calendar_toggle',
        calendarToggle: '.calendar_toggle',
        //LMS ID toggles
        lmsShow: '.lms-action-show',
        lmsHide: '.lms-action-hide',
        lmsEdit: '.lms-action-edit',
        lmsSave: '.lms-action-save',
        lmsCancel: '.lms-action-cancel',
        lmsSpanHide: "span.lms-id-hide",
        lmsSpanShow: "span.lms-id-show",
        lmsSpanEdit: "span.lms-id-edit",
        lmsInputId: "input#txtLmsId",
        lmsLabelId: ".lms-id-label"

};
    var _static = {
        launchpadItemsIncluded: undefined,
        removeButton: undefined
    };
    return {
        Init: function (launchpadItemsIncluded) {
            PxPage.OnProductLoaded(function () {
                PxInstructorConsoleWidget.InitRoutes();
            });

            $(HTMLSelector.dueLaterDays).die().live("blur", function () {
                if ($(HTMLSelector.dueLaterDays).val() != null && ($(HTMLSelector.dueLaterDays).val().indexOf(".") != -1 || isNaN(parseInt($(HTMLSelector.dueLaterDays).val())))) {
                    PxPage.Toasts.Warning("Number of 'due later days' must be an integer");
                    $(HTMLSelector.dueLaterDays).val("");
                    return false;
                }
            });

            $(HTMLSelector.homeButton).unbind('click').bind('click', function () {
                PxInstructorConsoleWidget.BindEditButton();
            });

            PxInstructorConsoleWidget.BindLmsId();
            PxInstructorConsoleWidget.BindEditButton();
            PxInstructorConsoleWidget.initGradebookCategoriesList();

            $(HTMLSelector.textboxCalendarToggle).unbind('click').bind('click', function () {                
                $(this).next(HTMLSelector.calendarToggle).click();
            });

            if (launchpadItemsIncluded != undefined) {
                _static.launchpadItemsIncluded = launchpadItemsIncluded.toLowerCase();

                if (_static.launchpadItemsIncluded == 'true') {
                    $(HTMLSelector.launchpadItemsLink).html('Remove these units from your course?');
                }
                else {
                    $(HTMLSelector.launchpadItemsLink).html('Add these units to your course?');
                }
            }

            $(HTMLSelector.launchpadItemsLink).die().live('click', function () {
                if (_static.launchpadItemsIncluded == 'true') {
                    var options = {
                        modal: true,
                        draggable: false,
                        closeOnEscape: true,
                        width: '30%',
                        height: '30%',
                        resizable: false,
                        autoOpen: false,
                        title: 'Are you sure?'
                    };

                    var tag = $("<div id='px-dialog'><ul><li>All contents of the Launch Pad units will still be available in \"Resources\".</li><li>You can always return here to re-add Launch Pad units to your course.</li></ul></div>");

                    tag.dialog({
                        modal: options.modal,
                        title: options.title,
                        dialogClass: 'remove-units-modal',
                        draggable: options.draggable,
                        closeOnEscape: options.closeOnEscape,
                        width: options.width,
                        resizable: options.resizable,
                        autoOpen: options.autoOpen,
                        buttons: {
                            "Remove units": function () {
                                PxInstructorConsoleWidget.setLaunchpadUnits(false);
                                $(this).dialog('destroy').remove();
                            },
                            Cancel: function () {
                                $(this).dialog('destroy').remove();
                            }
                        }
                    }).dialog('open');
                    var cancel = $("#px-dialog").parent().find('button')[1];
                    $(cancel).addClass("cancel");
                }
                else {
                    PxInstructorConsoleWidget.setLaunchpadUnits(true);
                }

                return false;
            });

            $(HTMLSelector.CW_ResourcesButton).die().live("click", function (event) {
                PxPage.LargeFNE.CloseFNE();
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow');
                return false;
            });
            $(HTMLSelector.CW_ReturnButton).die().live("click", function (event) {
                if (!$(this).hasClass("sandbox-inactive"))
                    document.location = PxPage.Routes.start_page;
            });
            $(HTMLSelector.settingsSave).die().live("click", function (event) {
                $(HTMLSelector.settingsForm).submit();
            });

            $(HTMLSelector.courseSettingsSave).die().live("click", function (event) {
                var isValidate = false;
                window.setTimeout(function () {
                    $(".errorMessage").each(function (index) {
                        if ($(this).html() != "") {
                            isValidate = true;
                        }
                    });
                    if (!isValidate) {
                        PxInstructorConsoleWidget.Update();
                        $(HTMLSelector.homeButton).click();
                    }
                }, 1000);

            });

            $(HTMLSelector.AnnouncementsLink).die().live("click", function (event) {
                var options = {
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    width: '50%',
                    height: '50%',
                    resizable: true,
                    autoOpen: false,
                    title: 'Announcements'
                };
                var tag = $("<div id='px-dialog'></div>"); //This tag will the hold the dialog content.

                tag.dialog({
                    modal: options.modal,
                    title: options.title,
                    draggable: options.draggable,
                    closeOnEscape: options.closeOnEscape,
                    width: options.width,
                    resizable: options.resizable,
                    autoOpen: options.autoOpen,
                    close: function () {
                        $(this).dialog('destroy').remove();
                    }
                }).dialog('open');

                tag.load(PxPage.Routes.AnnouncementsWidget, null, function (data, textStatus, XMLHttpRequest) {
                    PxFacePlate.AddInstructorConsoleBreadcrumb();
                    tag.dialog("option", "position", "center");
                });
            });

            $('#fromDueDateCal').die().live("click", function (event) {
                event.stopImmediatePropagation();

                var args = {
                    callback: PxInstructorConsoleWidget.setFromDate,
                    customValues: {},
                    calendarMode: 'single',
                    oldStartDate: '',
                    oldDueDate: ''
                };

                PxPage.ShowDatePicker(args);
                $("#PXAssignWithoutDueDate").attr("style", "display:none;");
            });

            $('#toDueDateCal').die().live("click", function (event) {
                event.stopImmediatePropagation();

                var args = {
                    callback: PxInstructorConsoleWidget.setToDate,
                    customValues: {},
                    calendarMode: 'single',
                    oldStartDate: '',
                    oldDueDate: '',
                    customTitle: "Please choose end date"
                };

                PxPage.ShowDatePicker(args);
                $("#PXAssignWithoutDueDate").attr("style", "display:none;");
            });

            $('#newDueDateCal').die().live("click", function (event) {
                event.stopImmediatePropagation();

                var args = {
                    callback: PxInstructorConsoleWidget.setNewDate,
                    customValues: {},
                    calendarMode: 'single',
                    oldStartDate: '',
                    oldDueDate: '',
                    customTitle: "Please choose new date"
                };

                PxPage.ShowDatePicker(args);
                $("#PXAssignWithoutDueDate").attr("style", "display:none;");
            });

            $('#btnBatchDueDateUpdate').die().live("click", function (event) {
                event.stopImmediatePropagation();
                PxInstructorConsoleWidget.batchDueDateUpdate();
            });
        },

        batchDueDateUpdate: function () {
            if ($(".dueDate #fromDate").val() == null || $(".dueDate #fromDate").val() == "") {
                PxPage.Toasts.Warning("Please select the from date...");
                return;
            }
            else if ($(".dueDate #toDate").val() == null || $(".dueDate #toDate").val() == "") {
                PxPage.Toasts.Warning("Please select the to date...");
                return;
            }
            else if ($(".dueDate #newDueDate").val() == null || $(".dueDate #newDueDate").val() == "") {
                PxPage.Toasts.Warning("Please select the new date...");
                return;
            }

            PxPage.Loading("fne-content");

            var args = {
                fromDate: $(".dueDate #fromDate").val(),
                toDate: $(".dueDate #toDate").val(),
                updateRestrictedDates: $('#updateRestrictedDates').prop('checked'),
                newDate: $("#newDueDate").val()
            };

            $.post(PxPage.Routes.InstructorConsole_BatchDueDateUpdate, args, function (response) {
                $(HTMLSelector.PX_MENU_ITEM_LAUNCHPAD).find(HTMLSelector.menuLink).trigger('click');

                PxPage.Toasts.Info(response.message);

                if (response.status == "success") {
                    $("#totalItemsdiv").hide();
                    $(".dueDate #fromDate").val("");
                    $(".dueDate #toDate").val("");
                    $("#newDueDate").val("");
                    $("#totalDaysShifted").text("");
                }

                PxPage.Loaded("fne-content");
            });
        },

        initGradebookCategoriesList: function () {
            $(document).not(HTMLSelector.btnDrop).unbind('click').bind('click', function (event) {
                if ($(event.target).attr('id') != $(HTMLSelector.passingScore).attr('id') && $(event.target).attr('id') != $(HTMLSelector.savePassingScore).attr('id')) {
                    $(HTMLSelector.savePassingScore).hide();
                }

                PxInstructorConsoleWidget.toggleViewEdit(event.target, HTMLSelector.labelName, HTMLSelector.textName, HTMLSelector.btnName);
                PxInstructorConsoleWidget.toggleViewEdit(event.target, HTMLSelector.labelDrop, HTMLSelector.textDrop, HTMLSelector.btnDrop);
                PxInstructorConsoleWidget.toggleViewEdit(event.target, HTMLSelector.labelWeight, HTMLSelector.textWeight, HTMLSelector.btnWeight);
            });

            $(HTMLSelector.categorySequence).die().live('change', function () {
                PxInstructorConsoleWidget.changeSequence($(this), 'categorySequence_', HTMLSelector.categorySequence, PxPage.Routes.InstructorConsole_MoveGradebookCategory);
            });

            $(HTMLSelector.itemSequence).die().live('change', function () {
                PxInstructorConsoleWidget.changeSequence($(this), 'itemSequence_', HTMLSelector.itemSequence, PxPage.Routes.InstructorConsole_MoveGradebookItem);
            });

            PxInstructorConsoleWidget.toggleWhatIsThis(HTMLSelector.whatIsPassingScore, HTMLSelector.passingScoreExplanation);
            PxInstructorConsoleWidget.toggleWhatIsThis(HTMLSelector.whatIsWeighted, HTMLSelector.weightedExplanation);
            PxInstructorConsoleWidget.toggleWhatIsThis(HTMLSelector.whatIsCategories, HTMLSelector.categoriesExplanation);

            $(HTMLSelector.showAssignments).die().live('click', function (event) {
                event.stopImmediatePropagation();

                PxPage.Loading("fne-content");

                if ($(HTMLSelector.categoryItems).css('display') == 'none') {
                    $(HTMLSelector.categoryItems).show();
                    $(this).text('Hide Assignments');
                }
                else {
                    $(HTMLSelector.categoryItems).hide();
                    $(this).text('Show Assignments');
                }

                PxPage.Loaded("fne-content");
            });

            $(HTMLSelector.removeCategory).unbind('click').bind('click', function () {
                _static.button = $(this);
                var tag = $("<div id='px-dialog' title='Remove Category'>Are you sure you want to remove this gradebook category?<br/>All items in the category will be moved to the \"Uncategorized\" category.</div>");

                tag.dialog({
                    resizable: false,
                    height: 250,
                    width: 500,
                    modal: true,
                    dialogClass: '',
                    buttons:
                [{
                    text: "Remove",
                    click: function () {
                        $(this).dialog("destroy").remove();

                        var categoryId = _static.button.parents('tr').find(HTMLSelector.categorySequence).attr('id').replace('categorySequence_', '');

                        var state = {
                            EntityId: categoryId
                        };

                        PxInstructorConsoleWidget.updateCategoryListbyAjax(PxPage.Routes.InstructorConsole_RemoveGradebookCategory, { state: state });
                    }
                },
                {
                    text: "Cancel",
                    click: function () {
                        $(this).dialog("destroy").remove();
                    }
                }]
                });
            });

            $(HTMLSelector.passingScore).unbind('focus').bind('focus', function () {
                $(HTMLSelector.savePassingScore).show();
            });

            $(HTMLSelector.savePassingScore).unbind('click').bind('click', function () {
                $(HTMLSelector.savePassingScore).hide();

                var passingScore = $(HTMLSelector.passingScore).val();

                if (isNaN(passingScore) || passingScore < 0 || passingScore > 100) {
                    PxPage.Toasts.Error("Please provide a valid value between 0 and 100!");
                    $(HTMLSelector.passingScore).focus();
                    return false;
                }
                else {
                    PxPage.Loading("fne-content");

                    $.ajax({
                        type: 'POST',
                        url: PxPage.Routes.InstructorConsole_UpdatePassingScore,
                        data: { passingScore: passingScore },
                        success: function (response) {
                            PxPage.Loaded("fne-content");
                            if (response.status == "success") {

                            }
                            else {
                                PxPage.Toasts.Error(response.message);
                            }
                        },
                        error: function (err) {
                            PxPage.Loaded("fne-content");
                            PxPage.Toasts.Error("Passing Score update failed!");
                        },
                        dataType: "JSON"
                    });
                }
            });

            $(HTMLSelector.useWeightedCategories).die().live('change', function () {
                var useIt = $(this).is(':checked');

                var state = {
                    UseWeighted: useIt
                };

                PxInstructorConsoleWidget.updateCategoryListbyAjax(PxPage.Routes.InstructorConsole_UpdateUseWeightedCategories, { state: state });
            });

            PxInstructorConsoleWidget.bindEditControls(HTMLSelector.labelName, HTMLSelector.textName, HTMLSelector.btnName, PxPage.Routes.InstructorConsole_RenameGradebookCategory, "Renaming category failed!");
            PxInstructorConsoleWidget.bindEditControls(HTMLSelector.labelWeight, HTMLSelector.textWeight, HTMLSelector.btnWeight, PxPage.Routes.InstructorConsole_UpdateGradebookCategoryWeight, "Updating category weight failed!");
            PxInstructorConsoleWidget.bindEditControls(HTMLSelector.labelDrop, HTMLSelector.textDrop, HTMLSelector.btnDrop, PxPage.Routes.InstructorConsole_UpdateGradebookCategoryDropLowest, "Updating category drop lowest failed!");
        },

        changeSequence: function (entity, prefix, selector, url) {
            var entityId = entity.attr(HTMLSelector.id).replace(prefix, '');
            var entitySequence = entity.attr(HTMLSelector.sequence);
            var entitySequenceNumber = entity.find('option:selected').text();
            var aboveId = "";
            var aboveSequence = "";
            var belowId = "";
            var belowSequence = "";

            $(selector).find('option:selected').each(function () {
                var current = $(this);

                if (current.text() == entitySequenceNumber &&
                    current.parent().attr('id') != entity.attr(HTMLSelector.id) &&
                    current.parent().attr(HTMLSelector.category) == entity.attr(HTMLSelector.category)) {

                    if (current.parent().attr(HTMLSelector.sequence) > entitySequence) {
                        aboveId = current.parent().attr(HTMLSelector.id);
                        aboveSequence = current.parent().attr(HTMLSelector.sequence);

                        if (selector == HTMLSelector.categorySequence) {
                            belowId = current.parent().parent().parent().nextAll().find(selector).attr(HTMLSelector.id);
                            belowSequence = current.parent().parent().parent().nextAll().find(selector).attr(HTMLSelector.sequence);
                        }
                        else {
                            belowId = current.parent().parent().parent().next().find('[category=' + entity.attr(HTMLSelector.category) + ']').attr(HTMLSelector.id);
                            belowSequence = current.parent().parent().parent().next().find('[category=' + entity.attr(HTMLSelector.category) + ']').attr(HTMLSelector.sequence);
                        }
                    }
                    else {
                        belowId = current.parent().attr(HTMLSelector.id);
                        belowSequence = current.parent().attr(HTMLSelector.sequence);

                        if (selector == HTMLSelector.categorySequence) {
                            aboveId = current.parent().parent().parent().prevAll().find(selector).attr(HTMLSelector.id);
                            aboveSequence = current.parent().parent().parent().prevAll().find(selector).attr(HTMLSelector.sequence);
                        }
                        else {
                            aboveId = current.parent().parent().parent().prev().find('[category=' + entity.attr(HTMLSelector.category) + ']').attr(HTMLSelector.id);
                            aboveSequence = current.parent().parent().parent().prev().find('[category=' + entity.attr(HTMLSelector.category) + ']').attr(HTMLSelector.sequence);
                        }
                    }
                }
            });

            if (aboveId == undefined) {
                aboveId = "";
            }
            else {
                aboveId = aboveId.replace('itemSequence_', '');
            }

            if (aboveSequence == undefined) {
                aboveSequence = "";
            }

            if (belowId == undefined) {
                belowId = "";
            }
            else {
                belowId = belowId.replace('itemSequence_', '');
            }

            if (belowSequence == undefined) {
                belowSequence = "";
            }

            var itemsIds = [];
            $('[category="' + entity.attr(HTMLSelector.category) + '"]').each(function () {
                itemsIds.push($(this).attr('id').replace('itemSequence_', ''));
            });

            var state = {
                EntityId: entityId,
                AboveId: aboveId,
                AboveSequence: aboveSequence,
                BelowId: belowId,
                BelowSequence: belowSequence,
                ItemIdList: itemsIds
            };

            PxInstructorConsoleWidget.updateCategoryListbyAjax(url, { state: state });
        },

        toggleViewEdit: function (target, labelClass, textClass, btnClass) {
            if (!$(target).hasClass(labelClass.replace('.', '')) && !$(target).hasClass(textClass.replace('.', ''))) {
                $(textClass).hide();
                $(btnClass).hide();
                $(labelClass).show();
            }
        },

        toggleWhatIsThis: function (link, div) {
            $(link).die().live('click', function (event) {
                event.stopImmediatePropagation();

                if ($(div).css('display') == 'none') {
                    $(div).show();
                }
                else {
                    $(div).hide();
                }
            });
        },

        bindEditControls: function (labelClass, textClass, btnClass, url, failedMessage) {
            $(labelClass).unbind('mouseover').bind('mouseover', function () {
                $(this).parent().css('background-color', HTMLSelector.highlightColor);
            });
            $(labelClass).unbind('mouseout').bind('mouseout', function () {
                $(this).parent().css('background-color', 'transparent');
            });
            $(labelClass).unbind('click').bind('click', function () {
                if ($(HTMLSelector.btnWeight + ':visible').length > 0) {
                    var categoryId = $(HTMLSelector.btnWeight + ':visible').parent().parent().find('select').attr('id').replace('categorySequence_', '');
                    var newValue = $(HTMLSelector.btnWeight + ':visible').prev().val();

                    var id = $(this).attr('id');

                    var state = {
                        EntityId: categoryId, 
                        NewValue: newValue
                    };

                    PxInstructorConsoleWidget.updateCategoryListbyAjax(PxPage.Routes.InstructorConsole_UpdateGradebookCategoryWeight, { state: state },
                        function () {
                            var elem = $('#' + id);

                            elem.hide();
                            elem.next(textClass).val(elem.text());
                            elem.next(textClass).show();
                            elem.nextAll(btnClass).show();
                        }
                    );
                }
                else if ($(HTMLSelector.btnDrop + ':visible').length > 0) {
                    $(HTMLSelector.btnDrop + ':visible').click();
                }
                else {
                    $(labelClass).show();
                    $(textClass).hide();
                    $(btnClass).hide();

                    $(this).hide();
                    $(this).next(textClass).val($(this).text());
                    $(this).next(textClass).show();
                    $(this).nextAll(btnClass).show();
                }
            });

            $(btnClass).die().live('click', function () {
                var elem = $(this);

                var categoryId = elem.parent().parent().find('select').attr('id').replace('categorySequence_', '');
                var newValue = elem.parent().find(textClass).val();

                if (textClass != HTMLSelector.textName && (isNaN(newValue) || newValue < 0)) {
                    PxPage.Toasts.Error("Please provide valid value!");
                    elem.prev(textClass).focus();
                    return false;
                }

                PxPage.Loading("fne-content");

                if (labelClass == HTMLSelector.labelWeight) {
                    var state = {
                        EntityId: categoryId,
                        NewValue: newValue
                    };

                    PxInstructorConsoleWidget.updateCategoryListbyAjax(url, { state: state });
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: { categoryId: categoryId, newValue: newValue },
                        success: function (response) {
                            PxPage.Loaded("fne-content");
                            if (response.status == "success") {
                                elem.prevAll(labelClass).text(elem.prevAll(textClass).val());
                                elem.prevAll(labelClass).show();
                                elem.prevAll(textClass).hide();
                                elem.hide();
                            }
                            else {
                                PxPage.Toasts.Error(response.message);
                            }
                        },
                        error: function (err) {
                            PxPage.Loaded("fne-content");
                            PxPage.Toasts.Error(failedMessage);
                        },
                        dataType: "JSON"
                    });
                }
            });
        },

        updateCategoryListbyAjax: function (url, data, callback) {
            var isCollapsed = $(HTMLSelector.categoryItems).css('display') == 'none';

            PxPage.Loading("instructor-console-wrapper");

            $.ajax({
                type: 'POST',
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                success: function (response) {
                    $(HTMLSelector.gradebookCategoriesList).html(response);
                    PxInstructorConsoleWidget.initGradebookCategoriesList();

                    if (!isCollapsed) {
                        $(HTMLSelector.showAssignments).trigger('click');
                    }

                    if (callback != undefined) {
                        callback();
                    }

                    PxPage.Loaded("instructor-console-wrapper");
                },
                dataType: "html"
            });
        },

        setLaunchpadUnits: function (include) {
            PxPage.Loading("fne-content");

            var args = {
                include: include
            };

            $.get(PxPage.Routes.InstructorConsole_SetLaunchpadUnits, args, function (response) {
                if (response.status == "success") {
                    $(HTMLSelector.PX_MENU_ITEM_LAUNCHPAD).find(HTMLSelector.menuLink).trigger('click');

                    if (include) {
                        PxPage.Toasts.Success('Default units successfully added to your course!');
                        _static.launchpadItemsIncluded = 'true';
                    }
                    else {
                        PxPage.Toasts.Success('Default units successfully removed from your course!');
                        _static.launchpadItemsIncluded = 'false';
                    }
                }
                else {
                    PxPage.Toasts.Error('Operation failed!');
                }

                PxPage.Loaded("fne-content");
            });
        },

        setFromDate: function (dateArgs) {
            var curDate = new Date();
            var strDate = dateArgs.DueDate ? (new Date(dateArgs.DueDate)).format("mm/dd/yyyy") : curDate.format("mm/dd/yyyy");
            if ($(".dueDate #toDate").val() != null && $(".dueDate #toDate").val() != "") {
                var toDate = new Date($(".dueDate #toDate").val()).format("mm/dd/yyyy");
                if (new Date(strDate) > new Date(toDate)) {
                    PxPage.Toasts.Warning("From Date can't be greater than To Date");
                    return;
                }
            }
            $(".dueDate #fromDate").val(strDate);
            if ($(".dueDate #fromDate").val() != null && $(".dueDate #fromDate").val() != "" && $(".dueDate #toDate").val() != null && $(".dueDate #toDate").val() != "") {
                var args = {
                    fromDate: $(".dueDate #fromDate").val(),
                    toDate: $(".dueDate #toDate").val()
                };

                PxPage.Loading("fne-content");

                $.post(PxPage.Routes.InstructorConsole_GetBatchDueDateItemCount, args, function (response) {
                    var strAssignment = " assignment";
                    if (response.itemCount > 1) {
                        strAssignment += "s";
                    }
                    var strMessage = "You have " + response.itemCount + strAssignment + " due between " + response.fromDate + " and " + response.toDate + ".";
                    $("#totalItems").text(strMessage);
                    $("#totalItemsdiv").show();

                    PxPage.Loaded("fne-content");
                }
                );
            }

            PxInstructorConsoleWidget.displayShiftDays();
        },

        setToDate: function (dateArgs) {
            var curDate = new Date();
            var strDate = dateArgs.DueDate ? (new Date(dateArgs.DueDate)).format("mm/dd/yyyy") : curDate.format("mm/dd/yyyy");
            if ($(".dueDate #fromDate").val() != null && $(".dueDate #fromDate").val() != "") {
                var fromDate = new Date($(".dueDate #fromDate").val()).format("mm/dd/yyyy");
                if (new Date(strDate) < new Date(fromDate)) {
                    PxPage.Toasts.Warning("To Date can't be less than From Date");
                    return;
                }
            }

            $(".dueDate #toDate").val(strDate);
            if ($(".dueDate #fromDate").val() != null && $(".dueDate #fromDate").val() != "" && $(".dueDate #toDate").val() != null && $(".dueDate #toDate").val() != "") {
                var args = {
                    fromDate: $(".dueDate #fromDate").val(),
                    toDate: $(".dueDate #toDate").val()
                };

                PxPage.Loading("fne-content");

                $.post(PxPage.Routes.InstructorConsole_GetBatchDueDateItemCount, args, function (response) {
                    var strAssignment = " assignment";
                    if (response.itemCount > 1) {
                        strAssignment += "s";
                    }
                    var strMessage = "You have " + response.itemCount + strAssignment + " due between " + response.fromDate + " and " + response.toDate + ".";
                    $("#totalItems").text(strMessage);
                    $("#totalItemsdiv").show();

                    PxPage.Loaded("fne-content");
                }
                );
            }
        },

        setNewDate: function (dateArgs) {
            var curDate = new Date();
            var strDate = dateArgs.DueDate ? (new Date(dateArgs.DueDate)).format("mm/dd/yyyy") : curDate.format("mm/dd/yyyy");
            $(".dueDate #newDueDate").val(strDate);

            PxInstructorConsoleWidget.displayShiftDays();
        },

        displayShiftDays: function () {
            if ($(".dueDate #fromDate").val() != null && $(".dueDate #fromDate").val() != "" && $(".dueDate #newDueDate").val() != null && $(".dueDate #newDueDate").val() != "") {
                var start = $(".dueDate #fromDate").val();
                var end = $(".dueDate #newDueDate").val();
                var diff = new Date(end) - new Date(start);
                var days = Math.round(diff / 1000 / 60 / 60 / 24);
                var strDay = " day";

                if (days != -1 && days != 0 && days && 1) {
                    strDay += "s";
                }

                var strPlus = "";

                if (days > 0) {
                    strPlus = "+";
                }

                var strMessage = "All selected due dates will be shifted  " + strPlus + days + " " + strDay + ".";
                $("#totalDaysShifted").text(strMessage);
            }
        },

        ViewResources: function (mode, edit) {
            if (mode == "all-chapters") {
                if ($(".instructor-console-resource-link .sub-icon.all-chapters").hasClass("expanded")) {
                    $(".instructor-console-resource-link .sub-icon.all-chapters").removeClass("expanded");
                } else {
                    $(".instructor-console-resource-link .sub-icon.all-chapters").addClass("expanded");
                }
                $(".chapters-result").toggle();
                return;
            } else if (mode == "all-types") {
                if ($(".instructor-console-resource-link .sub-icon.all-types").hasClass("expanded")) {
                    $(".instructor-console-resource-link .sub-icon.all-types").removeClass("expanded");
                } else {
                    $(".instructor-console-resource-link .sub-icon.all-types").addClass("expanded");
                }
                $(".types-result").toggle();
                return;
            }
            else if (mode == "ebook") {
                if ($(".instructor-console-resource-link .sub-icon.ebook").hasClass("expanded")) {
                    $(".instructor-console-resource-link .sub-icon.ebook").removeClass("expanded");
                } else {
                    $(".instructor-console-resource-link .sub-icon.ebook").addClass("expanded");
                }
                $(".ebook-result").toggle();
                return;
            }
        },
        OpenBrowseMoreResources: function (mode, keyword) {
            if (mode == "chapter") {
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "chapter", keyword);
            } else if (mode == "type") {
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "type", keyword);
            } else if (mode == "chaptertype") {
                if ($(".types-result").is(":visible")) {
                    $(HTMLSelector.homeButton).click();
                    $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "type", keyword);
                } else {
                    $(HTMLSelector.homeButton).click();
                    $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "chapter", keyword);
                }
            } else if (mode == "myresources") {
                $(HTMLSelector.homeButton).click();
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "myresources");
            } else if (mode == "ebook") {
                $(HTMLSelector.homeButton).click();
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', "ebook");
            } else {
                $(HTMLSelector.homeButton).click();
                $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', mode, keyword);
            }
        },
        OpenBrowseMoreRss: function (mode) {
            $.fn.FacePlateBrowseMoreResources('showRssFeedsInshowMoreResourcesWindow');
        },
        Update: function () {
            $('#PX_InstructorConsoleWidget').load(PxPage.Routes.update_instructor_console, this.BindEditButton);
            window.location.hash = "#state/instructorconsole";
        },
        ReloadLaunchPad: function () {
            $(HTMLSelector.activeLaunchpadMenu).click();
        },
        BindEditButton: function () {
            $(HTMLSelector.courseInfoWidget).find(HTMLSelector.widgetEditLink).die();
            $(HTMLSelector.courseInfoWidget).find(HTMLSelector.widgetEditLink).unbind();
            $(HTMLSelector.courseInfoWidget).find(HTMLSelector.widgetEditLink).attr('href', '#state/instructorconsole/general');
            $(HTMLSelector.courseInfoWidget).find(HTMLSelector.widgetEditLink).bind('click', function () {
                window.location.hash = "#state/instructorconsole/general";
                return false;
            });
        },
        BindLmsId: function () {
            if ($(HTMLSelector.lmsSpanShow).length) {
                var resetLmsHide = function() {
                    $(HTMLSelector.lmsSpanShow).hide();
                    $(HTMLSelector.lmsSpanEdit).hide();
                    $(HTMLSelector.lmsSpanHide).show();
                };
                var resetLmsShow = function() {
                    $(HTMLSelector.lmsSpanShow).show();
                    $(HTMLSelector.lmsSpanEdit).hide();
                    $(HTMLSelector.lmsSpanHide).hide();
                };
                $(HTMLSelector.lmsShow).die().live('click', resetLmsShow);
                $(HTMLSelector.lmsHide).die().live('click', resetLmsHide);
                $(HTMLSelector.lmsCancel).die().live('click', resetLmsShow);
                $(HTMLSelector.lmsEdit).die().live('click', function() {
                    $(HTMLSelector.lmsSpanShow).hide();
                    $(HTMLSelector.lmsSpanEdit).show();
                    $(HTMLSelector.lmsInputId).val($(HTMLSelector.lmsLabelId).text());
                    $(HTMLSelector.lmsInputId).focus();
                });
                $(HTMLSelector.lmsSave).die().live('click', function() {
                    var lmsid = $(HTMLSelector.lmsInputId).val();
                    if (lmsid != null && lmsid.length) {
                        $(HTMLSelector.lmsSave).attr('disabled', true);
                        $.post(
                            PxPage.Routes.update_lms_id,
                            { lmsId: lmsid },
                            function(response) {
                                if (response.success) {
                                    $(HTMLSelector.lmsLabelId).text(lmsid);
                                    $(HTMLSelector.lmsSave).attr('disabled', false);
                                    PxPage.Toasts.Success("LMS ID changes saved successfully");
                                    resetLmsShow();
                                }
                            });
                    } else {
                        PxPage.Toasts.Warning("Please enter a valid Campus LMS ID");
                    }
                });
                $(HTMLSelector.lmsInputId).die().live('keyup', function() {
                    var lmsid = $(HTMLSelector.lmsInputId).val();
                    if (lmsid.length == 0) {
                        $(HTMLSelector.lmsSave).attr('disabled', true);
                    } else {
                        $(HTMLSelector.lmsSave).attr('disabled', false);
                    }
                });
                $(PxPage.switchboard).rebind("fneprep", resetLmsHide);
            }
        },
        InitRoutes: function () {

            PxRoutes.AddComponentRoute('instructorconsole', ':path:', function (state, path) {
                var isSandbox = $(this).hasClass("sandbox-inactive");

                switch (path) {
                    case "undefined":
                        if (!isSandbox) {
                            $(PxPage.switchboard).trigger("OpenInstructorConsole");
                        }
                        break;

                    case "gradebook":
                        if (!isSandbox) {
                            $(PxPage.switchboard).trigger("OpenGradeBookWindow");
                        }
                        break;

                    case "gradebookpref":
                        $(PxPage.switchboard).trigger("OpenGradeBookPrefWindow");
                        break;

                    case "batchupdater":
                        $(PxPage.switchboard).trigger("OpenBatchUpdater");
                        break;

                    case "rosterandgroups":
                        if (!isSandbox) {
                            $(PxPage.switchboard).trigger("OpenManageGroupsWindow");
                        }
                        break;

                    default:
                        $(PxPage.switchboard).trigger("OpenInstructorConsole", path);
                        break;

                }



            });
        }
    };
} (jQuery);