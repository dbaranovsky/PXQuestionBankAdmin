/**
* @jsx React.DOM
*/

var QuestionList = React.createClass({

		componentDidMount: function() {

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

        var toggleAllPreviews = function (event){
       //ToDO: implement change of image
        var questionPreviews = $(event.target).closest('table').find('.question-preview');
        var chevronIcon =  $(event.target).closest('th').find('.glyphicon');
        $(chevronIcon).toggleClass('glyphicon-chevron-right').toggleClass('glyphicon-chevron-down');
        if($(chevronIcon).hasClass('glyphicon-chevron-down')){
            $.each(questionPreviews, function(index, value){
          expandPreview($(value).closest('td'));
        });

        }
        else{
           $.each(questionPreviews, function(index, value){
          collapsePreview($(value).closest('td'));
        });

       
        }
      

        }

        var toggleInlineHandler = function (event)
        {
          toggleInline(event.target);
        }

        var toggleInline = function (obj){
         if ($(obj).closest('td').find('.glyphicon').hasClass('glyphicon-chevron-right'))
         {
          expandPreview($(obj).closest('td'));
         }
         else
         {
          collapsePreview($(obj).closest('td'));
         }
        }

        var expandPreview = function (obj)
        {
           $(obj).find('.glyphicon').removeClass('glyphicon-chevron-right').addClass('glyphicon-chevron-down');
           $(obj).find('.question-preview').removeClass('preview-collapsed');

        }

         var collapsePreview = function (obj)
        {
           $(obj).find('.glyphicon').addClass('glyphicon-chevron-right').removeClass('glyphicon-chevron-down');
           $(obj).find('.question-preview').addClass('preview-collapsed');

        }


			     //ToDo: not(:first) not working, need fix this !!
            $('#question-table tr').hover(mouseIn,mousOut);
			      //$('#question-table').on('tr:not(:first)').mouseout(mousOut);
            $('#question-table').on('click', '.title-header', toggleAllPreviews);
            $('#question-table').on('click', '.title', toggleInlineHandler);
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
        			<th style={ {width:'40%'}} className="title-header">
                 <span className="glyphicon glyphicon-chevron-right"></span> Title
              </th>
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