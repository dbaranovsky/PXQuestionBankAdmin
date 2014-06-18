/**
* @jsx React.DOM
*/

var MetadataCardTemplateTab = React.createClass({displayName: 'MetadataCardTemplateTab',

  cardTemplateName: "questionCardLayout",


  getInitialState: function() {
        return {
          showPreviewCardTemplateDialog: false,
          cardHtml: ""
        };
  },

  createDummyObject: function() {
      var dummyObject = {};
      var curlyBracesPattern = /[^{}]+(?=\})/g;
      var results = this.props.data[this.cardTemplateName].match(curlyBracesPattern);
      if(results==null) {
        return dummyObject;
      }
      var charactersPattern = /^\w+$/;
      var properties=[];

      for(var i=0; i<results.length; i++) {
        if(charactersPattern.test(results[i])) {
          properties.push(results[i]);
        }
      }

      for(var i=0; i<properties.length; i++) {
        dummyObject[properties[i]]=properties[i];
      }

      return dummyObject;
  },

  runPreview: function() {
     try {
         var dummyObject = this.createDummyObject();
         var template = Handlebars.compile(this.props.data[this.cardTemplateName]);
         var cardHtml = template(dummyObject);
          
         this.setState({
              showPreviewCardTemplateDialog: true,
              cardHtml: cardHtml
         });
     }
     catch (exception) {
        notificationManager.showDanger("There is a syntax error on template: " +exception.message)
     }
  },


  changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
  },

  closePreviewCardTemplateDialogHandler: function() {
     this.setState({
                showPreviewCardTemplateDialog: false,
                cardHtml: ""
      });
  },

  renderPreviewCardTemplateDialog: function() {
    if(this.state.showPreviewCardTemplateDialog) {
      return (PreviewCardTemplateDialog( {closeDialogHandler:this.closePreviewCardTemplateDialogHandler, 
                                  cardHtml:this.state.cardHtml} 
                                  ));
    }

    return null;
  },

  render: function() {
       return (
         React.DOM.div(null, 
             React.DOM.div(null, 
              this.renderPreviewCardTemplateDialog()
             ),
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




