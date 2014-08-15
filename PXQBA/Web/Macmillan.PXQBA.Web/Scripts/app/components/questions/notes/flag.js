/**
* @jsx React.DOM
*/
var Flag = React.createClass({displayName: 'Flag',
   getInitialState: function() {
    return {isFlagged: this.props.isFlagged};
  },

  flaggingHandler: function() {
    this.setState({isFlagged: !this.state.isFlagged})
    this.props.flaggingHandler(!this.state.isFlagged);
  
  },
  render: function() {
    if (this.state.isFlagged){
       return ( React.DOM.div({className: "flag flagged", onClick: this.flaggingHandler}, React.DOM.span({className: "glyphicon glyphicon-flag"})) );
    }
       return ( React.DOM.div({className: "flag", onClick: this.flaggingHandler}, React.DOM.span({className: "glyphicon glyphicon-flag"})) );
  }
});