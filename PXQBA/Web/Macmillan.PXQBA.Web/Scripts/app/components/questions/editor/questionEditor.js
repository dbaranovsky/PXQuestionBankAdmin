/**
* @jsx React.DOM
*/
var QuestionEditor = React.createClass({displayName: 'QuestionEditor',

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
            return (React.DOM.div(null, 
                      "The changes made will affect the version of question  that is visible  to instructor",
                      React.DOM.br(null ),React.DOM.br(null ),
                      React.DOM.button( {className:"btn btn-primary", 'data-toggle':"modal", onClick:self.saveQuestion}, 
                                   "Make changes visible to instructors"
                      ),
                      React.DOM.br(null ),React.DOM.br(null ),
                      React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:self.saveQuestion} , 
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


     

    render: function() {
        return (
            React.DOM.div(null, 
                      React.DOM.div( {className:"header-buttons"}, 
                         React.DOM.button( {className:"btn btn-primary run-question", 'data-toggle':"modal", onClick:this.runQuestion}, 
                             React.DOM.span( {className:"glyphicon glyphicon-play"}), " Try Question"
                        )
                      ),
                
                React.DOM.div(null, 
                  QuestionEditorTabs( {question:this.state.question, closeDialog:this.closeDialog,  editSourceQuestionHandler:this.props.editSourceQuestionHandler, showSaveWarning:this.showSaveWarning,  metadata:this.props.metadata, editHandler:this.editHandler, isDuplicate:this.props.isDuplicate})
                ),
                this.renderWarningDialog()

         ));
    }
});