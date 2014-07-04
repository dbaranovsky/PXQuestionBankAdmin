var compareTitlesDataManager = (function () {

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


    self.getComparedData = function (firstCourse, secondCourse, page) {
        var request = {
            firstCourse: firstCourse,
            secondCourse: secondCourse,
            page: page
        };
        asyncManager.startWait();
        return $.ajax({
            url: window.actions.compareCourse.getComparedDataUrl,
            traditional: true,
            dataType: 'json',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
            asyncManager.stopWait();
        }).error(function (e) {
           asyncManager.stopWait();
        });
    };

    self.processDataResponse = function (response) {

    };

    return self;
}());