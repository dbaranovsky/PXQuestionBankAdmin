var titleDataManager = (function () {

    self.getTitles = function () {

        return $.ajax({
            url: window.actions.questionTitle.getTitleDataUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function (e) {
             
        });
    };

    self.processDataResponse = function(response) {

    };

    return self;
}());