(function () {
	'use strict';

	function bentoEditorController($scope,$sce, assetsService) {

		var vm = this;

		vm.remove = remove;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.canAddContent = canAddContent;
		vm.canAddMultiple = canAddMultiple;
		vm.updating = false;
		vm.deleteConfirmVisible = false;

		vm.item = {
			name: "...",
			icon: "icon-brick",
			contents: []
		};
		//vm.item.id = undefined;
		//vm.item.key = undefined;
		//vm.item.contentData = undefined;
		//vm.contentNode = undefined;
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
			} else {
				// some old version of Bento?
				data.id = $scope.model.value;
				data.key = undefined;
				data.contentData = undefined;
				data.icon = undefined;
			}
///

			//vm.item.id = data.id;
			//vm.item.key = data.key;
			//vm.item.contentData = data.contentData;
			vm.item.icon = data.icon;

			if (data.contents !== undefined) {
				vm.item.contents = data.contents;
			} else {
				if (data.contentData || data.id || data.key) { // if any of these things were set by $scope.model.value (should be unset by next save)
					data.contents.push({ id: data.id, name: data.name, alias: data.alias, key: data.key, contentData: data.contentData, icon: data.icon, contentNode: data.contentNode });
				}
			}
		}

		$scope.$watch('vm.updating', function (newValue, oldValue) {
			if (canAddMultiple()) {
				//save multiple
				if (vm.item.contents.length < $scope.model.config.minNumber || vm.item.contents.length > $scope.model.config.maxNumber) {
					// validation warning
				} else {
					$scope.model.value = JSON.stringify(vm.item);
                }
			} else {
				//save singular
				var data = vm.item.contents[0];
				if (data !== undefined && (data.id !== undefined || data.key !== undefined)) {
					data.icon = vm.item.icon;
					$scope.model.value = JSON.stringify(data);
				}
			}
		});

		function toggleDeleteConfirm(show) {
			vm.deleteConfirmVisible = show;
		}

		function canAddContent() {
			if (canAddMultiple()) {
				//allow multiple entries (depending on configuration)
				return vm.item.contents.length < $scope.model.config.maxNumber;
			}
			return vm.item.contents.length === 0; // allow single entry
		}

		//Use to check that this is a Multi Items by checking for config.minNumber/config.maxNumber 
		function canAddMultiple() {
			return ($scope.model.config.minNumber !== undefined && $scope.model.config.maxNumber !== undefined);
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