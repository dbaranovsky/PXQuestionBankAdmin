/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',


    getInitialState: function(){
      return {isHTS: this.props.question.questionType!= null && this.props.question.questionType.toLowerCase()=="hts"? true: false,
              isCustom: this.props.question.questionType!= null,
              isGraph: this.props.question.graphEditorHtml != null }
    },

    tabsInitializer: function (container) {
       //  container.find('a:first').tab('show')
    },

    componentDidMount: function() {
      //todo: refactor
         var tabs = this.getDOMNode();
         this.tabsInitializer($(tabs));
          
      //   $(tabs).find('iframe').load(function(){
      //     $(tabs).find('.waiting').hide();
      //     $(tabs).find('iframe').show();
       // });
    if (this.props.question.graphEditorHtml != null){
      $(this.getDOMNode()).find("#editoriframecontainer").html(this.props.question.graphEditorHtml);
      this.iframeLoaded();

    }else{
    this.loadQuestionEditor(this.props.question.editorUrl);
    }
          
    },

    componentDidUpdate: function () {
        this.tabsInitializer($(this.getDOMNode()));
    },

     loadSourceQuestion: function(event){
         event.preventDefault();
         this.props.editSourceQuestionHandler(this.props.question.sharedQuestionDuplicateFrom.questionId);
     },


    renderSharingNotification: function(){

     if(this.props.question.sharedQuestionDuplicateFrom!=null){
            return (React.DOM.div( {className:"shared-note"}, "This question is a duplicate of a ",
                    React.DOM.a( {className:"shared-question-link", href:"", onClick:this.loadSourceQuestion}, "shared question"),
                    "from ", React.DOM.b(null, this.props.question.sharedQuestionDuplicateFrom.sharedWith) 
               ));
      }

      if (this.props.question.isShared && !this.props.isDuplicate && !this.props.isNew){
                var sharedCourses = this.props.question.productCourses.length;
                return (React.DOM.div( {className:"shared-note"}, "Editing this question content would affect ", sharedCourses == 1 ?  "1 title" :"all "+sharedCourses+ " titles", " that use this question " ));
      }

      return null;
    },

     iframeLoaded: function(){
        $(this.getDOMNode()).find('.waiting').hide();
    },
     loadQuestionEditor: function(url) {

          var self= this;
         rpc = this.state.isHTS ? this.hookUpHTSEditor(url) :this.hookupBHEditor(url);
         this.setState({frameApi: rpc});
    },

    
    //move to helper
    hookupBHEditor: function(url){
      var self = this;
          rpc = new easyXDM.Rpc({
            //Standalone component URL for the BrainHoney component to display
            remote: url,
            //Name of the <div> or other element that will contain the iframe
            container: 'editoriframecontainer',
            //HTML props for the created iframe
            props: {
                frameborder: 0,
                height: '100%',
                width: '100%'
            }
      }, {
          //Your half of the communication implementation
          local: {
              // Perform any initialization, like registering for events
              init: function (successFn, errorFn) {
              //    inputPannel.hideLoad();
                  rpc.addListeners('componentsaved|componentcancelled|advancededitclicked');
                  if (successFn) {
                     self.iframeLoaded();
                  }
              },
              // Handle any events 
              onEvent: function (event) {
                  switch (event) {
                      //Handle each event using the 'arguments' variable           
                      case 'componentsaved':

                             self.props.saveQuestion();
                          
                          break;
                      case 'componentcancelled':
                        
                          break;
                      case "advancededitclicked":
                            alert("advancededitclicked");
                      break;
                  }
              }
          },
          //Define the Frame API stub interface
          remote: {
              addListeners: {},
              fireEvent: {},
              saveComponent: {},
              navigate: {},
              getProperties: {},
              getComponentState: {},
              callComponentMethod: {},
              hasRight: {},
              setShowBeforeUnloadPrompts: {}
          }
      });
        return rpc;
    },


    hookUpHTSEditor: function (url) {
      
        var self = this;
                var rpc = new easyXDM.Rpc({
                    remote: url,
                    container: 'editoriframecontainer',
                    //HTML props for the created iframe
                    props: {
                        frameborder: 0,
                        height: '1000px',
                        width: '100%',
                        scrolling: 'no',
                   }

                },
                {
                    local: {
                        questionSaved: function (questionId, success, error) {
                         // alert("saved");
                           self.showSaveWarning();
                        }
                    },
                    remote: {
                        saveQuestion: {},
                        previewQuestion: {},
                        isDirty: {}
                    }
                });
        return rpc; 
            
    },

    showSaveWarning: function(){
     
      this.props.showSaveWarning(this.state.frameApi);
    },

    renderFooterButtons: function(checkForCustomQuestion){
      if(this.state.isHTS && checkForCustomQuestion){
        return null;
      }

       return(
                        React.DOM.div( {className:"modal-footer"}, 
                         React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", onClick:this.props.closeDialog}, 
                              "Cancel"
                        ),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal", onClick:this.saveClickHandler} , 
                             "Save"
                        )
                      )
                     
                     );
    },
   

    saveClickHandler: function(){
      if(this.state.isGraph){
        var question = this.props.question;
        question.interactionData =  $(this.getDOMNode()).find("#flash")[0].getXML();
        this.props.editHandler(question);
      }
      if (this.state.isHTS){
        this.state.frameApi.saveQuestion();
      } else{
        this.showSaveWarning()
      }
    },

    render: function() {

       var iframeClass = ""
       if ((this.props.question.isShared && !this.props.isNew) || (this.props.question.sharedQuestionDuplicateFrom != null && this.props.isDuplicate)){
        iframeClass = "shared";
       }

       if (this.state.isHTS){
        iframeClass = iframeClass + " hts";
       }

       if (this.state.isGraph){
        iframeClass = iframeClass + " graph";
       }

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
                       React.DOM.div( {className:"tab-body .shared"}, 
                      
                           React.DOM.div(  {className:"iframe waiting"} ),
                          React.DOM.div( {id:"editoriframecontainer", className:iframeClass}),
                           this.renderFooterButtons(true)
                          
                       )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadata"}, 
                    this.renderSharingNotification(),
                       React.DOM.div( {className:!this.props.question.isShared  ? "tab-body" : "tab-body wide"},                            
                            QuestionMetadataEditor( {metadata:this.props.metadata, question:this.props.question, editHandler:this.props.editHandler, isDuplicate:this.props.isDuplicate} ),
                           this.renderFooterButtons()
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