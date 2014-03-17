/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({displayName: 'QuestionListPage',

    render: function() {
        return (
            React.DOM.div( {className:"QuestionListPage"}, 
                React.DOM.div( {className:"add-question-action"}, 
                    React.DOM.a( {href:""},  " Add question")
                ),
                React.DOM.div(null, 
                  QuestionTabs(
                        {data:this.props.data,
                        currentPage:this.props.currentPage,
                        totalPages:this.props.totalPages}  
                   )
                )
            ) 
            );
    }
});

