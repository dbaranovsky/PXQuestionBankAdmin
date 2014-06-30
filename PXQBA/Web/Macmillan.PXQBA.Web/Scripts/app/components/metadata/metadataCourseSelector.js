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

    renderLabel: function() {
      if(this.props.hideLabel) {
        return null;
      }
       return (React.DOM.div(null,  " Title " ));
    },

    render: function() {
       return (

               React.DOM.div( {className:"course-selector-container"},  
                    this.renderLabel(),
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




