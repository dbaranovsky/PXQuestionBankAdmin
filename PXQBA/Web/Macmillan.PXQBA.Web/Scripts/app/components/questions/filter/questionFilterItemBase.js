/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({displayName: 'QuestionFilterItemBase',
    render: function() {
        return (
            React.DOM.div( {className:"questionFilterItemBase"}, 
                 React.DOM.div(null,  
                     "Item"
                )
            )
            );
        }
});