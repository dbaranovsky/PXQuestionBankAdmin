/**
* @jsx React.DOM
*/

var CourseCompareSelector = React.createClass({

	selectFirstCourseHandler: function(items) {
		this.props.changeFirstCourseHandler(items[0])
	},

	selectSecondCourseHandler: function(items) {
		this.props.changeSecondCourseHandler(items[0])
    },

    render: function() {
       return (
            <div>
                  <table>
                  	<tr>
                  		<td>
                  		    Compare
                  		</td>
                  		<td>
               			  <MetadataCourseSelector selectCourseHandler={this.selectFirstCourseHandler} 
                                         availableCourses={this.props.availableCourses}
                                         currentCourse={this.props.currentFirstCourse}
                                         hideLabel={true}
                                         />
                        </td>
                        <td>
                           with
                        </td>
                        <td>
                    	 <MetadataCourseSelector selectCourseHandler={this.selectSecondCourseHandler} 
                                         availableCourses={this.props.availableCourses}
                                         currentCourse={this.props.currentSecondCourse}
                                         hideLabel={true}
                                         />
                        </td>
                     </tr>

                   </table>
            </div>
            );
    }
});




