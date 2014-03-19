/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({displayName: 'QuestionListPage',

    render: function() {
       return (
            React.DOM.div( {className:"QuestionListPage"}, 
                React.DOM.div( {className:"add-question-action"}, 
                    React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", 'data-target':"#addQuestionModal"}, 
                    "Add Question"
                    )
                ),

                
                React.DOM.div(null, 
                  QuestionTabs(
                        {data:this.props.data,
                        currentPage:this.props.currentPage,
                        totalPages:this.props.totalPages, 
                        order:this.props.order} 
                   )
                ),
                AddQuestionDialog(null )
            )
            );
    }
});

