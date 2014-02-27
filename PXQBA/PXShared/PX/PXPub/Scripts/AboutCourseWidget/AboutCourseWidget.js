   /*
    *   G. Chernyak
    */

var PxAboutCourseWidget = function ($) {

    return {
        BindControls: function () { 
        
        //add a new method to jquery validate to perform US phone number validation
        //In order to validate the dynamic fields this helper function can be given as a class.
        //Ex: <input type="text" name="ContactInfo[4].Info" id="ContactInfo_4_Info" class="InputForControllerAction txtContactInfo required Phone">
        $.validator.addMethod("Phone", function(phone_number, element) {
                phone_number = phone_number.replace(/\s+/g, "");                 
	            return this.optional(element) || phone_number.length > 9 && phone_number.match(/^(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})-?[2-9]\d{2}-?\d{4}$/);
        }, "Invalid phone number");

        //custom jquery validate function to validate email address
        $.validator.addMethod("Email", function(email_address, element) {

            var emailRegex = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
            if (emailRegex.test(email_address)) {
                return true;
            }
            else {
                return false;
            }

        }, "Invalid email address");

        //custom jquery validate function to validate Fax
        $.validator.addMethod("Fax", function(fax_number, element) {
                fax_number = fax_number.replace(/\s+/g, "");                 
	            return this.optional(element) || fax_number.length > 9 && fax_number.match(/^(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})-?[2-9]\d{2}-?\d{4}$/);
        }, "Invalid Fax");

        //custom jquery validate function to validate Other conatact type
        $.validator.addMethod("Other", function(other_value, element) {
              var otherRegex = /^[a-zA-Z\-0-9 ]+$/;
              if(otherRegex.test(other_value)) {
                    return true;
              }
              else {
                    return false;
              }
        }, "Invalid text");

        //validate the course information widget using Jquery validation plugin
        var validTitleRegexp = /^[A-Za-z0-9 \-.:'/"()]+$/;
        var validInstructorRegexp = /^[a-zA-Z0-9 ]+$/;
        var validOfficeHoursRegexp = /^[a-zA-Z\-0-9 ]+$/;
        $('#courseinformationform').validate({
            rules: {
                CourseName: { required: true, regex: validTitleRegexp },
                InstructorName: { required: true, regex: validInstructorRegexp },
                OfficeHours: { regex: validOfficeHoursRegexp }
                /* the validation for dynamic fields happens by embedding the validation rules inline ex: class="emailstyle required email" */
            },

            messages: {
                CourseName: { required: "Required", regex: 'Invalid course name.' },
                InstructorName: { required: "Required", regex: 'Invalid instructor name' },
                OfficeHours: { required: "Required", regex: 'Invalid office hours' }
            }
        });        

        //change the validation of the contact text based on the contact type chosen in the dropdown
        var previousType = '';
        $(document).off('focus', '.contactInfoType').on('focus', '.contactInfoType', function() { 
            previousType = $(this).val();            
        });

        $(document).off('change','.contactInfoType').on('change', '.contactInfoType', function() {
            var currentType = $(this).val();
            var contactText = $(this).closest('.contactInfoHolder').find('.txtContactInfo');
            contactText.val('');
            //remove the previous contactType class and the new contactType class to change the validation of the contact text entered
            contactText.removeClass(previousType);
            contactText.addClass(currentType);
        });

        //bind ValidateModalDialog method to the save button of the modal dialog 
        $(PxPage.switchboard).unbind("validateModalDialog", PxAboutCourseWidget.ValidateModalDialog);
        $(PxPage.switchboard).bind("validateModalDialog", PxAboutCourseWidget.ValidateModalDialog);

        
        $('#fileUploadForm').ajaxForm({
            beforeSubmit: ShowRequest,
            success: SubmitSuccesful,
            error: AjaxError
        });

        /*
        if ($("#SyllabusURL").val() == "") {
            $("#SyllabusURL").addClass("water_mark");
            $("#SyllabusURL").val("http://mysyllabus.com");
        }
        */

        //bind the remove button click
        $(document).off('click', '.removeContactInfo').on('click', '.removeContactInfo', function() {
            //remove the entire contact
            $(this).closest('.contactInfoHolder').remove();   
            //update the contact info counter
            var newCount = $.parseJSON($('#courseinformationform #hdnContactInfoCount').val()) - 1;
            $('#courseinformationform #hdnContactInfoCount').val(newCount);       
        });

        //bind the remove syllabus link
        $(document).off('click','.courseinformationwidget .removeSyllabus').on('click', '.courseinformationwidget .removeSyllabus', function() {

            //remove the link related syllabus details
            $('#SyllabusURL').val('');
            ClearWaterMark();
            AddWaterMark();

            //remove the file related syllabus details
            $('.courseinformationwidget #UploadResults').html('');
            $('.courseinformationwidget #postedFile').val('');
            $('.courseinformationwidget #RefUrlFilePath').val('');
            $('.courseinformationwidget #RefUrlFileName').val('');

        });

        }, 
              
        //validate the dialog, prepare the contact info submit form
        ValidateModalDialog : function() {

        var isFormValid = $('#courseinformationform').valid();

        //the syllabus link or the upload file path, also should be a valid url
        isFormValid = (isFormValid && ValidateURL())
        
        $('.ui-dialog').find('.isFormValid').val(isFormValid)

        //if the form is valid prepare the contact info list for submission
        if (isFormValid) {

            //clean up the syllabus link
            /*
            var syllabusUrl = $.trim($("#SyllabusURL").val());
            if(syllabusUrl == 'http://mysyllabus.com')
            {
                $("#SyllabusURL").val('');
            }
            */

            PxAboutCourseWidget.ContactInfoSubmission();
        }

     },

        // prepare the contact info list for submission
        // Ex:  <input type="hidden" class="InputForControllerAction" name = "ContacInfo[0].Type" value="Email"/>
        // Ex: <input type="hidden" class="InputForControllerAction" name = "ContacInfo[0].Info" value="someone@gmail.com"/>
        // the contact info submit thus prepared can be easily picked up by the widget dialog save functionality implemented in jquery.pageLayout.js
        ContactInfoSubmission : function() {
        var contactInfoSubmit = $('.contactInfoSubmit');

        var contactTypes = ['Phone', 'Email', 'Fax', 'Other'];
        var contactValues = [];
        var count = 0;

        var contactInfoHolders = $('#aboutEditContents .contactInfoHolder');
        $.each(contactTypes, function (index, contactType) {
            var contactValue = '';
                            $.each(contactInfoHolders, function (index, value) {
            if ($.trim($(this).find('.contactInfoType').val()) == contactType) {
                contactValue = $.trim($(this).find('.txtContactInfo').val());
            }
        });
            contactValues.push(contactValue);

            var contactTypeHtml = '<input type="hidden" class="InputForControllerAction" name = "contactInfo[' + count + '].Type" value="' + contactType + '"/>';
            var contactValueHtml = '<input type="hidden" class="InputForControllerAction" name = "contactInfo[' + count + '].Info" value="' + contactValue + '"/>';
            contactInfoSubmit.append(contactTypeHtml);
            contactInfoSubmit.append(contactValueHtml);

            count++;
        });
    }



    }


} (jQuery);



