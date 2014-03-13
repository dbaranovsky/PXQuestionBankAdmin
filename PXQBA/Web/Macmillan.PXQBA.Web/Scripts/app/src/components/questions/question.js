/**
* @jsx React.DOM
*/ 

var Question = React.createClass({
	render: function() {

		return ( 
			<tr className="question">
				<td> 
					<input type="checkbox"/>
				</td>

				<td className="eBookChapter">
					{this.props.metadata.eBookChapter}
				</td>

				<td className="questionBank">
				    {this.props.metadata.questionBank}
				</td>

				<td className="questionSeq">
					{this.props.metadata.questionSeq}
				</td>

				<td className="title">
				    <div>
				    <span className="glyphicon glyphicon-chevron-right"></span>
				    {this.props.metadata.title}
				    </div>
				    <QuestionPreview preview={this.props.questionHtmlInlinePreview}/>
				    
				</td>

				<td className="questionType">
					{this.props.metadata.questionType}
				</td>

			    <td className="actions">  
				
				</td>  
			</tr> 
			);
		}
});