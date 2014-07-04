/**
* @jsx React.DOM
*/
var EditQuestionNotesDialog = React.createClass({displayName: 'EditQuestionNotesDialog',

	componentDidMount: function(){
		$(this.getDOMNode()).modal("show");
        
        var self = this;
        $(this.getDOMNode()).on('hidden.bs.modal', function () {
            self.props.closeDialogHandler();
            if(self.state.noteChanged){
                questionDataManager.resetState();
            }
        })
	},

    getInitialState: function(){
        return ({noteChanged: false});
    },

    notesChangedHandler: function(){
        this.setState({noteChanged: true});
    },

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };

        var self = this;
        var qId = this.props.questionId;
        var renderBody = function(){
            return (NoteBox( {questionId:qId, canDelete:self.props.canDelete, canAddNote:self.props.canAddNote, notesChangedHandler:self.notesChangedHandler}));
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
