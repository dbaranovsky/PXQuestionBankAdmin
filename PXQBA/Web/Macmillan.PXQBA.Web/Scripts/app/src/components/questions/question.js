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
            return <QuestionListMenu />
        }

        return null;
    },

    renderCell: function(field) {
        // Hardcoded: only title has a preview
        // Refactor this after re-implement preview
        if(field=='dlap_title') {
            return (<QuestionCellWithPreview value={this.props.metadata.data[field]} 
                    classNameValue="title"
                    htmlPreview={this.props.metadata.data.questionHtmlInlinePreview}/>);
        }
        return ( <QuestionCell value={this.props.metadata.data[field]} field={field} questionId={this.props.metadata.data.id} />);
    },

    render: function() {
        var self = this;

        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
            });
        
        var cells = this.props.columns.map(function(descriptor) {
                return self.renderCell(descriptor.metadataName)
            });

        return ( 
            <tr className={componentClass} 
                    onMouseOver={this.mouseOverHandler}
                    onMouseLeave={this.mouseLeaveHandler}>
                <td> 
                    <input type="checkbox"/>
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