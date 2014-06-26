/**
* @jsx React.DOM
*/
var NoteForm = React.createClass({displayName: 'NoteForm',

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
      React.DOM.div( {className:"note-form-footer"}, 
          React.DOM.form( {className:"note-form", onSubmit:this.handleSubmit},     
            React.DOM.textarea( {className:"area-editor",  disabled:!this.props.canAddNote, rows:"5", type:"text", placeholder:this.props.canAddNote? "Enter text..." : "", ref:"text"} ),
            React.DOM.button( {type:"submit",  disabled:!this.props.canAddNote, className:"btn btn-default"}, "Add note")
          )
      )
    );
  }
});