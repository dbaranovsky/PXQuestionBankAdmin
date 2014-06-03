/**
* @jsx React.DOM
*/

var TextEditor = React.createClass({

  handleChange: function(event) {
    var text = event.target.value;
    this.props.dataChangeHandler(text);
  },

  render: function() {
       return (
             <div>  
                  <input type="text" value={this.props.value} onChange={this.handleChange} onBlur={this.props.onBlurHandler} />
              </div>
    );
  },
});