/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',


    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
    },

    componentDidMount: function() {
         var tabs = this.getDOMNode();
         this.tabsInitializer($(tabs));
         $(tabs).find('iframe').load(function(){
           $(tabs).find('.iframe-waiting').hide();
           $(tabs).find('iframe').show();
        });
          
    },

    componentDidUpdate: function () {
        this.tabsInitializer($(this.getDOMNode()));
    },

    loadSourceQuestion: function(event){
      event.preventDefault();
      this.props.getSourceQuestion();
    },

    renderSharingNotification: function(){
      if (this.props.question.isDuplicateOfSharedQuestion && this.props.isDuplicate) {
        return (React.DOM.div( {className:"shared-note"}, "This question is a duplicate of a  ",
                    React.DOM.a( {className:"shared-question-link", href:"", onClick:this.loadSourceQuestion}, "shared question"),
                    React.DOM.a( {href:"", onClick:this.loadSourceQuestion}, "Delete question")
               ));
      }

      return null;
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
                          React.DOM.div( {className:"iframe-waiting"} ),
                          React.DOM.iframe( {src:this.props.question.editorUrl} )
                          
                       )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadata"}, 
                       React.DOM.div( {className:this.props.question.sharedMetadata == null ? "tab-body" : "tab-body wide"}, 
                            this.renderSharingNotification(),
                           
                            QuestionMetadataEditor( {metadata:this.props.metadata, question:this.props.question, editHandler:this.props.editHandler, isDuplicate:this.props.isDuplicate} ),
                           
                           React.DOM.br(null )
                      )
                    ),
                     React.DOM.div( {className:"tab-pane", id:"history"}, 
                       React.DOM.div( {className:"tab-body"}, 
                       "Lorem Ipsum"
                       )
                )
                )
               

            )
            );
        }

});