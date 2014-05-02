/**
* @jsx React.DOM
*/
var ShareMetadataEditorRow = React.createClass({displayName: 'ShareMetadataEditorRow',

	getInitialState: function() {
      return { isOverriden: false};
    },

   renderSharedValue: function(){
        if (this.props.question.sourceQuestion != null){
             return  (React.DOM.div( {className:"cell"}, 
                     MetadataFieldEditor( {question:this.props.question.sourceQuestion, editMode:false, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:this.props.field, title:this.props.title} )
                 ));
        }
    },

    renderSwitchControl: function(){
       if (this.props.question.sourceQuestion != null){
         return  (React.DOM.div( {className:"cell control"}, 
                      React.DOM.a( {href:""}, "Restore")
                 ));
       }
    },

    sourceEditHandler: function(sourceQuestion){
        var question = this.props.question;
        question.sourceQuestion = sourceQuestion;
        this.props.editHandler(question);
    },

    renderLocalValue: function(){
      return  (React.DOM.div( {className:"cell"}, 
                 MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:this.props.field, title:this.props.title} )
                 ));

    },

    render: function() {
    		return(   React.DOM.div( {className:"row"}, 
                        this.renderSharedValue(),
                        this.renderSwitchControl(),
                        this.renderLocalValue()
                      )
                   );

   }
});