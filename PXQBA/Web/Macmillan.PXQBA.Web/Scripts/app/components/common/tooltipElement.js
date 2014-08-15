/**
* @jsx React.DOM
*/

var ToltipElement = React.createClass({displayName: 'ToltipElement',

  render: function() {
  	var className = "glyphicon glyphicon-question-sign"
  	if(this.props.classNameProp!=null) {
  		className+=" "+this.props.classNameProp;
  	}
       return (
                React.DOM.span({className: className, title: this.props.tooltipText, onClick: this.props.onClickHandler})
    );
  },
});