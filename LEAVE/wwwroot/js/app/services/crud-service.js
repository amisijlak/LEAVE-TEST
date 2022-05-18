/**
 * Implement default CRUD services
 * */
const crudServiceBuilder = function () {
    /**
     * Send data to the server and reloads the page.
     * @param {string} confirmationMessage Optional. When specified, asks the user to confirm the action.
     * @param {string} url The postback URL. The server should response with the following object {success:bool, errorMessage:string}
     * @param {string} httpMethod The HttpMethod.
     * @param {object} parameters The postback parameters.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     * @param {any} caller Optional, a reference to the button that was clicked. Preferrably within a form.
     */
    const sendDataToServerAndReloadPage = function (confirmationMessage, url, httpMethod, parameters, reloadFormSelector, caller = null) {
        const logicToExecute = () => _sendDataToServerAndReloadPage(url, httpMethod, parameters, reloadFormSelector, caller);

        if (confirmationMessage == null) {
            logicToExecute();
        }
        else {
            confirmMessage(confirmationMessage, logicToExecute);
        }
    }

    /**
     * Send a form to the server and reloads the page. The form is validated before postback.
     * @param {string} confirmationMessage Optional. When specified, asks the user to confirm the action.
     * @param {string} url The postback URL. The server should response with the following object {success:bool, errorMessage:string}
     * @param {string} httpMethod The HttpMethod.
     * @param {HTMLFormElement} form The html form to postback.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     * @param {any} caller Optional, a reference to the button that was clicked. Preferrably within a form.
     */
    const sendFormToServerAndReloadPage = function (confirmationMessage, url, httpMethod, form, reloadFormSelector, caller = null) {
        if (!app.controls.isFormValid(form)) {
            alert("Please fill all required fields!");
            return;
        }

        const logicToExecute = () => _sendDataToServerAndReloadPage(url, httpMethod, form.serialize(), reloadFormSelector, caller
            ?? $(app.services.postbackService.getSaveButtonSelector()).eq(0));

        if (confirmationMessage == null) {
            logicToExecute();
        }
        else {
            confirmMessage(confirmationMessage, logicToExecute);
        }
    }

    /**
     * Send data to the server and reloads the page.
     * @param {string} confirmationMessage Optional. When specified, asks the user to confirm the action.
     * @param {string} url The postback URL. The server should response with the following object {success:bool, errorMessage:string}
     * @param {string} httpMethod The HttpMethod.
     * @param {HTMLFormElement} form The html form to postback.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     */
    const sendAttachmentsToServerAndReloadPage = function (confirmationMessage, url, httpMethod, form, reloadFormSelector) {
        const logicToExecute = () => _sendAttachmentsToServerAndReloadPage(url, httpMethod, form, reloadFormSelector);

        if (confirmationMessage == null) {
            logicToExecute();
        }
        else {
            confirmMessage(confirmationMessage, logicToExecute);
        }
    }

    /**
     * Send data to the server and reloads the page.
     * @param {string} url The postback URL. The server should response with the following object {success:bool, errorMessage:string}
     * @param {string} httpMethod The HttpMethod.
     * @param {object} parameters The postback parameters.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     * @param {any} caller Optional, a reference to the button that was clicked. Preferrably within a form.
     */
    const _sendDataToServerAndReloadPage = function (url, httpMethod, parameters, reloadFormSelector, caller = null) {
        app.services.postbackService.execute(url, httpMethod, parameters, function (response) {
            _handleServerResponse(response, reloadFormSelector);
        }, caller);
    }

    /**
     * Send data to the server and reloads the page.
     * @param {string} url The postback URL. The server should response with the following object {success:bool, errorMessage:string}
     * @param {string} httpMethod The HttpMethod.
     * @param {HTMLFormElement} form The html form to postback.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     */
    const _sendAttachmentsToServerAndReloadPage = function (url, httpMethod, form, reloadFormSelector) {
        app.services.postbackService.sendFormData(url, httpMethod, form, function (response) {
            _handleServerResponse(response, reloadFormSelector);
        });
    }

    /**
     * Handle the response from the server.
     * @param {object} response The response from the server.
     * @param {string} reloadFormSelector The JQuery selector for the reload form.
     */
    const _handleServerResponse = function (response, reloadFormSelector) {
        if (response == null || response == "" || response.success || response.Success) {
            $(reloadFormSelector).submit();
        }
        else {
            error(response.errorMessage);
        }
    }

    return {
        sendDataToServerAndReloadPage: sendDataToServerAndReloadPage,
        sendFormToServerAndReloadPage: sendFormToServerAndReloadPage,
        sendAttachmentsToServerAndReloadPage: sendAttachmentsToServerAndReloadPage
    };
}