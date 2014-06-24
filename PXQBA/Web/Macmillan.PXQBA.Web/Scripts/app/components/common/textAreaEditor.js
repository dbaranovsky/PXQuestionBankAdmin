/**
* @jsx React.DOM
*/

var TextAreaEditor = React.createClass({displayName: 'TextAreaEditor',

  changeHandler: function(event) {
    var text = event.target.value
    this.props.dataChangeHandler(text);
  },

  render: function() {
       return (
             React.DOM.div(null,   
                React.DOM.textarea( 
                  {disabled: this.props.disabled ? 'disabled' : undefined,
                  className:this.props.classNameProps,
                  onChange:this.changeHandler, 
                  type:"text", 
                  placeholder:"Enter text...", 
                  value:this.props.value} )
              )
    );
  },
});