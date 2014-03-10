/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({

		componentDidMount: function() {

     		var mouseIn =  function(event) {
     			//ToDo: implement  show menu actions here
     			$(event.target).closest('tr').addClass('hover');
       		   }
        
            var mousOut = function (event) {
			    //ToDo: implement hide menu actions here
            	$(event.target).closest('tr').removeClass('hover');
        		}


			     //ToDo: not(:first) not working, need fix this
            $('#question-table').on('tr:not(:first)').mouseover(mouseIn);
			$('#question-table').on('tr:not(:first)').mouseout(mousOut);
		},


  	    render: function() {
        var questions = this.props.data.map(function (question) {
            return (<Question title={question.title}
            			     questionType={question.questionType} 
            			     eBookChapter={question.eBookChapter}
            			     questionBank={question.questionBank}
            			     questionSeq={question.questionSeq}
            			     />);
          });


        var header = function() {
        	return(
        		<tr>
        			<th> <input type="checkbox"/></th>
        			<th> Title</th>
        			<th> Chapter</th>
        			<th> Bank</th>
        			<th> Seq </th>
        			<th> Title </th>
        		</tr>
        	);}
 
        return (
          <div className="questionList">
           		<table className="table table" id="question-table">
           		   <thead>
           		  {header()}
           		  </thead>
           		  <tbody> 
          		  {questions}
          		  </tbody> 
           		</table>
          </div>
        );
      }
});