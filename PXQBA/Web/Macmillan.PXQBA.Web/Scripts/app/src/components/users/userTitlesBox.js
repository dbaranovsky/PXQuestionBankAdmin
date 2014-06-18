/**
* @jsx React.DOM
*/

var UserTitlesBox = React.createClass({


    changeTitleHandler: function(title){
      var titles = this.props.titles;
      var newTitles = [];
      $.each(titles, function(i, item){
          if(item.productCourseId == title.productCourseId){
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
        return (<div className="waiting"></div>);
      }

     var rows = [];
     rows = this.props.titles.map(function (userTitle, i) {

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


