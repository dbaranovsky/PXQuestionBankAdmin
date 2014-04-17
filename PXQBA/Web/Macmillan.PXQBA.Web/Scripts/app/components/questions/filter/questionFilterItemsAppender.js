/**
* @jsx React.DOM
*/

var QuestionFilterItemsAppender = React.createClass({displayName: 'QuestionFilterItemsAppender',
    render: function() {
        return (
            React.DOM.div( {className:"questionFilterItemsAppender"}, 
                 React.DOM.div(null,  
                     "+"
                )
            )
            );
        }
});