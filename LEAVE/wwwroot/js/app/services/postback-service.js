/**
 * Provides server postback functionality
 * */
const postbackServiceBuilder = function () {
    /**
     * Postback to Server WITH the Busy Div showing
     * @param {String} URL URL to postback
     * @param {Object} parameters Parameters to postback to Server
     * @param {Function} callbackFtn A function to callback when the response is received
     * @param {object} caller Optional, the control that was clicked. Preferrably within a form.
     * @param {Function} preferredErrorHandler If not NULL, a function to call incase an error occurs
     */
    const postToServer = function (URL, parameters, callbackFtn, caller = null, preferredErrorHandler = errorHandler) {
        execute(URL, "POST", parameters, callbackFtn, caller, preferredErrorHandler);
    };

    /**
     * Post a form with attachments to the server. The form MUST have an html id.
     * @param {Object} form The form to submit
     * @param {string} URL The postback URL
     * @param {Function} successCallbackFtn A function to callback when the response is received
     */
    const postFormDataToServer = function (URL, form, successCallbackFtn) {
        sendFormData(URL, "POST", form, successCallbackFtn);
    };

    /**
     * Postback to Server with the Busy Div showing, in the background
     * @param {string} URL URL to postback
     * @param {string} httpMethod The HttpMethod
     * @param {object} parameters Parameters to postback to Server
     * @param {Function} successCallbackFtn A function to callback when the response is received
     * @param {object} caller Optional, the control that was clicked. Preferrably within a form.
     * @param {Function} preferredErrorHandler If not NULL, a function to call incase an error occurs
     */
    const execute = function (URL, httpMethod, parameters, successCallbackFtn, caller = null, preferredErrorHandler = errorHandler) {

        if (!_isURLValid(URL)) return;

        let form = null;
        if (caller != null) {
            form = $(caller).closest("form");
            if (form.length == 0) form = $(caller).parent();
        }

        disableButtons(form, false);

        $.ajax({
            type: httpMethod,
            url: URL,
            data: parameters,
            success: function (response) {
                enableButtons(form);
                successCallbackFtn(response);
            },
            error: function (jqXHR, textStatus, errorMess) {
                enableButtons(form);
                preferredErrorHandler(jqXHR, textStatus, errorMess);
            }
        });
    };

    const _isURLValid = function (URL) {
        if (URL == null || URL.length == 0) {
            error("Postback URL Not Specified!");
            return false;
        }

        return true;
    }

    /**
     * Post a form with attachments to the server. The form MUST have an html id.
     * @param {HTMLFormElement} form The form to submit
     * @param {string} URL The postback URL
     * @param {string} httpMethod The HttpMethod
     * @param {Function} successCallbackFtn A function to callback when the response is received
     */
    const sendFormData = function (URL, httpMethod, form, successCallbackFtn) {
        if (!_isURLValid(URL)) return;

        var formData = new FormData(document.getElementById(form.attr("id")));

        ajax = new XMLHttpRequest();

        ajax.upload.addEventListener("progress", progressHandler, false);

        ajax.addEventListener("error", function (jqXHR, textStatus, errorMess) {
            enableButtons(form);
            errorHandler(jqXHR, textStatus, errorMess);
        });

        ajax.addEventListener("load", function (e) {
            updateProgressBar(100);
            enableButtons(form);
            if (e.currentTarget.status == 200) {
                successCallbackFtn(ajax.responseText == null || ajax.responseText.length == 0 ? null : processServerResponse(ajax.responseText));
            }
            else errorHandler({ statusText: e.currentTarget.statusText, responseText: e.currentTarget.response });
        }, false);

        disableButtons(form, true);

        ajax.open(httpMethod, URL);
        ajax.send(formData);
    };

    const processServerResponse = function (response) {
        try {
            return eval("(" + ajax.responseText + ")");
        }
        catch (e) { }

        return response;
    };

    function progressHandler(event) {
        var percent = Math.round((event.loaded / event.total) * 100);
        updateProgressBar(percent); //from bootstrap bar class
    };

    function updateProgressBar(value) {
        var container = $("form");
        var progressBar = container.find(".progress");
        progressBar.find(".progress-bar").css("width", value + "%");
        container.find(".progress-update").text(value + "%");
    };

    /**
     * Error handler for ajax calls
     * @param {any} jqXHR
     * @param {any} textStatus
     * @param {any} errorMess
     */
    const errorHandler = function (jqXHR, textStatus, errorMess) {
        $(".busy-container").hide();
        error("<div><b>" + jqXHR.statusText + "</b><br/><br/><i>" + jqXHR.responseText + "</i></div>");
    };

    const BUTTON_SELECTOR = ".save-button";
    const DISABLED_CLASS = "save-button-disabled";

    const getSaveButtonSelector = () => BUTTON_SELECTOR;

    const disableButtons = function (form, addProgress) {
        if (form == null) return;

        //disable buttons
        var buttons = form.find(BUTTON_SELECTOR + ":not(." + DISABLED_CLASS + ")");
        buttons.attr("disabled", "disabled");
        if (addProgress) buttons.after('<div class="progress" style="margin-left:5px;width:100px;display:inline-block">'
            + '<div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"'
            + 'style="width: 0%">'
            + '<span class="progress-update"></span>'
            + '</div>'
            + '</div>');
        buttons.after('<span class="fas fa-spinner fa-spin" style="margin-left:5px;margin-right:5px"></span>');
        buttons.addClass(DISABLED_CLASS);
    };

    const enableButtons = function (form) {
        if (form == null) return;

        //enable buttons
        var buttons = form.find(BUTTON_SELECTOR + "." + DISABLED_CLASS);
        buttons.removeAttr("disabled");
        form.find(".fa-spinner,.progress").remove();
        buttons.removeClass(DISABLED_CLASS);
    }

    return {
        getSaveButtonSelector: getSaveButtonSelector,

        postToServer: postToServer,
        postFormDataToServer: postFormDataToServer,

        execute: execute,
        sendFormData: sendFormData
    };
}