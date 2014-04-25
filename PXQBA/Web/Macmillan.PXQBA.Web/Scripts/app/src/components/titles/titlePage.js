/**
* @jsx React.DOM
*/

var TitlePage = React.createClass({

    getInitialState: function() {
      return { loading: false };
    },

    render: function() {
       return (
                <div>
                     <TitleList data={this.props.response.titles} />
                </div>
            );
    }
});

