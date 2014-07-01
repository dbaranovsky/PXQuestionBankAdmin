/**
* @jsx React.DOM
*/

var ComparedQuesion = React.createClass({displayName: 'ComparedQuesion',


	renderStub: function() {
		return (React.DOM.div(null,  " Not Shared here"));
	},

	renderQuestionContend: function() {
		return (React.DOM.div(null,  " ", this.props.data.title));
	},

	renderQuestion: function() {
		if(this.props.data.compareLocation==window.enums.сompareLocationType.bothCourses) {
			return (React.DOM.td( {colSpan:2},  " ", this.renderQuestionContend(), " " ));
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlyFirstCourse) {
			return [(React.DOM.td(null,  " ", this.renderQuestionContend(), " " )), (React.DOM.td(null, this.renderStub()))];
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlySecondCourse) {
			return [(React.DOM.td(null, this.renderStub())), (React.DOM.td(null,  " ", this.renderQuestionContend(), " " ))];
		}

		return null;

	},

    render: function() {
       return (React.DOM.tr(null,  " ", this.renderQuestion(), " " ));
    },
});




