/**
* @jsx React.DOM
*/
var EditQuestionNotesDialog = React.createClass({displayName: 'EditQuestionNotesDialog',

	componentDidMount: function(){
		$(this.getDOMNode()).modal("show");
        
        var closeDialogHandler = this.props.closeDialogHandler;
        $(this.getDOMNode()).on('hidden.bs.modal', function () {
            closeDialogHandler();
        })
	},

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };

        var self = this;
        var qId = this.props.questionId;
        var renderBody = function(){
            return (NoteBox( {questionId:qId, canDelete:self.props.canDelete, canAddNote:self.props.canAddNote}));
        };

        var renderFooterButtons = function() {
            return "";
        };
       
        return (ModalDialog( {renderHeaderText:renderHeaderText,
                             renderBody:renderBody, 
                             renderFooterButtons:renderFooterButtons,
                             dialogId:"editQuestionNotesModal"} )

                );
    }
});
