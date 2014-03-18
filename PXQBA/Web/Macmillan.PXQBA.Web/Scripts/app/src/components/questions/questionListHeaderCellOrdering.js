/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCellOrdering = React.createClass({

  renderClass: function (order) {
    switch(order) {
        case window.enums.orderType.asc:
            return 'glyphicon glyphicon-arrow-up';
        case window.enums.orderType.desc:
            return 'glyphicon glyphicon-arrow-down';
        default:
            return 'order-disabled';    
        } 
  },


  render: function() {
    return (   
            <span className={this.renderClass(this.props.order)}></span>
      );
    }
});