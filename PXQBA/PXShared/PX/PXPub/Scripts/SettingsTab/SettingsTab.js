var TemplateItems;
var TemplateSaveAs;
var TemplateResponseIdOld;
var TemplateResponseIdNew;
var TemplateProgateUpdates;
var TemplateRegularSave;
var PxSettingsTab = function ($) {
    var _static = {
        defaults: {
            context: '#assignment-settings'
        },
        fn : {
            checkReviewSettings: function () {
                // Enable all radio buttons
                $('#review-setting-show-questions-answers input[type="radio"]').prop('disabled', false);
                
                // Disable radio buttons that will not be valid in combination
                var showQuestionRadio = $('#review-setting-show-questions-answers input[type="radio"]:checked');
                // If not showing question and answer, disable the following options
                // 1) Show whether answers were right/wrong after...
                // 2) Show question score after...
                // 3) Show correct answers after...
                // 4) Show solutions after...
                if (showQuestionRadio.val() === 'DueDate') {
                    var effectedOptions = $('.review-setting').not('#review-setting-show-questions-answers, #review-setting-show-feedback-remarks');
                    effectedOptions.find('input[type="radio"].setting-header-each').prop('disabled', true);
                    effectedOptions.find('input[type="radio"].setting-header-due-date').prop('disabled', false);
                } else if (showQuestionRadio.val() === 'Never') {
                    $('.review-setting').not('#review-setting-show-questions-answers, #review-setting-show-feedback-remarks').find('input[type="radio"].setting-header-each, input[type="radio"].setting-header-due-date').prop('disabled', true);
                } else {
                    $('.review-setting input[type="radio"]').prop('disabled', false);
                }
                // If not showing whether answers were right/wrong , disable the following options
                // 2) Show question score after...
                // 3) Show correct answers after...
                // 4) Show solutions after...
                var showCorrectQuestion = $('#review-setting-show-right-wrong input[type="radio"]:checked');
                if (showCorrectQuestion.length && !showCorrectQuestion.prop('disabled')) {
                    if (showCorrectQuestion.val() === 'DueDate') {
                        $('.review-setting').not('#review-setting-show-questions-answers, #review-setting-show-right-wrong, #review-setting-show-feedback-remarks').find('input[type="radio"].setting-header-each').prop('disabled', true);
                    } else if (showCorrectQuestion.val() === 'Never') {
                        $('.review-setting').not('#review-setting-show-questions-answers, #review-setting-show-right-wrong, #review-setting-show-feedback-remarks').find('input[type="radio"].setting-header-each, input[type="radio"].setting-header-due-date').prop('disabled', true);
                    } else {
                        $('.review-setting input[type="radio"]').not('#review-setting-show-questions-answers').prop('disabled', false);
                    }
                }
                // If not showing correct answer , disable the show score option.
                var showCorrectAnswer = $('#review-setting-show-answers input[type="radio"]:checked');
                if (showCorrectAnswer.length && !showCorrectAnswer.prop('disabled')) {
                    if (showCorrectAnswer.val() === 'DueDate') {
                        $('#review-setting-Show-score-after input[type="radio"].setting-header-each').prop('disabled', true);
                    } else if (showCorrectAnswer.val() === 'Never') {
                        $('#review-setting-Show-score-after').find('input[type="radio"].setting-header-each, input[type="radio"].setting-header-due-date').prop('disabled', true);
                    } else {
                        $('#review-setting-Show-score-after input[type="radio"]').prop('disabled', false);
                    }
                }
                // If disabled radio button is checked, shift to the next one
                $('.review-setting input[type="radio"]:disabled:checked').each(function() {
                    $(this).prop('checked', false);
                    $(this).closest('.review-setting').find('input[type="radio"]').not(':disabled').first().prop('checked', true);
                });

            }
        }
    };
    return {
        GetTemplateList: function (itemType) {
            PxTemplates.GetRelatedTemplates(PxTemplates.TemplateContexts.Default, itemType, function (templates) {

                var data = [];
                var selectedItem = "";
                $(templates).each(function (i, v) {
                    if (v.Selected) selectedItem = "selected";
                    data.push("<option " + selectedItem + " value = " + v.Id + ">" + v.Title + "</option>");

                });
                var tplList = $("#ddlTemplateList");
                tplList.append("<option value='current'>Current</option>");
                tplList.append("<option disabled='disabled'>---------------------------</option>");
                tplList.append(data.join(""));
                tplList.append("<option disabled='disabled'>---------------------------</option>");
                tplList.append("<option value='TemplateManagement'>Template Management</option>");
            });
        },

        ReplaceString: function (url, groupId) {
            if (url != null) {
                var start = url.indexOf('GroupId');
                var end = url.indexOf('&ShowOnly');
                var value = url.substring(start, end);
                return url.replace(value, "GroupId=" + groupId);
            }
        },

        OnChangeTemplateList: function (event) {
            var selected = $('#ddlTemplateList').val();
            if (selected == "TemplateManagement") {
                event.href = PxPage.Routes.template_management + '?context=' + PxTemplates.TemplateContexts.Default.value;
                event.preventDefault = function () { };
                event.title = "Template Management";
                PxPage.OpenFneLink(event);
            } else {
                if (confirm("Would you like to apply to the template?")) {
                    // If one of the templates was selected, then get its id and
                    // make a call to copy its settings to the current item.
                    $.ajax({
                        url: PxPage.Routes.copy_item_settings,
                        type: "POST",
                        data: {
                            fromId: selected,
                            toId: $("input.item-id").val()
                        },
                        success: function () {
                            //                            var url = $('.settings-container .bh-component iframe').attr('src');
                            //                            $(".settings-container .bh-component iframe").remove();
                            //                            $(".settings-container .bh-component").attr('rel', url);
                            //                            PxPage.SetFrameApiHooks();
                            //                            $(PxPage.switchboard).trigger("contentcreated", $("input.item-id").val());

                            PxSettingsTab.UpdateQuizSettings("EntireClass");
                            var node = $("li#" + itemId);
                            var instance = $("div#toc");
                            $(PxPage.switchboard).trigger("reloadparent", [instance, node]);
                        }
                    });
                }
            }
        },

        ShowGearBox: function () {
            var templateParentId = $(".template-management #templateParentId").val();
            var options = "";
            if (templateParentId == '') {
                options = [{ name: 'edit', text: 'Edit Title/Description'}];
            } else {
                options = [{ name: 'delete', text: 'Delete' }, { name: 'edit', text: 'Edit Title/Description'}];
            }
            $('.gear-box').ActionWidget({
                menu: {
                    id: 'select-gearbox',
                    options: options
                },
                action: PxSettingsTab.SelectGearboxAction
            });
        },

        TemplateItemSelected: function (event) {
            var itemId = $(event.target).attr("itemid");
            var tempTitle = $(event.target).attr("title");
            var templateParentId = $(event.target).attr("templateparentid");
            $(".template-management #templateParentId").val(templateParentId);
            // rather than create a new template, load the existing one
            var newargs = {
                itemId: itemId
            };
            $.post(PxPage.Routes.template_management_details, newargs, function (response) {
                $(".template-management-details").html(response);
                $('.TemplateTitle').text(tempTitle);
                // Store the new ItemID, which will be used in SaveAs
                var newItemid = $("#divSaveTemplateAs #newItemId").val();
                $("#divSaveTemplateAs #newItemId").val(itemId);
                PxPage.SetFrameApiHooks();
            });
            // Highlight the selected item
            $(".template-management-list li").removeClass("selected");
            $(event.target).addClass("selected");
            PxSettingsTab.ShowGearBox();
        },

        SelectGearboxAction: function (event, name, gearbox) {
            var itemid = $(".template-management-list li.selected").attr('itemid');
            var templateParentId = $(".template-management #templateParentId").val();

            if (templateParentId == '') {
                $('#select-gearbox .delete').attr('disabled', true);
            }

            switch (name) {
                case "delete":
                    $("#delete-confirm").dialog('open');
                    break;
                case "edit":
                    PxSettingsTab.OpenEdit();
                    break;
                case "revert":
                    alert('Not yet implemented');
                    break;
            }
        },

        CheckSuccess: function () {
            //if there is a successful save, close the popup window
            if ($('.edit-link').text() == "") {
                ContentWidget.ContentCreated(null);
                // PxSettingsTab.ReplaceButton();
            } else {
                $('#fne-window').unblock();
                //Reload the page template Management window
                PxSettingsTab.UpdateTemplateManagement();
            }
        },

        UpdateTemplateManagement: function () {
            $(".template-management").html('');
            $(".template-management").load(
                PxPage.Routes.template_management, {
                    context: PxTemplates.TemplateContexts.Default.value
                }, function () {
                    PxSettingsTab.LoadTemplateFunction();
                    PxPage.Loaded('divSaveTemplateAs');
                    $('#fne-window').unblock();
                });
        },

        SaveClick: function (onClickValue) {
            var itemId = $("#divSaveTemplateAs #newItemId").val();
            var contentTitle = $("#txtTemplateName").val();
            var contentDesc = tinyMCE.activeEditor.getContent();
            if (contentTitle != '') {
                var args = {
                    itemId: itemId,
                    contentTitle: contentTitle,
                    contentDesc: contentDesc
                };
                //swap the templates
                $.post(PxPage.Routes.template_edits, args, function (response) {
                    PxSettingsTab.UpdateTemplateManagement();
                    // clear contents of save div
                    $("#divSaveTemplateAs").html('');
                    //Reload the template dropdown
                    $('#settingswrapper #ddlTemplateList').html('');
                    PxSettingsTab.GetTemplateList($("input.item-id").val());
                });
            } else {
                $('#divSaveTemplateAs #spnNameError').show();
                return false;
            }
        },

        AddIndividual: function () {
            var studentName = $('.studentName').val();
            var studentId = $('.studentId').val();

            if (studentId == "") {
                $('.errorMessage').text('The user ' + studentName + ' is not enrolled in this course.');
            } else {
                if (($('#ddlSettingsList').find(":contains('" + studentName + "')").length > 0)) {
                    $('.errorMessage').text("The user " + studentName + " is already added");
                    return false;
                } else {
                    $.post(
                        PxPage.Routes.student_exist, {
                            studentName: studentName,
                            studentId: studentId
                        }).done(function (response) {
                            if (response == "True") {

                                $.post(
                                PxPage.Routes.create_group, {
                                    groupName: studentName,
                                    studentId: studentId
                                }, function (response) {
                                    PxPage.log('create group: ' + response);
                                    $('.ui-icon-closethick').click();

                                    $('#SettingsEntityId').val(response);
                                    PxPage.LargeFNE.GetSettingsList();

                                    $('.studentId').val('');

                                });

                            } else {
                                $('.errorMessage').text("This student doesn't exist");
                                return false;
                            }

                        });
                }

            }
        },

        CloseDialog: function () {
            $('body').unblock();
        },

        BindControls: function () {
            $('#btnAddCancel').die().live('click', function () {
                PxSettingsTab.CloseDialog('btnAddCancel');
                $("#ddlSettingsList").val("EntireClass");

                var event = { target: $(".ddlSettingsList") };

                PxPage.LargeFNE.OnChangeSettingsList(event);
                PxSettingsTab.UpdateQuizSettings("EntireClass");
                $('#errorMessage').text("");
            });
            $('#btnTemplateCancel').die().live('click', function () {
                PxSettingsTab.CloseDialog('cancel-template');
            });
            $('#btnCancel').live('click', function () {
                $('#fne-window').unblock();
            });
            PxSettingsTab.BindReviewSettings();

        },
        BindReviewSettings: function () {
            if (!$('.review-setting').length)
                return;
            $('.review-setting input[type="radio"]').unbind().bind('click', _static.fn.checkReviewSettings);
           
        },
        AutoComplete: function () {
            $(".studentName").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: PxPage.Routes.get_searchInfo,
                        type: "POST",
                        dataType: "json",
                        data: {
                            contactName: request.term,
                            maxresults: 10
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    value: item.FirstName + " " + item.LastName,
                                    id: item.Id
                                }
                            }))
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            PxPage.Toasts.Error(textStatus);
                        }
                    });
                },
                select: function (event, ui) {
                    if (ui.item) {
                        $('.studentId').val(ui.item.id);
                    }
                },
                minLength: 2,
                mustMatch: true
            });
        },

        OnAdd: function () {
            var name = $('.studentName').val();
            if (name == "") {
                $('.field-validation-error').show();
                $('.studentName').focus();
                return false;
            } else {
                var val = PxSettingsTab.AddIndividual();
                if (val) {
                    $('.field-validation-error').hide();
                    PxSettingsTab.UpdateQuizSettings("EntireClass");
                }
                return false;
            }
        },

        LoadTemplateFunction: function () {
            PxSettingsTab.ShowGearBox();
            PxSettingsTab.TemplateItemSelected({
                target: $(".template-management-list li:first")
            });
            $(".template-management-list li").rebind("click", PxSettingsTab.TemplateItemSelected);

            $("#delete-confirm").dialog({
                autoOpen: false,
                title: "Delete Template",
                resizable: false,
                modal: true,
                buttons: {
                    "Cancel": function () {
                        $(this).dialog("close");
                    },
                    "Delete": function () {
                        var itemid = $(".template-management-list li.selected").attr('itemid');
                        $(this).dialog("close");
                        //Delete the template
                        $.post(PxPage.Routes.delete_template, {
                            itemid: itemid
                        }, function (response) {
                            //Reload the page template Management window
                            if (response == "deleted") {
                                PxSettingsTab.UpdateTemplateManagement();
                                $('#settingswrapper #ddlTemplateList').html('');
                                PxSettingsTab.GetTemplateList($("input.item-id").val());
                            } else {
                                PxPage.Toasts.Error('Cannot delete this template because it has content derived from it.');
                            }
                        });
                    }
                }
            });
        },

        OpenEdit: function () {
            $('#fne-window').unblock();
            $('#fne-window').block({
                message: $("#divSaveTemplateAs"),
                css: {
                    padding: 0,
                    margin: 0,
                    top: '30%',
                    left: '20%',
                    right: '23%'
                }
            });
            $("#divSaveTemplateAs #spnNameError").hide();
            $(".divPopupTitle").text('Edit Template Name / Description');
            // clean up the div if needed
            if ($(".txtTemplateName").length > 1) $("#divSaveTemplateAs").html('');
            $("#btnSaveAsTemplate").hide();
            $("#btnSaveEdit").show();
            $("#divSaveTemplateAs #txtTemplateName").val('');
            $("#divSaveTemplateAs #txtDescription").val('');
            // get data for form pre-population
            $.ajax({
                url: PxPage.Routes.get_Template_info,
                success: function (response) {
                    try {
                        var p = jQuery.parseJSON(response);
                        var description = "";
                        $.each(p, function (i) {
                            $.each(this, function (j) {
                                $("#divSaveTemplateAs #txtTemplateName").val(this.title);
                                description = this.description;
                            });
                        });
                        // Added the text area dynamically and else the tiny MCE is not editable
                        var txtarea = "<textarea class= 'html-editor' id='Content_Description' name='Content.Description' style = 'visibility:hidden;width:auto;'>" + description + "</textarea>";
                        $("#divSaveTemplateAs .description-text-area").html(txtarea);
                        PxPage.Update();
                    } catch (err) { }
                },
                data: {
                    'itemId': $(".template-management-list li.selected").attr('itemid')
                },
                type: "POST"
            });
        },

        OpenSaveAs: function () {
            // clean up the div if needed
            if ($(".txtTemplateName").length > 1) $("#divSaveTemplateAs").html('');
            $("#btnSaveEdit").hide();
            $("#btnSaveAsTemplate").show();
            $(".divPopupTitle").text('SAVE AS');
            // initialize form
            $("#divSaveTemplateAs #txtTemplateName").val('');
            $("#divSaveTemplateAs #txtDescription").val('');
            $("#divSaveTemplateAs #spnNameError").hide();
            var newItemid = $("#divSaveTemplateAs #newItemId").val();
            $('#fne-window').unblock();
            $('#fne-window').block({
                message: $("#divSaveTemplateAs"),
                css: {
                    padding: 0,
                    margin: 0,
                    top: '30%',
                    left: '20%',
                    right: '23%'
                }
            });
            // Added the text area dynamically and else the tiny MCE is not editable
            var txtarea = "<textarea class= 'html-editor' id='Content_Description' name='Content.Description' style = 'visibility:hidden;width:auto;'></textarea>";
            $("#divSaveTemplateAs .description-text-area").html(txtarea);
            PxPage.Update();
        },

        SaveAsTemplatePostSave: function () {
            PxPage.log("SavingTemplates");
            var itemId = $("#divSaveTemplateAs #newItemId").val();
            var title = $("#divSaveTemplateAs #txtTemplateName").val();
            var desc = tinyMCE.activeEditor.getContent();
            var oldItemId = $(".template-management-list li.selected").attr('itemid');
            var responseIdNew = TemplateResponseIdNew;
            var responseIdOld = TemplateResponseIdOld;
            $("#divSaveTemplateAs").html(''); //Remove the div
            var args = {
                responseIdNew: responseIdNew,
                responseIdOld: responseIdOld,
                title: title,
                description: desc,
                oldItemId: oldItemId
            };
            //swap the templates
            $.post(PxPage.Routes.save_templateas, args, function (response) {
                //Reload the page template Management window
                PxSettingsTab.UpdateTemplateManagement();
                //Reload the template dropdown 
                $('#settingswrapper #ddlTemplateList').html('');
                PxSettingsTab.GetTemplateList($("input.item-id").val());
            });
        },

        SaveAsTemplate: function () {
            TemplateSaveAs = true;
            var itemId = $("#divSaveTemplateAs #newItemId").val();
            var title = $("#divSaveTemplateAs #txtTemplateName").val();
            if (title != '') {
                PxPage.Loading('divSaveTemplateAs');
                //tinyMCE.triggerSave();
                var desc = tinyMCE.activeEditor.getContent();
                var oldItemId = $(".template-management-list li.selected").attr('itemid');
                $('#divSaveTemplateAs #spnNameError').hide();
                //Create a new template, copy the old one to it, save the old one, then switch the two
                var args = {
                    itemId: oldItemId
                };
                $.post(PxPage.Routes.create_template, args, function (responseId) {
                    TemplateResponseIdOld = responseId;

                    // If the template editor is a homework or quiz, take a different path for saving
                    if ($("#assessment-template-form").find("#assessment-settings-form").length === 1) {
                        $.post(PxPage.Routes.create_template, args, function (responseIdNew) {
                            TemplateResponseIdNew = responseIdNew;
                            PxSettingsTab.SaveAssessmentTemplate(responseIdNew);
                            PxSettingsTab.SaveAsTemplatePostSave();
                        });

                        return;
                    } else {
                        PxPage.SetFrameApiHooks();
                        PxPage.FrameAPI.saveComponent("itemeditor", "itemsettings");
                    }
                });
            } else {
                $('#divSaveTemplateAs #spnNameError').show();
                return false;
            }
        },

        SaveAsTemplateCancel: function () {
            $('#fne-window').unblock();
        },

        SaveTemplate: function () {
            $("#divSaveTemplate #ta1").val("Loading...");
            $(".divPopupTitle").text('SAVE');
            $('#fne-window').block({
                message: $("#divSaveTemplate"),
                css: {
                    padding: 0,
                    margin: 0,
                    top: '15%',
                    left: '30%',
                    right: '30%'
                }
            });

            $.ajax({
                url: PxPage.Routes.get_template_items,
                success: function (response) {
                    var textArea = $("#divSaveTemplate #ta1");
                    textArea.val(response.items.length ? '' : '(None)');
                    $.each(response.items, function (i, v) {
                        textArea.val(textArea.val() + v.title);
                    });
                },
                data: {
                    'templateId': $(".template-management-list li.selected").attr('itemid')
                },
                type: "POST"
            });
        },

        SaveTemplateCommit: function () {
            var answer = confirm("Are you sure you want to save?");
            if (answer) {
                // If the template editor is a homework or quiz, take a different path for saving
                if ($("#assessment-template-form").find("#assessment-settings-form").length === 1) {
                    var itemId = $("#AssessmentId").val();
                    if (PxSettingsTab.SaveAssessmentTemplate(itemId) === true) {
                        PxPage.Toasts.Success("Your settings have been saved");
                    }
                    $('#fne-window').unblock();
                    return;
                }

                $('#fne-window').unblock();
                $('#fne-window').unblock();
                $("#divSaveTemplate").remove('.description-text-area');
                // clear contents of save div
                if ($(".txtTemplateName").length > 1) $("#divSaveTemplateAs").html('');
                TemplateProgateUpdates = false;
                TemplateSaveAs = false;
                TemplateRegularSave = true;
                PxPage.SetFrameApiHooks();
                PxPage.FrameAPI.saveComponent("itemeditor", "itemsettings");
            }
        },

        SaveTemplateCommitUpdate: function () {
            var answer = confirm("Are you sure you want to save?");
            if (answer) {
                $("#divSaveTemplate").remove('.description-text-area');
                // clear contents of save div
                if ($(".txtTemplateName").length > 1) $("#divSaveTemplateAs").html('');
                $('#fne-window').unblock();
                PxPage.SetFrameApiHooks();
                TemplateProgateUpdates = true;
                PxPage.FrameAPI.saveComponent("itemeditor", "itemsettings");
            }
        },

        Init: function () {
            $('#txtTemplateName').change(function () {
                PxPage.log('change occurred to input');
            });
            // This check is done to see if the Init is for Assingment Center or Content Browsing
            var itemId = $('.selected').parent().find('input.item-id').val();
            if (!itemId == true) {
                itemId = $("input.item-id").val();
            }

            //This check is to determine whether we need to enable or disable number of hints percentage textfield
            var isChecked = $("#AllowViewHints").is(":checked");
            var textBox = $("#contentwrapper").find("#HintSubstractPercentage");
            if (!isChecked) {
                textBox.attr("disabled", true);
            } else {
                textBox.removeAttr("disabled");
            }

            PxSettingsTab.GetTemplateList(itemId);
            PxSettingsTab.BindControls();
            PxPage.FneInitHooks['template-management'] = PxSettingsTab.LoadTemplateFunction;
            $(window).trigger('resize');

            // Quiz Settings
            $("#contentwrapper").find("#assessment-settings-save").die("click");
            $("#contentwrapper").find("#assessment-settings-save").live("click", function (event) {
                var data = $("#contentwrapper").find("#assessment-settings-form").serialize();
                $.post($("#contentwrapper").find("#assessment-settings-form").attr("action"), data, function (response) {
                    PxPage.Toasts.Success(response.ReturnMessage);
                });
            });
            $("#contentwrapper").find("#AllowViewHints").live("click", function (event) {
                var isChecked = $("#AllowViewHints").is(":checked");
                if (!isChecked) {
                    $("#contentwrapper").find("#HintSubstractPercentage").attr("disabled", true);
                } else {
                    $("#contentwrapper").find("#HintSubstractPercentage").removeAttr("disabled");
                }
            });

            // Quiz Templating Settings
            $("#assessment-template-form").find("#assessment-settings-save").die("click");
            $("#assessment-template-form").find("#assessment-settings-save").live("click", function (event) {
                var data = $("#assessment-template-form").find("#assessment-settings-form").serialize();

                $.post($("#assessment-template-form").find("#assessment-settings-form").attr("action"), data, function (response) {
                    PxPage.Toasts.Success("Your settings have been saved");
                });
            });

            PxSettingsTab.SetFormBehavior();
            PxSettingsTab.SetupAssessmentTypeToggle();
            PxSettingsTab.SetGroupActions();
        },

        SetupAssessmentTypeToggle: function () {
            $(".assessment-toggles input").click(function (event) {
                var itemId = $("#AssessmentId").val(),
                        type = $(event.target).val();
                $.ajax({
                    url: PxPage.Routes.change_assessment_type,
                    type: "POST",
                    data: {
                        itemId: itemId,
                        newType: type
                    },
                    success: function () {
                        PxSettingsTab.UpdateQuizSettings("EntireClass");
                        var node = $("li#" + itemId);
                        var instance = $("div#toc");
                        if (instance.length > 0) {
                            $(PxPage.switchboard).trigger("reloadparent", [instance, node]);
                        }
                    }
                });
            });
        },

        SetGroupActions: function () {
            // When the groups dropdown is changed, refresh the settings view for the selected group.
            $("#ddlSettingsList").change(function (event) {
                PxSettingsTab.UpdateQuizSettings($(event.target).val());
            });
        },

        UpdateQuizSettings: function (entityId, callback) {
            if (entityId !== "AddIndividual") {
                $('#assessment-settings-container').block();
                var itemId = $("input[name=AssessmentId]").val();
                var entityId = $("#ddlSettingsList").val();

                if (entityId == "EntireClass") {
                    $('.visibility .lockIndividualStudent').hide();
                    $('.visibility :text').removeAttr('disabled');
                    $('.visibility :checkbox').removeAttr('disabled');
                    $('.visibility :checkbox').parent().css('color', '');
                    $('.visibility .calendar_toggle').show();
                    $('.visibility .li-cal-box').show();
                    $('.visibility #clearDateField').show();
                }
                else {
                    $('.visibility .lockIndividualStudent').show();
                    $('.visibility :text').attr('disabled', '');
                    $('.visibility :checkbox').attr('disabled', '');
                    $('.visibility :checkbox').parent().css('color', 'Gray');
                    $('.visibility .calendar_toggle').hide();
                    $('.visibility .li-cal-box').hide();
                    $('.visibility #clearDateField').hide();
                }

                $("#assessment-settings-container").load(
                    PxPage.Routes.assessment_view, {
                        itemId: itemId,
                        entityId: entityId
                    }, function () {
                        $("#assessment-settings-save").hide();

                        PxSettingsTab.SetupAssessmentTypeToggle();
                        PxSettingsTab.SetFormBehavior();
                    });
            }
        },

        SetFormBehavior: function () {
            // When the number of attempts dropdown is changed, disable the scored attempt dropdown and reset its 
            // value if the number of attemtps is 1.  Otherwise, enable the scored attempt dropdown.
            var numAttemptsonLoad = $("select[name='NumberOfAttempts\\.Attempts']").val();
            $(".assessment-settings-wrapper").find("input[value=Second]").attr("disabled", numAttemptsonLoad == 1)
            $(".assessment-settings-wrapper").find("input[value=Final]").attr("disabled", numAttemptsonLoad == 1)

            $("select[name='NumberOfAttempts\\.Attempts']").change(function (event) {
                var scoredAttempt = $("select[name='ScoredAttempt']");
                var numAttempts = $(event.target).val();
                scoredAttempt.attr("disabled", numAttempts == 1);
                $(".assessment-settings-wrapper").find("input[value=Second]").attr("disabled", numAttempts == 1)
                $(".assessment-settings-wrapper").find("input[value=Final]").attr("disabled", numAttempts == 1)
                if (numAttempts == 1) {
                    scoredAttempt[0].selectedIndex = 0;
                }
            });

            // Front end behavior for time limit.
            $(".time-limit-minutes").click(function () {
                $("input[name=time-limit]")[1].checked = true;
            });
            // If we've selected no time limit, set the number to 0.
            $("input[name=time-limit]").change(function (event) {
                if ($(event.target).attr('id') == "no-time-limit-radio") {
                    $("input[name=TimeLimit]").val(0);
                }

            });
            $("[name^='AssessmentSettings']").each(function (i, j) {
                $(j).attr("name", $(j).attr("name").replace("AssessmentSettings.", ""))
            });
        },

        SaveAssessmentTemplate: function (itemId) {
            $("#assessment-template-form").find("#assessment-settings-form #AssessmentId").val(itemId);
            var data = $("#assessment-template-form").find("#assessment-settings-form").serialize();
            var success = false;
            data = data + "&" + "itemId=" + itemId;

            $.post(PxPage.Routes.save_template, data, function (response) {
                $("#divSaveTemplateAs").hide();
                success = true;
                $('#fne-window').removeClass('require-confirm');
            });

            return success;
        }
    };
} (jQuery);