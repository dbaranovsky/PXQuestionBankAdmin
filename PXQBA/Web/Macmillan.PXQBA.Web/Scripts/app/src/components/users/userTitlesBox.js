/**
* @jsx React.DOM
*/

var UserTitlesBox = React.createClass({


    changeTitleHandler: function(title){
      var titles = this.props.titles;
      var courses = titles.productCourses;
      var newCourses = [];
      $.each(courses, function(i, item){
          if(item.id == title.id){
            newCourses.push(title);
          }else{
            newCourses.push(item);
          }
      });

      titles.productCourses = newCourses;
      this.props.changeTitles(titles);
    },

    renderRows: function(){
     var self= this;

     if (this.props.loading){
        return (<div className="waiting"></div>);
      }

     var rows = [];
     rows = this.props.titles.productCourses.map(function (userTitle, i) {

                return (<UserTitleRow title={userTitle} changeTitleHandler={self.changeTitleHandler} />);
               
          });

     if (rows.length == 0){
       return (<b>No titles are availible</b>);
     }

     return rows;

    },
  render: function(){

      return (<div >
                  <div className="roles-table role-selector">
                  {this.renderRows()}
                  </div>
                     
              </div>);

  }
});


