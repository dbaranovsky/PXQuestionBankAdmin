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
        this.setState({ showMenu: false});
    },

    onEditClickHandler: function() {
            this.setState({ 
                        showMenu: false,
                        editMode: true
                       });
    },

    renderMenu: function() {
        if(this.state.showMenu) {
            if((this.props.editorDescriptor.editorType==window.enums.editorType.none)||
              (!this.props.allowedEdit)) {
                return null;
            }
            return <QuestionCellMenu onEditClickHandler={this.onEditClickHandler} />
        }
        return null;
    },


    afterEditingHandler: function(){
         this.setState({ 
                        showMenu: false,
                        editMode: false
                      });
    },

    expandPreviewQuestionHandler: function(expanded) {
       this.props.expandPreviewQuestionHandler(this.props.questionId, expanded)
    },

    renderExpandButton: function() {
         return (<ExpandButton expanded={this.props.expanded} onClickHandler={this.expandPreviewQuestionHandler} targetCaption="question"/>);  
    },

    renderDraftLabel: function() {
      if(this.props.draft) {
        return (<span className="label label-default draft-label">draft</span>)
      }
      return null;
    },

    renderValue: function() {
        if(this.state.editMode) {
            return (<QuestionInlineEditorBase afterEditingHandler={this.afterEditingHandler}
                    metadata={ {field: this.props.field,
                               currentValue: this.props.value,
                               questionId: this.props.questionId,
                               editorDescriptor: this.props.editorDescriptor,
                               draft: this.props.draft
                             }}
             />);
        }
         
        if(this.props.field==window.consts.questionTitleName) {
           return (<div className="cell-value"> 
                     <table className="cell-value-table">
                        <tr>
                          <td>
                             {this.renderExpandButton()}
                          </td>
                          <td>
                             {this.props.value}
                          </td>
                          <td>
                             {this.renderDraftLabel()}
                         </td>
                        </tr>
                     </table>
                    </div>);
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