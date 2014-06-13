var userManager = (function() {
    var self = {};

    self.notifications = [];

   
    self.resetNotifications = function(){


        return $.ajax({
            url: window.actions.userOperations.getCurrentUserNotificationForUrl,
            traditional: true,
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json'
        }).done(function (response) {
            self.setNotifications(response);
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
         //   self.showErrorPopup();
        });
    };

    self.setNotifications = function(notifications){
        self.notifications = notifications;
    };

    self.getNotificationById = function(typeId){

        if (self.notifications.length == 0){
            return null;
        }

        return $.grep(self.notifications, function(el){ return el.notificationTypeId == typeId;})[0];
    }

    self.dontShowForCurrentUser = function(typeId){


        var request = {
            type: typeId
        };

        var type = self.getNotificationById(typeId);
        type.isShown = false;

        self.notifications = $.grep(self.notifications, function(el){ return el.notificationTypeId != typeId;});
        self.notifications.push(type);

        return $.ajax({
            url: window.actions.userOperations.dontShowForCurrentUserUrl,
            traditional: true,
            data: JSON.stringify(request),
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
        });
    }

    self.getRolesForCourse = function(courseId){

        var request = {
            courseId: courseId
        };
        
        return $.ajax({
            url: window.actions.userOperations.getRolesForCourseUrl,
            data: JSON.stringify(request),
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            questionDataManager.showErrorPopup();
        });
    }

    self.resetNotifications();

    return self;
}());