/**
* @jsx React.DOM
*/
var UserRow = React.createClass({

    showAvailibleTitlesHandler: function(){
        this.props.showAvailibleTitlesHandler(this.props.user);
    },

    editUser: function(){
        this.props.showUserEditDialog(this.props.user);
    },

    render: function() {
       return (
                <div className="role-row"> 

                 <div className="role-cell role-name">{this.props.user.fullName}</div>
                 <div className="role-cell capabilities "> <span className="capabilities-link" onClick={this.showAvailibleTitlesHandler}>{this.props.user.productCoursesCount} {this.props.user.productCoursesCount == 1? "title" : "titles"}</span></div>
                 <div className="role-cell menu">
                    <button type="button" className="btn btn-default btn-sm"  data-toggle="tooltip"  title="Edit Role" onClick={this.editUser}><span className="icon-pencil-1"></span> </button>
                 </div>
                </div>
            );
     }

});



