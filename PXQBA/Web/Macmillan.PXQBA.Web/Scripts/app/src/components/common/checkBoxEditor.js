/**
* @jsx React.DOM
*/

var CheckBoxEditor = React.createClass({

  onChangeHandler: function(event) {
    this.props.onChangeHandler(event.target.checked);
  },

  render: function() {
     return (
        <div className="checkbox">
          <label>
              <input type="checkbox" onChange={this.onChangeHandler} checked={this.props.value}/> {this.props.label}
          </label>
        </div>
     );
  },
});