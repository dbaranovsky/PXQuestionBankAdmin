/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorNumber = React.createClass({displayName: 'QuestionInlineEditorNumber',

  onAcceptEventHandler: function() {
        var value = this.state.value;
        this.props.saveVelueHandler(value)
  },

  onCancelEventHandler: function() {
      this.props.afterEditingHandler();
  },

  getInitialState: function() {
      return {value: this.props.metadata.currentValue};
  },

  handleChange: function(event) {
      var maxValue = 9999999;
      var minValue = 0;

      var value = event.target.value

      if(value=='') {
         value=0;
      }      
      else {
         value = parseInt(event.target.value);
      }

      if(value<minValue) {
        return;
      }

      if(value>maxValue) {
        return;
      }

      if(isNaN(value)) {
         return;
      }

       this.setState({value: value});
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