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

     runQuestion: function(){
      window.open(this.props.question.actionPlayerUrl, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },


     showSaveWarning: function(){
        if(!this.props.isNew && !this.props.isDuplicate){
          this.setState({showSaveWarning: true});
        } else{
          this.saveQuestion();
        }
        
     
     },

     closeSaveWarningDialog: function(){
         $('.modal-backdrop').first().remove(); 
         this.setState({showSaveWarning: false});
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
                      <button className="btn btn-primary" data-toggle="modal" onClick={self.saveQuestion}>
                                   Make changes visible to instructors
                      </button>
                      <br /><br />
                      <button className="btn btn-primary " data-toggle="modal" onClick={self.saveQuestion} >
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


      loadSourceQuestion: function(event){
         event.preventDefault();
         this.props.editSourceQuestionHandler(this.state.question.questionIdDuplicateFrom);
     },

      renderSharingNotification: function(){
         if (this.props.question.isDuplicateOfSharedQuestion && this.props.isDuplicate) {
        return (<div className="shared-note">This question is a duplicate of a&nbsp;
                    <a className="shared-question-link" href="" onClick={this.loadSourceQuestion}>shared question</a>
                    from <b>{this.props.question.productCourses.join(', ')}</b> 
               </div>);
      }

      if (this.props.question.isShared && !this.props.isDuplicate && !this.props.isNew){
                var sharedCourses = this.props.question.productCourses.length;
                return (<div className="shared-note">Editing this question content would affect {sharedCourses == 1 ?  "1 title" :"all "+sharedCourses+ " titles"} that use this question </div>);
      }

      return null;
    },



    render: function() {
        return (
            <div>
                   {this.renderSharingNotification()}
                      <div className="header-buttons">
                         <button className="btn btn-primary run-question" data-toggle="modal" onClick={this.runQuestion}>
                             <span className="glyphicon glyphicon-play"></span> Try Question
                        </button>
                        <button className="btn btn-default" data-toggle="modal" onClick={this.closeDialog}>
                             Cancel
                        </button>
                         <button className="btn btn-primary " data-toggle="modal" onClick={this.showSaveWarning} >
                             Save
                        </button>
                      </div>
                
                <div>
                  <QuestionEditorTabs question={this.state.question} metadata={this.props.metadata} editHandler={this.editHandler} isDuplicate={this.props.isDuplicate}/>
                </div>
                {this.renderWarningDialog()}

         </div>);
    }
});