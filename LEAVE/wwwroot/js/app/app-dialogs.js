/* Dialog Helper */
var appDialogFtn = function () {

    var alertMessage = function (message) {
        showMessage("Info", message, "fa-info-circle", "text-info", "Ok", null);
    };

    var errorMessage = function (message) {
        showMessage("Error!", message, "fa-exclamation-triangle", "text-danger", "Ok", null);
    };

    var confirmMessage = function (message, confirmBtnCallBack, cancelBtnCallBack) {
        showMessage("Confirm", message, "fa-question-circle", "text-warning", "Confirm"
            , confirmBtnCallBack == null ? function () { } : confirmBtnCallBack, "Cancel"
            , cancelBtnCallBack == null ? function () { } : cancelBtnCallBack);
    };

    var _closeModal = function (modal, callbackFtn) {
        modal.find(".modal-body .dialog-message").empty();
        modal.modal("hide");
        modal.unbind("hidden.bs.modal");
        modal.on("hidden.bs.modal", function () {
            if (callbackFtn != null) callbackFtn();
        });
    };

    var showMessage = function (title, message, iconClass, iconColor, okText, okBtnCallBack, cancelText, cancelBtnCallBack) {
        var modal = $("#systemDialog");
        //format icon
        modal.find("#dialog-icon").attr("class", "fas " + iconClass + " " + iconColor);

        //format title
        modal.find(".modal-title").text(title);

        //format buttons
        var okButton = modal.find(".modal-footer .btn-success");
        var cancelButton = modal.find(".modal-footer .btn-danger");

        okButton.unbind("click");
        if (okText == null) {
            okButton.hide();
        }
        else {
            okButton.show();
            okButton.text(okText);
            okButton.click(function () {
                _closeModal(modal, okBtnCallBack);
            });
        }

        cancelButton.unbind("click");
        if (cancelText == null) {
            cancelButton.hide();
        }
        else {
            cancelButton.show();
            cancelButton.text(cancelText);
            cancelButton.click(function () {
                _closeModal(modal, cancelBtnCallBack);
            });
        }

        //load body
        var body = modal.find(".modal-body .dialog-message");
        body.empty();
        body.append(message);

        //show
        modal.modal({ show: true, backdrop: 'static' });
    };

    return {
        initialize: function () {
            window.alert = alertMessage;
            window.error = errorMessage;
            window.confirmMessage = confirmMessage;
        },
        confirmMessage: confirmMessage
    };
};