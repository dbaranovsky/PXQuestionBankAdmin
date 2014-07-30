/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({displayName: 'VersionHistory',
   
    
    getInitialState: function(){
        return {loading: true, versionHistory: null, showPreview: false};
    },

    componentDidMount: function(){
        this.updateQuestionVersion();
    },

    updateQuestionVersion: function() {
        questionDataManager.getQuestionVersions().done(this.setVersions);
    },

    setVersions: function(data){
        this.setState({loading: false, versionHistory: data});
    },

    renderRows: function(){
        
    if (this.state.loading){
        return ( React.DOM.div( {className:"waiting"} ));
    }

     if (this.state.versionHistory == null){
            return( React.DOM.p(null,  " No version availible for this question. " ));
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

            return (VersionHistoryRow( {version:version, renderPreview:self.renderPreview.bind(this, version.questionPreview), handlers:self.props.handlers,
                     canTryVersion:self.props.question.canTestQuestionVersion,
                     canRestoreVersion:  self.props.question.canRestoreVersion, 
                     canCreateDraftFromVersion:  self.props.question.canCreateDraftFromVersion} ));
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
  
        return (ModalDialog( {renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             dialogId:"versionPreview",
                             closeDialogHandler:  this.closePreviewDialog,
                             showOnCreate:  true,
                             preventDefaultClose: true,
                             setInnerHtml:true,
                             innerHtml:this.state.htmlPreview})
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
      

        return ( React.DOM.div(null, 
                    React.DOM.div( {className:"versions"}, 
                          this.renderRows()                     
                    ),
                     this.renderPreviewDialog()  
                 )

           
         );
    }
});




var VersionHistoryRow = React.createClass({displayName: 'VersionHistoryRow',
   

    getInitialState: function(){

        return ({loading: false});
    },

    renderMenu: function(){
      return(React.DOM.div( {className:"menu-container-main version-history"}, 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", disabled:this.state.loading || !this.props.canTryVersion, 'data-toggle':"tooltip",  title:"Try Question", onClick:this.tryQuestion}, React.DOM.span( {className:"glyphicon glyphicon-play"}), " " ),
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Preview Question", onClick:this.props.renderPreview}, React.DOM.span( {className:"glyphicon glyphicon-search"})),
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"New Question from this Version", onClick:this.newQuestionFormVersionHandler}, React.DOM.span( {className:"glyphicon glyphicon-file"}), " " ), 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", disabled:!this.props.canCreateDraftFromVersion, 'data-toggle':"tooltip", title:"New Draft from this Version", onClick:this.newDraftFormVersionHandler}, React.DOM.span( {className:"icon-pencil-1"} )), 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", disabled:(this.state.loading && this.state.restoring) ||  !this.props.canRestoreVersion, 'data-toggle':"tooltip", title:"Restore this Version", onClick:this.restoreVersion}, React.DOM.span( {className:"glyphicon glyphicon-repeat"} )) 
               ));

    },

    restoreVersion: function(){
        this.setState({loading: true, restoring: true});
        questionDataManager.restoreVersion(this.props.version).done(this.reloadEditor).always(this.stopLoading);
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
        questionDataManager.getVersionPreviewLink(this.props.version.version).done(this.startQuiz).always(this.stopLoading);
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
        return(React.DOM.p(null, "Duplicate from: ", React.DOM.i(null, version.duplicateFrom.title),", ", React.DOM.b(null, "Chapter:"),React.DOM.i(null, version.duplicateFrom.chapter, " " ),",",React.DOM.b(null, "Bank:"),React.DOM.i(null, version.duplicateFrom.bank)));
        }

        return null
    },

    renderDraftInfo: function(){
        var version = this.props.version;
        if(version.isPublishedFromDraft){
            return(React.DOM.p(null, React.DOM.i(null, "Published from draft")));
        }
        return null;
    },

    renderRestoreInfo: function(){
           var version = this.props.version;
        if(version.restoredFromVersion != undefined && version.restoredFromVersion != null){
            return(React.DOM.p(null, React.DOM.i(null, "Restored from version: ", React.DOM.b(null, version.restoredFromVersion.version), " of ", version.restoredFromVersion.versionDate, " by ", version.restoredFromVersion.versionAuthor), " " ));
        }
        return null;
    },

    renderWaiter: function(){
        if (this.state.loading){
             return (React.DOM.div( {className:"waiting small"}));
        }
       return null;
    },

    render: function() {
        var version = this.props.version;
  
        return ( React.DOM.div( {className:"version-row"}, 
                        React.DOM.div( {className:"version-cell"}, 

                          React.DOM.span( {className: version.isCurrent? "version-text current" : "version-text"},  " Version of ", version.modifiedDate, " by ", version.modifiedBy, " ", version.isInitial? "(initial) ": "", " ", version.isDraftInitialVersion? "(draft was created)": ""),
                          React.DOM.br(null),
                           this.renderDraftInfo(),
                           this.renderRestoreInfo(),
                           this.renderDuplicateFromInfo()
                        ),     

                        React.DOM.div( {className:"version-cell menu"}, 

                         this.renderMenu(),
                         this.renderWaiter()
                        )
                 )
                        

           
         );
    }
});