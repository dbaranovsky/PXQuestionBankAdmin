
/*
This file contains js functions need in the implementaion of environments module
functions are called by partial views to add, update, delete environments
Some of the function are implemeneted to provided client side, validation and data processing
*/



// data.Result = bool to indicate if server code had success or not
// data.EnvironmentId = id of the environment
// data.Action = possible values are 'Added' or 'Updated'
function onAddUpdateEnvironment(data) {
    var message = "";

    showLoading();

    if (data.Result) {
        if (data.Action == "Added") {
            var envIdSelector = "input[id='EnvironmentId'][value='0']";  // selector for hiddend field that holder EnvironmentId
            var form = $(envIdSelector).closest("form"); // get reference for form before changing the value of hidden variable

            $(form).attr("id", "EnvForm" + data.EnvironmentId);

            $(envIdSelector).val(data.EnvironmentId);

            var envHeader = $(form).find(".envHeader");
            var envTitle = $(envHeader).find("#Title").val();

            // add new env to dropdown on site
            var ddlbEnv = GetEnvironmentDropdown();
            var newOption = '<option value = "' + envTitle + '">' + envTitle + '</option>';
            $(ddlbEnv).append(newOption);

            message = "Environment was added successfully";
        }
        else { // env updates
            message = "Environment was updated successfully";
        }

        EnableEnvironment({ envId: data.EnvironmentId, isEnable: false });

        var currentEnvId = GetCurrentEnvironmentId();

        if (currentEnvId == data.EnvironmentId) {
            // since current env has been updated, we need to update links in the main menu

            envIdSelector = "input[id='EnvironmentId'][value='" + data.EnvironmentId + "']";  // selector for hiddend field that holder EnvironmentId
            form = $(envIdSelector).closest("form"); // get reference for form before changing the value of hidden variable

            ChangeMenuLinkUrl('DLAP', form, 'DlapServer');
            ChangeMenuLinkUrl('BrainHoney', form, 'BrainHoneyServer');
            ChangeMenuLinkUrl('BrainHoneyDocs', form, 'BrainHoneyDocs');
            ChangeMenuLinkUrl('PXDocs', form, 'PxDocs');
        }
    }
    else {
        alert(data.Message);
    }

    hideLoading()
}



// this function is called to update the links in the main menu, when current environment is updated
// menu         id: is is used to find the anchor tag for menu
// form:        is the jquery object for the form which has environment details
// formFieldId: id of the txtbox in the form to get url from
function ChangeMenuLinkUrl(menuId, form, formFieldId) {
    var menuItem = $("a[id='" + menuId + "']")
    var newUrl = form.find("#" + formFieldId).val();
    menuItem.attr("href", newUrl);
}



// based on the argument 'delButton', get the env id and call server to delete the anv
function DeleteEnvironment(delButton) {

    var answer = confirm("Are you sure you wish to delete this environment?");
    if (!answer) {
        return;
    }

    showLoading();

    var form = $(delButton).closest("form");

    var envId = $(form).find("#EnvironmentId").val();
    //var envTitle = $(form).find(".envHeader").text().trim();
    var envTitle = $(form).find("#Title").val();
    
    if (envId > 0) {
        $.post("/dev/Environment/DeleteEnvironment", { environmentId: envId }, function (data) {
            var ddlb = GetEnvironmentDropdown();
            var option = ddlb.find("option[value='" + envTitle + "']");
            option.remove();
            //GetEnvironmentDropdown().find("option[value='" + envTitle + "']").remove();
            //alert(data.Message);
        });
    }

    form.remove();

    var currentEnvId = GetCurrentEnvironmentId();

    if (currentEnvId == envId) {
        var loc = window.location.protocol + "//" + window.location.host + "/home/index";
        window.location = loc;
    }

    hideLoading()
}


// get source text from textbox and add it to the listbox
// validate it using regex and also make sure that if the source is in the list, do not add it again
function AddSourceToList(addButton) {
    var form = $(addButton).closest("form");
    var listbox = $(form).find("#Sources");
    var newSource = $.trim($(form).find("#txtAddSource").val());

    ShowErrorEnvironmentForm("", form);
    if (newSource == "") {
        return;
    }

    var regExp = /^[ a-z0-9_-]+$/i;
    if (!regExp.test(newSource)) {
        isValid = false;
        var msg = "<ul><li>Source can only have alphanumeric, - and _ )</li></ul>";
        ShowErrorEnvironmentForm(msg, form);
        return;
    }

    var alreadyExists = false;
    listbox.find("option").each(function (index, option) {
        if ($(this).text() == newSource) {
            alreadyExists = true;
        }
    });

    if (!alreadyExists) {
        var newOption = '<option value = "' + newSource + '">' + newSource + '</option>';
        $(listbox).append(newOption);
    }

    $(form).find("#txtAddSource").val("");
}


// remeve selected resources from the listbox of resources
function RemoveSourceFromList(removeButton) {
    var form = $(removeButton).closest("form");
    var listbox = $(form).find("#Sources");

    listbox.find(":selected").each(function (index, option) {
        $(this).remove();
    });
}


