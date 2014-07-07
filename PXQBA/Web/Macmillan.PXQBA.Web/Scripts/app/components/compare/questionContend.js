/**
* @jsx React.DOM
*/

var QuestionContent = React.createClass({displayName: 'QuestionContent',

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
		 return (ExpandButton( {expanded:this.props.isExpanded, onClickHandler:this.expandPreviewQuestionHandler, targetCaption:"question"})); 
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
      return(React.DOM.span( {className:"badge shared-to-incompare"}, this.getTitleCount()));
    },

	renderSharedLabel: function() {
		return (React.DOM.button( {type:"button", className:"btn btn-default btn-sm custom-btn shared-to",
			     rel:"popover",  
			     'data-toggle':"popover", 
			      'data-title':this.isShared()? "Shared with:" : "",  
			      'data-content':this.isShared()? this.props.question.data[window.consts.questionSharedWithName] : "<b>Not Shared</b>"} , 
                 React.DOM.span( {className:"glyphicon icon-shared-to"} ),this.renderCourseCountBadge() 
               ));
	},
 
	renderTitle: function() {
		return(
		     React.DOM.tr( {style:{width:'100%'}}, 
		     	React.DOM.td(null, 
		     		React.DOM.table( {style:{width:'100%'}}, 
		     		 	React.DOM.tbody(null, 
		     			React.DOM.tr(null, 
                          React.DOM.td( {style:{width:'30px'}}, 
                             this.renderExpandButton()
                          ),
                          React.DOM.td(null, 
                          	 " ",
                             this.props.question.data[window.consts.questionTitleName]
                          ),
                          React.DOM.td( {style:{width:'90px'}}, 
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
				React.DOM.td( {style:{width:'100%'}},  
					React.DOM.div( {className:"compared-preview"}, 
					QuestionPreviewContent( 
					   {metadata:this.props.question.data, 
                       preview:this.props.question.data.questionHtmlInlinePreview, 
                       questionCardTemplate:this.props.questionCardTemplate}
					)
					)
				)
			)
			)
	},

     render: function() {
       return (React.DOM.div(null,  
       			 React.DOM.table( {style:{width:'100%'}}, 
       			 	React.DOM.tbody(null, 
                       this.renderTitle(),
                       this.renderPreview()
                    )
                 )
       		   ));
    },
 
 
});


