/**
 * Configures services
 * */
const appServiceBuilder = function () {
    return {
        postbackService: postbackServiceBuilder(),
        crudService: crudServiceBuilder()
    };
}