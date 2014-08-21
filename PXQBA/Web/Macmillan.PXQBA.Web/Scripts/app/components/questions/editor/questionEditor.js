/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({displayName: 'QuestionEditor',

   getInitialState: function() {

      return { question: this.props.question, viewHistoryMode: this.props.viewHistoryMode, saving: false, 
               savingCount: 0,
               isHTS: this.props.question.questionType!= null && this.props.question.questionType.toLowerCase()=="hts"? true: false,
               isCustom: this.props.question.questionType!= null,
               isGraph: this.props.question.questionType == "FMA_GRAPH"};
    },

    componentDidMount: function(){
      if(this.props.isEditedInPlace && this.state.question.status == window.enums.statusesId.inProgress){

          this.showNotificationForInProgress();
          return;
      }

      if(this.state.question.draftFrom != "" && !this.state.viewHistoryMode){
           this.showDraftNotification();
      }


    },

    showDraftNotification: function(){
         this.setState({showNotification: true, typeId: window.enums.notificationTypes.newDraftForAvailableToInstructors});
    },

    showNotificationForInProgress: function(switchTab){

        this.setState({showNotification: true, typeId: window.enums.notificationTypes.editInPlaceQuestionInProgress, switchTab: switchTab== undefined? null : switchTab});
    },

   showSaveAndPublish: function(){
   //     this.setState({showNotification: true, typeId: window.enums.notificationTypes.saveAndPublishDraft});
    },

    saveQuestion: function(){
      if(!this.validateQuestion()) {
        this.setState({saving: false});
        return;
      }

     this.resetChangesMonitor();


      if(this.state.saveAndPublishMode) {
          this.showSaveAndPublish();
      }
      else {
          var message = this.props.caption == window.enums.dialogCaptions.editQuestion || this.state.savingCount > 0?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;
           this.setState({saving: true});
           questionDataManager.updateQuestion(this.props.currentCourseId, this.state.question, message).done(this.updateQuestionHandler);
      }
    },

    validateQuestion: function() {
      var isValid = true;

      if((this.state.question.localSection.title=="")||(this.state.question.localSection.title==null)) {
         isValid = false;
          window.questionDataManager.showWarningPopup("Title metadata field is required.");
      }

      return isValid;
    },

    resetChangesMonitor: function(){
       monitorChanges(".local", true);
       monitorChanges(".local", false);
    },

    saveAndPublish: function(){
       questionDataManager.saveAndPublishDraftQuestion(this.props.currentCourseId, this.state.question).done(this.clearResources);
       this.closeDialog(false);
    },
    updateQuestionHandler: function(response) {
      this.setState({saving: false, savingCount: this.state.savingCount+1});
      if(!response.isError) {
         // this.props.finishSaving();
         this.reloadHistory();
      } 
      else {
        window.questionDataManager.showWarningPopup(window.enums.messages.warningQuestionEditorMessage);
      }
    },

    saveBHEditor: function(frameApi, saveAndPublish){
      var self = this;
        if (frameApi !== undefined && frameApi.saveComponent !== undefined && (!this.props.question.isShared || this.props.question.canEditSharedQuestionContent)){
             frameApi.saveComponent('questioneditor', 'quizeditorcomponent', function(result){
                      self.setState({saving: false});
                      if(!result) {
                         window.questionDataManager.showWarningPopup(window.enums.messages.warningQuestionEditorMessage);
                      }
              });
       } else{
          if(!saveAndPublish || saveAndPublish === undefined){
            this.saveQuestion();
          } 
            return;
       }
       
    },

  clearResources: function(response){
      this.props.clearResources();
  },
    reloadHistory: function(){
      this.refs.questionEditorTabs.refs.versionHistory.updateQuestionVersion();
    },

     runQuestion: function(){
      window.open(this.props.question.actionPlayerUrl, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },


     showSaveWarning: function(frameApi, saveAndPublish){
        this.setState({saveAndPublishMode: saveAndPublish, saving: true, showNotification: saveAndPublish, typeId: window.enums.notificationTypes.saveAndPublishDraft});
        this.saveBHEditor(frameApi, saveAndPublish);
    
     },

     closeSaveWarningDialog: function(){
         $('.modal-backdrop').first().remove(); 
         this.setState({showSaveWarning: false});
     },
  

     closeDialog: function(isClearResources){
   
      if(hasUnsavedData()){
       if (confirm("You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?")){
          this.props.closeDialog(isClearResources);
       }
     } else{
       this.props.closeDialog(isClearResources); 
     }
     },

     renderNotification: function(){
        if (this.state.showNotification){
           var notification =  userManager.getNotificationById(this.state.typeId);
           if (notification != null && (notification.isShown || ((this.state.isHTS || this.state.isGraph) && this.state.typeId==window.enums.notificationTypes.saveAndPublishDraft))){
             return ( NotificationDialog({closeDialog: this.closeNotificationDialog, proceedHandler: this.proceedHandler, notification: notification, isCustomCloseHandle: true}));
           }
           else {
            if(this.state.typeId==window.enums.notificationTypes.saveAndPublishDraft) {
              this.proceedHandler();
            }
           }
        }
        return null;
     },     

     closeNotificationDialog: function(){

         if(this.state.saveAndPublishMode) {
             $('.modal-backdrop').first().remove(); 
             this.setState({showNotification: false, saving: false});
         } else{
             this.closeDialog(true);
         }
        
     },

     proceedHandler: function(){
      if (this.state.saveAndPublishMode){
          this.resetChangesMonitor();
          this.saveAndPublish();
      }else{
        $('.modal-backdrop').first().remove(); 
          if (this.state.switchTab != undefined && this.state.switchTab != null){
             this.state.switchTab();
          }
         this.setState({showNotification: false, switchTab: null});

      }
         
     },

     renderEditInPlaceDialog: function(){
       if (this.state.showEditInPlaceDialog){
        var self = this;
        var renderHeaderText = function() {
            return ("Warning");
        };
        
        var renderBody = function(){
            return (React.DOM.div(null, 
                      "Do you want to create a draft question or edit in place?", 
                      React.DOM.br(null), React.DOM.br(null), 
                      React.DOM.button({className: "btn btn-primary", 'data-toggle': "modal", onClick: self.state.editInPlaceHandler}, 
                                   "Edit in place"
                      ), 
                        "   ", 
                      React.DOM.button({className: "btn btn-primary ", 'data-toggle': "modal", onClick: self.createDraft}, 
                                 "Create a Draft"
                      ), 
                         "   ", 
                      React.DOM.button({className: "btn btn-default", 'data-toggle': "modal", onClick: self.closeEditInPlaceDialog}, 
                             "Cancel"
                        )
                    )
              );
        };
        return (ModalDialog({renderHeaderText: renderHeaderText, 
                             renderBody: renderBody, 
                             dialogId: "editInPlace", 
                             closeDialogHandler: this.closeSaveWarningDialog, 
                             showOnCreate: true, 
                             preventDefaultClose: true})
                );
      }

      return null;
     },

     createDraft: function(){
       $('.modal-backdrop').first().remove(); 
        this.props.handlers.createDraftHandler(this.props.question.realQuestionId, null);
     },

     showEditInPlaceDialog: function(handler){
      this.setState({showEditInPlaceDialog: true, editInPlaceHandler: handler});
     },

     closeEditInPlaceDialog: function(){
      $('.modal-backdrop').first().remove(); 
      this.setState({showEditInPlaceDialog: false});
     },

    

     render: function() {
        return (
            React.DOM.div(null, 
                      React.DOM.div({className: "header-buttons"}, 
                         React.DOM.button({className: "btn btn-primary run-question", 'data-toggle': "modal", disabled: this.state.saving || !this.state.question.canTestQuestion, onClick: this.runQuestion}, 
                             React.DOM.span({className: "glyphicon glyphicon-play"}), " Try Question"
                        )
                      ), 
                
                React.DOM.div({className: "editor-tabs"}, 
                  QuestionEditorTabs({ref: "questionEditorTabs", 
                                      currentCourseId: this.props.currentCourseId, 
                                      question: this.state.question, 
                                      closeDialog: this.closeDialog, 
                                      editSourceQuestionHandler: this.props.editSourceQuestionHandler, 
                                      showSaveWarning: this.showSaveWarning, 
                                      showEditInPlaceDialog: this.showEditInPlaceDialog, 
                                      closeEditInPlaceDialog: this.closeEditInPlaceDialog, 
                                      showNotificationForInProgress: this.showNotificationForInProgress, 
                                      saveQuestion: this.saveQuestion, 
                                      metadata: this.props.metadata, 
                                      editHandler: this.editHandler, 
                                      isDuplicate: this.props.isDuplicate, 
                                      handlers: this.props.handlers, 
                                      viewHistoryMode: this.props.viewHistoryMode, 
                                      saving: this.state.saving, 
                                      notesChangedHandler: this.props.notesChangedHandler})
                ), 
                this.renderNotification(), 
                this.renderEditInPlaceDialog()


         ));
    }
});