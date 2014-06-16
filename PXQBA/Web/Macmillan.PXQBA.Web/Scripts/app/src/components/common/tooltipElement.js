/**
* @jsx React.DOM
*/

var ToltipElement = React.createClass({

  render: function() {
  	var className = "glyphicon glyphicon-question-sign"
  	if(this.props.classNameProp!=null) {
  		className+=" "+this.props.classNameProp;
  	}
       return (
                <span className={className} title={this.props.tooltipText} onClick={this.props.onClickHandler}></span>
    );
  },
});