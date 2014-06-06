/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({displayName: 'QuestionEditor',

   getInitialState: function() {

      return { question: this.props.question, viewHistoryMode: this.props.viewHistoryMode};
    },

    componentDidMount: function(){
      if(this.props.isEditedInPlace && this.state.question.status == window.enums.statusesId.inProgress){

          this.showNotificationForInProgress();
          return;
      }

      if(this.state.question.draftFrom != ""){
           this.showDraftNotification();
      }


    },

    showDraftNotification: function(){
         this.setState({showNotification: true, typeId: window.enums.notificationTypes.newDraftForAvailableToInstructors});
    },

    showNotificationForInProgress: function(){
        this.setState({showNotification: true, typeId: window.enums.notificationTypes.editInPlaceQuestionInProgress});
    },

   showSaveAndPublish: function(){
        this.setState({showNotification: true, typeId: window.enums.notificationTypes.saveAndPublishDraft});
    },
    saveQuestion: function(){
      if(this.state.saveAndPublishMode) {
          this.showSaveAndPublish();
      }
      else {
           questionDataManager.updateQuestion(this.state.question).done(this.updateQuestionHandler);
      }
    },

    saveAndPublish: function(){
       questionDataManager.saveAndPublishDraftQuestion(this.state.question).done(this.updateQuestionHandler);
    },

    updateQuestionHandler: function(response) {
      if(!response.isError) {
          this.props.finishSaving();
      } 
      else {
        window.questionDataManager.showWarningPopup(window.enums.messages.warningQuestionEditorMessage);
      }
    },

    saveBHEditor: function(frameApi){
      var self = this;
        if (frameApi !== undefined && frameApi.saveComponent !== undefined ){
          frameApi.saveComponent('questioneditor', 'quizeditorcomponent', function(result){
            if(!result) {
                window.questionDataManager.showWarningPopup(window.enums.messages.warningQuestionEditorMessage);
            }
        });
       } else{
         this.saveQuestion();
       }
       
    },

     runQuestion: function(){
      window.open(this.props.question.actionPlayerUrl, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },


     showSaveWarning: function(frameApi, saveAndPublish){
        this.setState({saveAndPublishMode: saveAndPublish});
        if(!this.props.isNew && !this.props.isDuplicate){
          this.setState({showSaveWarning: true, frameApi: frameApi});
        } else{
          this.saveBHEditor(frameApi);
        }
        
     
     },

     closeSaveWarningDialog: function(){
         $('.modal-backdrop').first().remove(); 
         this.setState({showSaveWarning: false});
     },

     makeChangesVisibleToInstructor: function(){
        this.closeSaveWarningDialog();
        this.saveBHEditor(this.state.frameApi);
     },

     renderWarningDialog: function(){
      if (this.state.showSaveWarning){
        var self = this;
        var renderHeaderText = function() {
            return ("Attention");
        };
        
        var renderBody = function(){
            return (React.DOM.div(null, 
                      "The changes made will affect the version of question  that is visible  to instructor",
                      React.DOM.br(null ),React.DOM.br(null ),
                      React.DOM.button( {className:"btn btn-primary", 'data-toggle':"modal", onClick:self.makeChangesVisibleToInstructor}, 
                                   "Make changes visible to instructors"
                      ),
                      React.DOM.br(null ),React.DOM.br(null ),
                      React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:self.makeChangesVisibleToInstructor} , 
                                   "Leave visible the previous version"
                      ),
                      React.DOM.br(null ),React.DOM.br(null ),
                      React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", onClick:self.closeSaveWarningDialog}, 
                             "Cancel"
                        )
                    )
              );
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             dialogId:"saveWarningDialog",
                             closeDialogHandler:  this.closeSaveWarningDialog,
                             showOnCreate:  true,
                             preventDefaultClose: true})
                );
      }

      return null;
     },

     closeDialog: function(){
      if(hasUnsavedData()){
       if (confirm("You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?")){
          this.props.closeDialog();
       }
     } else{

       this.props.closeDialog(); 
     }
     },

     renderNotification: function(){
        if (this.state.showNotification){
           var notification =  userManager.getNotificationById(this.state.typeId);
           if (notification.isShown){
             return ( NotificationDialog(  {closeDialog:this.closeNotificationDialog, proceedHandler:  this.proceedHandler, notification:notification, isCustomCloseHandle:true}));
           }
        }
        return null;
     },     

     closeNotificationDialog: function(){
        this.closeDialog();
     },

     proceedHandler: function(){
         $('.modal-backdrop').first().remove(); 
         this.setState({showNotification: false});
     },

    render: function() {
        return (
            React.DOM.div(null, 
                      React.DOM.div( {className:"header-buttons"}, 
                         React.DOM.button( {className:"btn btn-primary run-question", 'data-toggle':"modal", onClick:this.runQuestion}, 
                             React.DOM.span( {className:"glyphicon glyphicon-play"}), " Try Question"
                        )
                      ),
                
                React.DOM.div( {className:"editor-tabs"}, 
                  QuestionEditorTabs( {question:this.state.question, 
                                      closeDialog:this.closeDialog,  
                                      editSourceQuestionHandler:this.props.editSourceQuestionHandler, 
                                      showSaveWarning:this.showSaveWarning,  
                                      saveQuestion:  this.saveQuestion,
                                      metadata:this.props.metadata, 
                                      editHandler:this.editHandler, 
                                      isDuplicate:this.props.isDuplicate,
                                      handlers:this.props.handlers,
                                      viewHistoryMode: this.props.viewHistoryMode})
                ),
                this.renderWarningDialog(),
                this.renderNotification()


         ));
    }
});