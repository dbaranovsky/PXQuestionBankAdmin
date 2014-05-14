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
           $(tabs).find('.waiting').hide();
           $(tabs).find('iframe').show();
        });
          
    },

    componentDidUpdate: function () {
        this.tabsInitializer($(this.getDOMNode()));
    },

    renderSharingNotification: function(){
         if (this.props.question.isDuplicateOfSharedQuestion && this.props.isDuplicate && this.props.question.isShared) {
        return (React.DOM.div( {className:"shared-note"}, "This question is a duplicate of a ",
                    React.DOM.a( {className:"shared-question-link", href:"", onClick:this.loadSourceQuestion}, "shared question"),
                    "from ", React.DOM.b(null, this.props.question.productCourses.join(', ')) 
               ));
      }

      if (this.props.question.isShared && !this.props.isDuplicate && !this.props.isNew){
                var sharedCourses = this.props.question.productCourses.length;
                return (React.DOM.div( {className:"shared-note"}, "Editing this question content would affect ", sharedCourses == 1 ?  "1 title" :"all "+sharedCourses+ " titles", " that use this question " ));
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
                      this.renderSharingNotification(),
                       React.DOM.div( {className:"tab-body"}, 

                          React.DOM.div(  {className:"iframe waiting"} ),
                          
                          React.DOM.iframe( {src:this.props.question.editorUrl, className:this.props.question.isShared && !this.props.isNew? "shared": ""} )
                          
                       )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadata"}, 
                    this.renderSharingNotification(),
                       React.DOM.div( {className:this.props.question.sharedMetadata == null ? "tab-body" : "tab-body wide"},                            
                            QuestionMetadataEditor( {metadata:this.props.metadata, question:this.props.question, editHandler:this.props.editHandler, isDuplicate:this.props.isDuplicate} ),
                           
                    
                      React.DOM.div( {className:"modal-footer"}, 
                                React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", onClick:this.props.closeDialog}, 
                             "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:this.props.showSaveWarning} , 
                             "Save"
                        )
                      )
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