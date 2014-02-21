function InputPanel(inputContainer, waiterContainer) {

    if (!(this instanceof InputPanel)) {
        return new InputPanel(inputContainer, waiterContainer);
    }

    this.inputContainer = inputContainer;
    this.waiterContainer = waiterContainer;

    this.showControls = function() {
        this.waiterContainer.slideUp();
        this.inputContainer.slideDown();
    };

    this.showLoad = function() {
        this.waiterContainer.slideDown();
        this.inputContainer.slideUp(); 
    };

    this.hideLoad = function() {
        this.waiterContainer.slideUp();
    };
}

 