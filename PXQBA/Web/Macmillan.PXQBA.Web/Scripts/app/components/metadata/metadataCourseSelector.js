/**
* @jsx React.DOM
*/

var MetadataCourseSelector= React.createClass({displayName: 'MetadataCourseSelector',
  
    render: function() {
        var allOptions = [{text:'Text1', value:'v1'},{text:'Text2', value:'v2'},{text:'Text3', value:'v3'}];
       return (

               React.DOM.div( {className:"course-selector-container"},  
                    React.DOM.div(null, 
                      "Title"
                    ),
                   React.DOM.div(null,  
                      SingleSelectSelector( 
                        {allOptions:allOptions,
                        dataPlaceholder:"No Title"}
                      )
                    )
                )
            );
    }
});




