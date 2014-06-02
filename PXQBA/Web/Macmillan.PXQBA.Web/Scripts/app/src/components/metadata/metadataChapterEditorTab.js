/**
* @jsx React.DOM
*/

var MetadataChapterEditorTab = React.createClass({

	changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
	},

  render: function() {
       return (
       		<div>
               <table>
                  <tr>
                    <td>
                      <div className="metadata-multi-line-container">
                        <div className="metadata-multi-line-lable">  
                            Chapters/Modules 
                        </div>
 
                         <div>  
                            <TextAreaEditor 
                             classNameProps="metadata-multi-line-editor"
                              dataChangeHandler={this.changeHandler.bind(this, "chapters")} 
                              value={this.props.data.chapters} />
                         </div>
                      </div>
                    </td>
                    <td>
                      <div className="metadata-multi-line-container">
                        <div className="metadata-multi-line-lable"> 
                             Banks
                        </div>

                        <div>  
                           <TextAreaEditor 
                            classNameProps="metadata-multi-line-editor"
                            dataChangeHandler={this.changeHandler.bind(this, "banks")} 
                            value={this.props.data.banks} />
                        </div>
                      </div>
                    </td>
                    </tr>
                </table>
 			   </div>
            );
  }
});




