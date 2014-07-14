/**
* @jsx React.DOM
*/

var TitleSelector = React.createClass({

  

    getQuestionCountText: function(count) {
      if(count==1) {
        return '1 question';
      }

      return count + ' questions'
    },

  
    renderTitle: function(){

      if(this.props.data.isDraft)
      {
        return (<span className="draft-course">{this.props.data.title}</span>);
      }

       return (<span>{this.props.data.title}</span>);
    },


    selectTitleHandler: function(){
      this.props.selectTitleHandler(this.props.data.id);
    },


    render: function() {
         return (
                <div className="title-item">
                     <div>
                          <span className="course-list-title">
                            <a href="javascript:void(0)" className="title-link" onClick={this.selectTitleHandler}>  
                                 <span>  
                                       {this.renderTitle()}
                                 </span>
                            </a>
                             &nbsp; {this.props.data.isDraft? <span className="label label-default draft-label">draft</span> : ""}
                           </span>
                            <span className="course-list-count">
                                   {this.getQuestionCountText(this.props.data.questionsCount)}
                            </span>
                      </div>
                </div>
            );
    }
});

