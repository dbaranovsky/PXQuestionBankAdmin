/**
* @jsx React.DOM
*/

var MetadataTabs = React.createClass({

    render: function() {
       return (
            <div>
              <ul className="nav nav-tabs">
                <li className="active"> 
                    <a href="#chaptersTab" data-toggle="tab">Define Chapters and Banks</a>
                </li>
                <li>
                    <a href="#metadataTab" data-toggle="tab">Add Title-Specific Metadata Fields</a>
                </li>
              </ul>

               <div className="tab-content">
                    <div className="tab-pane active" id="chaptersTab">
                        <MetadataChapterEditorTab data={this.props.data}
                            dataChangeHandler={this.props.dataChangeHandler}/>
                    </div>
                    <div className="tab-pane" id="metadataTab">
                        <MetadataMetaEditorTab data={this.props.data} 
                            metadataFieldsHandlers={this.props.metadataFieldsHandlers}
                            availableFieldTypes={this.props.availableFieldTypes}
                            />
                    </div>
                </div>
            </div>
            );
    }
});




