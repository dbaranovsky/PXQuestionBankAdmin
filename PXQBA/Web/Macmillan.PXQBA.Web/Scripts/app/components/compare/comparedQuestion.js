/**
* @jsx React.DOM
*/

var ComparedQuesion = React.createClass({displayName: 'ComparedQuesion',

	renderStub: function() {
		return (React.DOM.div(null,  " Not Shared here"));
	},

	renderQuestionContend: function(template) {
		return (QuestionContent( 
				 {isExpanded:this.props.isExpanded,
				 question:this.props.data.questionMetadata,
				 questionCardTemplate:template,
				 expandPreviewQuestionHandler:this.props.expandPreviewQuestionHandler}
				 ));
	},

	renderQuestion: function() {


		if(this.props.data.compareLocation==window.enums.сompareLocationType.bothCourses) {
			return [React.DOM.td( {colSpan:2}, this.renderQuestionContend(this.props.templates.first))];
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlyFirstCourse) {
			return [React.DOM.td( {className:"compared-table-first-column"}, this.renderQuestionContend(this.props.templates.first)), React.DOM.td(null, this.renderStub())];
		}

		if(this.props.data.compareLocation==window.enums.сompareLocationType.onlySecondCourse) {
			return [React.DOM.td( {className:"compared-table-first-column"}, this.renderStub(this.props.templates.second)), React.DOM.td(null, this.renderQuestionContend())];
		}

		return null;

	},

    render: function() {
       return (React.DOM.tr( {className:"compared-table-seporator"}, this.renderQuestion()));
    },
});




