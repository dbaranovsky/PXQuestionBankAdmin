var questionDataManager = (function() {
    var self = {};

    self.cache = {};

    self.processDataResponse = function (response) {
        //save in cache here
    };

    self.loadFromCache = function (filter, page, orderType, orderField) {
        // load from cache here
        return null;
    };

    self.getQuestionsByQuery = function (filter, page, columns, orderType, orderField) {
        
        var cacheResult = self.loadFromCache(filter, page, orderType, orderField);

        if (cacheResult != null) {
            return $.Deferred(function() {
                this.resolve(cacheResult);
            });
        }

        var request = {
            filter: filter,
            pageNumber: page,
            orderType: orderType, 
            orderField: orderField,
            columns: columns
        };

        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            traditional: true,
            data: JSON.stringify(request),
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function (httpRequest, textStatus, errorThrown) {
            if (httpRequest.readyState == 0 || httpRequest.status == 0) {
                return;  
            }
            self.showErrorPopup();
        });
    };


    self.saveQuestionData = function (questionId, fieldName, fieldValue, isSharedField) {
        asyncManager.startWait();

        var request = {
            questionId: questionId,
            fieldName: fieldName,
            fieldValue: fieldValue,
            isSharedField: isSharedField === undefined? false : isSharedField
        };

        return $.ajax({
            url: window.actions.questionList.editQuestionFieldUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            if (response.isError) {
                console.error('Editing is unsuccessful');
            }
            console.log('Edited complete');
            self.resetState();
            console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    self.getQuestionNotes = function(questionId) {
        var request = {
            questionId: questionId
        };
        return $.ajax({
            url: window.actions.questionList.getQuestionNotesUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {
        }).error(function(e){
             self.showErrorPopup();
        });

    };

    self.createQuestionNote = function(note) {
          var request = {
            note: note
        };
        return $.ajax({
            url: window.actions.questionList.createQuestionNoteUrl,
            traditional: true,
            data: JSON.stringify(request),
             contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            console.log('Save note complete');
        }).error(function(e){
             self.showErrorPopup();

        });
    };

    self.deleteQuestionNote = function(note) {
         var request = {
            note: note
        };
         return $.ajax({
            url: window.actions.questionList.deleteQuestionNotesUrl,
            traditional: true,
            data: JSON.stringify(request),
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST'
        }).done(function (response) {
            console.log('Delete note complete');
        }).error(function(e){
             self.showErrorPopup();
        });
    };

     self.saveQuestionNote = function(note) {
              var request = {
                note: note
            };
            return $.ajax({
                url: window.actions.questionList.saveQuestionNoteUrl,
                traditional: true,
                data: JSON.stringify(request),
                 contentType: 'application/json',
                dataType: 'json',
                type: 'POST'
            }).done(function (response) {
                console.log('Save note complete');
            }).error(function(e){
                 self.showErrorPopup();
            });
        };

    self.getNewQuestionTemplate = function(question) {
        var request = {
            questionType: question.type,
            bank: question.bank,
            chapter: question.chapter
        };
          return $.ajax({
            url: window.actions.questionList.getNewQuestionTemplateUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
          }).done(function (response) {
              console.log('Create question template complete');
              self.resetState();
              console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
    };


    self.getQuestion = function(questionId) {
          var request = {
            questionId: questionId
        };
        return $.ajax({
            url: window.actions.questionList.getQuestionUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {
        }).error(function(e){
             self.showErrorPopup();
        });
    };


    self.getMetadataFields = function() {
         return $.ajax({
            url: window.actions.questionList.getMetadataFieldsUrl,
            traditional: true,
            dataType: 'json',
            contentType: 'application/json',
            type: 'GET'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function(e){
             self.showErrorPopup();
        });
    };


    self.updateQuestion = function(question) {
         var request = {
            questionJsonString:  JSON.stringify(question),
        };
         return $.ajax({
            url: window.actions.questionList.updateQuestionUrl,
            data: request,
            type: 'POST'
        }).done(function (response) {
            console.log('Update question complete');
            self.resetState();
            console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
    };
   
   


    self.getDuplicateQuestionTemplate = function (questionId) {
        var request = {            
            questionId: questionId
        };
        
        return $.ajax({
            url: window.actions.questionList.getDuplicateQuestionTemplateUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {

        }).error(function(e){
             self.resetState();
             self.showErrorPopup();
        });
    };

     self.removeTitle= function (questionId) {
        var questions = [];
        questions.push(questionId);
        
        var request = {            
            questionsId: questions
        };
        
        return $.ajax({
            url: window.actions.bulkOperations.removeFromTitleUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {

            self.resetState();
            self.showSuccessPopup("The shared question has been removed");

        }).error(function(e){
             self.resetState();
             self.showErrorPopup();
        });
    };

    self.getCourseMetadata= function (courseId) {
        
        var request = {            
            courseId: courseId
        };
        
        return $.ajax({
            url: window.actions.questionList.getCourseMetadataUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).error(function(e){
             self.showErrorPopup();
        });
    };

      self.flagQuestion= function (questionId, isFlagged) {

         var request = {
            questionId: questionId,
            fieldName: "flag",
            fieldValue: isFlagged? window.enums.flag.flagged : window.enums.flag.notFlagged,
            isSharedField: false
        };

        return $.ajax({
            url: window.actions.questionList.editQuestionFieldUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            if (response.isError) {
                console.error('Editing is unsuccessful');
            }
            console.log('Edited complete');
            self.showSuccessPopup("Question successfully "+ (isFlagged? "flagged": "unflagged"));
            console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
       
    };

    self.getQuestionVersions = function(questionId){
          var request = {
            questionId: questionId
        };
        return $.ajax({
            url: window.actions.questionList.getQuestionVersionsUrl,
            traditional: true,
            data: request,
            contentType: 'application/json',
            dataType: 'json',
            type: 'GET'
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    

    /* Bulk operations */
    self.bulk = {};

    self.bulk.updateMetadataField = function (questionIds, fieldName, fieldValue, isSharedField) {
        asyncManager.startWait();

        var request = {
            questionIds: questionIds,
            fieldName: fieldName,
            fieldValue: fieldValue,
            isSharedField: isSharedField === undefined? false : isSharedField
        };

        return $.ajax({
            url: window.actions.bulkOperations.bulkUpdateMetadataFieldUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            if (response.isError) {
                console.error('Bulk Editing is unsuccessful');
            }
            console.log('Bulk Edit complete');
            self.showSuccessPopup("Questions updated successfully");
            self.resetState();
            console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    self.bulk.removeTitle = function (questionsId) {
        asyncManager.startWait();

        var request = {
            questionsId: questionsId,
        };

        return $.ajax({
            url: window.actions.bulkOperations.removeFromTitleUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.resetState();
            var message = questionsId.length == 1? "Shared question has been removed" : "Shared questions have been removed" ;
            self.showSuccessPopup(message);
        }).error(function (e) {
            self.resetState();
            self.showErrorPopup();
        });
    };

    self.bulk.shareTitle = function (questionsId, courseViewModel) {
        asyncManager.startWait();

        var request = {
            questionsId: questionsId,
            courseId: courseViewModel[window.consts.questionCourseName][0],
            bank: courseViewModel[window.consts.questionBankName][0],
            chapter: courseViewModel[window.consts.questionChapterName][0]
        };

        return $.ajax({
            url: window.actions.bulkOperations.publishToTitle,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
        }).error(function (e) {
            self.resetState();
            self.showErrorPopup();
        });
    };

    /*  Common operations  */
    self.resetState = function(){
         crossroads.resetState();
         crossroads.parse(window.routsManager.buildHash());
    };

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

    return self;
}());