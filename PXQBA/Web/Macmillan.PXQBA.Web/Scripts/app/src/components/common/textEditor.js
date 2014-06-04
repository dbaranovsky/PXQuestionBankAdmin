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
                <div className="">
                  <input type="text" className="form-control" value={this.props.value} onChange={this.handleChange} onBlur={this.props.onBlurHandler} />
                </div>
     );
  },
});