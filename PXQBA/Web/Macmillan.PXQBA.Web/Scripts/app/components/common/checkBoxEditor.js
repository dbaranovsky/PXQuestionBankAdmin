/**
* @jsx React.DOM
*/

var CheckBoxEditor = React.createClass({displayName: 'CheckBoxEditor',

  onChangeHandler: function(event) {
    this.props.onChangeHandler(event.target.checked);
  },

  render: function() {
     return (
        React.DOM.div( {className:"checkbox"}, 
          React.DOM.label(null, 
              React.DOM.input( {type:"checkbox", onChange:this.onChangeHandler, checked:this.props.value}), " ", this.props.label
          )
        )
     );
  },
});