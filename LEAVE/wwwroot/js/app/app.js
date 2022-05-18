// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var app = (function () {
    const services = appServiceBuilder();
    const components = appComponentBuilder();

    const initialize = function () {
        app.controls.initialize();
        app.dialogs.initialize();
    };

    return {
        postToServer: services.postbackService.postToServer,
        postFormDataToServer: services.postbackService.postFormDataToServer,

        controls: appControlsFtn(),
        dialogs: appDialogFtn(),
        lookups: lookupManagerFtn(),
        logic: appLogic(),

        services: services,
        components: components,

        initialize: initialize
    };
})();
