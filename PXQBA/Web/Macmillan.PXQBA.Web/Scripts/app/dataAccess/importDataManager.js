var importDataManager = (function () {

    self.importFile = function (fileId, courseId) {

        var request = {
            fileId: fileId,
            courseId: courseId
        };

        return $.ajax({
            url: window.actions.metadataCfg.importFileUrl,
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