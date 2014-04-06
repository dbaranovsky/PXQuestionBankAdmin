/**
* @jsx React.DOM
*/

var QuestionEditorDialog = React.createClass({displayName: 'QuestionEditorDialog',

    
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

    render: function() {
        var renderHeaderText = function() {
            return "New Question";
        };
        var question = this.props.question;
        var renderBody = function(){
            return (QuestionEditor( {question:question} ));
        };
        var renderFooterButtons = function(){
            return ("");
        };
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             renderFooterButtons:renderFooterButtons, 
                             dialogId:"questionEditorModal"})
                );
    }
});

var QuestionEditor = React.createClass({displayName: 'QuestionEditor',

  
    render: function() {
       
        return (
            React.DOM.div(null, 
                      React.DOM.div( {className:"header-buttons"}, 
                         React.DOM.button( {className:"btn btn-primary run-question", 'data-toggle':"modal"} , 
                             React.DOM.span( {className:"glyphicon glyphicon-play"}), " Run Question"
                        ),
                        React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal"} , 
                             "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal"} , 
                             "Save"
                        )
                      ),
                
                React.DOM.div(null, 
                  QuestionEditorTabs( {question:this.props.question}  )
                )
         ));
    }
});


var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',

    tabsInitializer: function (container) {
         container.find('a:first').tab('show')
    },

    componentDidMount: function() {
         this.tabsInitializer($(this.getDOMNode()));
    },

    componentDidUpdate: function () {
        this.tabsInitializer($(this.getDOMNode()));
    },

    render: function() {
        return ( 
                React.DOM.div(null, 
                  
                   
                        React.DOM.ul( {className:"nav nav-tabs"}, 
                             React.DOM.li( {className:"active"},  
                                 React.DOM.a( {href:"#body", 'data-toggle':"tab"}, "Body")
                             ),
                             React.DOM.li(null, 
                                 React.DOM.a( {href:"#metadata", 'data-toggle':"tab"}, "Metadata")
                             ),
                              React.DOM.li(null, 
                                 React.DOM.a( {href:"#history", 'data-toggle':"tab"}, "History")
                             )
                        ),
             

                  
             
                React.DOM.div( {className:"tab-content"}, 
                    React.DOM.div( {className:"tab-pane active", id:"body"}, 
                       React.DOM.div( {className:"tab-body"}, 
                       React.DOM.td( {dangerouslySetInnerHTML:{__html: this.props.question.preview}} )
                       )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadata"}, 
                        React.DOM.div( {className:"tab-body"}, 
                           React.DOM.label(null, "Title"),
                           React.DOM.br(null ),
                           React.DOM.input( {type:"text", value:this.props.question.title}),
                            React.DOM.br(null ),React.DOM.br(null ),
                           React.DOM.label(null, "Chapter"),
                           React.DOM.br(null ),
                           React.DOM.input( {type:"text", value:this.props.question.chapter}),
                            React.DOM.br(null ),React.DOM.br(null ),
                           React.DOM.label(null, "Bank"),
                           React.DOM.br(null ),
                           React.DOM.input( {type:"text", value:this.props.question.bank} ),
                            React.DOM.br(null ),React.DOM.br(null ),
                           React.DOM.label(null, "Excercise"),
                           React.DOM.br(null ),
                           React.DOM.input( {type:"text", value:this.props.question.excerciseNo}), 
                           React.DOM.br(null ),React.DOM.br(null ),

                           React.DOM.label(null, "Format"),
                           React.DOM.br(null ),
                            React.DOM.textarea( {className:"question-body-editor",  rows:"10", type:"text", placeholder:"Enter text...", ref:"text", value:this.props.question.guidance} )
                           ),  
                           React.DOM.br(null )


                          
                        )
                ),
                React.DOM.div( {className:"tab-pane", id:"history"}, 
                       React.DOM.div( {className:"tab-body"}
                       
                       )
                )

            )
            );
        }

});