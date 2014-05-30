/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({displayName: 'VersionHistory',
   
    
    getInitialState: function(){
        return {loading: true, versionHistory: null};
    },

    componentDidMount: function(){
        var self= this;
        questionDataManager.getQuestionVersions(this.props.question.id).done(self.setVersions);
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


     var vesrions = this.state.versionHistory.versions.map(function (version) {
            return (VersionHistoryRow( {version:version}));
          });

        return vesrions;

    },

    render: function() {
        // var style = this.props.question.isShared? {} : {display: "none !important"};
        // var localClass = "local";
        // if (this.props.question.isShared){
        //   localClass+= " wide";
        // } else if(this.props.question.sharedQuestionDuplicateFrom != null && this.props.isDuplicate){
        //   localClass +=" with-notification";
        // }
      

        return ( React.DOM.div( {className:"versions"}, 
                                         
                          this.renderRows()                         
                 )

           
         );
    }
});




var VersionHistoryRow = React.createClass({displayName: 'VersionHistoryRow',
   
    renderRows: function(){
         
     // return rows;
    },

    renderMenu: function(){
      return(React.DOM.div( {className:"menu-container-main version-history"}, 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip",  title:"Try Question"}, React.DOM.span( {className:"glyphicon glyphicon-play"}), " " ),
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm", 'data-toggle':"tooltip", title:"Preview Question"}, React.DOM.span( {className:"glyphicon glyphicon-search"})),
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm",  'data-toggle':"tooltip", title:"New Question from this Version"}, React.DOM.span( {className:"glyphicon glyphicon-file"}), " " ), 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm ",  'data-toggle':"tooltip", title:"New Draft from this Version"}, React.DOM.span( {className:"glyphicon glyphicon-pencil"} )), 
                React.DOM.button( {type:"button", className:"btn btn-default btn-sm ",  'data-toggle':"tooltip", title:"Restore this Version"}, React.DOM.span( {className:"glyphicon glyphicon-repeat"} )) 
               ));
     

    

    },

    render: function() {
        var version = this.props.version;
     //   var style = this.props.question.isShared? {} : {display: "none !important"};
     //   var localClass = "local";
     //   if (this.props.question.isShared){
     //     localClass+= " wide";
     //   } else if(this.props.question.sharedQuestionDuplicateFrom != null && this.props.isDuplicate){
     //     localClass +=" with-notification";
     //   }

        return ( React.DOM.div( {className:"version-row"}, 
                        React.DOM.div( {className:"version-cell"}, 
                          React.DOM.span( {className: version.isCurrent? "version-text current" : "version-text"},  " Version of ", version.modifiedDate, " by ", version.modifiedBy, " ", version.isInitial? "(initial)": "", " " )
                        ),     
                        React.DOM.div( {className:"version-cell menu"}, 
                         this.renderMenu()
                        )
                 )
                        

           
         );
    }
});