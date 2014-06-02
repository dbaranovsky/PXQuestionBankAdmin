var metadataCfgDataManager = (function () {

    self.getAvailableCourses = function () {

        return $.ajax({
            url: window.actions.metadataCfg.getAvailableCoursesUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function (e) {

        });
    };


    self.getMetadataConfig = function (courseId) {
        var request = {
            courseId: courseId
        };

        return $.ajax({
            url: window.actions.metadataCfg.getMetadataConfigUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function (e) {

        });
    };

    self.saveMetadataConfig = function(metadataConfigViewModel) {
        var request = {
            metadataConfig: metadataConfigViewModel
        };

        return $.ajax({
            url: window.actions.metadataCfg.saveMetadataConfigUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function (e) {

        });
    };

    self.processDataResponse = function (response) {

    };

    return self;
}());