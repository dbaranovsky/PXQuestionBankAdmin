/**
* @jsx React.DOM
*/


var Note = React.createClass({displayName: 'Note',
  render: function() {
   
    return (
      React.DOM.div( {className:"note"}, 
        React.DOM.div( {className:"flag"}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"})),
        React.DOM.div( {className:"note-body"}, 
        this.props.children,
        React.DOM.div( {className:"note-menu"}, React.DOM.span( {className:"delete-button"},  " X " ))
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
        NoteList( {data:this.state.data} ),
        NoteForm( {onNoteSubmit:this.handleNoteSubmit} )
      )
    );
  }
});

var NoteList = React.createClass({displayName: 'NoteList',
  render: function() {
    var noteNodes = this.props.data.map(function (note, index) {
      return Note( {key:index, author:note.author}, note.text);
    });
    return React.DOM.div( {className:"noteList"}, noteNodes);
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
      React.DOM.form( {className:"note-form", onSubmit:this.handleSubmit}, 
      
        
        React.DOM.div( {className:"modal-footer"}, 
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
