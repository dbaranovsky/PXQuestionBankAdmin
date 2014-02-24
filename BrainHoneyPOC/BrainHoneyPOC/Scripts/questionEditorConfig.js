function configureUrl(enrollmentId, questionId, itemId) {
    var url = "http://root.qa.brainhoney.bfwpub.com/BrainHoney/Component/QuestionEditor?enrollmentid=[EntityId]&itemid=[ItemId]&questionid=[QuestionId]&showcancel=true"

    return url.replace("[EntityId]", enrollmentId)
                  .replace("[ItemId]", itemId)
                  .replace("[QuestionId]", questionId);
}

function loadQuestionEditor(url) {

    rpc = new easyXDM.Rpc({
        //Standalone component URL for the BrainHoney component to display
        remote: url,
        //Name of the <div> or other element that will contain the iframe
        container: 'contentFrame',
        //HTML props for the created iframe
        props: {
            frameborder: 0,
            height: '100%',
            width: '100%'
        }
    }, {
        //Your half of the communication implementation
        local: {
            // Perform any initialization, like registering for events
            init: function (successFn, errorFn) {
                inputPannel.hideLoad();
                rpc.addListeners('componentsaved|componentcancelled');
                if (successFn) {
                    successFn();
                }
            },
            // Handle any events 
            onEvent: function (event) {
                switch (event) {
                    //Handle each event using the 'arguments' variable           
                    case 'componentsaved':
                        if (arguments[1] == 'itemeditor' &&
                       arguments[2] == 'FrameContent') {
                            alert('componentsaved: ' + arguments[3].itemId);
                        }
                        break;
                    case 'componentcancelled':
                        if (arguments[1] == 'itemeditor' &&
                       arguments[2] == 'FrameContent') {
                            alert('componentcancelled');
                        }
                        break;
                }
            }
        },
        //Define the Frame API stub interface
        remote: {
            addListeners: {},
            fireEvent: {},
            saveComponent: {},
            navigate: {},
            getProperties: {},
            getComponentState: {},
            callComponentMethod: {},
            hasRight: {},
            setShowBeforeUnloadPrompts: {}
        }
    });
}




