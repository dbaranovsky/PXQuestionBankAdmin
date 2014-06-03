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
             React.DOM.div(null,   
                  React.DOM.input( {type:"text", value:this.props.value, onChange:this.handleChange, onBlur:this.props.onBlurHandler} )
              )
    );
  },
});