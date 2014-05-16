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

        <div className="note-body">
        <div className="note-text">{this.props.note.text}</div>
        <div className="note-menu" onClick={this.noteDeleteHandler}><span className="delete-button"> X </span></div>
        </div>
      </div>

    );
  }
});