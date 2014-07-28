/**
* @jsx React.DOM
*/

var QuestionTypeDialog = React.createClass({displayName: 'QuestionTypeDialog',

    componentDidMount: function(){
        if(this.props.showOnCreate)
        {
           $(this.getDOMNode()).modal("show");
        }
        var closeDialogHandler = this.props.closeDialogHandler;
         $(this.getDOMNode()).on('hidden.bs.modal', function () {
           closeDialogHandler();
        })
    },

    nextStepHandler: function(questionType){
      $(this.getDOMNode()).off('hidden.bs.modal');
       this.props.nextStepHandler(questionType);
    },

   
    render: function() {
        var renderHeaderText = function() {
            return "Add Question";
        };

        var nextStepHandler = this.nextStepHandler;
        var metadata = this.props.metadata;
        var renderBody = function(){
            return (React.DOM.div(null, 

                    React.DOM.ul( {className:"nav nav-tabs"}, 
                      React.DOM.li( {className:"active"},  
                         React.DOM.a( {href:"#newQuestion", 'data-toggle':"tab"}, "New question")
                      )
                                                
                    ),   
                  
                      React.DOM.div( {className:"tab-pane active", id:"newQuestion"}, 
                         React.DOM.div( {className:"tab-body"},        
                            AddQuestionBox( {nextStepHandler:nextStepHandler, metadata:metadata})
                          )
                      )
                    
                

                )
            );
        };

        var renderFooterButtons = function(){
            return ("");
        };

        return (ModalDialog( {renderHeaderText:renderHeaderText, renderBody:renderBody, renderFooterButtons:renderFooterButtons, dialogId:"addQuestionModal"})
                );
    }
});

var AddQuestionBox = React.createClass({displayName: 'AddQuestionBox',

 
   getInitialState: function() {
      return { 
               question: {type: 0, chapter:"", bank:""}
      };
    },


   nextStepHandler: function(){
      this.props.nextStepHandler(this.state.question);
   },

   editHandler: function(question){
      this.setState({question: question});
   },


   render: function() {
            return (React.DOM.div(null, 
                           
                           MetadataFieldEditor( {question:this.state.question, metadata:this.props.metadata, setDefault:true,  editHandler:this.editHandler, field:"type"}),
                           MetadataFieldEditor( {question:this.state.question, metadata:this.props.metadata, setDefault:true,  editHandler:this.editHandler, field:"chapter"}),
                           MetadataFieldEditor( {question:this.state.question, metadata:this.props.metadata, setDefault:true,  editHandler:this.editHandler, field:"bank"}),

                            React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal",  onClick:this.nextStepHandler}, "Next"),
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Close")
                            )
                   )
               );
    }
});