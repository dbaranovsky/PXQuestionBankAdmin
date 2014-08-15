/**
* @jsx React.DOM
*/ 

var ExpandButton = React.createClass({displayName: 'ExpandButton',

  onClickHandler: function() {
     this.props.onClickHandler(!this.props.expanded);
  },

  render: function() {   
    var buttonIcon = React.addons.classSet({
                'expand-button': true,
                'icon-right-open': !this.props.expanded,
                'icon-down-open': this.props.expanded,
    });

    var tooltipTitle = this.props.expanded ? "Collapse "+ this.props.targetCaption : "Expand "+ this.props.targetCaption;

      return (   
             React.DOM.span({className: buttonIcon, onClick: this.onClickHandler, 'data-toggle': "tooltip", title: tooltipTitle}, " ")
      );
    }
});