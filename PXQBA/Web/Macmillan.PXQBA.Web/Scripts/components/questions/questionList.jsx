/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({
  	    render: function() {
        var questions = this.props.data.map(function (question) {
            return (<Question title={question.title}
            			     questionType={question.questionType} 
            			     eBookChapter={question.eBookChapter}
            			     questionBank={question.questionBank}
            			     questionSeq={question.questionSeq}
            			     />);
          });
        return (
          <div className="questionList">
            {questions}
          </div>
        );
      }
});