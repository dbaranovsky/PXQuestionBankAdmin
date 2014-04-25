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

    renderChapters: function() {
       var chapters = [];
       for(var i=0; i<this.props.data.chapters.length; i++) {
          chapters.push(this.renderChapter(this.props.data.chapters[i]));
       }

       return chapters;
    },

    renderChapter: function(chapter) {
        return (<div>   
                  <a href={this.getUrlToList(this.props.data.id, chapter.id)} className="chapter-link">
                     {chapter.title} 
                  </a>
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

    render: function() {
       return (
                <div className="title-item">
                     <div>
                         <span>
                             <ExpandButton expanded={this.state.expanded} onClickHandler={this.expandHandler}/>
                          </span> 
                          <span>
                            <a href={this.getUrlToList(this.props.data.id)} className="title-link">  {this.props.data.title} </a>
                           </span>
                      </div>
                      {this.renderExpanded()}
                </div>
            );
    }
});

