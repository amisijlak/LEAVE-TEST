/**
 * Manages Templates from the server
 * Last Updated: 2021-10-02
 * Last Update Reason: 
 * --------------------
 * Cater for optional request method
 * Added addTemplateToElement()
 */
var templateManager = (function () {
    var templates = [];
    var queue = [];
    var _BUSY = false;
    var _BUTTONS_OFF = false;
    var BUTTON_SELECTOR = ".tm-add-row,.form-button";

    /**
     * Queues a template to be loaded
     * @param {any} key The template identifier
     * @param {string} url The URL to load the template from
     * @param {Object} parameters A parameter of object to post to server
     * @param {string} method Optional, the request method
     */
    var loadTemplate = function (key, url, parameters, method = "POST") {
        queue.push({ key: key, url: url, parameters: parameters, method: method });

        if (!_BUSY) _processQueue();
    };

    var _processQueueItemCompletion = function (queueItem) {
        _BUSY = false;
        queue = queue.slice(1);
        _processQueue();
    };

    //create busy info window
    var busyInfo = $("<div/>");
    busyInfo.addClass("tm-hide tm-info-window");
    busyInfo.append("<i class='glyphicon glyphicon-repeat fast-right-spinner'></i> ");
    var infoText = $("<span/>");
    busyInfo.append(infoText);
    $("body").append(busyInfo);

    var controlUI = function (disable) {
        if (disable) {
            $(BUTTON_SELECTOR).addClass("tm-hide");//disable buttons
            _BUTTONS_OFF = true;
            busyInfo.removeClass("tm-hide");
        }
        else {
            $(BUTTON_SELECTOR).removeClass("tm-hide");//enable buttons
            _BUTTONS_OFF = false;
            busyInfo.addClass("tm-hide");
        }
    }

    var _processQueue = function () {
        _BUSY = true;

        if (!_BUTTONS_OFF) {
            controlUI(true);
        }

        if (queue.length == 0) {//queue empty
            controlUI(false);
            _BUSY = false;
            return;
        }

        infoText.text("Loading templates... " + queue.length + " left.");

        //get next item in queue
        var queueItem = queue[0];

        $.ajax({
            type: queueItem.method,
            url: queueItem.url,
            data: queueItem.parameters,
            success: function (response) {
                _processQueueItemCompletion(queueItem);
                getTemplate(queueItem.key, true).html = response;
            },
            error: function (jqXHR, textStatus, errorMess) {
                _processQueueItemCompletion(queueItem);
                console.error("Template Manager [" + queueItem.key + "] Request Status: " + textStatus + "\n Details: " + errorMess);
            }
        });
    };

    var getTemplate = function (key, createIfMissing) {
        var found = false;
        var template = null;

        $(templates).each(function (i, d) {//find template
            if (d.key == key) {
                found = true;
                template = d;
                return false;
            }
        });

        if (!found && createIfMissing) {//if not found, create one
            template = { key: key, html: null };
            templates.push(template);
        }

        return template;
    };

    /**
     * Returns the HTML of a given template
     * @param {any} key The key of the template to find
     */
    var getTemplateHtml = function (key) {
        var template = getTemplate(key, false);

        return template == null ? null : template.html;
    };

    /**
     * Add the template to an element.
     * @param {string} key The key of the template to find
     * @param {any} parentElementSelector A JQuery selector for the parent element to append the template to
     */
    const addTemplateToElement = function (key, parentElementSelector) {
        const html = getTemplateHtml(key);

        if (html == null) {
            error("Template Not Found!");
        }
        else {
            const parent = $(parentElementSelector);
            parent.append(html);
            const helperParent = app ?? helper;
            if (helperParent != null) helperParent.controls.formatControls(parent.parent());
        }
    }

    return {
        loadTemplate: loadTemplate,
        getTemplateHtml: getTemplateHtml,
        addTemplateToElement: addTemplateToElement
    };
})();