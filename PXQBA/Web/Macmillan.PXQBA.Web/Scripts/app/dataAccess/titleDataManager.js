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

      self.addNewRepository = function (name) {
        var request = {
            name: name
        };
        return $.ajax({
            url: window.actions.questionTitle.addNewRepositoryUrl,
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
    };

     self.addSiteBuilderRepository = function (url) {
          var request = {
              url: url
          };
          return $.ajax({
              url: window.actions.questionTitle.addSiteBuilderRepositoryUrl,
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
      };


    self.processDataResponse = function(response) {

    };

    return self;
}());