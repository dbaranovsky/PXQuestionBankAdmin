/**
* @jsx React.DOM
*/


var Note = React.createClass({displayName: 'Note',

  getInitialState: function() {
    return {note: this.props.note};
  },
  noteDeleteHandler: function()
  {
    this.props.onNoteDelete();
  },

   noteUpdateHandler: function(isFlagged){

    var note = this.props.note;
    note.isFlagged = isFlagged;
    this.props.onNoteUpdate(note);
  },

  render: function() {
   
    return (
      React.DOM.div( {className:"note clearfix"}, 
        React.DOM.input( {type:"hidden", value:this.props.note.id} ),
        Flag( {flaggingHandler:this.noteUpdateHandler, isFlagged:this.props.note.isFlagged}),
        React.DOM.div( {className:"note-body"}, 
        React.DOM.div( {className:"note-text"}, this.props.note.text),
        React.DOM.div( {className:"note-menu", onClick:this.noteDeleteHandler}, React.DOM.span( {className:"delete-button"},  " X " ))
        )
      )

    );
  }
});

var NoteForm = React.createClass({displayName: 'NoteForm',

  getInitialState: function() {
    return {isFlagged: false};
  },

  flaggingHandler: function(isFlagged){

    this.setState({isFlagged: isFlagged});
  },
  handleSubmit: function() {

    var text = this.refs.text.getDOMNode().value.trim();
    var note = {
      "id": 0,
      "text": text,
      "questionId": this.props.questionId,
      "isFlagged": this.state.isFlagged
    };
    this.props.onNoteSubmit(note);
    this.refs.text.getDOMNode().value = '';
    return false;
  },
  render: function() {
    return (
      React.DOM.div( {className:"modal-footer clearfix"}, 
          React.DOM.form( {className:"note-form", onSubmit:this.handleSubmit},     
            Flag( {flaggingHandler:this.flaggingHandler, isFlagged:false}),
            React.DOM.textarea( {className:"area-editor",  rows:"5", type:"text", placeholder:"Enter text...", ref:"text"} ),
            React.DOM.button( {type:"submit", className:"btn btn-default"}, "Add note")
          )
      )
    );
  }
});

var Flag = React.createClass({displayName: 'Flag',
   getInitialState: function() {
    return {isFlagged: this.props.isFlagged};
  },

  flaggingHandler: function() {
    this.setState({isFlagged: !this.state.isFlagged})
    this.props.flaggingHandler(!this.state.isFlagged);
  
  },
  render: function() {
    if (this.state.isFlagged){
       return ( React.DOM.div( {className:"flag flagged", onClick:this.flaggingHandler}, React.DOM.span( {className:"glyphicon glyphicon-flag"})) );
    }
       return ( React.DOM.div( {className:"flag", onClick:this.flaggingHandler}, React.DOM.span( {className:"glyphicon glyphicon-flag"})) );
  }
});


var NoteBox = React.createClass({displayName: 'NoteBox',
  loadNotes: function(data) {
   this.setState({data: data});

  },
  handleNoteSubmit: function(note) {
    var notes = this.state.data;
    questionDataManager.createQuestionNote(note).done(function(data){
    notes.push(data);
    this.setState({data: notes});
    })
   
  },

  noteUpdateHandler: function(note){
    var notes = this.state.data;
    
     for (var i = 0; i < notes.length; i++) {
        var cur = notes[i];
        if (cur.id == note.id) {
            notes[i]= note;
            break;
        }
    }
    questionDataManager.saveQuestionNote(note);
    this.setState({data: notes});

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
        NoteList( {data:this.state.data,  onNoteDelete:  this.noteDeleteHandler, onNoteUpdate:this.noteUpdateHandler} ),
        NoteForm( {onNoteSubmit:this.handleNoteSubmit, questionId:this.props.questionId} )
      )
    );
  }
});

var NoteList = React.createClass({displayName: 'NoteList',
  render: function() {
    var self = this;
    var noteNodes = this.props.data.map(function (note, index) {
      return Note( {note:note, author:note.author, onNoteDelete:self.props.onNoteDelete.bind(null, note), onNoteUpdate:self.props.onNoteUpdate} );
    });
    return React.DOM.div( {className:"note-list clearfix"}, noteNodes);
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
