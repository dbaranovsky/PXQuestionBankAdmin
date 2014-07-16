var importDataManager = (function () {

    self.importFile = function (fileId, courseId) {

        var request = {
            fileId: fileId,
            courseId: courseId
        };

        return $.ajax({
            url: window.actions.importActions.importFileUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {
           
        }).error(function (e) {

        });
    };

    self.saveQuestionsForImport = function(questionsId) {
        var request = {
            questionsId: questionsId
        };
        
        return $.ajax({
            url: window.actions.importActions.saveQuestionsForImportUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {

        }).error(function (e) {

        });
    };

    self.importQuestionsTo = function (courseId) {
        var request = {
            toCourseId: courseId
        };

        return $.ajax({
            url: window.actions.importActions.importQuestionsToUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {

        }).error(function (e) {

        });
    };

    return self;
}());