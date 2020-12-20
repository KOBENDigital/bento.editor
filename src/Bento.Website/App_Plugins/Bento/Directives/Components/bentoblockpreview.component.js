(function () {
    "use strict";

    angular
        .module("umbraco")
        .component("bentoBlockPreview", {
            controller: BentoBlockPreviewController,
            controllerAs: "model",
            bindings: {
                stylesheet: "@",
                previewHtml: "@",
                useCss: "@"
            }
        }
        );

    function BentoBlockPreviewController($scope, $compile, $element) {
        var model = this;


        model.$onInit = function () {

            var shadowRoot = $element[0].attachShadow({ mode: 'open' });

            $scope.$watch('model.previewHtml', function (oldVal, newVal) {

                if (newVal !== oldVal) {

                    if (model.stylesheet && model.useCss === 'true') {

                        shadowRoot.innerHTML = `
                            <link href="${model.stylesheet}" rel="stylesheet" type="text/css">
                            ${model.previewHtml}
                        `;

                    }
                    else {
                        shadowRoot.innerHTML = `
                            ${model.previewHtml}
                        `;
                    }

                    $compile(shadowRoot)($scope);
                }

            });
        };

    }


})();