/**
* @jsx React.DOM
*/


var Note = React.createClass({displayName: 'Note',
  noteDeleteHandler: function()
  {
    this.props.onNoteDelete();
  },

  render: function() {
   
    return (
      React.DOM.div( {className:"note clearfix"}, 
        React.DOM.input( {type:"hidden", value:this.props.note.id} ),
        React.DOM.div( {className:"flag"}, React.DOM.span( {className:"glyphicon glyphicon-flag"})),
        React.DOM.div( {className:"note-body"}, 
        React.DOM.div( {className:"note-text"}, this.props.note.text),
        React.DOM.div( {className:"note-menu", onClick:this.noteDeleteHandler}, React.DOM.span( {className:"delete-button"},  " X " ))
        )
      )

    );
  }
});


var NoteBox = React.createClass({displayName: 'NoteBox',
  loadNotes: function(data) {
   this.setState({data: data});

  },
  handleNoteSubmit: function(note) {
    var notes = this.state.data;
    questionDataManager.saveQuestionNote(note).done(function(data){
    notes.push(data);
    this.setState({data: notes});
    })
   
  },

  noteDeleteHandler: function(note) {
    var notes = this.state.data;
    for (var i = 0; i < notes.length; i++) {
        var cur = notes[i];
        if (cur.id == note.id) {
            notes.splice(i, 1);
            break;
        }
    }
    questionDataManager.deleteQuestionNote(note);
    this.setState({data: notes});

  },

  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    var response = questionDataManager.getQuestionNotes(this.props.questionId);
    response.done(this.loadNotes);
  },
  render: function() {
    return (
      React.DOM.div( {className:"note-box"}, 
        NoteList( {data:this.state.data,  onNoteDelete:  this.noteDeleteHandler} ),
        NoteForm( {onNoteSubmit:this.handleNoteSubmit, questionId:this.props.questionId} )
      )
    );
  }
});

var NoteList = React.createClass({displayName: 'NoteList',
  render: function() {
    var onDeleteHandler = this.props.onNoteDelete;
    var noteNodes = this.props.data.map(function (note, index) {
      return Note( {note:note, author:note.author, onNoteDelete:onDeleteHandler.bind(null, note)} );
    });
    return React.DOM.div( {className:"note-list clearfix"}, noteNodes);
  }
});

var NoteForm = React.createClass({displayName: 'NoteForm',
  handleSubmit: function() {

    var text = this.refs.text.getDOMNode().value.trim();
    var note = {
      "id": 0,
      "text": text,
      "questionId": this.props.questionId,
      "isFlagged": false
    };
    this.props.onNoteSubmit(note);
    this.refs.text.getDOMNode().value = '';
    return false;
  },
  render: function() {
    return (
      React.DOM.div( {className:"modal-footer clearfix"}, 
          React.DOM.form( {className:"note-form", onSubmit:this.handleSubmit},     
            React.DOM.div( {className:"flag"}, React.DOM.span( {className:"glyphicon glyphicon-flag"})),  
            React.DOM.textarea( {className:"area-editor",  rows:"5", type:"text", placeholder:"Enter text...", ref:"text"} ),
            React.DOM.button( {type:"submit", className:"btn btn-default"}, "Add note")
          )
      )
    );
  }
});


var EditQuestionNotesDialog = React.createClass({displayName: 'EditQuestionNotesDialog',

	componentDidMount: function(){
		$(this.getDOMNode()).modal("show");
	},

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };

        var qId = this.props.questionId;
        var renderBody = function(){
            return (NoteBox( {questionId:qId} ));
        };

        var renderFooterButtons = function() {
            return "";
        };
       
        return (ModalDialog( {renderHeaderText:renderHeaderText,
                             renderBody:renderBody, 
                             renderFooterButtons:renderFooterButtons,
                             dialogId:"editQuestionNotesModal",
                             closeDialogHandler:this.props.closeDialogHandler})

                );
    }
});
