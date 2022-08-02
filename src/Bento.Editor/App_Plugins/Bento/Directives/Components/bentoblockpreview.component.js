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

					var bentoPreview = 'bentoPreview-' + Math.floor(Math.random() * Math.floor(Math.random() * Date.now()));
					$element[0].id = bentoPreview;
					var shadowRoot = $element[0].shadowRoot !== null ? $element[0].shadowRoot : $element[0].attachShadow({ mode: 'open' });

					var content = document.createElement('div');
					content.innerHTML = `${model.previewHtml}`;
					shadowRoot.replaceChildren(content);

					if (model.useBackofficeJs === 'true') {
						var script = shadowRoot.querySelectorAll('script');
						if (script.length) {
							for (let i = 0; i < script.length; i++) {
								
								var clonedScript = document.createElement('script');

								var type = script[i].getAttribute('type');
								if (type) {
									clonedScript.setAttribute('type', type);
								}

								clonedScript.textContent = script[i].innerHTML;
								shadowRoot.appendChild(clonedScript);
							}
						}
					}

					if ((model.stylesheet && model.useCss === 'true') || model.useBackofficeJs === 'true') {

						if (model.stylesheet && model.useCss === 'true') {
							//todo: we're replacing the 'wwwroot' that gets appended in v9/core... not ideal, we're looking into getting the tree picker to start in the wwwroot https://our.umbraco.com/forum/using-umbraco-and-getting-started//108099-is-it-possible-to-start-the-editor-service-file-picker-in-the-wwwroot
							var style = document.createElement('link');
							style.setAttribute('href', model.stylesheet.replace('/wwwroot', ''));
							style.setAttribute('rel', 'stylesheet');
							style.setAttribute('type', 'text/css');
							shadowRoot.appendChild(style);
						}

						if (model.useBackofficeJs === 'true') {
							var script = document.createElement('script');
							var scriptSource = `var bentoPreview = document.querySelector('#${bentoPreview}').shadowRoot;${model.userCode}`;
							script.textContent = `${scriptSource}`;
							shadowRoot.appendChild(script);
						}
					}

					$compile(shadowRoot)($scope);
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