function ShowRequest(formData, jqForm, options) {

    if (formData[0].value == null || formData[0].value == undefined || formData[0].value == "") {
        $('#UploadResults').html('Please select a file!');
        return false;
    }
        var queryString = $.param(formData);
        var message = queryString.replace("postedFile=", "Uploading file ");
        $('#UploadResults').html(message);

    return true;
}

function AjaxError(responseText, statusText) {
    alert("An AJAX error occurred." + responseText);
}

function SubmitSuccesful(result, statusText) {
    $('#UploadResults').html(result.resultMessage.UploadMessage);
    $('#RefUrlFilePath').val(result.resultMessage.UploadPath);
    $('#RefUrlFileName').val(result.resultMessage.UploadFileName);
    $('#isUploadValid').val('true');
}

function addFields() {
    var newId = $.parseJSON($('#courseinformationform #hdnContactInfoCount').val());
    var num = 0;
    var elem = '<div id="' + newId + '" class="contactInfoHolder"><div id="ContactType_' + newId + '"><div id="ddlContactTypeContainer_' + newId + '" class="fieldtext"><select class="contactInfoType" id="ContactInfo_' + newId + '_Type" name="ContactInfo[' + newId + '].Type"><option>Phone</option><option>Email</option><option>Fax</option><option>Other</option></select></div><input class="txtContactInfo Phone" type="text" id="ContactInfo_' + newId + '_Info" name="ContactInfo[' + newId + '].Info" /><div class="subtractfield"><a class="removeContactInfo">-</a></div></div></div>';
    $('#aboutEditContents').append(elem);
    $('#courseinformationform #hdnContactInfoCount').val(newId + 1);  
}

function removeFields(f) {
    if (f > 0) {
        $('#' + f).remove();
    }
}

function showUpload() {
    $('#uploadblock').show();
    $('#syllabusblock').hide();
    $('#UploadResults').html('');
    $('#isUploadValid').val('false');  //invalidate the upload form
    $('#aboutCourseLinkType').val('File');
}

function showLinkBox() {
    $('#syllabusblock').show();
    $('#postedFile').val('');
    $('#uploadblock').hide();
    $('#UploadResults').html('');
    $('#isUploadValid').val('false');  //invalidate the upload form
    $('#aboutCourseLinkType').val('Url');

    //default the syllabus url
    $("#SyllabusURL").val('');
    ClearWaterMark()
    AddWaterMark();
}

function ClearWaterMark() {
    var hasWaterMark = $("#SyllabusURL").hasClass("water_mark");
    if(hasWaterMark) {
        $('#SyllabusURL').val('');
        $("#SyllabusURL").removeClass("water_mark");
    }
}

function AddWaterMark() {

    var hasWaterMark = $("#SyllabusURL").hasClass("water_mark");

    //if the textbox is empty and has no watermark class implement watermark on it
    /*
    if (hasWaterMark == false && $('#SyllabusURL').val() == '') {
        $('#SyllabusURL').val('http://mysyllabus.com');
        $("#SyllabusURL").addClass("water_mark");
    }
    */

    //if a url is already present in the textbox then validate the same
    if (hasWaterMark == false && $('#SyllabusURL').val().length > 1) {
        ValidateURL();
    }
}

//validates the syllabus url and upload file functionality
function ValidateURL() {

    //validation happens based on the radio button clicked (Link/Upload)
    var rdchecked = $('.urlform input[type=radio]:checked').val();

    if(rdchecked == "Link")
    {
        var urlRegex = new RegExp("(https?|http)\://|www\.[A-Za-z0-9\.\-]+( /[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*");
        var syllabusUrl = $("#SyllabusURL").val();
        var defaultUrl = 'http://mysyllabus.com';

        if ( !urlRegex.test(syllabusUrl) ) {       
            $('#UploadResults').html("Please enter a valid URL");   
            return false;
        }
        else {
            $('#UploadResults').html("");            
        }
    }

    return true;
}



