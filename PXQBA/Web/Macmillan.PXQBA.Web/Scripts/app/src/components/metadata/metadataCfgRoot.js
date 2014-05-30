/**
* @jsx React.DOM
*/

var MetadataCfgRoot = React.createClass({

    render: function() {
       return (
                <div>
                    <div className="metadata-button-holder">
                      <div className="metadata-button-container">
                          <MetadataMainButtonsContainer />
                      </div>
                    </div>
                      <MetadataCourseSelector />
                      <MetadataTabs />
                     <div className="metadata-button-holder">
                       <div className="metadata-button-container">
                         <MetadataMainButtonsContainer />
                       </div>
                    </div>
                </div>
            );
    }
});

