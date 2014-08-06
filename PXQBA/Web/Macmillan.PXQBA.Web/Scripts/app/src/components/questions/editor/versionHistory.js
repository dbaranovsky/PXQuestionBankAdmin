/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({
   
    
    getInitialState: function(){
        return {loading: true, versionHistory: null, showPreview: false};
    },

    componentDidMount: function(){
        this.updateQuestionVersion();
    },

    updateQuestionVersion: function() {
        this.setState({loading: true});
        questionDataManager.getQuestionVersions(this.props.currentCourseId, this.props.question.realQuestionId).done(this.setVersions);
    },

    setVersions: function(data){
        this.setState({loading: false, versionHistory: data});
    },

    renderRows: function(){
        
    if (this.state.loading){
        return ( <div className="waiting" />);
    }

     if (this.state.versionHistory == null){
            return( <p> No version availible for this question. </p>);
    }

    var self= this;

     var versionsCount = this.state.versionHistory.versions.length;
     var vesrions = this.state.versionHistory.versions.map(function (version, i) {
            if (i== 0){
                version.isCurrent = true;
            }

            if(i+1 == versionsCount){
                version.isInitial = true;
            }

            return (<VersionHistoryRow 
                     currentCourseId={self.props.currentCourseId}
                     version={version} renderPreview={self.renderPreview.bind(this, version.questionPreview)}
                     handlers={self.props.handlers}
                     canTryVersion={self.props.question.canTestQuestionVersion}
                     canRestoreVersion = {self.props.question.canRestoreVersion} 
                     canCreateDraftFromVersion = {self.props.question.canCreateDraftFromVersion} />);
          });

        return vesrions;

    },

      closePreviewDialog: function(){
         $('.modal-backdrop').first().remove(); 
         this.setState({showPreview: false});
     },



     renderPreviewDialog: function(htmlPreview){
        
      if (this.state.showPreview){
       
        var self = this;
        var renderHeaderText = function() {
            return ("Preview");
        };
        
        var renderBody = function(){
            return ("");
        };
  
        return (<ModalDialog renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             dialogId="versionPreview"
                             closeDialogHandler = {this.closePreviewDialog}
                             showOnCreate = {true}
                             preventDefaultClose ={true}
                             setInnerHtml={true}
                             innerHtml={this.state.htmlPreview}/>
                );
      }

      return null;
     },

     renderPreview: function(htmlPreview){
        this.setState({showPreview: true, htmlPreview: htmlPreview});
     },

    render: function() {
        // var style = this.props.question.isShared? {} : {display: "none !important"};
        // var localClass = "local";
        // if (this.props.question.isShared){
        //   localClass+= " wide";
        // } else if(this.props.question.sharedQuestionDuplicateFrom != null && this.props.isDuplicate){
        //   localClass +=" with-notification";
        // }
      

        return ( <div>
                    <div className="versions">
                          {this.renderRows()}                     
                    </div>
                     {this.renderPreviewDialog()}  
                 </div>

           
         );
    }
});




var VersionHistoryRow = React.createClass({
   

    getInitialState: function(){

        return ({loading: false});
    },

    renderMenu: function(){
      return(<div className="menu-container-main version-history">
                <button type="button" className="btn btn-default btn-sm" disabled={this.state.loading || !this.props.canTryVersion} data-toggle="tooltip"  title="Try Question" onClick={this.tryQuestion}><span className="glyphicon glyphicon-play"></span> </button>
                <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Preview Question" onClick={this.props.renderPreview}><span className="glyphicon glyphicon-search"></span></button>
                <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="New Question from this Version" onClick={this.newQuestionFormVersionHandler}><span className="glyphicon glyphicon-file"></span> </button> 
                <button type="button" className="btn btn-default btn-sm" disabled={!this.props.canCreateDraftFromVersion} data-toggle="tooltip" title="New Draft from this Version" onClick={this.newDraftFormVersionHandler}><span className="icon-pencil-1" ></span></button> 
                <button type="button" className="btn btn-default btn-sm" disabled={(this.state.loading && this.state.restoring) ||  !this.props.canRestoreVersion} data-toggle="tooltip" title="Restore this Version" onClick={this.restoreVersion}><span className="glyphicon glyphicon-repeat" ></span></button> 
               </div>);

    },

    restoreVersion: function(){
        this.setState({loading: true, restoring: true});
        debugger;
        questionDataManager.restoreVersion(this.props.currentCourseId, this.props.version.id, this.props.version).done(this.reloadEditor).always(this.stopLoading);
    },

    reloadEditor: function(){

            this.props.handlers.reloadEditor();
    },

    newQuestionFormVersionHandler: function() {
        this.props.handlers.createQuestionFromVersionHandler(this.props.version.id, this.props.version.version)
    },

    newDraftFormVersionHandler: function() {
        this.props.handlers.createDraftHandler(this.props.version.id, this.props.version.version);
    },

    tryQuestion: function(){
        this.setState({loading: true});
        questionDataManager.getVersionPreviewLink(this.props.currentCourseId, this.props.version.id, this.props.version.version).done(this.startQuiz).always(this.stopLoading);
    },

    startQuiz: function(data){
        var previewWindow = window.open(data.url, '_blank', 'location=yes,height=600,width=600,scrollbars=yes,status=yes');
        previewWindow.focus();
    },


    stopLoading: function(){
         this.setState({loading: false, restoring: false});
    },

    renderDuplicateFromInfo: function(){
           var version = this.props.version;
        if(version.duplicateFrom.id !="" && version.duplicateFrom.id !=null){
        return(<p>Duplicate from: <i>{version.duplicateFrom.title}</i>, <b>Chapter:</b><i>{version.duplicateFrom.chapter} </i>,<b>Bank:</b><i>{version.duplicateFrom.bank}</i></p>);
        }

        return null
    },

    renderDraftInfo: function(){
        var version = this.props.version;
        if(version.isPublishedFromDraft){
            return(<p><i>Published from draft</i></p>);
        }
        return null;
    },

    renderRestoreInfo: function(){
           var version = this.props.version;
        if(version.restoredFromVersion != undefined && version.restoredFromVersion != null){
            return(<p><i>Restored from version: <b>{version.restoredFromVersion.version}</b> of {version.restoredFromVersion.versionDate} by {version.restoredFromVersion.versionAuthor}</i> </p>);
        }
        return null;
    },

    renderWaiter: function(){
        if (this.state.loading){
             return (<div className="waiting small"></div>);
        }
       return null;
    },

    render: function() {
        var version = this.props.version;
  
        return ( <div className="version-row">
                        <div className="version-cell">

                          <span className= {version.isCurrent? "version-text current" : "version-text"}> Version of {version.modifiedDate} by {version.modifiedBy} {version.isInitial? "(initial) ": ""} {version.isDraftInitialVersion? "(draft was created)": ""}</span>
                          <br/>
                           {this.renderDraftInfo()}
                           {this.renderRestoreInfo()}
                           {this.renderDuplicateFromInfo()}
                        </div>     

                        <div className="version-cell menu">

                         {this.renderMenu()}
                         {this.renderWaiter()}
                        </div>
                 </div>
                        

           
         );
    }
});