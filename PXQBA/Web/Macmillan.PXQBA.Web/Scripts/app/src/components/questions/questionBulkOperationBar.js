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

                          <span> {this.getSelectedQuestionCount()} questions selectd </span>
                          <span className="deselect-button" onClick={this.deselectsAllHandler}> X </span>
                    </td>
                  </tr>
            );
        }
});