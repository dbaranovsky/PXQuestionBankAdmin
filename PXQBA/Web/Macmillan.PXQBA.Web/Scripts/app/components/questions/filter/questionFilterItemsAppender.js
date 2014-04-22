/**
* @jsx React.DOM
*/ 

var QuestionFilterItemsAppender = React.createClass({displayName: 'QuestionFilterItemsAppender',

    getInitialState: function() {
        return { expanded: false };
    },

    onClickHandler: function() {
        this.setState({expanded: true});
    },

    getSelectableFields: function () {
        var allFields = this.props.allFields;
        var filteredFields =  this.props.filteredFields;
        
        return this.excludeDisplayedFileds(allFields, filteredFields);
    },

    excludeDisplayedFileds: function(allFields, filteredFields) {
      var resultArray = allFields.filter( function( el ) {
         for(var i=0; i<filteredFields.length; i++) {
            if(filteredFields[i].field==el.metadataName) {
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
            routsManager.addFiltration(field, []);
        }
    },

    render: function() {
        return (
            React.DOM.div(null, 
              React.DOM.div( {className:"dropdown"}, 
                    React.DOM.div( {className:"add-column-container"}, 
                        React.DOM.span( {'data-toggle':"dropdown", className:"dropdown-toggle add-column-button"},   "  ",  React.DOM.span( {className:"glyphicon glyphicon-plus"}), " " ),
                        QuestionMetadataList( {onClickEventHandler:this.questionMetadataListOnClickEventHandler, 
                        					  fields:this.getSelectableFields(), 
                        					  noValueLabel:"All filters already added."})
                    )
              )

             )
            );
    }
});