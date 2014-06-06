/**
* @jsx React.DOM
*/
var QuestionEditorTabs = React.createClass({


    getInitialState: function(){
      return {isHTS: this.props.question.questionType!= null && this.props.question.questionType.toLowerCase()=="hts"? true: false,
              isCustom: this.props.question.questionType!= null,
              isGraph: this.props.question.graphEditorHtml != null,
              viewHistoryMode: this.props.viewingHistory != undefined ? this.props.viewingHistory : false}
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
            return (<div className="shared-note">This question is a duplicate of a&nbsp;
                    <a className="shared-question-link" href="" onClick={this.loadSourceQuestion}>shared question</a>
                    from <b>{this.props.question.sharedQuestionDuplicateFrom.sharedWith}</b> 
               </div>);
      }

      if (this.props.question.isShared && !this.props.isDuplicate && !this.props.isNew){
                var sharedCourses = this.props.question.productCourses.length;
                return (<div className="shared-note">Editing this question content would affect {sharedCourses == 1 ?  "1 title" :"all "+sharedCourses+ " titles"} that use this question </div>);
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
                    <div className="modal-footer">
                        <button className="btn btn-default" data-toggle="modal" title="Cancel" onClick={this.props.closeDialog}>
                             Cancel
                        </button>
                         {this.renderPublishButton()}
                         <button className="btn btn-primary " data-toggle="modal"  title="Save" onClick={this.saveClickHandler} >
                             Save
                        </button>
                      </div>
                     );
    },

    renderPublishButton: function() {
        if(this.props.question.isDraft) {
          return (<button className="btn btn-default" data-toggle="modal" title="Save and Publish" onClick={this.saveAndPublishHandler}>
                              Save and Publish
                   </button>);
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
        question.interactionData =  $(this.getDOMNode()).find("#flash")[0].getXML();
        this.props.editHandler(question);
      }
      if (this.state.isHTS){
        this.state.frameApi.saveQuestion();
      } else{
        this.showSaveWarning(saveAndPublish)
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
                <div>
                  
                 
                        <ul className="nav nav-tabs">
                             <li className="active"> 
                                 <a href="#body" id="body-tab"data-toggle="tab">Body</a>
                             </li>
                             <li>
                                 <a href="#metadata" data-toggle="tab">Metadata</a>
                             </li>
                              <li>
                                 <a href="#history" id="history-tab" data-toggle="tab">History</a>
                             </li>
                        </ul>
               
             
                <div className="tab-content">

                    <div className="tab-pane active" id="body">
                      {this.renderSharingNotification()}
                       <div className="tab-body .shared">
                      
                           <div  className="iframe waiting" />
                          <div id="quizeditorcomponent" className={iframeClass}></div>
                           {this.renderFooterButtons(true)}
                          
                       </div>
                    </div>
                    <div className="tab-pane" id="metadata">
                    {this.renderSharingNotification()}
                       <div className={!this.props.question.isShared  ? "tab-body" : "tab-body wide"}>                           
                            <QuestionMetadataEditor metadata={this.props.metadata} question={this.props.question} editHandler={this.props.editHandler} isDuplicate={this.props.isDuplicate} />
                           {this.renderFooterButtons()}
                       </div>
                    </div>
                     <div className="tab-pane" id="history">

                       <div className="tab-body">
                          <VersionHistory question={this.props.question} handlers={this.props.handlers}/>
                       </div>

                </div>
                </div>
               

            </div>
            );
        }

});