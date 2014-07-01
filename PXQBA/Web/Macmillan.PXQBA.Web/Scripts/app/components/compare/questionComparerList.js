/**
* @jsx React.DOM
*/

var QuestionComparerList = React.createClass({displayName: 'QuestionComparerList',

	renderHeader: function() {
		if(!this.props.compareEnabled) {
			return (	React.DOM.tr(null, 
               			 	React.DOM.th(null,  " ", React.DOM.span(null, "Please select a title for comparision"), " " ),
               			 	React.DOM.th(null,  " ", React.DOM.span(null, "Please select a title for comparision"), " " )
               	        ));
		}

		return  (React.DOM.tr(null, 
               	 	React.DOM.th(null,  " ", React.DOM.span(null, this.props.firstTitleCaption), " " ),
               		React.DOM.th(null,  " ", React.DOM.span(null, this.props.secondTitleCaption), " " )
               	));
	},


    renderQuestions: function() {
		var questionsHtml =[];
		var questions = this.props.questions;
		if((questions==null)||(!this.props.compareEnabled)) {
			return questionsHtml;
		}

		for(var i=0; i<questions.length; i++) {
			questionsHtml.push(ComparedQuesion( {data:questions[i]} ));
		}
    
    	return questionsHtml;
	},

    render: function() {  
      return (React.DOM.table( {className:"table table"}, 
      				this.renderHeader(),
      				this.renderQuestions()
      		  ));
    }
});



