/**
* @jsx React.DOM
*/

var MetadataCfgRoot = React.createClass({

   getInitialState: function() {
        return { metadataConfigViewModelOriginal: null,
                 metadataConfigViewModelDirty: false,
                 dataLoading: false,
                 currentCourse: null,
                 dirty: false,
               };
    },


   componentDidMount: function() {
       if((this.props.options.courseId!=null)&&(this.props.options.courseId!='')) {
          this.selectCourseHandler([this.props.options.courseId]);
       }
   },

   confirmDiscardChanges: function () {
    if(!this.state.dirty) {
      return true;
    }

    if (confirm("You have unsaved changes on this title. Do you want discard your changes?")){
          return true;
       }
     return false;
   },



   // Handlers

   addMetadataFieldFieldHandler: function() {
        var viewModel = $.extend(true, {}, this.state.metadataConfigViewModelDirty);
        viewModel.fields.push({fieldType:0});
        this.setState({
                        metadataConfigViewModelDirty: viewModel,
                        dirty: true
                        });
   },

   deleteMetadataFieldHandler: function(index) {
        var viewModel = $.extend(true, {}, this.state.metadataConfigViewModelDirty);
        viewModel.fields.splice(index, 1);
        this.setState({
                        metadataConfigViewModelDirty: viewModel,
                        dirty: true
                        });
   },

   updateMetadataFieldHandler: function(index, fieldName, value) {
        var viewModel = $.extend(true, {}, this.state.metadataConfigViewModelDirty);
        viewModel.fields[index][fieldName] = value;
        this.setState({
                        metadataConfigViewModelDirty: viewModel,
                        dirty: true
        });
   },

   selectCourseHandler: function(items) {
    if(!this.confirmDiscardChanges()) {
      this.forceUpdate()
      return;
    }
      var value = items[0];
      this.setState({dataLoading: true});
      metadataCfgDataManager.getMetadataConfig(value).done(this.getMetadataConfigHandler);
   },
    
   getMetadataConfigHandler: function(metadataConfigViewModel) {
     this.setState({
                    metadataConfigViewModelOriginal: metadataConfigViewModel,
                    metadataConfigViewModelDirty: $.extend(true, {}, metadataConfigViewModel),
                    currentCourse: metadataConfigViewModel.courseId,
                    dirty: false
                  });
     this.setState({dataLoading: false})
   },

   changeViewModelHandler: function(updatedViewModel) {
       this.setState({metadataConfigViewModelDirty: updatedViewModel, 
                      dirty: true})
   },

   cancelEditConfigHandler: function() {
      if(!this.confirmDiscardChanges()) {
          return;
      }
      notificationManager.showWarning('Metadata configuration restored');
      this.setState({metadataConfigViewModelDirty: $.extend(true, {}, this.state.metadataConfigViewModelOriginal),
                     dirty: false})
   },

   saveEditConfigHandler: function() {
        this.setState({dataLoading: true});
        metadataCfgDataManager.saveMetadataConfig(this.state.metadataConfigViewModelDirty)
                                                 .done(this.saveSuccessHandler)
                                                 .error(this.saveErrorHandler);
   },

   saveSuccessHandler: function() {
       this.setState({dataLoading: false});
       notificationManager.showSuccess('Metadata configuration successfully updated');
       this.setState({
                    metadataConfigViewModelOriginal: $.extend(true, {}, this.state.metadataConfigViewModelDirty),
                    dirty: false
                  });
   },

   saveErrorHandler: function() {
      this.setState({dataLoading: false});
      notificationManager.showDanger('Error occurred, please try again later');
   },

   // Renders

   renderLoader: function() {
      if(this.state.dataLoading) {
        return (<Loader />)
      }
      
      return null;
   },

   renderMetadataTabs: function() {
     if(this.state.currentCourse!=null) {
        return ( <MetadataTabs 
                  data={this.state.metadataConfigViewModelDirty}
                  dataChangeHandler={this.changeViewModelHandler}
                  metadataFieldsHandlers={{
                      addHandler: this.addMetadataFieldFieldHandler,
                      deleteHandler: this.deleteMetadataFieldHandler,
                      updateHandler: this.updateMetadataFieldHandler
                    }}
                  availableFieldTypes={this.state.metadataConfigViewModelDirty.availableFieldTypes}
                  />);
      }
      return null;
   },


   renderButtons: function() {
     if(this.state.currentCourse!=null) {
        return (<MetadataMainButtonsContainer 
                    saveHandler={this.saveEditConfigHandler}
                    cancelHandler={this.cancelEditConfigHandler}
                />);
      }
      return null;
   },

    render: function() {
       return (
                <div>
                    <div className="metadata-button-holder">
                      <div className="metadata-button-container">
                           {this.renderButtons()}
                      </div>
                    </div>
                      <MetadataCourseSelector selectCourseHandler={this.selectCourseHandler} 
                                              availableCourses={this.props.availableCourses}
                                              currentCourse={this.state.currentCourse}
                                              />
                       {this.renderLoader()}
                       {this.renderMetadataTabs()}
                     <div className="metadata-button-holder">
                       <div className="metadata-button-container">
                          {this.renderButtons()}
                       </div>
                    </div>
                </div>
            );
    }
});

