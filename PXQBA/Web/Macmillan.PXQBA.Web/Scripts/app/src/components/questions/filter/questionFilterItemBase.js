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

    renderCloseButton: function() {
        if(this.props.descriptor.filterType==window.enums.filterType.singleSelect) {
            return null;
        }
        return <span className="filter-closer" onClick={this.closeFilterHandler}  data-toggle="tooltip" title="Cancel"> Х </span>
    },

    renderFilterBody: function() {
        switch(this.props.descriptor.filterType) {
            case window.enums.filterType.singleSelect:
                return ( <QuestionFilterSingleSelect allOptions={this.props.descriptor.allOptions}  
                                                    currentValues={this.props.descriptor.currentValues} 
                                                    onChangeHandler={this.filtrationChangeHandler}/>)
            case window.enums.filterType.multiSelectWithAddition:
                return ( <QuestionFilterMultiSelect allOptions={this.props.descriptor.allOptions}  
                                                    currentValues={this.props.descriptor.currentValues} 
                                                    onChangeHandler={this.filtrationChangeHandler}/>);
            default: 
                return null;
        }
    },

    render: function() {
        return (
            <div className="questionFilterItemBase">
                     <div className="filter-header"> 
                        <span> {this.props.descriptor.caption} </span> 
                         {this.renderCloseButton()}
                     </div>
                     <div className="filter-body">
                        {this.renderFilterBody()}
                     </div>
            </div>
           
            );
        }
});