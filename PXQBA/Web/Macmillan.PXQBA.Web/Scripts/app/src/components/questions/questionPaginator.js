/**
* @jsx React.DOM
*/ 

var QuestionPaginator = React.createClass({

    paginatorInitializer: function (container) {
        var options = {
            currentPage: this.props.metadata.currentPage,
            totalPages: this.props.metadata.totalPages,
            onPageChanged: function(event, oldPage, newPage) {
                routsManager.setPage(newPage);
            }
        };

        container.bootstrapPaginator(options);
    },

    componentDidMount: function() {
         this.paginatorInitializer($(this.getDOMNode()));
    },

    componentDidUpdate: function () {
        this.paginatorInitializer($(this.getDOMNode()));
    },

    render: function() {
        return ( 
               <div className="questionPaginator"> 
               </div>
            );
    }
});