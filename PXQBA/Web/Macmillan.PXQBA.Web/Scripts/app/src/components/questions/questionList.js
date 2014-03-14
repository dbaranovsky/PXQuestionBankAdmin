/**
* @jsx React.DOM  
*/    

var QuestionList = React.createClass({

    componentDidMount: function() {

        var questionListContainer = $(this.getDOMNode());
        var menuContainer = questionListContainer.find('.question-menu-container');
 
       	var mouseInRowHandler =  function(event) {
              var tr = $(event.target).closest('tr');
         			tr.addClass('hover');
              tr.find('.actions').append(menuContainer.html());
        };
        
        var mousOutRowHandler = function (event) {
              var tr = $(event.target).closest('tr');
              tr.removeClass('hover');
              tr.find('.actions').empty();
        };

        //ToDo: need fix:
        // 1) not(:first) not working, need fix this. 
        // 2) if hover in button, mouseout execute.
        questionListContainer.find('.question-table').on('tr:not(:first)').mouseover(mouseInRowHandler);
        questionListContainer.find('.question-table').on('tr:not(:first)').mouseout(mousOutRowHandler);

        var toggleAllPreviews = function (event) {
              //ToDO: implement change of image
              var questionPreviews = $(event.target).closest('table').find('.question-preview');
              var chevronIcon =  $(event.target).closest('th').find('.glyphicon');
              $(chevronIcon).toggleClass('glyphicon-chevron-right').toggleClass('glyphicon-chevron-down');
              if($(chevronIcon).hasClass('glyphicon-chevron-down')) {
                  $.each(questionPreviews, function(index, value) {
                  expandPreview($(value).closest('td'));
                  });
              }
              else {
                  $.each(questionPreviews, function(index, value) {
                  collapsePreview($(value).closest('td'));
                  });
              }
        };

        var toggleInlineHandler = function (event) {
            toggleInline(event.target);
        };

        var toggleInline = function (obj) {
          if ($(obj).closest('td').find('.glyphicon').hasClass('glyphicon-chevron-right')) {
              expandPreview($(obj).closest('td'));
          }
          else {
              collapsePreview($(obj).closest('td'));
          }
        };

        var expandPreview = function (obj) {
            $(obj).find('.glyphicon').removeClass('glyphicon-chevron-right').addClass('glyphicon-chevron-down');
            $(obj).find('.question-preview').removeClass('preview-collapsed');
        };

        var collapsePreview = function (obj) {
            $(obj).find('.glyphicon').addClass('glyphicon-chevron-right').removeClass('glyphicon-chevron-down');
            $(obj).find('.question-preview').addClass('preview-collapsed');
        };

        questionListContainer.find('.question-table').on('click', '.title-header', toggleAllPreviews);
        questionListContainer.find('.question-table').on('click', '.title', toggleInlineHandler);
		    
    },

    render: function() {
        var questions = this.props.data.map(function (question) {
            return (<Question metadata={question} />);
          });

        return (
          <div className="questionList">
              <div className="question-menu-container" style={{display:'none'}}>
                  <QuestionListMenu />
              </div>

           		<table className="table table question-table">
           		   <thead>
           		    <QuestionListHeader />
           		  </thead>
           		  <tbody> 
          		  {questions}
          		  </tbody> 
           		</table>
          </div>
        );
    }
});