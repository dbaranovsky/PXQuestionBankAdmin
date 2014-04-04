/**
* @jsx React.DOM
*/

var AddQuestionDialog = React.createClass({

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.props.nextStepHandler;
        var renderBody = function(){

            return (<AddQuestionBox nextStepHandler={nextStepHandler}/>);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} renderBody={renderBody} renderFooterButtons={renderFooterButtons} dialogId="addQuestionModal"/>
                );
    }
});

var AddQuestionBox = React.createClass({

    render: function() {

            return (<div>
                        <QuestionTypeList questionTypes={filterDataManager.getQuestionTypeList()}/>
                        
                        
                            <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-primary" data-dismiss="modal"  onClick={this.props.nextStepHandler}>Next</button>
                                 <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                            </div>
                    

                   </div>
               );
        
      
       
    }
});