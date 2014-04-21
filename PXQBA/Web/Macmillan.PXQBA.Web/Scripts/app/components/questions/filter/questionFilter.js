/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({displayName: 'QuestionFilter',

    render: function() {
        return (
            React.DOM.div( {className:"questionFilter"}, 
                 React.DOM.div(null,  
                    React.DOM.span(null, 
                         React.DOM.strong(null,  " Filter: " ),  "  ",  QuestionFilterItemsAppender( {filteredFields:this.props.filter,  allFields:this.props.allAvailableColumns} ) 
                    ),
               
                    QuestionFilterItemsContainer( {filter:this.props.filter, allAvailableColumns:this.props.allAvailableColumns})
                )
            )
            );
        }
});


 