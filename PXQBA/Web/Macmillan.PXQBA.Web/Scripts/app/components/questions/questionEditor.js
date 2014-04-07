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

   getInitialState: function() {

      return { question: this.props.question };
    },

    saveQuestion: function(){
        if(this.props.isNew)
        {
            //create
        }

        //save existing
    },

    editHandler: function(){
        alert("ok");
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
                        React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal"} , 
                             "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:this.props.saveQuestion} , 
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
       
        return (
             React.DOM.div( {className:"tab-body"}, 
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"title"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"chapter"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"bank"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"excerciseNo", title:"Excercise Number"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"guidance", isMultiline:true})
             ) 
         );
    }
});

var MetadataFieldEditor = React.createClass({displayName: 'MetadataFieldEditor',

     saveValueHandler: function(){

     },

    renderMenuItems: function(availableChoices) {
        var items = [];
        for (var propertyName in availableChoices) {
            items.push(this.renderMenuItem(availableChoices[propertyName], propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        return (React.DOM.option( {value:value}, label));
    },

     renderBody: function(){


       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return e.name === field; });
       var editorType = metadataField.length>0 ? metadataField[0].typeDescriptor.type : 0;
       switch (editorType) {
          //case window.enums.editorType.singleSelect:
          // Magic number! Do something with that!
          case 1:
             return (React.DOM.select(null,  " ", this.renderMenuItems(metadataField[0].typeDescriptor.availableChoice)) );
          default: 
            if(!this.props.isMultiline){
                 return (React.DOM.input( {type:"text",  value:this.props.question[this.props.field]}))
             }
            return ( React.DOM.textarea( {className:"question-body-editor",  rows:"10", type:"text", placeholder:"Enter text...", ref:this.props.title, value:this.props.question[this.props.field]} ));
             
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