angular.module('umbraco.services').config([
    '$httpProvider',
    function ($httpProvider) {

        $httpProvider.interceptors.push(function ($q) {
            return {
                'request': function (request) {

                    if (request.url.includes("views/content/apps/content/content.html")) {
                        var queryString = request.url.split("?");
                        var qs = "";
                        if (queryString[1]) {
                            qs = "?" + queryString[1];
                        }
                        request.url = "/App_Plugins/Cultiv.Tabify/tabify.html" + qs;
                    }

                    return request || $q.when(request);
                }
            };
        });

    }]);