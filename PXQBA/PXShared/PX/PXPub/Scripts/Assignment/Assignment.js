var PxAssignment = function ($) {
    var _static = {
        state: {
            closedItemId: '',
            reload: false
        }
    };
    return {
        BindControls: function () {
            $('.divPopupClose, .divPopupWin input[name="cancel"]').bind('click', function () {
                if ($(".ui-icon-closethick").length > 0) {
                    $(".ui-icon-closethick").click();
                    return false;
                }
                $('#spnLinkError').hide();
                $('body').unblock(); ;
            });

            $(PxPage.switchboard).bind("componentcancelled", function (componentType, id, state) {
                if (componentType == "RubricEditor" || id == "rubriceditor") {
                    if ($('#content-modes #view').length)
                        '#content-modes #view'.click();
                }
            });
            $(PxPage.switchboard).bind('fneclosed', PxAssignment.OnAssignmentEditorClosed);            
            
            PxAssignment.BindQtipForRubricGuides();

            //select all checkboxes in the document list when selectAll is clicked
            $('#assignmentResTable input[name="cbkSelectAll"]').live('click', function (event) {
                if (event.target.checked == true)
                    $('#assignmentResTable .cbResource').prop('checked', true);
                else
                    $('#assignmentResTable .cbResource').prop('checked', false);
            });
            $('#lnkDeleteSavedDoc').unbind('click');
            $('#lnkDeleteSavedDoc').bind('click', PxAssignment.DeletedSelectedDoc);

            $('#lnkDownloadDocs').unbind('click').bind('click', PxAssignment.OnDownloadLinkClicked);

            $('#btnDownloadConfirm').bind('click', PxAssignment.StartDownloadDocs);
            $('#btnCancelDownload').bind('click', function () {
                PxAssignment.CloseDialog($('.downloadBox'));
            });
            $('#btnSubmitReflection').unbind('click').bind('click', function () {
                PxAssignment.OnSubmissionReflectionBegin(true);
            });

            $(".faceplatefne-assignmentwidget").live("click", function () {
                $(".ui-icon-closethick").click();
            });

            $(document).off('click', '#assignmentViewContent .quickgradebutton').on("click", '#assignmentViewContent .quickgradebutton', function (event) {
                event.preventDefault();
                var quickgradebutton = $(this);
                var gradingurl = quickgradebutton.attr("href");
                PxPage.LargeFNE.OpenFNELink(gradingurl, "Gradebook", false, null);

            });

            PxPage.Loaded();
        },

        BindQtipForRubricGuides: function () {
            try {
                $('#divAssignmentViewer #divInstructorView td.rubric-score-guides').add('#divAssignmentViewer #divInstructorView .rubricrow .rubric-score-guides').qtip("destroy");
            } catch (e) { }
            $('#divAssignmentViewer #divInstructorView td.rubric-score-guides').add('#divAssignmentViewer #divInstructorView .rubricrow .rubric-score-guides').each(function () {
                $(this).qtip({
                    content: { text: $(this).html() },
                    show: 'mouseover',
                    name: 'dark',
                    delay: 5000,
                    hide: { when: 'mouseout', fixed: true },
                    position: { corner: { target: "bottomLeft", tooltip: "leftTop"} },
                    style: {
                        padding: 0, margin: 10, background: '#FAF9E8',
                        border: { width: 3, radius: 3, color: '#0081C6', style: 'solid' },
                        color: 'black',
                        width: { min: 180, max: 370 },
                        'height': '150px',
                        'overflow': 'auto'
                    }
                });
            });
        },

        ClearPopupFields: function () {
            $('#docTitle').val('');
            $('#linkTitle').val('').focus();
            $('#linkUrl').val('http://');
            if ($.browser.msie) {
                $("#docFile").replaceWith("<input type='file' id='docFile' name='docFile' />");
            }
            else {
                $('#docFile').val('');
            }
            $('#spnLinkError').hide();
            $('#spnAddDocError').hide();
        },

        OnRefDocUploadComplete: function (response) {
            $('#doc-collection').replaceWith(response);
        },

        ValidateDeleteResources: function () {
            if (!$('#assignmentResTable .cbResource').is(':checked')) {
                alert("Please select atleast one resource to delete");
                return false;
            }
            else {
                PxPage.OnFormSubmit();
                return true;
            }
        },

        OnDownloadLinkClicked: function (event) {
            var selectedIds = $.map($("tr.jqgrow[aria-selected='true']"), function (i) { return $(i).attr('id'); });
            var eid = $('#content-item-id').text();
            if (selectedIds.length) {
                PxAssignment.ShowDialogueBox($('.downloadBox'), 'Download Document');
            }
            else {
                alert('Select atleast one item to download.');
            }
            event.preventDefault ? event.preventDefault() : event.returnValue = false;
        },

        StartDownloadDocs: function () {
            var selectedIds = $.map($("tr.jqgrow[aria-selected='true']"), function (i) { return $(i).attr('id'); });
            $('#format').val($("#downloadFormat option:selected").val());
            $('#documentIds').val(selectedIds.join(','));
            PxAssignment.CloseDialog($('.downloadBox'));
        },



        SetUserAssignmentView: function () {
            $('#attributeTitle').hide();
            $('#References').hide();
            $('#Rubrics').hide();
            $('#Details').hide();
        },

        ShowTab: function () {
            var selected = $('#attributeList option:selected');
            $('.divAssignmentWriter').hide();
            $('#' + selected.val()).show();
            $('#attributeName').text(selected.val());
            if ($('#attributeList').val() != "Writing")
                $('#attributeTitle').show();
        },

        ShowRubric: function (index, desc) {
            $('#RubricsTable_InstructorView tr, #RubricsTable_InstructorView .rubricrow').hide();
            $('#RubricsTable_InstructorView tr:eq(' + index + ')').show();
            $('#RubricsTable_InstructorView .rubricrow:eq(' + index + ')').show();
            $('#rubricDesc').text(desc);
            $('.current-rubric').text(index + 1);

            //For writing assignment, send event to display specific rubric
            $('.rubricView').trigger('show_rubric', ['.tblAssignmentView .rubricrow:eq(' + index + ')']);

        },

        ValidateScore: function (id, maxScore, index, desc) {
            var floatRegex = /^(0|[1-9][0-9]*|[1-9][0-9]{0,2}(,[0-9]{3,3})*)$/;
            var item = 'inputScore' + id;
            var score = $('#' + item).val();

            if ((!isNaN(score)) && (score > 0) && (!floatRegex.test(score))) {
                alert('Decimal Score is not allowed.');
                setTimeout('document.getElementById(\'' + item + '\').focus();document.getElementById(\'' + item + '\').select();', 0);
                return false;
            }
            if ($('#' + item).length > 0 && (isNaN(score) || (score < 0 || score > maxScore))) {
                alert('Enter a score from 0 to ' + maxScore);
                setTimeout('document.getElementById(\'' + item + '\').focus();document.getElementById(\'' + item + '\').select();', 0);
                return false;
            }
            else {
                PxAssignment.ShowRubric(index, desc);
            }
        },

        ValidateFinalGrade: function (id, maxScore) {

            var floatRegex = /^(0|[1-9][0-9]*|[1-9][0-9]{0,2}(,[0-9]{3,3})*)$/;
            var assignedScore = '';
            if (!isNaN(assignedScore)) {
                maxScore = maxScore * 1;
            }
            if (id == 'txtassignedScore' || id == 'AssignedScore') {
                assignedScore = $('#' + id).val();
                assignedScore = jQuery.trim(assignedScore);
                if (assignedScore == '') {
                    assignedScore = -1;
                }
                if (!isNaN(assignedScore)) {
                    assignedScore = assignedScore * 1;
                }
                else {
                    assignedScore = -1;
                }
                if ((!isNaN(assignedScore)) && (assignedScore > 0) && (!floatRegex.test(assignedScore))) {
                    alert('Decimal Score is not allowed.');
                    setTimeout('document.getElementById(\'txtassignedScore\').focus();document.getElementById(\'txtassignedScore\').select();', 0);
                    return false;
                }
                if (assignedScore < 0 || assignedScore > maxScore) {
                    alert("Please enter a grade between 0 and " + maxScore);
                    setTimeout('document.getElementById(\'txtassignedScore\').focus();document.getElementById(\'txtassignedScore\').select();', 0);
                }
            }
            else {
                var item = 'inputScore' + id;
                assignedScore = $('#' + item).val();

                if ((!isNaN(assignedScore)) && (assignedScore > 0) && (!floatRegex.test(assignedScore))) {
                    alert('Decimal Score is not allowed.');
                    setTimeout('document.getElementById(\'' + item + '\').focus();document.getElementById(\'' + item + '\').select();', 0);
                    return false;
                }
                if (isNaN(assignedScore) || (assignedScore < 0 || assignedScore > maxScore)) {
                    alert("Please enter a score between 0 and " + maxScore);
                    setTimeout('document.getElementById(\'' + item + '\').focus();document.getElementById(\'' + item + '\').select();', 0);
                }
            } return false;
        },

        AddScore: function (id, maxScore) {
            var sum = 0;
            $('input.rubric-score-assigned').each(function () {
                if (this.value != '' && (!isNaN(this.value))) {
                    sum += parseInt(this.value);
                }
            });
            $('#currScore').text(sum);
        },

        ShowIntLinkPopup: function () {
            var options = { modal: true, draggable: false, closeOnEscape: true, width: '800px', height: '700px', resizable: false, autoOpen: false };
            var tag = $("#divSelectIntLink"); //This tag will the hold the dialog content.

            var args = {
                filterid: '',
                syllabustype: '',
                isReadOnly: false
            };

            tag.dialog({ modal: options.modal, title: options.title, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, resizable: options.resizable, autoOpen: options.autoOpen, close: function () { } }).dialog('open');

            $(".ui-dialog-title").html("Add Internal Link");
            $(".divPopupTitle").hide();


            /*$('.divPopupClose, .divPopupWin input[name="cancel"]').bind('click', function () {
            $('body').unblock();
            return false;
            });
            $('body').block({
            message: $("#divSelectIntLink"),
            css: {
            padding: 0,
            margin: 0,
            top: '10%',
            left: '20%',
            right: '20%'
            }
            });
            */
        },

        CheckIntLinkPopupLoaded: function () {
            PxAssignment.ShowIntLinkPopup();
            if ($("#IntLinkPopupLoaded").val() == "true") {
                return false;
            }
        },

        SetSectionAddLink: function () {

            var firstTreeNode = jQuery('#divToc > .level > .expandContainer.folder a.expand');

            $('.int-link-toc div.level').hover(
                function () {
                    $(this).children('.expandContainer').addClass('int-link-active');
                },
                function () {
                    $('.int-link-toc .expandContainer').removeClass('int-link-active');
                }
            );

            $('#divToc').find('div.level:odd').addClass('odd');

            if (jQuery('#divToc > .level > .expandContainer.folder').next('span.children').children().length === 0) {

                firstTreeNode.click();
            }
        },

        ToggleSection: function () {
            $('.toc .level .expandContainer a.expand.active').parent().parent().children('span.children').toggle();
            PxAssignment.SetSectionAddLink();
        },

        OnIntLinkAdded: function () {
            //$.unblockUI();
            $(".ui-icon-closethick").click();
            //ContentWidget.ContentCreated();
            //$('body').unblock();
        },

        ShowWaitOverlay: function (containerId) {
            $('#' + containerId).block({ css: {} });
        },

        HideOverlay: function (containerId) {
            $('#' + containerId).unblock();
        },

        EditorLoaded: function () {
            $('#fne-content').css("background", "#F0F0F0");
            $('#References').hide();
            $('#Rubrics').hide();
            $('#Details').hide();
            $('#attributeTitle').hide();
            PxAssignment.ShowTinyMceCustomCtrls();
            PxAssignment.InitDialog($('.confirmBox'), $("#frmAssignmentEdit"));
            PxAssignment.InitDialog($('.inputBox'), $("#frmAssignmentEdit"));
            PxAssignment.InitDialog($('.statusBox'), $("#frmAssignmentEdit"));
            PxAssignment.BindEditorControls();
            //            if ($('#assignmentPopup').attr('status') != 'New' && $('#assignmentPopup').attr('status') != 'Saved') {
            //                $('.btnSubmit').hide();
            //                $('#spnSubmissionStatus').text('One of your documents was already submitted, you can no longer edit.');
            //            }
        },

        BindEditorControls: function () {
            PxPage.FneCloseHooks['assignment-editor'] = PxAssignment.ShowAssignmentFne;
            $('#attributeList').bind('change', PxAssignment.ShowTab);
            $('#btnSave').bind('click', PxAssignment.OnEditorSaveClicked);
            $('#btnSaveReflection').bind('click', PxAssignment.OnEditorSaveReflectionClicked);
            $('#btnSaveAs').bind('click', PxAssignment.OnEditorSaveAsClicked);
            $('#btnSubmitSub').bind('click', function () {
                PxAssignment.OnEditorSubmitClicked();
            });


            $('#btnCancel').bind('click', PxAssignment.OnAssignmentEditorCancel);
            $('#btnConfirmYes').bind('click', function () {
                PxAssignment.CloseDialog($('.confirmBox'));
                PxAssignment.OnSubmissionBegin();
            });
            $('#btnConfirmNo').bind('click', function () { PxAssignment.CloseDialog($('.confirmBox')); });
            $('#btnSaveDocName').bind('click', PxAssignment.OnDocDialogSaveClicked);
            $('#btnCancelDocName').bind('click', function () { PxAssignment.CloseDialog($('.inputBox')); });
            $('#btnStatusClose').bind('click', function () {
                PxAssignment.CloseDialog($('.statusBox'));
                if ($('#spnSubmissionStatus').text() == 'submitted')
                    PxAssignment.OnAssignmentEditorClosing();
            });
        },

        ShowTinyMceCustomCtrls: function () {
            $.getJSON($('#ddlAssignmentItems').val(), function (data) {
                $('.mceNativeListBox').show();
                data = $.map(data, function (item, a) {
                    var selectedItem = "";
                    if (item.Selected) selectedItem = "selected";
                    return "<option " + selectedItem + " value = " + item.Value + ">" + item.Text + "</option>";
                });
                $(".mceNativeListBox").html(data.join(""));
            });
        },

        InitDialog: function (divBox, formCtrl) {
            try {
                var dialogBox = divBox.dialog({
                    autoOpen: false,
                    modal: true,
                    resizable: false,
                    closeOnEscape: false,
                    open: function (event, ui) {
                        dialogBox.parent().appendTo(formCtrl);
                        //$(".ui-dialog-titlebar-close").hide();
                        $("#ui-dialog-title-inputBox").css('font-size', '15px');
                        $('.ui-widget-overlay').appendTo(formCtrl);
                    },
                    width: 'auto',
                    resize: 'auto'
                });
                dialogBox.parent().appendTo(formCtrl);
            }
            catch (e) { }
        },

        ViewerResized: function () {
            PxAssignment.SetViewerBottomViewHeight();
        },

        ViewerLoaded: function () {
            PxAssignment.BindViewerControls();
            $('#fne-content').css("background", "#F0F0F0");
            PxAssignment.GetStudents();
            $('#RubricsTable_InstructorView tr').hide();
            $('#RubricsTable_InstructorView tr:eq(0)').show();
            PxAssignment.SetViewerBottomViewHeight();
        },

        SetViewerBottomViewHeight: function () {
            if ($('#fne-content #divAssignmentViewer').length > 0)
                $('#fne-content #bottomView').height("100%");
            else
                $('#fne-content #bottomView').height($('#fne-content').outerHeight(true) - $('#fne-content #topView').outerHeight(true));
        },

        BindViewerControls: function () {
            $('#waStudentList').live('change', PxAssignment.ShowAssignment);
        },

        StudentNavData: {},

        GetStudents: function () {
            if (PxAssignment.StudentNavData.length == undefined) {
                $.getJSON($('#studentListUrl').val(), function (data) {
                    PxAssignment.StudentNavData = data;
                    PxAssignment.SetWANavigation();
                });
            }
            else
                PxAssignment.SetWANavigation();
        },

        SetWANavigation: function () {
            var currEid = $('#EnrollmentId').val();
            var dropDownHtml = $.map(PxAssignment.StudentNavData, function (item, a) {
                var selectedItem = "";
                if (currEid == item.Eid) selectedItem = "selected";
                return "<option " + selectedItem + " value=" + item.Value + ' >' + item.Text + "</option>";
            });
            $("#waStudentList").html(dropDownHtml.join(''));
        },

        ShowAssignment: function () {
            var selectedStudent = $("#waStudentList option:selected").val();
            PxPage.Loading('fne-content');
            $.get(selectedStudent, function (data) {
                var htmlAssignmentView = $('<div>' + data + '</div>').children('#divAssignmentViewer');
                $('#divAssignmentViewer').replaceWith(htmlAssignmentView.clone());
                $('#RubricsTable_InstructorView tr').hide();
                $('#RubricsTable_InstructorView tr:eq(0)').show();
                PxAssignment.GetStudents();
                PxPage.Loaded('fne-content');
                bfw_secondaryId = $('#EnrollmentId').val();
                bfw_itemUrl = $('#itemUrl').val();
            });
        },

        Init: function () {
            PxPage.FneInitHooks['assignment-editor'] = PxAssignment.EditorLoaded;
            PxPage.FneInitHooks['wa-viewer'] = PxAssignment.ViewerLoaded;
            PxPage.FneResizeHooks['wa-viewer'] = PxAssignment.ViewerResized;
            PxPage.FneCloseHooks['wa-viewer'] = PxAssignment.OnAssignmentViewerClosed;
            PxAssignment.BindControls();
            PxAssignment.SetUserAssignmentView();
            PxAssignment.BindResizeEvent();
            PxPage.SetWindowResizeHandler();
            PxAssignment.InitDialog($('.downloadBox'), $("#frmDownload"));
            PxAssignment.CloseHiddenPopUps();
        },

        BindResizeEvent: function () {
            $(window).bind('resize', function () {
                if ($('#content-item #gridResults').length) {
                    $('#content-item #gridResults').setGridWidth($('#content').width() - 40, true);
                }
                try {
                    $(".inputBox").dialog("option", "position", "center");
                    $('.statusBox').dialog("option", "position", "center");
                    $('.confirmBox').dialog("option", "position", "center");
                    $('.downloadBox').dialog("option", "position", "center");
                }
                catch (e) { }
            }).trigger('resize');
        },

        FixGridHeight: function (grid) {
            var gviewNode = grid[0].parentNode.parentNode.parentNode;
            var bdiv = jQuery(".ui-jqgrid-bdiv", gviewNode);
            if (bdiv.length) {
                var delta = bdiv[0].scrollHeight - bdiv[0].clientHeight;
                var height = grid.height();
                if (delta !== 0 && height && (height - delta > 0)) {
                    grid.setGridHeight(height - delta);
                }
            }
        },

        OnAssignmentEditorCancel: function () {
            if ($('#fne-unblock-action').length > 0) {
                $('#fne-unblock-action').click();
            }
        },

        OnAssignmentEditorClosing: function () {
            $('#frmAssignmentEdit > .ui-dialog').remove();
            $(".inputBox").remove();
            $('.statusBox').remove();
            $('.confirmBox').remove();
            $.unblockUI();
            if ($('#spnSubmissionStatus').text() == 'submitted') {
                $('#divComposeItems').hide();
            }
            if ($('#spnSubmissionStatus').text() == 'submitted' || $('#spnSubmissionStatus').text() == 'saved') {
                $("#gridStudentsSubmissions, #fne_gridStudentsSubmissions").trigger("reloadGrid");
            }

            //Set values from dialog that are need after the dialog is closed.
            _static.state.closedItemId = $('.assignment-editor #Id').val();
            _static.state.reload = $('.btnCancelReflection').length > 0;
        },
        OnAssignmentEditorClosed: function () {
            var itemUrl = PxPage.Routes.display_content + "/" + _static.state.closedItemId + "?mode=4&includeToc=False&includeDiscussion=True&isStudentUpdated=False";
            if (_static.state.reload) {
                PxPage.Loading('#right');
                $.get(itemUrl, null, function (response) {
                    $('#right').html(response);
                    PxPage.Loaded('#right');
                    ContentWidget.ContentLoaded();
                });
            }
            _static.state.reload = false;
            _static.state.closedItemId = '';
        },
        OnEditorSaveClicked: function () {
            if ($('#Submission_Name').val() == '') {
                $('#spnDocNameMessage').hide();
                $('#btnSaveDocName').attr('value', 'Save');
                PxAssignment.ShowDialogueBox($('.inputBox'), 'Save');
                $('#txtSubmissionName').val('');
                $('#txtSubmissionName').focus();
                return false;
            }
            else {
                PxAssignment.OnSubmissionBegin();
            }
        },

        OnEditorSaveReflectionClicked: function () {
            $('#Submission_Name').val('reflection');
            if ($('#Submission_Name').val() == '') {
                $('#spnDocNameMessage').hide();
                $('#btnSaveDocName').attr('value', 'Save');
                PxAssignment.ShowDialogueBox($('.inputBox'), 'Save');
                $('#txtSubmissionName').val('');
                $('#txtSubmissionName').focus();
                return false;
            }
            else {
                PxAssignment.OnSubmissionReflectionBegin(false);
            }
        },

        OnEditorSubmitClicked: function () {
            if ($('#Submission_Name').val() == '') {
                $('#spnDocNameMessage').hide();
                $('#btnSaveDocName').attr('value', 'Submit');
                PxAssignment.ShowDialogueBox($('.inputBox'), 'Submit');
                $('#txtSubmissionName').val('');
                $('#txtSubmissionName').focus();
                return false;
            }
            else {
                $('#confirmText').text('Are you sure you want to submit this assignment?');
                PxAssignment.ShowDialogueBox($('.confirmBox'), 'Are you sure?');
            }
        },


        OnEditorSaveAsClicked: function () {
            $('#spnDocNameMessage').hide();
            $('#btnSaveDocName').attr('value', 'Save As');
            PxAssignment.ShowDialogueBox($('.inputBox'), 'Save As');
            $('#txtSubmissionName').val('');
            $('#txtSubmissionName').focus();
        },

        ValidateDocName: function () {
            if ($('.inputBox #txtSubmissionName').val() == '') {
                $('#spnDocNameMessage').show();
                $('#spnDocNameMessage').text('Document name cannot be empty.');
                return false;
            }
            else if ($('.inputBox #txtSubmissionName').val() == $('#Submission_Name').val()) {
                $('#spnDocNameMessage').show();
                $('#spnDocNameMessage').text('Document name cannot be same as current document.');
                return false;
            }
            return true;
        },

        CloseDialog: function (dialogBox) {
            dialogBox.dialog('close');
            $('.blockUI').css('z-index', '1001'); // Hack when closing a dialog from blockUI popup.
        },

        ShowDialogueBox: function (dialogBox, title) {
            dialogBox.filter(':gt(0)').remove(); // Remove any duplicate dialog.
            if (title && title != '')
                dialogBox.dialog('option', 'title', title);
            $('.blockUI').css('z-index', '3'); // Hack when opening a dialog from blockUI popup.
            dialogBox.dialog('open');
            dialogBox.dialog("option", "position", "center");
            dialogBox.css("display", "block");
        },

        OnDocDialogSaveClicked: function () {
            if (!PxAssignment.ValidateDocName()) return false;
            $('#Submission_Name').val($('#txtSubmissionName').val());
            var btnSaveDocName = $('#btnSaveDocName').attr('value');
            if (btnSaveDocName.toLowerCase() == "save as") {
                $('#Submission_ResourcePath').val('');
            }
            else if (btnSaveDocName.toLowerCase() == "submit") {
                PxAssignment.CloseDialog($('.inputBox'));
                $('#confirmText').text('Are you sure you want to submit this assignment?');
                PxAssignment.ShowDialogueBox($('.confirmBox'), 'Are you sure?');
                return false;
            }
            PxAssignment.OnSubmissionBegin();
            PxAssignment.CloseDialog($('.inputBox'));
        },

        OnSubmissionBegin: function () {
            $('#spnSubmissionStatus').removeClass('field-validation-error').text('Please wait...').show();
            $('.btnSubmit').hide();
            PxPage.TriggerAssignmentHtmlSave();
            var wordCount = PxPage.GetTinyMceWordCount();
            $('#Submission_WordCount').val(wordCount);
        },

        OnSubmissionReflectionBegin: function (showConfirm) {
            //$('#confirmText').text('Are you sure you want to submit this assignment?');
            //PxAssignment.ShowDialogueBox($('.confirmBox'), 'Are you sure?');
            var doSave = true;
            if (showConfirm) {
                doSave = confirm('Are you ready to submit this assignment? (This action cannot be undone.)');
            }
            if (doSave) {
                $('#spnSubmissionStatus').removeClass('field-validation-error').text('Please wait...').show();
                $('#btnSubmitReflection').hide();
                $('#student-reflection .submission-body .sub-title').empty();
                $('#student-reflection .submission-body .sub-title').append('Student Reflection (Submitting, please wait...):');
                PxPage.TriggerAssignmentHtmlSave();
            }
            else {
                $('#btnSubmitReflection').hide();
                $('#btnSubmitReflection').addClass("cancelAction");
                $('#btnSubmitReflection').val("cancel");
            }
        },

        OnSubmissionSuccessful: function (response) {
            var data = response;
            $('#Submission_ResourcePath').val(data.path);
            switch (data.status) {
                case "submitted":
                    $('.btnSubmit').hide();
                    $('#Writing > textarea').attr("disabled", "disabled");
                    $('#spnSubmissionStatus').text('submitted').hide();
                    $('#divSBoxStatus').text('Assignment submitted successfully.');
                    PxAssignment.ShowDialogueBox($('.statusBox'), 'Status');
                    break;
                case "saved":
                    $('#spnSubmissionStatus').text('saved').hide();
                    $('#divSBoxStatus').text($('#Submission_Name').val() + ' saved successfully.');
                    PxAssignment.ShowDialogueBox($('.statusBox'), 'Status');
                    $('.btnSubmit').show();
                    break;
                default:
                    alert('Error when saving/submitting your submission.');
                    $.unblockUI();
            }
        },

        OnDropboxSubmissionSuccessful: function (response) {
            var data = response;
            $('#Submission_ResourcePath').val(data.path);
            switch (data.status) {
                case "submitted":
                    alert('Dropbox submitted successfully.');
                    $('#spnSubmissionStatus').text('submitted').hide();
                    $('.btnSubmit').show();
                    break;
                default:
                    alert('Error when saving/submitting your submission.');
                    $.unblockUI();
            }
        },


        OnReflectionSubmissionSuccessful: function (response) {
            var data = response;
            $('#Submission_ResourcePath').val(data.path);
            switch (data.status) {
                case "submitted":
                    $('#spnSubmissionStatus').text($('#Submission_Name').val() + ' submitted successfully.');
                    //$('#btnSubmitReflection').show();
                    //$('.btnSubmit').hide();
                    //alert("Submitted successfully");
                    PxAssignment.OnAssignmentEditorClosing();

                    $('#student-reflection .submission-body .sub-title').empty();
                    $('#btnSubmitReflection').hide();

                    //$('#txtsubmitted').removeClass('notvisible');
                    //$('#txtsubmitted').append('Submitted on: ' + data.submitteddate);
                    //$('#txtsubmitted').addClass('bold');
                    $('#nosubmissiontext').text('').append('Submitted on: ' + data.submitteddate).addClass('bold submitted');
                    $('#fne-window').removeClass('require-confirm');
                    break;
                case "saved":
                    $('#spnSubmissionStatus').text($('#Submission_Name').val() + ' saved successfully.');
                    $('#btnSubmitReflection').show();
                    var newReflectionBody = $('textarea.html-editor').val();
                    $('#student-reflection').empty();
                    $('#student-reflection').append('<div class="submission-body">');
                    $('.submission-body').append($('<div />').html(newReflectionBody).text());
                    $('.submission-body').append('<hr>');
                    $('#btnSubmitReflection').removeAttr('disabled');
                    $('#btnSubmitReflection').attr({
                        'class': ''
                    });

                    $('#Submission_Body').val($('<div />').html(newReflectionBody).text());
                    $('#lnkCompose').text("Edit Your Reflection");
                    $('#fne-window').removeClass('require-confirm');
                    //TODO: Throw event that the assignment has been saved.
                    break;
                case "nosubmissionsaved":
                    alert("Please save submission before submitting the reflection.");
                    $('#btnSubmitReflection').show();
                    break;
                case "unsubmit":
                    $("#studentsubmitted").hide();
                    $("#studentnotsubmitted").show();
                    break;
                case "canceled":
                    if ($('#btnSubmitReflection').hasClass("cancelAction")) {
                        $('#btnSubmitReflection').removeClass("cancelAction");
                    }
                    $('#btnSubmitReflection').val("Submit");
                    $('#btnSubmitReflection').show();
                    break;
                default:
                    alert('Error when saving/submitting your submission.');
                    $.unblockUI();
            }
        },

        OnSubmissionFailure: function () {
            $('#spnSubmissionStatus').addClass('field-validation-error').text('There was an error when storing your submission, please try again later.');
            $('.btnSubmit').show();
        },

        OnGradeSubmissionBegin: function () {
            var confirmMessage = "";
            var showConfirm = false;

            var assignedScore = $('#txtassignedScore').val();

            if (isNaN(assignedScore)) {
                alert('Please enter a valid score.');
                return false;
            }

            if (this.data.indexOf("Submit") >= 0) {
                confirmMessage = "Are you sure you want to submit this assignment?";
                showConfirm = true;
            }
            else if (this.data.indexOf("Unsubmit") >= 0) {
                confirmMessage = "Are you sure you want to unsubmit this assignment?";
                showConfirm = true;
            }

            if (showConfirm) {
                if (confirm(confirmMessage)) {
                    $('#gradeSubmissionStatus').show();
                    $('#gradeSubmissionStatus').text('Please wait...');
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                $('#gradeSubmissionStatus').show();
                $('#gradeSubmissionStatus').text('Please wait...');
                return true;
            }

        },

        OnGradeSubmissionSuccessful: function (response) {
            var responseData = response;
            var status = '';
            var message = '';
            if (responseData.indexOf(":") != -1) {
                var vArr = new String(responseData).split(":");
                status = vArr[0];
                message = vArr[1];
            }
            else {
                status = responseData;
            }
            $('#gradeSubmissionStatus').removeClass("field-validation-error");
            switch (status.toLowerCase()) {
                case "submitted":
                    $('#gradeSubmissionStatus').show();
                    $('#gradeSubmissionStatus').text('Grades successfully submitted.');
                    $('.rubric-score-assigned').attr("disabled", "disabled");
                    $('#txtassignedScore').attr("disabled", "disabled");
                    $('.btnVSubmit#btnSubmit').attr("disabled", "disabled");
                    $('.btnVSubmit#btnSubmit').addClass("disabledButton");
                    $('.btnVSubmit#btnUnsubmit').removeAttr("disabled");
                    $('.btnVSubmit#btnUnsubmit').removeClass("disabledButton");
                    $('.btnVSubmit#btnSave').attr("disabled", "disabled");
                    $('.btnVSubmit#btnSave').addClass("disabledButton");
                    $('#divInstructorView .rubric-table-wrapper').addClass('grade-submitted');
                    setTimeout(function () {
                        $('#gradeSubmissionStatus').hide('slow');
                    }, 2000);
                    $('.btnVSubmit').show();

                    break;
                case "unsubmitted":
                    $('#gradeSubmissionStatus').show();
                    $('#gradeSubmissionStatus').text('Grades successfully unsubmitted.');
                    $('.rubric-score-assigned').attr("disabled", "disabled");
                    $('#txtassignedScore').removeAttr("disabled");
                    $('.btnVSubmit#btnSubmit').removeAttr("disabled");
                    $('.btnVSubmit#btnSubmit').removeClass("disabledButton");
                    $('.btnVSubmit#btnUnsubmit').attr("disabled", "disabled");
                    $('.btnVSubmit#btnUnsubmit').addClass("disabledButton");
                    $('.btnVSubmit#btnSave').removeAttr("disabled");
                    $('.btnVSubmit#btnSave').removeClass("disabledButton");
                    $('#divInstructorView .rubric-table-wrapper').removeClass('grade-submitted');
                    setTimeout(function () {
                        $('#gradeSubmissionStatus').hide('slow');
                    }, 2000);
                    $('.btnVSubmit').show();

                    break;
                case "saved":
                    $('#gradeSubmissionStatus').show();
                    $('#gradeSubmissionStatus').text('Grades successfully saved.');
                    setTimeout(function () {
                        $('#gradeSubmissionStatus').hide('slow');
                    }, 2000);
                    $('.btnVSubmit').show();
                    break;

                case "error":

                    $('#gradeSubmissionStatus').addClass("field-validation-error");
                    $('#gradeSubmissionStatus').show();
                    $('#gradeSubmissionStatus').text(message);
                    $('.btnVSubmit').show();

                    break;
                default:
                    alert('Error when saving/submitting your grades.');
                    $.unblockUI();
            }
        },

        ManualUnblockUI: function () {
            $.unblockUI();
        },

        OnAssignmentViewerClosed: function () {
            $.unblockUI();
            $('#spnSubmissionStatus').hide();
            if ($('#gradeSubmissionStatus').text() != '')
                $("#gridResults").trigger("reloadGrid");
        },

        OnDropboxUploadAndSubmitComplete: function () {
            var stats = $('.fne-action-status .pxunit-display-points');

            if (stats.find('.achievedPoints').length == 0) {
                stats.html("<span class='achievedPoints'>&mdash; / </span>" + stats.html());
                $('.fne-action-status').append('<span class="item-submitted"></span>');
            }

            PxAssignment.OnUploadAndSubmitComplete();
        },

        OnUploadAndSubmitComplete: function () {
            var args = {};
            $.post($('#gridSavedSubmissionAjaxUrl').val(), args, function (res) {
                if (tinyMCE.activeEditor) {
                    try {
                        tinyMCE.activeEditor.remove();
                    }
                    catch (ex) {
                    }
                }
                PxPage.Loaded('ui-dialog', true)
                $(".ui-icon-closethick").click();
                $("#doc-upload-and-submit").remove();
                $("#divComposeItems").html(res);
            });
        },

        OnUploadComplete: function () {
            $(".ui-icon-closethick").click();
            $("#gridStudentsSubmissions, #fne_gridStudentsSubmissions").trigger("reloadGrid");
        },

        OnUploadDocComplete: function (response) {
            $(".ui-icon-closethick").click();
            $("#content-item").replaceWith(response); //updated assignment with uploaded doc
        },

        DeletedSelectedDoc: function () {
            var selectedRows = $("#gridStudentsSubmissions, #fne_gridStudentsSubmissions").getGridParam('selarrrow');
            if (selectedRows.length) {
                if (confirm('Are you sure that you want to delete?')) {
                    var url = $(this).attr('path');
                    PxAssignment.ShowWaitOverlay('content');
                    $.post(url, { resourceIds: selectedRows.join(',') }, function (result) {
                        if (result == "success") {
                            $("#gridStudentsSubmissions, #fne_gridStudentsSubmissions").trigger("reloadGrid");
                        }
                        else {
                            alert('Error when deleting document(s).');
                        }
                        PxAssignment.HideOverlay('content');
                    });
                }
            }
            else {
                alert('Select atleast one row to delete.');
            }
            return false;
        },

        ToggleInstructions: function () {
            if ($('#toggle-instructions').val() == 'Hide Instructions') {
                $('#instructions-details').css({ 'visibility': 'hidden', 'display': 'none' });
                $('.sub-title').css({ 'visibility': 'hidden' });
                $('#toggle-instructions').attr('value', 'Show Instructions');
            }
            else if ($('#toggle-instructions').val() == 'Show Instructions') {
                $('#instructions-details').css({ 'visibility': 'visible', 'display': 'inline' });                
                $('.sub-title').css({ 'visibility': 'visible' });
                $('#toggle-instructions').attr('value', 'Hide Instructions');
            }
        },

        CloseHiddenPopUps: function () {
            $(".ui-dialog .ui-dialog-content").dialog('destroy');
        },
        ShowAssignmentFne: function () {
            var isFacePlate = $('.product-type-faceplate').length > 0;
            if (!isFacePlate) {
                PxAssignment.OnAssignmentEditorClosing();
                return false;
            }
            var itemId = $("#content-item-id").text();
            if (itemId == "") {
                itemId = $("#hidden-content-id").val();
            }
            var isFacePlate = $('.product-type-faceplate').length > 0;
            ContentWidget.ShowContentItem(itemId, 4, "", "");
        }
    };
} (jQuery);
