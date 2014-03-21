/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({displayName: 'QuestionListHeader',

 
 // ToDo: replace this on property, after implementation add column functionality 
  initializationHeaderCells: function(ordering) {
    var columns = [
      {width:'10%', caption:'Chapter'},
      {width:'10%', caption:'Bank'},
      {width:'10%', caption:'Seq'},
      {width:'40%', caption:'Title', leftIcon:"glyphicon glyphicon-chevron-right titles-expander"}, 
      {width:'10%', caption:'Format'},
    ];
     
    return this.applayOrdering(columns, ordering);
  },

  applayOrdering: function(columns, ordering) {
    if(ordering.orderType!='none') {
      for(var i=0; i<columns.length; i++) {
        if(columns[i].caption==ordering.orderField) {
           columns[i].order=ordering.orderType;
           break;
        }
      }
    }
    
    return columns;
  },

  renderCell: function(cell) {
      return (QuestinListHeaderCell( 
                  {width:cell.width, 
                  caption:cell.caption,
                  order:cell.order,
                  leftIcon:cell.leftIcon} ));
  },

  render: function() {
    // ToDo: replace this on property, after implementation add column functionality 
    var cells = this.initializationHeaderCells(this.props.ordering);
 
    var renderedCell = cells.map(this.renderCell);

    return ( 
        React.DOM.tr(null, 
            React.DOM.th( {style: {width:'5%'}},  " ", React.DOM.input( {type:"checkbox"})),
             renderedCell,
            QuestinListHeaderCell( {width:"15%", caption:""})
        )
      );
    }
});

