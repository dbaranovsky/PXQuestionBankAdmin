/**
* @jsx React.DOM
*/
var NoteList = React.createClass({displayName: 'NoteList',
  render: function() {
    var self = this;
    var noteNodes = this.props.data.map(function (note, index) {
      return Note( {note:note, author:note.author, onNoteDelete:self.props.onNoteDelete.bind(null, note), onNoteUpdate:self.props.onNoteUpdate, canDelete:self.props.canDelete});
    });
    return React.DOM.div( {className:"note-list clearfix"}, noteNodes);
  }
});
