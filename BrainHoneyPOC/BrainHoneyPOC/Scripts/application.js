var dlapWrapper;
var inputPannel;

$(appInitialization);

/*
 * Initialization block
 */

// main initialization
function appInitialization() {

    // hardcoded params
    var dlapUrl = 'http://qa.dlap.bfwpub.com/dlap.ashx';
    var defaultUser = 'root/administrator';
    var defaultPassword = 'Password1';

    controlsInit();
    dlapInit(dlapUrl);
    inputPannelInit();
    authorizationInit(defaultUser, defaultPassword);
}

function controlsInit() {
    $("#button-edit").click(function (event) {
        runEditorEx($("#entityId").val(),
                    $("#questionId").val());
    });
}

function inputPannelInit() {
    inputPannel = new InputPanel($('#inputControls'), $('#waiting'));
}

function dlapInit(dlapUrl) {
    dlapWrapper = new DlapWrapper(dlapUrl);
}

function authorizationInit(defaultUser, defaultPassword) {
    dlapWrapper.getuser(function (result) {
        if (!isAuthorized(result.response)) {
            dlapWrapper.login(defaultUser, defaultPassword);
        }
    });
}

function isAuthorized(response) {
    if (response.code == "NoAuthentication") {
        return false;
    } else {
        return true;
    }
}


/*
 * logic block
 */


//  get enrollmentId then invoke callback:
//  callback(entityId, enrollmentId)
function getEnrollmentForQuestionEditor(entityId, callback) {
    
    // http://qa.dlap.bfwpub.com/js/docs/#!/Enum/RightsFlags
    var updateCourseFlag = 0x40000;
    dlapWrapper.getEntityEnrollmentList(entityId, updateCourseFlag, function (result) {

        if (!isResponseCodeOk(result.response.code)) {
            inputPannel.showControls();
            return;
        }

        if (result.response.enrollments.enrollment === undefined ||
                 result.response.enrollments.enrollment.length == 0) {
            alert('Enrollments not found');
            inputPannel.showControls();
            return;
        }

        callback(entityId, result.response.enrollments.enrollment[0].id);
    });
}

// get ItemId then invoke callback:
// callback(entityId, questionId, itemId);
function getItemIdForQuestionEditor(enrollmentId, entityId, questionId, callback) {
    dlapWrapper.getItemList(entityId, questionId, function (result) {

        if (!isResponseCodeOk(result.response.code)) {
            inputPannel.showControls();
            return;
        }

        if (result.response.items.item === undefined ||
                 result.response.items.item.length == 0) {
            alert('Items not found');
            inputPannel.showControls();
            return;
        }

        callback(enrollmentId, questionId, result.response.items.item[0].id);
    });
  
}

function isResponseCodeOk(code) {

    if (code != "OK") {
        alert('Error code: ' + code);
        return false;
    }

    return true;
}


function runEditorEx(entityId, questionId) {

    inputPannel.showLoad();

    getEnrollmentForQuestionEditor(entityId, function (entId, enrollmentId) {

        getItemIdForQuestionEditor(enrollmentId, entityId, questionId,
            function (enrollmernIdParam, questionIdParam, itemIdParam) {
                loadQuestionEditor(configureUrl(enrollmernIdParam, questionIdParam, itemIdParam));
            });

    });
}

