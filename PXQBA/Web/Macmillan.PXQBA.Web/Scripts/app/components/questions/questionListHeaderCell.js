/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({displayName: 'QuestinListHeaderCell',

  getNextOrdering: function(currentOrder) {
    switch(currentOrder) {
        case window.enums.orderType.asc:
            return window.enums.orderType.desc;
        case window.enums.orderType.desc:
            return window.enums.orderType.none;
        default:
            return window.enums.orderType.asc;    
    } 
  },

  changeOrdering: function(event) {
    var title = $(event.target).data('title');
    routsManager.setOrder(this.getNextOrdering(this.props.order), title);
 },

  render: function() {
      return (   
            React.DOM.th( {style: {width: this.props.width}, className:this.props.customClassName}, 
                   React.DOM.span( {className:this.props.leftIcon}),
                 React.DOM.span( {className:"header-caption", onClick:this.changeOrdering, 'data-title':this.props.caption}, 
                     this.props.caption
                 ),
                QuestinListHeaderCellOrdering( {order:this.props.order} ) 
            )
      );
    }
});