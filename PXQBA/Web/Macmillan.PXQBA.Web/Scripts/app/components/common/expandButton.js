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
                'glyphicon': true,
                'glyphicon-chevron-right': !this.props.expanded,
                'glyphicon-chevron-down': this.props.expanded,
    });

      return (   
             React.DOM.span( {className:buttonIcon, onClick:this.onClickHandler},  " " )
      );
    }
});