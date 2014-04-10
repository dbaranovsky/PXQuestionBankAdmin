/**
* @jsx React.DOM
*/
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