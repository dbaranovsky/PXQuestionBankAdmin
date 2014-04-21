/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({

    filtrationChangeHandler: function(newValues) {
        routsManager.addFiltration(this.props.descriptor.field, newValues);
    },

    render: function() {
        return (
            <div className="questionFilterItemBase">
                 <div> 
                     <span> {this.props.descriptor.caption}</span>
                     <QuestionFilterMultiSelect allValues={this.props.descriptor.allValues}  currentValues={this.props.descriptor.currentValues} onChangeHandler={this.filtrationChangeHandler}/>
                </div>
            </div>
            );
        }
});