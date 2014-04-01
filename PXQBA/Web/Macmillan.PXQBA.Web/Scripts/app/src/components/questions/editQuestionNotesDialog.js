/**
* @jsx React.DOM
*/


var Note = React.createClass({
  noteDeleteHandler: function()
  {
    this.props.onNoteDelete();
  },

  render: function() {
   
    return (
      <div className="note clearfix">
        <input type="hidden" value={this.props.key} />
        <div className="flag"><span className="glyphicon glyphicon-flag"></span></div>
        <div className="note-body">
        <div className="note-text">{this.props.children} {this.props.key} {this.props.noteId}</div>
        <div className="note-menu" onClick={this.noteDeleteHandler}><span className="delete-button"> X </span></div>
        </div>
      </div>

    );
  }
});


var NoteBox = React.createClass({
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
      <div className="note-box">
        <NoteList data={this.state.data}  onNoteDelete = {this.noteDeleteHandler} />
        <NoteForm onNoteSubmit={this.handleNoteSubmit} />
      </div>
    );
  }
});

var NoteList = React.createClass({
  render: function() {
    var onDeleteHandler = this.props.onNoteDelete;
    var noteNodes = this.props.data.map(function (note, index) {
      return <Note noteId={note.noteId} author={note.author} onNoteDelete={onDeleteHandler.bind(null, note.noteId)} >{note.text}</Note>;
    });
    return <div className="note-list clearfix">{noteNodes}</div>;
  }
});

var NoteForm = React.createClass({
  handleSubmit: function() {
  //  var author = this.refs.author.getDOMNode().value.trim();
    var text = this.refs.text.getDOMNode().value.trim();
    this.props.onNoteSubmit({text: text});
    this.refs.text.getDOMNode().value = '';
    return false;
  },
  render: function() {
    return (
      <div className="modal-footer clearfix">
      <form className="note-form" onSubmit={this.handleSubmit}>
      
        
        <textarea className="area-editor"  rows="5" type="text" placeholder="Enter text..." ref="text" />
        <button type="submit" className="btn btn-default">Add note</button>
      
        
      </form>
        </div>
    );
  }
});


var EditQuestionNotesDialog = React.createClass({

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };
        var renderBody = function(){
            return (<NoteBox/>);
        };

        var renderFooterButtons = function() {
            return "";
        };
       
        return (<ModalDialog renderHeaderText={renderHeaderText} renderBody={renderBody} renderFooterButtons={renderFooterButtons} dialogId="editQuestionNotesModal"/>
                );
    }
});
