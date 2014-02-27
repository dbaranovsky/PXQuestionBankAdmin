

// Call this function to get the dropdown, (on master page) which has environment options
function GetEnvironmentDropdown() {
    return $("select[id='Environment']");
}

function GetCurrentEnvironment() {
    var env = GetEnvironmentDropdown().find(":selected").text();
    return env;
}

function GetCurrentEnvironmentId() {
    var env = GetEnvironmentDropdown().find(":selected").val();
    return env;
}

// show gif file loading data from server
function showLoading() {
    $("#loading").show();
}


// hide gif after data has been recoverved from server
function hideLoading() {
    $("#loading").hide();
}