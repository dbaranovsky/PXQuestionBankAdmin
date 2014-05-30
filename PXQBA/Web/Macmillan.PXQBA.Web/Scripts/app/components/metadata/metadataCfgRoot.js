/**
* @jsx React.DOM
*/

var MetadataCfgRoot = React.createClass({displayName: 'MetadataCfgRoot',

    render: function() {
       return (
                React.DOM.div(null, 
                    React.DOM.div( {className:"metadata-button-holder"}, 
                      React.DOM.div( {className:"metadata-button-container"}, 
                          MetadataMainButtonsContainer(null )
                      )
                    ),
                      MetadataCourseSelector(null ),
                      MetadataTabs(null ),
                     React.DOM.div( {className:"metadata-button-holder"}, 
                       React.DOM.div( {className:"metadata-button-container"}, 
                         MetadataMainButtonsContainer(null )
                       )
                    )
                )
            );
    }
});

