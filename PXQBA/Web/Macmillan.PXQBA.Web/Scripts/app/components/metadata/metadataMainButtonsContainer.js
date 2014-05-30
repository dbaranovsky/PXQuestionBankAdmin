/**
* @jsx React.DOM
*/

var MetadataMainButtonsContainer= React.createClass({displayName: 'MetadataMainButtonsContainer',
    render: function() {
       return (
               React.DOM.div(null,  
               	React.DOM.button( {type:"button", className:"btn btn-default metadata-button"}, "Cancel"),
               	React.DOM.button( {type:"button", className:"btn btn-primary metadata-button"}, "Save")
               )
            );
    }
});




