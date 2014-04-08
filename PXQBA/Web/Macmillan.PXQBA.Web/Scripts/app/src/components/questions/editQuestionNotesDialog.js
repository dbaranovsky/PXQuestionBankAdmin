/**
* @jsx React.DOM
*/


var Note = React.createClass({

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
      <div className="note clearfix">
        <input type="hidden" value={this.props.note.id} />
        <Flag flaggingHandler={this.noteUpdateHandler} isFlagged={this.props.note.isFlagged}/>
        <div className="note-body">
        <div className="note-text">{this.props.note.text}</div>
        <div className="note-menu" onClick={this.noteDeleteHandler}><span className="delete-button"> X </span></div>
        </div>
      </div>

    );
  }
});

var NoteForm = React.createClass({

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
      <div className="modal-footer clearfix">
          <form className="note-form" onSubmit={this.handleSubmit}>    
            <Flag flaggingHandler={this.flaggingHandler} isFlagged={false}/>
            <textarea className="area-editor"  rows="5" type="text" placeholder="Enter text..." ref="text" />
            <button type="submit" className="btn btn-default">Add note</button>
          </form>
      </div>
    );
  }
});

var Flag = React.createClass({
   getInitialState: function() {
    return {isFlagged: this.props.isFlagged};
  },

  flaggingHandler: function() {
    this.setState({isFlagged: !this.state.isFlagged})
    this.props.flaggingHandler(!this.state.isFlagged);
  
  },
  render: function() {
    if (this.state.isFlagged){
       return ( <div className="flag flagged" onClick={this.flaggingHandler}><span className="glyphicon glyphicon-flag"></span></div> );
    }
       return ( <div className="flag" onClick={this.flaggingHandler}><span className="glyphicon glyphicon-flag"></span></div> );
  }
});


var NoteBox = React.createClass({
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
      <div className="note-box">
        <NoteList data={this.state.data}  onNoteDelete = {this.noteDeleteHandler} onNoteUpdate={this.noteUpdateHandler} />
        <NoteForm onNoteSubmit={this.handleNoteSubmit} questionId={this.props.questionId} />
      </div>
    );
  }
});

var NoteList = React.createClass({
  render: function() {
    var self = this;
    var noteNodes = this.props.data.map(function (note, index) {
      return <Note note={note} author={note.author} onNoteDelete={self.props.onNoteDelete.bind(null, note)} onNoteUpdate={self.props.onNoteUpdate} />;
    });
    return <div className="note-list clearfix">{noteNodes}</div>;
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
