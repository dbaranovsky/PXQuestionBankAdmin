/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarBase = React.createClass({


    getSelectedQuestionCount: function() {
        return this.props.parameters.selectedQuestions.length;
    },


    getCaption: function() {
        switch (this.props.bulkOperationBarType) {
          case window.enums.bulkOperationBarType.standart:
            return "Bulk action";
          case window.enums.bulkOperationBarType.importQuestions:
            return "Import";
          default:
            return "";
        }
    },

    getTextMessage: function() {
        var count = this.getSelectedQuestionCount();
        var selected = ""
        if(count==1) {
          selected = "1 question selected";
        }
        else {
          selected = " "+ count + " questions selected";
        }
        var caption = this.getCaption();

        return caption + " ( " + selected + " ):";
    },


    renderBar: function() {
       switch (this.props.bulkOperationBarType) {
          case window.enums.bulkOperationBarType.standart:
            return (<QuestionBulkOperationBarStandart 
                            message={this.getTextMessage()}
                            selectedQuestions={this.props.parameters.selectedQuestions}
                            deselectsAllHandler={this.props.parameters.deselectsAllHandler}
                            columns={this.props.parameters.columns}
                            bulkShareHandler = {this.props.parameters.bulkShareHandler}
                            isShared = {this.props.parameters.isShared}
                            capabilities= {this.props.parameters.capabilities} />);
          case window.enums.bulkOperationBarType.importQuestions:
            return (<QuestionBulkOperationBarImport 
                      message={this.getTextMessage()}
                      deselectsAllHandler={this.props.parameters.deselectsAllHandler}
                      selectedQuestions={this.props.parameters.selectedQuestions}
                   />);
          default:
            return "";
        }
    },

    render: function() {
        return ( 
                  <tr>
                    <td colSpan={this.props.colSpan} className="bulk-operation-bar">
                        {this.renderBar()}
                    </td>
                  </tr>
            );
        }
});