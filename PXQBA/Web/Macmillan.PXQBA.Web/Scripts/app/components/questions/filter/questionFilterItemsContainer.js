/**
* @jsx React.DOM
*/

var QuestionFilterItemsContainer = React.createClass({displayName: 'QuestionFilterItemsContainer',
    render: function() {
        return (
            React.DOM.div( {className:"questionFilterContainer"}, 
                 React.DOM.div(null,  
                     "Container ", QuestionFilterItemBase(null ), " ", QuestionFilterItemBase(null ), " ", QuestionFilterItemBase(null )
                )
            )
            );
        }
});