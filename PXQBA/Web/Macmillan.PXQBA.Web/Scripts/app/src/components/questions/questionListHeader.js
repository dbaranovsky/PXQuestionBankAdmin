/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({

  initializationHeaderCells: function(ordering) {
    var columns = this.props.columns;
     
    return this.applyOrdering(columns, ordering);
  },

  applyOrdering: function(columns, ordering) {
    if(ordering.orderType!='none') {
      for(var i=0; i<columns.length; i++) {
        if(columns[i].metadataName==ordering.orderField) {
           columns[i].order=ordering.orderType;
           break;
        }
      }
    }
    
    return columns;
  },

  renderCell: function(descriptor) {
      return (<QuestinListHeaderCell 
                  width={descriptor.width} 
                  caption={descriptor.friendlyName}
                  metadataName={descriptor.metadataName}
                  order={descriptor.order}
                  leftIcon={descriptor.leftIcon}
                  canNotDelete={descriptor.canNotDelete} />);
  },

  render: function() {
    var cells = this.initializationHeaderCells(this.props.ordering);
    var renderedCell = cells.map(this.renderCell);
    
    return ( 
        <tr>
            <th style={ {width:'5%'}}> <input type="checkbox"/></th>
             {renderedCell}
            <th> <QuestionListColumnAppender displayedFields={this.props.columns} 
                                             allFields={this.props.allAvailableColumns}  /></th>
        </tr>
      );
    }
});

