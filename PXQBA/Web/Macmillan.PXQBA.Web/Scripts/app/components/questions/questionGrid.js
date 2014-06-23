/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({displayName: 'QuestionGrid',
  
  customPaginatorClickHandle: function(page){
      routsManager.setPage(page);
  },

  getCapabilities: function(){
    return {
            canDuplicateQuestion: this.props.response.canDuplicateQuestion
          };
  },
     
    render: function() { 
        return (
              React.DOM.div( {className:"questionGrid"}, 
                React.DOM.div( {className:"question-grid-item"},  
                     QuestionFilter( {filter:this.props.response.filter, allAvailableColumns:this.props.response.allAvailableColumns})
                ),
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionList( {data:this.props.response.questionList, 
                                        filter:this.props.response.filter,
                                        order:this.props.response.order, 
                                        columns:this.props.response.columns,
                                        questionCardTemplate:  this.props.response.questionCardLayout,
                                        allAvailableColumns:this.props.response.allAvailableColumns,
                                        handlers:this.props.handlers,
                                        currentPage:this.props.response.pageNumber,
                                        capabilities:  this.getCapabilities()}
                                        )
                ), 
                React.DOM.div( {className:"question-grid-item"},  
                    Paginator( {metadata:{
                            currentPage: this.props.response.pageNumber,
                            totalPages: this.props.response.totalPages}, 
                            customPaginatorClickHandle:this.customPaginatorClickHandle})
                ) 
                 
            ) 

            );
    }
});
