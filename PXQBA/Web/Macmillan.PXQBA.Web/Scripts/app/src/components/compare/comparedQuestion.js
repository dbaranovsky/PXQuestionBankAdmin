/**
* @jsx React.DOM
*/

var ComparedQuesion = React.createClass({


	/*
   if(this.props.field==window.consts.questionTitleName) {
           return (<div className="cell-value"> 
                     <table className="cell-value-table">
                        <tr>
                          <td>
                             {this.renderExpandButton()}
                          </td>
                          <td>
                             {this.props.value}
                          </td>
                          <td>
                             {this.renderDraftLabel()}
                         </td>
                        </tr>
                     </table>
                    </div>);


	*/


	renderStub: function() {
		return (<div> Not Shared here</div>);
	},

	renderQuestionContend: function() {
		debugger;
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




