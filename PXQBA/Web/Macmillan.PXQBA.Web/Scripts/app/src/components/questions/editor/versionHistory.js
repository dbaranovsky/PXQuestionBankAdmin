/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({
   
    
    getInitialState: function(){
        return {loading: true, versionHistory: null};
    },

    componentDidMount: function(){
        var self= this;
        questionDataManager.getQuestionVersions().done(self.setVersions);
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


     var vesrions = this.state.versionHistory.versions.map(function (version) {
            return (<VersionHistoryRow version={version}/>);
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
      

        return ( <div className="versions">
                                         
                          {this.renderRows()}                         
                 </div>

           
         );
    }
});




var VersionHistoryRow = React.createClass({
   
    renderRows: function(){
         
     // return rows;
    },

    renderMenu: function(){
      return(<div className="menu-container-main version-history">
                <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip"  title="Try Question"><span className="glyphicon glyphicon-play"></span> </button>
                <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Preview Question"><span className="glyphicon glyphicon-search"></span></button>
                <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip" title="New Question from this Version"><span className="glyphicon glyphicon-file"></span> </button> 
                <button type="button" className="btn btn-default btn-sm " data-toggle="tooltip" title="New Draft from this Version"><span className="glyphicon glyphicon-pencil" ></span></button> 
                <button type="button" className="btn btn-default btn-sm " data-toggle="tooltip" title="Restore this Version"><span className="glyphicon glyphicon-repeat" ></span></button> 
               </div>);
     

    

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

        return ( <div className="version-row">
                        <div className="version-cell">
                          <span className= {version.isCurrent? "version-text current" : "version-text"}> Version of {version.modifiedDate} by {version.modifiedBy} {version.isInitial? "(initial)": ""} </span>
                        </div>     
                        <div className="version-cell menu">
                         {this.renderMenu()}
                        </div>
                 </div>
                        

           
         );
    }
});