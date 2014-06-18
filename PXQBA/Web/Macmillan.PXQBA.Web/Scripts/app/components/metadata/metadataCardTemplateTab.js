/**
* @jsx React.DOM
*/

var MetadataCardTemplateTab = React.createClass({displayName: 'MetadataCardTemplateTab',

  cardTemplateName: "questionCardLayout",

  runPreview: function() {
      alert('run!');
  },


  changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
  },

  render: function() {
       return (
         React.DOM.div(null, 
             React.DOM.div( {className:"metadata-template-header"}, 
                  React.DOM.div( {className:"metadata-template-description"}, 
                      React.DOM.span(null,  " Edit question card HTML code Here " ),
                      React.DOM.span( {className:"metadata-dispplay-options-help"},  
                      ToltipElement( {tooltipText:"Edit question card HTML code Here"}) 
                      )
                  ),
                  React.DOM.div( {className:"metadata-template-button-container"}, 
                      React.DOM.button( {type:"button", className:"btn btn-primary",  onClick:this.runPreview}, "Preview")
                  )
             ),
       	    	React.DOM.div(null, 
                   TextAreaEditor( 
                        {classNameProps:"metadata-template-editor",
                        dataChangeHandler:this.changeHandler.bind(this, this.cardTemplateName), 
                        value:this.props.data[this.cardTemplateName]} )
 			        )
         )
            );
  }
});




