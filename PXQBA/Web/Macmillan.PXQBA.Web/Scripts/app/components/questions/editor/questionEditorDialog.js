/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({displayName: 'QuestionEditorDialog',

    
    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
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
         $(this.getDOMNode()).modal("hide");
    },


    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (QuestionEditor( {question:self.props.question, finishSaving:  self.finishSaving, closeDialog:self.closeDialog}));
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             renderFooterButtons:renderFooterButtons, 
                             dialogId:"questionEditorModal"})
                );
    }
});