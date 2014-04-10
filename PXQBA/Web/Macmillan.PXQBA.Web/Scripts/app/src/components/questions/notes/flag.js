/**
* @jsx React.DOM
*/
var Flag = React.createClass({
   getInitialState: function() {
    return {isFlagged: this.props.isFlagged};
  },

  flaggingHandler: function() {
    this.setState({isFlagged: !this.state.isFlagged})
    this.props.flaggingHandler(!this.state.isFlagged);
  
  },
  render: function() {
    if (this.state.isFlagged){
       return ( <div className="flag flagged" onClick={this.flaggingHandler}><span className="glyphicon glyphicon-flag"></span></div> );
    }
       return ( <div className="flag" onClick={this.flaggingHandler}><span className="glyphicon glyphicon-flag"></span></div> );
  }
});