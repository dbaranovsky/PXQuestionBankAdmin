/**
* @jsx React.DOM
*/

var QuestionContent = React.createClass({displayName: 'QuestionContent',


    expandPreviewQuestionHandler: function(expanded) {
       this.props.expandPreviewQuestionHandler(this.props.question.data.id, expanded)
    },

	renderExpandButton: function() {
		 return (ExpandButton({expanded: this.props.isExpanded, onClickHandler: this.expandPreviewQuestionHandler, targetCaption: "question"})); 
	},

    renderSharedLabel: function() {
      if (this.props.question.data[window.consts.questionSharedWithName].split("<br>").length < 2){
        return null;
      }
		return (SharedButton({sharedWith: this.props.question.data[window.consts.questionSharedWithName], trigger: "hover"}));
	},

	renderTitle: function() {
		return(
		     React.DOM.tr({style: {width:'100%'}}, 
		     	React.DOM.td(null, 
		     		React.DOM.table({style: {width:'100%'}}, 
		     		 	React.DOM.tbody(null, 
		     			React.DOM.tr(null, 
                          React.DOM.td({style: {width:'30px'}}, 
                             this.renderExpandButton()
                          ), 
                          React.DOM.td(null, 
                          	 " ", 
                             this.props.question.data[window.consts.questionTitleName]
                          ), 
                          React.DOM.td({style: {width:'90px'}}, 
                             this.renderSharedLabel()
                         )
                        )
                        )
                    )
                )
              ));
	},


	renderPreview: function() {
		if(!this.props.isExpanded) {
			return null;
		}
		return(
			React.DOM.tr(null, 
				React.DOM.td({style: {width:'100%'}}, 
					React.DOM.div({className: "compared-preview"}, 
					QuestionPreviewContent({
					   metadata: this.props.question.data, 
                       preview: this.props.question.data.questionHtmlInlinePreview, 
                       questionCardTemplate: this.props.questionCardTemplate}
					)
					)
				)
			)
			)
	},

     render: function() {
       return (React.DOM.div(null, 
       			 React.DOM.table({style: {width:'100%'}}, 
       			 	React.DOM.tbody(null, 
                       this.renderTitle(), 
                       this.renderPreview()
                    )
                 )
       		   ));
    },
 
 
});


