/**
* @jsx React.DOM
*/

var Title = React.createClass({displayName: 'Title',

    getInitialState: function() {
        return { expanded: false };
    },

    expandHandler: function() {
       this.setState({expanded: !this.state.expanded});
    },

    getQuestionCountText: function(count) {
      if(count==1) {
        return '1 question';
      }

      return count + ' questions'
    },

    renderChapters: function() {
       var chapters = [];
       for(var i=0; i<this.props.data.chapters.length; i++) {
          chapters.push(this.renderChapter(this.props.data.chapters[i]));
       }

       return chapters;
    },

    renderChapter: function(chapter) {
       if (this.props.data.canViewQuestionList){
        return (React.DOM.div(null, 
                  React.DOM.span({className: "chapter-list-title"}, 
                    React.DOM.a({href: this.getUrlToList(this.props.data.id, chapter.id), className: "chapter-link"}, 
                       chapter.title
                    )
                  ), 
                  React.DOM.span({className: "chapter-list-count"}, 
                    this.getQuestionCountText(chapter.questionsCount)
                  )
                ));
      }

      return (React.DOM.div(null, 
                  React.DOM.span({className: "chapter-list-title"}, 
                       chapter.title
                  ), 
                  React.DOM.span({className: "chapter-list-count"}, 
                    this.getQuestionCountText(chapter.questionsCount)
                  )
                ));
    },

    renderExpanded: function() {
      if(this.state.expanded) {
          return (React.DOM.div({className: "chapters-container"}, " ", this.renderChapters()));
      }
      return null; 
    },

    getUrlToList: function(titleId, chapterId) {
      return window.actions.questionList.buildQuestionListIndexUrl(titleId, chapterId)
    },

    renderTitle: function(){

      if(this.props.data.isDraft)
      {
        return (React.DOM.span({className: "draft-course"}, this.props.data.title));
      }

       return (React.DOM.span(null, this.props.data.title));
    },

    render: function() {
      if (this.props.data.canViewQuestionList){
         return (
                React.DOM.div({className: "title-item"}, 
                     React.DOM.div(null, 
                         React.DOM.span(null, 
                             ExpandButton({expanded: this.state.expanded, onClickHandler: this.expandHandler, targetCaption: "course"})
                          ), 
                          React.DOM.span({className: "course-list-title"}, 
                            React.DOM.a({href: this.getUrlToList(this.props.data.id), className: "title-link"}, 
                                 React.DOM.span(null, 
                                       this.renderTitle()
                                 )
                            ), 
                             "  ", this.props.data.isDraft? React.DOM.span({className: "label label-default draft-label"}, "DRAFT") : ""
                           ), 
                            React.DOM.span({className: "course-list-count"}, 
                                   this.getQuestionCountText(this.props.data.questionsCount)
                            )
                      ), 
                      this.renderExpanded()
                )
            );
      }

       return (
                React.DOM.div({className: "title-item"}, 
                     React.DOM.div(null, 
                         React.DOM.span(null, 
                             ExpandButton({expanded: this.state.expanded, onClickHandler: this.expandHandler, targetCaption: "course"})
                          ), 
                          React.DOM.span({className: "course-list-title"}, 
                                  this.renderTitle(), 
                                 this.props.data.isDraft? React.DOM.span({className: "label label-default draft-label"}, "draft") : ""
                           ), 
                            React.DOM.span({className: "course-list-count"}, 
                                   this.getQuestionCountText(this.props.data.questionsCount)
                            )
                      ), 
                      this.renderExpanded()
                )
            );

      
    }
});

