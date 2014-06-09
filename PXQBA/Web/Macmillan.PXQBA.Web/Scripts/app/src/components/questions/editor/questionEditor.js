/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({

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

    showNotificationForInProgress: function(switchTab){

        this.setState({showNotification: true, typeId: window.enums.notificationTypes.editInPlaceQuestionInProgress, switchTab: switchTab== undefined? null : switchTab});
    },

   showSaveAndPublish: function(){
        this.setState({showNotification: true, typeId: window.enums.notificationTypes.saveAndPublishDraft});
    },

    saveQuestion: function(){
      if(!this.validateQuestion()) {
        return;
      }

      if(this.state.saveAndPublishMode) {
          this.showSaveAndPublish();
      }
      else {
           questionDataManager.updateQuestion(this.state.question).done(this.updateQuestionHandler);
      }
    },

    validateQuestion: function() {
      var isValid = true;

      if((this.state.question.localSection.title=="")||(this.state.question.localSection.title==null)) {
         isValid = false;
         window.questionDataManager.showWarningPopup("Title metadata field is requaried.")
      }

      return isValid;
    },

    saveAndPublish: function(){
       questionDataManager.saveAndPublishDraftQuestion(this.state.question).done(this.updateQuestionHandler);
       this.closeDialog();
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
            return (<div>
                      The changes made will affect the version of question  that is visible  to instructor
                      <br /><br />
                      <button className="btn btn-primary" data-toggle="modal" onClick={self.makeChangesVisibleToInstructor}>
                                   Make changes visible to instructors
                      </button>
                      <br /><br />
                      <button className="btn btn-primary " data-toggle="modal" onClick={self.makeChangesVisibleToInstructor} >
                                   Leave visible the previous version
                      </button>
                      <br /><br />
                      <button className="btn btn-default" data-toggle="modal" onClick={self.closeSaveWarningDialog}>
                             Cancel
                        </button>
                    </div>
              );
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             dialogId="saveWarningDialog"
                             closeDialogHandler = {this.closeSaveWarningDialog}
                             showOnCreate = {true}
                             preventDefaultClose ={true}/>
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

   //   questionDataManager.deleteQuestion();
     },

     renderNotification: function(){
        if (this.state.showNotification){
           var notification =  userManager.getNotificationById(this.state.typeId);
           if (notification.isShown){
             return ( <NotificationDialog  closeDialog={this.closeNotificationDialog} proceedHandler = {this.proceedHandler} notification={notification} isCustomCloseHandle={true}/>);
           }
        }
        return null;
     },     

     closeNotificationDialog: function(){

         if(this.state.saveAndPublishMode) {
             $('.modal-backdrop').first().remove(); 
             this.setState({showNotification: false});
         } else{
             this.closeDialog();
         }
        
     },

     proceedHandler: function(){
      if (this.state.saveAndPublishMode){
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
            return (<div>
                      Do you want to create a draft question or edit in place?
                      <br /><br />
                      <button className="btn btn-primary" data-toggle="modal" onClick={self.state.editInPlaceHandler}>
                                   Edit in place
                      </button>
                        &nbsp; &nbsp;
                      <button className="btn btn-primary " data-toggle="modal" onClick={self.createDraft} >
                                 Create a Draft
                      </button>
                         &nbsp; &nbsp;
                      <button className="btn btn-default" data-toggle="modal" onClick={self.closeEditInPlaceDialog}>
                             Cancel
                        </button>
                    </div>
              );
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             dialogId="editInPlace"
                             closeDialogHandler = {this.closeSaveWarningDialog}
                             showOnCreate = {true}
                             preventDefaultClose ={true}/>
                );
      }

      return null;
     },

     createDraft: function(){
       $('.modal-backdrop').first().remove(); 
        this.props.handlers.createDraftHandler(null, null);
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
            <div>
                      <div className="header-buttons">
                         <button className="btn btn-primary run-question" data-toggle="modal" onClick={this.runQuestion}>
                             <span className="glyphicon glyphicon-play"></span> Try Question
                        </button>
                      </div>
                
                <div className="editor-tabs">
                  <QuestionEditorTabs question={this.state.question} 
                                      closeDialog={this.closeDialog}  
                                      editSourceQuestionHandler={this.props.editSourceQuestionHandler} 
                                      showSaveWarning={this.showSaveWarning}  
                                      showEditInPlaceDialog = {this.showEditInPlaceDialog}
                                      closeEditInPlaceDialog = {this.closeEditInPlaceDialog}
                                      showNotificationForInProgress = {this.showNotificationForInProgress}
                                      saveQuestion = {this.saveQuestion}
                                      metadata={this.props.metadata} 
                                      editHandler={this.editHandler} 
                                      isDuplicate={this.props.isDuplicate}
                                      handlers={this.props.handlers}
                                      viewHistoryMode= {this.props.viewHistoryMode}/>
                </div>
                {this.renderWarningDialog()}
                {this.renderNotification()}
                {this.renderEditInPlaceDialog()}


         </div>);
    }
});