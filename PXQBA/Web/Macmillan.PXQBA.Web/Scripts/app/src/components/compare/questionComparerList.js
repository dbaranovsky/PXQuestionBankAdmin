/**
* @jsx React.DOM
*/

var QuestionComparerList = React.createClass({

	renderHeader: function() {
		if(!this.props.compareEnabled) {
			return (	<tr>
               			 	<th> <span>Please select a title for comparision</span> </th>
               			 	<th> <span>Please select a title for comparision</span> </th>
               	        </tr>);
		}

		return  (<tr>
               	 	<th> <span>{this.props.firstTitleCaption}</span> </th>
               		<th> <span>{this.props.secondTitleCaption}</span> </th>
               	</tr>);
	},


    renderQuestions: function() {
		var questionsHtml =[];
		var questions = this.props.questions;
		if((questions==null)||(!this.props.compareEnabled)) {
			return questionsHtml;
		}

		for(var i=0; i<questions.length; i++) {
			questionsHtml.push(<ComparedQuesion data={questions[i]} />);
		}
    
    	return questionsHtml;
	},

    render: function() {  
      return (<table className="table table">
      				{this.renderHeader()}
      				{this.renderQuestions()}
      		  </table>);
    }
});



