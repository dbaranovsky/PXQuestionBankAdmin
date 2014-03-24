var questionMetadataManager = (function () {
    var self = {};

    self.availableFields = {};
    $.ajax({
        url: window.actions.questionList.availableFieldsUrl,
        dataType: 'json',
        type: 'GET'
    }).done(function (response) {
        self.availableFields = response;
    });

    self.getQuestionAvailableFields = function () {
        return self.availableFields;
    };

    return self;
}());