/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({displayName: 'QuestionList',

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
            return (Question( {title:question.title,
            			     questionType:question.questionType, 
            			     eBookChapter:question.eBookChapter,
            			     questionBank:question.questionBank,
            			     questionSeq:question.questionSeq}
            			     ));
          });


        var header = function() {
        	return(
        		React.DOM.tr(null, 
        			React.DOM.th(null,  " ", React.DOM.input( {type:"checkbox"})),
        			React.DOM.th(null,  " Title"),
        			React.DOM.th(null,  " Chapter"),
        			React.DOM.th(null,  " Bank"),
        			React.DOM.th(null,  " Seq " ),
        			React.DOM.th(null,  " Title " )
        		)
        	);}
 
        return (
          React.DOM.div( {className:"questionList"}, 
           		React.DOM.table( {className:"table table", id:"question-table"}, 
           		   React.DOM.thead(null, 
           		  header()
           		  ),
           		  React.DOM.tbody(null,  
          		  questions
          		  ) 
           		)
          )
        );
      }
});