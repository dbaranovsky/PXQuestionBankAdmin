var questionFilterDataManager = (function () {
    var self = {};
    
    self.getQuestionTypeList = function() {
        return $.ajax({
            url: window.actions.questionFilter.getQuestionTypeListUrl,
             cache: false,
            dataType: 'json',
            type: 'GET'
        }).done(function(response) {
        });
    };


    return self;
}());