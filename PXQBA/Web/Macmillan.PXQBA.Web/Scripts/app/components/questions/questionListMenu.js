/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({displayName: 'QuestionListMenu',


    getInitialState: function() {
       return { isFlagged: this.props.data.flag == window.enums.flag.flagged,
                questionId: this.props.data.id
               };
    },

    componentWillReceiveProps: function(nextProps) {
        if(nextProps.data.id!=this.state.questionId) {
            this.setState({ isFlagged: this.props.data.flag == window.enums.flag.flagged,
                            questionId: this.props.data.id
                       });
        }
    }, 

    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      if(this.props.capabilities.canDuplicateQuestion){
       this.props.copyQuestionHandler();
      }
    },

    editQuestionHandler: function() {
        if(this.props.data[window.consts.questionStatusName] == window.enums.statuses.availibleToInstructor  && !this.props.isShared){
           this.createDraftHandler();
          return;
        }

        if(!this.props.isShared && this.props.data[window.consts.questionStatusName] != window.enums.statuses.inProgress){
          this.props.editQuestionHandler();
          return;
        }
    },

    viewQuestionHistoryHandler: function(){
         this.props.editQuestionHandler();
    },

    removeTitleHandler: function(){
      if(confirm("Are you sure you want to remove this question from the current title?")){
         questionDataManager.removeTitle(this.props.data.id);
      }
    },

    publishDraftHandler: function() {
      var notification = userManager.getNotificationById(window.enums.notificationTypes.publishChangesMadeWithinDraft);

      if(notification == null || !notification.isShown){
          this.props.publishDraftHandler();
          return;
      }

      this.props.showNotification(notification, this.props.publishDraftHandler);
      
    },

    createDraftHandler: function() {
      this.props.createDraftHandler(null, this.props.data.status);
    },

    shareHandler: function(){
        this.props.shareHandler();
    },

    toggleFlag: function(){

      questionDataManager.flagQuestion(this.props.data.id, !this.state.isFlagged);
      this.setState( {isFlagged: !this.state.isFlagged, questionId: this.props.data.id});

    },



    componentDidUpdate: function(){
      
      this.initializePopovers();
    },

    componentDidMount: function(){
    },

   
    initializePopovers: function(){
      
       $(this.getDOMNode()).find('[rel="popover"]').popover('destroy'); 
       $(this.getDOMNode()).popover({
                                        selector: '[rel="popover"]',
                                        trigger: 'click', 
                                        placement:'bottom',           
                                        html: true,
                                        container: 'body'
                                        });  
       if(!this.props.showAll){
         $('.popover').remove();
       }


               
    },

    renderCourseCountBadge: function(){
      if (!this.props.isShared){
        return "";
      }
      return(React.DOM.span( {className:"badge"}, this.props.titleCount));
    },

    renderSharedMenu: function(){
      if(this.props.showAll){


    return ( React.DOM.div( {className:"shared-placeholder"} ,  
              React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to", rel:"popover", onClick:this.showPopover,  'data-toggle':"popover",  'data-title':this.props.isShared? "Shared with:" : "",  'data-content':this.props.isShared? this.props.data[window.consts.questionSharedWithName] : "<b>Not Shared</b>"} , 
                 React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge() 
               ),
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", disabled:!this.props.capabilities.canShareQuestion, onClick:this.shareHandler, 'data-toggle':"tooltip", title:"Share this question"}, React.DOM.span( {className:"glyphicon glyphicon-plus-sign"})), 
                    this.props.isShared?
                      React.DOM.button( {type:"button", className:"btn btn-default btn-sm tiny", onClick:this.removeTitleHandler, 'data-toggle':"tooltip", title:"Remove from title"}, React.DOM.span( {className:"glyphicon glyphicon-minus-sign"})) :
                    ""
               ));
     }

      if(this.props.isShared){
      return(
         React.DOM.div( {className:"shared-placeholder"}, 
                    
                         
           React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to",  rel:"popover"} , 
                    React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge()
           ) 
         
                             
          ));
    } 

     

    return ( React.DOM.div( {className:"shared-placeholder"},  " " ));

    },



    renderEditMenu: function(){
      var status = this.props.data[window.consts.questionStatusName];

                  if (!this.props.isShared && status == window.enums.statuses.deleted){
                    return null;
                  }

                  if (this.props.isShared){

                      if(status==window.enums.statuses.inProgress) {
                          return(
                            React.DOM.ul( {className:"dropdown-menu show-menu", role:"menu", 'aria-labelledby':"dropdownMenuType", 'aria-labelledby':"edit-question"}, 
                               React.DOM.li( {role:"presentation", className:"dropdown-header"}, "Edit options"),
                               React.DOM.li( {role:"presentation", className:"divider"}),
                               React.DOM.li( {role:"presentation", className:this.props.metadataCapabilities.canEditQuestion? "" :"disabled", onClick:this.props.metadataCapabilities.canEditQuestion? this.props.editQuestionHandler.bind(this, false, true) : null}, 
                                  React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1"} , 
                                   "Edit in ", this.props.titleCount+1 == 1? "1 title" : "all "+(this.props.titleCount+1)+" titles"
                                  )
                               ),
                               React.DOM.li( {role:"presentation", className:this.props.capabilities.canDuplicateQuestion? "" : "disabled", onClick:this.copyQuestionHandler}, 
                                  React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1"} , 
                                    "Create a copy"
                                  )
                                ),
                               React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.createDraftHandler}, "Create a Draft"))
                            ));
                      }

                  return(
                     React.DOM.ul( {className:"dropdown-menu show-menu", role:"menu", 'aria-labelledby':"dropdownMenuType", 'aria-labelledby':"edit-question"}, 
                       React.DOM.li( {role:"presentation", className:"dropdown-header"}, "Edit options"),
                       React.DOM.li( {role:"presentation", className:"divider"}),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.createDraftHandler}, "Create a Draft")),

                       React.DOM.li( {role:"presentation",  className:this.props.capabilities.canDuplicateQuestion? "" : "disabled", onClick:this.copyQuestionHandler}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1"} , "Create a copy"))
                       
                     ));
                 }

                if (status == window.enums.statuses.inProgress){
                   return(
                     React.DOM.ul( {className:"dropdown-menu show-menu", role:"menu", 'aria-labelledby':"dropdownMenuType",  'aria-labelledby':"edit-question"}, 
                       React.DOM.li( {role:"presentation", className:"dropdown-header"}, "Edit options"),
                       React.DOM.li( {role:"presentation", className:"divider"}),
                       React.DOM.li( {role:"presentation", className:this.props.metadataCapabilities.canEditQuestion? "" :"disabled"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick: this.props.metadataCapabilities.canEditQuestion? this.props.editQuestionHandler.bind(this, false, true) : null}, "Edit in Place")),
                       React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", onClick:this.createDraftHandler}, "Create a Draft"))
                     ));
                }
    },

    renderMenu: function(){
      if (this.props.showAll){
      var isDeleted = this.props.data[window.consts.questionStatusName] == window.enums.statuses.deleted;
      var isDisabled =false;
      if(!this.props.metadataCapabilities.canEditQuestion && !this.props.isShared){
        isDisabled= true;
      }else{
        if(this.props.data[window.consts.questionStatusName] == window.enums.statuses.availibleToInstructor  && !this.props.isShared && !this.props.metadataCapabilities.canCreateDraftFromAvailableQuestion){
          isDisabled = true;
        }
      }

      return(React.DOM.div( {className:"menu-container-main"}, 
                    this.renderDraftButton(),
               React.DOM.div( {className:"dropdown"}, 
                  React.DOM.button( {id:"edit-question", type:"button", className:"btn btn-default btn-sm", onClick:this.editQuestionHandler, disabled:isDisabled,  'data-target':"#", 'data-toggle':"dropdown", title:"Edit Question"}, 
                         React.DOM.span( {className:"glyphicon glyphicon-pencil", 'data-toggle':"tooltip", title:"Edit Question"})
                  ),
                    this.renderEditMenu()
                ),
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm", disabled:!this.props.capabilities.canDuplicateQuestion, onClick:this.copyQuestionHandler,  'data-toggle':"tooltip", title:"Duplicate Question"}, React.DOM.span( {className:"glyphicon glyphicon-copyright-mark"})),
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editNotesHandler, 'data-toggle':"tooltip", title:"Edit Notes"}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"}), " " ), 
               React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn", disabled:!this.props.capabilities.canViewHistory, onClick:this.props.editQuestionHandler.bind(this, true, false), 'data-toggle':"tooltip", title:"View Question History"}, React.DOM.span( {className:"glyphicon icon-version-history"} )) 
               ));
     }

      return (React.DOM.div( {className:"menu-container-main"}));
    },

    renderDraftButton: function() {
      if(this.props.draft) {
        return ( React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  disabled:!this.props.capabilities.canPublishDraft, onClick:this.publishDraftHandler,  'data-toggle':"tooltip", title:"Publish"}, React.DOM.span( {className:"glyphicon glyphicon-open"})));
      }

      return null;
    },

    renderFlagMenu: function(){
        if (this.props.showAll){
          return(React.DOM.div( {className:"menu-container-flag"}, 
                     React.DOM.button( {type:"button", className:"btn btn-default btn-sm", disabled:(!this.state.isFlagged && !this.props.capabilities.canFlagQuestion) || (this.state.isFlagged && !this.props.capabilities.canUnflagQuestion), onClick:this.toggleFlag, 'data-toggle':"tooltip", title:this.state.isFlagged? "Unflag question" : "Flag question"}, 
                     React.DOM.span( {className:this.state.isFlagged? "glyphicon glyphicon-flag flagged" : "glyphicon glyphicon-flag"})
                     ) 
                  ));
      }

      return (React.DOM.div( {className:"menu-container-flag"}, 
                React.DOM.div( {className:"notification-icons-container"}, 
                    this.state.isFlagged ? React.DOM.span( {className:"glyphicon glyphicon-flag flagged"}) : ""
                )
              ));
    },



    render: function() {

        return ( 
                React.DOM.div( {onmouseover:this.hidePopover}, 
                   this.renderMenu(),  
                   this.renderFlagMenu(),  
                   this.renderSharedMenu()
                 )
            );
    


  }
});