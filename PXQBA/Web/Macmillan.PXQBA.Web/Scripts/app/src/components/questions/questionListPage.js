/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    getInitialState: function() {
      return { loading: false };
    },

    renderLoader: function() {
        if((this.state.loading)) {
            return (<Loader />);
        }
        return null;
    },

    render: function() {
       return (
            <div className="QuestionListPage">
             {this.renderLoader()}
                <div className="add-question-action">
                    <button className="btn btn-primary " data-toggle="modal" data-target="#addQuestionModal">
                    Add Question
                    </button>
                </div>
                <div>
                  <QuestionTabs response={this.props.response} />
                </div>
                <AddQuestionDialog />
            </div>
            );
    }
});

