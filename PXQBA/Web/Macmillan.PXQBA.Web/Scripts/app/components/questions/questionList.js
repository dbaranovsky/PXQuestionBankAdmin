/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({displayName: 'QuestionList',

		componentDidMount: function() {

        var test = function() {
          return (React.DOM.span(null,  " component"));
        }

     		var mouseIn =  function(event) {
     			//ToDo: implement  show menu actions here
          var tr = $(event.target).closest('tr');
     			tr.addClass('hover');
           
          //ToDo Extract to react.js component (menu)
       		tr.find('.actions').append('<div>'
                                      +'<button type="button" class="btn btn-default btn-sm"><span class="glyphicon glyphicon-pencil"></span></button>'
                                      +'<button type="button" class="btn btn-default btn-sm"><span class="glyphicon glyphicon-copyright-mark"></span></button>'
                                      +'<button type="button" class="btn btn-default btn-sm"><span class="glyphicon glyphicon-trash"></span></button>'
                                      +'</div>');
        }
        
        var mousOut = function (event) {
			        //ToDo: implement hide menu actions here    
               var tr = $(event.target).closest('tr');
               tr.removeClass('hover');
               tr.find('.actions').empty();
        }


			     //ToDo: not(:first) not working, need fix this !!
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
        			React.DOM.th( {style: {width:'5%'}},  " ", React.DOM.input( {type:"checkbox"})),
         			React.DOM.th( {style: {width:'10%'}},  " Chapter"),
        			React.DOM.th( {style: {width:'10%'}},  " Bank"),
        			React.DOM.th( {style: {width:'10%'}},  " Seq " ),
        			React.DOM.th( {style: {width:'40%'}},  " Title " ),
              React.DOM.th( {style: {width:'10%'}},  " Format " ),
              React.DOM.th( {style: {width:'15%'}},  " " )
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