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
        if(this.state.showMenu) {
            if(this.props.editorType==window.enums.editorType.none) {
                return null;
            }
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
            return (<QuestionInlineEditorBase afterEditingHandler={this.afterEditingHandler}
                    metadata={ {field: this.props.field,
                               currentValue: this.props.value,
                               questionId:  this.props.questionId,
                               editorType: this.props.editorType}}
             />);
        }

        if(this.props.previewInfo.hasPriview) {
           return ( <div>
                        <div className={this.props.previewInfo.classNameForCell}>
                        <span className="glyphicon glyphicon-chevron-right title-expander"></span>
                           {this.props.value}
                        </div>
                        <QuestionPreview preview={this.props.previewInfo.htmlPreview}/>
                    </div>
                    );
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