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
                    <QuestionList data={this.props.data} order={this.props.order} columns={this.props.columns}/>
                </div> 
                <div className="question-grid-item"> 
                    <QuestionPaginator metadata={{
                            currentPage: this.props.currentPage,
                            totalPages: this.props.totalPages}} />
                </div> 
            </div> 
            );
    }
});
