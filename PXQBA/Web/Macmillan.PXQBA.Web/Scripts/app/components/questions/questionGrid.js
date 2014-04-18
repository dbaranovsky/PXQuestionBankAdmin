/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({displayName: 'QuestionGrid',
  
    render: function() { 
        return (
              React.DOM.div( {className:"questionGrid"}, 
                React.DOM.div( {className:"question-grid-item"},  
                     QuestionFilter( {filter:this.props.response.filter, allAvailableColumns:this.props.response.allAvailableColumns})
                ),
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionList( {data:this.props.response.questionList, 
                                        order:this.props.response.order, 
                                        columns:this.props.response.columns,
                                        allAvailableColumns:this.props.response.allAvailableColumns,
                                        handlers:this.props.handlers,
                                        currentPage:this.props.response.pageNumber}
                                        )
                ), 
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionPaginator( {metadata:{
                            currentPage: this.props.response.pageNumber,
                            totalPages: this.props.response.totalPages}} )
                ) 
                 
            ) 

            );
    }
});
