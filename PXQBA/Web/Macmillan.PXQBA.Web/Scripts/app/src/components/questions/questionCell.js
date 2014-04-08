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
      if(!this.state.editMode) {
            this.setState({ showMenu: true});
      }

    },

    mouseLeaveHandler: function() {
        this.setState({ 
                        showMenu: false,
                        editMode: this.state.editMode
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
            if(this.props.editorDescriptor.editorType==window.enums.editorType.none) {
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
                               editorDescriptor: this.props.editorDescriptor}}
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
          var cellClass = React.addons.classSet({
                 'cell-edit-mode': this.state.editMode
            });

        return ( 
                <td onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}
                    className={cellClass}
                    >

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