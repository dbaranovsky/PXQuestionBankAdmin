/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBar = React.createClass({

    deselectsAllHandler: function() {
        this.props.deselectsAllHandler();
    },

    getSelectedQuestionCount: function() {
        return this.props.selectedQuestions.length;
    },

    render: function() {
        return ( 
                  <tr>
                    <td colSpan={this.props.colSpan} className="bulk-operation-bar">
                          <div className="bulk-operation-item">
                               <span> {this.getSelectedQuestionCount()} questions selected</span>
                          </div>
                          <div className="bulk-operation-item">
                              
                          </div>
                          <div className="deselect-button" onClick={this.deselectsAllHandler}>
                                <span > X </span>
                          </div>
                    </td>
                  </tr>
            );
        }
});