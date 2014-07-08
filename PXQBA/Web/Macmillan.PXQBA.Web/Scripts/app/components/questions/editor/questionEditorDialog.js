/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({displayName: 'QuestionEditorDialog',

     getInitialState: function() {

      return { notesChanged: false};
    },

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
         monitorChanges(".local", false);
         var self = this;
       
        $(this.getDOMNode()).on('hidden.bs.modal', function () {
            self.props.closeDialogHandler();
               if(self.state.notesChanged){
                      questionDataManager.resetState();
                 }

        })

    },
    finishSaving: function(e){

        questionDataManager.resetState();
     //   $(this.getDOMNode()).modal("hide");

        var text = this.props.caption == window.enums.dialogCaptions.editQuestion?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;

        notificationManager.showSuccess(text);
    },

    closeDialog: function(){
         monitorChanges(".local", true);
         $(this.getDOMNode()).modal("hide");
         $('.modal-backdrop').remove(); 
         var questionType = this.props.question.questionType == null? "" :this.props.question.questionType.toLowerCase();
         if ( questionType != "hts" && questionType !="fma_graph"){
            questionDataManager.clearResources();
         }
         
    },

    notesChangedHandler: function(){
       this.setState({notesChanged: true});
     },


    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (QuestionEditor( {question:self.props.question,
                                     metadata:self.props.metadata,  
                                     editSourceQuestionHandler:self.props.editSourceQuestionHandler, 
                                     finishSaving:  self.finishSaving, 
                                     closeDialog:self.closeDialog, 
                                     isNew:self.props.isNew, 
                                     isDuplicate:self.props.caption === window.enums.dialogCaptions.duplicateQuestion,
                                     handlers:self.props.handlers,
                                     viewHistoryMode:  self.props.viewHistoryMode,
                                     isEditedInPlace:  self.props.isEditedInPlace,
                                     caption:self.props.caption,
                                     notesChangedHandler:  self.notesChangedHandler}
                                     ));
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             renderFooterButtons:renderFooterButtons, 
                             closeDialogHandler:  this.closeDialog,
                             dialogId:"questionEditorModal"})
                );
    }
});