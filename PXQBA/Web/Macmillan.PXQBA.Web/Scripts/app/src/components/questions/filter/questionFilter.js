/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({

    render: function() {
        return (
            <div className="questionFilter">
                 <div> 
                    <span>
                         <strong> Filter: </strong>  <QuestionFilterItemsAppender filteredFields={this.props.filter}  allFields={this.props.allAvailableColumns} /> 
                    </span>
               
                    <QuestionFilterItemsContainer filter={this.props.filter} allAvailableColumns={this.props.allAvailableColumns}/>
                </div>
            </div>
            );
        }
});


 