var filterDataManager = (function () {
    var self = {};

    self.questionTypeList = [];
    $.ajax({
        url: window.actions.questionFilter.getQuestionTypeListUrl,
        dataType: 'json',
        type: 'GET'
    }).done(function (response) {
        self.questionTypeList = response;
    });

    self.getQuestionTypeList = function () {
        return self.questionTypeList;
    };

    return self;
}());