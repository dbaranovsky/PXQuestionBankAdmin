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
        if(this.props.descriptor.canCloseOnFilter) {
            return (<span className="filter-closer" onClick={this.closeFilterHandler}  data-toggle="tooltip" title="Cancel"> Х </span>);
        }
        return null;
    },

    renderFilterBody: function() {
        switch(this.props.descriptor.filterType) {
            case window.enums.filterType.singleSelect:
                return ( <SingleSelectSelector allOptions={this.props.descriptor.allOptions}  
                                                    currentValues={this.props.descriptor.currentValues} 
                                                    onChangeHandler={this.filtrationChangeHandler}
                                                    dataPlaceholder="No Filtration"
                                                    />);
            case window.enums.filterType.multiSelectWithAddition:
                return ( <QuestionFilterItemMultiSelect allOptions={this.props.descriptor.allOptions}  
                                                    currentValues={this.props.descriptor.currentValues} 
                                                    onChangeHandler={this.filtrationChangeHandler}/>);
            case window.enums.filterType.text:
                return (<QuestionFilterItemText onChangeHandler={this.filtrationChangeHandler} 
                                                currentValues={this.props.descriptor.currentValues}/>);

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