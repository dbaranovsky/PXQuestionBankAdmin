/**
* @jsx React.DOM
*/

var QuestionComparerList = React.createClass({

	getInitialState: function() {
        return { expandedQuestions: [],
                 expandedAllFirst: false,
                 expandedAllSecond: false
               };
    },

    componentWillReceiveProps: function(nextProps) {
        this.resetExpanded();
    }, 

    resetExpanded: function() {
        this.setState({ expandedQuestions: [], expandedAll: false });
    },

    changeCollection: function(item, collection, isInsert) {
        var index = $.inArray(item, collection);
        if(isInsert) {
          if (index == -1) {
              collection.push(item)
          }
        } 
        else {
           if (index != -1) {
              collection.splice(index, 1);
           }
        }
        return collection;
    },


    expandPreviewQuestionHandler: function(questionId, expanded) {
    	this.setState({expandedQuestions: this.changeCollection(
                                 questionId,
                                 this.state.expandedQuestions, 
                                 expanded)});
    },

    expandAllQuestionHandler: function(isFirst, isExpanded) {
    	var compareLocation = null;
    	if(isFirst) {
    		compareLocation=window.enums.сompareLocationType.onlyFirstCourse;
    	}
    	else {
    		compareLocation=window.enums.сompareLocationType.onlySecondCourse;
    	}
    	
        for(var i=0; i<this.props.questions.length; i++) {
        	debugger;
        	if((this.props.questions[i].compareLocation==compareLocation)||
        	 (this.props.questions[i].compareLocation==window.enums.сompareLocationType.bothCourses))
        	 {
        		this.expandPreviewQuestionHandler(this.props.questions[i].questionMetadata.data.id, isExpanded)
        	}
        }
        this.setState({expandedAll:isExpanded})
    },

    isQuestionExpanded: function(questionId) {
         return this.isItemInCollection(questionId, this.state.expandedQuestions);
    },

    
    isItemInCollection: function(item, collection) {
       var index = $.inArray(item, collection);
         if(index==-1) {
            return false;
         }
         return true;
    },




	renderHeader: function() {
		if(!this.props.compareEnabled) {
			return (	<tr>
               			 	<th> <span>Please select a title for comparision</span> </th>
               			 	<th> <span>Please select a title for comparision</span> </th>
               	        </tr>);
		}

		return  (<tr>
               	 	<th> 
               	 		 <span><ExpandButton expanded={this.state.expandedAllFirst} onClickHandler={this.expandAllQuestionHandler.bind(null, true)} targetCaption="all"/> </span> 
               	 		 <span>{this.props.firstTitleCaption}</span> 
               	   </th>
               		<th> 
               			<span> <ExpandButton expanded={this.state.expandedAllSecond} onClickHandler={this.expandAllQuestionHandler.bind(null, false)} targetCaption="all"/></span>
               			<span>{this.props.secondTitleCaption}</span> 
               		</th>
               	</tr>);
	},


    renderQuestions: function() {
		var questionsHtml =[];
		var questions = this.props.questions;
		if((questions==null)||(!this.props.compareEnabled)) {
			return questionsHtml;
		}

		for(var i=0; i<questions.length; i++) {
			debugger;
			questionsHtml.push(<ComparedQuesion data={questions[i]}
								 templates={this.props.templates}
								 expandPreviewQuestionHandler={this.expandPreviewQuestionHandler}
								 isExpanded={this.isQuestionExpanded(questions[i].questionMetadata.data.id)}
								 />);
		}
    
    	return questionsHtml;
	},

    render: function() {  
      return (<table className="table table">
      				{this.renderHeader()}
      				{this.renderQuestions()}
      		  </table>);
    }
});



