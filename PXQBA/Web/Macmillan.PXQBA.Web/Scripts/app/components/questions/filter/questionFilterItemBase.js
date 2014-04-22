/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({displayName: 'QuestionFilterItemBase',

    filtrationChangeHandler: function(newValues) {
        routsManager.addFiltration(this.props.descriptor.field, newValues);
    },

    closeFilterHandler: function() {
        routsManager.deleteFiltration(this.props.descriptor.field);
    },

    render: function() {
        return (
            React.DOM.div( {className:"questionFilterItemBase"}, 
                     React.DOM.div( {className:"filter-header"},  
                        React.DOM.span(null,  " ", this.props.descriptor.caption, " " ), 
                        React.DOM.span( {className:"filter-closer", onClick:this.closeFilterHandler},  " Х " )
                     ),
                     React.DOM.div( {className:"filter-body"}, 
                         QuestionFilterMultiSelect( {allValues:this.props.descriptor.allValues,  
                                                    currentValues:this.props.descriptor.currentValues, 
                                                    onChangeHandler:this.filtrationChangeHandler})
                     )
            )
           
            );
        }
});