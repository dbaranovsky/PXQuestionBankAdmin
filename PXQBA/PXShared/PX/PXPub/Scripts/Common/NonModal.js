// Implements non modal window to show any html content on the page.
// PxNonmodal window also can be shown from any anchor tag by setting class as 'nonmodal-link'.
// params:
// Title - title to non modal window
// onCompleted - Function to execute when non modal window was shown
// onResize - Function to execute when window is resized.
(function ($) {
    var defaults = {
        title: '',
        widthOverride: '',
        heightOverride: '',
        blockUI: false,
        center: false,
        centerFaceplate: false,
        onCompleted: function () { },
        onClosed: function () { },
        onResize: function () { },
        topOverride:''
    };
    var pluginName = "PxNonModal";
    var dataKey = "NonModal";
    var nonmodal = null;

    _initNonModal = function () {
        $("body:not(:has(#nonmodal))").append('<div id="nonmodal"><div id="nonmodal-window" class="nonmodal-window"></div></div>');

        nonmodal = $("#nonmodal");
        nonmodal.hide();
    };

    _setUpNonModal = function () {
        var data = nonmodal.data(dataKey);

        if (data.settings.center) {
            var opts = {
                message: $('#nonmodal')
            };
            $.blockUI(opts);
        }

        var fneWindow = nonmodal.children('#nonmodal-window');
        fneWindow.html('');
        if (data.target.find('.fne-title').length) {
            fneWindow.append(data.target.find('.fne-title'));
        }
        else {
            var title = '<h2 class="fne-title" id="fne-title"><span id="title-text">' + data.settings.title + '</span>';
            title += '<span class="nonmodal-actions"><a href="#" class="nonmodal-unblock-action" id="nonmodal-unblock-action"></a></span></h2>';
            fneWindow.append(title);
        }

        fneWindow.append('<div id="nonmodal-content"></div>');

        var nonModalContent = fneWindow.children('#nonmodal-content');
        nonModalContent.append(data.target.not('.fne-title'));
        setTimeout('_resizeNonModal(' + data.settings.widthOverride + ')', 500);
        if ($.isFunction(data.settings.onCompleted)) {
            data.settings.onCompleted();
        }
    };

    _resizeNonModal = function (widthOverride) {
        $("#nonmodal").show();
        var data = nonmodal.data(dataKey),
            windowHeight = $(window).height(),
            windowWidth = $(window).width() - 60,
            newWidth = $("#nonmodal").outerWidth(),
            newHeight = windowHeight - 190,
            left = (windowWidth - $("#nonmodal").outerWidth()) / 2,
            top = $('#main').offset().top + 5 + "px",
            leftWidth = $('#left').length ? $('#left').width() : 0;

        if (data == undefined)
            return;
        if (widthOverride == undefined) widthOverride = '';

        if (data.settings.center) {
            var opts = {
                message: $('#nonmodal')
            };
            $.blockUI(opts);

            newWidth = widthOverride != '' ? widthOverride : newWidth;
            newHeight = $('#nonmodal').outerHeight();
            $('.blockUI.blockMsg').CenterNonModal();
        }
        else if (data.settings.centerFaceplate) {
            newWidth = widthOverride != '' ? widthOverride : newWidth;
            newHeight = data.settings.heightOverride != '' ? data.settings.heightOverride : $(window).height() - 100; //$('#nonmodal').outerHeight() +100;
            data.settings.heightOverride = newHeight;
            top = ($(window).height() - newHeight) / 2;
            left = $(window).width() / 2 - newWidth / 2;
            $("#nonmodal").css({ 'left': left, 'top': top });
            $("#nonmodal").width(newWidth);
            $("#nonmodal").height(newHeight);
            $("#nonmodal").css({ 'position': 'fixed' });
        }
        else {

            left = ($('#left').outerWidth() + $('#left').offset().left + 20) + "px";
            var topOverride = data.settings.topOverride;
            if (topOverride > 0) {
                top = topOverride + "px";
            }else{
                top = $('#left').offset().top + 5 + "px";
            }
            newWidth = widthOverride != '' ? widthOverride : (windowWidth - (leftWidth + 26));

            $("#nonmodal").css({ 'left': left, 'top': top });
            $("#nonmodal").width(newWidth);

            var heightOverride = data.settings.heightOverride; //$("#heightOverride").val();
            if (heightOverride > 0) {
                $("#nonmodal").height(heightOverride);
            }
            else {
                $("#nonmodal").height(newHeight);
            }
        }

        $('#nonmodal-content').height($('#nonmodal').height() - $('#nonmodal #fne-title').height());
        $("#nonmodal #nonmodal-unblock-action").rebind("click", _closeNonModal);
        $("#nonmodal .close-launchpad-modal").rebind("click", _closeNonModal);
        if (data && $.isFunction(data.settings.onResize)) {
            data.settings.onResize();
        }

    };

    _closeNonModal = function (event) {
        event.preventDefault();
        PxPage.TriggerHtmlSave();
        PxPage.CloseNonModal();
    };

    var intrface = {
        init: function (options) {
            _initNonModal();
            var settings = $.extend(true, {}, defaults, options),
                $this = this;
            nonmodal.data(dataKey, {
                target: $this,
                settings: settings
            });
            _setUpNonModal();
            $(window).rebind("resize", _resizeNonModal);
        }
    };

    jQuery.fn.CenterNonModal = function (widthOverride) {
        this.css("position", "absolute");
        this.css("top", ($(window).height() - this.height()) / 2 + $(window).scrollTop() + "px");
        this.css("left", ($(window).width() - $(this).children('#nonmodal').width()) / 2 + $(window).scrollLeft() + "px");
        return this;
    }

    jQuery.fn.PxNonModal = function () {
        return intrface.init.apply(this, arguments);
    };

    jQuery.fn.PxNonModal.Close = function () {
        if (nonmodal && nonmodal.data) {
            var nonModalData = nonmodal.data(dataKey);
            if (nonModalData && nonModalData.settings && nonModalData.settings.onClosed && $.isFunction(nonModalData.settings.onClosed)) {
                nonModalData.settings.onClosed();
            }
        }

        $("#nonmodal").remove();
        if ($('#left').length == 0) {
            $.unblockUI();
        }

    };

    jQuery.fn.PxNonModal.OpenNonModalFromLink = function (args) {
        var title = args.title,
            url = args.url,
            center = args.center ? args.center : false;

        $.get(url, function (response) {
            $(response).PxNonModal({ onCompleted: PxPage.Update, center: center });
        });
    };

} (jQuery))