/**
* @jsx React.DOM
*/

var ComparedQuesion = React.createClass({


	renderStub: function() {
		return (<div> Not Shared here</div>);
	},

	renderQuestionContend: function() {
		return (<div> {this.props.data.title}</div>);
	},

	renderQuestion: function() {
		if(this.props.data.compareLocation==window.enums.сompareLocationType.bothCourses) {
			return (<td colSpan={2}> {this.renderQuestionContend()} </td>);
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlyFirstCourse) {
			return [(<td> {this.renderQuestionContend()} </td>), (<td>{this.renderStub()}</td>)];
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlySecondCourse) {
			return [(<td>{this.renderStub()}</td>), (<td> {this.renderQuestionContend()} </td>)];
		}

		return null;

	},

    render: function() {
       return (<tr> {this.renderQuestion()} </tr>);
    },
});




