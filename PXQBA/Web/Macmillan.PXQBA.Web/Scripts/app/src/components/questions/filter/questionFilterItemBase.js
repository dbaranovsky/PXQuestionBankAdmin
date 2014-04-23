/**
* @jsx React.DOM
*/

var QuestionFilterItemBase = React.createClass({

    filtrationChangeHandler: function(newValues) {
        routsManager.addFiltration(this.props.descriptor.field, newValues);
    },

    closeFilterHandler: function() {
        routsManager.deleteFiltration(this.props.descriptor.field);
    },

    render: function() {
        return (
            <div className="questionFilterItemBase">
                     <div className="filter-header"> 
                        <span> {this.props.descriptor.caption} </span> 
                        <span className="filter-closer" onClick={this.closeFilterHandler}> Х </span>
                     </div>
                     <div className="filter-body">
                         <QuestionFilterMultiSelect allOptions={this.props.descriptor.allOptions}  
                                                    currentValues={this.props.descriptor.currentValues} 
                                                    onChangeHandler={this.filtrationChangeHandler}/>
                     </div>
            </div>
           
            );
        }
});