/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({

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
            <th style={ {width: this.props.width}}>
                   <span className={this.props.leftIcon}></span>
                 <span className="header-caption" onClick={this.changeOrdering} data-title={this.props.caption}>
                     {this.props.caption}
                 </span>
                <QuestinListHeaderCellOrdering order={this.props.order} /> 
            </th>
      );
    }
});