/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCellOrdering = React.createClass({displayName: 'QuestinListHeaderCellOrdering',

  renderClass: function (order) {

    switch(order) {
        case "asc":
            return 'glyphicon glyphicon-arrow-up';
        case "desc":
            return 'glyphicon glyphicon-arrow-down';
        default:
            return 'order-disabled';    
        } 
  },


  render: function() {
    return (   
            React.DOM.span( {className:this.renderClass(this.props.order)})
      );
    }
});