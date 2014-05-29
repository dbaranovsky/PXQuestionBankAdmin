/**
* @jsx React.DOM  
*/  

var VersionHistory = React.createClass({displayName: 'VersionHistory',
   
    renderRows: function(){
         
     var vesrions = this.props.vesrions.map(function (version) {
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
                        "Version of ", version.modifiedDate, " by ", version.modifiedBy  
                         
                 )

           
         );
    }
});