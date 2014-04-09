/**
* @jsx React.DOM
*/

var QuestionTypeDialog = React.createClass({

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
        var closeDialogHandler = this.props.closeDialogHandler;
         $(this.getDOMNode()).on('hidden.bs.modal', function () {
           closeDialogHandler();
        })
    },

    nextStepHandler: function(questionType){
      $(this.getDOMNode()).off('hidden.bs.modal');
       this.props.nextStepHandler(questionType);
    },

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.nextStepHandler;
        var questionTypes = this.props.questionTypes;
        var renderBody = function(){

            return (<div>

                    <ul className="nav nav-tabs">
                      <li className="active"> 
                         <a href="#newQuestion" data-toggle="tab">New question</a>
                      </li>
                                                
                    </ul>   
                  
                      <div className="tab-pane active" id="newQuestion">
                         <div className="tab-body">       
                            <AddQuestionBox nextStepHandler={nextStepHandler} questionTypes={questionTypes}/>
                          </div>
                      </div>
                    
                

                </div>

);
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} renderBody={renderBody} renderFooterButtons={renderFooterButtons} dialogId="addQuestionModal"/>
                );
    }
});

var AddQuestionBox = React.createClass({


    getInitialState: function() {
      return { 
               questionType: 0
             };
    },
   
   nextStepHandler: function(){
      this.props.nextStepHandler(this.state.questionType);
   },

   changeQuestionType: function(questionType){
    this.setState({questionType: questionType});
   },
    render: function() {
            return (<div>
                    <QuestionTypeList questionTypes={this.props.questionTypes} changeQuestionType={this.changeQuestionType} changeHandler={this.changeQuestionType}/>

                            <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-primary" data-dismiss="modal"  onClick={this.nextStepHandler}>Next</button>
                                 <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                            </div>
                   </div>
               );
    }
});