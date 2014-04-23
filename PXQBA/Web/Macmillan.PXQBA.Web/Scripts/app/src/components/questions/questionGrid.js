/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({
  
    render: function() { 
        return (
              <div className="questionGrid">
                <div className="question-grid-item"> 
                     <QuestionFilter filter={this.props.response.filter} allAvailableColumns={this.props.response.allAvailableColumns}/>
                </div>
                <div className="question-grid-item"> 
                    <QuestionList data={this.props.response.questionList} 
                                        order={this.props.response.order} 
                                        columns={this.props.response.columns}
                                        questionCardTemplate = {this.props.response.questionCardLayout}
                                        allAvailableColumns={this.props.response.allAvailableColumns}
                                        handlers={this.props.handlers}
                                        currentPage={this.props.response.pageNumber}
                                        />
                </div> 
                <div className="question-grid-item"> 
                    <QuestionPaginator metadata={{
                            currentPage: this.props.response.pageNumber,
                            totalPages: this.props.response.totalPages}} />
                </div> 
                 
            </div> 

            );
    }
});
