function bentoPreviewProcessorFactory($http, $q) {

    var getPreview = function (data) {
        var canceller = $q.defer();

        let url = '/umbraco/backoffice/Api/Bento/LoadPreview';

        var cancel = function (reason) {
            canceller.resolve(reason);
        };

        var promise =
            $http.post(url, data, { timeout: canceller.promise })
                .then(function (response) {
                    return response.data;
                });

        return {
            promise: promise,
            cancel: cancel
        };
    };

    return {
        getPreview: getPreview
    };

}
angular.module('umbraco.resources').factory('bentoPreviewProcessorFactory', bentoPreviewProcessorFactory);