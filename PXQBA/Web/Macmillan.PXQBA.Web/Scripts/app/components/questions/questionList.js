/**
* @jsx React.DOM  
*/    

var QuestionList = React.createClass({displayName: 'QuestionList',

    specialColumnsCount : 2,

    getAllColumnCount: function() {
        return this.specialColumnsCount + this.props.columns.length;
    },

    getInitialState: function() {

        return { selectedQuestions: [],
                 selectedAll: false,
                 expandedQuestions: [],
                 expandedAll: false
               };
    },

    componentWillReceiveProps: function(nextProps) {
      if(this.isShouldResetSelected(nextProps)) {
         this.resetSelection();
      }
    }, 

    isShouldResetSelected: function(nextProps) {
      var shouldResetSelected = false;

      if(this.props.currentPage!=nextProps.currentPage) {
          shouldResetSelected = true;
      }

      if(this.props.order.orderType!=nextProps.order.orderType) {
        shouldResetSelected = true;
      }
 
      if((this.props.order.orderField!=nextProps.order.orderField)
          &&(this.props.order.orderType!=window.enums.orderType.none)) {
        shouldResetSelected = true;
      }

      return shouldResetSelected;
    },

    resetSelection: function() {
       this.setState({ selectedQuestions: [], selectedAll:false });
    },

    componentDidMount: function() {
    expandAllQuestionHandler: function(isExpanded) {
        for(var i=0; i<this.props.data.length; i++) {
          this.expandPreviewQuestionHandler(this.props.data[i].data.id, isExpanded)
        }
        this.setState({expandedAll:isExpanded})
    },

    expandPreviewQuestionHandler: function(questionId, isExpanded) {
        var expandedQuestions = this.state.expandedQuestions;
        var index = $.inArray(questionId, expandedQuestions);
        if(isExpanded) {
          if (index == -1) {
              expandedQuestions.push(questionId)
          }
        } 
        else {
           if (index != -1) {
              expandedQuestions.splice(index, 1);
           }
        }

        this.setState({expandedQuestions: expandedQuestions});
    },

    isQuestionExpanded: function(questionId) {
         var expandedQuestions = this.state.expandedQuestions;
         var index = $.inArray(questionId, expandedQuestions);
         if(index==-1) {
            return false;
         }
         return true;
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
        this.resetSelection();
    },

    renderQuestion: function() {
       var self = this;
       var questions = this.props.data.map(function (question) {

            var isQuestionExpanded = self.isQuestionExpanded(question.data.id);

            var questionHtml = (Question( {metadata:question,
                       columns:self.props.columns, 
                       menuHandlers:self.props.handlers,
                       selectQuestionHandler:self.selectQuestionHandler,
                       selected:self.isQuestionSelected(question.data.id),
                       expandPreviewQuestionHandler:  self.expandPreviewQuestionHandler,
                       expanded:isQuestionExpanded}

                       ));

            var preview = null;

            if(isQuestionExpanded) {
              preview = (QuestionPreview( {colSpan:self.getAllColumnCount(), preview:question.data.questionHtmlInlinePreview} ));
            }

            return [questionHtml, preview];
          });

       if(questions.length==0) {
           questions.push(QuestionNoDataStub( {colSpan:this.getAllColumnCount()} ));
        } 

        return questions;
    },

   

    renderBulkOperationBar: function() {
      if(this.state.selectedQuestions.length>1) {
        return (QuestionBulkOperationBar( {colSpan:this.getAllColumnCount(), 
                                          selectedQuestions:this.state.selectedQuestions,
                                          deselectsAllHandler:this.deselectsAllQuestionHandler}));
      }
      return null;
    },

    render: function() {
        return (
          React.DOM.div( {className:"questionList"}, 
                React.DOM.table( {className:"table table question-table"}, 
                   React.DOM.thead(null, 
                    QuestionListHeader( {ordering:this.props.order, 
                                        columns:this.props.columns, 
                                        allAvailableColumns:this.props.allAvailableColumns, 
                                        selectAllQuestionHandelr:this.selectAllQuestionHandelr,
                                        selectedAll:this.state.selectedAll,
                                        expandAllQuestionHandler:this.expandAllQuestionHandler,
                                        expandedAll:this.state.expandedAll}
                                        )
                  ),
                  React.DOM.tbody(null,  
                    this.renderBulkOperationBar(),
                    this.renderQuestion()
                  ) 
                ),
              React.DOM.div( {className:"dialogs-container"}, 
                  React.DOM.div( {className:"notifications top-center center"} )
              )
          )
        );
    }
});