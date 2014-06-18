/**
* @jsx React.DOM
*/

var UserTitlesBox = React.createClass({displayName: 'UserTitlesBox',


    changeTitleHandler: function(title){
      var titles = this.props.titles;
      var newTitles = [];
      $.each(titles, function(i, item){
          if(item.titleId == title.titleId){
            newTitles.push(title);
          }else{
            newTitles.push(item);
          }
      });

      this.props.changeTitles(newTitles);
    },

    renderRows: function(){
     var self= this;

     if (this.props.loading){
        return (React.DOM.div( {className:"waiting"}));
      }

     var rows = [];
     rows = this.props.titles.map(function (userTitle, i) {

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


