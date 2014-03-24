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

    renderPreview: function(fiedl) {
        return (
                <td className="title">
                    <div>
                    <span className="glyphicon glyphicon-chevron-right title-expander"></span>
                    {this.props.metadata.data[fiedl]}
                    </div>
                       <QuestionPreview preview={this.props.metadata.data.questionHtmlInlinePreview}/>
                </td>);
    },
 
    renderCell: function(field) {
        if(field=='dlap_title') {
            return this.renderPreview(field);
        }
        return (<td>
                    {this.props.metadata.data[field]}
                </td>);
    },

    render: function() {
        var componentClass = React.addons.classSet({
                'question': true,
                 hover: this.state.showMenu
        });
        
        var renderCell = this.renderCell;

        var cells = this.props.columns.map(function(descriptor)
            {
                return renderCell(descriptor.metadataName)
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