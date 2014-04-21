/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({displayName: 'QuestionFilterItemBase',

    filtrationChangeHandler: function(newValues) {
        routsManager.addFiltration(this.props.descriptor.field, newValues);
    },

    render: function() {
        return (
            React.DOM.div( {className:"questionFilterItemBase"}, 
                 React.DOM.div(null,  
                     React.DOM.span(null,  " ", this.props.descriptor.caption),
                     QuestionFilterMultiSelect( {allValues:this.props.descriptor.allValues,  currentValues:this.props.descriptor.currentValues, onChangeHandler:this.filtrationChangeHandler})
                )
            )
            );
        }
});