/**
* @jsx React.DOM
*/

var MetadataCourseSelector= React.createClass({
  
    render: function() {
        var allOptions = [{text:'Text1', value:'v1'},{text:'Text2', value:'v2'},{text:'Text3', value:'v3'}];
       return (

               <div className="course-selector-container"> 
                    <div>
                      Title
                    </div>
                   <div> 
                      <SingleSelectSelector 
                        allOptions={allOptions}
                        dataPlaceholder="No Title"
                      />
                    </div>
                </div>
            );
    }
});




