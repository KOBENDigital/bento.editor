(function () {
	"use strict";

	angular
		.module("umbraco")
		.component("bentoBlockPreview", {
				controller: bentoBlockPreviewController,
				controllerAs: "model",
				bindings: {
					stylesheet: "@",
					useCss: "@",
					useBackofficeJs: "@",
					userCode: "@"
				}
			}
		);

	function bentoBlockPreviewController($scope, $compile, $element, $sce) {
		var model = this;
		model.previewHtml = "";
		model.$onInit = function () {
			$scope.$watch('model.previewHtml', function (oldVal, newVal) {
				if (newVal !== oldVal) {
					if ((model.stylesheet && model.useCss === 'true') || model.useBackofficeJs === 'true') {
						var bentoPreview = 'bentoPreview-' + Math.floor(Math.random() * Math.floor(Math.random() * Date.now()));
						$element[0].id = bentoPreview;
						var shadowRoot = $element[0].shadowRoot !== null ? $element[0].shadowRoot : $element[0].attachShadow({ mode: 'open' });
						if (model.stylesheet && model.useCss === 'true') {
							//todo: we're replacing the 'wwwroot' that gets appended in v9/core... not ideal, we're looking into getting the tree picker to start in the wwwroot https://our.umbraco.com/forum/using-umbraco-and-getting-started//108099-is-it-possible-to-start-the-editor-service-file-picker-in-the-wwwroot
							shadowRoot.innerHTML = `<link href="${model.stylesheet.replace('/wwwroot', '')}" rel="stylesheet" type="text/css">${model.previewHtml}`;
						}
						if (model.useBackofficeJs === 'true') {
							var script = document.createElement('script');
							var scriptSource = `var bentoPreview = document.querySelector('#${bentoPreview}').shadowRoot;${model.userCode}`;
							script.textContent = `${scriptSource}`;
							shadowRoot.appendChild(script);
						}
						$compile(shadowRoot)($scope);
					}
					else {
						$element[0].innerHTML = `${model.previewHtml}`;
					}
				}
			});
		};

		var unsubscribe = $scope.$on("bentoSyncPreview", function (ev, args) {
			model.previewHtml = $sce.trustAsHtml(args);
		});

		$scope.$on('$destroy', function () {
			unsubscribe();
		});
	}
})();