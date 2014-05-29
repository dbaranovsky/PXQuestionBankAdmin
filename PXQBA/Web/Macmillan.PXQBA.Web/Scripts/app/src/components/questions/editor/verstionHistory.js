/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({
   
    renderRows: function(){
         
     var vesrions = this.props.vesrions.map(function (version) {
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
                        Version of {version.modifiedDate} by {version.modifiedBy}  
                         
                 </div>

           
         );
    }
});