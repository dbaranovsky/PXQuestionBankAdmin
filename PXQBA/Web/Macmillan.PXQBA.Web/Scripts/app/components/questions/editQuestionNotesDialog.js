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
        React.DOM.input( {type:"hidden", value:this.props.key} ),
        React.DOM.div( {className:"flag"}, React.DOM.span( {className:"glyphicon glyphicon-flag"})),
        React.DOM.div( {className:"note-body"}, 
        React.DOM.div( {className:"note-text"}, this.props.children, " ", this.props.key, " ", this.props.noteId),
        React.DOM.div( {className:"note-menu", onClick:this.noteDeleteHandler}, React.DOM.span( {className:"delete-button"},  " X " ))
        )
      )

    );
  }
});


var NoteBox = React.createClass({displayName: 'NoteBox',
  loadNotesFromServer: function() {
    $.ajax({
      url: this.props.url,
      success: function(data) {
        this.setState({data: data});
      }.bind(this)
    });
  },
  handleNoteSubmit: function(note) {
    var notes = this.state.data;
    note.noteId = Math.floor((Math.random()*100)+1);
    notes.push(note);
    this.setState({data: notes});
  //  $.ajax({
  //    url: this.props.url,
  //    type: 'POST',
  //    data: note,
  //    success: function(data) {
  //      this.setState({data: data});
  //    }.bind(this)
  //  });
  },

  noteDeleteHandler: function(noteId) {
    var notes = this.state.data;
    for (var i = 0; i < notes.length; i++) {
        var cur = notes[i];
        if (cur.noteId == noteId) {
            notes.splice(i, 1);
            break;
        }
    }
    this.setState({data: notes});

  },

  getInitialState: function() {
    return {data: []};
  },
  componentWillMount: function() {
    //this.loadNotesFromServer();
    //setInterval(this.loadNotesFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      React.DOM.div( {className:"note-box"}, 
        NoteList( {data:this.state.data,  onNoteDelete:  this.noteDeleteHandler} ),
        NoteForm( {onNoteSubmit:this.handleNoteSubmit} )
      )
    );
  }
});

var NoteList = React.createClass({displayName: 'NoteList',
  render: function() {
    var onDeleteHandler = this.props.onNoteDelete;
    var noteNodes = this.props.data.map(function (note, index) {
      return Note( {noteId:note.noteId, author:note.author, onNoteDelete:onDeleteHandler.bind(null, note.noteId)} , note.text);
    });
    return React.DOM.div( {className:"note-list clearfix"}, noteNodes);
  }
});

var NoteForm = React.createClass({displayName: 'NoteForm',
  handleSubmit: function() {
  //  var author = this.refs.author.getDOMNode().value.trim();
    var text = this.refs.text.getDOMNode().value.trim();
    this.props.onNoteSubmit({text: text});
    this.refs.text.getDOMNode().value = '';
    return false;
  },
  render: function() {
    return (
      React.DOM.div( {className:"modal-footer clearfix"}, 
      React.DOM.form( {className:"note-form", onSubmit:this.handleSubmit}, 
      
        
        React.DOM.textarea( {className:"area-editor",  rows:"5", type:"text", placeholder:"Enter text...", ref:"text"} ),
        React.DOM.button( {type:"submit", className:"btn btn-default"}, "Add note")
      
        
      )
        )
    );
  }
});


var EditQuestionNotesDialog = React.createClass({displayName: 'EditQuestionNotesDialog',

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };
        var renderBody = function(){
            return (NoteBox(null));
        };

        var renderFooterButtons = function() {
            return "";
        };
       
        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody, renderFooterButtons:renderFooterButtons, dialogId:"editQuestionNotesModal"})
                );
    }
});
