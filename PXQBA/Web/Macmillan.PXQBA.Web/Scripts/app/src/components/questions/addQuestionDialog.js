/**
* @jsx React.DOM
*/

var AddQuestionDialog = React.createClass({

    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };
        var renderBody = function(){
            return (<QuestionTypeList questionTypes={filterDataManager.getQuestionTypeList()}/>);
        };
        var renderFooterButtons = function(){
            return (<div>
                        <button type="button" className="btn btn-primary">Save changes</button>
                        <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                    </div>);
        };
        return (<ModalDialog renderHeaderText={renderHeaderText} renderBody={renderBody} renderFooterButtons={renderFooterButtons} dialogId="addQuestionModal"/>
                );
    }
});
