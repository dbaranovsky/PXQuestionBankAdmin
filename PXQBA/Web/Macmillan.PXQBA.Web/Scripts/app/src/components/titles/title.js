/**
* @jsx React.DOM
*/

var Title = React.createClass({

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
        return (<div>   
                  <span className="chapter-list-title">
                    <a href={this.getUrlToList(this.props.data.id, chapter.id)} className="chapter-link">
                       {chapter.title} 
                    </a>
                  </span>
                  <span className="chapter-list-count">
                    {this.getQuestionCountText(chapter.questionsCount)}
                  </span>
                </div>);
      }

      return (<div>   
                  <span className="chapter-list-title">
                       {chapter.title} 
                  </span>
                  <span className="chapter-list-count">
                    {this.getQuestionCountText(chapter.questionsCount)}
                  </span>
                </div>);
    },

    renderExpanded: function() {
      if(this.state.expanded) {
          return (<div className="chapters-container"> {this.renderChapters()}</div>);
      }
      return null; 
    },

    getUrlToList: function(titleId, chapterId) {
      return window.actions.questionList.buildQuestionListIndexUrl(titleId, chapterId)
    },

    renderTitle: function(){

      if(this.props.data.isDraft)
      {
        return (<span className="draft-course">{this.props.data.title}</span>);
      }

       return (<span>{this.props.data.title}</span>);
    },

    render: function() {
      if (this.props.data.canViewQuestionList){
         return (
                <div className="title-item">
                     <div>
                         <span>
                             <ExpandButton expanded={this.state.expanded} onClickHandler={this.expandHandler} targetCaption="course"/>
                          </span> 
                          <span className="course-list-title">
                            <a href={this.getUrlToList(this.props.data.id)} className="title-link">  
                                 <span>  
                                       {this.renderTitle()}
                                 </span>
                            </a>
                           </span>
                            <span className="course-list-count">
                                   {this.getQuestionCountText(this.props.data.questionsCount)}
                            </span>
                      </div>
                      {this.renderExpanded()}
                </div>
            );
      }

       return (
                <div className="title-item">
                     <div>
                         <span>
                             <ExpandButton expanded={this.state.expanded} onClickHandler={this.expandHandler} targetCaption="course"/>
                          </span> 
                          <span className="course-list-title">
                                  {this.renderTitle()}
                           
                           </span>
                            <span className="course-list-count">
                                   {this.getQuestionCountText(this.props.data.questionsCount)}
                            </span>
                      </div>
                      {this.renderExpanded()}
                </div>
            );

      
    }
});

