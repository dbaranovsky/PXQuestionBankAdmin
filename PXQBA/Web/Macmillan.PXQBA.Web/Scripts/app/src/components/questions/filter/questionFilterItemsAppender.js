/**
* @jsx React.DOM
*/ 

var QuestionFilterItemsAppender = React.createClass({

    getInitialState: function() {
        return { expanded: false };
    },

    onClickHandler: function() {
        this.setState({expanded: true});
    },

    getSelectableFields: function () {
        var allFields = $.grep(this.props.allFields, function(fieldDescriptor) {
				    return fieldDescriptor.filterType != window.enums.filterType.none;
				});

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
            <div>
              <div className="dropdown">
                    <div className="add-column-container">
                        <span data-toggle="dropdown" className="dropdown-toggle add-column-button">  <span className="glyphicon glyphicon-plus"></span> </span>
                        <QuestionMetadataList onClickEventHandler={this.questionMetadataListOnClickEventHandler} 
                        					  fields={this.getSelectableFields()} 
                        					  noValueLabel="All filters already added."/>
                    </div>
              </div>

             </div>
            );
    }
});