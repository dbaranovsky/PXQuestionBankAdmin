/**
* @jsx React.DOM
*/

var QuestionContent = React.createClass({

    componentDidUpdate: function(){
      this.initializePopovers();
    },

    componentDidMount: function(){
      this.initializePopovers();
    },

    initializePopovers: function() {
    	$(this.getDOMNode()).popover({
                             selector: '[rel="popover"]',
                            // trigger: 'click', 
                             trigger: 'hover', 
                             placement:'bottom',           
                             html: true,
                             container: 'body'});  

    },


    expandPreviewQuestionHandler: function(expanded) {
       this.props.expandPreviewQuestionHandler(this.props.question.data.id, expanded)
    },

	renderExpandButton: function() {
		 return (<ExpandButton expanded={this.props.isExpanded} onClickHandler={this.expandPreviewQuestionHandler} targetCaption="question"/>); 
	},

 
     getTitleCount: function() {
        var isShared = true;
        var shareWith = this.props.question.data[window.consts.questionSharedWithName];
        var titleCount=0;
        if(shareWith==null) {
            isShared = false;
        }
        else {
            isShared =  shareWith != "";
        }
        if(isShared) {
            titleCount = shareWith.split("<br>").length;
        }

        return titleCount;
    },

    isShared: function(){
            var titleCount = this.getTitleCount();
            return titleCount > 0;
    },

	 renderCourseCountBadge: function(){
      if (!this.isShared()){
        return "";
      }
      return(<span className="badge shared-to-incompare">{this.getTitleCount()}</span>);
    },

	renderSharedLabel: function() {
		return (<button type="button" className="btn btn-default btn-sm custom-btn shared-to"
			     rel="popover"  
			     data-toggle="popover" 
			      data-title={this.isShared()? "Shared with:" : ""}  
			      data-content={this.isShared()? this.props.question.data[window.consts.questionSharedWithName] : "<b>Not Shared</b>"} >
                 <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()} 
               </button>);
	},
 
	renderTitle: function() {
		return(
		     <tr  style={{width:'100%'}}>
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
              </tr>);
	},


	renderPreview: function() {
		if(!this.props.isExpanded) {
			return null;
		}
		return(
			<tr> 
				<td colspan={3} style={{width:'100%'}}> 
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


