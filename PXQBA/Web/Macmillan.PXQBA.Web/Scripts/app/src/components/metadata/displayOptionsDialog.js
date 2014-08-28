/**
* @jsx React.DOM
*/

var DisplayOptionsDialog = React.createClass({

    getInitialState: function() {
        var displayOptions =  this.props.value;
        if(displayOptions==null) {
             displayOptions = {
                displayInBanks: false,
                displayInCurrentQuiz: false,
                displayInInstructorQuiz: false,
                displayInResources: false,
                filterable: false,
                matchInBanks: false,
                matchInResources: false,
                showFilterInBanks: false,
                showFilterInResources: false
             };
        }
        return {displayOptions: displayOptions}
        
    },

    editInternalFieldHandler: function() {
       this.props.updateHandler(this.props.itemIndex, "displayOptions",  this.state.displayOptions)
    },

    onChangeHandler: function(fieldName, value) {
        var displayOptions = this.state.displayOptions;
        displayOptions[fieldName] = value
        this.setState( {displayOptions: displayOptions} );
    },

    onClickTooltipHandler: function(imgName) {
        this.refs.modalDialog.refs.cancelButton.getDOMNode().click();
        this.props.showDisplayImageDialogHandler(window.content.img.displayOptionsHelp[imgName]);
    },
 

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Display options";
        };
      
        var renderBody = function(){
             return (<div>
                        <div>
                         <div> <b> Question Picker (Question in Question Banks)</b>
                                <span className="metadata-dispplay-options-help">
                                  <ToltipElement classNameProp="tooltip-img" 
                                                 tooltipText="Click for details" 
                                                 onClickHandler={self.onClickTooltipHandler.bind(null, "questionsInQuestionBanksUrl")}/>
                                </span>
                         </div>
                       
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

                         <div> <b> Question Picker (Question in Current Quiz)</b>
                                <span className="metadata-dispplay-options-help">
                                  <ToltipElement classNameProp="tooltip-img" 
                                                 tooltipText="Click for details" 
                                                 onClickHandler={self.onClickTooltipHandler.bind(null, "questionsInCurrentQuizUrl")}/>
                                </span>
                         </div>

                         <CheckBoxEditor value={self.state.displayOptions.displayInCurrentQuiz} 
                            label="Display this field when listing questions" 
                            onChangeHandler={self.onChangeHandler.bind(null, "displayInCurrentQuiz")}/>

                         <div> <b> Quiz Instructor View</b>
                                <span className="metadata-dispplay-options-help">
                                  <ToltipElement classNameProp="tooltip-img" 
                                                 tooltipText="Click for details" 
                                                 onClickHandler={self.onClickTooltipHandler.bind(null, "quizInstructorViewUrl")}/> 
                                </span>
                         </div>
                         
                         <CheckBoxEditor value={self.state.displayOptions.displayInInstructorQuiz} 
                            label="Display this field when listing questions" 
                            onChangeHandler={self.onChangeHandler.bind(null, "displayInInstructorQuiz")}/>

                         <div> <b> Recource Panel</b>
                                <span className="metadata-dispplay-options-help">
                                  <ToltipElement classNameProp="tooltip-img" 
                                                 tooltipText="Click for details"
                                                 onClickHandler={self.onClickTooltipHandler.bind(null, "resourcePanelUrl")}/> 
                                </span>
                         </div>

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
                                 <button ref="cancelButton" type="button" className="btn btn-default" data-dismiss="modal" data-target="displayOptionsDialog">Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={self.editInternalFieldHandler}>Save</button>
                         </div>
                    </div>
            );
        };

        return (<ModalDialog ref="modalDialog"
                             showOnCreate={true}
                             renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             closeDialogHandler = {this.props.closeDialogHandler}
                             dialogId="displayOptionsDialog"/>);
    }
});




