/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorNumber = React.createClass({displayName: 'QuestionInlineEditorNumber',

  onAcceptEventHandler: function() {
        var value = this.state.value;
        if(value != null) {
            questionDataManager.saveQuestionData(this.props.metadata.questionId,
                                                 this.props.metadata.field,
                                                 value);
 
        }
        this.props.afterEditingHandler();
  },

  onCancelEventHandler: function() {
      this.props.afterEditingHandler();
  },

  getInitialState: function() {
      return {value: this.props.metadata.currentValue};
  },

  handleChange: function(event) {
      var value = parseInt(event.target.value);
      if(!isNaN(value)) {
          this.setState({value: value});
      }
  },

  render: function() {
        var value = this.state.value;
        return ( 
                React.DOM.div( {className:"text-editor-container"},  
                          React.DOM.div( {className:"input-group"}, 
                              React.DOM.input( {type:"text", value:value, onChange:this.handleChange, className:"form-control"} ),
                              React.DOM.span( {className:"input-group-btn"}, 
                                React.DOM.button( {className:"btn btn-default", type:"button", onClick:this.onAcceptEventHandler}, "Accept"),
                                React.DOM.button( {className:"btn btn-default", type:"button",onClick:this.onCancelEventHandler}, "Cancel")
                             )
                           )
                )
            );
     }

});