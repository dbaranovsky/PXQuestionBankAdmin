/**
* @jsx React.DOM
*/

var QuestionGrid = React.createClass({
  
    render: function() { 
        return (
            <div className="questionGrid">
                <div className="question-grid-item"> 
                     <QuestionFilter/>
                </div>
                <div className="question-grid-item"> 
                    <QuestionList data={this.props.response.questionList} 
                                        order={this.props.response.order} 
                                        columns={this.props.response.columns}
                                        allAvailableColumns={this.props.response.allAvailableColumns}
                                        handlers={this.props.handlers}
                                        />
                </div> 
                <div className="question-grid-item"> 
                    <QuestionPaginator metadata={{
                            currentPage: this.props.response.currentPage,
                            totalPages: this.props.response.totalPages}} />
                </div> 
                 <div className='notifications bottom-right'></div>
            </div> 

            );
    }
});
