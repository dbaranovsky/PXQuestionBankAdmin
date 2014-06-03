/**
* @jsx React.DOM
*/

var TitleSpecificMetadataField = React.createClass({displayName: 'TitleSpecificMetadataField',

 	deleteHandler: function() {
 		alert('deleted');
 	},

    render: function() {
       return (
               React.DOM.tr(null,   
               		React.DOM.td(null,  " ", this.props.data.fieldName),
               		React.DOM.td(null,  " ", this.props.data.internalName),
               		React.DOM.td(null,  " ", this.props.data.fieldType),
               		React.DOM.td(null,  " ..."),
               		React.DOM.td(null,  " ..."),
               		React.DOM.td(null,    
               			 React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.deleteHandler, 
               		 				 'data-toggle':"tooltip",
               		 				  title:"Delete field"}, 
               		 				  React.DOM.span( {className:"glyphicon glyphicon-trash"}
               		 				  )
               		 				)
               		)
                )
            );
    }
});




