/**
* @jsx React.DOM
*/


var Note = React.createClass({
  render: function() {
   
    return (
      <div className="note">
        <div className="flag"><span className="glyphicon glyphicon-list-alt"></span></div>
        <div className="note-body">
        {this.props.children}
        <div className="note-menu"><span className="delete-button"> X </span></div>
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
      <div className="note-box">
        <NoteList data={this.state.data} />
        <NoteForm onNoteSubmit={this.handleNoteSubmit} />
      </div>
    );
  }
});

var NoteList = React.createClass({
  render: function() {
    var noteNodes = this.props.data.map(function (note, index) {
      return <Note key={index} author={note.author}>{note.text}</Note>;
    });
    return <div className="noteList">{noteNodes}</div>;
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
      <form className="note-form" onSubmit={this.handleSubmit}>
      
        
        <div className="modal-footer">
        <textarea className="area-editor"  rows="5" type="text" placeholder="Enter text..." ref="text" />
        <button type="submit" className="btn btn-default">Add note</button>
        </div>
        
      </form>
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
