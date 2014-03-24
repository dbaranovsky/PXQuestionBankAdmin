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

    renderSelector: function() {
            return (<QuestionListColumnSelectDialog columns={this.props.columns}/>);
    },


    getSelectableFields: function () {

      var allFields = this.props.allFields;
      var displayedFields =  this.props.displayedFields;

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

    columnSelectorOnClickEventHandler: function(event) {
        var field = event.target.getAttribute("data-field");
        if(field != null) {
            routsManager.addField(field);
        }
    },

    render: function() {

        var fields = this.getSelectableFields();

        var menuOptions = [];
        fields.forEach(function(questionFieldDescriptor) {
            menuOptions.push(
                <li> <a className="add-column-item" data-field={questionFieldDescriptor.metadataName}>{questionFieldDescriptor.friendlyName}</a></li>
                ); 
        }); 

        if(menuOptions.length==0) {
            menuOptions.push(<li> <div className="add-columns-message"> All columns already added. </div></li>)
        }

        return (
            <div>
              <div className="dropdown">
                    <div className="add-column-container">
                        <span data-toggle="dropdown" className="dropdown-toggle add-column-button">  <span className="glyphicon glyphicon-plus"></span> </span>
                        
                        <ul className="dropdown-menu" onClick={this.columnSelectorOnClickEventHandler}>
                            {menuOptions}
                        </ul>
                        
                    </div>
              </div>

             </div>
            );
    }
});