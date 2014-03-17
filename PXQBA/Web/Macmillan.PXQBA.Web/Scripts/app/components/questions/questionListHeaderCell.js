/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({displayName: 'QuestinListHeaderCell',
 
  render: function() {
             leftIcon = this.renderLeftIcon(this.props.leftIcon);
      return (   
            React.DOM.th( {style: {width: this.props.width}, className:this.props.customClassName}, 
                   React.DOM.span( {className:this.props.leftIcon},  " " ),
                 React.DOM.span(null, 
                     this.props.caption
                 ),
                QuestinListHeaderCellOrdering( {order:this.props.order}) 
            )
      );
    }
});