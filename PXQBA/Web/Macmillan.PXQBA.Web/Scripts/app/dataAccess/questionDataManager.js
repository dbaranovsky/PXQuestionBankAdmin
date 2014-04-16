var questionDataManager = (function() {
    var self = {};

    self.cache = {};

    self.processDataResponse = function (response) {
        //save in cache here
    };

    self.loadFromCache = function (query, page, orderType, orderField) {
        // load from cache here
        return null;
    };

    self.getQuestionsByQuery = function (query, page, columns, orderType, orderField) {
        
        var cacheResult = self.loadFromCache(query, page, orderType, orderField);

        if (cacheResult != null) {
            return $.Deferred(function() {
                this.resolve(cacheResult);
            });
        }

        var request = {
            query: query,
            pageNumber: page,
            orderType: orderType, 
            orderField: orderField,
            columns: columns
        };

        return $.ajax({
            url: window.actions.questionList.getQuestionListUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.processDataResponse(response);
        }).error(function(e){
             self.showErrorPopup();
        });
    };


    self.saveQuestionData = function (questionId, fieldName, fieldValue) {
        asyncManager.startWait();
        var request = {
            questionId: questionId,
            fieldName: fieldName,
            fieldValue: fieldValue
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
            self.resetState();
            console.log('Refresh complite');
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
            question: question
        };
         return $.ajax({
            url: window.actions.questionList.updateQuestionUrl,
            traditional: true,
            data: JSON.stringify(request),
            dataType: 'json',
            contentType: 'application/json',
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

    self.bulk = {};

    self.bulk.setStatus = function (questionsId, newQuestionStatus) {
        asyncManager.startWait();
        
        var request = {
            questionsId: questionsId,
            newQuestionStatus: newQuestionStatus
        };

        return $.ajax({
            url: window.actions.bulkOperations.setStatusUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.resetState();
        }).error(function (e) {
            self.resetState();
            self.showErrorPopup();
        });
    };

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

    return self;
}());