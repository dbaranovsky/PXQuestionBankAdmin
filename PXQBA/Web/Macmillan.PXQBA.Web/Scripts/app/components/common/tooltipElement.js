/**
* @jsx React.DOM
*/

var ToltipElement = React.createClass({displayName: 'ToltipElement',

  render: function() {
       return (
                 React.DOM.span( {className:"glyphicon glyphicon-question-sign", title:this.props.tooltipText, onClick:this.props.onClickHandler})
    );
  },
});