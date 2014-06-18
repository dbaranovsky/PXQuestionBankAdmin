/**
* @jsx React.DOM
*/

var MetadataCardTemplateTab = React.createClass({

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
         <div>
             <div className="metadata-template-header">
                  <div className="metadata-template-description">
                      <span> Edit question card HTML code Here </span>
                      <span className="metadata-dispplay-options-help"> 
                      <ToltipElement tooltipText="Edit question card HTML code Here"/> 
                      </span>
                  </div>
                  <div className="metadata-template-button-container">
                      <button type="button" className="btn btn-primary"  onClick={this.runPreview}>Preview</button>
                  </div>
             </div>
       	    	<div>
                   <TextAreaEditor 
                        classNameProps="metadata-template-editor"
                        dataChangeHandler={this.changeHandler.bind(this, this.cardTemplateName)} 
                        value={this.props.data[this.cardTemplateName]} />
 			        </div>
         </div>
            );
  }
});




