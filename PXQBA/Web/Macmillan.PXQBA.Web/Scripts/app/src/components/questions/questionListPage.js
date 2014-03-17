/**
* @jsx React.DOM
*/

var QuestionListPage = React.createClass({

    render: function() {
        return (
            <div className="QuestionListPage">
                <div className="add-question-action">
                    <a href=""> Add question</a>
                </div>
                <div>
                  <QuestionTabs
                        data={this.props.data}
                        currentPage={this.props.currentPage}
                        totalPages={this.props.totalPages}  
                   />
                </div>
            </div> 
            );
    }
});

