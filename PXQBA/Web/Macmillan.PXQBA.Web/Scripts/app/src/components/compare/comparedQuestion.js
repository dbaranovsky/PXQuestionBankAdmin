/**
* @jsx React.DOM
*/

var ComparedQuesion = React.createClass({

	renderStub: function() {
		return (<div> Not Shared here</div>);
	},

	renderQuestionContend: function(template) {
		return (<QuestionContent 
				 isExpanded={this.props.isExpanded}
				 question={this.props.data.questionMetadata}
				 questionCardTemplate={template}
				 expandPreviewQuestionHandler={this.props.expandPreviewQuestionHandler}
				 />);
	},

	renderQuestion: function() {
		if(this.props.data.compareLocation==window.enums.сompareLocationType.bothCourses) {
			return (<td colSpan={2}> {this.renderQuestionContend(this.props.templates.first)} </td>);
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlyFirstCourse) {
			return [(<td> {this.renderQuestionContend(this.props.templates.first)} </td>), (<td>{this.renderStub()}</td>)];
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlySecondCourse) {
			return [(<td>{this.renderStub(this.props.templates.second)}</td>), (<td> {this.renderQuestionContend()} </td>)];
		}

		return null;

	},

    render: function() {
       return (<tr> {this.renderQuestion()} </tr>);
    },
});




