/**
* @jsx React.DOM
*/ 

var Question = React.createClass({

    getInitialState: function() {
        return { showMenu: false };
    },

    mouseOverHandler: function() {
        this.setState({ showMenu: true });
    },

    mouseLeaveHandler: function() {
        this.setState({ showMenu: false });
    },

    renderMenu: function() {
        if(this.state.showMenu) {
            var questionId = this.props.metadata.data["id"];
            return <QuestionListMenu
                        questionId={questionId} 
                        copyQuestionHandler={this.props.menuHandlers.copyQuestionHandler.bind(null, questionId)}
                        editQuestionHandler={this.props.menuHandlers.editQuestionHandler.bind(null, questionId)}
                        editNotesHandler={this.props.menuHandlers.editNotesHandler.bind(null, questionId)}
                        />
        }

        return null;
    },

    selectQuestionHandler: function(event) {
        var quesionId = this.props.metadata.data.id;
        var isSelected = event.target.checked;
        this.props.selectQuestionHandler(quesionId, isSelected);
    },

    renderCell: function(metadataName, editorDescriptor, allowedEdit) {
        // Hardcoded: only title has a preview
        // Refactor this after re-implement preview
        var previewInfo = { hasPriview: false };

        if(metadataName=='dlap_title') {
            previewInfo.hasPriview = true;
            previewInfo.classNameForCell = "title";
            previewInfo.htmlPreview = this.props.metadata.data.questionHtmlInlinePreview;
        }
        return ( <QuestionCell value={this.props.metadata.data[metadataName]}
                               field={metadataName} 
                               questionId={this.props.metadata.data.id}
                               editorDescriptor={editorDescriptor}
                               allowedEdit = {allowedEdit}
                               previewInfo={previewInfo}
                                />);
    },

    render: function() {
        var self = this;
        var componentClass = React.addons.classSet({
                'question': true,
                'hover': this.state.showMenu,
                'question-selected': this.props.selected
            });
        
        var cells = this.props.columns.map(function(descriptor) {
                return self.renderCell(descriptor.metadataName, descriptor.editorDescriptor, descriptor.isInlineEditingAllowed)
            });

        return ( 
            <tr className={componentClass} 
                    onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}>
                <td> 
                    <input type="checkbox" checked={this.props.selected} onChange={this.selectQuestionHandler}/>
                </td>
                 {cells}
                 <td className="actions">  
                   <div className="actions-container">
                        {this.renderMenu()}
                   </div>
                </td>  
            </tr> 
            );
        }
});