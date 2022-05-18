
/**
 * Provides support for the DevExpress Data Grid
 * */
var dataGridHelper = function () {
    const instanceHtmlId = "gridContainer";
    const editFormId = "datagrid-edit-form";
    const formRecordId = "datagrid-recordId";

    let selectedRecordIds = [];
    let recordIdFieldName = "Id";

    let deleteCallback = null;
    let deleteURL = null;
    let deleteParameterName = null;
    let deleteReloadFormSelector = null;
    let deleteHttpMethod = null;

    let editCallback = null;
    let editURLSet = false;

    /**
     * Update the recordIds of the checkboxes selected in the grid.
     * @param {Array} selectedItems This is an object passed by the Grid on selection change.
     */
    const handleSelectionChange = function (selectedItems) {
        let updatedRecordIds = [];

        selectedItems.selectedRowsData.forEach(function (item, i) {
            updatedRecordIds.push(item[recordIdFieldName]);
        });

        selectedRecordIds = updatedRecordIds;
    }

    /**
     * Set the preferred record Id field name. Default value is Id.
     * @param {string} fieldName
     */
    const setRecordIdFieldName = function (fieldName) {
        recordIdFieldName = fieldName;
    }

    /**
     * Creates a form that will be posted to when a record's edit button is clicked.
     * @param {URL} editPageURL The URL for the edit page.
     * @param {string} urlParameterName Optional. Defaults to Id.
     */
    const enableDefaultEditFeature = function (editPageURL, urlParameterName = "Id") {
        $("body").append("<form method='get' action='" + editPageURL + "' id='" + editFormId + "'><input type='hidden' name='" + urlParameterName + "' "
            + "id='" + formRecordId + "'/></form>");
        editURLSet = true;
    }

    /**
     * Register the function to call onClick of Edit button.
     * @param {Function} editCallbackFtn The custom edit function.
     */
    const registerCustomEditHandler = function (editCallbackFtn) {
        editCallback = editCallbackFtn;
    }

    const editItem = function (e) {
        var recordId = e.row.data[recordIdFieldName];

        if (editCallback != null) {
            editCallback(recordId);
        }
        else {
            if (!editURLSet) {
                error("Edit URL Not Set and no editCallback() provided!");
                return;
            }
            $("#" + formRecordId).val(recordId);
            $("#" + editFormId).submit();
        }
    }

    /**
     * Enable the default delete feature.
     * @param {string} deletePostbackURL The delete postback URL.
     * @param {string} reloadFormSelector The JQuery selector for the reload form to call on success. Defaults to #reload-form.
     * @param {string} httpMethod The HttpMethod.
     * @param {string} postbackParameterName The parameter name to use when executing recordId postback. Default value is Id.
     */
    const enableDefaultDeleteFeature = function (deletePostbackURL, httpMethod, reloadFormSelector ="#reload-form", postbackParameterName = "Id") {
        deleteURL = deletePostbackURL;
        deleteParameterName = postbackParameterName;
        deleteReloadFormSelector = reloadFormSelector;
        deleteHttpMethod = httpMethod;
    }

    /**
     * Register the function to call onClick of Delete button.
     * @param {Function} deleteCallbackFtn The custom delete function.
     */
    const registerCustomDeleteHandler = function (deleteCallbackFtn) {
        deleteCallback = deleteCallbackFtn;
    }

    const deleteItem = function (e) {
        var recordId = e.row.data[recordIdFieldName];

        if (deleteCallback != null) {
            deleteCallback(recordId);
        }
        else {
            if (deleteURL == null) {
                error("Delete Postback URL Not Set and no deleteCallback() provided!");
                return;
            }
            handleDeleteItem(recordId);
        }
    }

    const handleDeleteItem = function (id) {
        var parameters = {};
        parameters[deleteParameterName] = id;

        app.services.crudService.sendDataToServerAndReloadPage("Do you confirm deleting the selected record?"
            , deleteURL, deleteHttpMethod, parameters, deleteReloadFormSelector);
    }

    /**
     * Returns the datagrid instance.
     * */
    const getInstance = function () {
        return $("#" + instanceHtmlId).dxDataGrid("instance");
    }

    /**
     * Returns the array of record Ids that were selected in the grid.
     * */
    const getSelectedRecordIds = function () {
        return selectedRecordIds;
    }

    const computeRowNumber = function (container, options) {
        var dataGrid = getInstance();
        var rowIndex = (dataGrid.pageIndex() * dataGrid.pageSize()) + options.rowIndex + 1;
        container.text(rowIndex + '.'); 
    }

    return {
        handleSelectionChange: handleSelectionChange,
        getSelectedRecordIds: getSelectedRecordIds,
        setRecordIdFieldName: setRecordIdFieldName,
        getInstance: getInstance,

        computeRowNumber: computeRowNumber,

        enableDefaultEditFeature: enableDefaultEditFeature,
        registerCustomEditHandler: registerCustomEditHandler,
        editItem: editItem,

        enableDefaultDeleteFeature: enableDefaultDeleteFeature,
        registerCustomDeleteHandler: registerCustomDeleteHandler,
        deleteItem: deleteItem
    };
}