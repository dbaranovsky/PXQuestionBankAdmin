/**
* @jsx React.DOM
*/

var MetadataChapterEditorTab = React.createClass({

  chaptersName: "chapters",
  banksName: "banks",

	changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
	},

  render: function() {
       return (
       		<div>
              <div> 
              <p>
                Each question in this title must be aligned to a specific chapter and bank (as listed here). List all chapters 
                and banks in the area below (one per line). Chapters and banks will appear to editors and instructors in the order listed.
              </p>
              </div>
               <table>
                  <tr>
                    <td>
                      <div className="metadata-multi-line-container">
                        <div className="metadata-multi-line-lable">  
                            Chapters/Modules 
                        </div>
 
                         <div>  
                            <TextAreaEditor 
                             disabled={!this.props.data.canEditMetadataValues}
                             classNameProps="metadata-multi-line-editor"
                             dataChangeHandler={this.changeHandler.bind(this, this.chaptersName)} 
                             value={this.props.data[this.chaptersName]} />
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
                             disabled={!this.props.data.canEditMetadataValues}
                            classNameProps="metadata-multi-line-editor"
                            dataChangeHandler={this.changeHandler.bind(this, this.banksName)} 
                            value={this.props.data[this.banksName]} />
                        </div>
                      </div>
                    </td>
                    </tr>
                </table>
 			   </div>
            );
  }
});




