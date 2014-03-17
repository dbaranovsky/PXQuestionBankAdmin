/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({displayName: 'QuestionFilter',
    render: function() {
        return (
            React.DOM.div( {className:"questionFilter"}, 
                 React.DOM.div(null,  
                    React.DOM.span(null, 
                         React.DOM.strong(null,  " Filter: " ) 
                    ),
                    React.DOM.a( {href:"#/filter/query/page/1"},  " Favourite filter")
                )
            )
            );
        }
});