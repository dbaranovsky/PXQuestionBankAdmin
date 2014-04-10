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