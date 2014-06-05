/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({

    
    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
         monitorChanges(".local", false);
        var closeDialogHandler = this.props.closeDialogHandler;
        $(this.getDOMNode()).on('hidden.bs.modal', function () {
            closeDialogHandler();
        })

    },
    finishSaving: function(e){

        questionDataManager.resetState();
        $(this.getDOMNode()).modal("hide");

        var text = this.props.caption == window.enums.dialogCaptions.editQuestion?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;

        var notifyOptions = {message: { text: text }, 
                             type: 'success',
                             fadeOut: { enabled: true, delay: 3000 } };
        $('.top-center').notify(notifyOptions).show();
    },

    closeDialog: function(){
         monitorChanges(".local", true);
         $(this.getDOMNode()).modal("hide");
         $('.modal-backdrop').remove(); 
    },


    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (<QuestionEditor question={self.props.question}
                                     metadata={self.props.metadata}  
                                     editSourceQuestionHandler={self.props.editSourceQuestionHandler} 
                                     finishSaving = {self.finishSaving} 
                                     closeDialog={self.closeDialog} 
                                     isNew={self.props.isNew} 
                                     isDuplicate={self.props.caption === window.enums.dialogCaptions.duplicateQuestion}
                                     handlers={self.props.handlers}
                                     viewHistoryMode = {self.props.viewHistoryMode}/>);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             renderFooterButtons={renderFooterButtons} 
                             closeDialogHandler = {this.closeDialog}
                             dialogId="questionEditorModal"/>
                );
    }
});