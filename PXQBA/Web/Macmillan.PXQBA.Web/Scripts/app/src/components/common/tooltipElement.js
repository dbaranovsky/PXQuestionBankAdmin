/**
* @jsx React.DOM
*/

var ToltipElement = React.createClass({

  render: function() {
       return (
                 <span className="glyphicon glyphicon-question-sign" title={this.props.tooltipText}></span>
    );
  },
});