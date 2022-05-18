/*
    Manage Lookups
*/
var lookupManagerFtn = function () {
    var clearCombo = function (combo) {
        combo.val('');
        combo.find("option:not(:first-of-type)").remove();
        app.controls.updateRefreshComboOptions(combo);
    };

    var populateCombo = function (combo, lookupValues) {
        $(lookupValues).each(function (i, d) {
            combo.append("<option value='" + d.value + "'>" + d.key + "</option>");
        });

        app.controls.updateRefreshComboOptions(combo);
    };

    var trackAndUpdateDependantCombos = function (comboSelector, dependantSelector, parentContainerSelector, serverLookupURL, getParameterCallbackFtn) {
        $("body").on("change", comboSelector, function (event) {
            var combo = $(event.target);
            var dependantCombo = combo.closest(parentContainerSelector).find(dependantSelector);
            clearCombo(dependantCombo);

            app.postToServer(serverLookupURL, getParameterCallbackFtn(combo.val()), function (response) {
                populateCombo(dependantCombo, response);
            });
        });
    };

    var RELATED_SELECTOR = ".related-combo";

    var clearDependentCombosOnChange = function (parentSelector, callbackFtn) {
        var comboSelector = parentSelector + " select" + RELATED_SELECTOR;

        $(comboSelector).change(function (event) {
            var found = false;
            var callerId = $(event.target).attr("id");

            $(comboSelector).each(function (i, d) {
                if (!found) {
                    if ($(d).attr("id") == callerId) {
                        found = true;
                    }
                }
                else {
                    clearCombo($(d));
                }
            });

            if (callbackFtn != null) callbackFtn(event);
        });
    };

    var postbackOnComboChange = function (parentSelector, ignoreValidation) {
        var submitForm = function (event) {
            var form = $(event.target).closest("form");

            if (ignoreValidation) {
                var validator = form.validate();
                validator.destroy();
            }

            form.submit();
        };

        clearDependentCombosOnChange(parentSelector, submitForm);

        var comboSelector = parentSelector + " select:not(" + RELATED_SELECTOR+")";

        $(comboSelector).change(submitForm);
    };

    return {
        trackAndUpdateDependantCombos: trackAndUpdateDependantCombos,
        clearDependentCombosOnChange: clearDependentCombosOnChange,
        postbackOnComboChange: postbackOnComboChange
    };
};