// this function enables/disables UI env form
// oArg can have following 3 properties
// sender   : button used to enable
// envId    : id of the environment, which is in the form as hidden field
// isEnable : boolean value to indicate, whether to enable or disable
// when enabled, "hide" button is removed and save button is shown
// when disabled, "save" button is removed and "edit" button is shown
function EnableEnvironment(oArg) {

    var form = null;
    if (oArg.sender != null) {
        form = $(oArg.sender).closest("form");
    }
    else if (oArg.envId != null) {
        var envIdSelector = "input[id='EnvironmentId'][value='" + oArg.envId + "']";  // selector for hiddend field that holder EnvironmentId
        form = $(envIdSelector).closest("form");
    }

    var isEnable = true;
    if (oArg.isEnable != null) {
        isEnable = oArg.isEnable;
    }


    if (isEnable) {
        $(form).find(".envField").removeAttr("disabled");
        $(form).find("#Title").css("background", "white");
    }
    else {
        $(form).find(".envField").attr("disabled", "disabled");
        $(form).find("#Title").css("background", "transparent");
    }

    if (isEnable) {
        $(form).find("#Edit").hide();
        $(form).find("#delete").hide();
        $(form).find("#Cancel").show();
        $(form).find("#submit").show();
        $(form).find("#divAddResource").show();
        $(form).find("#removeResource").show();
    }
    else {
        $(form).find("#Edit").show();
        $(form).find("#delete").show();
        $(form).find("#Cancel").hide();
        $(form).find("#submit").hide();
        $(form).find("#divAddResource").hide();
        $(form).find("#removeResource").hide();
    }
}


// when user clicks cancel:-
// remove env form if it was a new env
// retreive form again from server and replace existing form with it, becasue user could have potientially chnaged the text
function CancelEnv(cancelButton) {
    var form = $(cancelButton).closest("form");
    var environmentId = form.find("#EnvironmentId").val();
    if (environmentId == 0) {
        // since env is not saved and does not exist, just remove the form
        form.remove();
    }
    else {

        // Remove the form and add it again by getting it from server
        // because user might have changed the data, and we need to revert it back to original state

        $.get("/dev/Environment/AddEnvironment", { environmentId: environmentId }, function (data) {

            form.before(data);
            form.remove();
            // make the env disabled so that buttons can be hidden/shown correctly
            EnableEnvironment({ envId: environmentId, isEnable: false });
        });
    }
}


// This function has code to validate form, before saving it
function ValidateForm(submitButton) {
    var form = $(submitButton).closest("form");
    ShowErrorEnvironmentForm("", form);

    var isValid = true;
    var errorList = "Please correct following:<ul>";

    var title = $(form).find("#Title").val();
    GetEnvironmentDropdown().find("option").each(function () {
        if ($(this).val() == title) {
            isValid = false;
            errorList += "<li>An environmemnt with this Title already exists, please select a different title.</li>";
        }
    });

    var regExp = /^[a-z0-9_-]+$/i;
    if (!regExp.test(title)) {
        isValid = false;
        errorList += "<li>Title must be provided (only alphanumeric, - and _ are allowed with no spaces)</li>";
    }

    var description = $(form).find("#Description").val();
    regExp = /^[ a-z0-9-,_!.]*$/i;
    if (!regExp.test(description)) {
        isValid = false;
        errorList += "<li>Description can only have alphanumeric, - , _ , !, comma and period</li>";
    }

    regExp = /^(http|https):\/\/(\w+:{0,1}\w*@@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@@!\-\/]))?/i; // for valid url
    var dlapServerUrl = $(form).find("#DlapServer").val();
    if (dlapServerUrl != "" && !regExp.test(dlapServerUrl)) {
        isValid = false;
        errorList += "<li>DLAP Server should be a valid url</li>";
    }

    var brainhoneyUrl = $(form).find("#BrainHoneyServer").val();
    if (brainhoneyUrl != "" && !regExp.test(brainhoneyUrl)) {
        isValid = false;
        errorList += "<li>BrainHoney Server should be a valid url</li>";
    }

    var pxDocsUrl = $(form).find("#PxDocs").val();
    if (pxDocsUrl != "" && !regExp.test(pxDocsUrl)) {
        isValid = false;
        errorList += "<li>PX Docs should be a valid url</li>";
    }

    var brainHoneyDocsUrl = $(form).find("#BrainHoneyDocs").val();
    if (brainHoneyDocsUrl != "" && !regExp.test(brainHoneyDocsUrl)) {
        isValid = false;
        errorList += "<li>Brain honey Docs  should be a valid url</li>";
    }

    errorList += "</li>";

    if (!isValid) {
        ShowErrorEnvironmentForm(errorList, form);
    }

    // if all validation succeeds, select all items in listbox of source so that they can se sent to server

    var listbox = $(form).find("#Sources");
    listbox.find("option").each(function (index, option) {
        $(this).attr("selected", "selected");
    });

    return isValid;
}

function ShowErrorEnvironmentForm(errorText, form) {
    var errorDiv = $(form).find("#error");
    errorDiv.html(errorText);
}