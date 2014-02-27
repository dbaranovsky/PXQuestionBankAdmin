var globalEditRoute, globalShowRoute, globalSaveRoute;
$(document).ready(function () {
    
});

var PXProfileSummaryWidget = function ($) {
    setTimeout(function () { $(PXProfileSummaryWidget.Init); }, 10);

    return {

        Init: function () {
            globalEditRoute = PxPage.Routes.edit_eportfolio_profilewidget;
            globalShowRoute = PxPage.Routes.show_eportfolio_profilewidget;
            globalSaveRoute = PxPage.Routes.save_eportfolio_profilewidget;

        },


        ShowProfileSummaryEditorWidgetModal: function (obj) {

            if (obj.isStudentView == true) {
                return false;
            }

            var showProfileSummaryEditorWidgetModal = $('.showProfileSummaryEditorWidgetModal').first();
            $(showProfileSummaryEditorWidgetModal).html('<div style="padding-top:10px;padding-bottom:10px;">Loading..</div>');
            $(showProfileSummaryEditorWidgetModal).attr('id', 'showProfileSummaryEditorWidgetModal');
            $(showProfileSummaryEditorWidgetModal).attr('style', 'z-index:9999;position:relative;');
            $(showProfileSummaryEditorWidgetModal).dialog({
                width: 700,
                height: 350,
                minWidth: 400,
                minHeight: 350,
                modal: true,
                resizable: false,
                draggable: false,
                closeOnEscape: true,
                title: 'Edit your profile'
            });
            var bodyPostContent = $.ajax({
                type: "POST",
                url: globalEditRoute,
                data: {
                    userId: obj.userId,
                    userRefId: obj.userRefId,
                    firstName: obj.firstName,
                    lastName: obj.lastName,
                    email: obj.email,
                    imageUrl: obj.imageUrl
                },
                success: function (data) {
                    $(showProfileSummaryEditorWidgetModal).html(data);
                    $(".ui-dialog-content").attr("style", "font-size:12px;height: 350px;overflow:auto;");
                },

                fail: function (msg) {
                    alert(msg);
                }
            });
        },

        RedirectToEportfolioDashboard: function (obj) {
            if (obj.isStudentView == true) {
                return false;
            }
            window.location.href = obj.dashboardLnk;
        },

        Save: function (obj) {
            var instructorName = obj.firstname + ' ' + obj.lastname;
            var email = obj.email;
            var isInstructor = obj && obj.access && obj.access == "instructor";
            $('#frmProfileEditorWidget').ajaxSubmit({
                data: { userRefId: obj.userRefId },
                success: function (response) {
                    $('.showProfileSummaryEditorWidgetModal').remove();
                    var x = $('#PX_UserProfileWidget').find('#widgetBody');
                    x.html(response);

                    var courseTitleDiv = $.trim($('.course-title-lable').html());
                    var instructorNameLink = $('.instructor-name a');
                    var accountActionList = $('#accountActionsList option[value="user"]');

                    if (accountActionList.length > 0) accountActionList.html(instructorName + ' -  ' + courseTitleDiv);

                    if (isInstructor && instructorNameLink.length > 0) {
                        instructorNameLink.html('Instructor: ' + instructorName);
                        instructorNameLink.attr('href', 'mailto:' + email);
                    }

                    return true;
                }
            });
        },
        Cancel: function () {

            if (confirm("Are you sure you want to close without saving?")) {
                var showProfileSummaryEditorWidgetModal = $('.showProfileSummaryEditorWidgetModal');
                showProfileSummaryEditorWidgetModal.dialog('close');
            }
            return false;
        },

        ValidateProfileWidget: function () {
            var retval = true;
            if (!PxEportfolioDashboard.ValidateName(true, '#FirstName', '#FirstNameErrMsg')) {
                retval = false;
            }

            if (!PxEportfolioDashboard.ValidateName(true, '#LastName', '#LastNameErrMsg')) {
                retval = false;
            }

            var fieldValue = $('#Email').val();
            if (!PxEportfolioDashboard.ValidateEmail(fieldValue, '#EmailErrMsg')) {
                retval = false;
            }

            return retval;
        },

        ValidateEmail: function (emailAddress, messageField) {
            //validate email
            var retval = false;
            var email = $.trim(emailAddress);
            var emailRegEx = /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i;

            if (!emailRegEx.test(email)) {
                //alert(messageField);
                if ($(messageField).length > 0)
                    $(messageField).show();

                return false;
            } else {
                if ($(messageField)) {
                    $(messageField).hide();
                }
            }

            return true;
        },

        ValidateName: function (validateBlank, titleSelector, messageField) {
            var sel = 'input.title';
            if (titleSelector)
                sel = titleSelector;

            if ($(sel).val().indexOf('<') != -1 ||
				$(sel).val().indexOf('>') != -1) {
                //alert('Name cannot contain html tags.');
                if ($(messageField).length > 0)
                    $(messageField).show();

                $(sel).focus();
                return false;
            }

            if (validateBlank != null) {
                var title = $(sel).val();
                title = jQuery.trim(title);
                if (title == '') {
                    if ($(messageField).length > 0)
                        $(messageField).show();
                    //alert('Name cannot be blank');
                    $(sel).focus();
                    return false;
                }
            }

            $(messageField).hide();

            return true;
        }
    };


} (jQuery);

