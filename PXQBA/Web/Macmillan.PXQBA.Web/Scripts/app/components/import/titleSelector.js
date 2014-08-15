/**
* @jsx React.DOM
*/

var TitleSelector = React.createClass({displayName: 'TitleSelector',

  

    getQuestionCountText: function(count) {
      if(count==1) {
        return '1 question';
      }

      return count + ' questions'
    },

  
    renderTitle: function(){

      if(this.props.data.isDraft)
      {
        return (React.DOM.span({className: "draft-course"}, this.props.data.title));
      }

       return (React.DOM.span(null, this.props.data.title));
    },


    selectTitleHandler: function(){
      this.props.selectTitleHandler(this.props.data.id);
    },


    render: function() {
         return (
                React.DOM.div({className: "title-item"}, 
                     React.DOM.div(null, 
                          React.DOM.span({className: "course-list-title"}, 
                            React.DOM.a({href: "javascript:void(0)", className: "title-link", onClick: this.selectTitleHandler}, 
                                 React.DOM.span(null, 
                                       this.renderTitle()
                                 )
                            ), 
                             "  ", this.props.data.isDraft? React.DOM.span({className: "label label-default draft-label"}, "DRAFT") : ""
                           ), 
                            React.DOM.span({className: "course-list-count"}, 
                                   this.getQuestionCountText(this.props.data.questionsCount)
                            )
                      )
                )
            );
    }
});

