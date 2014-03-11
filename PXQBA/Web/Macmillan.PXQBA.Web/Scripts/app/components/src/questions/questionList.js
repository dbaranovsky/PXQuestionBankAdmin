/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({

		componentDidMount: function() {

        var test = function() {
          return (<span> component</span>);
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
        			<th style={ {width:'5%'}}> <input type="checkbox"/></th>
         			<th style={ {width:'10%'}}> Chapter</th>
        			<th style={ {width:'10%'}}> Bank</th>
        			<th style={ {width:'10%'}}> Seq </th>
        			<th style={ {width:'40%'}}> Title </th>
              <th style={ {width:'10%'}}> Format </th>
              <th style={ {width:'15%'}}> </th>
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