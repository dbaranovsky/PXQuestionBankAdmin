/**
* @jsx React.DOM
*/

var TextAreaEditor = React.createClass({

  changeHandler: function(event) {
    var text = event.target.value
    this.props.dataChangeHandler(text);
  },

  render: function() {
       return (
             <div>  
                <textarea 
                  disabled={this.props.disabled ? 'disabled' : undefined}
                  className={this.props.classNameProps}
                  onChange={this.changeHandler} 
                  type="text" 
                  placeholder="Enter text..." 
                  value={this.props.value} />
              </div>
    );
  },
});