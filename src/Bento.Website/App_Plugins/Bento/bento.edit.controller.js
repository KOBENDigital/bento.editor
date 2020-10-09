(function () {
	'use strict';

	function bentoEditController($scope, $injector, contentResource) {

		var vm = this;

		vm.loading = true;
		vm.submit = submit;
		vm.close = close;
		vm.embed = $scope.model.embed;

		//check for Matryoshka
		vm.directiveExists = $injector.has('matryoshkaTabbedContentDirective');

		vm.title = "Edit " + $scope.model.documentTypeName;

		contentResource.getScaffold(-20, $scope.model.documentTypeAlias).then(function (data) {




			// Merge current value
			if ($scope.model.nodeData) {
				for (var v = 0; v < data.variants.length; v++) {
					var variant = data.variants[v];
					for (var t = 0; t < variant.tabs.length; t++) {
						var tab = variant.tabs[t];
						for (var p = 0; p < tab.properties.length; p++) {
							var prop = tab.properties[p];
							if ($scope.model.nodeData[prop.alias]) {
								prop.value = $scope.model.nodeData[prop.alias];
							}
						}
					}
				}
			}

			// Assign the model to scope
			$scope.model.node = data;
			vm.content = data.variants[0];

			vm.loading = false;
		});

		function submit() {
			if ($scope.model.submit) {
				$scope.$broadcast('formSubmitting', { scope: $scope });
				if (vm.bentoEditForm.$valid) {
					$scope.model.submit($scope.model);
				}
			}
		}
		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}
	}

	angular.module('umbraco').controller('bento.edit.controller', bentoEditController);
})();
