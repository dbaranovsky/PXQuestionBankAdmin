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
                    QuestionList( {data:this.props.data, order:this.props.order, columns:this.props.columns})
                ), 
                React.DOM.div( {className:"question-grid-item"},  
                    QuestionPaginator( {metadata:{
                            currentPage: this.props.currentPage,
                            totalPages: this.props.totalPages}} )
                ) 
            ) 
            );
    }
});
