/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({


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
        if(!this.props.isShared && this.props.data[window.consts.questionStatusName] == window.enums.statuses.deleted){
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
      
     $(this.getDOMNode()).find('span.notes').popover({

                                        trigger: 'hover', 
                                        placement:'bottom',           
                                        html: true,
                                        container: 'body'
                                        });  

     
       if(!this.props.showAll){
         $('body').children('.popover').remove();
       }
               
    },

    renderCourseCountBadge: function(){
      if (!this.props.isShared){
        return "";
      }
      return(<span className="badge">{this.props.titleCount}</span>);
    },

    renderSharedMenu: function(){
      if(this.props.showAll){


    return ( <div className="shared-placeholder" > 
               <div className="btn-group">
               <SharedButton sharedWith={this.props.data[window.consts.questionSharedWithName]} trigger='click'/>
               <button type="button" className="btn btn-default btn-sm custom-small-btn" disabled={!this.props.capabilities.canShareQuestion} onClick={this.shareHandler} data-toggle="tooltip" title="Share this question">
                  <span className="icon-plus-squared"></span>
                </button> 
                    {this.props.isShared?
                <button type="button" className="btn btn-default btn-sm custom-small-btn" onClick={this.removeTitleHandler} data-toggle="tooltip" title="Remove from title">
                  <span className="icon-minus-squared"></span>
                </button> : ""}
               </div>
               </div>);
     }

      if(this.props.isShared){
      return(
         <div className={"shared-placeholder"+ (this.state.isFlagged? " flagged" : "")}>
                  
        
          <SharedButton sharedWith={this.props.data[window.consts.questionSharedWithName]} trigger='click'/>               
          </div>);
    } 

     

    return ( <div className="shared-placeholder"> </div>);

    },



    renderEditMenu: function(){
      var status = this.props.data[window.consts.questionStatusName];

                  if (!this.props.isShared && status == window.enums.statuses.deleted){
                    return null;
                  }

                  if (this.props.isShared) {

                      if(status==window.enums.statuses.inProgress) {
                          return(
                            <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType" aria-labelledby="edit-question">
                               <li role="presentation" className="dropdown-header">Edit options</li>
                               <li role="presentation" className="divider"></li>
                               <li role="presentation" className={this.props.metadataCapabilities.canEditQuestion? "" :"disabled"} onClick={this.props.metadataCapabilities.canEditQuestion? this.props.editQuestionHandler.bind(this, false, true) : null}>
                                  <a className="edit-field-item" role="menuitem" tabIndex="-1" >
                                   Edit in {this.props.titleCount+1 == 1? "1 title" : "all "+(this.props.titleCount+1)+" titles"}
                                  </a>
                               </li>
                               <li role="presentation" className={this.props.capabilities.canDuplicateQuestion? "" : "disabled"} onClick={this.copyQuestionHandler}>
                                  <a className="edit-field-item" role="menuitem" tabIndex="-1" >
                                    Create a copy
                                  </a>
                                </li>
                               <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.createDraftHandler}>Create a Draft</a></li>
                            </ul>);
                      }

                  return(
                     <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType" aria-labelledby="edit-question">
                       <li role="presentation" className="dropdown-header">Edit options</li>
                       <li role="presentation" className="divider"></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.createDraftHandler}>Create a Draft</a></li>

                       <li role="presentation"  className={this.props.capabilities.canDuplicateQuestion? "" : "disabled"} onClick={this.copyQuestionHandler}><a className="edit-field-item" role="menuitem" tabIndex="-1" >Create a copy</a></li>
                       
                     </ul>);
                 }

                if (status == window.enums.statuses.inProgress){
                   return(
                     <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType"  aria-labelledby="edit-question">
                       <li role="presentation" className="dropdown-header">Edit options</li>
                       <li role="presentation" className="divider"></li>
                       <li role="presentation" className={this.props.metadataCapabilities.canEditQuestion? "" :"disabled"}><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={ this.props.metadataCapabilities.canEditQuestion? this.props.editQuestionHandler.bind(this, false, true) : null}>Edit in Place</a></li>
                       <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.createDraftHandler}>Create a Draft</a></li>
                     </ul>);
                }


               if (status == window.enums.statuses.availibleToInstructor){
                   return(
                     <ul className="dropdown-menu show-menu" role="menu" aria-labelledby="dropdownMenuType"  aria-labelledby="edit-question">
                       <li role="presentation" className="dropdown-header">Edit options</li>
                       <li role="presentation" className="divider"></li>
                       <li role="presentation" className={this.props.metadataCapabilities.canEditQuestion ? "" :"disabled"}>
                          <a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={ this.props.metadataCapabilities.canEditQuestion? this.props.editQuestionHandler.bind(this, false, true) : null}>Edit in Place</a>
                       </li>
                       <li role="presentation" className={this.props.metadataCapabilities.canCreateDraftFromAvailableQuestion ? "" :"disabled"}>
                         <a className="edit-field-item" role="menuitem" tabIndex="-1" onClick={this.props.metadataCapabilities.canCreateDraftFromAvailableQuestion ? this.createDraftHandler : null }>Create a Draft</a>
                        </li>
                     </ul>);
                }
    },

    renderMenu: function(){
      if (this.props.showAll){
      var isDeleted = this.props.data[window.consts.questionStatusName] == window.enums.statuses.deleted;
      var isDisabled =false;
      if(!this.props.metadataCapabilities.canEditQuestion && 
         !this.props.isShared &&
         !this.props.data[window.consts.questionStatusName] == window.enums.statuses.availibleToInstructor){
        isDisabled= true;
      }else{
        if(this.props.data[window.consts.questionStatusName] == window.enums.statuses.availibleToInstructor  && 
          !this.props.isShared && 
          !this.props.metadataCapabilities.canCreateDraftFromAvailableQuestion &&
          !this.props.metadataCapabilities.canEditQuestion) {
          isDisabled = true;
        }
      }

      return(<div className="menu-container-main">
                    {this.renderDraftButton()}
               <div className="dropdown">
                  <button id="edit-question" type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler} disabled={isDisabled}  data-target="#" data-toggle="dropdown" title="Edit Question">
                         <span className="icon-pencil-1" data-toggle="tooltip" title="Edit Question"></span>
                  </button>
                    {this.renderEditMenu()}
                </div>
               <button type="button" className="btn btn-default btn-sm" disabled={!this.props.capabilities.canDuplicateQuestion} onClick={this.copyQuestionHandler}  data-toggle="tooltip" title="Duplicate Question"><span className="icon-docs"></span></button>
               <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler} data-toggle="tooltip" title="Edit Notes"><span className="glyphicon glyphicon-list-alt"></span> </button> 
               <button type="button" className="btn btn-default btn-sm custom-btn" disabled={!this.props.capabilities.canViewHistory} onClick={this.props.editQuestionHandler.bind(this, true, false)} data-toggle="tooltip" title="View Question History"><span className="glyphicon icon-version-history" ></span></button> 
               </div>);
     }

      return (<div className="menu-container-main"></div>);
    },

    renderDraftButton: function() {
      if(this.props.draft) {
        return ( <button type="button" className="btn btn-default btn-sm"  disabled={!this.props.capabilities.canPublishDraft} onClick={this.publishDraftHandler}  data-toggle="tooltip" title="Publish"><span className="glyphicon glyphicon-open"></span></button>);
      }

      return null;
    },

    renderFlagMenu: function(){

        if (this.props.showAll){
          return(<div className="menu-container-flag">
                     <button type="button" className="btn btn-default btn-sm" disabled={(!this.state.isFlagged && !this.props.capabilities.canFlagQuestion) || (this.state.isFlagged && !this.props.capabilities.canUnflagQuestion)} onClick={this.toggleFlag} data-toggle="tooltip" title={this.state.isFlagged? "Unflag question" : "Flag question"}>
                     <span className={this.state.isFlagged? "icon-embassy flagged" : "icon-embassy not-flagged"}></span>
                     </button> 
                     <div className="list-menu-icon-container">
                       {this.props.data.notes != "" ? <span className="glyphicon glyphicon-list-alt notes" rel="notes"  data-toggle="popover" data-title="Notes"  data-content={this.props.data.notes} ></span> : <span>&nbsp;</span>}
                     </div>
                  </div>);
      }

      var notesClassName = "glyphicon glyphicon-list-alt notes";
    //  if (this.state.isFlagged){
    //    notesClassName += " flagged";
    //  }

      return (<div className="menu-container-flag">
                
                  <div className="list-menu-icon-container">
                    {this.state.isFlagged ? <span className="icon-embassy flagged flagged-container"></span> : <span>&nbsp;</span>}
                  </div>
                  <div className="list-menu-icon-container">
                    {this.props.data.notes != "" ? <span className="glyphicon glyphicon-list-alt notes"></span> : <span>&nbsp;</span>}
                  </div>
            
              </div>);
    },



    render: function() {

        return ( 
                <div onmouseover={this.hidePopover}>
                   {this.renderMenu()}  
                   {this.renderFlagMenu()}  
                   {this.renderSharedMenu()}
                 </div>
            );
    


  }
});