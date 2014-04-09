/**
* @jsx React.DOM  
*/    

var QuestionList = React.createClass({

    specialColumnsCount : 2,

    getAllColumnCount: function() {
        return this.specialColumnsCount + this.props.columns.length;
    },

    getInitialState: function() {
        return { showEditDialog: false, 
                 questionIdForNotes: 0,
                 selectedQuestions: [],
                 selectedAll: false
               };
    },

    componentDidMount: function() {

        var questionListContainer = $(this.getDOMNode());

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

        questionListContainer.find('.question-table').on('click', '.titles-expander', toggleAllPreviews);
        questionListContainer.find('.question-table').on('click', '.title', toggleInlineHandler);
            
    },

    renderNotes: function(qId) {
       this.setState({ 
                showEditDialog: true,
                questionIdForNotes: qId});
    },

    selectQuestionHandler: function(questionId, isSelected) {
        var selectedQuestions = this.state.selectedQuestions;
        var index = $.inArray(questionId, selectedQuestions);
        if(isSelected) {
          if (index == -1) {
              selectedQuestions.push(questionId)
          }
        } 
        else {
           if (index != -1) {
              selectedQuestions.splice(index, 1);
           }
        }

        this.setState({selectedQuestions: selectedQuestions});
    },

    isQuestionSelected: function(questionId) {
         var selectedQuestions = this.state.selectedQuestions;
         var index = $.inArray(questionId, selectedQuestions);
         if(index==-1) {
            return false;
         }
         return true;
    },

    selectAllQuestionHandelr: function(isSelected) {
        for(var i=0; i<this.props.data.length; i++) {
          this.selectQuestionHandler(this.props.data[i].data.id, isSelected)
        }
        this.setState({selectedAll: isSelected});
    },

    deselectsAllQuestionHandler: function() {
        this.setState({selectedQuestions: [], selectedAll:false});
    },

    renderQuestion: function() {
       var self = this;
       var questions = this.props.data.map(function (question) {
            return (<Question metadata={question}
                       columns={self.props.columns} 
                       renderNotes={self.renderNotes} 
                       copyQuestionHandler={self.props.handlers.copyQuestionHandler} 
                       selectQuestionHandler={self.selectQuestionHandler}
                       selected={self.isQuestionSelected(question.data.id)}/>);
          });

       if(questions.length==0) {
           questions.push(<QuestionNoDataStub colSpan={this.getAllColumnCount()} />);
        } 

        return questions;
    },

   
    closeNoteDialogHandler: function(){
       this.setState({ 
                showEditDialog: false,
                questionIdForNotes: 0});
    },

    renderNotesDialog: function()
    {
      if(this.state.showEditDialog) {
        return (<EditQuestionNotesDialog closeDialogHandler={this.closeNoteDialogHandler} questionId={this.state.questionIdForNotes}/>);
      }
      return null;
    },

    renderBulkOperationBar: function() {
      if(this.state.selectedQuestions.length>1) {
        return (<QuestionBulkOperationBar colSpan={this.getAllColumnCount()} 
                                          selectedQuestions={this.state.selectedQuestions}
                                          deselectsAllHandler={this.deselectsAllQuestionHandler}/>);
      }
      return null;
    },

    render: function() {
        return (
          <div className="questionList">
                <table className="table table question-table">
                   <thead>
                    <QuestionListHeader ordering={this.props.order} 
                                        columns={this.props.columns} 
                                        allAvailableColumns={this.props.allAvailableColumns} 
                                        selectAllQuestionHandelr={this.selectAllQuestionHandelr}
                                        selectedAll={this.state.selectedAll}/>
                  </thead>
                  <tbody> 
                    {this.renderBulkOperationBar()}
                    {this.renderQuestion()}
                  </tbody> 
                </table>
              <div className="dialogs-container">
                {this.renderNotesDialog()}
                  <div className='notifications top-center center' />
              </div>
          </div>
        );
    }
});