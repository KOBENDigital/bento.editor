(function () {
	'use strict';

	function bentoGridController($scope, editorService, bentoResource) {

		$scope.page = {};
		$scope.page.loading = true;

		var vm = this;

		vm.init = init;
		vm.value = undefined;
		vm.editorLabel = null;
		vm.config = null;
		vm.itemUpdating = false;
		
		function init() {
			if ($scope.control.value) {
				vm.value = $scope.control.value;
			}
			vm.config = $scope.control.editor.config;
		}

		$scope.$watch('vm.itemUpdating', function (newValue, oldValue) {

				$scope.control.value = vm.value;

		});
		

		init();
	}

	angular.module('umbraco').controller('bento.grid.editor.controller', bentoGridController);
})();