/**
* @jsx React.DOM
*/

var UserTitlesBox = React.createClass({displayName: 'UserTitlesBox',


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
        return (React.DOM.div( {className:"waiting"}));
      }

     var rows = [];
     rows = this.props.titles.productCourses.map(function (userTitle, i) {

                return (UserTitleRow( {title:userTitle, changeTitleHandler:self.changeTitleHandler} ));
               
          });

     if (rows.length == 0){
       return (React.DOM.b(null, "No titles are availible"));
     }

     return rows;

    },
  render: function(){

      return (React.DOM.div(null , 
                  React.DOM.div( {className:"roles-table role-selector"}, 
                  this.renderRows()
                  )
                     
              ));

  }
});


