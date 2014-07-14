/**
* @jsx React.DOM
*/

var AvailibleValuesDialog = React.createClass({

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Available values";
        };
      
 
        var renderBody = function(){
          if(self.props.fieldType==window.enums.metadataFieldType.itemLink) {
              return (<AvailibleValuesBodyItemLinks 
                           closeDialogHandler={self.props.closeDialogHandler} 
                           value={self.props.value} 
                           itemIndex={self.props.itemIndex}
                           updateHandler={self.props.updateHandler}
                           fieldNameCaption={self.props.fieldNameCaption}
                           canEdit={self.props.canEdit}
                        />);
          }
          
          return (<AvailibleValuesBodyTextArea 
                           closeDialogHandler={self.props.closeDialogHandler} 
                           value={self.props.value} 
                           itemIndex={self.props.itemIndex}
                           updateHandler={self.props.updateHandler}
                           fieldNameCaption={self.props.fieldNameCaption}
                           fieldType={self.props.fieldType}
                           canEdit={self.props.canEdit}
          />);
          };


        return (<ModalDialog showOnCreate={true}
                             renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             closeDialogHandler = {this.props.closeDialogHandler}
                             dialogId="availibleValuesDialog"/>);
    }
});



 