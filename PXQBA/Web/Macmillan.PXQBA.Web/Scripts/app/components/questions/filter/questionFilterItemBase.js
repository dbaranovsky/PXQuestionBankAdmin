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
                        React.DOM.span( {className:"filter-closer", onClick:this.closeFilterHandler, 'data-toggle':"tooltip", title:"Cancel"},  " Х " )
                     ),
                     React.DOM.div( {className:"filter-body"}, 
                         QuestionFilterMultiSelect( {allOptions:this.props.descriptor.allOptions,  
                                                    currentValues:this.props.descriptor.currentValues, 
                                                    onChangeHandler:this.filtrationChangeHandler})
                     )
            )
           
            );
        }
});