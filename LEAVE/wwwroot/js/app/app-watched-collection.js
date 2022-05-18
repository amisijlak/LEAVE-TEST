/*
    VERSION 2.1
    With fix to sorting of watched collection names
*/
var watchedCollectionManagerFtn = function () {
    var WATCHED_CONTAINER_CLASS = "wc-watched-container";
    var WATCHED_FIELD_CLASS = "wc-tracked-field";
    var WC_FIELD_ATTRIBUTE = "wc-field";
    var WC_NAME_ATTRIBUTE = "wc-name";
    var WC_NAME_SUFFIX_ATTRIBUTE = "wc-name-suffix";
    var WC_ID_ATTRIBUTE = "wc-id";
    var VALIDATION_MESSAGE_SELECTOR = ".field-validation-valid";
    var WATCHED_COLLECTION_SELECTOR = ".watched-collection";
    var WC_UNSET_CONTAINER = "wc-unset";

    var KNOWN_COLLECTION_NAMES = [];

    var trackUnwatchedFields = function () {
        var selector = null;
        //generate selector
        $(["input", "select", "textarea"]).each(function (i, d) {
            var fieldSelector = WATCHED_COLLECTION_SELECTOR + " " + d + ":not(." + WATCHED_FIELD_CLASS + ")";

            if (selector == null) selector = fieldSelector;
            else selector += "," + fieldSelector;
        });

        //find and track fields
        $(selector).each(function (i, d) {
            var field = $(d);
            //mark with field name
            field.attr(WC_FIELD_ATTRIBUTE, field.attr("name"));
            //track
            field.addClass(WATCHED_FIELD_CLASS);
        });
    };

    var trackUnwatchedCollectionContainers = function () {
        $(WATCHED_COLLECTION_SELECTOR + ":not(." + WATCHED_CONTAINER_CLASS + ")").each(function (i, d) {
            var container = $(d);
            var collectionName = container.attr(WC_NAME_ATTRIBUTE);
            if (!_arrayContains(KNOWN_COLLECTION_NAMES, collectionName)) {
                KNOWN_COLLECTION_NAMES.push(collectionName);
            }

            //identify collection suffix i.e. the actual Collection name minus it's parent's name
            var collectionNameSuffix = collectionName;
            var lastIndexOfPeriod = collectionName.lastIndexOf(".");
            if (lastIndexOfPeriod >= 0) {
                collectionNameSuffix = collectionName.substr(lastIndexOfPeriod + 1);
            }
            container.attr(WC_NAME_SUFFIX_ATTRIBUTE, collectionNameSuffix);

            container.addClass(WATCHED_CONTAINER_CLASS);
        });

    };

    var sortKnowCollectionNames = function () {
        if (KNOWN_COLLECTION_NAMES.length > 0) {
            var tempField = null;
            for (var i = 0; i < KNOWN_COLLECTION_NAMES.length - 1; i++) {
                for (var j = i + 1; j < KNOWN_COLLECTION_NAMES.length; j++) {
                    if (KNOWN_COLLECTION_NAMES[i].localeCompare(KNOWN_COLLECTION_NAMES[j]) > 0) {
                        var tempField = KNOWN_COLLECTION_NAMES[i];
                        KNOWN_COLLECTION_NAMES[i] = KNOWN_COLLECTION_NAMES[j];
                        KNOWN_COLLECTION_NAMES[j] = tempField;
                    }
                }
            }
        }
    };

    var updateContainerIds = function () {
        //mark containers as unset
        $("." + WATCHED_CONTAINER_CLASS).addClass(WC_UNSET_CONTAINER);

        //update unset container collectionIds
        $(KNOWN_COLLECTION_NAMES).each(function (collectionIndex, collectionName) {//for each collection name
            //for each child element of collection name
            $("." + WATCHED_CONTAINER_CLASS + "." + WC_UNSET_CONTAINER + "[" + WC_NAME_ATTRIBUTE + "='" + collectionName + "']").each(function (childIndex, childElement) {
                var container = $(childElement);
                container.attr(WC_ID_ATTRIBUTE, _generateContainerCollectionId(container));
                container.removeClass(WC_UNSET_CONTAINER);//cleanup
            });
        });
    };

    var updateFieldIds = function () {
        $("." + WATCHED_FIELD_CLASS).each(function (i, d) {//for each field
            var field = $(d);
            var collectionId = _generateFieldCollectionId(field);
            field.attr("name", collectionId);
            field.attr("id", collectionId);

            //update validation message
            var nextElement = field.next();
            if (nextElement.hasClass("field-validation-valid")) {
                nextElement.attr("data-valmsg-for", collectionId);
            }
        });
    };

    var _generateContainerCollectionId = function (element) {
        var collectionId = element.attr(WC_NAME_SUFFIX_ATTRIBUTE);
        var closestCollectionParent = element.parent().closest(WATCHED_COLLECTION_SELECTOR);

        var selectorForSimilarCollectionsAlreadySet = "." + WATCHED_CONTAINER_CLASS
            + "[" + WC_NAME_ATTRIBUTE + "='" + element.attr(WC_NAME_ATTRIBUTE) + "']"
            + ":not(." + WC_UNSET_CONTAINER + ")";

        var elementIndex = 0;

        if (closestCollectionParent.length > 0)//if closest watched-collection parent found
        {
            collectionId = closestCollectionParent.attr(WC_ID_ATTRIBUTE) + "." + collectionId;

            elementIndex = closestCollectionParent.find(selectorForSimilarCollectionsAlreadySet).length;
        }
        else//resolve from the root
        {
            elementIndex = $(selectorForSimilarCollectionsAlreadySet).length;
        }

        return collectionId + "[" + elementIndex + "]";
    };

    var _generateFieldCollectionId = function (element) {
        var collectionId = element.attr(WC_FIELD_ATTRIBUTE);
        var closestCollectionParent = element.closest(WATCHED_COLLECTION_SELECTOR);

        if (closestCollectionParent != null)//closest watched-collection found
        {
            collectionId = closestCollectionParent.attr(WC_ID_ATTRIBUTE) + "." + collectionId;
        }

        return collectionId;
    };

    var _arrayContains = function (arrayList, element) {
        var found = false;

        $(arrayList).each(function (i, d) {
            if (d == element) { found = true; return false; }
        });

        return found;
    };

    return {
        update: function () {
            trackUnwatchedFields();
            trackUnwatchedCollectionContainers();
            sortKnowCollectionNames();
            updateContainerIds();
            updateFieldIds();
        }
    };
};