/**
* @jsx React.DOM
*/

var MetadataCourseSelector= React.createClass({displayName: 'MetadataCourseSelector',
  
    buildAllOptions: function() {
      var allOptions = [];
      for(var i=0; i<this.props.availableCourses.length; i++) {
        allOptions.push({
          value: this.props.availableCourses[i].id,
          text: this.props.availableCourses[i].title
        })
      }

      return allOptions;
    },

    getCurrentValues: function() {
        if(this.props.currentCourse!=null) {
          return [this.props.currentCourse];
        }
        return [];
    },

    render: function() {
       return (

               React.DOM.div( {className:"course-selector-container"},  
                    React.DOM.div(null, 
                      "Title"
                    ),
                   React.DOM.div(null,  
                      SingleSelectSelector( 
                        {allOptions:this.buildAllOptions(),
                        dataPlaceholder:"No Title",
                        onChangeHandler:this.props.selectCourseHandler,
                        currentValues:  this.getCurrentValues()}
                      )
                    )
                )
            );
    }
});




