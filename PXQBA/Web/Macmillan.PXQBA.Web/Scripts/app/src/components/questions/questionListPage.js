/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    render: function() {
       return (
            <div className="QuestionListPage">
                <div className="add-question-action">
                    <button className="btn btn-primary " data-toggle="modal" data-target="#addQuestionModal">
                    Add Question
                    </button>
                </div>

                
                <div>
                  <QuestionTabs
                        data={this.props.data}
                        currentPage={this.props.currentPage}
                        totalPages={this.props.totalPages} 
                        order={this.props.order} 
                        columns={this.props.columns}
                   />
                </div>
                <AddQuestionDialog />
            </div>
            );
    }
});

