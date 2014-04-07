/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({displayName: 'QuestionGrid',
  
    render: function() { 
        return (
            React.DOM.div( {className:"questionGrid"}, 
                React.DOM.div( {className:"question-grid-item"},  
                     QuestionFilter(null)
                ),
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionList( {data:this.props.response.questionList, 
                                        order:this.props.response.order, 
                                        columns:this.props.response.columns,
                                        allAvailableColumns:this.props.response.allAvailableColumns,
                                        handlers:this.props.handlers}
                                        )
                ), 
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionPaginator( {metadata:{
                            currentPage: this.props.response.currentPage,
                            totalPages: this.props.response.totalPages}} )
                ) 
            ) 
            );
    }
});
