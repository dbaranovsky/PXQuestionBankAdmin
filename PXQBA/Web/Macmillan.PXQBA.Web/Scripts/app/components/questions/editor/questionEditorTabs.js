/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({displayName: 'QuestionEditorTabs',


    getInitialState: function(){
      return {isHTS: this.props.question.questionType!= null && this.props.question.questionType.toLowerCase()=="hts"? true: false,
              isCustom: this.props.question.questionType!= null,
              isGraph: this.props.question.graphEditorHtml != null,
              viewHistoryMode: this.props.viewHistoryMode != undefined ? this.props.viewHistoryMode : false}
    },

    tabsInitializer: function (container) {
      if(this.props.viewHistoryMode){
        container.find('#history-tab').tab('show');
      } else{
        container.find('#body-tab').tab('show');
      }
         
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
      $(this.getDOMNode()).find("#quizeditorcomponent").html(this.props.question.graphEditorHtml);
      this.iframeLoaded();

    }else{
       this.loadQuestionEditor(this.props.question.editorUrl);
    }
          
    },

    componentDidUpdate: function () {
      //  this.tabsInitializer($(this.getDOMNode()));
    },

     loadSourceQuestion: function(event){
         event.preventDefault();
         this.props.editSourceQuestionHandler(this.props.question.sharedQuestionDuplicateFrom.questionId);
     },

    renderSharingNotification: function(){

     if(this.props.question.sharedQuestionDuplicateFrom!=null && this.props.isDuplicate){
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
        $(this.getDOMNode()).find('#body').find('.waiting').hide();
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
            container: 'quizeditorcomponent',
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
                    container: 'quizeditorcomponent',
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

    showSaveWarning: function(saveAndPublish){
     
      this.props.showSaveWarning(this.state.frameApi, saveAndPublish);
    },

    renderFooterButtons: function(checkForCustomQuestion){
      if(this.state.isHTS && checkForCustomQuestion){
        return null;
      }
      return(
                    React.DOM.div( {className:"modal-footer"}, 
                        React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", title:"Cancel", onClick:this.props.closeDialog}, 
                             "Cancel"
                        ),
                         this.renderPublishButton(),
                         React.DOM.button( {className:"btn btn-primary ",  'data-toggle':"modal",  title:"Save", onClick:this.saveClickHandler} , 
                             "Save"
                        )
                      )
                     );
    },

    renderPublishButton: function() {
        if(this.props.question.isDraft) {
          return (React.DOM.button( {className:"btn btn-default", 'data-toggle':"modal", title:"Save and Publish", onClick:this.saveAndPublishHandler}, 
                              "Save and Publish"
                   ));
        }
        return null;
    },
   
   saveClickHandler: function() {
      this.saveHandler(false);
   },

   saveAndPublishHandler: function() {
      this.saveHandler(true);
   },

   saveHandler: function(saveAndPublish){
      if(this.state.isGraph){
        var question = this.props.question;
        var flashElenemnt = $(this.getDOMNode()).find("#flash")[0]
        if(flashElenemnt.getXML) {
            question.interactionData = flashElenemnt.getXML();
            this.props.editHandler(question);
        } else {
           window.questionDataManager.showWarningPopup(window.enums.messages.warningQuestionEditorMessage);
           return;
        }

      }
      if (this.state.isHTS){
        this.state.frameApi.saveQuestion();
      } else{
        this.showSaveWarning(saveAndPublish)
      }
    },

    switchTabBody: function(event){
        event.preventDefault();
        event.stopPropagation();
        if (this.props.question.status == window.enums.statusesId.deleted){
          alert("You can't edit a deleted question");
           return;
        }

         if (this.props.question.draftFrom != ""){
            $(this.getDOMNode()).find('#body').tab('show');
            this.setState({viewHistoryMode: false});
            return;
        }

        if (this.props.question.status == window.enums.statusesId.availibleToInstructor){
           if (confirm("Do you want to create a draft question?")){
             this.props.handlers.createDraftHandler(null, null);
           }
           return;
        }

         if (this.props.question.status == window.enums.statusesId.inProgress){
           this.props.showEditInPlaceDialog(this.editInPlaceHandler.bind(this, "#body"));
           return;
        }


    },

    switchTabMetadata: function(event){
        event.preventDefault();
        event.stopPropagation();
        if (this.props.question.status == window.enums.statusesId.deleted){
          alert("You can't edit a deleted question");
          return;
        }

        if (this.props.question.draftFrom != ""){
           $(this.getDOMNode()).find('#metadata').tab('show');
           this.setState({viewHistoryMode: false});
            return;
        }

         if (this.props.question.status == window.enums.statusesId.availibleToInstructor){
           if (confirm("Do you want to create a draft question?")){
             this.props.handlers.createDraftHandler(null, null);
           }
           return;
        }

         if (this.props.question.status == window.enums.statusesId.inProgress){
           this.props.showEditInPlaceDialog(this.editInPlaceHandler.bind(this, "#metadata"));
           return;
        }
        
    },

    editInPlaceHandler: function(tab){
      this.props.closeEditInPlaceDialog();
      this.setState({viewHistoryMode: false});
      this.props.showNotificationForInProgress(this.switchTab.bind(this, tab));
      this.switchTab(tab);
    },

    switchTab: function(tab){
        $(this.getDOMNode()).find(tab+'-tab').tab('show');
    },

    renderTabsHeader: function(){
      if (this.state.viewHistoryMode){
        return (React.DOM.ul( {className:"nav nav-tabs"}, 
                             React.DOM.li( {className:"active"},  
                                 React.DOM.a( {href:"#body", id:"body-tab",  onClick:this.switchTabBody}, "Body")
                             ),
                             React.DOM.li(null, 
                                 React.DOM.a( {href:"#metadata",  onClick:this.switchTabMetadata}, "Metadata")
                             ),
                              React.DOM.li(null, 
                                 React.DOM.a( {href:"#history", id:"history-tab", 'data-toggle':"tab"}, "History")
                             )
                        ));
      }

      return (React.DOM.ul( {className:"nav nav-tabs"}, 
                             React.DOM.li( {className:"active"},  
                                 React.DOM.a( {href:"#body", id:"body-tab", 'data-toggle':"tab"} , "Body")
                             ),
                             React.DOM.li(null, 
                                 React.DOM.a( {href:"#metadata", id:"metadata-tab", 'data-toggle':"tab"} , "Metadata")
                             ),
                              React.DOM.li(null, 
                                 React.DOM.a( {href:"#history", id:"history-tab", 'data-toggle':"tab"}, "History")
                             )
                        ));
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

       var isPreventDefault = this.state.viewHistoryMode;
        return ( 
                React.DOM.div(null, 
                  
                 this.renderTabsHeader(),

                React.DOM.div( {className:"tab-content"}, 

                    React.DOM.div( {className:"tab-pane active", id:"body"}, 
                      this.renderSharingNotification(),
                       React.DOM.div( {className:"tab-body .shared"}, 
                      
                           React.DOM.div(  {className:"iframe waiting"} ),
                          React.DOM.div( {id:"quizeditorcomponent", className:iframeClass}),
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
                          VersionHistory( {question:this.props.question, handlers:this.props.handlers})
                       )

                )
                )
               

            )
            );
        }

});