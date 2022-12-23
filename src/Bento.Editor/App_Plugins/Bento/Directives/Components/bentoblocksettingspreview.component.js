(function () {
	"use strict";

	angular
		.module("umbraco")
		.component("bentoBlockSettingsPreview", {
				controller: bentoBlockPreviewController,
				controllerAs: "model",
				bindings: {
					stylesheet: "@",
					styleHtml: "@",
					useBlockSettingsCss: "@"
				}
			}
		);

	function bentoBlockPreviewController($scope, $compile, $element) {
		var model = this;

		model.$onInit = function () {
			var shadowRoot = $element[0].shadowRoot !== null ? $element[0].shadowRoot : $element[0].attachShadow({ mode: 'open' });
			$scope.$watch('model.styleHtml', function (oldVal, newVal) {
				if (newVal !== oldVal) {
					if (model.stylesheet && model.useBlockSettingsCss === 'true') {
						//todo: we're replacing the 'wwwroot' that gets appended in v9/core... not ideal, we're looking into getting the tree picker to start in the wwwroot https://our.umbraco.com/forum/using-umbraco-and-getting-started//108099-is-it-possible-to-start-the-editor-service-file-picker-in-the-wwwroot
						shadowRoot.innerHTML = `<link href="/app_plugins/bento/css/bento.css" rel="stylesheet" type="text/css"><link href="${model.stylesheet.replace('/wwwroot', '')}" rel="stylesheet" type="text/css">${model.styleHtml}`;
						$compile(shadowRoot)($scope);
					}
				}
			});
		};
	}
})();