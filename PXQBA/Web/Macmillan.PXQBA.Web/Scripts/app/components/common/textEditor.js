/**
* @jsx React.DOM
*/

var TextEditor = React.createClass({displayName: 'TextEditor',

  handleChange: function(event) {
    var text = event.target.value;
    this.props.dataChangeHandler(text);
  },

  render: function() {
       
   var classNameProps="form-control";
   if(this.props.classNameProps!=null) {
      classNameProps+=" "+this.props.classNameProps;
   }
   
   return (
           React.DOM.div( {className:""}, 
              React.DOM.input( {type:"text", className:classNameProps,
                                 disabled: this.props.disabled ? 'disabled' : undefined,
                                 value:this.props.value, 
                                 onChange:this.handleChange, 
                                 onBlur:this.props.onBlurHandler, 
                                 onKeyPress:this.props.onKeyPressHandler})
            )
     );
  },
});