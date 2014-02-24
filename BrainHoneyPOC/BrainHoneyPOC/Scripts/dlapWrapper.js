/*
    Base wrapper over dlap
    
    Example of usage:
    var wrapper = new DlapWrapper('http://qa.dlap.bfwpub.com/dlap.ashx');
    wrapper.login('root/administrator','Password1', function(e) {});
    wrapper.getEntityEnrollmentList2(6712, function(e) {}, 0x40000);
 */
function DlapWrapper(dlapUrl) {

    if (!(this instanceof DlapWrapper)) {
        return new DlapWrapper(dlapUrl);
    }

    this.dlapUrl = dlapUrl;

    var defaultErrorCallback = function (result) {
        alert('Error Code: ' + result.status);
    };

    var buildErrorCallback = function(errorCallback) {
        if (errorCallback === undefined) {
            return defaultErrorCallback;
        }
        return errorCallback;
    };

    var buildSuccessCallBack = function (callback) {
        if (callback === undefined) {
            callback = function () {
            };
        }
        return callback;
    };

    // see http://qa.dlap.bfwpub.com/js/docs/#!/Command/Login
    var loginDlap = function (username, password, successCallback, errorCallback) {
        var data = {
            cmd: "login",
            username: username,
            password: password
        };

        $.ajax({
            type: "POST",
            url: dlapUrl,
            dataType: "jsonp",
            data: data,
            crossDomain: true,
            cache: false
        }).error(buildErrorCallback(errorCallback)).success(buildSuccessCallBack(successCallback));
    };

    // see http://qa.dlap.bfwpub.com/js/docs/#!/Command/GetUser2
    var getuser2 = function (successCallback, errorCallback) {

        $.ajax({
            type: "POST",
            url: dlapUrl,
            dataType: "jsonp",
            data: {
                cmd: "getuser2"
            },
            crossDomain: true,
            cache: false
        }).error(buildErrorCallback(errorCallback)).success(buildSuccessCallBack(successCallback));
    };

    // see http://qa.dlap.bfwpub.com/js/docs/#!/Command/GetEntityEnrollmentList2
    var getEntityEnrollmentList2 = function (entityid, flags, successCallback, errorCallback) {

        var urlParams = '?cmd=getentityenrollmentlist2&entityid=[entityid]&flags=[flags]';
        urlParams = urlParams.replace('[entityid]', entityid);

        if (flags === undefined) {
            urlParams = urlParams.replace('&flags=[flags]', '');
        } else {
            urlParams = urlParams.replace('[flags]', flags);
        }

        $.ajax({
            type: "GET",
            url: dlapUrl + urlParams,
            dataType: "jsonp",
            crossDomain: true,
            cache: false
        }).error(buildErrorCallback(errorCallback)).success(buildSuccessCallBack(successCallback));
    };

    var getItemList = function (entityId, questionId, successCallback, errorCallback) {

        var urlParams = '?cmd=getitemlist&entityid=[entityid]&query=/Questions/question@id="[questionId]"';
        urlParams = urlParams.replace('[entityid]', entityId).replace('[questionId]', questionId);

        $.ajax({
            type: "GET",
            url: dlapUrl + urlParams,
            dataType: "jsonp",
            crossDomain: true,
            cache: false
        }).error(buildErrorCallback(errorCallback)).success(buildSuccessCallBack(successCallback));

    };

    return {
        login: loginDlap,
        getuser: getuser2,
        getEntityEnrollmentList: getEntityEnrollmentList2,
        getItemList: getItemList
    };
}

