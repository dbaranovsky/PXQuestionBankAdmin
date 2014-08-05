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


    self.saveQuestionData = function (courseId, questionId, fieldName, fieldValue, isSharedField) {
        asyncManager.startWait();

        var request = {
            courseId: courseId,
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
            cache: false,
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
            cache: false,
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
            cache: false,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {
        }).error(function(e){
             self.showErrorPopup();
              asyncManager.stopWait();
        });
    };


    self.getMetadataFields = function() {
         return $.ajax({
            url: window.actions.questionList.getMetadataFieldsUrl,
            cache: false,
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


    self.updateQuestion = function (question, message) {
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
             self.showSuccessPopup(message);
            console.log('Refresh complite');
        }).error(function(e){
             self.showErrorPopup();
        });
    };
   
    self.saveAndPublishDraftQuestion = function (question) {
        
        asyncManager.startWait();
        var request = {
            questionJsonString: JSON.stringify(question),
        };
        return $.ajax({
            url: window.actions.questionList.saveAndPublishDraftUrl,
            data: request,
            type: 'POST'
        }).done(function (response) {
            console.log('Save and publish question complete');
            asyncManager.stopWait();
            self.showSuccessPopup("Question was successfully saved and published");
            self.resetState();

            console.log('Refresh complite');
        }).error(function (e) {
            self.showErrorPopup();
            asyncManager.stopWait();
        });
    };


    self.getDuplicateQuestionTemplate = function (questionId, version) {
        var request = {            
            questionId: questionId,
            version: version
        };
        
        return $.ajax({
            url: window.actions.questionList.getDuplicateQuestionTemplateUrl,
             cache: false,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            self.resetState();
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


      self.flagQuestion= function (courseId, questionId, isFlagged) {

          var request = {
            courseId: courseId,
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

    self.getQuestionVersions = function(){

        return $.ajax({
            url: window.actions.questionList.getQuestionVersionsUrl,
            traditional: true,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',
            type: 'GET'
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    self.updateSharedMetadataField = function (questionId, fieldName, fieldValues) {

        var request = {
            questionId: questionId,
            fieldName: fieldName,
            fieldValues: fieldValues
        };

        return $.ajax({
            url: window.actions.questionList.updateSharedMetadataFieldUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {      
            console.log('updateSharedMetadataField complete');
            self.showSuccessPopup("Shared field updated successfully");
        }).error(function(e){
             self.showErrorPopup();
        });
    };


     self.deleteQuestion = function (questionId, fieldName, fieldValues) {

        return $.ajax({
            url: window.actions.questionList.deleteQuestionUrl,
             cache: false,
            traditional: true,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {      
            console.log('updateSharedMetadataField complete');
            if(response.resetState){
                self.resetState();
            }
            
        }).error(function(e){
             self.showErrorPopup();
        });
    };

   self.getUpdatedGraphEditor = function (interactionData) {

       var request = {
            interactionData: interactionData
        };

        return $.ajax({
            url: window.actions.questionList.getUpdatedGraphEditorUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {      
             console.log('getUpdatedGraphEditor complete');
        }).error(function(e){
             console.log('getUpdatedGraphEditor error');
        });
    };



   self.restoreVersion = function (version) {

       var request = {
            version: version.version
        };

        return $.ajax({
            url: window.actions.questionList.restoreVersionUrl,
            traditional: true,
            data: JSON.stringify(request),
            contentType: 'application/json',
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {      
             console.log('restoreVersion complete');
             self.showSuccessPopup('The version of '+ version.modifiedDate + ' by '+version.modifiedBy+' was restored by creating a duplicate version');
        }).error(function(e){
             console.log('restoreVersion error');
             self.showErrorPopup();
        });
    };
    
    

    /* Bulk operations */
    self.bulk = {};

    self.bulk.updateMetadataField = function (courseId, questionIds, fieldName, fieldValue, isSharedField) {
        asyncManager.startWait();
        //Update status
        var request = {
            courseId: courseId,
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
            if (!response.isSuccess) {
                console.error('Bulk Editing is unsuccessful');
            }
            console.log('Bulk Edit complete');
            self.resetState();
            self.showSuccessPopup("Questions updated successfully");
            
            if (response.draftSkipped > 0) {
                self.showWarningPopupHtml(response.draftSkipped + " drafts were skipped while setting status to 'Available to instructors'." +
                                                                   "<a class='notification-link'" +
                                                                    "ref='javascript:void(0);'" +
                                                                    " onclick='window.alerts.draftAvailableForInstructorsInfo()'>" +
                                                                    "See More Info.</a>");
            }
            
            if (response.permissionStatusSkipped > 0) {
                self.showWarningPopup("You have no permission for status changing - " + response.permissionStatusSkipped + " questions/drafts were skipped");
            }
            
            if (response.permissionSkipped > 0) {
                self.showWarningPopup("You have no permission for fields changing - " + response.permissionSkipped + " questions/drafts were skipped");
            }
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
            courseId: courseViewModel[window.consts.targetProductCourse][0],
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

    /* Versions and draft */
    self.getVersionPreviewLink = function(version){
       var request = {
            version: version,
        };
         return $.ajax({
            url: window.actions.questionList.getVersionPreviewLinkUrl,
             cache: false,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    self.publishDraftToOriginal = function (questionId) {
        asyncManager.startWait();
        var request = {
            draftQuestionId: questionId,
        };
        return $.ajax({
            url: window.actions.questionList.publishDraftToOriginalUrl,
             cache: false,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'GET'
        }).done(function (response) {
            asyncManager.stopWait();
            self.resetState();
            self.showSuccessPopup('The question was published successfully');
        }).error(function (e) {
            self.showErrorPopup();
            asyncManager.stopWait();
        });

    };
    
    self.createDraft = function (questionId, version, status) {
        asyncManager.startWait();
        var request = {
            questionId: questionId,
            version: version == undefined? null : version,
            status:  status == undefined? null : status
        };
        return $.ajax({
            url: window.actions.questionList.createDraftUrl,
            traditional: true,
            data: request,
            dataType: 'json',
            type: 'POST'
        }).done(function (response) {
            asyncManager.stopWait();
            self.resetState();
        }).error(function (e) {
            self.showErrorPopup();
            asyncManager.stopWait();
        });

    };

    self.clearResources  = function(){
        return $.ajax({
            url: window.actions.questionList.clearQuestionResourcesUrl,
             cache: false,
            traditional: true,
            contentType: 'application/json',
            dataType: 'json',
            type: 'GET'
        }).error(function(e){
             self.showErrorPopup();
        });
    };

    /*  Common operations  */
    self.resetState = function(){
         crossroads.resetState();
         crossroads.parse(window.routsManager.buildHash());
    };

    self.showErrorPopup = function() {
        notificationManager.showDanger(window.enums.messages.errorMessage);
    };

    self.showSuccessPopup = function(message){
        notificationManager.showSuccess(message);
    };

    self.showWarningPopup = function (message) {
        notificationManager.showWarning(message);
    };
    
    self.showWarningPopupHtml = function (html) {
        notificationManager.showWarningHtml(html);
    };

    return self;
}());