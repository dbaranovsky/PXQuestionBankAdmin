var pxRenderingHelper = {
    controller: {
        init: function () {

        },
        
        generateWidgetModel: function (app, folder, data, options) {
            
            var contentData = {
                viewPath: data.viewPath,
                viewModel: data.viewModel,
                viewModelType: data.viewModelType,
                viewData: JSON.stringify(data.viewData) 
            };

            return PxViewRender.RenderView( app, folder, contentData, "POST", options);
        }
    }
}