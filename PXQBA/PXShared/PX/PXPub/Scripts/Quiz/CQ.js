if (typeof xmlencode !== 'function') {
    function xmlencode(string) {
        return string.replace(/\&/g, '&' + 'amp;').replace(/</g, '&' + 'lt;')
            .replace(/>/g, '&' + 'gt;').replace(/\'/g, '&' + 'apos;').replace(/\"/g, '&' + 'quot;');
    }
}

if (typeof CQ === 'undefined') {
    window.CQ = {
        getInfo: function (id) {
            return this.questionInfoList[id];
        },
        setAnswer: function (id, answer) {
            var el = document.getElementById(id);
            if (el) el.value = answer;
            this.getInfo(id).submission.answer = answer;
        },
        onBeforeSave: function (method, data) { this.beforeSaveList.push({ method: method, data: data }); },
        beforeSaveList: [],
        questionInfoList: [],
        beforeSave: function () {
            for (var index = 0; index < this.beforeSaveList.length; index++)
                this.beforeSaveList[index].method(this.beforeSaveList[index].data);
        }
    };

    CQ.questionInfoList[9999] = {
        divId: 'custom1'
    , version: '1'
    , mode: 'Active'
    , question: { body: 'Test Question ADVANCED', data: '', response: { pointspossible: 1, pointsassigned: NaN } }
    };
}
//Singleton that implements the CQ javascript API
//window.CQ = function ($) {

//    var _divId = "hts-editor-ui";
//    var _setDataSel = "#hts-set-data";

//    //stores callback registered with onGetQuestionData
//    var _cb_getQuestionData = {
//        func: null,
//        data: null
//    };

//    //Gets information about the custom question editing context
//    var _getInfo = function (id) {
//        var base = {
//            divId: _divId,
//            mode: 'Edit'
//        };

//        return base;
//    };

//    //Registers a callback method that the assessment editor calls 
//    //whenever it needs updated question data. This could be in 
//    //response to a preview or a save request from the user, or 
//    //a timed auto-save event.
//    var _onGetQuestionData = function (method, data) {
//        _cb_getQuestionData.func = method;
//        _cb_getQuestionData.data = data;

//        $('.question-editor button.save').unbind('click').click(method);
//    };

//    //Sets the updated data for a question. This method may be called
//    //while servicing GetQuestionData requests with the function 
//    //registered by calling CQEdit.onGetQuestionData, or it may be 
//    //called after finishing servicing GetQuestionData requests. 
//    //(The assessment editor will wait until CQ.setQuestionData is 
//    //called before using the updated question data.)
//    var _setQuestionData = function (id, data) {
//        if ($(".hts-data .hts-text").val() == "") {
//            alert("Please enter a Title for the HTS question.");
//        } else {
//            PxPage.Loading('fne-content');
//            $.post(PxPage.Routes.save_hts_question, {
//                questionId: id,
//                dataContents: xmlencode(data),
//                text: $(".hts-data .hts-text").val(),
//                points: $(".hts-data .hts-points").val()
//            },
//            function (response) {
//                PxPage.Loaded('fne-content');
//                alert("Question Saved");
//                PxPage.log('saved question: ' + response);
//            });
//        }
//    };

//    //resets the CQ object when a new question is created
//    var _newQuestion = function () {
//        _cb_getQuestionData = { func: null, data: null };
//    };

//    //triggers the onGetQuestionData callback
//    var _saveQuestion = function () {
//        if (_cb_getQuestionData.func != null && typeof (_cb_getQuestionData.func) == "function") {
//            _cb_getQuestionData.func(_cb_getQuestionData.data);
//        }
//    };

//    //grabs elements from the active editor
//    var _activateEditor = function (editor) {
//        _divId = editor + "-editor-ui";
//        _setDataSel = "#" + editor + "-set-data";
//    };

//    var _setAnswer = function (id, answer) {
//    };

//    var _onBeforeSave = function (method, data) {
//        if ($.isFunction(method)) {
//            method();
//        }
//    };

//    //the object's public interface
//    return {
//        getInfo: _getInfo,
//        setAnswer: _setAnswer,
//        onBeforeSave: _onBeforeSave,
//        onGetQuestionData: _onGetQuestionData,
//        setQuestionData: _setQuestionData,
//        newQuestion: _newQuestion,
//        activateEditor: _activateEditor,
//        saveQuestion: _saveQuestion
//    };

//} (jQuery);