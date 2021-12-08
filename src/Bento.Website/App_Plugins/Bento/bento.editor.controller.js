(function () {
	'use strict';

	function bentoEditorController($scope,$sce, assetsService) {

		var vm = this;

		vm.remove = remove;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.updating = false;
		vm.deleteConfirmVisible = false;

		vm.item = {};
		vm.item.id = undefined;
		vm.item.key = undefined;
		vm.item.contentData = undefined;
		vm.contentNode = undefined;
		vm.config = $scope.model.config;


		if ($scope.model.config.useCssFile && $scope.model.config.fontCssUrls) {
			vm.cssFonts = $scope.model.config.fontCssUrls.split('*');
		}

		vm.trustSrc = function (src) {
			return $sce.trustAsResourceUrl(src);
		};
		

		vm.name = "...";
		vm.item.icon = "icon-brick";

		$scope.model.hideLabel = $scope.model.config.hideLabel;

		if ($scope.model.value) {
/// old bento hacks
			var data = {};

			if ($scope.model.value.id !== undefined) {
				data = $scope.model.value;
			} else{
				data.id = $scope.model.value;
				data.key = undefined;
				data.contentData = undefined;
				data.icon = undefined;
			}
///

			vm.item.id = data.id;
			vm.item.key = data.key;
			vm.item.contentData = data.contentData;
			vm.item.icon = data.icon;
		}

		$scope.$watch('vm.updating', function (newValue, oldValue) {
			if (vm.item.id !== undefined || vm.item.key !== undefined) {
				$scope.model.value = JSON.stringify(vm.item);
			}
		});

		function toggleDeleteConfirm(show) {
			vm.deleteConfirmVisible = show;
		}

		function remove() {
			vm.item = {};
			vm.item.id = undefined;
			vm.item.key = undefined;
			vm.item.contentData = undefined;
			vm.contentNode = undefined;

			vm.name = "...";
			vm.item.icon = "icon-brick";
			$scope.model.value = undefined;

			vm.deleteConfirmVisible = false;
		}
	}

	angular.module('umbraco').controller('bento.editor.controller', bentoEditorController);
})();