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
        <input type="hidden" value={this.props.note.id} />
        <div className="flag"><span className="glyphicon glyphicon-flag"></span></div>
        <div className="note-body">
        <div className="note-text">{this.props.note.text}</div>
        <div className="note-menu" onClick={this.noteDeleteHandler}><span className="delete-button"> X </span></div>
        </div>
      </div>

    );
  }
});


var NoteBox = React.createClass({
  loadNotes: function(data) {
   this.setState({data: data});

  },
  handleNoteSubmit: function(note) {
    alert(note.text);
    var notes = this.state.data;
    note.id = Math.floor((Math.random()*100)+1);
    notes.push(note);
    this.setState({data: notes});
    questionDataManager.saveQuestionNote(note);
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
      <div className="note-box">
        <NoteList data={this.state.data}  onNoteDelete = {this.noteDeleteHandler} />
        <NoteForm onNoteSubmit={this.handleNoteSubmit} questionId={this.props.questionId} />
      </div>
    );
  }
});

var NoteList = React.createClass({
  render: function() {
    var onDeleteHandler = this.props.onNoteDelete;
    var noteNodes = this.props.data.map(function (note, index) {
      return <Note note={note} author={note.author} onNoteDelete={onDeleteHandler.bind(null, note)} />;
    });
    return <div className="note-list clearfix">{noteNodes}</div>;
  }
});

var NoteForm = React.createClass({
  handleSubmit: function() {

    var text = this.refs.text.getDOMNode().value.trim();
    var note = {
      "id": 0,
      "text": text,
      "questionId": this.props.questionId,
      "isFlagged": false
    };
    alert(note.text);
    this.props.onNoteSubmit(note);
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

	componentDidMount: function(){
		$(this.getDOMNode()).modal("show");
	},

    render: function() {
        var renderHeaderText = function() {
            return "Notes";
        };

        var qId = this.props.questionId;
        var renderBody = function(){
            return (<NoteBox questionId={qId} />);
        };

        var renderFooterButtons = function() {
            return "";
        };
       
        return (<ModalDialog renderHeaderText={renderHeaderText}
                             renderBody={renderBody} 
                             renderFooterButtons={renderFooterButtons}
                             dialogId="editQuestionNotesModal"
                             closeDialogHandler={this.props.closeDialogHandler}/>

                );
    }
});
