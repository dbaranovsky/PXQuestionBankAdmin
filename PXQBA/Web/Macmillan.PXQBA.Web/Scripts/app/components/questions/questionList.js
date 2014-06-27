/**
* @jsx React.DOM  
*/    

var QuestionList = React.createClass({displayName: 'QuestionList',

    specialColumnsCount : 3,

    /* Lifecycle Methods */

    getInitialState: function() {
        return { selectedQuestions: [],
                 selectedAll: false,
                 expandedQuestions: [],
                 expandedAll: false
               };
    },

   componentWillReceiveProps: function(nextProps) {
       if(this.isShouldResetState(nextProps)) {
          this.resetSelection();
          this.resetExpanded();
       }
    }, 

    /* Common Helpers */

    getAllColumnCount: function() {
        return this.specialColumnsCount + this.props.columns.length;
    },

    changeCollection: function(item, collection, isInsert) {
        var index = $.inArray(item, collection);
        if(isInsert) {
          if (index == -1) {
              collection.push(item)
          }
        } 
        else {
           if (index != -1) {
              collection.splice(index, 1);
           }
        }
        return collection;
    },

     changeSelectedCollection: function(itemId, collection, isInsert, isShared) {
        var index = $.inArray(itemId, $.map(collection, function(e){ return e.id}));
        if(isInsert) {
          if (index == -1) {
             var item = {id: itemId, isShared: isShared};
              collection.push(item);
          }
        } 
        else {
           if (index != -1) {
              collection.splice(index, 1);
           }
        }
        return collection;
    },

    isItemInCollection: function(item, collection) {
       var index = $.inArray(item, collection);
         if(index==-1) {
            return false;
         }
         return true;
    },

    /*  Handlers */

    expandAllQuestionHandler: function(isExpanded) {
        for(var i=0; i<this.props.data.length; i++) {
          this.expandPreviewQuestionHandler(this.props.data[i].data.id, isExpanded)
        }
        this.setState({expandedAll:isExpanded})
    },

    selectAllQuestionHandelr: function(isSelected) {
        for(var i=0; i<this.props.data.length; i++) {
          var question = this.props.data[i];
          this.selectQuestionHandler(question.data.id, isSelected, question.data.sharedWith !== "")
        }
        this.setState({selectedAll: isSelected});
    },

    expandPreviewQuestionHandler: function(questionId, isExpanded) {
      this.setState({expandedQuestions: this.changeCollection(
                                  questionId,
                                  this.state.expandedQuestions, 
                                  isExpanded)});
    },

    selectQuestionHandler: function(questionId, isSelected, isShared) {
        this.setState({selectedQuestions: this.changeSelectedCollection(
                                  questionId,
                                  this.state.selectedQuestions, 
                                  isSelected, 
                                  isShared)});
    },

    deselectsAllQuestionHandler: function() {
        this.resetSelection();
    },

    /* Helpers */

    isQuestionExpanded: function(questionId) {
         return this.isItemInCollection(questionId, this.state.expandedQuestions);
    },

    isQuestionSelected: function(questionId) {
         var index = $.inArray(questionId, $.map(this.state.selectedQuestions, function(e){ return e.id;}));
         if(index==-1) {
            return false;
         }
         return true;
    },

    isShouldResetState: function(nextProps) {
       var shouldResetState = false;
 
       if(this.props.currentPage!=nextProps.currentPage) {
           shouldResetState = true;
       }
 
       if(this.props.order.orderType!=nextProps.order.orderType) {
         shouldResetState = true;
       }
  
       if((this.props.order.orderField!=nextProps.order.orderField)
           &&(this.props.order.orderType!=window.enums.orderType.none)) {
         shouldResetState = true;
       }
 

       if(this.props.filter.length!=nextProps.filter.length) {
          shouldResetState = true;
       }
       else {
          for(var i=0; i<this.props.filter.length; i++) {
            if(this.props.filter[i].field!=nextProps.filter[i].field) {
              shouldResetState = true;
            }
            if(this.props.filter[i].values.length!=nextProps.filter[i].values.length) {
              shouldResetState = true;
            }
            else {
              for(var j=0; j<this.props.filter.length; j++) {
                if(this.props.filter[i].values[j]!=nextProps.filter[i].values[j])  {
                  shouldResetState = true;
                }
              }
            }
          }
       }
  
       return shouldResetState;
     },
 
    resetSelection: function() {
       this.setState({ selectedQuestions: [], selectedAll: false });
    },

    resetExpanded: function() {
       this.setState({ expandedQuestions: [], expandedAll: false });
    },
 
    /* Renders */

    renderQuestions: function() {
       var self = this;

       var questionsForRender = [];

       for(var i=0; i<this.props.data.length; i++) {

          var currentQuestion = this.props.data[i];
          var nextQuestion = this.props.data[i+1];
          var isDraftCurrentQuestion = this.isDraftQuestion(currentQuestion)
          
          var isGrouped = isDraftCurrentQuestion;
          var isSeparatorNeed = false;

          if((nextQuestion!=null)) {
            var isDraftNextQuestion = this.isDraftQuestion(nextQuestion);
            if(isDraftCurrentQuestion) {
              if(!isDraftNextQuestion) {
                 isSeparatorNeed = true;
              }
            }
            else {
              if(isDraftNextQuestion) {
                isGrouped = true;
              }
            }
         }

          questionsForRender.push(this.renderQuestion(currentQuestion, isDraftCurrentQuestion, isGrouped))
          
          if(isSeparatorNeed) {
            questionsForRender.push(QuestionListGroupSeparator( {colSpan:this.getAllColumnCount()} ));
          }
       }

       
       if(questionsForRender.length==0) {
           questionsForRender.push(QuestionNoDataStub( {colSpan:this.getAllColumnCount()} ));
        } 

        return questionsForRender;
    },
     
    isDraftQuestion: function(question) {
      var flag = question.data[window.consts.questionDraftFlagName];

      if(flag!="") {
          return true;
      }

      return false;
    },

    renderQuestion: function(question, isDraft, isGrouped) {
       
      var isQuestionExpanded = this.isQuestionExpanded(question.data.id);

      var questionHtml = (Question( {metadata:question,
                       columns:this.props.columns, 
                       menuHandlers:this.props.handlers,
                       selectQuestionHandler:this.selectQuestionHandler,
                       selected:this.isQuestionSelected(question.data.id),
                       expandPreviewQuestionHandler:  this.expandPreviewQuestionHandler,
                       expanded:isQuestionExpanded,
                       grouped:isGrouped,
                       draft:isDraft,
                       capabilities:  this.props.capabilities}
                      ));

      var preview = null;
      if(isQuestionExpanded) {
          preview = (QuestionPreview( 
                      {colSpan:this.getAllColumnCount()-1, 
                      metadata:question.data, 
                      preview:question.data.questionHtmlInlinePreview, 
                      questionCardTemplate:this.props.questionCardTemplate,
                      grouped:isGrouped}
                      ));
      }
 
      return [questionHtml, preview];
   },


    renderBulkOperationBar: function() {
      if(this.state.selectedQuestions.length>0) {
        var isAllQuestionsShared = $.inArray(false, $.map(this.state.selectedQuestions, function(e){ return e.isShared;})) == -1;
        return (QuestionBulkOperationBar( {colSpan:this.getAllColumnCount(), 
                                          selectedQuestions: $.map(this.state.selectedQuestions,function(e){return e.id;}), 
                                          deselectsAllHandler:this.deselectsAllQuestionHandler,
                                          columns:this.props.allAvailableColumns,
                                          bulkShareHandler:  this.props.handlers.shareHandler,
                                          isShared:  isAllQuestionsShared,
                                          capabilities: this.props.capabilities} ));
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
                                        expandedAll:this.state.expandedAll,  
                                        canViewPreview:this.props.capabilities.canPreviewQuestion})
                  ),
                  React.DOM.tbody(null,  
                    this.renderBulkOperationBar(),
                    this.renderQuestions()
                  ) 
                ),
              React.DOM.div( {className:"dialogs-container"}
                 
              )
          )
        );
    }
});