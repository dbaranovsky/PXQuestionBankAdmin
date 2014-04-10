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
    finishSaving: function(e){

        questionDataManager.resetState();
        $(this.getDOMNode()).modal("hide");

        var text = this.props.caption == window.enums.dialogCaptions.editQuestion?  
                      window.enums.messages.succesUpdate :
                      window.enums.messages.succesCreate;


        var notifyOptions = {message: { text: text }, 
                             type: 'success',
                             fadeOut: { enabled: true, delay: 3000 } };
        if(e.status != 200)
        {
            notifyOptions.type = 'danger';
            notifyOptions.message.text = window.enums.message.errorMessage;
        }
        $('.top-center').notify(notifyOptions).show();
    },

    closeDialog: function(){
         $(this.getDOMNode()).modal("hide");
    },


    render: function() {
         var self = this;
        var renderHeaderText = function() {
            return self.props.caption;
        };
        var renderBody = function(){
            return (QuestionEditor( {question:self.props.question, finishSaving:  self.finishSaving, closeDialog:self.closeDialog}));
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

   getInitialState: function() {

      return { question: this.props.question };
    },


    saveQuestion: function(){
       // if(this.props.isNew)
       // {
       // }
        var finishSaving = this.props.finishSaving;
        questionDataManager.updateQuestion(this.state.question).always(finishSaving);

    },

    editHandler: function(editedQuestion){
      this.setState({question: editedQuestion});
    },

    componentDidMount: function(){
       
    },

    render: function() {
        return (
            React.DOM.div(null, 
                      React.DOM.div( {className:"header-buttons"}, 
                         React.DOM.button( {className:"btn btn-primary run-question", 'data-toggle':"modal"} , 
                             React.DOM.span( {className:"glyphicon glyphicon-play"}), " Run Question"
                        ),
                        React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", onClick:this.props.closeDialog}, 
                             "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:this.saveQuestion} , 
                             "Save"
                        )
                      ),
                
                React.DOM.div(null, 
                  QuestionEditorTabs( {question:this.state.question, editHandler:this.editHandler} )
                )
         ));
    }
});


var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',

    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
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
                       QuestionMetadataEditor(  {question:this.props.question, editHandler:this.props.editHandler} ),
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

var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',

    getInitialState: function() {
      return { metadata: []};
    },

    
    loadMetadata: function(data)
    {
        this.setState({metadata: data});
    },

    componentDidMount: function(){
       questionDataManager.getMetadataFields().done(this.loadMetadata); 
    },
    render: function() {
       //   <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"status"} />
        return (
             React.DOM.div( {className:"tab-body"}, 
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"title"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"chapter"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"bank"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"excerciseNo", title:"Excercise Number"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"difficulty"} ),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"cognitiveLevel", title:"CognitiveLevel"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"status"} ),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"guidance", isMultiline:true})
             ) 
         );
    }
});

var MetadataFieldEditor = React.createClass({displayName: 'MetadataFieldEditor',

     saveValueHandler: function(){

     },

     editHandler: function(){
      
       var node = this.refs.editor.getDOMNode();
       var text = "";
       if (node.selectedOptions !== undefined){
            text = node.selectedOptions[0].text;
            value = node.selectedOptions[0].value;
            if (text.toLowerCase()!= value.toLowerCase())
            {
              text = value;
            }
       } 
       else {
            text = node.value;
       }

      var question = this.props.question;
      if (question[this.props.field] !== text)
      {
        question[this.props.field] = text;
        this.props.editHandler(question);
      }

     },

    renderMenuItems: function(availableChoices) {
        var items = [];
        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            items.push(this.renderMenuItem(availableChoice, propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        if(label.toLowerCase()== value.toLowerCase()){
          return (React.DOM.option( {value:label}, label));
        }
        return (React.DOM.option( {value:value}, label));
    },

     renderBody: function(){


       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return (e.metadataName === field) || (e.metadataName === "dlap_q_"+field); });
       var editorType = metadataField.length>0 ? metadataField[0].editorDescriptor.editorType : 0;
       var currentValue = this.props.question[this.props.field];
       switch (editorType) {
          case window.enums.editorType.singleSelect:
             return (React.DOM.select( {ref:"editor", onChange:this.editHandler, value:currentValue},  " ", this.renderMenuItems(metadataField[0].editorDescriptor.availableChoice), " " ) );
          default: 
            if(!this.props.isMultiline){
                 return (React.DOM.input( {type:"text", onChange:this.editHandler, ref:"editor", value:currentValue}))
             }
            return ( React.DOM.textarea( {onChange:this.editHandler,  ref:"editor", className:"question-body-editor",  rows:"10", type:"text", placeholder:"Enter text...", value:currentValue} ));
             
        }
    },



    render: function() {
        return (

            React.DOM.div( {className:"metadata-field-editor"}, 
                   React.DOM.label(null, this.props.title === undefined ? this.props.field : this.props.title),
                   React.DOM.br(null ),
                    this.renderBody(),
                   React.DOM.br(null )
                          
            ) 
         );
    }

});