/**
* @jsx React.DOM
*/

var CourseCompareSelector = React.createClass({displayName: 'CourseCompareSelector',

	selectFirstCourseHandler: function(items) {
		this.props.changeFirstCourseHandler(items[0])
	},

	selectSecondCourseHandler: function(items) {
		this.props.changeSecondCourseHandler(items[0])
    },

    render: function() {
       return (
            React.DOM.div(null, 
                  React.DOM.table(null, 
                  	React.DOM.tr(null, 
                  		React.DOM.td(null, 
                  		    "Compare"
                  		), 
                  		React.DOM.td(null, 
               			  MetadataCourseSelector({selectCourseHandler: this.selectFirstCourseHandler, 
                                         availableCourses: this.props.availableCourses, 
                                         currentCourse: this.props.currentFirstCourse, 
                                         hideLabel: true}
                                         )
                        ), 
                        React.DOM.td(null, 
                           "with"
                        ), 
                        React.DOM.td(null, 
                    	 MetadataCourseSelector({selectCourseHandler: this.selectSecondCourseHandler, 
                                         availableCourses: this.props.availableCourses, 
                                         currentCourse: this.props.currentSecondCourse, 
                                         hideLabel: true}
                                         )
                        )
                     )

                   )
            )
            );
    }
});




