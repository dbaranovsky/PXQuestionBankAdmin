/**
* @jsx React.DOM
*/

var TextEditor = React.createClass({displayName: 'TextEditor',

  handleChange: function(event) {
    var text = event.target.value;
    this.props.dataChangeHandler(text);
  },

  render: function() {
       return (
                React.DOM.div( {className:""}, 
                  React.DOM.input( {type:"text", className:"form-control", value:this.props.value, onChange:this.handleChange, onBlur:this.props.onBlurHandler} )
                )
     );
  },
});