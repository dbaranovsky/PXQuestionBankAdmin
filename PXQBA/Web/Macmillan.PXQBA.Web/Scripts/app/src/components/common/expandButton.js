/**
* @jsx React.DOM
*/ 

var ExpandButton = React.createClass({

  onClickHandler: function() {
     this.props.onClickHandler(!this.props.expanded);
  },

  render: function() {   
    var buttonIcon = React.addons.classSet({
                'expand-button': true,
                'glyphicon': true,
                'glyphicon-chevron-right': !this.props.expanded,
                'glyphicon-chevron-down': this.props.expanded,
    });

    var tooltipTitle = this.props.expanded ? "Collapse "+ this.props.targetCaption : "Expand "+ this.props.targetCaption;

      return (   
             <span className={buttonIcon} onClick={this.onClickHandler} data-toggle="tooltip" title={tooltipTitle}> </span>
      );
    }
});