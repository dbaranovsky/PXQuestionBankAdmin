/**
* @jsx React.DOM
*/
var NoteList = React.createClass({
  render: function() {
    var self = this;
    var noteNodes = this.props.data.map(function (note, index) {
      return <Note note={note} author={note.author} onNoteDelete={self.props.onNoteDelete.bind(null, note)} onNoteUpdate={self.props.onNoteUpdate} />;
    });
    return <div className="note-list clearfix">{noteNodes}</div>;
  }
});
