var appLogic = function () {
    var DELETE_URL = null;
    var SAVE_URL = null;

    /**
     * Deletes rows with checked checkboxes
     * @param {string} extraNote optional, additional text to show in confirmation message
     */
    function deleteCheckedItems(extraNote) {
        app.controls.processCheckedRecords(function (ids) {
            _deleteItems({ Ids: ids }, extraNote);
        });
    }

    /**
     * Deletes a record using the provided parameter object
     * @param {object} parameter required
     * @param {string} extraNote optional, additional text to show in confirmation message
     */
    function _deleteItems(parameter, extraNote) {
        if (DELETE_URL == null) {
            error("The Delete URL has not been set!");
            return;
        }

        var message = "Do you confirm deleting the selected record(s)?";
        if (extraNote != null) {
            message += "<br/><b>Note:</b><br/>" + extraNote;
        }

        confirmMessage(message, function () {
            app.postToServer(DELETE_URL, parameter, function (response) {
                if (response.success) {
                    $("#reload-form #SuccessMessage").val("Successfully Deleted Record(s)!");
                    $("#reload-form").submit();
                }
                else {
                    error(response.errorMessage);
                }
            });
        });
    }

    /**
     * Deletes a record with the specified id
     * @param {object} id required
     * @param {string} extraNote optional, additional text to show in confirmation message
     */
    function deleteItem(id, extraNote) {
        _deleteItems({ id: id }, extraNote);
    }

    function processResponse(response, formSuffix,preferredFormSelector) {
        if (response.success) {
            var selector = preferredFormSelector == null ? "#reload-form" : preferredFormSelector;

            if (formSuffix != null) selector += "-" + formSuffix;

            $(selector + " #SuccessMessage").val("Successfully Saved Record!");

            $(Object.keys(response)).each(function (i, d) {
                if (d != "success" && d != "errorMessage") {
                    $(selector + " #" + d).val(response[d]);
                }
            });

            $(selector).submit();
        }
        else {
            error(response.errorMessage);
        }
    }

    /**
     * Posts a record for saving
     * @param {object} caller the control that was clicked
     * @param {number} formSuffix optional, a suffix that is used to differentiate the reload form
     */
    function saveData(caller, formSuffix) {
        if (SAVE_URL == null) {
            error("The Save URL has not been set!");
            return;
        }

        var form = $("#Id").closest("form");

        if (!app.controls.isFormValid(form)) {
            alert("Please fill all required fields!");
            return;
        }

        confirmMessage("Do you confirm saving these details?", function () {
            handleServerTransaction(caller, SAVE_URL, form.serialize(), formSuffix);
        });
    };

    /**
     * Post data to server
     * @param {object} caller the control that was clicked
     * @param {string} url url to server response
     * @param {object} parameters parameters to post
     * @param {string} formSuffix (optional)
     * @param {string} preferredFormSelector (optional)
     */
    function handleServerTransaction(caller, url, parameters, formSuffix, preferredFormSelector) {
        if (url == null) {
            error("The Save URL has not been set!");
            return;
        }

        app.postToServer(url, parameters, function (response) {
            processResponse(response, formSuffix, preferredFormSelector);
        }, caller);
    }

    /**
     * Posts a record with attachments for saving
     * @param {any} caller the control that was clicked
     * @param {any} formSuffix optional, a suffix that is used to differentiate the reload form
     */
    function saveDataWithAttachments(caller, formSuffix) {
        if (SAVE_URL == null) {
            error("The Save URL has not been set!");
            return;
        }

        var form = $("#Id").closest("form");

        if (!app.controls.isFormValid(form)) {
            alert("Please fill all required fields!");
            return;
        }

        confirmMessage("Do you confirm saving these details?", function () {
            app.postFormDataToServer(SAVE_URL, form, function (response) {
                processResponse(response, formSuffix);
            }, caller);
        });
    };

    /**
     * Adds an element from the templateManager to the specified destination
     * @param {string} templateKey The key of the template to find
     * @param {object} destinationContainerSelector A JQuery selector to the destination container
     */
    function addElementFromTemplateManager(templateKey, destinationContainerSelector) {
        var html = templateManager.getTemplateHtml(templateKey);

        if (html == null) {
            error("Template Not Found!");
        }
        else {
            $(destinationContainerSelector).append(html);
            app.controls.formatControls();
        }
    };

    return {
        initializeDeleteURL: function (url) { DELETE_URL = url; },
        initializeSaveURL: function (url) { SAVE_URL = url; },
        deleteSelectedRecords: deleteCheckedItems,
        deleteRecordById: deleteItem,
        deleteRecordByParameterObject: _deleteItems,
        saveRecord: saveData,
        saveRecordWithAttachments: saveDataWithAttachments,
        addElementFromTemplateManager: addElementFromTemplateManager,
        handleServerTransaction: handleServerTransaction
    };
};