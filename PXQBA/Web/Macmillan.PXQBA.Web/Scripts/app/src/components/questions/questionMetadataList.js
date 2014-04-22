/**
* @jsx React.DOM
*/

var QuestionMetadataList = React.createClass({

    renderOption: function(questionFieldDescriptor) {
         return ( <li> <a className="add-column-item" data-field={questionFieldDescriptor.metadataName}>
                        {questionFieldDescriptor.friendlyName}</a>
                  </li>
                );
    },

    renderStub: function() {
        return (<li> <div className="add-columns-message">{this.props.noValueLabel}</div></li>);
    },

    onClickEventHandler: function(event) {
        this.props.onClickEventHandler(event);
    },

    renderMenuOption: function() {
        var self = this;
        options = this.props.fields.map(function(questionFieldDescriptor) {
                                    return self.renderOption(questionFieldDescriptor);
                                 });
        
        if(options.length==0) {
            options.push(this.renderStub());
        }
        return options;
    },

    render: function() {
         return (
                 <ul className="dropdown-menu" onClick={this.onClickEventHandler}>
                            {this.renderMenuOption()}
                 </ul>
            );
    }
});
