/**
* @jsx React.DOM
*/

var CourseComparerRoot = React.createClass({displayName: 'CourseComparerRoot',

   getInitialState: function() {
        return { 
            firstCourse: null,
            secondCourse: null,
            currentPage: null,
        };
    },

    changeFirstCourseHandler: function(courseId) {
        this.setState({firstCourse: courseId});
        this.getQuestions();

    },

    changeSecondCourseHandler: function(courseId) {
        this.setState({secondCourse: courseId});
        this.getQuestions(0);
    },

    isGetQuestionPossible: function() {
        if(this.state.firstCourse==null) {
            return false;
        }

        if(this.state.secondCourse==null) {
            return false;
        }

        return true;
    },

    getQuestions: function(page) {
        if(!this.isGetQuestionPossible()) {
            return;
        }


        compareTitlesDataManager.getComparedData(this.state.firstCourse, 
                                                 this.state.secondCourse,
                                                 page).done(this.getComparedDataDone);
       

    },

    paginatorClickHandle:function(page) {
        this.getQuestions(page);
    },

    getComparedDataDone: function(response) {
        this.setState({page: response.page})
        this.setState({questions: response.questions})
        this.setState({totalPages: response.totalPages})
    },

    render: function() {
       return (
            React.DOM.div(null, 
                CourseCompareSelector( {availableCourses:this.props.availableCourses, 
                                       currentFirstCourse:this.state.firstCourse,
                                       currentSecondCourse:this.state.secondCourse,
                                       changeFirstCourseHandler:this.changeFirstCourseHandler,
                                       changeSecondCourseHandler:this.changeSecondCourseHandler}
                ),

                QuestionComparerList( {questions:this.state.questions} ),
                 Paginator( {metadata:{
                            currentPage: this.state.page,
                            totalPages: this.state.totalPages}, 
                            customPaginatorClickHandle:this.paginatorClickHandle})
            )
            );
    }
});




