/**
* @jsx React.DOM
*/

var QuestionContent = React.createClass({


    expandPreviewQuestionHandler: function(expanded) {
       this.props.expandPreviewQuestionHandler(this.props.question.data.id, expanded)
    },

	renderExpandButton: function() {
		 return (<ExpandButton expanded={this.props.isExpanded} onClickHandler={this.expandPreviewQuestionHandler} targetCaption="question"/>); 
	},

    renderSharedLabel: function() {
      if (this.props.question.data[window.consts.questionSharedWithName].split("<br>").length < 2){
        return null;
      }
		return (<SharedButton sharedWith={this.props.question.data[window.consts.questionSharedWithName]} trigger='hover'/>);
	},

	renderTitle: function() {
		return(
		     <tr style={{width:'100%'}}>
		     	<td>
		     		<table style={{width:'100%'}}>
		     		 	<tbody>
		     			<tr>
                          <td style={{width:'30px'}}>
                             {this.renderExpandButton()}
                          </td>
                          <td>
                          	 &nbsp;
                             {this.props.question.data[window.consts.questionTitleName]}
                          </td>
                          <td style={{width:'90px'}}>
                             {this.renderSharedLabel()}
                         </td>
                        </tr>
                        </tbody>
                    </table>
                </td>
              </tr>);
	},


	renderPreview: function() {
		if(!this.props.isExpanded) {
			return null;
		}
		return(
			<tr> 
				<td style={{width:'100%'}}> 
					<div className="compared-preview">
					<QuestionPreviewContent 
					   metadata={this.props.question.data} 
                       preview={this.props.question.data.questionHtmlInlinePreview} 
                       questionCardTemplate={this.props.questionCardTemplate}
					/>
					</div>
				</td>
			</tr>
			)
	},

     render: function() {
       return (<div> 
       			 <table style={{width:'100%'}}>
       			 	<tbody>
                       {this.renderTitle()}
                       {this.renderPreview()}
                    </tbody>
                 </table>
       		   </div>);
    },
 
 
});


