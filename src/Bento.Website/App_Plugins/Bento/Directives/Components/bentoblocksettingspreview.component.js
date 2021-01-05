(function () {
    "use strict";

    angular
        .module("umbraco")
        .component("bentoBlockSettingsPreview", {
            controller: BentoBlockPreviewController,
            controllerAs: "model",
            bindings: {
                stylesheet: "@",
                styleHtml: "@",
                useBlockSettingsCss: "@"
            }
        }
        );

    function BentoBlockPreviewController($scope, $compile, $element) {
        var model = this;


        model.$onInit = function () {

            var shadowRoot = $element[0].shadowRoot !== null ? $element[0].shadowRoot : $element[0].attachShadow({ mode: 'open' });


            $scope.$watch('model.styleHtml', function (oldVal, newVal) {

                if (newVal !== oldVal) {

                    if (model.stylesheet && model.useBlockSettingsCss === 'true') {

                        shadowRoot.innerHTML = `
                            <link href="/app_plugins/bento/css/bento.css" rel="stylesheet" type="text/css">
                            <link href="${model.stylesheet}" rel="stylesheet" type="text/css">
                            ${model.styleHtml}
                        `;
                        $compile(shadowRoot)($scope);
                    } 
                }

            });
        };

    }


})();