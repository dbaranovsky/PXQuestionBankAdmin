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
            crossroads.resetState();
            crossroads.parse(window.routsManager.buildHash());
            console.log('Refresh complite');
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
            crossroads.resetState();
            crossroads.parse(window.routsManager.buildHash());
            console.log('Refresh complite');
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
            crossroads.resetState();
            crossroads.parse(window.routsManager.buildHash());
            console.log('Refresh complite');
        });
    };


    self.getNewQuestionTemplate = function() {
       // var request = {
       //     questionType: questionType
       // };
          return $.ajax({
            url: window.actions.questionList.getNewQuestionTemplateUrl,
            traditional: true,
         //   data: request,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {
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
        });
    };

     self.createQuestion = function(courseId, question) {
         var request = {
            courseId: courseId,
            question: question
        };
         return $.ajax({
            url: window.actions.questionList.createQuestionUrl,
            traditional: true,
            data: JSON.stringify(request),
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST'
        }).done(function (response) {
            console.log('Create question complete');
            crossroads.resetState();
            crossroads.parse(window.routsManager.buildHash());
            console.log('Refresh complite');
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
 
        });
 
 
    };

    return self;
}());