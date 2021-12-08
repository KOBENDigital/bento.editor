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
                useCss: "@",
                useBackofficeJs: "@",
                userCode: "@",
            }
        }
        );


    function BentoBlockPreviewController($scope, $compile, $element) {
        var model = this;


        model.$onInit = function () {


            $scope.$watch('model.previewHtml', function (oldVal, newVal) {

                if (newVal !== oldVal) {

                    if (model.stylesheet && model.useCss === 'true') {

                        var bentoPreview = 'bentoPreview-' + Math.floor(Math.random() * Math.floor(Math.random() * Date.now()));
                        $element[0].id = bentoPreview;
                        var shadowRoot = $element[0].shadowRoot !== null ? $element[0].shadowRoot : $element[0].attachShadow({ mode: 'open' });


                        shadowRoot.innerHTML = `
                            <link href="${model.stylesheet}" rel="stylesheet" type="text/css">
                            ${model.previewHtml}
                            `;
                            if (model.useBackofficeJs === 'true') {
                                var script = document.createElement('script');
                                var scriptSource = `
                                var bentoPreview = document.querySelector('#${bentoPreview}').shadowRoot;
                                ${model.userCode}
                            `;
                            script.textContent = `${scriptSource}`;
                            shadowRoot.appendChild(script);
                        }

                        $compile(shadowRoot)($scope);
                    }
                    else {
                        $element[0].innerHTML = `
                            ${model.previewHtml}
                        `;
                    }


                }

            });
        };

    }


})();