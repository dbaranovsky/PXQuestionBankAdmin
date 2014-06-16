/**
* @jsx React.DOM
*/

var DisplayOptionsDialog = React.createClass({

    getInitialState: function() {
        return { displayOptions: this.props.value };
    },

    editInternalFieldHandler: function() {
       this.props.updateHandler(this.props.itemIndex, "displayOptions",  this.state.displayOptions)
    },

    onChangeHandler: function(fieldName, value) {
        var displayOptions = this.state.displayOptions;
        displayOptions[fieldName] = value
        this.setState( {displayOptions: displayOptions} );
    },

    onClickTooltipHandler: function() {
        
    },
 

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Display options";
        };
      
        var renderBody = function(){
             return (<div>
                        <div>
                         <div> <b> Question Picker (Question in Question Banks)</b> <ToltipElement tooltipText="Click for details" onClickHandler={self.onClickTooltipHandler}/></div>

                         <CheckBoxEditor value={self.state.displayOptions.displayInBanks} 
                            label="Display this field when listing questions" 
                            onChangeHandler={self.onChangeHandler.bind(null, "displayInBanks")}/>

                         <CheckBoxEditor value={self.state.displayOptions.showFilterInBanks} 
                            label="Show filter for this field"
                            onChangeHandler={self.onChangeHandler.bind(null, "showFilterInBanks")}/>

                         <CheckBoxEditor 
                            value={self.state.displayOptions.matchInBanks} 
                            label="Search result match this field" 
                            onChangeHandler={self.onChangeHandler.bind(null, "matchInBanks")}/>

                         <div> <b> Question Picker (Question in Current Quiz)</b> <ToltipElement tooltipText="Click for details"/> </div>

                         <CheckBoxEditor value={self.state.displayOptions.displayInCurrentQuiz} 
                            label="Display this field when listing questions" 
                            onChangeHandler={self.onChangeHandler.bind(null, "displayInCurrentQuiz")}/>

                         <div> <b> Quiz Instractor View</b> <ToltipElement tooltipText="Click for details"/> </div>
                         
                         <CheckBoxEditor value={self.state.displayOptions.displayInInstructorQuiz} 
                            label="Display this field when listing questions" 
                            onChangeHandler={self.onChangeHandler.bind(null, "displayInInstructorQuiz")}/>

                         <div> <b> Recourse Panel</b> <ToltipElement tooltipText="Click for details"/> </div>

                         <CheckBoxEditor value={self.state.displayOptions.displayInResources}
                             label="Display this field when listing questions" 
                             onChangeHandler={self.onChangeHandler.bind(null, "displayInResources")}/>

                         <CheckBoxEditor value={self.state.displayOptions.showFilterInResources} 
                            label="Show filter for this field" 
                            onChangeHandler={self.onChangeHandler.bind(null, "showFilterInResources")}/>

                         <CheckBoxEditor value={self.state.displayOptions.matchInResources} 
                            label="Search result match this field" 
                            onChangeHandler={self.onChangeHandler.bind(null, "matchInResources")}/>

                         
                        </div>
                         <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal" onClick={self.props.closeDialogHandler}>Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={self.editInternalFieldHandler}>Save</button>
                            </div>
                    </div>
            );
        };

        return (<ModalDialog showOnCreate={true}
                             renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             closeDialogHandler = {this.props.closeDialogHandler}
                             dialogId="displayOptionsDialog"/>);
    }
});




