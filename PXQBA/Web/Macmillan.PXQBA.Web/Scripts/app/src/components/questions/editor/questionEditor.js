/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({

   getInitialState: function() {

      return { question: this.props.question };
    },


    saveQuestion: function(){
       // if(this.props.isNew)
       // {
       // }
     
        var finishSaving = this.props.finishSaving;
        questionDataManager.updateQuestion(this.state.question).done(finishSaving);

    },

    saveBHEditor: function(frameApi){
        if (frameApi !== undefined){
          frameApi.saveComponent('questioneditor', 'editoriframecontainer');
       }
          this.saveQuestion();
       
    },

     runQuestion: function(){
      window.open(this.props.question.actionPlayerUrl, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },


     showSaveWarning: function(frameApi){
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
                                      editSourceQuestionHandler={this.props.editSourceQuestionHandler.bind(this,this.state.question.questionIdDuplicateFrom)} 
                                      showSaveWarning={this.showSaveWarning}  
                                      saveQuestion = {this.saveQuestion}
                                      metadata={this.props.metadata} 
                                      editHandler={this.editHandler} 
                                      isDuplicate={this.props.isDuplicate}/>
                </div>
                {this.renderWarningDialog()}

         </div>);
    }
});