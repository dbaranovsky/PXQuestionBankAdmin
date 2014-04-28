/**
* @jsx React.DOM
*/ 

var QuestionListColumnAppender = React.createClass({

    getInitialState: function() {
        return { expanded: false };
    },

    onClickHandler: function() {
        this.setState({expanded: true});
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
            <div>
              <div className="dropdown">
                    <div className="add-column-container" >
                        <span data-toggle="dropdown" className="dropdown-toggle add-column-button">  <span className="glyphicon glyphicon-plus" data-toggle="tooltip" title="Add column"></span> </span>
                        <QuestionMetadataList onClickEventHandler={this.questionMetadataListOnClickEventHandler} fields={this.getSelectableFields()} noValueLabel='All columns already added.'/>
                    </div>
              </div>

             </div>
            );
    }
});