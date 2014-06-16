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
            self.showErrorPopup();
        });
    }

     self.getNewRoleTemplate = function(){

               
        return $.ajax({
            url: window.actions.userOperations.getNewRoleTemplateUrl,
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json'
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    }

      self.getRolesCapabilities = function(roleId, courseId){

           var request = {
            roleId: roleId,
            courseId: courseId
        };
               
        return $.ajax({
            url: window.actions.userOperations.getRoleCapabilitiesUrl,
            data: request,
            dataType: 'json',
            type: 'GET'   
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    }


      self.saveRole = function(role, courseId){
          var request = {
            role: role,
            courseId: courseId
        };
               
        return $.ajax({
            url: window.actions.userOperations.saveRoleUrl,
             data: JSON.stringify(request),
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            self.showSuccessPopup("Role successfully created")
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    }

    self.getUsers = function(){

        return $.ajax({
            url: window.actions.userOperations.getUsersUrl,
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json'
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    }

    self.getTitlesWithRolesForUser = function(userId){

         var request = {
            userId: userId
        };

        return $.ajax({
            url: window.actions.userOperations.getTitlesWithRolesForUserUrl,
            dataType: 'json',
            data: JSON.stringify(request),
            type: 'GET',
            contentType: 'application/json'
        }).done(function (response) {
            
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    }

   self.showErrorPopup = function() {
        var notifyOptions = {
            message: { text: window.enums.messages.errorMessage },
            type: 'danger',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $('.top-center').notify(notifyOptions).show();
    };

    self.showSuccessPopup = function(message){
         var notifyOptions = {
            message: { text: message },
            type: 'success',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $('.top-center').notify(notifyOptions).show();
    };

    self.showWarningPopup = function (message) {
        var notifyOptions = {
            message: { text: message },
            type: 'warning',
            fadeOut: { enabled: true, delay: 3000 }
        };
        $('.top-center').notify(notifyOptions).show();
    };


    self.resetNotifications();

    return self;
}());