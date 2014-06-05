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

     self.resetNotifications();

    return self;
}());