/**
* @jsx React.DOM
*/ 

var QuestionListColumnAppender = React.createClass({displayName: 'QuestionListColumnAppender',

    getInitialState: function() {
        return { expanded: false };
    },

    onClickHandler: function() {
        this.setState({expanded: true});
    },

    renderSelector: function() {
        return (QuestionListColumnSelectDialog( {columns:this.props.columns}));
    },


    getSelectableFields: function () {
        var allFields = this.props.allFields;
        var displayedFields =  this.props.displayedFields;
        
        return this.excludeDisplayedFileds(allFields, displayedFields);
    },

    excludeDisplayedFileds: function(allFields, displayedFields) {
      var resultArray = allFields.filter( function( el ) {
         for(var i=0; i<displayedFields.length; i++) {
            if(displayedFields[i].metadataName==el.metadataName) {
              return false;
            }
         }
         return true;
      });

      return resultArray;
    },

    questionMetadataListOnClickEventHandler: function(event) {
        var field = event.target.getAttribute("data-field");
        if(field != null) {
            routsManager.addField(field);
        }
    },

    render: function() {
        return (
            React.DOM.div(null, 
              React.DOM.div( {className:"dropdown"}, 
                    React.DOM.div( {className:"add-column-container"}, 
                        React.DOM.span( {'data-toggle':"dropdown", className:"dropdown-toggle add-column-button"},   "  ",  React.DOM.span( {className:"glyphicon glyphicon-plus"}), " " ),
                        QuestionMetadataList( {onClickEventHandler:this.questionMetadataListOnClickEventHandler, fields:this.getSelectableFields()} )
                    )
              )

             )
            );
    }
});