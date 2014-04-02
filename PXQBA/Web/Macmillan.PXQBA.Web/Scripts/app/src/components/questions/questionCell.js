/**
* @jsx React.DOM
*/ 

var QuestionCell = React.createClass({

    getInitialState: function() {
        return { 
                 showMenu: false,
                 editMode: false 
               };
    },

    mouseOverHandler: function() {
            this.setState({
                         showMenu: true,
                         editMode: this.state.editMode
                         });
    },

    mouseLeaveHandler: function() {
        this.setState({ 
                        showMenu: false,
                        editMode: false
                      });
    },

    onEditClickHandler: function() {
            this.setState({ 
                        showMenu: false,
                        editMode: true
                       });
    },

    renderMenu: function() {
        //Hardcoded!
        // After implement common mechanism for editiong delete this guard
        if(this.props.field!='dlap_q_status') {
            return null;
        }
        if(this.state.showMenu) {
            return <QuestionCellMenu onEditClickHandler={this.onEditClickHandler} />
        }
        return null;
    },


    afterEditingHandler: function(){
         this.setState({ 
                showMenu: false,
                editMode: false});
    },

    renderValue: function() {

        if(this.state.editMode) {
            return (<QuestionStatusEditor afterEditingHandler={this.afterEditingHandler}
                    metadata={ {field: this.props.field,
                               currentValue: this.props.value,
                               questionId:  this.props.questionId}}
                    statusEnum={window.enums.questionStatus}
             />);
        }
        return (<div className="cell-value">{this.props.value} </div>);
    },

    render: function() {
        return ( 
                <td onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}>

                    <table>
                        <tr>
                            <td>
                            {this.renderValue()}
                            </td>
                            <td className="cell-menu-holder">
                               
                                 {this.renderMenu()}
                               
                            </td>
                        </tr>
                    </table>
                </td>
            );
        }
});